import { PageHeader } from '../components/PageHeader';

export function SettingsPage() {
  return (
    <div className="settings-page">
      <PageHeader
        eyebrow="Configuration"
        title="Settings"
        description="API base URL, scheduler status and AI model preferences will live here as the platform grows."
      />

      <section className="settings-panel">
        <div>
          <span>API base URL</span>
          <code>VITE_ALPHAMIND_API_BASE_URL</code>
        </div>
        <div>
          <span>Data mode</span>
          <strong>Mock fallback enabled</strong>
        </div>
      </section>
    </div>
  );
}

