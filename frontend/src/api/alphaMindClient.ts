import type { ApiResult, Stock, StockAnalysis } from '../types/stock';
import { apiConfig } from './config';
import { normalizeAnalysis, normalizeStock } from './mappers';
import { mockAnalyses, mockStocks } from './mockData';

const jsonHeaders = {
  'Content-Type': 'application/json',
};

export class AlphaMindApiError extends Error {
  constructor(
    message: string,
    public readonly status?: number,
  ) {
    super(message);
  }
}

async function request<T>(path: string, init?: RequestInit): Promise<T> {
  if (!apiConfig.baseUrl) {
    throw new AlphaMindApiError('API base URL is not configured.');
  }

  const response = await fetch(`${apiConfig.baseUrl}${path}`, {
    ...init,
    headers: {
      ...jsonHeaders,
      ...init?.headers,
    },
  });

  if (!response.ok) {
    throw new AlphaMindApiError(`AlphaMind API request failed: ${response.status}`, response.status);
  }

  return response.json() as Promise<T>;
}

function fallbackResult<T>(data: T, error: unknown): ApiResult<T> {
  return {
    data,
    isFallback: true,
    error: error instanceof Error ? error.message : 'The AlphaMind API request failed.',
  };
}

export const alphaMindClient = {
  async getAnalyses(): Promise<ApiResult<StockAnalysis[]>> {
    // TODO: Backend needed: GET /api/analysis returning latest persisted StockAnalysis rows.
    // Existing backend routes only expose GET /api/analysis/preview and POST /api/analysis/run.
    try {
      const analyses = await request<unknown[]>('/api/analysis');
      return {
        data: analyses.map(normalizeAnalysis),
        isFallback: false,
      };
    } catch (error) {
      return fallbackResult(mockAnalyses, error);
    }
  },

  async getStocks(): Promise<ApiResult<Stock[]>> {
    // TODO: Backend needed: GET /api/stocks returning tracked stocks.
    // Existing backend loads stocks internally for scheduler flows but has no public stock list endpoint.
    try {
      const stocks = await request<unknown[]>('/api/stocks');
      return {
        data: stocks.map(normalizeStock),
        isFallback: false,
      };
    } catch (error) {
      return fallbackResult(mockStocks, error);
    }
  },

  async addStock(_stock: Pick<Stock, 'ticker' | 'name' | 'exchange'>): Promise<Stock> {
    const stock = await request<unknown>('/api/stocks', {
      method: 'POST',
      body: JSON.stringify(_stock),
    });

    return normalizeStock(stock);
  },

  async updateStock(_stock: Stock): Promise<Stock> {
    const stock = await request<unknown>(`/api/stocks/${_stock.id}`, {
      method: 'PUT',
      body: JSON.stringify(_stock),
    });

    return normalizeStock(stock);
  },

  async updateStockOrder(_stocks: Stock[]): Promise<Stock[]> {
    const stocks = await request<unknown[]>('/api/stocks/order', {
      method: 'PUT',
      body: JSON.stringify(_stocks.map((stock) => ({ id: stock.id, sortOrder: stock.sortOrder }))),
    });

    return stocks.map(normalizeStock);
  },

  async removeStock(_id: number): Promise<void> {
    await request<unknown>(`/api/stocks/${_id}`, {
      method: 'DELETE',
    });
  },
};
