namespace AlphaMind.Api.Integrations.OpenAI;

public class OpenAiOptions
{
    public const string SectionName = "OpenAI";

    public string ApiKey { get; set; } = string.Empty;

    public string Model { get; set; } = string.Empty;
}

