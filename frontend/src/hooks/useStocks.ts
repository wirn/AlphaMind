import { useEffect, useMemo, useState } from 'react';
import { alphaMindClient } from '../api/alphaMindClient';
import type { Stock } from '../types/stock';

export function useStocks() {
  const [stocks, setStocks] = useState<Stock[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | undefined>();
  const [isFallback, setIsFallback] = useState(false);

  useEffect(() => {
    let isMounted = true;

    alphaMindClient.getStocks().then((result) => {
      if (isMounted) {
        setStocks(result.data);
        setError(result.error);
        setIsFallback(result.isFallback);
        setIsLoading(false);
      }
    });

    return () => {
      isMounted = false;
    };
  }, []);

  const orderedStocks = useMemo(
    () => [...stocks].sort((a, b) => a.sortOrder - b.sortOrder),
    [stocks],
  );

  async function addStock(stock: Pick<Stock, 'ticker' | 'name' | 'exchange'>) {
    try {
      const createdStock = await alphaMindClient.addStock(stock);
      setStocks((current) => [...current, createdStock]);
      setError(undefined);
      return true;
    } catch (actionError) {
      setError(actionError instanceof Error ? actionError.message : 'Unable to add stock.');
      return false;
    }
  }

  async function removeStock(id: number) {
    try {
      await alphaMindClient.removeStock(id);
      setStocks((current) => current.filter((stock) => stock.id !== id));
      setError(undefined);
    } catch (actionError) {
      setError(actionError instanceof Error ? actionError.message : 'Unable to remove stock.');
    }
  }

  async function toggleStock(id: number) {
    const stock = stocks.find((item) => item.id === id);

    if (!stock) {
      return;
    }

    try {
      const updatedStock = await alphaMindClient.updateStock({ ...stock, isActive: !stock.isActive });
      setStocks((current) => current.map((item) => (item.id === id ? updatedStock : item)));
      setError(undefined);
    } catch (actionError) {
      setError(actionError instanceof Error ? actionError.message : 'Unable to update stock.');
    }
  }

  async function moveStock(id: number, direction: -1 | 1) {
    const ordered = [...orderedStocks];
    const index = ordered.findIndex((stock) => stock.id === id);
    const targetIndex = index + direction;

    if (index < 0 || targetIndex < 0 || targetIndex >= ordered.length) {
      return;
    }

    [ordered[index], ordered[targetIndex]] = [ordered[targetIndex], ordered[index]];

    const reorderedStocks = ordered.map((stock, sortIndex) => ({
      ...stock,
      sortOrder: sortIndex + 1,
    }));

    try {
      const savedStocks = await alphaMindClient.updateStockOrder(reorderedStocks);
      setStocks(savedStocks);
      setError(undefined);
    } catch (actionError) {
      setError(actionError instanceof Error ? actionError.message : 'Unable to update stock order.');
    }
  }

  return {
    stocks: orderedStocks,
    isLoading,
    error,
    isFallback,
    addStock,
    removeStock,
    toggleStock,
    moveStock,
  };
}
