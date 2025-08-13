namespace VerifyOtpLambda;

public class VerifyOtpRequest
{
    public string Email { get; set; } = default!;
    public string Otp { get; set; } = default!;
}
