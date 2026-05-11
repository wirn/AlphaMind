import { BarChart3, BrainCircuit, LayoutDashboard, Settings, SlidersHorizontal } from 'lucide-react';
import { useMemo, useState } from 'react';
import { Sidebar } from './components/Sidebar';
import { AdminPage } from './pages/AdminPage';
import { AnalysesPage } from './pages/AnalysesPage';
import { DashboardPage } from './pages/DashboardPage';
import { SettingsPage } from './pages/SettingsPage';

export type AppPage = 'dashboard' | 'analyses' | 'admin' | 'settings';

const navItems = [
  { page: 'dashboard' as const, label: 'Dashboard', icon: LayoutDashboard },
  { page: 'analyses' as const, label: 'Analyses', icon: BarChart3 },
  { page: 'admin' as const, label: 'Stocks', icon: SlidersHorizontal },
  { page: 'settings' as const, label: 'Settings', icon: Settings },
];

export function App() {
  const [activePage, setActivePage] = useState<AppPage>('dashboard');
  const ActivePage = useMemo(() => {
    switch (activePage) {
      case 'analyses':
        return <AnalysesPage />;
      case 'admin':
        return <AdminPage />;
      case 'settings':
        return <SettingsPage />;
      default:
        return <DashboardPage />;
    }
  }, [activePage]);

  return (
    <div className="app-shell">
      <Sidebar
        brandIcon={BrainCircuit}
        items={navItems}
        activePage={activePage}
        onNavigate={setActivePage}
      />
      <main className="app-shell__main">
        {ActivePage}
      </main>
    </div>
  );
}

