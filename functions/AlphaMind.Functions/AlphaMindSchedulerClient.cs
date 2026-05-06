using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AlphaMind.Functions;

public class AlphaMindSchedulerClient(
    HttpClient httpClient,
    IConfiguration configuration,
    ILogger<AlphaMindSchedulerClient> logger)
{
    private const string SchedulerKeyHeaderName = "X-AlphaMind-Scheduler-Key";

    public async Task<string> PostAsync(string path, CancellationToken cancellationToken)
    {
        var baseUrl = configuration["AlphaMindApi:BaseUrl"];
        var schedulerKey = configuration["AlphaMindApi:SchedulerKey"];

        if (string.IsNullOrWhiteSpace(baseUrl))
        {
            throw new InvalidOperationException("AlphaMindApi:BaseUrl is not configured.");
        }

        if (string.IsNullOrWhiteSpace(schedulerKey))
        {
            throw new InvalidOperationException("AlphaMindApi:SchedulerKey is not configured.");
        }

        var requestUri = new Uri(new Uri(baseUrl.TrimEnd('/') + "/"), path.TrimStart('/'));
        using var request = new HttpRequestMessage(HttpMethod.Post, requestUri);
        request.Headers.Add(SchedulerKeyHeaderName, schedulerKey);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        logger.LogInformation("Calling AlphaMind scheduler endpoint: {Method} {Url}", request.Method, requestUri);

        using var response = await httpClient.SendAsync(request, cancellationToken);
        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

        logger.LogInformation(
            "AlphaMind scheduler endpoint responded with {StatusCode}. Body: {Body}",
            (int)response.StatusCode,
            responseBody);

        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException(
                $"AlphaMind scheduler call failed with {(int)response.StatusCode} {response.ReasonPhrase}.");
        }

        return responseBody;
    }
}
