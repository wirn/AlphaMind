import { Clock3 } from "lucide-react";
import { ConfidenceBar } from "./ConfidenceBar";
import { DirectionBadge } from "./DirectionBadge";
import { ExpectedMove } from "./ExpectedMove";
import { ScoreRing } from "./ScoreRing";
import type { StockAnalysis } from "../types/stock";

interface AnalysisCardProps {
  analysis: StockAnalysis;
}

export function AnalysisCard({ analysis }: AnalysisCardProps) {
  return (
    <article className="analysis-card">
      <div className="analysis-card__header">
        <div>
          <span className="ticker-chip">{analysis.ticker}</span>
          <h3>{analysis.companyName}</h3>
        </div>
        <DirectionBadge direction={analysis.direction} />
      </div>

      <p className="analysis-card__summary">{analysis.summary}</p>

      <div className="analysis-card__metrics">
        <ScoreRing value={analysis.impactScore} label="Impact" />
        <ConfidenceBar value={analysis.confidenceScore} />
        <ExpectedMove value={analysis.expectedMove} />
      </div>

      <div className="analysis-card__lists">
        <div>
          <span>Opportunities</span>
          <ul>
            {analysis.opportunities.slice(0, 2).map((item) => (
              <li key={item}>{item}</li>
            ))}
          </ul>
        </div>
        <div>
          <span>Risks</span>
          <ul>
            {analysis.risks.slice(0, 2).map((item) => (
              <li key={item}>{item}</li>
            ))}
          </ul>
        </div>
      </div>

      <footer className="analysis-card__footer">
        <span>
          <Clock3 size={15} />
          {new Intl.DateTimeFormat(undefined, {
            hour: "2-digit",
            minute: "2-digit",
            month: "short",
            day: "numeric",
          }).format(new Date(analysis.analyzedAt))}
        </span>
        <span>{analysis.usedNewsCount} news items</span>
      </footer>
    </article>
  );
}
