using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace Shared;

public class DynamoDbHelper
{
    private readonly IAmazonDynamoDB _ddb;

    public DynamoDbHelper(IAmazonDynamoDB ddb) => _ddb = ddb;

    public static string NowIso8601() => DateTime.UtcNow.ToString("o");

    public static string AddMinutesToIso8601(string isoDate, int minutes)
    {
        var dateTime = DateTime.Parse(isoDate, null, DateTimeStyles.RoundtripKind);
        return dateTime.AddMinutes(minutes).ToString("o");
    }

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
            Key = new() { ["Id"] = new AttributeValue { N = id } }
        });
        return resp.Item.Count == 0 ? null : resp.Item;
    }

    public async Task PutUserAsync(UserAccess u)
    {
        var item = new Dictionary<string, AttributeValue>
        {
            ["Id"] = new AttributeValue { N = u.Id.ToString("D4") }, // Auto-increment with leading zeros
            ["Email"] = new AttributeValue { S = u.Email },
            ["MobileNumber"] = new AttributeValue { S = u.MobileNumber },
            ["VerificationAttempt"] = new AttributeValue { N = u.VerificationAttempt.ToString() },
            ["IsValidated"] = new AttributeValue { BOOL = u.IsValidated },
            ["CreateDate"] = new AttributeValue { S = u.CreateDate },
            ["UpdateDate"] = new AttributeValue { S = u.UpdateDate },
            ["VerificationCodeHash"] = new AttributeValue { S = u.VerificationCodeHash },
            ["VerificationCodeExpiry"] = new AttributeValue { S = u.VerificationCodeExpiry }
        };
        await _ddb.PutItemAsync(new PutItemRequest
        {
            TableName = Config.UserAccessTable,
            Item = item,
            ConditionExpression = "attribute_not_exists(Id)"
        });
    }

    public async Task UpdateUserOnResendAsync(long id, string updateDate)
    {
        await _ddb.UpdateItemAsync(new UpdateItemRequest
        {
            TableName = Config.UserAccessTable,
            Key = new() { ["Id"] = new AttributeValue { N = id.ToString() } },
            UpdateExpression = "SET UpdateDate = :ud",
            ConditionExpression = "IsValidated = :false",
            ExpressionAttributeValues = new()
            {
                [":ud"] = new AttributeValue { S = updateDate },
                [":false"] = new AttributeValue { BOOL = false }
            }
        });
    }

    public async Task InitUserIfMissingAsync(string email, string mobile)
    {
        var user = await GetUserByEmailAsync(email);
        if (user is null)
        {
            var now = NowIso8601();
            await PutUserAsync(new UserAccess(
                Id: Config.GetNextId(),
                Email: email,
                MobileNumber: mobile,
                VerificationAttempt: 0,
                IsValidated: false,
                CreateDate: now,
                UpdateDate: now,
                VerificationCodeHash: string.Empty,
                VerificationCodeExpiry: now
            ));
        }
    }

    public async Task IncrementAttemptAsync(long id, int maxAttempts)
    {
        await _ddb.UpdateItemAsync(new UpdateItemRequest
        {
            TableName = Config.UserAccessTable,
            Key = new() { ["Id"] = new AttributeValue { N = id.ToString() } },
            UpdateExpression = "SET VerificationAttempt = VerificationAttempt + :one, UpdateDate = :ud",
            ExpressionAttributeValues = new()
            {
                [":one"] = new AttributeValue { N = "1" },
                [":ud"] = new AttributeValue { S = NowIso8601() }
            }
        });
    }

    public async Task MarkVerifiedAsync(long id)
    {
        await _ddb.UpdateItemAsync(new UpdateItemRequest
        {
            TableName = Config.UserAccessTable,
            Key = new() { ["Id"] = new AttributeValue { N = id.ToString() } },
            UpdateExpression = "SET IsValidated = :true, VerificationAttempt = :zero, UpdateDate = :ud",
            ExpressionAttributeValues = new()
            {
                [":true"] = new AttributeValue { BOOL = true },
                [":zero"] = new AttributeValue { N = "0" },
                [":ud"] = new AttributeValue { S = NowIso8601() }
            }
        });
    }

    public async Task UpdateOtpAsync(string email, string otpHash, string expiry)
    {
        // Retrieve user by email to get the Id
        var user = await GetUserByEmailAsync(email);
        if (user == null)
            throw new Exception("User not found.");

        var id = long.Parse(user["Id"].N);

        // Update the item using the Id as the primary key
        await _ddb.UpdateItemAsync(new UpdateItemRequest
        {
            TableName = Config.UserAccessTable,
            Key = new() { ["Id"] = new AttributeValue { N = id.ToString() } },
            UpdateExpression = "SET VerificationCodeHash = :hash, VerificationCodeExpiry = :exp",
            ExpressionAttributeValues = new()
            {
                [":hash"] = new AttributeValue { S = otpHash },
                [":exp"] = new AttributeValue { S = expiry }
            }
        });
    }
}