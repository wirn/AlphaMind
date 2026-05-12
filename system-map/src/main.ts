import mermaid from "mermaid";
import systemMapMarkdown from "../alpha-mind-system-map.md?raw";
import "./styles.css";

const app = document.querySelector<HTMLDivElement>("#app");

if (!app) {
  throw new Error("App root was not found.");
}

const diagramSource = extractMermaidDiagram(systemMapMarkdown);

app.innerHTML = `
  <main class="page-shell">
    <header class="page-header">
     
   
        <h1>AlphaMind</h1>

      <a class="source-link" href="./alpha-mind-system-map.md" target="_blank" rel="noreferrer">
        View Markdown
      </a>
    </header>


    <section class="diagram-panel" aria-label="AlphaMind architecture diagram">
      <div id="diagram" class="diagram" role="img" aria-label="AlphaMind system architecture diagram">
        <div class="loading">Rendering Mermaid diagram...</div>
      </div>
    </section>
  </main>
`;

mermaid.initialize({
  startOnLoad: false,
  securityLevel: "strict",
  theme: "base",
  themeVariables: {
    fontFamily:
      'Inter, ui-sans-serif, system-ui, -apple-system, BlinkMacSystemFont, "Segoe UI", sans-serif',
    primaryColor: "#e0f2fe",
    primaryTextColor: "#0f172a",
    primaryBorderColor: "#0284c7",
    lineColor: "#64748b",
    clusterBkg: "#f8fafc",
    clusterBorder: "#cbd5e1",
    edgeLabelBackground: "#ffffff",
  },
  flowchart: {
    curve: "basis",
    htmlLabels: true,
    padding: 18,
  },
});

renderDiagram(diagramSource).catch((error: unknown) => {
  const diagram = document.querySelector<HTMLDivElement>("#diagram");
  if (!diagram) {
    return;
  }

  diagram.innerHTML = `
    <div class="error-state">
      <strong>Could not render the system map.</strong>
      <pre>${escapeHtml(error instanceof Error ? error.message : String(error))}</pre>
    </div>
  `;
});

async function renderDiagram(source: string) {
  const diagram = document.querySelector<HTMLDivElement>("#diagram");
  if (!diagram) {
    return;
  }

  const { svg } = await mermaid.render("alpha-mind-system-map", source);
  diagram.innerHTML = svg;
}

function extractMermaidDiagram(markdown: string) {
  const match = markdown.match(/```mermaid\s*([\s\S]*?)```/);

  if (!match?.[1]?.trim()) {
    throw new Error(
      "No Mermaid code block was found in alpha-mind-system-map.md.",
    );
  }

  return match[1].trim();
}

function escapeHtml(value: string) {
  return value
    .replaceAll("&", "&amp;")
    .replaceAll("<", "&lt;")
    .replaceAll(">", "&gt;")
    .replaceAll('"', "&quot;")
    .replaceAll("'", "&#039;");
}
