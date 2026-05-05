namespace AlphaMind.Api.Models;

public class StockAnalysisPreviewResult
{
    public string Ticker { get; set; } = string.Empty;

    public string CompanyName { get; set; } = string.Empty;

    public string Direction { get; set; } = string.Empty;

    public int ImpactScore { get; set; }

    public int ConfidenceScore { get; set; }

    public int ExpectedMove { get; set; }

    public string Summary { get; set; } = string.Empty;

    public string[] Opportunities { get; set; } = [];

    public string[] Risks { get; set; } = [];

    public int UsedNewsCount { get; set; }

    public DateTime AnalyzedAt { get; set; }
}
