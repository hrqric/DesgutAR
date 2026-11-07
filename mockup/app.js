const dishes = [
  {
    id: "risoto",
    name: "Risoto de cogumelos",
    description: "Arroz arbóreo, cogumelos frescos e parmesão.",
    price: 49.9,
    serving: 350,
    className: "",
    nutrition: { "Energia": "520 kcal", "Carboidratos": "72 g", "Proteínas": "16 g", "Gorduras": "19 g", "Fibras": "5 g", "Sódio": "680 mg" },
    allergens: ["Leite"]
  },
  {
    id: "frango",
    name: "Frango grelhado",
    description: "Filé de frango, arroz integral e legumes.",
    price: 42,
    serving: 420,
    className: "chicken",
    nutrition: { "Energia": "480 kcal", "Carboidratos": "48 g", "Proteínas": "46 g", "Gorduras": "12 g", "Fibras": "7 g", "Sódio": "590 mg" },
    allergens: []
  },
  {
    id: "penne",
    name: "Penne ao molho pesto",
    description: "Massa, manjericão, azeite, castanhas e queijo.",
    price: 45.5,
    serving: 380,
    className: "pasta",
    nutrition: { "Energia": "610 kcal", "Carboidratos": "79 g", "Proteínas": "19 g", "Gorduras": "24 g", "Fibras": "6 g", "Sódio": "720 mg" },
    allergens: ["Glúten", "Leite", "Castanhas"]
  }
];

const susQuestions = [
  "Eu gostaria de utilizar este aplicativo com frequência.",
  "Eu achei o aplicativo desnecessariamente complexo.",
  "Eu achei o aplicativo fácil de usar.",
  "Eu precisaria do apoio de uma pessoa para utilizar o aplicativo.",
  "As funções do aplicativo estão bem integradas.",
  "Eu achei que havia muita inconsistência no aplicativo.",
  "A maioria das pessoas aprenderia a usar o aplicativo rapidamente.",
  "Eu achei o aplicativo muito complicado de usar.",
  "Eu me senti confiante utilizando o aplicativo.",
  "Eu precisei aprender muitas coisas antes de usar o aplicativo."
];

const state = { route: "home", selectedDish: dishes[0], detailTab: "nutrition", question: 0, answers: {} };
const view = document.querySelector("#view");

const money = value => value.toLocaleString("pt-BR", { style: "currency", currency: "BRL" });
const icon = (path) => `<svg viewBox="0 0 24 24" aria-hidden="true">${path}</svg>`;
const arrow = icon('<path d="m9 18 6-6-6-6"/>');
const back = icon('<path d="m15 18-6-6 6-6"/>');
const scanIcon = icon('<path d="M4 8V5a1 1 0 0 1 1-1h3M16 4h3a1 1 0 0 1 1 1v3M20 16v3a1 1 0 0 1-1 1h-3M8 20H5a1 1 0 0 1-1-1v-3"/><circle cx="12" cy="12" r="3"/>');

function plate(className = "") { return `<div class="plate ${className}" aria-label="Representação ilustrativa do prato"></div>`; }

function setRoute(route) {
  state.route = route;
  document.querySelectorAll(".nav-item").forEach(button => button.classList.toggle("active", button.dataset.nav === route));
  view.style.animation = "none";
  render();
  document.activeElement?.blur();
  window.scrollTo({ top: 0, behavior: "auto" });
  requestAnimationFrame(() => {
    view.style.animation = "view-in .38s ease both";
    window.scrollTo({ top: 0, behavior: "auto" });
  });
}

