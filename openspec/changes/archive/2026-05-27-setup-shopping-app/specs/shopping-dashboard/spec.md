## ADDED Requirements

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
O front-end SHALL disponibilizar um formulário intuitivo para inserção de novas compras, com validações em tempo de digitação (descrição obrigatória, valor numérico positivo maior que zero, seleção de uma categoria).

#### Scenario: Envio de formulário válido
- **WHEN** o usuário preenche o formulário de compra corretamente e clica em "Salvar"
- **THEN** o front-end SHALL enviar os dados para a API, limpar o formulário e atualizar os dados do painel/lista na tela.
