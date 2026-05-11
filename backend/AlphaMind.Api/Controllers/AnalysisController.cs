using AlphaMind.Api.Data;
using AlphaMind.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AlphaMind.Api.Controllers;

[ApiController]
[Route("api/analysis")]
public class AnalysisController(AlphaMindDbContext dbContext) : ControllerBase
{
    private const int DefaultLimit = 100;
    private const int MaxLimit = 500;

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<AnalysisListItemResponse>>> GetAnalyses(
        [FromQuery] int? limit,
        [FromQuery] int? stockId,
        CancellationToken cancellationToken)
    {
        if (limit is <= 0)
        {
            return BadRequest("Limit must be greater than 0.");
        }

        if (stockId is <= 0)
        {
            return BadRequest("StockId must be greater than 0.");
        }

        var take = Math.Min(limit ?? DefaultLimit, MaxLimit);

        var query = dbContext.StockAnalyses
            .AsNoTracking()
            .Include(analysis => analysis.Stock)
            .AsQueryable();

        if (stockId.HasValue)
        {
            query = query.Where(analysis => analysis.StockId == stockId.Value);
        }

        var analyses = await query
            .OrderByDescending(analysis => analysis.AnalyzedAt)
            .Take(take)
            .Select(analysis => new AnalysisListItemResponse(
                analysis.Id,
                analysis.StockId,
                analysis.Stock.Ticker,
                analysis.Stock.Name,
                analysis.ImpactScore,
                analysis.ConfidenceScore,
                analysis.ExpectedMove,
                analysis.Direction,
                analysis.Summary,
                analysis.OpportunitiesJson,
                analysis.RisksJson,
                analysis.UsedNewsCount,
                analysis.AnalyzedAt,
                analysis.CreatedAt))
            .ToListAsync(cancellationToken);

        return Ok(analyses);
    }
}
