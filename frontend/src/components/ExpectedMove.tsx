import { ArrowDown, ArrowRight, ArrowUp } from 'lucide-react';

interface ExpectedMoveProps {
  value: number;
}

export function ExpectedMove({ value }: ExpectedMoveProps) {
  const clampedValue = Math.max(-10, Math.min(50, value));
  const position = ((clampedValue + 10) / 60) * 100;
  const Icon = value > 3 ? ArrowUp : value < -3 ? ArrowDown : ArrowRight;
  const tone = value > 3 ? 'is-positive' : value < -3 ? 'is-negative' : 'is-neutral';

  return (
    <div className={`expected-move ${tone}`}>
      <div className="expected-move__value">
        <Icon size={18} />
        <strong>{value > 0 ? `+${value}` : value}</strong>
      </div>
      <div className="expected-move__scale" aria-label={`Expected move ${value}, scale -10 to +50`}>
        <span className="expected-move__zero" />
        <span className="expected-move__marker" style={{ left: `${position}%` }} />
      </div>
      <div className="expected-move__ticks">
        <span>-10</span>
        <span>0</span>
        <span>+50</span>
      </div>
    </div>
  );
}

