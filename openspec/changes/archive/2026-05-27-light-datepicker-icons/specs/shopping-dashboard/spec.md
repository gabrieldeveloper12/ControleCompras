## MODIFIED Requirements

### Requirement: Adaptação de Formulários e Modais de Toque
Os formulários internos do painel e do modal de categorias SHALL se adaptar para digitação e toque fáceis em dispositivos móveis, redimensionando e empilhando os campos dinamicamente. Além disso, todos os indicadores visuais de seletores de data e mês SHALL possuir excelente contraste e legibilidade sob o tema dark mode da aplicação, utilizando inversão de cores e efeitos de foco.

#### Scenario: Formulário de Categorias no Celular
- **WHEN** o modal de gerenciamento de categorias é aberto em uma tela menor que 600px
- **THEN** o formulário de categoria `.form-row-cat` SHALL empilhar os campos de Nome, Ícone, Cor e Ações verticalmente de forma 100% responsiva.

#### Scenario: Contraste de ícones de data no Dark Mode
- **WHEN** um campo de entrada de data ou mês é renderizado na tela principal
- **THEN** o indicador visual do seletor (calendário) SHALL possuir um filtro de cor invertida (claro) para garantir legibilidade contra o fundo escuro da caixa de entrada.
