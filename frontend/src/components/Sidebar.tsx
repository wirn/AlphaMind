import type { LucideIcon } from 'lucide-react';
import type { AppPage } from '../App';

interface SidebarItem {
  page: AppPage;
  label: string;
  icon: LucideIcon;
}

interface SidebarProps {
  brandIcon: LucideIcon;
  items: SidebarItem[];
  activePage: AppPage;
  onNavigate: (page: AppPage) => void;
}

export function Sidebar({ brandIcon: BrandIcon, items, activePage, onNavigate }: SidebarProps) {
  return (
    <aside className="sidebar">
      <div className="sidebar__brand">
        <span className="sidebar__brand-mark">
          <BrandIcon size={22} />
        </span>
        <div>
          <strong>AlphaMind</strong>
          <span>AI Trading OS</span>
        </div>
      </div>

      <nav className="sidebar__nav" aria-label="Primary navigation">
        {items.map((item) => {
          const Icon = item.icon;
          const isActive = item.page === activePage;

          return (
            <button
              key={item.page}
              className={isActive ? 'sidebar__nav-item is-active' : 'sidebar__nav-item'}
              type="button"
              onClick={() => onNavigate(item.page)}
            >
              <Icon size={18} />
              <span>{item.label}</span>
            </button>
          );
        })}
      </nav>

      <div className="sidebar__status">
        <span className="pulse-dot" />
        <div>
          <strong>Neural feed</strong>
          <span>Mock mode active</span>
        </div>
      </div>
    </aside>
  );
}

