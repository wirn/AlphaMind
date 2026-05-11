import { useEffect, useMemo, useState } from 'react';
import { alphaMindClient } from '../api/alphaMindClient';
import type { AnalysisFilters, GroupedAnalysesByDate, StockAnalysis } from '../types/stock';

const defaultFilters: AnalysisFilters = {
  search: '',
  impactMin: 0,
  impactMax: 100,
  confidenceMin: 0,
  confidenceMax: 100,
  moveMin: -10,
  moveMax: 50,
  direction: 'all',
};

export function useAnalyses() {
  const [analyses, setAnalyses] = useState<StockAnalysis[]>([]);
  const [filters, setFilters] = useState<AnalysisFilters>(defaultFilters);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | undefined>();
  const [isFallback, setIsFallback] = useState(false);

  useEffect(() => {
    let isMounted = true;

    alphaMindClient.getAnalyses().then((result) => {
      if (isMounted) {
        setAnalyses(result.data);
        setError(result.error);
        setIsFallback(result.isFallback);
        setIsLoading(false);
      }
    });

    return () => {
      isMounted = false;
    };
  }, []);

  const filteredAnalyses = useMemo(() => {
    return analyses.filter((analysis) => {
      const searchTarget = `${analysis.ticker} ${analysis.companyName}`.toLowerCase();
      const searchMatches = searchTarget.includes(filters.search.trim().toLowerCase());

      return (
        searchMatches &&
        analysis.impactScore >= filters.impactMin &&
        analysis.impactScore <= filters.impactMax &&
        analysis.confidenceScore >= filters.confidenceMin &&
        analysis.confidenceScore <= filters.confidenceMax &&
        analysis.expectedMove >= filters.moveMin &&
        analysis.expectedMove <= filters.moveMax &&
        (filters.direction === 'all' || analysis.direction === filters.direction)
      );
    });
  }, [analyses, filters]);

  const groupedAnalyses = useMemo<GroupedAnalysesByDate[]>(() => {
    const groups = new Map<string, StockAnalysis[]>();

    for (const analysis of filteredAnalyses) {
      const date = new Date(analysis.analyzedAt);
      const key = date.toISOString().slice(0, 10);
      groups.set(key, [...(groups.get(key) ?? []), analysis]);
    }

    return [...groups.entries()]
      .sort(([a], [b]) => b.localeCompare(a))
      .map(([date, groupAnalyses]) => ({
        date,
        label: new Intl.DateTimeFormat(undefined, {
          weekday: 'long',
          month: 'short',
          day: 'numeric',
          year: 'numeric',
        }).format(new Date(`${date}T00:00:00Z`)),
        analyses: groupAnalyses.sort((a, b) => {
          const byAnalyzedAt = new Date(b.analyzedAt).getTime() - new Date(a.analyzedAt).getTime();
          return byAnalyzedAt || b.impactScore - a.impactScore;
        }),
      }));
  }, [filteredAnalyses]);

  return {
    analyses,
    filteredAnalyses,
    groupedAnalyses,
    filters,
    setFilters,
    isLoading,
    error,
    isFallback,
  };
}
