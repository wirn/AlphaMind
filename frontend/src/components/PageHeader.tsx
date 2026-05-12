interface PageHeaderProps {
  eyebrow: string;
  title: string;
  description: string;
  meta?: string;
}

export function PageHeader({
  eyebrow,
  title,
  description,
  meta,
}: PageHeaderProps) {
  return (
    <header className="page-header">
      <div>
        <span className="eyebrow">{eyebrow}</span>
        <h1>{title}</h1>
        {description ? <p>{description}</p> : null}
      </div>
      {meta ? <span className="page-header__meta">{meta}</span> : null}
    </header>
  );
}
