## Why

Atualmente, embora as correções do tema claro tenham sido finalizadas com sucesso, a interface móvel do ControleCompras apresenta limitações de usabilidade física que prejudicam a experiência tátil: botões e inputs possuem altura abaixo de 44px (o mínimo ideal recomendado pelo Apple HIG e Google Material Design), toques nos botões sofrem um atraso de 300ms em navegadores iOS devido ao double-tap zoom padrão, a fonte padrão do sistema carece de hierarquia visual e consistência entre sistemas operacionais, e o gráfico donut side-by-side fica excessivamente espremido em telas pequenas de 360px/480px. Esta proposta inicia a "Fase 1 (Design & Responsividade)" para elevar a experiência móvel a um patamar profissional de aplicativos nativos.

## What Changes

- **Otimização da Área de Toque (`min-height: 44px`)**: Aumento da altura mínima de todos os inputs, seletores e botões gerais para garantir conformidade estrita de ergonomia móvel.
- **Remoção de Latência de Toque (`touch-action: manipulation`)**: Adição de CSS especializado nos botões para eliminar o delay de toque de 300ms do iOS/Safari, tornando cliques instantâneos.
- **Tipografia Moderna (Outfit + Inter)**: Instalação e aplicação da tipografia Inter para corpo de texto, tabelas e inputs para máxima legibilidade móvel, mantendo a Outfit para cabeçalhos premium.
- **Breakpoint Específico para Telas Ultra-Pequenas (< 480px)**: Empilhamento vertical e ajuste de escala do Donut Chart e sua legenda, impedindo deformações de layout em telas de 360px.
- **Normalização de Textos de Formulário**: Ajuste de caixas de textos de E-MAIL, SENHA etc. de caixa alta para Sentence Case (`E-mail`, `Senha`) aumentando a legibilidade.

## Capabilities

### New Capabilities

### Modified Capabilities
- `shopping-dashboard`: Otimizar a tipografia, tamanhos de toque de formulários e empilhamento responsivo de gráficos em viewports ultra-pequenas.

## Impact

- **Frontend**: Instalação do pacote `@fontsource/inter`. Modificações de estilização global e responsiva em `style.css`, `App.vue` e `DashboardView.vue` para fontes, paddings, alturas mínimas, e viewport.
- **APIs**: Sem impacto.
- **Dependencies**: Adição de `@fontsource/inter` nas dependências do projeto.
