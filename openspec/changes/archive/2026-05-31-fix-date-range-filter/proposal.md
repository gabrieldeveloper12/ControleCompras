## Why

A experiência do usuário no filtro de intervalo de datas (período) apresenta barreiras de usabilidade e problemas de acurácia. A inicialização automática com a data atual e a falta de flexibilidade para filtrar períodos unilaterais (ex: compras a partir de uma data) confundem o usuário, que muitas vezes enxerga todos os registros mesmo após aplicar um filtro. Além disso, a falta de feedback visual sobre o período ativo aumenta a carga cognitiva.

## What Changes

- **Datas Iniciais Vazias**: Os campos de data inicial e final no filtro por intervalo começarão vazios, dando flexibilidade para filtros parciais ou unilaterais.
- **Filtros Unilaterais**: 
  - Se apenas a Data Inicial for preenchida, o sistema filtra compras a partir dessa data (sem limite final).
  - Se apenas a Data Final for preenchida, o sistema filtra compras até essa data (sem limite inicial).
- **Filtro de Dia Estrito**: Se ambas as datas forem preenchidas com o mesmo dia, exibe apenas as compras daquele dia.
- **Card de Período Ativo (Visual Feedback)**: Um card dinâmico no estilo glassmorphism que traduz em linguagem natural qual intervalo de tempo está sendo visualizado (ex: "Exibindo compras do dia 29/05/2026").

## Capabilities

### New Capabilities
<!-- None -->

### Modified Capabilities
- `shopping-dashboard`: Adiciona suporte a filtros unilaterais, inicialização vazia dos campos de data e feedback visual dinâmico em linguagem natural sobre o período selecionado.

## Impact

- **Frontend**: Alteração do `DashboardView.vue` nos campos de input de data, no método `fetchCompras`, no reset de filtros e inclusão de um novo computed property `periodoAtivoTexto` junto com sua representação visual no template HTML.
- **CSS**: Novos estilos CSS para o card de feedback dinâmico em `style.css`.
- **API Backend**: Nenhuma alteração de impacto direto, pois a API já foi corrigida e está pronta para receber e tratar parâmetros de data opcionais ou nulos.
