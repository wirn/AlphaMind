# Scheduled Analysis Flow

This diagram shows the daily scheduled analysis process from timer trigger to user-visible results.

```mermaid
flowchart TD
    timer[Weekday Timer Trigger<br/>13:40 UTC]
    callFetcher[Call /api/scheduler/run-fetcher]
    fetchNews[Fetch recent company news]
    storeNews[(Store StockNews and FetcherRun)]
    callAnalysis[Call /api/scheduler/run-analysis<br/>in batches]
    aiAnalysis[Run AI analysis with latest news]
    persistResults[(Persist StockAnalyses)]
    frontend[Frontend displays latest analyses]
    alerts[Future email alerts]

    timer --> callFetcher
    callFetcher --> fetchNews
    fetchNews --> storeNews
    storeNews --> callAnalysis
    callAnalysis --> aiAnalysis
    aiAnalysis --> persistResults
    persistResults --> frontend
    persistResults -.-> alerts

    classDef step fill:#e0f2fe,stroke:#0284c7,color:#082f49
    classDef data fill:#fef3c7,stroke:#d97706,color:#78350f
    classDef future fill:#f1f5f9,stroke:#64748b,color:#334155,stroke-dasharray: 5 5

    class timer,callFetcher,fetchNews,callAnalysis,aiAnalysis,frontend step
    class storeNews,persistResults data
    class alerts future
```

## Flow Notes

1. Azure Functions starts the weekday timer trigger.
2. The scheduler client calls the backend fetcher endpoint with the scheduler key header.
3. The backend fetches recent Finnhub company news for active eligible stocks.
4. News and fetcher run metadata are stored in SQL Server/Azure SQL.
5. The scheduler calls the analysis endpoint repeatedly in batches.
6. The backend builds stock-specific news context and sends it to OpenAI.
7. Structured analysis results are persisted.
8. The React frontend reads persisted analyses from the backend API.
9. Future email alerts can evaluate newly persisted analysis results.
