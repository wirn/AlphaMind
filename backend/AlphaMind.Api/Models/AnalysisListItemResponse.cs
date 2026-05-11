namespace AlphaMind.Api.Models;

public record AnalysisListItemResponse(
    int Id,
    int StockId,
    string Ticker,
    string StockName,
    int ImpactScore,
    int ConfidenceScore,
    int ExpectedMove,
    string Direction,
    string Summary,
    string OpportunitiesJson,
    string RisksJson,
    int UsedNewsCount,
    DateTime AnalyzedAt,
    DateTime CreatedAt);
