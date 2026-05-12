using AlphaMind.Api.Data;
using AlphaMind.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace AlphaMind.Api.Services;

public class StockAlertService(
    AlphaMindDbContext dbContext,
    IEmailSender emailSender,
    IOptions<AlertSettings> options,
    ILogger<StockAlertService> logger)
{
    public async Task<AlertEvaluationResult> RunAsync(CancellationToken cancellationToken = default)
    {
        var settings = options.Value;
        var minImpactScore = settings.MinImpactScore > 0 ? settings.MinImpactScore : 80;

        logger.LogInformation(
            "Alert run started. Enabled={Enabled}. MinImpactScore={MinImpactScore}.",
            settings.Enabled,
            minImpactScore);

        if (!settings.Enabled)
        {
            logger.LogInformation("Alert run skipped because AlertSettings:Enabled is false.");

            return new AlertEvaluationResult(
                AlertsEnabled: false,
                MinImpactScore: minImpactScore,
                AnalysesMatched: 0,
                NotificationsCreated: 0,
                NotificationsSent: 0,
                NotificationsFailed: 0,
                SkippedDuplicates: 0);
        }

        if (settings.SendEmails)
        {
            logger.LogInformation(
                "AlertSettings:SendEmails is true, but only LogOnlyEmailSender is registered. Alerts will be logged only.");
        }

        var recipients = settings.Recipients
            .Where(recipient => !string.IsNullOrWhiteSpace(recipient))
            .Select(recipient => recipient.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (recipients.Count == 0)
        {
            logger.LogInformation("Alert run skipped because no recipients are configured.");

            return new AlertEvaluationResult(
                AlertsEnabled: true,
                MinImpactScore: minImpactScore,
                AnalysesMatched: 0,
                NotificationsCreated: 0,
                NotificationsSent: 0,
                NotificationsFailed: 0,
                SkippedDuplicates: 0);
        }

        var analyses = await dbContext.StockAnalyses
            .Include(analysis => analysis.Stock)
            .Where(analysis => analysis.ImpactScore >= minImpactScore)
            .OrderByDescending(analysis => analysis.AnalyzedAt)
            .ToListAsync(cancellationToken);

        var analysisIds = analyses.Select(analysis => analysis.Id).ToList();
        var existingNotifications = await dbContext.AlertNotifications
            .Where(notification => analysisIds.Contains(notification.StockAnalysisId))
            .Select(notification => new
            {
                notification.StockAnalysisId,
                notification.RecipientEmail
            })
            .ToListAsync(cancellationToken);

        var existingKeys = existingNotifications
            .Select(notification => CreateNotificationKey(
                notification.StockAnalysisId,
                notification.RecipientEmail))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var notificationsCreated = 0;
        var notificationsSent = 0;
        var notificationsFailed = 0;
        var skippedDuplicates = 0;

        foreach (var analysis in analyses)
        {
            foreach (var recipient in recipients)
            {
                var notificationKey = CreateNotificationKey(analysis.Id, recipient);
                if (!existingKeys.Add(notificationKey))
                {
                    skippedDuplicates++;
                    logger.LogInformation(
                        "Duplicate alert skipped for StockAnalysisId={StockAnalysisId}, RecipientEmail={RecipientEmail}.",
                        analysis.Id,
                        recipient);
                    continue;
                }

                var notification = new AlertNotification
                {
                    StockAnalysisId = analysis.Id,
                    StockId = analysis.StockId,
                    Ticker = analysis.Stock.Ticker,
                    RecipientEmail = recipient,
                    ImpactScore = analysis.ImpactScore,
                    Direction = analysis.Direction,
                    Status = "Pending",
                    CreatedAtUtc = DateTime.UtcNow
                };

                dbContext.AlertNotifications.Add(notification);
                notificationsCreated++;

                try
                {
                    await emailSender.SendAsync(
                        recipient,
                        BuildSubject(analysis),
                        BuildBody(analysis, settings),
                        cancellationToken);

                    notification.Status = "Sent";
                    notification.SentAtUtc = DateTime.UtcNow;
                    notificationsSent++;

                    logger.LogInformation(
                        "Alert email logged/sent for StockAnalysisId={StockAnalysisId}, RecipientEmail={RecipientEmail}.",
                        analysis.Id,
                        recipient);
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (Exception exception)
                {
                    notification.Status = "Failed";
                    notification.ErrorMessage = TrimToMaxLength(exception.Message, 4000);
                    notificationsFailed++;

                    logger.LogError(
                        exception,
                        "Alert email failed for StockAnalysisId={StockAnalysisId}, RecipientEmail={RecipientEmail}.",
                        analysis.Id,
                        recipient);
                }
            }
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Alert run completed. AnalysesMatched={AnalysesMatched}. NotificationsCreated={NotificationsCreated}. NotificationsSent={NotificationsSent}. NotificationsFailed={NotificationsFailed}. SkippedDuplicates={SkippedDuplicates}.",
            analyses.Count,
            notificationsCreated,
            notificationsSent,
            notificationsFailed,
            skippedDuplicates);

        return new AlertEvaluationResult(
            AlertsEnabled: true,
            MinImpactScore: minImpactScore,
            AnalysesMatched: analyses.Count,
            NotificationsCreated: notificationsCreated,
            NotificationsSent: notificationsSent,
            NotificationsFailed: notificationsFailed,
            SkippedDuplicates: skippedDuplicates);
    }

    private static string CreateNotificationKey(int stockAnalysisId, string recipientEmail)
    {
        return $"{stockAnalysisId}\u001F{recipientEmail}";
    }

    private static string BuildSubject(StockAnalysis analysis)
    {
        return $"AlphaMind alert: {analysis.Stock.Ticker} impact score {analysis.ImpactScore}";
    }

    private static string BuildBody(StockAnalysis analysis, AlertSettings settings)
    {
        var senderName = string.IsNullOrWhiteSpace(settings.SenderName)
            ? "AlphaMind"
            : settings.SenderName.Trim();
        var senderEmail = string.IsNullOrWhiteSpace(settings.SenderEmail)
            ? "alerts@alphamind.local"
            : settings.SenderEmail.Trim();

        return string.Join(
            Environment.NewLine,
            $"{senderName} detected a high-impact stock analysis.",
            string.Empty,
            $"Ticker: {analysis.Stock.Ticker}",
            $"Company: {analysis.Stock.Name}",
            $"Impact score: {analysis.ImpactScore}",
            $"Confidence score: {analysis.ConfidenceScore}",
            $"Expected move: {analysis.ExpectedMove}",
            $"Direction: {analysis.Direction}",
            $"Analyzed at UTC: {analysis.AnalyzedAt:u}",
            string.Empty,
            analysis.Summary,
            string.Empty,
            $"Sender: {senderName} <{senderEmail}>");
    }

    private static string TrimToMaxLength(string value, int maxLength)
    {
        return value.Length <= maxLength ? value : value[..maxLength];
    }
}
