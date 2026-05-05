using AlphaMind.Api.Data;
using AlphaMind.Api.Integrations.Finnhub;
using AlphaMind.Api.Integrations.OpenAI;
using AlphaMind.Api.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddDbContext<AlphaMindDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
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

app.Run();
