## ADDED Requirements

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
