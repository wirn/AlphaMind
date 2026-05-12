# AlphaMind Architecture Notes

## Purpose

AlphaMind collects tracked stock news, generates AI-assisted stock analysis, persists the results, and presents them through a React frontend. This document summarizes the current implementation and the planned direction.

## Existing Functionality

### Frontend

- React/Vite app under `frontend`.
- Uses `VITE_API_BASE_URL` to call the backend API.
- Main views include dashboard, persisted analyses, tracked stock admin, and settings.
- `alphaMindClient` calls `/api/analysis` and `/api/stocks` endpoints and falls back to mock data when API calls fail.
- Admin UI can add, update, reorder, enable/disable, and remove tracked stocks through the stock API.

### Backend API

- .NET API under `backend/AlphaMind.Api`.
- Uses controllers for:
  - `GET /api/analysis` to list persisted stock analyses.
  - `GET/POST/PUT/DELETE /api/stocks` and `PUT /api/stocks/order` for tracked stock administration.
- Uses minimal API endpoints for operational flows:
  - `POST /api/fetcher/run`
  - `GET /api/analysis/preview`
  - `POST /api/analysis/run`
  - `POST /api/scheduler/run-fetcher`
  - `POST /api/scheduler/run-analysis`
  - `POST /api/scheduler/run-daily`
- Scheduler endpoints are protected with the `X-AlphaMind-Scheduler-Key` header.

### Data And Persistence

- Entity Framework Core is configured with SQL Server through `DefaultConnection`.
- The current model includes:
  - `Stocks`
  - `StockNews`
  - `FetcherRuns`
  - `StockAnalyses`
- EF migrations seed the initial tracked stock list.
- News rows are deduplicated by external id and source.
- Analysis rows store scores, direction, expected move, Swedish summary text, opportunities, risks, and metadata.

### Integrations

- Finnhub integration fetches company news from `https://finnhub.io/api/v1/company-news`.
- OpenAI integration uses the Responses API at `https://api.openai.com/v1/responses`.
- OpenAI output is requested as strict JSON with fields for direction, impact score, confidence score, expected move, summary, opportunities, and risks.
- The configured default OpenAI model in `appsettings.json` is `gpt-4.1-mini`.

### Scheduled Execution

- Azure Functions project lives under `functions/AlphaMind.Functions`.
- `DailyAlphaMindScheduler` runs on weekdays at `13:40 UTC`.
- The scheduler calls:
  - `/api/scheduler/run-fetcher` to fetch and store recent news.
  - `/api/scheduler/run-analysis` repeatedly in batches until no more stocks are analyzed.
- The function client reads backend base URL and scheduler key from configuration.

### Environments

- Local development uses Vite for the frontend and the .NET API locally or against a configured deployed API.
- Production is Azure-oriented:
  - Frontend is configured for Azure Static Web Apps in CORS.
  - Backend appears to run on Azure App Service.
  - Scheduler runs as Azure Functions.
  - Database is expected to be Azure SQL or compatible SQL Server.

## Planned / Future Functionality

- Email alerts or notifications based on saved analysis results.
- Alert rules or thresholds, such as high impact score, negative expected move, or specific tracked tickers.
- A notification worker or scheduled job that evaluates new analyses and sends messages through an email provider.
- More explicit production infrastructure documentation, including deployment topology, secret ownership, and environment-specific configuration.

## Assumptions

- Azure SQL is the intended production database because EF Core is configured for SQL Server and the project requirements call out Azure SQL.
- Azure App Service is the intended backend host based on the root README API URL.
- Azure Static Web Apps is the intended frontend host based on backend CORS configuration.
- Email notifications are not implemented in the current source tree and are represented as future architecture only.
