using System.ComponentModel.DataAnnotations;

namespace AlphaMind.Api.Entities;

public class FetcherRun
{
    public int Id { get; set; }

    public DateTime StartedAt { get; set; }

    public DateTime? FinishedAt { get; set; }

    [Required]
    [MaxLength(50)]
    public string Status { get; set; } = string.Empty;

    [MaxLength(4000)]
    public string? Message { get; set; }

    public int StocksChecked { get; set; }

    public int NewsFetched { get; set; }

    public int NewNewsSaved { get; set; }

    public int DuplicatesSkipped { get; set; }

    public int ErrorsCount { get; set; }
}

