## Why

Embora as fases anteriores tenham consolidado o visual premium com a paleta HSL Índigo e o layout de cartões para dispositivos móveis, o aplicativo ControleCompras ainda carece de funcionalidades de usabilidade fundamentais apontadas no relatório de auditoria UI/UX. Para atingir 100% de conformidade, é necessária a introdução de busca por descrição com debounce, ordenação de registros por cabeçalho/categoria, transição fluida com skeleton loaders durante o carregamento de dados e substituição das caixas de diálogo nativas do navegador por modais customizados integrados ao design premium.

## What Changes

- **Barra de Busca com Debounce:** Inclusão de um campo de pesquisa inline acima da lista de compras para filtrar registros por descrição, utilizando uma lógica de debounce de 300ms para poupar ciclos de renderização.
- **Ordenação de Registros Multicolunas:** Implementação de ordenação ascendente/descendente interativa na tabela e nos cards ao clicar nos cabeçalhos (Descrição, Categoria, Data e Valor).
- **Modal de Confirmação Glassmorphic:** Substituição das funções `confirm()` nativas por modais customizados de confirmação ao deletar compras e categorias, assegurando uma identidade visual uniforme.
- **Skeleton Screen Loaders:** Adição de animações de skeleton loading nos cards de estatísticas e na lista de lançamentos durante a consulta inicial à API para evitar quebras visuais e trepidações de layout.

## Capabilities

### New Capabilities
<!-- None -->

### Modified Capabilities
- `shopping-dashboard`: Atualizar as especificações de registros e tabelas de compras para incorporar busca com debounce, cabeçalhos de ordenação interativos, modais de confirmação customizados de toque e skeleton loaders.

## Impact

- **Frontend:** Alteração dos estilos CSS e do template/scripts reativos no componente `DashboardView.vue`.
- **APIs & Dependencies:** Sem alteração ou impacto em contratos de rotas ou novas bibliotecas externas.
