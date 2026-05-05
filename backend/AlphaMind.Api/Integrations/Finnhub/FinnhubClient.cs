using System.Globalization;
using System.Net.Http.Json;
using Microsoft.Extensions.Options;

namespace AlphaMind.Api.Integrations.Finnhub;

public class FinnhubClient(HttpClient httpClient, IOptions<FinnhubOptions> options) : IFinnhubClient
{
    private readonly FinnhubOptions options = options.Value;

    public async Task<IReadOnlyList<FinnhubCompanyNewsItemDto>> GetCompanyNewsAsync(
        string ticker,
        DateOnly from,
        DateOnly to,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(ticker))
        {
            throw new ArgumentException("Ticker is required.", nameof(ticker));
        }

        if (string.IsNullOrWhiteSpace(options.ApiKey))
        {
            throw new InvalidOperationException("Finnhub API key is not configured.");
        }

        var url = BuildCompanyNewsUrl(ticker, from, to);
        var news = await httpClient.GetFromJsonAsync<List<FinnhubCompanyNewsItemDto>>(url, cancellationToken);

        return news ?? [];
    }

    private string BuildCompanyNewsUrl(string ticker, DateOnly from, DateOnly to)
    {
        var fromValue = from.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        var toValue = to.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

        return "company-news" +
            $"?symbol={Uri.EscapeDataString(ticker)}" +
            $"&from={Uri.EscapeDataString(fromValue)}" +
            $"&to={Uri.EscapeDataString(toValue)}" +
            $"&token={Uri.EscapeDataString(options.ApiKey)}";
    }
}

