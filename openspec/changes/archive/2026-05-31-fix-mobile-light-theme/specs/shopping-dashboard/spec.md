## ADDED Requirements

### Requirement: Opacidade de Ambient Orbs no Tema Claro
Quando o tema claro ("light") estiver ativo, os indicadores de ambientação luminosa de fundo (`body::before` e `body::after`) SHALL ter sua opacidade drasticamente reduzida para no máximo 8% (0.08 alpha) para garantir contraste tipográfico excelente e aparência de tema claro autêntico.

#### Scenario: Visualização do tema claro com ambient orbs sutis
- **WHEN** o usuário seleciona o tema "Claro"
- **THEN** o plano de fundo SHALL reduzir a opacidade das luzes de ambientação decorativas de fundo para evitar uma saturação escura/roxa de fundo.

### Requirement: Contraste de Botões Secundários no Tema Claro
Os botões secundários (`.btn-secondary`, como "Gerenciar Categorias", "Limpar Filtros" e "Cancelar") SHALL ter fundo de cor clara com contraste e legibilidade suficientes contra a cor de texto escuro sob o tema claro ("light") da aplicação.

#### Scenario: Contraste do botão secundário no tema claro
- **WHEN** o tema claro estiver ativo e um botão secundário for renderizado
- **THEN** a sua cor de texto SHALL possuir excelente contraste com um fundo claro de botão para perfeita leitura.

### Requirement: Variáveis Semânticas do Cabeçalho no Tema Claro
Os elementos visuais do cabeçalho (incluindo status da API, botão de engrenagem, avatar e hovers do dropdown de temas) SHALL utilizar variáveis semânticas CSS (`var(--surface-X)`) no lugar de opacidades estáticas brancas, garantindo contrastes e realces de hover visíveis no tema claro.

#### Scenario: Hover sobre opção do dropdown no tema claro
- **WHEN** o usuário passa o mouse ou toca em uma opção de tema no dropdown sob o tema claro
- **THEN** o fundo de destaque do hover SHALL ser visível usando uma matiz escura sutil sobre o fundo claro.

### Requirement: Alinhamento dos Dropdowns no Scroll do Celular
Os dropdowns de configurações de tema e do usuário no cabeçalho SHALL ser posicionados absolutamente em relação aos seus contêineres wrappers pais (`.settings-wrapper` e `.user-wrapper`), de forma que rolem nativa e perfeitamente junto com o cabeçalho em dispositivos móveis, sem flutuar soltos e fixos no meio da viewport.

#### Scenario: Rolagem da página com dropdown aberto no celular
- **WHEN** o usuário abre um dropdown no cabeçalho e rola a tela
- **THEN** o painel de dropdown SHALL manter o alinhamento perfeito com o seu respectivo ícone acionador e rolar suavemente com a página.
