namespace Shared;

public record UserAccess(
    long Id,
    string Email,
    string MobileNumber,
    int VerificationAttempt,
    int ResendCount,
    bool IsValidated,
    long CreateDate,
    long UpdateDate,
    bool IsLocked = false
);

public record OtpItem(
    string Email,
    string VerificationCodeHash,
    long VerificationCodeExpiry,
    long ttl
);