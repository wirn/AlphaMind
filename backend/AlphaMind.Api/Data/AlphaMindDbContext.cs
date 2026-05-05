using AlphaMind.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace AlphaMind.Api.Data;

public class AlphaMindDbContext(DbContextOptions<AlphaMindDbContext> options) : DbContext(options)
{
    public DbSet<Stock> Stocks => Set<Stock>();

    public DbSet<StockNews> StockNews => Set<StockNews>();

    public DbSet<FetcherRun> FetcherRuns => Set<FetcherRun>();

    public DbSet<StockAnalysis> StockAnalyses => Set<StockAnalysis>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var seedCreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        modelBuilder.Entity<Stock>()
            .HasIndex(stock => stock.Ticker)
            .IsUnique();

        modelBuilder.Entity<Stock>()
            .Property(stock => stock.SortOrder)
            .IsRequired();

        modelBuilder.Entity<Stock>()
            .HasIndex(stock => stock.SortOrder);

        modelBuilder.Entity<Stock>()
            .HasData(
                new Stock
                {
                    Id = 1,
                    Ticker = "INVE-A.ST",
                    Name = "Investor A",
                    Exchange = "Stockholm",
                    IsActive = true,
                    SortOrder = 1,
                    CreatedAt = seedCreatedAt
                },
                new Stock
                {
                    Id = 2,
                    Ticker = "GOOG",
                    Name = "Alphabet Inc Class C",
                    Exchange = "US",
                    IsActive = true,
                    SortOrder = 2,
                    CreatedAt = seedCreatedAt
                },
                new Stock
                {
                    Id = 3,
                    Ticker = "MSFT",
                    Name = "Microsoft",
                    Exchange = "US",
                    IsActive = true,
                    SortOrder = 3,
                    CreatedAt = seedCreatedAt
                },
                new Stock
                {
                    Id = 4,
                    Ticker = "TSM",
                    Name = "Taiwan Semiconductor Mfg Co",
                    Exchange = "US",
                    IsActive = true,
                    SortOrder = 4,
                    CreatedAt = seedCreatedAt
                },
                new Stock
                {
                    Id = 5,
                    Ticker = "INDU-C.ST",
                    Name = "Industrivärden C",
                    Exchange = "Stockholm",
                    IsActive = true,
                    SortOrder = 5,
                    CreatedAt = seedCreatedAt
                },
                new Stock
                {
                    Id = 6,
                    Ticker = "ASML",
                    Name = "ASML HOLDING",
                    Exchange = "Netherlands",
                    IsActive = true,
                    SortOrder = 6,
                    CreatedAt = seedCreatedAt
                },
                new Stock
                {
                    Id = 7,
                    Ticker = "NVDA",
                    Name = "NVIDIA",
                    Exchange = "US",
                    IsActive = true,
                    SortOrder = 7,
                    CreatedAt = seedCreatedAt
                },
                new Stock
                {
                    Id = 8,
                    Ticker = "SVOL-B.ST",
                    Name = "Svolder B",
                    Exchange = "Stockholm",
                    IsActive = true,
                    SortOrder = 8,
                    CreatedAt = seedCreatedAt
                },
                new Stock
                {
                    Id = 9,
                    Ticker = "AMD",
                    Name = "Advanced Micro Devices",
                    Exchange = "US",
                    IsActive = true,
                    SortOrder = 9,
                    CreatedAt = seedCreatedAt
                },
                new Stock
                {
                    Id = 10,
                    Ticker = "AMZN",
                    Name = "Amazon.com",
                    Exchange = "US",
                    IsActive = true,
                    SortOrder = 10,
                    CreatedAt = seedCreatedAt
                });

        modelBuilder.Entity<StockNews>()
            .HasIndex(news => new { news.ExternalId, news.Source })
            .IsUnique();

        modelBuilder.Entity<StockNews>()
            .HasIndex(news => news.StockId);

        modelBuilder.Entity<StockNews>()
            .HasIndex(news => news.PublishedAt);

        modelBuilder.Entity<StockAnalysis>()
            .HasOne(analysis => analysis.Stock)
            .WithMany(stock => stock.Analyses)
            .HasForeignKey(analysis => analysis.StockId);

        modelBuilder.Entity<StockAnalysis>()
            .HasIndex(analysis => analysis.StockId);

        modelBuilder.Entity<StockAnalysis>()
            .HasIndex(analysis => analysis.AnalyzedAt);

        modelBuilder.Entity<StockAnalysis>()
            .HasIndex(analysis => analysis.ImpactScore);
    }
}
