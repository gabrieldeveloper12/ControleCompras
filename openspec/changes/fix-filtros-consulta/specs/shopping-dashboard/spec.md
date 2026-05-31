## MODIFIED Requirements

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
