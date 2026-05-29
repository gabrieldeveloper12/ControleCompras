## ADDED Requirements

### Requirement: Responsividade de Grades e Painéis
A interface do dashboard SHALL utilizar um design fluído baseado em Media Queries CSS que reorganiza a grade em telas médias e pequenas, mantendo a integridade visual sem truncar textos ou estourar a largura do navegador.

#### Scenario: Visualização em Tablet
- **WHEN** a largura da tela estiver entre 600px e 900px
- **THEN** a barra lateral de estatísticas SHALL se reorganizar em uma grade horizontal de 3 colunas, e o gráfico donut SHALL se dispor lado a lado.

#### Scenario: Visualização em Celular Estrito
- **WHEN** a largura da tela for inferior a 600px
- **THEN** as estatísticas, o gráfico e os formulários SHALL se empilhar verticalmente de forma compacta para caber em uma única coluna vertical.

### Requirement: Adaptação de Formulários e Modais de Toque
Os formulários internos do painel e do modal de categorias SHALL se adaptar para digitação e toque fáceis em dispositivos móveis, redimensionando e empilhando os campos dinamicamente.

#### Scenario: Formulário de Categorias no Celular
- **WHEN** o modal de gerenciamento de categorias é aberto em uma tela menor que 600px
- **THEN** o formulário de categoria `.form-row-cat` SHALL empilhar os campos de Nome, Ícone, Cor e Ações verticalmente de forma 100% responsiva.
