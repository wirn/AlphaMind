namespace AlphaMind.Api.Integrations.Finnhub;

public interface IFinnhubClient
{
    Task<IReadOnlyList<FinnhubCompanyNewsItemDto>> GetCompanyNewsAsync(
        string ticker,
        DateOnly from,
        DateOnly to,
        CancellationToken cancellationToken = default);
}

