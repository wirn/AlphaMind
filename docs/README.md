# AlphaMind

AlphaMind is a small AI-driven stock analysis app.

## Frontend API Configuration

Use the deployed Azure backend for frontend development:

```env
VITE_API_BASE_URL=https://alpha-mind-api-hnashcabdrd8hkbh.northeurope-01.azurewebsites.net
```

The API CORS allow-list includes the local Vite dev origins:

- `http://localhost:5173`
- `http://127.0.0.1:5173`

When the frontend is deployed, add the deployed frontend origin to the backend CORS allow-list.
