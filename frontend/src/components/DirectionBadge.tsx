import type { Direction } from '../types/stock';

interface DirectionBadgeProps {
  direction: Direction;
}

export function DirectionBadge({ direction }: DirectionBadgeProps) {
  return <span className={`direction-badge direction-badge--${direction}`}>{direction}</span>;
}

