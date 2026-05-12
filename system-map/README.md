# AlphaMind System Map

This folder contains Mermaid-based architecture documentation for AlphaMind. The goal is to give new contributors and maintainers a quick, accurate view of the main application parts, with detailed flows split into focused diagrams.

## Files

- `alpha-mind-system-map.md` contains the compact high-level architecture overview.
- `backend-flow.md` contains backend-specific details, including controllers, scheduler endpoints, services, integrations, and Entity Framework Core.
- `scheduled-analysis-flow.md` shows the daily scheduled analysis process step by step.
- `architecture-notes.md` summarizes the current architecture, planned functionality, and assumptions.

## Viewing The Diagrams

GitHub renders Mermaid diagrams directly in Markdown files. Open any of the diagram files in GitHub or in an editor with Mermaid preview support, such as Visual Studio Code with a Mermaid extension.

This folder can also run as a standalone local preview:

```bash
npm install
npm run dev
```

For local rendering, paste the Mermaid block into the Mermaid Live Editor:

https://mermaid.live

## Architecture Overview

AlphaMind is an AI-assisted stock analysis system with a React/Vite frontend and a .NET backend API. The frontend displays dashboards, persisted analyses, and admin controls for tracked stocks. The backend exposes stock and analysis endpoints, stores data through Entity Framework Core, fetches company news from Finnhub, and sends curated news context to OpenAI for structured AI analysis.

Scheduled execution is handled by an Azure Functions project. The scheduler calls protected backend endpoints to fetch recent news and run analysis batches for active tracked stocks. Results are persisted in SQL Server/Azure SQL and then read by the frontend.

Future architecture includes email alerts or notifications driven by saved analysis results and alert rules.

Use `alpha-mind-system-map.md` for a one-screen overview. Use `backend-flow.md` and `scheduled-analysis-flow.md` when you need implementation-level flow details.
