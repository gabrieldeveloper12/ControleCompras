## 1. Estilos e Contraste do Tema Claro (style.css)

- [x] 1.1 Reduzir a opacidade de `--primary-glow` e `--secondary-glow` para 0.06 (6%) em `style.css` sob o seletor `[data-theme="light"]`
- [x] 1.2 Sobrescrever a cor de fundo de `.btn-secondary` no tema claro (`[data-theme="light"] .btn-secondary`) usando `--surface-3` e o hover usando `--surface-4` para legibilidade premium de texto

## 2. Refatoração de Dropdowns e Estilos do Cabeçalho (Header.vue)

- [x] 2.1 Remover a tag `<Teleport to="body">` dos dropdowns de tema e do usuário no HTML de `Header.vue`
- [x] 2.2 Atualizar o estilo de `.theme-dropdown` em `Header.vue` de `position: fixed` para `position: absolute`, adicionando `top: calc(100% + 10px)` e `right: 0`
- [x] 2.3 Excluir no bloco script de `Header.vue` as funções de cálculo de posição `updateDropdownPosition()`, `updateUserDropdownPosition()` e as propriedades `dropdownStyle`, `userDropdownStyle`
- [x] 2.4 Ajustar a função `handleOutsideClick(event)` em `Header.vue` para fechar os menus quando o clique ocorrer fora dos contêineres `.settings-wrapper` e `.user-wrapper`
- [x] 2.5 Substituir fundos e hovers brancos estáticos (`rgba(255, 255, 255, X)`) por variáveis semânticas CSS (`var(--surface-X)`) nos seletores `.api-status`, `.settings-btn`, `.settings-btn:hover`, `.user-avatar` e `.theme-option:hover` em `Header.vue`

## 3. Sincronização de Estilos Globais Duplicados (App.vue e DashboardView.vue)

- [x] 3.1 Sincronizar e replicar as melhorias aplicadas aos botões secundários no tema claro (`[data-theme="light"] .btn-secondary`) nas tags `<style>` de `App.vue` e `DashboardView.vue`
