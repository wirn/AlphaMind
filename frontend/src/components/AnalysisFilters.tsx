import type {
  AnalysisFilters as AnalysisFiltersType,
  Direction,
} from "../types/stock";

interface AnalysisFiltersProps {
  filters: AnalysisFiltersType;
  onChange: (filters: AnalysisFiltersType) => void;
}

const directions: Array<Direction | "all"> = [
  "all",
  "positive",
  "negative",
  "neutral",
  "mixed",
];

export function AnalysisFilters({ filters, onChange }: AnalysisFiltersProps) {
  return (
    <section className="filters-panel" aria-label="Analysis filters">
      <label className="field field--wide">
        <span>Search</span>
        <input
          value={filters.search}
          placeholder="Ticker or company"
          onChange={(event) =>
            onChange({ ...filters, search: event.target.value })
          }
        />
      </label>
      <RangeField
        label="Impact"
        min={0}
        max={100}
        minValue={filters.impactMin}
        maxValue={filters.impactMax}
        onMinChange={(impactMin) => onChange({ ...filters, impactMin })}
        onMaxChange={(impactMax) => onChange({ ...filters, impactMax })}
      />
      <RangeField
        label="Confidence"
        min={0}
        max={100}
        minValue={filters.confidenceMin}
        maxValue={filters.confidenceMax}
        onMinChange={(confidenceMin) => onChange({ ...filters, confidenceMin })}
        onMaxChange={(confidenceMax) => onChange({ ...filters, confidenceMax })}
      />
      <RangeField
        label="Expected Move"
        min={-10}
        max={50}
        minValue={filters.moveMin}
        maxValue={filters.moveMax}
        onMinChange={(moveMin) => onChange({ ...filters, moveMin })}
        onMaxChange={(moveMax) => onChange({ ...filters, moveMax })}
      />
      <label className="field">
        <span>Direction</span>
        <select
          value={filters.direction}
          onChange={(event) =>
            onChange({
              ...filters,
              direction: event.target.value as AnalysisFiltersType["direction"],
            })
          }
        >
          {directions.map((direction) => (
            <div>
              <option key={direction} value={direction}>
                {direction}
              </option>
            </div>
          ))}
        </select>
      </label>
    </section>
  );
}

interface RangeFieldProps {
  label: string;
  min: number;
  max: number;
  minValue: number;
  maxValue: number;
  onMinChange: (value: number) => void;
  onMaxChange: (value: number) => void;
}

function RangeField({
  label,
  min,
  max,
  minValue,
  maxValue,
  onMinChange,
  onMaxChange,
}: RangeFieldProps) {
  return (
    <div className="field field--range">
      <span>{label}</span>
      <div>
        <input
          type="number"
          min={min}
          max={max}
          value={minValue}
          onChange={(event) => onMinChange(Number(event.target.value))}
        />
        <input
          type="number"
          min={min}
          max={max}
          value={maxValue}
          onChange={(event) => onMaxChange(Number(event.target.value))}
        />
      </div>
    </div>
  );
}