function homeView() {
  return `
    <div class="hero">
      <div>
        <h1>Cardápio com pratos em 3D e informação nutricional.</h1>
        <p class="lead">Aponte a câmera para a etiqueta do prato para ver o modelo, a porção, os nutrientes e os alergênicos.</p>
        <div class="hero-actions">
          <button class="primary-button" data-nav="scan">${scanIcon} Escanear etiqueta</button>
          <button class="secondary-button" id="explore-menu">Explorar cardápio ${arrow}</button>
        </div>
      </div>
      <div class="hero-visual">
        <img src="assets/models/risotto/thumbnail.jpg" alt="Risoto servido em prato branco" />
        <div class="hero-dish-caption"><strong>Risoto de cogumelos</strong><span>R$ 49,90 · 350 g · 520 kcal</span></div>
      </div>
    </div>
    <div id="menu-section" class="section-head">
      <div><h2>Pratos do cardápio</h2></div>
      <p>${dishes.length} opções cadastradas</p>
    </div>
    <div class="dish-grid">
      ${dishes.map((dish, index) => `
        <article class="dish-card" data-dish="${dish.id}" tabindex="0" role="button" aria-label="Ver ${dish.name}">
          <div class="dish-art ${dish.id === "risoto" ? "has-photo" : ""}"><span class="dish-number">0${index + 1}</span>${dish.id === "risoto" ? `<img src="assets/models/risotto/thumbnail.jpg" alt="${dish.name}" />` : plate(dish.className)}</div>
          <div class="dish-content">
            <h3>${dish.name}</h3><p>${dish.description}</p>
            <div class="dish-meta"><span class="dish-price">${money(dish.price)}</span><span class="dish-kcal">${dish.nutrition.Energia}</span></div>
          </div>
        </article>`).join("")}
    </div>`;
}

function scanView() {
  return `
    <div class="screen-header"><button class="back-button" data-nav="home">${back}</button><div><h2>Escanear etiqueta</h2><p>A leitura abaixo é simulada nesta versão.</p></div></div>
    <div class="scanner-layout">
      <div class="camera" id="camera">
        <div class="camera-copy"><strong>Posicione a etiqueta no centro</strong><span>mantenha o celular estável</span></div>
        <div class="scan-frame"><i class="corner tl"></i><i class="corner tr"></i><i class="corner bl"></i><i class="corner br"></i><span class="scan-line"></span></div>
        <div class="marker-card"><div class="marker-pattern"></div></div>
        <button class="primary-button camera-action" id="simulate-scan">${scanIcon}<span>Simular leitura</span></button>
      </div>
      <aside class="scan-sidebar">
        <div class="info-card"><h3>Após a leitura</h3><ol class="steps"><li>A câmera identifica o marcador.</li><li>O catálogo localiza o prato associado.</li><li>O modelo e os dados aparecem juntos.</li></ol></div>
        <div class="info-card simulation-note"><h3>Nesta versão</h3><p>O botão simula a câmera para demonstrar o fluxo enquanto o rastreamento real ainda está em desenvolvimento.</p></div>
      </aside>
    </div>`;
}

function detailPanel(dish) {
  if (state.detailTab === "allergens") {
    return dish.allergens.length
      ? `<div class="allergen-list">${dish.allergens.map(item => `<span class="allergen">Contém ${item}</span>`).join("")}</div>`
      : `<div class="safe-note"><span>✓</span> Nenhum alergênico declarado no catálogo.</div>`;
  }
  return `<div class="nutrition-grid">${Object.entries(dish.nutrition).map(([label, value]) => `<div class="nutrient"><strong>${value}</strong><span>${label}</span></div>`).join("")}</div>`;
}

function detailView() {
  const dish = state.selectedDish;
  const stageContent = dish.id === "risoto"
    ? `<canvas id="dish-model" aria-label="Modelo tridimensional real do risoto"></canvas><div class="model-loading" id="model-loading"><span></span>Carregando modelo 3D</div>`
    : plate(dish.className);
  return `
    <div class="screen-header"><button class="back-button" data-nav="home">${back}</button><div><h2>Detalhes do prato</h2><p>Etiqueta associada ao catálogo.</p></div></div>
    <div class="dish-detail">
      <div class="ar-stage ${dish.id === "risoto" ? "real-model-stage" : ""}">${stageContent}<p class="stage-hint">${dish.id === "risoto" ? `Arraste para girar • use a roda para aproximar<br><a href="https://poly.cam/capture/d9fc177c-d6a8-453c-8fdd-23637a745e2a" target="_blank" rel="noreferrer">Modelo de BeguilingShadowstalker2310 via Polycam</a>` : "Modelo ilustrativo • visualização simulada em 3D"}</p></div>
      <div class="detail-copy">
        <p class="recognition-status">Etiqueta reconhecida</p><h1>${dish.name}</h1>
        <div class="price-row"><strong>${money(dish.price)}</strong><span class="serving">Porção de ${dish.serving} g</span></div>
        <p class="detail-description">${dish.description}</p>
        <div class="detail-tabs">
          <button class="detail-tab ${state.detailTab === "nutrition" ? "active" : ""}" data-tab="nutrition">Informação nutricional</button>
          <button class="detail-tab ${state.detailTab === "allergens" ? "active" : ""}" data-tab="allergens">Alergênicos</button>
        </div>
        <div id="detail-panel">${detailPanel(dish)}</div>
        <div class="detail-actions"><button class="primary-button" data-nav="scan">Escanear outro prato</button><button class="secondary-button" data-nav="survey">Avaliar experiência ${arrow}</button></div>
      </div>
    </div>`;
}

