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
	7e. Skapa Azure App Service för backend
	7f. Publicera AlphaMind.Api till Azure
	7g. Lägg in connection string i Azure App Service config
	7h. Lägg in Finnhub och OpenAI keys som App Settings
	7i. Testa Swagger i Azure
	7j. Testa /api/fetcher/run i Azure
	7k. Testa /api/analysis/run i Azure
	
8. Scheduler
9. E-post/alerts
10. Frontend/Admin för aktier
11. Loggning + felhantering