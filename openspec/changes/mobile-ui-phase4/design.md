## Context

O frontend do ControleCompras está com a base visual premium e a tabela de cartões móveis concluídas. No entanto, para atingir o estado da arte de usabilidade recomendado nos relatórios de auditoria, o gerenciamento de registros carece de busca debounced, ordenação interativa de colunas, transições suaves via skeleton loading e uma alternativa elegante à caixa de diálogo `confirm()` nativa do browser. Este documento desenha a implementação puramente reativa dessas melhorias, maximizando performance e aderência estética sem introduzir bibliotecas externas pesadas.

## Goals / Non-Goals

**Goals:**
* **Busca Reativa Debounced (300ms):** Campo de busca inline acima dos registros filtrando descrições sem lag visual.
* **Ordenação Interativa:** Ordenar dados de forma ascendente e descendente (Descrição, Categoria, Data e Valor) na tabela e nos cartões móveis.
* **Skeleton Screen Loader:** Substituir loaders genéricos por animações de pulso HSL nos cartões de estatísticas e na lista de lançamentos durante a consulta inicial à API.
* **Modal Customizado Glassmorphic:** Exibir confirmações de exclusão de compras e categorias em modais premium via `<Teleport>` do Vue 3.

**Non-Goals:**
* Alterações em contratos de rotas na API backend do ASP.NET.
* Adição de pacotes npm externos (como lodash ou bibliotecas de UI pesadas).
* Reescrita da persistência de categorias ou dados locais do Pinia.

## Decisions

### 1. Busca Inline Reativa com Debounce Nativo
* **Decisão:** Associar o input de pesquisa a uma propriedade reativa `searchQuery` no estado local. Utilizar um watcher para capturar mudanças em `searchQuery` e, após um `setTimeout` de 300ms (cancelando o timer anterior se houver), repassar o valor para `debouncedSearchQuery`, o qual alimentará o computed `filteredCompras`.
* **Razão:** Evita a inclusão de dependências como `lodash.debounce` mantendo o bundle leve e garante que a lista só recalcule em momentos de repouso da digitação do usuário, economizando ciclos de renderização no celular.

### 2. Ordenação Client-Side via Computed Property
* **Decisão:** Inserir duas variáveis reativas de controle: `sortKey` (coluna ativa) e `sortAsc` (direção, booleano). O computed `filteredCompras` aplicará a ordenação após os filtros temporais e de busca. O clique nas `<th>` da tabela e um botão de ação nos cartões mobile alterará estas chaves.
* **Razão:** Processamento no cliente é instantâneo e perfeitamente escalável para listas de até centenas de registros, poupando requisições repetitivas ao servidor.

### 3. Skeleton Loading em CSS Puro
* **Decisão:** Adicionar uma animação de pulso `@keyframes skeletonPulse { 0% { opacity: 0.6; } 50% { opacity: 0.3; } 100% { opacity: 0.6; } }` aplicada a elementos de placeholder `.skeleton-block`. Estes blocos substituirão visualmente os valores e linhas de compras enquanto a propriedade `isLoading` estiver ativa durante o fetching à API.
* **Razão:** Elimina trepidações e quebras de layout repentinas (layout shifts) durante a inicialização, gerando a percepção de um carregamento muito mais veloz.

### 4. Confirmação Exclusiva via Vue `<Teleport>`
* **Decisão:** Criar um modal de confirmação no template do `DashboardView.vue` estilizado com glassmorphism (backdrop blur de 12px, borda contrastante HSL, botões táteis amplos) e renderizá-lo anexado à tag `body` através do componente nativo `<Teleport to="body">`.
* **Razão:** Garante o isolamento de empilhamento de z-index (eliminando quebras com elementos pais que possuam transformações tridimensionais ou overscrolling) e eleva a experiência visual alinhada às diretrizes da Apple HIG.

## Risks / Trade-offs

* **[Risco]** Aumento na complexidade de lógica reativa e variáveis de estado local no componente `DashboardView.vue`.
  * **Mitigação:** Documentar os métodos e manter a reatividade isolada em computed properties focadas e separadas por contexto.
