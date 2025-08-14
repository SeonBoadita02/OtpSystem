namespace Shared;

public record UserAccess(
    long Id,
    string Email,
    string MobileNumber,
    int VerificationAttempt,
    bool IsValidated,
    string CreateDate,
    string UpdateDate,
    string VerificationCodeHash,
    string VerificationCodeExpiry
);