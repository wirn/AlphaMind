import { AnalysisCard } from "../components/AnalysisCard";
import { AnalysisFilters } from "../components/AnalysisFilters";
import { MetricTile } from "../components/MetricTile";
import { PageHeader } from "../components/PageHeader";
import { useAnalyses } from "../hooks/useAnalyses";

export function DashboardPage() {
  const {
    groupedAnalyses,
    filteredAnalyses,
    filters,
    setFilters,
    isLoading,
    error,
    isFallback,
  } = useAnalyses();
  const averageImpact =
    filteredAnalyses.length === 0
      ? 0
      : Math.round(
          filteredAnalyses.reduce((sum, item) => sum + item.impactScore, 0) /
            filteredAnalyses.length,
        );
  const averageMove =
    filteredAnalyses.length === 0
      ? 0
      : Math.round(
          filteredAnalyses.reduce((sum, item) => sum + item.expectedMove, 0) /
            filteredAnalyses.length,
        );

  return (
    <div className="dashboard-page">
      <PageHeader
        eyebrow="Live intelligence"
        title="Alpha Mind"
        meta={
          isLoading ? "Synchronizing" : `${filteredAnalyses.length} analyses`
        }
      />

      {error && (
        <div className="status-callout" role="status">
          {isFallback
            ? `Showing fallback mock data. API detail: ${error}`
            : error}
        </div>
      )}

      <section className="metrics-grid" aria-label="Dashboard metrics">
        <MetricTile
          label="Analyses"
          value={String(filteredAnalyses.length)}
          accent="cyan"
        />
        <MetricTile
          label="Avg Impact"
          value={String(averageImpact)}
          accent="green"
        />
        <MetricTile
          label="Avg Move"
          value={averageMove > 0 ? `+${averageMove}` : String(averageMove)}
          accent="pink"
        />
        <MetricTile label="Signal Mode" value="Preview" accent="amber" />
      </section>

      <AnalysisFilters filters={filters} onChange={setFilters} />

      <section className="analysis-groups">
        {isLoading && (
          <div className="status-callout">
            Loading analyses from AlphaMind API...
          </div>
        )}
        {!isLoading && groupedAnalyses.length === 0 && (
          <div className="status-callout">
            No analyses matched the current filters.
          </div>
        )}
        {groupedAnalyses.map((group) => (
          <div className="analysis-group" key={group.date}>
            <div className="analysis-group__header">
              <h2>{group.label}</h2>
              <span>{group.analyses.length} cards</span>
            </div>
            <div className="analysis-grid">
              {group.analyses.map((analysis) => (
                <AnalysisCard key={analysis.id} analysis={analysis} />
              ))}
            </div>
          </div>
        ))}
      </section>
    </div>
  );
}
