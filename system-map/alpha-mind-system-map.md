# AlphaMind System Map

This diagram is the high-level architecture overview. It intentionally avoids backend internals so the whole system can be understood on one screen.

```mermaid
flowchart LR
    user[User / Admin]
    frontend[React Frontend]
    backend[.NET Backend API]
    database[(Azure SQL / SQL Server)]
    scheduler[Azure Functions Scheduler]
    finnhub[Finnhub]
    openai[OpenAI]
    alerts[Future Email Alerts]
    hosting[Azure Hosting / App Settings]

    user --> frontend
    frontend --> backend
    scheduler --> backend
    backend --> database
    backend --> finnhub
    backend --> openai
    database --> frontend
    database -.-> alerts
    hosting -.-> frontend
    hosting -.-> backend
    hosting -.-> scheduler

    classDef actor fill:#f8fafc,stroke:#475569,color:#0f172a
    classDef app fill:#e0f2fe,stroke:#0284c7,color:#082f49
    classDef data fill:#fef3c7,stroke:#d97706,color:#78350f
    classDef external fill:#fae8ff,stroke:#a21caf,color:#581c87
    classDef future fill:#f1f5f9,stroke:#64748b,color:#334155,stroke-dasharray: 5 5
    classDef infra fill:#eef2ff,stroke:#4f46e5,color:#312e81

    class user actor
    class frontend,backend,scheduler app
    class database data
    class finnhub,openai external
    class alerts future
    class hosting infra
```

## Notes

- The React frontend is the user-facing application for dashboards, analyses, and stock administration.
- The .NET backend API owns application logic, integration calls, scheduler endpoints, and database access.
- Azure Functions triggers the scheduled analysis flow by calling protected backend endpoints.
- Email alerts are planned future functionality and are shown as a dashed dependency from stored analysis data.
