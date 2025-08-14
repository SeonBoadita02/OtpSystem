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
                var id = long.Parse(user["Id"].N);
                try
                {
                    await _ddb.UpdateUserOnResendAsync(id, DynamoDbHelper.NowIso8601());
                }
                catch (Amazon.DynamoDBv2.Model.ConditionalCheckFailedException)
                {
                    var isValidated = user.ContainsKey("IsValidated") && user["IsValidated"].BOOL.GetValueOrDefault();
                    var verificationAttempt = user.ContainsKey("VerificationAttempt") ? int.Parse(user["VerificationAttempt"].N) : 0;

                    if (isValidated)
                        return Conflict("User already validated.");
                    if (verificationAttempt >= Config.MaxAttempts)
                        return TooManyRequests("Verification attempt limit reached. Try again later or contact support.");
                }
            }

            // Generate + hash OTP
            var otp = OtpHelper.GenerateNumericCode(Config.OtpLength);
            var hash = OtpHelper.Sha256Hex(otp);
            var expiry = DynamoDbHelper.AddMinutesToIso8601(DynamoDbHelper.NowIso8601(), Config.OtpExpiryMinutes);

            // Update OTP directly in DATA.UserAccess
            await _ddb.UpdateOtpAsync(payload.Email, hash, expiry);

            // Email it (SES)
            await _emailer.SendOtpAsync(payload.Email, otp);

            return Ok(new
            {
                message = "OTP sent",
                createDate = user?["CreateDate"].S,
                updateDate = user?["UpdateDate"].S,
                verificationCodeExpiry = expiry
            });
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