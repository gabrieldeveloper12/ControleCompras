## MODIFIED Requirements

### Requirement: Filtro Temporal Dinâmico
A tela principal SHALL disponibilizar um componente de filtro que permita ao usuário selecionar um mês e ano específicos, ou definir um período personalizado de datas (De/Até) com suporte a buscas unilaterais ou parciais (deixando os campos em branco), atualizando os dados do painel instantaneamente. Além disso, a tela principal SHALL exibir um card dinâmico no estilo glassmorphism que traduz em linguagem natural qual intervalo de tempo está sendo visualizado (ex: "Exibindo compras do dia 29/05/2026").

#### Scenario: Filtragem por mês específico
- **WHEN** o usuário altera o filtro de mês para "Maio/2026"
- **THEN** a interface SHALL atualizar a lista de compras e os gráficos para mostrar apenas os dados relativos a maio de 2026.

#### Scenario: Filtro por data inicial apenas
- **WHEN** o usuário seleciona apenas a "Data Inicial" como 29/05/2026 e deixa a "Data Final" limpa
- **THEN** a interface SHALL atualizar os dados para mostrar apenas compras realizadas a partir de 29/05/2026, e o card de status SHALL exibir "Exibindo compras a partir de 29/05/2026".

#### Scenario: Filtro por data final apenas
- **WHEN** o usuário seleciona apenas a "Data Final" como 29/05/2026 e deixa a "Data Inicial" limpa
- **THEN** a interface SHALL atualizar os dados para mostrar apenas compras realizadas até 29/05/2026, e o card de status SHALL exibir "Exibindo compras realizadas até 29/05/2026".

#### Scenario: Filtro estrito de dia único
- **WHEN** o usuário define tanto a "Data Inicial" quanto a "Data Final" como 29/05/2026
- **THEN** a interface SHALL atualizar os dados para mostrar apenas compras do dia 29/05/2026, e o card de status SHALL exibir "Exibindo compras do dia 29/05/2026".
