## Context

O tema claro ("light") do aplicativo de ControleCompras possui graves falhas de visibilidade e usabilidade, especialmente quando acessado em dispositivos móveis (celulares). Os problemas identificados são decorrentes de decisões de estilização estática para o tema escuro original (Dark Mode) que não foram devidamente adaptadas ou portadas quando a chave do tema claro foi implementada:
- A opacidade de 25% das esferas decorativas roxas e cianas no fundo (`body::before` e `body::after`) gera manchas escuras pesadas no fundo cinza-claro, descaracterizando a paleta clara.
- Os botões secundários apresentam contraste nulo (texto quase preto sob fundo cinza escuro de 40% de preto semitransparente).
- Componentes como cabeçalho, avatar do usuário, indicador de status e dropdowns utilizam opacidades estáticas brancas (`rgba(255,255,255,0.05)`) herdadas do dark mode, fazendo com que fiquem brancos-no-branco (invisíveis) ou sem feedback de hover.
- O dropdown de configurações e usuário é teleportado para o `<body>` com coordenadas pixel fixadas em tela via JS. Ao efetuar rolagem em telas pequenas, o menu permanece preso no meio da janela do navegador enquanto o cabeçalho rola, gerando uma quebra severa de layout.

## Goals / Non-Goals

**Goals:**
- Garantir que o tema claro seja visualmente legível, premium e confortável no celular, com excelente contraste de texto.
- Corrigir a sobreposição escura dos ambient orbs no tema claro.
- Garantir que os dropdowns de configurações do cabeçalho acompanhem perfeitamente a rolagem nativa da tela no celular e fiquem ancorados de forma confiável aos botões acionadores.
- Eliminar o código de cálculo dinâmico de coordenadas JS em favor de um posicionamento CSS nativo e robusto.

**Non-Goals:**
- Alterar as variáveis do tema escuro padrão.
- Adicionar novos temas (como alto contraste) ou novos componentes.
- Redesenhar a lógica de funcionamento ou cálculos do painel de controle.

## Decisions

### Decisão 1: Redução da Opacidade dos Ambient Orbs no Tema Claro
No arquivo `style.css`, na regra seletora de `[data-theme="light"]`, sobrescreveremos a opacidade das cores de brilho decorativo para evitar manchas pesadas no fundo claro:
```css
[data-theme="light"] {
  /* ... variáveis existentes ... */
  --primary-glow: hsla(263 80% 62% / 0.06);
  --secondary-glow: hsla(192 90% 50% / 0.06);
}
```
*Raciocínio:* Manter o efeito premium sutil de esferas de fundo sem escurecer ou sujar o design claro da página.

### Decisão 2: Estilização do Botão Secundário no Tema Claro
Sobrescreveremos a cor de fundo do botão secundário no tema claro para garantir excelente legibilidade:
```css
[data-theme="light"] .btn-secondary {
  background: var(--surface-3);
  border-color: var(--border-glass);
}

[data-theme="light"] .btn-secondary:hover {
  background: var(--surface-4);
}
```
*Raciocínio:* Utilizar as variáveis de superfície do próprio tema claro para garantir que o fundo do botão fique muito sutil e contraste adequadamente com `var(--text-primary)` (quase preto).

### Decisão 3: Refatoração de Dropdowns em Posicionamento Absoluto CSS
Substituiremos o `Teleport` e a lógica de cálculo via JS em `Header.vue` por um aninhamento puro estruturado com CSS absoluto:
- Remover as tags `<Teleport to="body">` de ambos os dropdowns de configurações de tema e usuário.
- Deixar a estrutura HTML do `<transition>` e da `.theme-dropdown` diretamente dentro dos seus wrappers pais (`.settings-wrapper` e `.user-wrapper`), que já possuem a propriedade `position: relative`.
- Ajustar os estilos CSS de `.theme-dropdown` para:
```css
.theme-dropdown {
  position: absolute;
  top: calc(100% + 10px);
  right: 0;
  z-index: 99999;
  width: 175px;
  /* ... restante dos estilos mantido ... */
}
```
- Excluir os métodos de ciclo de vida e JS `updateDropdownPosition()`, `updateUserDropdownPosition()`, e as variáveis reativas `dropdownStyle`, `userDropdownStyle`.
*Raciocínio:* CSS absoluto nativo é robusto, elimina retrabalho de cálculo e corrige o bug de "flutuação aérea" ao rolar a página no celular.

### Decisão 4: Utilização de CSS Variables para Itens do Cabeçalho e Hovers
Atualizar os estilos de `Header.vue` substituindo os fundos estáticos brancos por variáveis semânticas CSS:
- `.api-status`: `background: var(--surface-1)` ou `var(--surface-2)` (em vez de `rgba(255,255,255,0.03)`).
- `.settings-btn`: `background: var(--surface-2)` (em vez de `rgba(255,255,255,0.05)`).
- `.settings-btn:hover`: `background: var(--surface-3)`.
- `.user-avatar`: `background: var(--surface-3)`.
- `.theme-option:hover`: `background: var(--surface-4)` (em vez de `rgba(255,255,255,0.06)`).

## Risks / Trade-offs

- **[Risco] clique fora com dropdown absoluto** → *Mitigação*: O manipulador `handleOutsideClick` continuará a funcionar sem problemas. Adaptaremos a lógica dele para checar se o clique ocorreu fora dos wrappers pais `.settings-wrapper` e `.user-wrapper`, que encapsulam agora tanto o botão quanto o respectivo dropdown.
