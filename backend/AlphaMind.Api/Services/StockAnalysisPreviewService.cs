using System.Globalization;
using System.Text;
using AlphaMind.Api.Data;
using AlphaMind.Api.Entities;
using AlphaMind.Api.Integrations.OpenAI;
using AlphaMind.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace AlphaMind.Api.Services;

public class StockAnalysisPreviewService(
    AlphaMindDbContext dbContext,
    IOpenAiClient openAiClient)
{
    public async Task<StockAnalysisPreviewResult> AnalyzeAsync(
        string ticker,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(ticker))
        {
            throw new ArgumentException("Ticker is required.", nameof(ticker));
        }

        var normalizedTicker = ticker.Trim();
        var stock = await dbContext.Stocks
            .FirstOrDefaultAsync(
                stock => stock.Ticker == normalizedTicker,
                cancellationToken);

        if (stock is null)
        {
            throw new InvalidOperationException($"Stock '{normalizedTicker}' was not found.");
        }

        return await AnalyzeAsync(stock, cancellationToken);
    }

    public async Task<StockAnalysisPreviewResult> AnalyzeAsync(
        Stock stock,
        CancellationToken cancellationToken = default)
    {
        var newsItems = await dbContext.StockNews
            .Where(news => news.StockId == stock.Id)
            .OrderByDescending(news => news.PublishedAt)
            .Take(10)
            .ToListAsync(cancellationToken);

        if (newsItems.Count == 0)
        {
            throw new InvalidOperationException($"No news found for stock '{stock.Ticker}'.");
        }

        var aiResult = await openAiClient.CreateStockAnalysisAsync(
            BuildPrompt(stock.Ticker, stock.Name, newsItems),
            cancellationToken);

        return new StockAnalysisPreviewResult
        {
            Ticker = stock.Ticker,
            CompanyName = stock.Name,
            Direction = aiResult.Direction,
            ImpactScore = aiResult.ImpactScore,
            ConfidenceScore = aiResult.ConfidenceScore,
            ExpectedMove = aiResult.ExpectedMove,
            Summary = aiResult.Summary,
            Opportunities = aiResult.Opportunities,
            Risks = aiResult.Risks,
            UsedNewsCount = newsItems.Count,
            AnalyzedAt = DateTime.UtcNow
        };
    }

    private static string BuildPrompt(string ticker, string stockName, IReadOnlyList<StockNews> newsItems)
    {
        var builder = new StringBuilder();
        builder.AppendLine("Analyze the recent stock news context below for a stock analysis preview.");
        builder.AppendLine("Use only the provided news context.");
        builder.AppendLine("Do not invent facts, events, numbers, guidance, or market context.");
        builder.AppendLine("ImpactScore is 1-100 and measures only the strength or importance of potential stock impact, regardless of positive or negative direction.");
        builder.AppendLine("ConfidenceScore is 1-100 and measures how reliable the assessment is based only on the supplied context.");
        builder.AppendLine("ExpectedMove is an integer from -50 to 50. Negative means expected negative stock impact, 0 means neutral or unclear, and positive means expected positive stock impact. It is not a percentage prediction.");
        builder.AppendLine("Direction must be positive, negative, neutral, or mixed.");
        builder.AppendLine("If the provided context is weak, stale, repetitive, or insufficient, say so in the summary, use lower ConfidenceScore, and keep ExpectedMove near 0 unless the provided news clearly supports direction.");
        builder.AppendLine("Return strict JSON only. Do not include markdown, comments, or explanatory text outside JSON.");
        builder.AppendLine("The JSON field summary must be written in Swedish.");
        builder.AppendLine("The JSON arrays opportunities and risks must also be written in Swedish.");
        builder.AppendLine("Keep the JSON property names in English.");
        builder.AppendLine(CultureInfo.InvariantCulture, $"Ticker: {ticker}");
        builder.AppendLine(CultureInfo.InvariantCulture, $"Company: {stockName}");
        builder.AppendLine("Latest news:");

        for (var index = 0; index < newsItems.Count; index++)
        {
            var news = newsItems[index];
            builder.AppendLine(CultureInfo.InvariantCulture, $"{index + 1}. PublishedAt: {news.PublishedAt:O}");
            builder.AppendLine(CultureInfo.InvariantCulture, $"Headline: {news.Headline}");
            builder.AppendLine(CultureInfo.InvariantCulture, $"Summary: {news.Summary ?? "(no summary provided)"}");
        }

        builder.AppendLine("JSON shape:");
        builder.AppendLine("""
{
  "direction": "positive | negative | neutral | mixed",
  "impactScore": 1,
  "confidenceScore": 1,
  "expectedMove": 0,
  "summary": "string",
  "opportunities": ["string"],
  "risks": ["string"]
}
""");

        return builder.ToString();
    }
}
