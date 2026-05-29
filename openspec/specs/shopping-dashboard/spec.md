# shopping-dashboard Specification

## Purpose
TBD - created by archiving change setup-shopping-app. Update Purpose after archive.
## Requirements
### Requirement: Painel de Dashboard com Gráficos
O front-end em Vue 3 SHALL possuir um painel de dashboard dinâmico que exiba o total de gastos acumulados e um gráfico comparativo (ex: rosca ou pizza) mostrando a distribuição percentual e em valor dos gastos por categoria.

#### Scenario: Visualização do dashboard com compras cadastradas
- **WHEN** o usuário acessa a página principal e existem compras salvas
- **THEN** a tela SHALL calcular a soma total e exibir o gráfico de gastos correspondente dividido pelas categorias.

### Requirement: Filtro Temporal Dinâmico
A tela principal SHALL disponibilizar um componente de filtro que permita ao usuário selecionar um mês e ano específicos, ou definir um período personalizado de datas (De/Até), atualizando os dados do painel instantaneamente.

#### Scenario: Filtragem por mês específico
- **WHEN** o usuário altera o filtro de mês para "Maio/2026"
- **THEN** a interface SHALL atualizar a lista de compras e os gráficos para mostrar apenas os dados relativos a maio de 2026.

### Requirement: Formulário de Lançamento de Compra
O front-end SHALL disponibilizar um formulário intuitivo para inserção de novas compras, com validações em tempo de digitação (descrição obrigatória, valor numérico positivo maior que zero, seleção de uma categoria). O campo "Valor" SHALL aceitar entradas numéricas tolerantes (usando vírgula ou ponto decimal), exibir a formatação com duas casas decimais brasileiras (ex: "2,00") ao perder o foco (blur), e expor o teclado numérico decimal nativo do celular utilizando o atributo `inputmode="decimal"`. Ao clicar em editar uma compra existente, a interface SHALL realizar uma rolagem suave focando o formulário para feedback imediato do usuário, e o registro que está sendo editado SHALL ser temporariamente ocultado da tabela de exibição de compras.

#### Scenario: Envio de formulário válido
- **WHEN** o usuário preenche o formulário de compra corretamente e clica em "Salvar"
- **THEN** o front-end SHALL normalizar o campo "Valor" substituindo qualquer vírgula por ponto antes de transmitir à API, enviar os dados, limpar o formulário e atualizar os dados do painel.

#### Scenario: Formatação de inteiro ao desfocar
- **WHEN** o usuário digita "2" no campo "Valor" e desfoca o campo
- **THEN** o campo SHALL formatar o valor exibido para "2,00".

#### Scenario: Formatação de decimal com vírgula ao desfocar
- **WHEN** o usuário digita "2,5" no campo "Valor" e desfoca o campo
- **THEN** o campo SHALL formatar o valor exibido para "2,50".

#### Scenario: Remoção de formatação ao focar o campo
- **WHEN** o usuário foca o campo "Valor" que possui o valor "2,50"
- **THEN** o campo SHALL exibir o valor cru editável "2,50" (ou "2.5") para facilitar a alteração pelo usuário sem lutar contra caracteres extras.

#### Scenario: Rolagem automática suave ao clicar em editar
- **WHEN** o usuário clica no botão de edição de uma compra na tabela de registros
- **THEN** a interface SHALL preencher os dados no formulário e rolar suavemente a página até a caixa de texto de descrição do formulário.

#### Scenario: Ocultar item em edição na tabela
- **WHEN** a interface entra em modo de edição de uma compra (editMode ativo e ID da compra carregado)
- **THEN** a linha da tabela correspondente a essa compra específica SHALL ser ocultada da tabela de registros de compras.

### Requirement: Responsividade de Grades e Painéis
A interface do dashboard SHALL utilizar um design fluído baseado em Media Queries CSS que reorganiza a grade em telas médias e pequenas, mantendo a integridade visual sem truncar textos ou estourar a largura do navegador.

#### Scenario: Visualização em Tablet
- **WHEN** a largura da tela estiver entre 600px e 900px
- **THEN** a barra lateral de estatísticas SHALL se reorganizar em uma grade horizontal de 3 colunas, e o gráfico donut SHALL se dispor lado a lado.

#### Scenario: Visualização em Celular Estrito
- **WHEN** a largura da tela for inferior a 600px
- **THEN** as estatísticas, o gráfico e os formulários SHALL se empilhar verticalmente de forma compacta para caber em uma única coluna vertical.

### Requirement: Adaptação de Formulários e Modais de Toque
Os formulários internos do painel e do modal de categorias SHALL se adaptar para digitação e toque fáceis em dispositivos móveis, redimensionando e empilhando os campos dinamicamente. Além disso, todos os indicadores visuais de seletores de data e mês SHALL possuir excelente contraste e legibilidade sob o tema dark mode da aplicação, utilizando inversão de cores e efeitos de foco.

#### Scenario: Formulário de Categorias no Celular
- **WHEN** o modal de gerenciamento de categorias é aberto em uma tela menor que 600px
- **THEN** o formulário de categoria `.form-row-cat` SHALL empilhar os campos de Nome, Ícone, Cor e Ações verticalmente de forma 100% responsiva.

#### Scenario: Contraste de ícones de data no Dark Mode
- **WHEN** um campo de entrada de data ou mês é renderizado na tela principal
- **THEN** o indicador visual do seletor (calendário) SHALL possuir um filtro de cor invertida (claro) para garantir legibilidade contra o fundo escuro da caixa de entrada.

