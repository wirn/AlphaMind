namespace AlphaMind.Api.Integrations.OpenAI;

public interface IOpenAiClient
{
    Task<StockAnalysisAiResult> CreateStockAnalysisAsync(
        string prompt,
        CancellationToken cancellationToken = default);
}

