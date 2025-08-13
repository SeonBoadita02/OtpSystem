using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Shared;
using System.Text.Json;
using System.Threading.Tasks;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace VerifyOtpLambda;

public class Function
{
    private readonly DynamoDbHelper _ddb;

    public Function()
    {
        _ddb = new DynamoDbHelper(new AmazonDynamoDBClient());
    }

    public async Task<APIGatewayProxyResponse> FunctionHandler(VerifyOtpRequest payload, ILambdaContext context)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(payload.Email) || string.IsNullOrWhiteSpace(payload.Otp))
                return BadRequest("Email and Otp are required.");

            var user = await _ddb.GetUserByEmailAsync(payload.Email);
            if (user is null)
                return NotFound("User not found.");

            var id = long.Parse(user["Id"].N);

            var isLocked = user.ContainsKey("IsLocked") && user["IsLocked"].BOOL.GetValueOrDefault();
            if (isLocked)
                return Locked("Account locked due to too many failed attempts.");

            // Get OTP item
            var otpItem = await _ddb.GetOtpAsync(payload.Email);
            if (otpItem is null)
            {
                await _ddb.IncrementAttemptAsync(id, Config.MaxAttempts);
                return BadRequest("OTP not found or expired.");
            }

            var now = DynamoDbHelper.NowEpoch();
            var expiry = long.Parse(otpItem["VerificationCodeExpiry"].N);
            if (now > expiry)
            {
                // Expired: count attempt and fail
                await _ddb.IncrementAttemptAsync(id, Config.MaxAttempts);
                // Optional: delete stale OTP (TTL will clean it up anyway)
                try { await _ddb.DeleteOtpAsync(payload.Email); } catch { /* ignore */ }
                return BadRequest("OTP expired.");
            }

            var providedHash = OtpHelper.Sha256Hex(payload.Otp);
            var storedHash = otpItem["VerificationCodeHash"].S;

            if (!string.Equals(providedHash, storedHash, StringComparison.Ordinal))
            {
                await _ddb.IncrementAttemptAsync(id, Config.MaxAttempts);
                return BadRequest("Invalid OTP.");
            }

            // Success: mark verified & reset resend count to 0
            await _ddb.MarkVerifiedAsync(id);

            // Remove OTP (not strictly required because TTL, but immediate cleanup is nice)
            try { await _ddb.DeleteOtpAsync(payload.Email); } catch { /* ignore */ }

            return Ok(new { verified = true, message = "Verification successful." });
        }
        catch (Exception ex)
        {
            context.Logger.LogError(ex.ToString());
            return ServerError("Failed to process verification.");
        }
    }

    private static APIGatewayProxyResponse Ok(object body) => Response(200, body);
    private static APIGatewayProxyResponse BadRequest(string msg) => Response(400, new { verified = false, error = msg });
    private static APIGatewayProxyResponse NotFound(string msg) => Response(404, new { verified = false, error = msg });
    private static APIGatewayProxyResponse Locked(string msg) => Response(423, new { verified = false, error = msg }); // 423 Locked
    private static APIGatewayProxyResponse ServerError(string msg) => Response(500, new { verified = false, error = msg });

    private static APIGatewayProxyResponse Response(int status, object body) => new()
    {
        StatusCode = status,
        Body = JsonSerializer.Serialize(body),
                Headers = new Dictionary<string, string>() { ["Content-Type"] = "application/json" }

    };
}
