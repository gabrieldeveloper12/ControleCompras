## 1. Frontend: Ajustar Filtro e Busca de Compras

- [x] 1.1 Modificar `data()` em `DashboardView.vue` para inicializar `filtroDataInicio` e `filtroDataFim` como vazios (`''`).
- [x] 1.2 Atualizar o método `resetFilters()` em `DashboardView.vue` para limpar as datas de filtro de período (`''` ao invés de `getTodayDateString()`).
- [x] 1.3 Adicionar validação em `fetchCompras()` para enviar parâmetros de `inicio` e `fim` à API apenas se os mesmos possuírem valor real.

## 2. Frontend: Feedback de Período Ativo (Dynamic Text Card)

- [x] 2.1 Criar a computed property `periodoAtivoTexto` em `DashboardView.vue` que gera descrições amigáveis e precisas do filtro em pt-BR.
- [x] 2.2 Inserir a visualização do card de status no template HTML de `DashboardView.vue` logo abaixo da área de seleção de filtros utilizando o estilo glassmorphic.
- [x] 2.3 Adicionar estilização para `.filter-status-card` em `style.css` com bordas sutis e fundo semitransparente (estilo glassmorphism e cores HSL).
