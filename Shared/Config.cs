namespace Shared;

public static class Config
{
    public static readonly string Region = Environment.GetEnvironmentVariable("AWS_REGION") ?? "ap-south-1";
    public static readonly string UserAccessTable = Environment.GetEnvironmentVariable("USER_ACCESS_TABLE") ?? "DATA.UserAccess";
    public static readonly string SenderEmail = Environment.GetEnvironmentVariable("SENDER_EMAIL") ?? "iitintern002@gmail.com";

    public static readonly int OtpExpiryMinutes = int.TryParse(Environment.GetEnvironmentVariable("OTP_EXPIRY_MINUTES"), out var m) ? m : 10;
    public static readonly int OtpLength = int.TryParse(Environment.GetEnvironmentVariable("OTP_LENGTH"), out var l) ? l : 4;
    public static readonly int MaxResends = int.TryParse(Environment.GetEnvironmentVariable("MAX_RESENDS"), out var r) ? r : 5;
    public static readonly int MaxAttempts = int.TryParse(Environment.GetEnvironmentVariable("MAX_ATTEMPTS"), out var a) ? a : 5;

    // Auto-increment starting point
    private static long _nextId = 1;
    public static long GetNextId() => Interlocked.Increment(ref _nextId);
}