## 1. Estruturação do Back-end e Entidades

- [x] 1.1 Criar a estrutura do projeto ASP.NET Core Web API (Minimal API) na pasta backend.
- [x] 1.2 Instalar os pacotes NuGet necessários para EF Core e SQLite (`Microsoft.EntityFrameworkCore.Sqlite` e `Microsoft.EntityFrameworkCore.Design`).
- [x] 1.3 Criar as classes de entidades de domínio `Categoria` e `Compra` no projeto C#.

## 2. Configuração do Entity Framework e Banco de Dados (Passo a Passo)

- [x] 2.1 Criar o arquivo `ComprasDbContext` configurando as relações e mapeamento das tabelas.
- [x] 2.2 Configurar a string de conexão do SQLite no arquivo `appsettings.json`.
- [x] 2.3 Executar o comando do EF Core para gerar a Migration inicial (`Add-Migration InitialCreate`) e inspecionar os arquivos criados.
- [x] 2.4 Implementar a migração automática de banco de dados (`.Database.Migrate()`) e o Seed Data inicial com as categorias padrões ("Mercado", "Farmácia", "Uber", "Outros").

## 3. Desenvolvimento dos Endpoints da API

- [x] 3.1 Desenvolver o endpoint `GET /api/categorias` para listar as categorias com suas cores e ícones.
- [x] 3.2 Desenvolver o endpoint `POST /api/compras` para cadastrar compras com validações de dados (valor > 0, descrição não vazia).
- [x] 3.3 Desenvolver o endpoint `GET /api/compras` com suporte a filtros de data/mês e ordenação decrescente por data.
- [x] 3.4 Desenvolver o endpoint `DELETE /api/compras/{id}` para exclusão rápida de registros.
- [x] 3.5 Desenvolver o endpoint `PUT /api/compras/{id}` para atualização de compras existentes.
- [x] 3.6 Desenvolver os endpoints de gerenciamento de categorias: `POST /api/categorias`, `PUT /api/categorias/{id}` e `DELETE /api/categorias/{id}` (com validação de bloqueio se houver compras vinculadas).

## 4. Setup do Front-end em Vue.js 3 e Estilo Premium

- [x] 4.1 Scaffold do projeto front-end em Vue 3 usando Vite na pasta frontend.
- [x] 4.2 Configurar a arquitetura CSS com variáveis personalizadas para paletas HSL premium, Glassmorphism e transições fluidas.
- [x] 4.3 Criar componentes básicos de layout (Header, Container e Footer) responsivos.

## 5. Implementação das Telas e Dashboard no Front-end

- [x] 5.1 Desenvolver a lista de últimas compras e o formulário de cadastro rápido integrado à API.
- [x] 5.2 Implementar os componentes de filtros temporais (seletor de mês/ano e datas De/Até).
- [x] 5.3 Implementar a exibição gráfica simples (donut ou pizza em SVG estilizado ou biblioteca leve) mostrando a distribuição dos gastos por categoria.
