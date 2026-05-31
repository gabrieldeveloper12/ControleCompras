## ADDED Requirements

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
