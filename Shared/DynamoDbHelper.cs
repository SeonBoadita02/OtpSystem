using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Shared;

public class DynamoDbHelper
{
    private readonly IAmazonDynamoDB _ddb;
    public DynamoDbHelper(IAmazonDynamoDB ddb) => _ddb = ddb;

    public static long NowEpoch() => DateTimeOffset.UtcNow.ToUnixTimeSeconds();

    // USER ACCESS
    public async Task<Dictionary<string, AttributeValue>?> GetUserByEmailAsync(string email)
    {
        var resp = await _ddb.QueryAsync(new QueryRequest
        {
            TableName = Config.UserAccessTable,
            IndexName = "EmailIndex",
            KeyConditionExpression = "Email = :v_email",
            ExpressionAttributeValues = new Dictionary<string, AttributeValue>
            {
                { ":v_email", new AttributeValue { S = email } }
            }
        });
        return resp.Items.Count > 0 ? resp.Items[0] : null;
    }

    public async Task<Dictionary<string, AttributeValue>?> GetUserAsync(string id)
    {
        var resp = await _ddb.GetItemAsync(new GetItemRequest
        {
            TableName = Config.UserAccessTable,
            Key = new() { ["Id"] = new AttributeValue { N = id } } // Id is a Number
        });
        return resp.Item.Count == 0 ? null : resp.Item;
    }

    public async Task PutUserAsync(UserAccess u)
    {
        var item = new Dictionary<string, AttributeValue>
        {
            ["Id"] = new AttributeValue { N = u.Id.ToString() }, // Id is a Number
            ["Email"] = new AttributeValue { S = u.Email },
            ["MobileNumber"] = new AttributeValue { S = u.MobileNumber },
            ["VerificationAttempt"] = new AttributeValue { N = u.VerificationAttempt.ToString() },
            ["ResendCount"] = new AttributeValue { N = u.ResendCount.ToString() },
            ["IsValidated"] = new AttributeValue { BOOL = u.IsValidated },
            ["CreateDate"] = new AttributeValue { N = u.CreateDate.ToString() },
            ["UpdateDate"] = new AttributeValue { N = u.UpdateDate.ToString() },
            ["IsLocked"] = new AttributeValue { BOOL = u.IsLocked }
        };
        await _ddb.PutItemAsync(new PutItemRequest
        {
            TableName = Config.UserAccessTable,
            Item = item,
            ConditionExpression = "attribute_not_exists(Id)" // Ensure no overwrite on first save
        });
    }

    public async Task UpdateUserOnResendAsync(long id, long updateDate, int maxResends)
    {
        await _ddb.UpdateItemAsync(new UpdateItemRequest
        {
            TableName = Config.UserAccessTable,
            Key = new() { ["Id"] = new AttributeValue { N = id.ToString() } }, // Id is a Number
            UpdateExpression = "SET ResendCount = ResendCount + :one, UpdateDate = :ud",
            ConditionExpression = "ResendCount < :max AND IsValidated = :false",
            ExpressionAttributeValues = new()
            {
                [":one"] = new AttributeValue { N = "1" },
                [":ud"] = new AttributeValue { N = updateDate.ToString() },
                [":max"] = new AttributeValue { N = maxResends.ToString() },
                [":false"] = new AttributeValue { BOOL = false }
            }
        });
    }

    public async Task InitUserIfMissingAsync(string email, string mobile)
    {
        var user = await GetUserByEmailAsync(email);
        if (user is null)
        {
            var now = NowEpoch();
            await PutUserAsync(new UserAccess(
                Id: DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                Email: email,
                MobileNumber: mobile,
                VerificationAttempt: 0,
                ResendCount: 0,
                IsValidated: false,
                CreateDate: now,
                UpdateDate: now,
                IsLocked: false
            ));
        }
    }

    public async Task IncrementAttemptAsync(long id, int maxAttempts)
    {
        await _ddb.UpdateItemAsync(new UpdateItemRequest
        {
            TableName = Config.UserAccessTable,
            Key = new() { ["Id"] = new AttributeValue { N = id.ToString() } }, // Id is a Number
            UpdateExpression = "SET VerificationAttempt = VerificationAttempt + :one, IsLocked = if_not_exists(IsLocked, :false) OR (VerificationAttempt + :one >= :max), UpdateDate = :ud",
            ExpressionAttributeValues = new()
            {
                [":one"] = new AttributeValue { N = "1" },
                [":max"] = new AttributeValue { N = maxAttempts.ToString() },
                [":false"] = new AttributeValue { BOOL = false },
                [":ud"] = new AttributeValue { N = NowEpoch().ToString() }
            }
        });
    }

    public async Task MarkVerifiedAsync(long id)
    {
        await _ddb.UpdateItemAsync(new UpdateItemRequest
        {
            TableName = Config.UserAccessTable,
            Key = new() { ["Id"] = new AttributeValue { N = id.ToString() } }, // Id is a Number
            UpdateExpression = "SET IsValidated = :true, ResendCount = :zero, UpdateDate = :ud",
            ExpressionAttributeValues = new()
            {
                [":true"] = new AttributeValue { BOOL = true },
                [":zero"] = new AttributeValue { N = "0" },
                [":ud"] = new AttributeValue { N = NowEpoch().ToString() }
            }
        });
    }

    // OTP
    public async Task PutOtpAsync(string email, string otpHash, long expiry)
    {
        var item = new Dictionary<string, AttributeValue>
        {
            ["Email"] = new AttributeValue { S = email },
            ["VerificationCodeHash"] = new AttributeValue { S = otpHash },
            ["VerificationCodeExpiry"] = new AttributeValue { N = expiry.ToString() },
            ["ttl"] = new AttributeValue { N = expiry.ToString() }
        };
        await _ddb.PutItemAsync(new PutItemRequest
        {
            TableName = Config.OtpTable,
            Item = item
        });
    }

    public async Task<Dictionary<string, AttributeValue>?> GetOtpAsync(string email)
    {
        var resp = await _ddb.GetItemAsync(new GetItemRequest
        {
            TableName = Config.OtpTable,
            Key = new() { ["Email"] = new AttributeValue { S = email } },
            ConsistentRead = true
        });
        return resp.Item.Count == 0 ? null : resp.Item;
    }

    public async Task DeleteOtpAsync(string email)
    {
        await _ddb.DeleteItemAsync(new DeleteItemRequest
        {
            TableName = Config.OtpTable,
            Key = new() { ["Email"] = new AttributeValue { S = email } }
        });
    }
}