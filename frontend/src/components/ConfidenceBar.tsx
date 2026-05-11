interface ConfidenceBarProps {
  value: number;
}

export function ConfidenceBar({ value }: ConfidenceBarProps) {
  const normalizedValue = Math.max(0, Math.min(100, value));

  return (
    <div className="confidence-bar" aria-label={`Confidence score: ${value}`}>
      <div className="confidence-bar__label">
        <span>Confidence</span>
        <strong>{value}</strong>
      </div>
      <div className="confidence-bar__track">
        <span style={{ width: `${normalizedValue}%` }} />
      </div>
    </div>
  );
}

