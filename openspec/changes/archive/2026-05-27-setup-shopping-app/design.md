## Context

Atualmente, não há um sistema local simples e integrado para gerenciar gastos cotidianos pessoais de forma intuitiva, rápida e esteticamente premium. O projeto ControleCompras será criado do zero para fornecer essa solução, com foco em simplicidade, facilidade de implantação e uma experiência de usuário excepcional.

## Goals / Non-Goals

**Goals:**
- Prover um back-end leve e robusto em C# (ASP.NET Core Minimal APIs) integrado ao SQLite.
- Utilizar Entity Framework Core para mapeamento objeto-relacional (ORM) e gerenciamento de migrações automáticas.
- Garantir que as categorias "Mercado", "Farmácia", "Uber" e "Outros" sejam geradas automaticamente (Seed Data) na primeira execução.
- Desenvolver um front-end SPA ágil em Vue.js 3 + Vite, com estilo de altíssimo nível (Glassmorphism, transições suaves, variáveis CSS personalizadas).
- Implementar um dashboard interativo no front-end com estatísticas de gastos por categoria e filtros avançados por mês/ano e datas específicas.

**Non-Goals:**
- Autenticação avançada e controle de múltiplos usuários (foco em uso individual/doméstico).
- Integração direta com bancos ou raspagem automática de notas fiscais (lançamento manual rápido).
- Suporte a múltiplas moedas (foco exclusivo em Real - BRL).

## Decisions

- **SQLite como Banco de Dados**: Escolhido por sua simplicidade operacional (arquivo único local `compras.db`), portabilidade total e custo zero de infraestrutura.
- **EF Core com Migrações Automáticas**: Para garantir uma abordagem Code-First. As tabelas serão mapeadas em C# e o banco de dados será gerado e atualizado via migrações do EF Core (`Microsoft.EntityFrameworkCore.Sqlite`). Adicionaremos a execução automática de `.Database.Migrate()` na inicialização do back-end para que o usuário não precise rodar comandos manuais de migração no banco de dados.
- **Vue.js 3 + Vite**: Fornece uma compilação veloz, uso da moderna Composition API, reatividade simples e excelente performance em dispositivos móveis e desktops.
- **Estilização com Vanilla CSS**: Uso completo de variáveis CSS customizadas, efeitos de Glassmorphism (backdrops, desfoques e sombras sutis) e micro-animações nas interações para garantir um visual extremamente premium e limpo.
- **Gráficos no Front-end**: Uso de uma biblioteca simples de gráficos (ex: Chart.js ou SVG puro bem estilizado) para renderizar a distribuição de despesas por categoria de forma atraente.

## Risks / Trade-offs

- **Bloqueio de Escrita no SQLite (Concorrência)**:
  - *Risco*: SQLite pode bloquear acessos se houver múltiplas escritas concorrentes intensas.
  - *Mitigação*: Como a aplicação é voltada para controle de compras pessoal e de baixo volume, o risco de concorrência de escrita é praticamente inexistente.
- **Separação de Projetos (Back e Front)**:
  - *Risco*: Necessidade de rodar duas ferramentas simultaneamente no ambiente de desenvolvimento (.NET CLI e Node/NPM).
  - *Mitigação*: Estruturaremos uma arquitetura de pastas limpa com diretórios separados `/backend` e `/frontend`, e detalharemos comandos claros de inicialização ou scripts de facilitação no futuro.
