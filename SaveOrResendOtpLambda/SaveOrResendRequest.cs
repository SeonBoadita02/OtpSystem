namespace SaveOrResendOtpLambda;

public class SaveOrResendRequest
{
    public string Email { get; set; } = default!;
    public string MobileNumber { get; set; } = default!; // required on first save
}