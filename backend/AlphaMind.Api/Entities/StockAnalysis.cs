using System.ComponentModel.DataAnnotations;

namespace AlphaMind.Api.Entities;

public class StockAnalysis
{
    public int Id { get; set; }

    public int StockId { get; set; }

    public Stock Stock { get; set; } = null!;

    public int ImpactScore { get; set; }

    public int ConfidenceScore { get; set; }

    public int ExpectedMove { get; set; }

    [Required]
    [MaxLength(20)]
    public string Direction { get; set; } = string.Empty;

    [Required]
    [MaxLength(4000)]
    public string Summary { get; set; } = string.Empty;

    [Required]
    [MaxLength(8000)]
    public string OpportunitiesJson { get; set; } = string.Empty;

    [Required]
    [MaxLength(8000)]
    public string RisksJson { get; set; } = string.Empty;

    public int UsedNewsCount { get; set; }

    public DateTime AnalyzedAt { get; set; }

    public DateTime CreatedAt { get; set; }
}

