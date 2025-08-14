using Amazon.SimpleEmailV2;
using Amazon.SimpleEmailV2.Model;
using System.Threading.Tasks;

namespace Shared;

public class SesEmailer
{
    private readonly IAmazonSimpleEmailServiceV2 _ses;
    public SesEmailer(IAmazonSimpleEmailServiceV2 ses) => _ses = ses;

    public async Task SendOtpAsync(string toEmail, string otp)
    {
        var subject = "Your Verification Code [No Reply]";
        var textBody = $"Your OTP is: {otp}\nIt will expire in {Config.OtpExpiryMinutes} minutes.\nDo not reply to this email.";
        var htmlBody = $"<p>Your OTP is: <strong>{otp}</strong></p><p>It will expire in {Config.OtpExpiryMinutes} minutes.</p><p><em>Do not reply to this email.</em></p>";

        var req = new SendEmailRequest
        {
            FromEmailAddress = Config.SenderEmail,
            Destination = new Destination { ToAddresses = new() { toEmail } },
            Content = new EmailContent
            {
                Simple = new Message
                {
                    Subject = new Content { Data = subject },
                    Body = new Body
                    {
                        Text = new Content { Data = textBody },
                        Html = new Content { Data = htmlBody }
                    }
                }
            }
            // ConfigurationSetName removed since it was causing the error
        };
        await _ses.SendEmailAsync(req);
    }
}