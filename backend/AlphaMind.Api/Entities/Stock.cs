using System.ComponentModel.DataAnnotations;

namespace AlphaMind.Api.Entities;

public class Stock
{
    public int Id { get; set; }

    [Required]
    [MaxLength(30)]
    public string Ticker { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? Exchange { get; set; }

    public bool IsActive { get; set; }

    public int SortOrder { get; set; }

    public DateTime CreatedAt { get; set; }

    public ICollection<StockNews> NewsItems { get; set; } = [];

    public ICollection<StockAnalysis> Analyses { get; set; } = [];
}
