import { ArrowDown, ArrowUp, Power, Trash2 } from 'lucide-react';
import { FormEvent, useState } from 'react';
import { PageHeader } from '../components/PageHeader';
import { useStocks } from '../hooks/useStocks';

export function AdminPage() {
  const { stocks, isLoading, error, isFallback, addStock, removeStock, toggleStock, moveStock } = useStocks();
  const [ticker, setTicker] = useState('');
  const [name, setName] = useState('');
  const [exchange, setExchange] = useState('US');

  async function handleSubmit(event: FormEvent) {
    event.preventDefault();

    if (!ticker.trim() || !name.trim()) {
      return;
    }

    const wasSaved = await addStock({ ticker, name, exchange });

    if (wasSaved) {
      setTicker('');
      setName('');
    }
  }

  return (
    <div className="admin-page">
      <PageHeader
        eyebrow="Control plane"
        title="Tracked stocks"
        description="Manage the tickers AlphaMind follows. Local controls are ready for backend stock admin endpoints."
      />

      {(error || isFallback) && (
        <div className="status-callout" role="status">
          {isFallback
            ? `Showing fallback stock data. Stock admin API endpoints are not available yet. ${error ?? ''}`
            : error}
        </div>
      )}

      <form className="admin-form" onSubmit={handleSubmit}>
        <label className="field">
          <span>Ticker</span>
          <input value={ticker} placeholder="MSFT" onChange={(event) => setTicker(event.target.value)} />
        </label>
        <label className="field">
          <span>Company</span>
          <input value={name} placeholder="Microsoft" onChange={(event) => setName(event.target.value)} />
        </label>
        <label className="field">
          <span>Exchange</span>
          <input value={exchange} placeholder="US" onChange={(event) => setExchange(event.target.value)} />
        </label>
        <button className="button button--primary" type="submit">
          Add stock
        </button>
      </form>

      <section className="stock-table" aria-label="Tracked stocks">
        {isLoading && <div className="status-callout">Loading stocks from AlphaMind API...</div>}
        {stocks.map((stock, index) => (
          <article className={stock.isActive ? 'stock-row' : 'stock-row is-disabled'} key={stock.id}>
            <div className="stock-row__identity">
              <span>{stock.sortOrder}</span>
              <div>
                <strong>{stock.ticker}</strong>
                <small>{stock.name}</small>
              </div>
            </div>
            <span className="stock-row__exchange">{stock.exchange ?? 'Unknown'}</span>
            <span className={stock.isActive ? 'status-pill is-on' : 'status-pill'}>{stock.isActive ? 'Active' : 'Off'}</span>
            <div className="stock-row__actions">
              <button type="button" title="Move up" onClick={() => void moveStock(stock.id, -1)} disabled={index === 0}>
                <ArrowUp size={16} />
              </button>
              <button type="button" title="Move down" onClick={() => void moveStock(stock.id, 1)} disabled={index === stocks.length - 1}>
                <ArrowDown size={16} />
              </button>
              <button type="button" title="Enable or disable" onClick={() => void toggleStock(stock.id)}>
                <Power size={16} />
              </button>
              <button type="button" title="Remove" onClick={() => void removeStock(stock.id)}>
                <Trash2 size={16} />
              </button>
            </div>
          </article>
        ))}
      </section>
    </div>
  );
}
