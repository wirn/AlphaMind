# AlphaMind

AlphaMind is an AI-driven stock analysis app with a .NET API backend and a React/Vite frontend.

## Frontend API Configuration

The frontend reads the backend URL from `VITE_API_BASE_URL`.

Use the deployed Azure API for local frontend development:

```env
VITE_API_BASE_URL=https://alpha-mind-api-hnashcabdrd8hkbh.northeurope-01.azurewebsites.net
```

The backend CORS policy currently allows Vite dev origins:

- `http://localhost:5173`
- `http://127.0.0.1:5173`

When the frontend is deployed, add that deployed frontend origin to the backend CORS allow-list.
