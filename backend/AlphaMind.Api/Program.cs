using AlphaMind.Api.Data;
using AlphaMind.Api.Integrations.Finnhub;
using AlphaMind.Api.Integrations.OpenAI;
using AlphaMind.Api.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddDbContext<AlphaMindDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlServerOptions => sqlServerOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorNumbersToAdd: null)));
builder.Services.Configure<FinnhubOptions>(
    builder.Configuration.GetSection(FinnhubOptions.SectionName));
builder.Services.Configure<OpenAiOptions>(
    builder.Configuration.GetSection(OpenAiOptions.SectionName));
builder.Services.AddHttpClient<IFinnhubClient, FinnhubClient>(client =>
{
    client.BaseAddress = new Uri("https://finnhub.io/api/v1/");
});
builder.Services.AddHttpClient<IOpenAiClient, OpenAiClient>(client =>
{
    client.BaseAddress = new Uri("https://api.openai.com/v1/");
});
builder.Services.AddScoped<StockNewsFetcher>();
builder.Services.AddScoped<StockAnalysisPreviewService>();
builder.Services.AddScoped<StockAnalysisRunService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddOpenApi();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/api/debug/finnhub-news", async (
    string? ticker,
    DateOnly? from,
    DateOnly? to,
    IFinnhubClient finnhubClient,
    CancellationToken cancellationToken) =>
{
    if (string.IsNullOrWhiteSpace(ticker))
    {
        return Results.BadRequest("Ticker is required.");
    }

    var toDate = to ?? DateOnly.FromDateTime(DateTime.Today);
    var fromDate = from ?? toDate.AddDays(-7);
    var news = await finnhubClient.GetCompanyNewsAsync(ticker, fromDate, toDate, cancellationToken);

    return Results.Ok(news);
})
.WithName("DebugFinnhubNews");

app.MapPost("/api/fetcher/run", async (
    StockNewsFetcher fetcher,
    CancellationToken cancellationToken) =>
{
    var run = await fetcher.FetchAsync(cancellationToken);

    return Results.Ok(new
    {
        run.Id,
        run.StartedAt,
        run.FinishedAt,
        run.Status,
        run.Message,
        run.StocksChecked,
        run.NewsFetched,
        run.NewNewsSaved,
        run.DuplicatesSkipped,
        run.ErrorsCount
    });
})
.WithName("RunFetcher");

app.MapGet("/api/analysis/preview", async (
    string? ticker,
    StockAnalysisPreviewService analysisPreviewService,
    CancellationToken cancellationToken) =>
{
    if (string.IsNullOrWhiteSpace(ticker))
    {
        return Results.BadRequest("Ticker is required.");
    }

    var result = await analysisPreviewService.AnalyzeAsync(ticker, cancellationToken);

    return Results.Ok(result);
})
.WithName("AnalysisPreview");

app.MapPost("/api/analysis/run", async (
    string? ticker,
    StockAnalysisRunService analysisRunService,
    CancellationToken cancellationToken) =>
{
    if (string.IsNullOrWhiteSpace(ticker))
    {
        return Results.BadRequest("Ticker is required.");
    }

    var analysis = await analysisRunService.RunAsync(ticker, cancellationToken);

    return Results.Ok(analysis);
})
.WithName("RunAnalysis");

app.MapPost("/api/scheduler/run-daily", async (
    HttpContext httpContext,
    IConfiguration configuration,
    AlphaMindDbContext dbContext,
    StockNewsFetcher fetcher,
    StockAnalysisRunService analysisRunService,
    int? maxStocks,
    CancellationToken cancellationToken) =>
{
    if (!IsSchedulerAuthorized(httpContext, configuration))
    {
        return Results.Unauthorized();
    }

    var startedAt = DateTime.UtcNow;
    var errors = new List<string>();
    var fetcherRun = await fetcher.FetchAsync(cancellationToken);

    if (fetcherRun.Status == "Failed")
    {
        errors.Add($"Fetcher failed: {fetcherRun.Message ?? "Unknown error."}");
    }
    else if (fetcherRun.ErrorsCount > 0 && !string.IsNullOrWhiteSpace(fetcherRun.Message))
    {
        errors.Add($"Fetcher completed with errors: {fetcherRun.Message}");
    }

    var analysisSummary = await RunAnalysisBatchAsync(
        dbContext,
        analysisRunService,
        offset: 0,
        maxStocks: maxStocks ?? 1,
        cancellationToken);

    errors.AddRange(analysisSummary.Errors);

    var status = errors.Count switch
    {
        0 => "Completed",
        _ when analysisSummary.StocksAnalyzed > 0 => "CompletedWithErrors",
        _ => "Failed"
    };

    return Results.Ok(new
    {
        StartedAt = startedAt,
        FinishedAt = DateTime.UtcNow,
        Status = status,
        FetcherRunId = fetcherRun.Id,
        MaxStocks = maxStocks ?? 1,
        analysisSummary.StocksAnalyzed,
        analysisSummary.StocksSkipped,
        analysisSummary.AnalyzedTickers,
        analysisSummary.SkippedTickers,
        analysisSummary.AnalysisResults,
        Errors = errors
    });
})
.WithName("RunDailyScheduler");

app.MapPost("/api/scheduler/run-fetcher", async (
    HttpContext httpContext,
    IConfiguration configuration,
    StockNewsFetcher fetcher,
    CancellationToken cancellationToken) =>
{
    if (!IsSchedulerAuthorized(httpContext, configuration))
    {
        return Results.Unauthorized();
    }

    var run = await fetcher.FetchAsync(cancellationToken);

    return Results.Ok(ToFetcherRunResponse(run));
})
.WithName("RunSchedulerFetcher");

