namespace AlphaMind.Api.Services;

public class LogOnlyEmailSender(
    ILogger<LogOnlyEmailSender> logger) : IEmailSender
{
    public Task SendAsync(
        string recipientEmail,
        string subject,
        string body,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation(
            "LogOnly email sender would send alert email to {RecipientEmail}. Subject: {Subject}. Body: {Body}",
            recipientEmail,
            subject,
            body);

        return Task.CompletedTask;
    }
}
