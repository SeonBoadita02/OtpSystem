using Amazon.DynamoDBv2;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.SimpleEmailV2;
using Shared;
using System.Text.Json;
using System.Threading.Tasks;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace SaveOrResendOtpLambda;

public class Function
{
    private readonly DynamoDbHelper _ddb;
    private readonly SesEmailer _emailer;

    public Function()
    {
        var ddb = new AmazonDynamoDBClient();
        var ses = new AmazonSimpleEmailServiceV2Client();
        _ddb = new DynamoDbHelper(ddb);
        _emailer = new SesEmailer(ses);
    }

    public async Task<APIGatewayProxyResponse> FunctionHandler(SaveOrResendRequest payload, ILambdaContext context)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(payload.Email))
                return BadRequest("Email is required.");

            payload.MobileNumber ??= string.Empty;

            await _ddb.InitUserIfMissingAsync(payload.Email, payload.MobileNumber);

            var user = await _ddb.GetUserByEmailAsync(payload.Email);
            if (user is not null)
            {
                var id = long.Parse(user["Id"].N); // Id is a Number
                try
                {
                    await _ddb.UpdateUserOnResendAsync(id, DynamoDbHelper.NowEpoch(), Config.MaxResends);
                }
                catch (Amazon.DynamoDBv2.Model.ConditionalCheckFailedException)
                {
                    // Either validated already or resend limit reached
                    var isValidated = user.ContainsKey("IsValidated") && user["IsValidated"].BOOL.GetValueOrDefault();
                    var resendCount = user.ContainsKey("ResendCount") ? int.Parse(user["ResendCount"].N) : 0;

                    if (isValidated)
                        return Conflict("User already validated.");
                    if (resendCount >= Config.MaxResends)
                        return TooManyRequests("Resend limit reached. Try again later or contact support.");
                }
            }

            // Generate + hash OTP
            var otp = OtpHelper.GenerateNumericCode(Config.OtpLength);
            var hash = OtpHelper.Sha256Hex(otp);
            var expiry = DynamoDbHelper.NowEpoch() + (Config.OtpExpiryMinutes * 60);

            // Put/overwrite OTP item
            await _ddb.PutOtpAsync(payload.Email, hash, expiry);

            // Email it (SES)
            await _emailer.SendOtpAsync(payload.Email, otp);

            return Ok(new { message = "OTP sent" });
        }
        catch (Exception ex)
        {
            context.Logger.LogError(ex.ToString());
            return ServerError("Failed to process request.");
        }
    }

    private static APIGatewayProxyResponse Ok(object body) => Response(200, body);
    private static APIGatewayProxyResponse BadRequest(string msg) => Response(400, new { error = msg });
    private static APIGatewayProxyResponse Conflict(string msg) => Response(409, new { error = msg });
    private static APIGatewayProxyResponse TooManyRequests(string msg) => Response(429, new { error = msg });
    private static APIGatewayProxyResponse ServerError(string msg) => Response(500, new { error = msg });

    private static APIGatewayProxyResponse Response(int status, object body) => new()
    {
        StatusCode = status,
        Body = JsonSerializer.Serialize(body),
        Headers = new Dictionary<string, string> { ["Content-Type"] = "application/json" }
    };
}