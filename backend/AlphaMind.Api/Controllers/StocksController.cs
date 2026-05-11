using AlphaMind.Api.Data;
using AlphaMind.Api.Entities;
using AlphaMind.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AlphaMind.Api.Controllers;

[ApiController]
[Route("api/stocks")]
public class StocksController(AlphaMindDbContext dbContext) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<StockResponse>>> GetStocks(CancellationToken cancellationToken)
    {
        var stocks = await OrderedStocksQuery()
            .Select(stock => new StockResponse(
                stock.Id,
                stock.Ticker,
                stock.Name,
                stock.Exchange,
                stock.IsActive,
                stock.SortOrder,
                stock.CreatedAt))
            .ToListAsync(cancellationToken);

        return Ok(stocks);
    }

    [HttpPost]
    public async Task<ActionResult<StockResponse>> CreateStock(
        CreateStockRequest request,
        CancellationToken cancellationToken)
    {
        var ticker = NormalizeTicker(request.Ticker);
        var name = NormalizeRequiredText(request.Name);
        var exchange = NormalizeOptionalText(request.Exchange);

        if (ticker is null)
        {
            return BadRequest("Ticker is required.");
        }

        if (name is null)
        {
            return BadRequest("Name is required.");
        }

        if (await TickerExistsAsync(ticker, exceptStockId: null, cancellationToken))
        {
            return Conflict($"Stock ticker '{ticker}' already exists.");
        }

        var sortOrder = request.SortOrder ?? await GetNextSortOrderAsync(cancellationToken);
        if (sortOrder <= 0)
        {
            return BadRequest("SortOrder must be greater than 0.");
        }

        var stock = new Stock
        {
            Ticker = ticker,
            Name = name,
            Exchange = exchange,
            IsActive = request.IsActive,
            SortOrder = sortOrder,
            CreatedAt = DateTime.UtcNow
        };

        dbContext.Stocks.Add(stock);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Created($"/api/stocks/{stock.Id}", ToResponse(stock));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<StockResponse>> UpdateStock(
        int id,
        UpdateStockRequest request,
        CancellationToken cancellationToken)
    {
        var stock = await dbContext.Stocks.FirstOrDefaultAsync(stock => stock.Id == id, cancellationToken);

        if (stock is null)
        {
            return NotFound($"Stock with id {id} was not found.");
        }

        if (request.Ticker is not null)
        {
            var ticker = NormalizeTicker(request.Ticker);
            if (ticker is null)
            {
                return BadRequest("Ticker cannot be empty.");
            }

            if (await TickerExistsAsync(ticker, id, cancellationToken))
            {
                return Conflict($"Stock ticker '{ticker}' already exists.");
            }

            stock.Ticker = ticker;
        }

        if (request.Name is not null)
        {
            var name = NormalizeRequiredText(request.Name);
            if (name is null)
            {
                return BadRequest("Name cannot be empty.");
            }

            stock.Name = name;
        }

        if (request.Exchange is not null)
        {
            stock.Exchange = NormalizeOptionalText(request.Exchange);
        }

        if (request.IsActive.HasValue)
        {
            stock.IsActive = request.IsActive.Value;
        }

        if (request.SortOrder.HasValue)
        {
            if (request.SortOrder.Value <= 0)
            {
                return BadRequest("SortOrder must be greater than 0.");
            }

            stock.SortOrder = request.SortOrder.Value;
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return Ok(ToResponse(stock));
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<DeleteStockResponse>> DeleteStock(int id, CancellationToken cancellationToken)
    {
        var stock = await dbContext.Stocks.FirstOrDefaultAsync(stock => stock.Id == id, cancellationToken);

        if (stock is null)
        {
            return NotFound($"Stock with id {id} was not found.");
        }

        var hasRelatedRows = await dbContext.StockNews.AnyAsync(news => news.StockId == id, cancellationToken) ||
            await dbContext.StockAnalyses.AnyAsync(analysis => analysis.StockId == id, cancellationToken);

        if (hasRelatedRows)
        {
            stock.IsActive = false;
            await dbContext.SaveChangesAsync(cancellationToken);

            return Ok(new DeleteStockResponse(
                stock.Id,
                stock.Ticker,
                Deleted: false,
                IsActive: stock.IsActive,
                Message: "Stock has related news or analyses and was disabled instead of deleted."));
        }

        dbContext.Stocks.Remove(stock);

        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException)
        {
            dbContext.Entry(stock).State = EntityState.Unchanged;
            stock.IsActive = false;
            await dbContext.SaveChangesAsync(cancellationToken);

            return Ok(new DeleteStockResponse(
                stock.Id,
                stock.Ticker,
                Deleted: false,
                IsActive: stock.IsActive,
                Message: "Stock could not be deleted because of related data and was disabled instead."));
        }

        return Ok(new DeleteStockResponse(
            stock.Id,
            stock.Ticker,
            Deleted: true,
            IsActive: false,
            Message: "Stock deleted."));
    }

    [HttpPut("order")]
    public async Task<ActionResult<IReadOnlyList<StockResponse>>> UpdateStockOrder(
        IReadOnlyList<StockOrderRequest> request,
        CancellationToken cancellationToken)
    {
        if (request.Count == 0)
        {
            return BadRequest("At least one stock order item is required.");
        }

        if (request.Any(item => item.Id <= 0))
        {
            return BadRequest("All stock ids must be greater than 0.");
        }

        if (request.Any(item => item.SortOrder <= 0))
        {
            return BadRequest("All sortOrder values must be greater than 0.");
        }

        var duplicateIds = request
            .GroupBy(item => item.Id)
            .Where(group => group.Count() > 1)
            .Select(group => group.Key)
            .ToList();

        if (duplicateIds.Count > 0)
        {
            return BadRequest($"Duplicate stock ids are not allowed: {string.Join(", ", duplicateIds)}.");
        }

        var ids = request.Select(item => item.Id).ToList();
        var stocks = await dbContext.Stocks
            .Where(stock => ids.Contains(stock.Id))
            .ToListAsync(cancellationToken);

        if (stocks.Count != ids.Count)
        {
            var foundIds = stocks.Select(stock => stock.Id).ToHashSet();
            var missingIds = ids.Where(id => !foundIds.Contains(id));
            return NotFound($"Stocks were not found for ids: {string.Join(", ", missingIds)}.");
        }

        var orderById = request.ToDictionary(item => item.Id, item => item.SortOrder);
        foreach (var stock in stocks)
        {
            stock.SortOrder = orderById[stock.Id];
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        var orderedStocks = await OrderedStocksQuery()
            .Select(stock => new StockResponse(
                stock.Id,
                stock.Ticker,
                stock.Name,
                stock.Exchange,
                stock.IsActive,
                stock.SortOrder,
                stock.CreatedAt))
            .ToListAsync(cancellationToken);

        return Ok(orderedStocks);
    }

    private IQueryable<Stock> OrderedStocksQuery()
    {
        return dbContext.Stocks
            .AsNoTracking()
            .OrderBy(stock => stock.SortOrder)
            .ThenBy(stock => stock.Ticker);
    }

    private async Task<bool> TickerExistsAsync(string ticker, int? exceptStockId, CancellationToken cancellationToken)
    {
        return await dbContext.Stocks.AnyAsync(
            stock => stock.Ticker == ticker && (!exceptStockId.HasValue || stock.Id != exceptStockId.Value),
            cancellationToken);
    }

    private async Task<int> GetNextSortOrderAsync(CancellationToken cancellationToken)
    {
        var currentMax = await dbContext.Stocks
            .Select(stock => (int?)stock.SortOrder)
            .MaxAsync(cancellationToken);

        return (currentMax ?? 0) + 1;
    }

    private static StockResponse ToResponse(Stock stock)
    {
        return new StockResponse(
            stock.Id,
            stock.Ticker,
            stock.Name,
            stock.Exchange,
            stock.IsActive,
            stock.SortOrder,
            stock.CreatedAt);
    }

    private static string? NormalizeTicker(string? value)
    {
        var trimmed = value?.Trim();
        return string.IsNullOrWhiteSpace(trimmed)
            ? null
            : trimmed.ToUpperInvariant();
    }

    private static string? NormalizeRequiredText(string? value)
    {
        var trimmed = value?.Trim();
        return string.IsNullOrWhiteSpace(trimmed) ? null : trimmed;
    }

    private static string? NormalizeOptionalText(string? value)
    {
        var trimmed = value?.Trim();
        return string.IsNullOrWhiteSpace(trimmed) ? null : trimmed;
    }
}
