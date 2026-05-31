## ADDED Requirements

### Requirement: Filtro de Busca com Debounce
A interface de registros de compras SHALL disponibilizar um campo de busca textual posicionado acima da lista de compras. A digitação nesse campo de busca SHALL filtrar dinamicamente a lista de registros pela descrição com uma latência (debounce) de 300ms para evitar sobressaltos e re-renderizações excessivas.

#### Scenario: Filtrando compras por descrição
- **WHEN** o usuário digita "Feira" no campo de busca de lançamentos e aguarda 300ms
- **THEN** a lista de compras SHALL filtrar reativamente as linhas da tabela e cartões exibindo unicamente registros que contenham o termo "Feira" na descrição.

### Requirement: Ordenação Interativa de Lançamentos
Tanto a visualização de tabela horizontal quanto a visualização em cartões mobile SHALL permitir a ordenação interativa ascendente e descendente com base nos campos de Descrição, Categoria, Data e Valor. O estado de ordenação ativa SHALL exibir um indicador direcional visual correspondente (ex: seta indicativa).

#### Scenario: Ordenando lançamentos por valor decrescente
- **WHEN** o usuário clica no seletor de ordenação da coluna "Valor" mudando para decrescente
- **THEN** a lista de lançamentos de compras SHALL reordenar as linhas e cartões instantaneamente posicionando os maiores valores no topo.

### Requirement: Modal Customizado Glassmorphic de Confirmação
O aplicativo SHALL abolir o uso de caixas de diálogo síncronas nativas do navegador (`window.confirm`) para exclusão de dados e implementar diálogos de confirmação customizados renderizados no topo da tela com glassmorphism (backdrop filter blur, bordas translúcidas HSL) e área de toque confortável para celular.

#### Scenario: Cancelando exclusão de registro
- **WHEN** o modal de confirmação de exclusão de compra é exibido e o usuário clica no botão "Cancelar"
- **THEN** o modal SHALL fechar e o registro da compra SHALL permanecer intacto no banco de dados e na interface.

### Requirement: Skeleton Screen Loaders para Transições de Dados
A interface do painel SHALL exibir blocos placeholder dinâmicos com animação de pulso (skeleton loading) no lugar exato dos cartões de estatísticas e da lista de compras durante o tempo de espera da consulta da API, suavizando o layout e evitando pulos bruscos de tela.

#### Scenario: Carregamento de dados inicial
- **WHEN** o aplicativo inicializa e realiza a busca de compras no servidor
- **THEN** a interface SHALL ocultar as estatísticas vazias e exibir as caixas animadas em pulse no mesmo formato de largura e altura dos cartões e tabela finais.
