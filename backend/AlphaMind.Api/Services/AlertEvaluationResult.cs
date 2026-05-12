namespace AlphaMind.Api.Services;

public record AlertEvaluationResult(
    bool AlertsEnabled,
    int MinImpactScore,
    int AnalysesMatched,
    int NotificationsCreated,
    int NotificationsSent,
    int NotificationsFailed,
    int SkippedDuplicates);
