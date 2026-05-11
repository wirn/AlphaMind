using System.ComponentModel.DataAnnotations;

namespace AlphaMind.Api.Models;

public record StockResponse(
    int Id,
    string Ticker,
    string Name,
    string? Exchange,
    bool IsActive,
    int SortOrder,
    DateTime CreatedAt);

public class CreateStockRequest
{
    [MaxLength(30)]
    public string? Ticker { get; set; }

    [MaxLength(200)]
    public string? Name { get; set; }

    [MaxLength(50)]
    public string? Exchange { get; set; }

    public bool IsActive { get; set; } = true;

    public int? SortOrder { get; set; }
}

public class UpdateStockRequest
{
    [MaxLength(30)]
    public string? Ticker { get; set; }

    [MaxLength(200)]
    public string? Name { get; set; }

    [MaxLength(50)]
    public string? Exchange { get; set; }

    public bool? IsActive { get; set; }

    public int? SortOrder { get; set; }
}

public record StockOrderRequest(int Id, int SortOrder);

public record DeleteStockResponse(
    int Id,
    string Ticker,
    bool Deleted,
    bool IsActive,
    string Message);
