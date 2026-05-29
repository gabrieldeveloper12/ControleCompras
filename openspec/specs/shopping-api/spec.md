# shopping-api Specification

## Purpose
TBD - created by archiving change setup-shopping-app. Update Purpose after archive.
## Requirements
### Requirement: Inicialização de Categorias Padrão
O sistema back-end SHALL verificar a existência de categorias ao iniciar e, caso o banco esteja vazio, SHALL criar as seguintes categorias padrões: "Mercado", "Farmácia", "Uber", e "Outros". Cada uma SHALL ter um nome, um ícone (emoji ou classe CSS) e uma cor hexadecimal padrão definidos.

#### Scenario: Banco de dados vazio na inicialização
- **WHEN** a API inicia e o banco de dados de categorias está vazio
- **THEN** o sistema SHALL inserir as categorias "Mercado", "Farmácia", "Uber" e "Outros" automaticamente.

### Requirement: Registro de Nova Compra
A API SHALL disponibilizar um endpoint para registrar uma nova compra. Cada compra SHALL conter obrigatoriamente uma descrição válida (não vazia), um valor maior que zero, uma data de compra válida e o identificador de uma categoria existente.

#### Scenario: Registro de compra válido
- **WHEN** uma requisição POST é feita para `/api/compras` com descrição "Compras do mês", valor 150.50, data atual e ID de categoria existente
- **THEN** a API SHALL persistir a compra no banco de dados SQLite e retornar o status 201 Created com os dados gerados.

#### Scenario: Registro de compra com valor inválido
- **WHEN** uma requisição POST é feita para `/api/compras` com valor menor ou igual a zero
- **THEN** a API SHALL rejeitar a requisição retornando status 400 Bad Request com mensagem explicativa.

### Requirement: Consulta de Compras com Filtros
A API SHALL fornecer um endpoint para listar todas as compras registradas, com suporte opcional para filtros por intervalo de datas (Data Inicial e Data Final) e por mês/ano específico.

#### Scenario: Consulta sem filtros
- **WHEN** uma requisição GET é feita para `/api/compras` sem parâmetros
- **THEN** a API SHALL retornar todas as compras ordenadas pela data mais recente de forma decrescente.

#### Scenario: Consulta filtrada por mês e ano
- **WHEN** uma requisição GET é feita para `/api/compras?mes=5&ano=2026`
- **THEN** a API SHALL retornar apenas as compras realizadas em maio de 2026.

### Requirement: Atualização de Compra
A API SHALL fornecer um endpoint `PUT /api/compras/{id}` para atualizar as informações de uma compra existente. Os dados de entrada SHALL seguir as mesmas validações do cadastro.

#### Scenario: Atualização válida de compra
- **WHEN** uma requisição PUT é feita para `/api/compras/1` com descrição "Novo valor", valor 10.00, data atual e ID de categoria existente
- **THEN** a API SHALL atualizar os dados da compra e retornar 204 No Content.

### Requirement: Cadastro de Nova Categoria
A API SHALL fornecer um endpoint `POST /api/categorias` para cadastrar uma nova categoria com nome obrigatório e não duplicado.

#### Scenario: Cadastro de categoria válido
- **WHEN** uma requisição POST é feita para `/api/categorias` com nome "Lazer", ícone "🎮" e cor "#9C27B0"
- **THEN** a API SHALL persistir a nova categoria e retornar 201 Created.

### Requirement: Atualização de Categoria
A API SHALL fornecer um endpoint `PUT /api/categorias/{id}` para atualizar uma categoria existente.

#### Scenario: Atualização válida de categoria
- **WHEN** uma requisição PUT é feita para `/api/categorias/1` com novos dados
- **THEN** a API SHALL atualizar a categoria e retornar 204 No Content.

### Requirement: Exclusão de Categoria
A API SHALL fornecer um endpoint `DELETE /api/categorias/{id}` para excluir uma categoria. Caso existam compras vinculadas a essa categoria, a exclusão SHALL ser bloqueada.

#### Scenario: Exclusão bloqueada por compras vinculadas
- **WHEN** uma requisição DELETE é feita para `/api/categorias/1` e existem compras vinculadas a essa categoria
- **THEN** a API SHALL rejeitar a exclusão retornando status 400 Bad Request com mensagem indicando o vínculo.

