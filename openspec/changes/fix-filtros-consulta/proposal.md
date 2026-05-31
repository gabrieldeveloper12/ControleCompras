## Why

Os filtros de consulta por data (tanto "Mês Específico" quanto "Intervalo de Datas") não funcionam corretamente devido a três bugs interligados: (1) `this.getTodayDateString()` é chamado dentro de `data()` antes do ciclo de vida do Vue disponibilizar o método, resultando em `undefined`; (2) o formato de data enviado pelo frontend (`YYYY-MM-DD`) pode ser parseado com timezone incorreto pelo backend ASP.NET, causando desalinhamento de dias; e (3) a conversão de datas ao salvar compras via `new Date("YYYY-MM-DD").toISOString()` pode deslocar o dia em fusos negativos como UTC-3 (Brasil).

## What Changes

- **Corrigir inicialização de datas no `data()`**: Substituir chamadas a `this.getTodayDateString()` por lógica inline (`new Date().toISOString().split('T')[0]`) para que os valores de filtro e formulário sejam corretamente inicializados.
- **Corrigir normalização de timezone ao salvar compras**: Ajustar a conversão de data no `saveCompra()` para preservar o dia selecionado pelo usuário independente do fuso horário local, construindo a data como meio-dia UTC ao invés de meia-noite.
- **Corrigir envio de datas no filtro de período**: Garantir que os parâmetros `inicio` e `fim` enviados na query string sejam strings `YYYY-MM-DD` puras e que o backend os interprete de forma timezone-safe.
- **Corrigir `resetFilters()`**: Usar a mesma lógica inline corrigida para redefinir os filtros para a data atual.

## Capabilities

### New Capabilities

_Nenhuma nova capability. Esta mudança corrige bugs em capabilities existentes._

### Modified Capabilities

- `shopping-dashboard`: O requisito "Filtro Temporal Dinâmico" não funciona corretamente devido aos bugs de inicialização e timezone. A correção garante que os filtros de mês e período operem de forma confiável.

## Impact

- **Frontend** (`frontend/src/views/DashboardView.vue`): Alterações no `data()`, `saveCompra()`, `fetchCompras()`, e `resetFilters()`.
- **Backend** (`backend/Program.cs`): Possível ajuste no endpoint `GET /api/compras` para parsing seguro de datas como `DateOnly` ou `DateTime` com `DateTimeStyles.AssumeUniversal`.
- **Dados existentes**: Compras já salvas com datas deslocadas por timezone podem exibir no dia anterior ao esperado. Nenhum dado será perdido, mas compras existentes podem aparecer em meses/dias ligeiramente diferentes após a correção.
