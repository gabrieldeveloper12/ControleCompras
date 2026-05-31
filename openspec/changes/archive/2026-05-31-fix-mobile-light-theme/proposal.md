## Why

Atualmente, o tema claro da aplicação "ControleCompras" apresenta sérias falhas de contraste, usabilidade e comportamento em dispositivos móveis. Elementos cruciais da interface (como botões secundários) ficam ilegíveis (texto escuro sob fundo escuro), as esferas luminosas de ambientação continuam excessivamente intensas em fundo claro, e os dropdowns de configurações do cabeçalho "flutuam" de forma desalinhada no meio da tela ao rolar a página no celular (devido a um Teleport com posição fixa estática). Esta alteração visa unificar a experiência responsiva e garantir que o tema claro seja tão fluido, legível e premium quanto o tema escuro original.

## What Changes

- **Ajuste da Opacidade dos Ambient Orbs no Tema Claro**: Redução da intensidade das esferas decorativas de fundo (`body::before` e `body::after`) de 25% para 6-8%, impedindo manchas escuras no fundo claro e otimizando o contraste de leitura.
- **Correção de Legibilidade dos Botões Secundários**: Aplicação de um fundo claro adequado aos botões secundários (`.btn-secondary`) quando o tema claro estiver ativo, evitando o contraste nulo (texto escuro sobre fundo escuro).
- **Substituição de Cores Estáticas por CSS Variables no Cabeçalho**: Atualização dos botões e painéis de `Header.vue` (como `.api-status`, `.settings-btn` e `.user-avatar` e hovers do dropdown) para usar variáveis semânticas CSS (`var(--surface-X)`), corrigindo hovers invisíveis e contrastes "branco-no-branco".
- **Refatoração dos Dropdowns de Posição JS/Fixed para CSS/Absolute**: Remoção do `Teleport` e da lógica de posicionamento calculada via JS. Os dropdowns do cabeçalho passarão a ser filhos com posicionamento absoluto em relação aos seus respectivos wrappers (`.settings-wrapper` e `.user-wrapper`), rolando de forma natural e responsiva no celular.

## Capabilities

### New Capabilities
<!-- Capabilities being introduced. Replace <name> with kebab-case identifier (e.g., user-auth, data-export, api-rate-limiting). Each creates specs/<name>/spec.md -->

### Modified Capabilities
<!-- Existing capabilities whose REQUIREMENTS are changing (not just implementation).
     Only list here if spec-level behavior changes. Each needs a delta spec file.
     Use existing spec names from openspec/specs/. Leave empty if no requirement changes. -->
- `shopping-dashboard`: Otimizar os requisitos de contraste e usabilidade no tema claro (Light Theme) em resoluções móveis, incluindo a ancoragem e comportamento de rolagem dos menus flutuantes de opções.

## Impact

- **Frontend**: Modificações em `c:\ControleCompras\frontend\src\style.css`, `c:\ControleCompras\frontend\src\components\Header.vue`, `c:\ControleCompras\frontend\src\App.vue` e `c:\ControleCompras\frontend\src\views\DashboardView.vue` para correções de variáveis CSS, responsividade e posicionamento de elementos.
- **APIs**: Sem impacto.
- **Dependencies**: Sem novas dependências.
