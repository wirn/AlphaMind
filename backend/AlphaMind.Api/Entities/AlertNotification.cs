using System.ComponentModel.DataAnnotations;

namespace AlphaMind.Api.Entities;

public class AlertNotification
{
    public int Id { get; set; }

    public int StockAnalysisId { get; set; }

    public StockAnalysis StockAnalysis { get; set; } = null!;

    public int StockId { get; set; }

    [Required]
    [MaxLength(30)]
    public string Ticker { get; set; } = string.Empty;

    [Required]
    [MaxLength(320)]
    public string RecipientEmail { get; set; } = string.Empty;

    public int ImpactScore { get; set; }

    [Required]
    [MaxLength(20)]
    public string Direction { get; set; } = string.Empty;

    [Required]
    [MaxLength(30)]
    public string Status { get; set; } = string.Empty;

    [MaxLength(4000)]
    public string? ErrorMessage { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public DateTime? SentAtUtc { get; set; }
}
