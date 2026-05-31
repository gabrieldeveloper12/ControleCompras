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
A tela principal SHALL disponibilizar um componente de filtro que permita ao usuário selecionar um mês e ano específicos, ou definir um período personalizado de datas (De/Até), atualizando os dados do painel instantaneamente. Os campos de data do filtro de período SHALL ser inicializados com a data atual do sistema ao carregar a página. Ao limpar os filtros, os campos SHALL retornar à data atual. A inicialização dos valores de data SHALL utilizar lógica inline compatível com o ciclo de vida do Vue Options API (sem chamadas a `this.methods` dentro de `data()`). O envio dos parâmetros de data para a API SHALL garantir que strings válidas no formato `YYYY-MM-DD` sejam transmitidas, nunca `undefined`, `null` ou strings vazias. Além disso, a tela principal SHALL exibir um card dinâmico no estilo glassmorphism que traduz em linguagem natural qual intervalo de tempo está sendo visualizado.

#### Scenario: Inicialização dos filtros de data com data atual
- **WHEN** o usuário acessa a página principal pela primeira vez
- **THEN** os campos "Data Inicial" e "Data Final" do filtro de período SHALL estar preenchidos com a data atual do sistema no formato `YYYY-MM-DD`.

#### Scenario: Reset dos filtros retorna à data atual
- **WHEN** o usuário clica em "Limpar Filtros"
- **THEN** o filtro SHALL retornar ao modo "Mês Específico" com o mês/ano atual selecionado, e os campos "Data Inicial" e "Data Final" SHALL ser redefinidos para a data atual.

#### Scenario: Filtragem por mês específico
- **WHEN** o usuário altera o filtro de mês para "Maio/2026"
- **THEN** a interface SHALL atualizar a lista de compras e os gráficos para mostrar apenas os dados relativos a maio de 2026.

#### Scenario: Filtro por intervalo de datas com ambas as datas preenchidas
- **WHEN** o usuário define "Data Inicial" como 01/05/2026 e "Data Final" como 31/05/2026
- **THEN** a interface SHALL enviar os parâmetros `inicio=2026-05-01&fim=2026-05-31` à API e exibir apenas compras dentro desse intervalo.

#### Scenario: Filtro estrito de dia único
- **WHEN** o usuário define tanto a "Data Inicial" quanto a "Data Final" como 29/05/2026
- **THEN** a interface SHALL atualizar os dados para mostrar apenas compras do dia 29/05/2026, e o card de status SHALL exibir "Exibindo compras do dia 29/05/2026".

#### Scenario: Preservação do dia ao salvar compra independente do fuso horário
- **WHEN** o usuário seleciona a data "31/05/2026" no formulário de cadastro de compra e clica em salvar, estando em qualquer fuso horário (ex: UTC-3 Brasil)
- **THEN** a compra SHALL ser armazenada com a data 2026-05-31 no banco de dados, sem deslocamento de dia por conversão de timezone.

#### Scenario: Valores de filtro nunca são undefined ou null
- **WHEN** a página é carregada e o componente Vue é montado
- **THEN** as propriedades `filtroDataInicio`, `filtroDataFim` e `formCompra.data` SHALL conter strings válidas no formato `YYYY-MM-DD`, nunca `undefined`, `null` ou string vazia.

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

### Requirement: Dicas e Validação Inline de Inputs
Todos os campos do formulário de lançamento de compra SHALL exibir um texto de ajuda (hint/placeholder descritivo) abaixo do campo detalhando o formato esperado (ex: "ex: Supermercado", "ex: R$ 29,90") e aplicar estilização de validação inline instantânea (borda verde para entrada válida e borda vermelha para entrada inválida ou vazia).

#### Scenario: Preenchimento de campo de descrição válido
- **WHEN** o usuário digita uma descrição com 3 ou mais caracteres no campo correspondente
- **THEN** a caixa de texto SHALL exibir uma borda verde indicando validação de sucesso.

#### Scenario: Preenchimento de valor inválido
- **WHEN** o usuário deixa o campo de valor vazio ou digita zero/número negativo
- **THEN** o campo de valor SHALL exibir uma borda vermelha indicando estado de erro visual.

### Requirement: Prevenção de Envio Duplo e Feedback de Envio
O botão de submissão do formulário de compras ("Salvar") SHALL ter seu estado alterado para `disabled` (desabilitado para cliques) e exibir uma mensagem de processamento (como "Salvando..." acompanhada de um ícone spinner) enquanto a requisição de API estiver pendente, eliminando cliques repetitivos e registros duplicados.

#### Scenario: Clicando em salvar formulário válido
- **WHEN** o usuário clica no botão de salvar com dados corretos no formulário
- **THEN** o botão de salvar SHALL se desabilitar imediatamente e exibir "Salvando..." com um spinner animado até a resposta do servidor.

### Requirement: Componente de Alertas Toast Dinâmicos
A aplicação SHALL disponibilizar um sistema de notificações flutuantes (Toast Notifications) posicionado estrategicamente na tela (canto inferior direito no desktop e barra inferior cheia no mobile) para emitir alertas reativos instantâneos de sucesso, erro ou aviso nas ações de criar, editar ou excluir compras e categorias, com fechamento automático após 3 segundos.

#### Scenario: Notificação de sucesso ao cadastrar compra
- **WHEN** uma nova compra é adicionada com sucesso no sistema
- **THEN** um toast de sucesso com a mensagem "Compra cadastrada com sucesso!" SHALL surgir na tela e desaparecer automaticamente após 3 segundos.

### Requirement: Tooltips Interativos e Entrada Suave no Donut Chart
O gráfico donut de categorias SHALL possuir uma animação de entrada suave ao carregar a página (crescimento radial das fatias utilizando o atributo `stroke-dasharray` transicionando de zero ao valor final) e expor um painel flutuante de informações (Tooltip) reativo ao passar o cursor ou tocar (touch) em cada segmento, exibindo o nome da categoria, valor absoluto em R$ e o percentual correspondente.

#### Scenario: Hover ou toque em um segmento do gráfico
- **WHEN** o usuário posiciona o cursor ou toca em um segmento colorido do Donut Chart
- **THEN** um tooltip flutuante SHALL surgir imediatamente na tela contendo as informações legíveis de nome da categoria, valor total e porcentagem daquele grupo de gastos.

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

### Requirement: Cabeçalho com Configurações e Avatar do Usuário
O cabeçalho principal SHALL conter uma seção à direita agrupando o status de conexão da API, um botão de engrenagem (`⚙️`) interativo para configurações rápidas, e um avatar circular que simula o perfil do usuário logado contendo dicas acessíveis (ex: "Gabriel Ramos").

#### Scenario: Rotação interativa da engrenagem
- **WHEN** o usuário passa o mouse (hover) sobre o botão de engrenagem no cabeçalho
- **THEN** a engrenagem SHALL rotacionar suavemente em 180 graus e destacar sua cor utilizando o brilho primário.

#### Scenario: Integração do botão de engrenagem com modal de categorias
- **WHEN** o usuário clica no botão de engrenagem no cabeçalho
- **THEN** a interface SHALL disparar um evento customizado que abre o modal de gerenciamento de categorias na tela principal para centralizar as configurações do dashboard.

