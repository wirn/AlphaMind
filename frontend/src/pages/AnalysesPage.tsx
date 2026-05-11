import { AnalysisCard } from '../components/AnalysisCard';
import { AnalysisFilters } from '../components/AnalysisFilters';
import { PageHeader } from '../components/PageHeader';
import { useAnalyses } from '../hooks/useAnalyses';

export function AnalysesPage() {
  const { groupedAnalyses, filters, setFilters, isLoading, error, isFallback } = useAnalyses();

  return (
    <div className="analyses-page">
      <PageHeader
        eyebrow="Research archive"
        title="Grouped analyses"
        description="Browse persisted and preview-ready AI reads by analysis date."
      />

      <AnalysisFilters filters={filters} onChange={setFilters} />

      <section className="analysis-groups">
        {error && (
          <div className="status-callout" role="status">
            {isFallback ? `Showing fallback mock data. API detail: ${error}` : error}
          </div>
        )}
        {isLoading && <div className="status-callout">Loading analyses from AlphaMind API...</div>}
        {!isLoading && groupedAnalyses.length === 0 && (
          <div className="status-callout">No analyses matched the current filters.</div>
        )}
        {groupedAnalyses.map((group) => (
          <div className="analysis-group" key={group.date}>
            <div className="analysis-group__header">
              <h2>{group.label}</h2>
              <span>{group.analyses.length} analyses</span>
            </div>
            <div className="analysis-grid analysis-grid--wide">
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
