## Context

O ControleCompras está com as funcionalidades táteis concluídas, mas carece do visual premium definitivo sugerido pelo relatório de melhorias UI/UX. A paleta atual usa tons de cinza escuro quase pretos puros de baixo contraste que geram cansaço aos olhos. Adicionalmente, a tabela de compras se torna ilegível em larguras de tela de celulares menores que 640px. Este design planeja o refactoring do sistema de variáveis CSS e a reestruturação da lista de compras no celular para cartões individuais sem comprometer a integridade de rotas ou dados da API.

## Goals / Non-Goals

**Goals:**
* **Aplicação da Paleta HSL Índigo:** Substituir as cores de fundo, cards e bordas do `:root` por variáveis baseadas no matiz `238` (Índigo).
* **Tabela responsiva em Cards no Mobile (< 640px):** Substituir a tabela por um contêiner de cards individuais empilhados verticalmente no celular, mantendo a visualização de tabela no desktop.
* **Preservar Suporte a Temas:** Garantir que as alterações de cores no HSL suportem tanto o Dark Mode de 3 níveis quanto o Light Theme já existente sem quebras.

**Non-Goals:**
* Alterações de rotas, API ou Pinia stores.
* Modificação nos métodos CRUD ou validações já implementados na Fase 2.

## Decisions

### 1. Migração Baseada em Matiz HSL CSS
* **Decisão:** Mudar a variável `--hue-base` para `238` no `:root` e recalibrar as proporções de luminosidade/saturação para corresponder exatamente às cores `#12132a`, `#1e2040` e `#252641` do relatório.
* **Razão:** Permite alterar todo o visual do site mudando apenas 5 linhas de variáveis CSS de forma dinâmica, inteligente e extremamente leve, sem mexer nos arquivos de componentes individuais e preservando o suporte a temas.

### 2. Abordagem Híbrida de Layout CSS para Lista de Compras
* **Decisão:** Renderizar tanto a tabela quanto o contêiner de cards em `DashboardView.vue`, controlando a visibilidade de ambos via CSS Media Queries (`@media (max-width: 640px)`) com regras de `display: none` e `display: block/flex`.
* **Razão:** Utilizar media queries nativas no CSS é mais ágil, leve e evita overhead de Javascript ou listeners reativos de redimensionamento (`useBreakpoints`), eliminando trepidação visual ou quebras na renderização inicial do app (SSR/Hydration).

### 3. Estilização Glassmorphic para `.purchase-card`
* **Decisão:** Estilizar cada cartão de compra mobile `.purchase-card` utilizando o mesmo efeito de glassmorphism dos painéis do app (bordas translúcidas de contraste, backdrop-filter de blur e sombras suaves).
* **Razão:** Cria um apelo estético moderno de aplicativo de alta qualidade (premium), facilitando o toque em botões de ação tátil de 44px bem espaçados em coluna única.

## Risks / Trade-offs

* **[Risco]** Elementos redundantes na DOM (tabela e cards de compras renderizados juntos) podem aumentar a memória consumida pelo navegador.
  * **Mitigação:** Como a lista de compras em sessões comuns raramente passa de 100 registros antes de sofrer paginação ou filtro, o impacto na árvore DOM é irrelevante frente ao ganho de velocidade de renderização obtido ao evitar listeners Javascript de resize.
