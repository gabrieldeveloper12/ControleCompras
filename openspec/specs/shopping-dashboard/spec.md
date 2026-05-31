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

### Requirement: Área de Toque Mínima de 44px
Todos os botões (`.btn`, `.action-btn`), caixas de entrada de texto (`.input-control`) e seletores de categoria (`.select-control`) SHALL ter uma altura mínima garantida de 44px em resoluções móveis de toque para assegurar total facilidade de uso físico (conforme HIG e Material Design).

#### Scenario: Elemento interativo renderizado no celular
- **WHEN** um botão ou caixa de entrada é desenhado em tela
- **THEN** a sua área de toque total (incluindo paddings e bordas) SHALL medir pelo menos 44px de altura.

### Requirement: Latência de Clique no Mobile
Todos os botões clicáveis da aplicação (tabela de compras, cabeçalho e formulário) SHALL possuir o comportamento de toque instantâneo desativando o delay nativo de 300ms de duplo clique em navegadores iOS/Safari.

#### Scenario: Toque instantâneo no botão de ação da tabela
- **WHEN** o usuário toca em um botão de ação rápida de edição ou exclusão no celular
- **THEN** o navegador móvel SHALL registrar o evento de clique de forma instantânea sem qualquer delay de 300ms.

### Requirement: Escala e Breakpoint do Donut Chart
Em larguras de tela de celulares de até 480px, a área de exibição do gráfico donut de categoria SHALL limitar o tamanho máximo do círculo em 200px e forçar a legenda a se empilhar perfeitamente abaixo dele em uma única coluna.

#### Scenario: Gráfico visualizado em tela de 360px
- **WHEN** a página é aberta em uma viewport física ultra-pequena
- **THEN** o gráfico donut SHALL ter no máximo 200px de diâmetro e a legenda SHALL ficar empilhada verticalmente de forma legível.

### Requirement: Tipografia Premium Inter
Toda a tipografia de corpo de texto, tabelas, rótulos e caixas de entrada SHALL usar a família de fontes Inter para máxima nitidez em telas de baixa densidade (Android mid-range).

#### Scenario: Texto de tabela exibido na tela
- **WHEN** o usuário lê um registro de compra na tabela ou visualiza uma label
- **THEN** a tipografia exibida SHALL ser da família 'Inter' com pesos bem definidos.

### Requirement: Sentencing de Labels de Formulário
Todos os rótulos de caixas de entrada do formulário de autenticação (como login, cadastro e esqueci a senha) SHALL ser escritos em formato de frase com apenas a primeira letra maiúscula e possuir espaçamento de letras sutil.

#### Scenario: Visualização do formulário de login
- **WHEN** a tela de login é exibida
- **THEN** as labels dos inputs SHALL ser exibidas como "E-mail" e "Senha" em vez de caixas altas completas.

### Requirement: Paleta de Cores Premium HSL Índigo
A estilização global do aplicativo SHALL implementar um sistema profissional de cores baseado em tons de azul-escuro índigo com 3 níveis de profundidade utilizando variáveis CSS baseadas no matiz HSL `238` para redução extrema de fadiga ocular em sessões de uso longas.

#### Scenario: Visualização do tema escuro premium
- **WHEN** o aplicativo é carregado no tema escuro padrão
- **THEN** o fundo do app SHALL ser `hsl(238, 40%, 12%)` (#12132a), os cards `hsla(238, 36%, 18% / 0.65)` (#1e2040) e as bordas `hsla(238, 30%, 25% / 0.15)` (#2e2e55).

### Requirement: Visualização de Compras em Cards no Mobile
Em larguras de telas de celulares inferiores a 640px, a lista de lançamentos de compras SHALL ocultar completamente a tabela HTML tradicional e renderizar os dados em formato de cartões individuais (.purchase-card) verticais e ergonômicos para toques rápidos.

#### Scenario: Visualizando lançamentos no celular
- **WHEN** a largura de tela do navegador for menor que 640px
- **THEN** a tabela de registros SHALL ficar oculta e cada compra SHALL ser exibida como um card tátil contendo descrição, valor BRL realçado, tag da categoria com emoji, data e botões de ação (editar/deletar) com área de toque mínima de 44px.

