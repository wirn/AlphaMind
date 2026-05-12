namespace AlphaMind.Api.Services;

public class AlertSettings
{
    public const string SectionName = "AlertSettings";

    public bool Enabled { get; set; }

    public int MinImpactScore { get; set; } = 80;

    public string[] Recipients { get; set; } = [];

    public bool SendEmails { get; set; }

    public string SenderName { get; set; } = string.Empty;

    public string SenderEmail { get; set; } = string.Empty;
}
