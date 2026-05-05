using System.ComponentModel.DataAnnotations;

namespace AlphaMind.Api.Entities;

public class StockNews
{
    public int Id { get; set; }

    public int StockId { get; set; }

    public Stock Stock { get; set; } = null!;

    [Required]
    [MaxLength(200)]
    public string ExternalId { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Source { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string Headline { get; set; } = string.Empty;

    [MaxLength(4000)]
    public string? Summary { get; set; }

    [Required]
    [MaxLength(1000)]
    public string Url { get; set; } = string.Empty;

    public DateTime PublishedAt { get; set; }

    public DateTime FetchedAt { get; set; }
}

