## 1. Busca Inline com Debounce

- [x] 1.1 Adicionar propriedades reativas `searchQuery` e `debouncedSearchQuery` ao `data()` em `DashboardView.vue`
- [x] 1.2 Implementar watcher `searchQuery` com `setTimeout`/`clearTimeout` para debounce de 300ms atualizando `debouncedSearchQuery`
- [x] 1.3 Adicionar o campo de busca `.search-bar` no template acima da seção de lista de compras
- [x] 1.4 Atualizar o computed `filteredCompras` para filtrar resultados por `debouncedSearchQuery` (case-insensitive na descrição) combinado com o filtro temporal existente

## 2. Ordenação Interativa de Registros

- [x] 2.1 Adicionar propriedades reativas `sortKey` (string) e `sortAsc` (booleano) ao `data()` em `DashboardView.vue`
- [x] 2.2 Criar método `sortBy(key)` que alterna `sortAsc` quando a chave é a mesma ou redefine ao mudar de coluna
- [x] 2.3 Atualizar o computed `filteredCompras` para aplicar a ordenação com `Array.prototype.sort()` após os filtros de busca e período
- [x] 2.4 Adicionar botões de cabeçalho de coluna clicáveis (`@click="sortBy('...')"`) com indicadores ▲/▼ no `<thead>` da tabela desktop
- [x] 2.5 Estilizar os cabeçalhos de ordenação e indicadores de direção em `style.css`

## 3. Modal Customizado Glassmorphic de Confirmação

- [x] 3.1 Adicionar propriedades de estado `confirmModal` (objeto com `show`, `message`, `resolve`) ao `data()` em `DashboardView.vue`
- [x] 3.2 Criar método `showConfirm(message)` que retorna uma Promise resolvida pela ação do usuário no modal
- [x] 3.3 Substituir as chamadas de `confirm()` nos métodos `deleteCompra()` e `deleteCategoria()` pelo novo `await showConfirm(...)`
- [x] 3.4 Adicionar o template do modal de confirmação usando `<Teleport to="body">` com glassmorphism (backdrop blur, botões táteis de 44px)
- [x] 3.5 Estilizar o modal customizado `.confirm-modal-overlay` e `.confirm-modal-box` em `style.css`

## 4. Skeleton Screen Loaders

- [x] 4.1 Adicionar propriedade reativa `isLoading` (booleano) ao `data()` em `DashboardView.vue`, iniciando em `true` e definindo como `false` após o primeiro fetch completar
- [x] 4.2 Adicionar blocos placeholder de skeleton (`.skeleton-block`) nos cards de estatísticas usando `v-if/v-else` guiados por `isLoading`
- [x] 4.3 Adicionar blocos skeleton na seção da lista de compras substituindo a tabela/cards durante o loading inicial
- [x] 4.4 Estilizar a animação de pulso `.skeleton-block` com `@keyframes skeletonPulse` em `style.css` usando as variáveis HSL do tema

## 5. Validação de Build

- [x] 5.1 Executar `npm run build` em `frontend/` e garantir que o build compila com 0 erros ou avisos críticos
