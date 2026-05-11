using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AlphaMind.Functions;

public class DailyAlphaMindScheduler(
    AlphaMindSchedulerClient schedulerClient,
    IConfiguration configuration,
    ILogger<DailyAlphaMindScheduler> logger)
{
    private const int MaxAnalysisIterations = 50;

    [Function(nameof(DailyAlphaMindScheduler))]
    public async Task RunAsync(
        [TimerTrigger("0 40 13 * * 1-5")] TimerInfo timerInfo,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Daily AlphaMind scheduler started at {StartedAt:u}.", DateTime.UtcNow);

        var maxStocksPerBatch = GetMaxStocksPerBatch();

        await schedulerClient.PostAsync("/api/scheduler/run-fetcher", cancellationToken);

        var offset = 0;

        for (var iteration = 0; iteration < MaxAnalysisIterations; iteration++)
        {
            try
            {
                var path = $"/api/scheduler/run-analysis?maxStocks={maxStocksPerBatch}&offset={offset}";
                var responseBody = await schedulerClient.PostAsync(path, cancellationToken);
                var stocksAnalyzed = GetStocksAnalyzed(responseBody);

                if (stocksAnalyzed == 0)
                {
                    logger.LogInformation(
                        "AlphaMind analysis batching stopped at offset {Offset} because no stocks were analyzed.",
                        offset);
                    break;
                }
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception exception)
            {
                logger.LogError(
                    exception,
                    "AlphaMind analysis batch failed for offset {Offset}. Continuing with remaining offsets.",
                    offset);
            }

            offset += maxStocksPerBatch;
        }

        logger.LogInformation("Daily AlphaMind scheduler finished at {FinishedAt:u}.", DateTime.UtcNow);
    }

    private int GetMaxStocksPerBatch()
    {
        var configuredValue = configuration.GetValue("AlphaMindScheduler:MaxStocksPerBatch", 1);

        return Math.Max(1, configuredValue);
    }

    private static int? GetStocksAnalyzed(string responseBody)
    {
        using var document = JsonDocument.Parse(responseBody);

        if (document.RootElement.TryGetProperty("stocksAnalyzed", out var camelCaseValue) &&
            camelCaseValue.TryGetInt32(out var camelCaseStocksAnalyzed))
        {
            return camelCaseStocksAnalyzed;
        }

        if (document.RootElement.TryGetProperty("StocksAnalyzed", out var pascalCaseValue) &&
            pascalCaseValue.TryGetInt32(out var pascalCaseStocksAnalyzed))
        {
            return pascalCaseStocksAnalyzed;
        }

        return null;
    }
}
