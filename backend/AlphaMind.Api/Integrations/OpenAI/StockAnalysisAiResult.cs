namespace AlphaMind.Api.Integrations.OpenAI;

public class StockAnalysisAiResult
{
    public string Direction { get; set; } = string.Empty;

    public int ImpactScore { get; set; }

    public int ConfidenceScore { get; set; }

    public int ExpectedMove { get; set; }

    public string Summary { get; set; } = string.Empty;

    public string[] Opportunities { get; set; } = [];

    public string[] Risks { get; set; } = [];
}
