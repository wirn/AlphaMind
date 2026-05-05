using System.Globalization;
using System.Text;
using AlphaMind.Api.Data;
using AlphaMind.Api.Entities;
using AlphaMind.Api.Integrations.Finnhub;
using Microsoft.EntityFrameworkCore;

namespace AlphaMind.Api.Services;

public class StockNewsFetcher(
    AlphaMindDbContext dbContext,
    IFinnhubClient finnhubClient,
    ILogger<StockNewsFetcher> logger)
{
    private const int MaxNewsItemsPerStock = 20;
    private static readonly TimeSpan DelayBetweenApiCalls = TimeSpan.FromMilliseconds(750);

    public async Task<FetcherRun> FetchAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var run = new FetcherRun
        {
            StartedAt = now,
            Status = "Running"
        };

        dbContext.FetcherRuns.Add(run);
        await dbContext.SaveChangesAsync(cancellationToken);

        try
        {
            var to = DateOnly.FromDateTime(now);
            var from = to.AddDays(-7);
            var successfulStocks = 0;
            var errorMessages = new List<string>();
            var stocks = await dbContext.Stocks
                .Where(stock => stock.IsActive && stock.Exchange != "Stockholm")
                .OrderBy(stock => stock.SortOrder)
                .ThenBy(stock => stock.Ticker)
                .ToListAsync(cancellationToken);

            for (var index = 0; index < stocks.Count; index++)
            {
                var stock = stocks[index];
                run.StocksChecked++;

                try
                {
                    var newsItems = await finnhubClient.GetCompanyNewsAsync(stock.Ticker, from, to, cancellationToken);
                    run.NewsFetched += newsItems.Count;
                    var processedNewsItems = newsItems
                        .OrderByDescending(newsItem => newsItem.DateTime)
                        .Take(MaxNewsItemsPerStock);

                    foreach (var newsItem in processedNewsItems)
                    {
                        var externalId = newsItem.Id.ToString(CultureInfo.InvariantCulture);
                        var source = TrimToMaxLength(newsItem.Source, 100);
                        var headline = TrimToMaxLength(newsItem.Headline, 500);
                        var url = TrimToMaxLength(newsItem.Url, 1000);

                        if (string.IsNullOrWhiteSpace(externalId) ||
                            string.IsNullOrWhiteSpace(source) ||
                            string.IsNullOrWhiteSpace(headline) ||
                            string.IsNullOrWhiteSpace(url))
                        {
                            run.ErrorsCount++;
                            AddErrorMessage(errorMessages, stock.Ticker, "Skipped news item with missing required data.");
                            continue;
                        }

                        var alreadyExists = await dbContext.StockNews
                            .AnyAsync(news => news.ExternalId == externalId, cancellationToken);

                        if (alreadyExists)
                        {
                            run.DuplicatesSkipped++;
                            continue;
                        }

                        dbContext.StockNews.Add(new StockNews
                        {
                            StockId = stock.Id,
                            ExternalId = externalId,
                            Source = source,
                            Headline = headline,
                            Summary = TrimToMaxLength(newsItem.Summary, 4000),
                            Url = url,
                            PublishedAt = DateTimeOffset.FromUnixTimeSeconds(newsItem.DateTime).UtcDateTime,
                            FetchedAt = DateTime.UtcNow
                        });
                        run.NewNewsSaved++;
                    }

                    successfulStocks++;
                    await dbContext.SaveChangesAsync(cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (Exception exception)
                {
                    logger.LogWarning(exception, "Stock news fetch failed for {Ticker}.", stock.Ticker);
                    run.ErrorsCount++;
                    AddErrorMessage(errorMessages, stock.Ticker, exception.Message);
                    dbContext.ChangeTracker.Clear();
                    dbContext.FetcherRuns.Attach(run);
                    run.Status = "Running";
                    await dbContext.SaveChangesAsync(cancellationToken);
                }

                if (index < stocks.Count - 1)
                {
                    await Task.Delay(DelayBetweenApiCalls, cancellationToken);
                }
            }

            run.Message = BuildMessage(errorMessages);
            run.Status = successfulStocks switch
            {
                0 when run.StocksChecked > 0 => "Failed",
                _ when run.ErrorsCount > 0 => "CompletedWithErrors",
                _ => "Completed"
            };
            run.FinishedAt = DateTime.UtcNow;
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Stock news fetcher failed.");
            run.Status = "Failed";
            run.Message = TrimToMaxLength(exception.Message, 4000);
            run.FinishedAt = DateTime.UtcNow;
            run.ErrorsCount++;
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return run;
    }

    private static void AddErrorMessage(List<string> errorMessages, string ticker, string message)
    {
        errorMessages.Add(TrimToMaxLength($"{ticker}: {message}", 500) ?? ticker);
    }

    private static string? BuildMessage(List<string> errorMessages)
    {
        if (errorMessages.Count == 0)
        {
            return null;
        }

        var builder = new StringBuilder();

        foreach (var errorMessage in errorMessages)
        {
            if (builder.Length > 0)
            {
                builder.Append("; ");
            }

            var remainingLength = 4000 - builder.Length;
            if (remainingLength <= 0)
            {
                break;
            }

            builder.Append(errorMessage.Length <= remainingLength
                ? errorMessage
                : errorMessage[..remainingLength]);
        }

        return builder.ToString();
    }

    private static string? TrimToMaxLength(string? value, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return value;
        }

        return value.Length <= maxLength ? value : value[..maxLength];
    }
}
