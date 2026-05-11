interface MetricTileProps {
  label: string;
  value: string;
  accent: 'cyan' | 'green' | 'pink' | 'amber';
}

export function MetricTile({ label, value, accent }: MetricTileProps) {
  return (
    <div className={`metric-tile metric-tile--${accent}`}>
      <span>{label}</span>
      <strong>{value}</strong>
    </div>
  );
}

