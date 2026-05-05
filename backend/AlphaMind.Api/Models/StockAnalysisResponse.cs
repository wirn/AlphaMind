namespace AlphaMind.Api.Models;

public class StockAnalysisResponse
{
    public int Id { get; set; }

    public int StockId { get; set; }

    public string Ticker { get; set; } = string.Empty;

    public string CompanyName { get; set; } = string.Empty;

    public int ImpactScore { get; set; }

    public int ConfidenceScore { get; set; }

    public int ExpectedMove { get; set; }

    public string Direction { get; set; } = string.Empty;

    public string Summary { get; set; } = string.Empty;

    public string[] Opportunities { get; set; } = [];

    public string[] Risks { get; set; } = [];

    public int UsedNewsCount { get; set; }

    public DateTime AnalyzedAt { get; set; }

    public DateTime CreatedAt { get; set; }
}

