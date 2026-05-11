import type { CSSProperties } from 'react';

interface ScoreRingProps {
  value: number;
  label: string;
}

export function ScoreRing({ value, label }: ScoreRingProps) {
  const normalizedValue = Math.max(0, Math.min(100, value));
  const style = {
    '--score': `${normalizedValue * 3.6}deg`,
  } as CSSProperties;

  return (
    <div className="score-ring" style={style} aria-label={`${label}: ${value}`}>
      <div className="score-ring__inner">
        <strong>{value}</strong>
        <span>{label}</span>
      </div>
    </div>
  );
}
