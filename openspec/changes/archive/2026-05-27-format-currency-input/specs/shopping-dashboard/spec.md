## MODIFIED Requirements

### Requirement: Formulário de Lançamento de Compra
O front-end SHALL disponibilizar um formulário intuitivo para inserção de novas compras, com validações em tempo de digitação (descrição obrigatória, valor numérico positivo maior que zero, seleção de uma categoria). O campo "Valor" SHALL aceitar entradas numéricas tolerantes (usando vírgula ou ponto decimal), exibir a formatação com duas casas decimais brasileiras (ex: "2,00") ao perder o foco (blur), e expor o teclado numérico decimal nativo do celular utilizando o atributo `inputmode="decimal"`.

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
