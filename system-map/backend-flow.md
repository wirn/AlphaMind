# Backend Flow

This diagram shows the main backend components behind the compact system map. It focuses on controllers, services, integrations, scheduler endpoints, and persistence.

```mermaid
flowchart LR
    frontend[React Frontend]
    scheduler[Azure Functions Scheduler]

    subgraph api[".NET Backend API"]
        stocksController[Stocks Controller]
        analysisController[Analysis Controller]
        schedulerEndpoints[Scheduler Endpoints]
        fetcher[StockNewsFetcher]
        preview[StockAnalysisPreviewService]
        runner[StockAnalysisRunService]
        ef[Entity Framework Core]
    end

    sql[(Azure SQL / SQL Server)]
    finnhub[Finnhub Company News API]
    openai[OpenAI Responses API]

    frontend -->|Stock admin requests| stocksController
    frontend -->|Read persisted analyses| analysisController
    scheduler -->|Protected scheduler calls| schedulerEndpoints

    stocksController --> ef
    analysisController --> ef
    schedulerEndpoints --> fetcher
    schedulerEndpoints --> runner

    fetcher -->|Fetch recent news| finnhub
    fetcher -->|Store news and run status| ef

    runner --> preview
    preview -->|Read latest stock news| ef
    preview -->|Send contextual prompt| openai
    runner -->|Persist structured AI result| ef

    ef --> sql

    classDef client fill:#e0f2fe,stroke:#0284c7,color:#082f49
    classDef backend fill:#ecfdf5,stroke:#059669,color:#064e3b
    classDef data fill:#fef3c7,stroke:#d97706,color:#78350f
    classDef external fill:#fae8ff,stroke:#a21caf,color:#581c87

    class frontend,scheduler client
    class stocksController,analysisController,schedulerEndpoints,fetcher,preview,runner,ef backend
    class sql data
    class finnhub,openai external
```

## Component Notes

- `StocksController` manages tracked stocks, including create, update, reorder, disable, and delete behavior.
- `AnalysisController` exposes persisted analysis results for the frontend.
- Scheduler endpoints are protected by `X-AlphaMind-Scheduler-Key`.
- `StockNewsFetcher` loads active non-Stockholm stocks, fetches Finnhub news, deduplicates news, and records fetcher runs.
- `StockAnalysisPreviewService` builds a news-context prompt and requests strict JSON analysis from OpenAI.
- `StockAnalysisRunService` persists AI analysis results as `StockAnalyses` rows.
- Entity Framework Core is the single persistence boundary for SQL Server/Azure SQL access.
