1. Databas/datamodell ✅
	1a. Använd SQL Server LocalDB lokalt och Azure SQL Database Free Offer i molnet, med EF Core som ORM ✅
	1b. Installera EF Core-paket i backend ✅
	1c. Skapa entiteter (Stock, StockNews) ✅
	1d. Skapa DbContext (AlphaMindDbContext) ✅
	1e. Lägg till connection string i config ✅
	1f. Skapa migration och uppdatera databasen ✅
	1g. Lägg in seed-data (aktier) ✅
	1h. Skapa FetcherRun-tabell ✅
	1i. Lägg till SortOrder på Stock ✅

2. Fetcher (tex Finnhub, Polygon.io, Alpha Vantage) ✅
	2a. Välj första API-källa (Finnhub) ✅
	2b. Skaffa API-nyckel och spara i user-secrets ✅
	2c. Konfigurera HttpClient ✅
	2d. Implementera FinnhubClient ✅
	2e. Skapa DTO för API-respons ✅
	2f. Implementera StockNewsFetcher ✅
	2g. Skapa manuell trigger endpoint ✅
	2h. Testa fetchern via Swagger ✅
	2i. Begränsa antal nyheter per aktie ✅
	2j. Skippa Stockholm-aktier i Finnhub-fetchern tills annan källa finns ✅

3. Spara fetcher-data ✅
	3a. Mappa Finnhub DTO till StockNews ✅
	3b. Spara nyheter i StockNews-tabellen ✅
	3c. Koppla varje nyhet till rätt StockId ✅
	3d. Undvik dubletter med ExternalId ✅
	3e. Spara endast senaste 20 nyheterna per aktie ✅
	3f. Verifiera sparad data i databasen ✅

4. Manuell trigger ✅
	4a. Skapa POST /api/fetcher/run ✅
	4b. Kör StockNewsFetcher via dependency injection ✅
	4c. Returnera FetcherRun-resultat som JSON ✅
	4d. Testa endpointen via Swagger ✅
	4e. Verifiera att körningen loggas i FetcherRuns ✅
	4f. Verifiera att nyheter sparas i StockNews ✅

5. AI-analys ✅
	5a. Lägg till OpenAI-konfiguration ✅
	5b. Spara OpenAI API-nyckel och modell i user-secrets ✅
	5c. Skapa OpenAI-klient ✅
	5d. Skapa AI-preview-service ✅
	5e. Hämta senaste 10 StockNews för vald ticker ✅
	5f. Bygg prompt baserad endast på sparad nyhetsdata ✅
	5g. Returnera strikt JSON från AI:n ✅
	5h. Inför ImpactScore, ConfidenceScore och ExpectedMove ✅
	5i. Lägg till Direction, Summary, Opportunities och Risks ✅
	5j. Skapa GET /api/analysis/preview ✅
	5k. Testa nya AI-scoremodellen i Swagger ✅

6. Spara AI-analys ✅
	6a. Skapa StockAnalysis-entitet ✅
	6b. Lägg till DbSet<StockAnalysis> ✅
	6c. Konfigurera relation mellan Stock och StockAnalysis ✅
	6d. Skapa migration och uppdatera databasen ✅
	6e. Skapa service som kör AI-analys och sparar resultat ✅
	6f. Skapa POST /api/analysis/run ✅
	6g. Spara ImpactScore, ConfidenceScore, ExpectedMove och Direction ✅
	6h. Spara Summary, Opportunities och Risks ✅
	6i. Spara UsedNewsCount och AnalyzedAt ✅
	6j. Testa sparad analys via Swagger ✅
	6k. Verifiera sparad analys i databasen ✅

7. Azure deployment
	7a. Skapa Azure SQL Database för AlphaMind ✅
	7b. Koppla backend lokalt till Azure SQL ✅
	7c. Kör EF Core migration mot Azure SQL ✅
	7d. Verifiera att databasen funkar (lokalt mot Azure) ✅
	7e. Skapa Azure App Service för backend ✅
	7f. Publicera AlphaMind.Api till Azure ✅
	7g. Lägg in connection string i Azure App Service config ✅
	7h. Lägg in Finnhub och OpenAI keys som App Settings ✅
	7i. Testa Swagger i Azure ✅
	7j. Testa /api/fetcher/run i Azure ✅
	7k. Testa /api/analysis/run i Azure ✅
	
8. Scheduler
    8a. Skapa skyddad scheduler-endpoint med API-key ✅
    8b. Dela upp scheduler i run-fetcher och run-analysis ✅
    8c. Testa run-fetcher i Azure ✅
    8d. Testa run-analysis i Azure med maxStocks/offset ✅
    8e. Skapa Azure Function Timer Trigger ✅
    8f. Lägg API base URL och scheduler-key i Function config ✅
    8g. Publicera Function App till Azure ✅
    8h. Kör fetcher en gång per dag ✅
    8i. Kör analys i batchar med maxStocks/offset ✅
    8j. Verifiera att FetcherRuns och StockAnalyses uppdateras automatiskt ✅
    8k. Lägg till tydlig loggning i Function App för varje steg ✅
    8l. Lägg till felhantering så analys inte avbryts helt om en batch failar ✅
    8m. Dokumentera scheduler-konfiguration i README/appsettings-exempel ⬜

9. E-post/alerts
    9a. Skapa AlertSettings-konfiguration ✅
    9b. Skapa AlertNotification-entitet ✅
    9c. Skapa EF migration för AlertNotifications ✅
    9d. Skapa alert evaluator-service ✅
    9e. Skapa IEmailSender-interface ✅
    9f. Implementera LogOnlyEmailSender ✅
    9g. Skapa endpoint: POST /api/alerts/run ✅
    9h. Testa alert-flöde lokalt ⬜
    9i. Koppla alerts till scheduler-flödet ⬜
    9j. Lägg till riktig e-postleverantör ⬜
	
10. Frontend/Admin för aktier
    10a. Verifiera att frontend kör mot Azure API ✅
    10b. Visa riktiga analyser från /api/analysis ✅
    10c. Visa riktiga aktier från /api/stocks
    10d. Lägg till aktie
    10e. Redigera aktie
    10f. Aktivera/inaktivera aktie
    10g. Ta bort aktie
    10h. Ändra sorteringsordning
    10i. Visa loading/error/empty states
    10j. Ta bort fallback mock data när API-flödet är stabilt
    10k. Lägg till adminvy för schedulerstatus
    10l. Lägg till adminvy för senaste fetcher-runs och analyser
    10m. Lägg senare till alertinställningar

11. Loggning + felhantering
    11a. Strukturera backendloggar
        - Fetcher started/completed/failed
        - Analysis started/completed/failed
        - Alert evaluation started/completed/failed

    11b. Lägg till korrelations-id/run-id
        - Koppla FetcherRun, analysis batch och alert run där det går

    11c. Spara tekniska fel i databas där det är relevant
        - FetcherRuns.ErrorMessage
        - AlertNotifications.ErrorMessage
        - Eventuellt AnalysisRun-logg senare

    11d. Förbättra API-fel
        - Returnera tydliga ProblemDetails
        - Undvik att läcka secrets eller interna detaljer

    11e. Lägg till Application Insights
        - Backend API
        - Azure Function
        - Eventuellt frontend senare

    11f. Lägg till health checks
        - API lever
        - Databas nåbar
        - Scheduler config finns
        - OpenAI/Finnhub config finns, utan att exponera nycklar

    11g. Dokumentera felsökning
        - Hur man testar API
        - Hur man testar scheduler
        - Hur man ser Function-loggar
        - Hur man verifierar e-postutskick