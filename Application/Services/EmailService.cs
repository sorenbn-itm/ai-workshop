namespace CleanCQRSPOC.Application.Services;

public class EmailService(ILogger<EmailService> logger) : IEmailService
{
    private readonly ILogger<EmailService> _logger = logger;

    public void SendEmail(string recipient, string subject, string body)
    {
        _logger.LogInformation("Sending email to {Recipient}: {Subject} - {Body}", recipient, subject, body);
    }
}