function surveyView() {
  const q = state.question;
  if (q >= susQuestions.length) {
    return `<div class="survey-wrap"><div class="question-card completion"><div class="completion-icon">✓</div><h2>Avaliação concluída</h2><p class="lead" style="margin-inline:auto">As respostas do questionário SUS ajudam a medir a percepção de usabilidade do protótipo.</p><button class="primary-button" data-nav="home">Voltar ao início</button></div></div>`;
  }
  const selected = state.answers[q];
  return `
    <div class="survey-wrap">
      <div class="survey-intro"><div><h1>Questionário SUS</h1><p>Avalie a usabilidade pensando na interação que acabou de realizar.</p></div><div class="progress-label">${q + 1} de ${susQuestions.length}<div class="progress-track"><div class="progress-fill" style="width:${(q + 1) * 10}%"></div></div></div></div>
      <div class="question-card"><span class="question-number">Questão ${q + 1}</span><h2>${susQuestions[q]}</h2>
        <div class="scale">${[1,2,3,4,5].map(n => `<button class="${selected === n ? "selected" : ""}" data-answer="${n}"><strong>${n}</strong><small>${n === 1 ? "Discordo" : n === 5 ? "Concordo" : ""}</small></button>`).join("")}</div>
        <div class="question-actions"><button class="secondary-button" id="prev-question" ${q === 0 ? "disabled" : ""}>${back} Anterior</button><button class="primary-button" id="next-question" ${selected ? "" : "disabled"}>${q === 9 ? "Concluir" : "Próxima"} ${arrow}</button></div>
      </div>
    </div>`;
}

function render() {
  view.innerHTML = state.route === "home" ? homeView() : state.route === "scan" ? scanView() : state.route === "detail" ? detailView() : surveyView();
}

document.addEventListener("click", event => {
  const nav = event.target.closest("[data-nav]");
  if (nav) { setRoute(nav.dataset.nav); return; }
  const dishCard = event.target.closest("[data-dish]");
  if (dishCard) { state.selectedDish = dishes.find(d => d.id === dishCard.dataset.dish); state.detailTab = "nutrition"; setRoute("detail"); return; }
  if (event.target.closest("#explore-menu")) { document.querySelector("#menu-section")?.scrollIntoView({ behavior: "smooth" }); }
  if (event.target.closest("#simulate-scan")) {
    const camera = document.querySelector("#camera"); const button = document.querySelector("#simulate-scan");
    camera.classList.add("scanning"); button.disabled = true; button.querySelector("span").textContent = "Reconhecendo...";
    setTimeout(() => { state.selectedDish = dishes[0]; state.detailTab = "nutrition"; setRoute("detail"); showToast("Etiqueta reconhecida com sucesso"); }, 2200);
  }
  const tab = event.target.closest("[data-tab]");
  if (tab) { state.detailTab = tab.dataset.tab; render(); }
  const answer = event.target.closest("[data-answer]");
  if (answer) { state.answers[state.question] = Number(answer.dataset.answer); render(); }
  if (event.target.closest("#next-question") && state.answers[state.question]) { state.question += 1; render(); }
  if (event.target.closest("#prev-question") && state.question > 0) { state.question -= 1; render(); }
});

document.addEventListener("keydown", event => {
  const card = event.target.closest?.("[data-dish]");
  if (card && (event.key === "Enter" || event.key === " ")) { event.preventDefault(); card.click(); }
});

const helpDialog = document.querySelector("#help-dialog");
document.querySelector("#help-button").addEventListener("click", () => helpDialog.showModal());
document.querySelector("#close-help").addEventListener("click", () => helpDialog.close());

function showToast(message) {
  const toast = document.createElement("div"); toast.className = "toast"; toast.textContent = message; document.body.appendChild(toast);
  requestAnimationFrame(() => toast.classList.add("show"));
  setTimeout(() => { toast.classList.remove("show"); setTimeout(() => toast.remove(), 300); }, 1800);
}

render();
