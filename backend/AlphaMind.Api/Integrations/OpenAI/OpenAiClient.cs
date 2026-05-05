using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;

namespace AlphaMind.Api.Integrations.OpenAI;

public class OpenAiClient(HttpClient httpClient, IOptions<OpenAiOptions> options) : IOpenAiClient
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private readonly OpenAiOptions options = options.Value;

    public async Task<StockAnalysisAiResult> CreateStockAnalysisAsync(
        string prompt,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(options.ApiKey))
        {
            throw new InvalidOperationException("OpenAI API key is not configured.");
        }

        if (string.IsNullOrWhiteSpace(options.Model))
        {
            throw new InvalidOperationException("OpenAI model is not configured.");
        }

        using var request = new HttpRequestMessage(HttpMethod.Post, "responses");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", options.ApiKey);
        request.Content = JsonContent.Create(BuildRequestBody(prompt), options: JsonOptions);

        using var response = await httpClient.SendAsync(request, cancellationToken);
        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException(
                $"OpenAI request failed with {(int)response.StatusCode} {response.ReasonPhrase}: {responseBody}");
        }

        var outputText = ExtractOutputText(responseBody);
        var result = JsonSerializer.Deserialize<StockAnalysisAiResult>(outputText, JsonOptions);

        if (result is null)
        {
            throw new InvalidOperationException("OpenAI returned empty analysis JSON.");
        }

        ValidateResult(result);
        return result;
    }

    private object BuildRequestBody(string prompt)
    {
        return new
        {
            model = options.Model,
            store = false,
            input = prompt,
            text = new
            {
                format = new
                {
                    type = "json_schema",
                    name = "stock_analysis_preview",
                    strict = true,
                    schema = new
                    {
                        type = "object",
                        additionalProperties = false,
                        required = new[]
                        {
                            "direction",
                            "impactScore",
                            "confidenceScore",
                            "expectedMove",
                            "summary",
                            "opportunities",
                            "risks"
                        },
                        properties = new
                        {
                            direction = new
                            {
                                type = "string",
                                @enum = new[] { "positive", "negative", "neutral", "mixed" }
                            },
                            impactScore = new
                            {
                                type = "integer",
                                minimum = 1,
                                maximum = 100
                            },
                            confidenceScore = new
                            {
                                type = "integer",
                                minimum = 1,
                                maximum = 100
                            },
                            expectedMove = new
                            {
                                type = "integer",
                                minimum = -50,
                                maximum = 50
                            },
                            summary = new
                            {
                                type = "string"
                            },
                            opportunities = new
                            {
                                type = "array",
                                items = new
                                {
                                    type = "string"
                                }
                            },
                            risks = new
                            {
                                type = "array",
                                items = new
                                {
                                    type = "string"
                                }
                            }
                        }
                    }
                }
            }
        };
    }

    private static string ExtractOutputText(string responseBody)
    {
        using var document = JsonDocument.Parse(responseBody);

        if (document.RootElement.TryGetProperty("output_text", out var outputTextElement) &&
            outputTextElement.ValueKind == JsonValueKind.String)
        {
            return outputTextElement.GetString() ?? throw new InvalidOperationException("OpenAI output_text was empty.");
        }

        if (!document.RootElement.TryGetProperty("output", out var outputElement) ||
            outputElement.ValueKind != JsonValueKind.Array)
        {
            throw new InvalidOperationException("OpenAI response did not contain output text.");
        }

        var builder = new StringBuilder();

        foreach (var outputItem in outputElement.EnumerateArray())
        {
            if (!outputItem.TryGetProperty("content", out var contentElement) ||
                contentElement.ValueKind != JsonValueKind.Array)
            {
                continue;
            }

            foreach (var contentItem in contentElement.EnumerateArray())
            {
                if (contentItem.TryGetProperty("type", out var typeElement) &&
                    typeElement.GetString() == "output_text" &&
                    contentItem.TryGetProperty("text", out var textElement) &&
                    textElement.ValueKind == JsonValueKind.String)
                {
                    builder.Append(textElement.GetString());
                }
            }
        }

        if (builder.Length == 0)
        {
            throw new InvalidOperationException("OpenAI response did not contain output text.");
        }

        return builder.ToString();
    }

    private static void ValidateResult(StockAnalysisAiResult result)
    {
        if (result.Direction is not ("positive" or "negative" or "neutral" or "mixed"))
        {
            throw new InvalidOperationException("OpenAI returned invalid direction.");
        }

        result.ImpactScore = Math.Clamp(result.ImpactScore, 1, 100);
        result.ConfidenceScore = Math.Clamp(result.ConfidenceScore, 1, 100);
        result.ExpectedMove = Math.Clamp(result.ExpectedMove, -50, 50);

        if (string.IsNullOrWhiteSpace(result.Summary))
        {
            throw new InvalidOperationException("OpenAI returned an empty summary.");
        }
    }
}
