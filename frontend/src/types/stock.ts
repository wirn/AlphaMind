export type Direction = 'positive' | 'negative' | 'neutral' | 'mixed';

export interface Stock {
  id: number;
  ticker: string;
  name: string;
  exchange?: string | null;
  isActive: boolean;
  sortOrder: number;
}

export interface StockAnalysis {
  id: number;
  stockId: number;
  ticker: string;
  companyName: string;
  impactScore: number;
  confidenceScore: number;
  expectedMove: number;
  direction: Direction;
  summary: string;
  opportunities: string[];
  risks: string[];
  usedNewsCount: number;
  analyzedAt: string;
  createdAt?: string;
}

export interface ApiResult<T> {
  data: T;
  isFallback: boolean;
  error?: string;
}

export interface GroupedAnalysesByDate {
  date: string;
  label: string;
  analyses: StockAnalysis[];
}

export interface AnalysisFilters {
  search: string;
  impactMin: number;
  impactMax: number;
  confidenceMin: number;
  confidenceMax: number;
  moveMin: number;
  moveMax: number;
  direction: Direction | 'all';
}
