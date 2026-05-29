## Why

Atualmente, não há uma forma simples e integrada de registrar compras cotidianas (como Mercado, Farmácia, Uber, etc.) com categorização automática e relatórios visuais fáceis. Esta mudança propõe a criação de uma solução moderna, rápida e auto-hospedável de Controle de Compras com back-end em C# (ASP.NET Core + EF Core + SQLite) e front-end em Vue.js 3 para resolver esse problema de maneira prática.

## What Changes

- Criação de uma API REST back-end utilizando ASP.NET Core, Entity Framework Core e SQLite para armazenar as compras e categorias.
- Inicialização automática (Seed Data) de categorias comuns (Mercado, Farmácia, Uber, Outros) no banco de dados.
- Criação de uma aplicação front-end SPA com Vue.js 3 e Vite, estilizada de forma moderna (Glassmorphism, responsiva).
- Implementação de um painel de controle (Dashboard) simples com visualização gráfica dos gastos por categoria.
- Implementação de filtros avançados por data e mês para controle de fluxo de caixa pessoal.

## Capabilities

### New Capabilities

- `shopping-api`: API RESTful contendo endpoints CRUD para registrar compras, listar categorias padrões e filtrar dados. Banco de dados SQLite integrado e migrações do EF Core prontas para uso.
- `shopping-dashboard`: Interface SPA moderna com Vue 3 integrada à API, exibindo gráficos estatísticos de gastos por categoria e filtros temporais dinâmicos.

### Modified Capabilities

*(Nenhuma capacidade existente está sendo modificada, pois este é o início do projeto).*

## Impact

- **Tecnologias novas**: Introdução do ecossistema .NET 8/9 com C#, Entity Framework Core (Sqlite) e Vue.js 3 no diretório do projeto.
- **Armazenamento**: Criação de arquivo SQLite local (`compras.db`) no diretório do back-end.
- **Ferramental**: Necessidade de SDK do .NET e Node.js para rodar e testar o back-end e o front-end, respectivamente.
