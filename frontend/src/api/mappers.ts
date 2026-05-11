import type { Direction, Stock, StockAnalysis } from '../types/stock';

type UnknownRecord = Record<string, unknown>;

const validDirections: Direction[] = ['positive', 'negative', 'neutral', 'mixed'];

function asRecord(value: unknown): UnknownRecord {
  return value && typeof value === 'object' ? (value as UnknownRecord) : {};
}

function readString(source: UnknownRecord, keys: string[], fallback = ''): string {
  for (const key of keys) {
    const value = source[key];
    if (typeof value === 'string') {
      return value;
    }
  }

  return fallback;
}

function readNumber(source: UnknownRecord, keys: string[], fallback = 0) {
  for (const key of keys) {
    const value = source[key];
    if (typeof value === 'number' && Number.isFinite(value)) {
      return value;
    }
  }

  return fallback;
}

function readBoolean(source: UnknownRecord, keys: string[], fallback = false) {
  for (const key of keys) {
    const value = source[key];
    if (typeof value === 'boolean') {
      return value;
    }
  }

  return fallback;
}

function readNullableString(source: UnknownRecord, keys: string[]) {
  for (const key of keys) {
    const value = source[key];
    if (typeof value === 'string' || value === null) {
      return value;
    }
  }

  return null;
}

function readOptionalString(source: UnknownRecord, keys: string[]): string | undefined {
  for (const key of keys) {
    const value = source[key];
    if (typeof value === 'string') {
      return value;
    }
  }

  return undefined;
}

export function parseStringArray(value: unknown): string[] {
  if (Array.isArray(value)) {
    return value.filter((item): item is string => typeof item === 'string');
  }

  if (typeof value !== 'string' || !value.trim()) {
    return [];
  }

  try {
    const parsed = JSON.parse(value);
    return Array.isArray(parsed)
      ? parsed.filter((item): item is string => typeof item === 'string')
      : [];
  } catch {
    return [];
  }
}

export function normalizeStock(value: unknown): Stock {
  const stock = asRecord(value);

  return {
    id: readNumber(stock, ['id', 'Id']),
    ticker: readString(stock, ['ticker', 'Ticker'], 'UNKNOWN'),
    name: readString(stock, ['name', 'Name'], 'Unknown company'),
    exchange: readNullableString(stock, ['exchange', 'Exchange']),
    isActive: readBoolean(stock, ['isActive', 'IsActive']),
    sortOrder: readNumber(stock, ['sortOrder', 'SortOrder']),
  };
}

export function normalizeAnalysis(value: unknown): StockAnalysis {
  const analysis = asRecord(value);
  const stock = asRecord(analysis.stock ?? analysis.Stock);
  const direction = readString(analysis, ['direction', 'Direction'], 'neutral') as Direction;

  return {
    id: readNumber(analysis, ['id', 'Id']),
    stockId: readNumber(analysis, ['stockId', 'StockId']),
    ticker: readString(analysis, ['ticker', 'Ticker'], readString(stock, ['ticker', 'Ticker'], 'UNKNOWN')),
    companyName: readString(
      analysis,
      ['companyName', 'CompanyName', 'stockName', 'StockName'],
      readString(stock, ['name', 'Name'], 'Unknown company'),
    ),
    impactScore: readNumber(analysis, ['impactScore', 'ImpactScore']),
    confidenceScore: readNumber(analysis, ['confidenceScore', 'ConfidenceScore']),
    expectedMove: readNumber(analysis, ['expectedMove', 'ExpectedMove']),
    direction: validDirections.includes(direction) ? direction : 'neutral',
    summary: readString(analysis, ['summary', 'Summary'], 'No summary returned.'),
    opportunities: parseStringArray(
      analysis.opportunities ?? analysis.Opportunities ?? analysis.opportunitiesJson ?? analysis.OpportunitiesJson,
    ),
    risks: parseStringArray(analysis.risks ?? analysis.Risks ?? analysis.risksJson ?? analysis.RisksJson),
    usedNewsCount: readNumber(analysis, ['usedNewsCount', 'UsedNewsCount']),
    analyzedAt: readString(analysis, ['analyzedAt', 'AnalyzedAt'], new Date().toISOString()),
    createdAt: readOptionalString(analysis, ['createdAt', 'CreatedAt']),
  };
}
