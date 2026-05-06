using System.Text.Json;
using AlphaMind.Api.Data;
using AlphaMind.Api.Entities;
using AlphaMind.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace AlphaMind.Api.Services;

public class StockAnalysisRunService(
    AlphaMindDbContext dbContext,
    StockAnalysisPreviewService previewService)
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public async Task<StockAnalysisResponse> RunAsync(
        string ticker,
        CancellationToken cancellationToken = default)
    {
        var preview = await previewService.AnalyzeAsync(ticker, cancellationToken);
        var stock = await dbContext.Stocks
            .FirstOrDefaultAsync(stock => stock.Ticker == preview.Ticker, cancellationToken);

        if (stock is null)
        {
            throw new InvalidOperationException($"Stock '{preview.Ticker}' was not found.");
        }

        return await SaveAsync(stock, preview, cancellationToken);
    }

    public async Task<StockAnalysisResponse> RunForStockAsync(
        Stock stock,
        CancellationToken cancellationToken = default)
    {
        var preview = await previewService.AnalyzeAsync(stock, cancellationToken);

        return await SaveAsync(stock, preview, cancellationToken);
    }

    private async Task<StockAnalysisResponse> SaveAsync(
        Stock stock,
        StockAnalysisPreviewResult preview,
        CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var analysis = new StockAnalysis
        {
            StockId = stock.Id,
            ImpactScore = preview.ImpactScore,
            ConfidenceScore = preview.ConfidenceScore,
            ExpectedMove = preview.ExpectedMove,
            Direction = preview.Direction,
            Summary = preview.Summary,
            OpportunitiesJson = JsonSerializer.Serialize(preview.Opportunities, JsonOptions),
            RisksJson = JsonSerializer.Serialize(preview.Risks, JsonOptions),
            UsedNewsCount = preview.UsedNewsCount,
            AnalyzedAt = preview.AnalyzedAt,
            CreatedAt = now
        };

        dbContext.StockAnalyses.Add(analysis);
        await dbContext.SaveChangesAsync(cancellationToken);

        return ToResponse(analysis, stock);
    }

    private static StockAnalysisResponse ToResponse(StockAnalysis analysis, Stock stock)
    {
        return new StockAnalysisResponse
        {
            Id = analysis.Id,
            StockId = analysis.StockId,
            Ticker = stock.Ticker,
            CompanyName = stock.Name,
            ImpactScore = analysis.ImpactScore,
            ConfidenceScore = analysis.ConfidenceScore,
            ExpectedMove = analysis.ExpectedMove,
            Direction = analysis.Direction,
            Summary = analysis.Summary,
            Opportunities = DeserializeStringArray(analysis.OpportunitiesJson),
            Risks = DeserializeStringArray(analysis.RisksJson),
            UsedNewsCount = analysis.UsedNewsCount,
            AnalyzedAt = analysis.AnalyzedAt,
            CreatedAt = analysis.CreatedAt
        };
    }

    private static string[] DeserializeStringArray(string json)
    {
        return JsonSerializer.Deserialize<string[]>(json, JsonOptions) ?? [];
    }
}