app.MapPost("/api/scheduler/run-analysis", async (
    HttpContext httpContext,
    IConfiguration configuration,
    AlphaMindDbContext dbContext,
    StockAnalysisRunService analysisRunService,
    int? maxStocks,
    int? offset,
    CancellationToken cancellationToken) =>
{
    if (!IsSchedulerAuthorized(httpContext, configuration))
    {
        return Results.Unauthorized();
    }

    var summary = await RunAnalysisBatchAsync(
        dbContext,
        analysisRunService,
        offset: offset ?? 0,
        maxStocks,
        cancellationToken);

    return Results.Ok(summary);
})
.WithName("RunSchedulerAnalysis");

app.Run();

static bool IsSchedulerAuthorized(HttpContext httpContext, IConfiguration configuration)
{
    var configuredApiKey = configuration["Scheduler:ApiKey"];

    return !string.IsNullOrWhiteSpace(configuredApiKey) &&
        httpContext.Request.Headers.TryGetValue("X-AlphaMind-Scheduler-Key", out var providedApiKey) &&
        string.Equals(providedApiKey.ToString(), configuredApiKey, StringComparison.Ordinal);
}

static object ToFetcherRunResponse(AlphaMind.Api.Entities.FetcherRun run)
{
    return new
    {
        run.Id,
        run.StartedAt,
        run.FinishedAt,
        run.Status,
        run.Message,
        run.StocksChecked,
        run.NewsFetched,
        run.NewNewsSaved,
        run.DuplicatesSkipped,
        run.ErrorsCount
    };
}

static async Task<SchedulerAnalysisSummary> RunAnalysisBatchAsync(
    AlphaMindDbContext dbContext,
    StockAnalysisRunService analysisRunService,
    int offset,
    int? maxStocks,
    CancellationToken cancellationToken)
{
    var startedAt = DateTime.UtcNow;
    var errors = new List<string>();
    var analysisResults = new List<object>();
    var analyzedTickers = new List<string>();
    var skippedTickers = new List<string>();
    var normalizedOffset = Math.Max(0, offset);

    var activeStocks = await dbContext.Stocks
        .AsNoTracking()
        .Where(stock => stock.IsActive)
        .OrderBy(stock => stock.SortOrder)
        .ThenBy(stock => stock.Ticker)
        .ToListAsync(cancellationToken);

    var stockCandidates = activeStocks
        .Select(stock => new
        {
            Stock = stock,
            SkipReason = stock.Exchange == "Stockholm"
                ? "Stockholm exchange is skipped for the current Finnhub flow."
                : null
        })
        .ToList();

    var eligibleStocks = stockCandidates
        .Where(candidate => candidate.SkipReason is null)
        .Select(candidate => candidate.Stock)
        .ToList();

    foreach (var skippedStock in stockCandidates.Where(candidate => candidate.SkipReason is not null))
    {
        skippedTickers.Add($"{skippedStock.Stock.Ticker}: {skippedStock.SkipReason}");
    }

    foreach (var skippedStock in eligibleStocks.Take(normalizedOffset))
    {
        skippedTickers.Add($"{skippedStock.Ticker}: skipped by offset.");
    }

    var remainingEligibleStocks = eligibleStocks
        .Skip(normalizedOffset)
        .ToList();

    var stocksToAnalyze = maxStocks is > 0
        ? remainingEligibleStocks.Take(maxStocks.Value).ToList()
        : remainingEligibleStocks;

    if (maxStocks is <= 0)
    {
        skippedTickers.Add("maxStocks: value must be greater than 0; no eligible stocks were analyzed.");
        stocksToAnalyze = [];
    }

    foreach (var skippedStock in remainingEligibleStocks.Skip(stocksToAnalyze.Count))
    {
        skippedTickers.Add($"{skippedStock.Ticker}: skipped by maxStocks limit.");
    }

    foreach (var stock in stocksToAnalyze)
    {
        try
        {
            var analysis = await analysisRunService.RunForStockAsync(stock, cancellationToken);
            analysisResults.Add(analysis);
            analyzedTickers.Add(stock.Ticker);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception exception)
        {
            errors.Add($"{stock.Ticker}: {exception.Message}");
        }
    }

    var finishedAt = DateTime.UtcNow;
    var stocksAnalyzed = analysisResults.Count;
    var status = errors.Count switch
    {
        0 => "Completed",
        _ when stocksAnalyzed > 0 => "CompletedWithErrors",
        _ => "Failed"
    };

    return new SchedulerAnalysisSummary(
        StartedAt: startedAt,
        FinishedAt: finishedAt,
        Status: status,
        Offset: normalizedOffset,
        MaxStocks: maxStocks,
        StocksAnalyzed: stocksAnalyzed,
        StocksSkipped: skippedTickers.Count,
        AnalyzedTickers: analyzedTickers,
        SkippedTickers: skippedTickers,
        AnalysisResults: analysisResults,
        Errors: errors);
}

public record SchedulerAnalysisSummary(
    DateTime StartedAt,
    DateTime FinishedAt,
    string Status,
    int Offset,
    int? MaxStocks,
    int StocksAnalyzed,
    int StocksSkipped,
    IReadOnlyList<string> AnalyzedTickers,
    IReadOnlyList<string> SkippedTickers,
    IReadOnlyList<object> AnalysisResults,
    IReadOnlyList<string> Errors);
