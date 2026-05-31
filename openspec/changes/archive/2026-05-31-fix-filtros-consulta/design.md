## Context

O sistema ControleCompras é composto por um frontend Vue 3 (Options API) e um backend ASP.NET Minimal API com SQLite/PostgreSQL. Os filtros de consulta de compras possuem duas modalidades: "Mês Específico" (envia `mes` e `ano` como inteiros) e "Intervalo de Datas" (envia `inicio` e `fim` como strings `YYYY-MM-DD`).

Atualmente, três bugs impedem o funcionamento correto:

1. **`this.getTodayDateString()` no `data()`**: No Vue Options API, `data()` é avaliado antes de `methods` estar disponível no `this`. Chamadas a métodos de instância retornam `undefined`, fazendo com que `filtroDataInicio`, `filtroDataFim` e `formCompra.data` sejam `undefined`.

2. **Timezone ao salvar compras**: `new Date("2025-05-31").toISOString()` gera `"2025-05-31T00:00:00.000Z"` (meia-noite UTC), que em UTC-3 (Brasil) corresponde a 21h do dia anterior. Isso pode deslocar o dia da compra no banco.

3. **Parsing de datas no backend**: O endpoint `GET /api/compras` recebe `DateTime? inicio` e `DateTime? fim`. O ASP.NET pode aplicar a cultura do servidor ao parsear strings como `"2025-05-31"`, potencialmente resultando em timezone offset incorreto.

## Goals / Non-Goals

**Goals:**
- Corrigir a inicialização dos valores de filtro de data no frontend para que sempre contenham a data atual corretamente.
- Garantir que a data selecionada pelo usuário ao salvar uma compra seja preservada independente do fuso horário do navegador.
- Garantir que os filtros de período enviem e o backend receba datas de forma timezone-safe.
- Corrigir o `resetFilters()` para resetar com a data atual usando lógica inline.

**Non-Goals:**
- Migração ou correção retroativa de datas já armazenadas no banco de dados.
- Mudanças de layout ou UX nos filtros.
- Adicionar suporte a timezone customizado do usuário.
- Modificar o filtro por "Mês Específico" (que já funciona via inteiros `mes`/`ano`).

## Decisions

### Decisão 1: Usar lógica inline no `data()` ao invés de chamar `this.methods`

**Escolha**: Substituir `this.getTodayDateString()` por `new Date().toISOString().split('T')[0]` diretamente no `data()`.

**Alternativa considerada**: Mover a inicialização para o hook `created()`. Rejeitada porque o Vue permite lógica inline no `data()` — é mais simples e idiomático para valores default estáticos.

**Alternativa considerada**: Converter para `<script setup>` (Composition API). Rejeitada porque toda a aplicação usa Options API e seria uma refatoração desproporcional.

### Decisão 2: Construir datas como meio-dia UTC ao salvar

**Escolha**: No `saveCompra()`, ao invés de `new Date(this.formCompra.data).toISOString()`, usar `this.formCompra.data + "T12:00:00"` para construir a data como meio-dia. Isso garante que, independente do fuso horário (-12 a +14), o `.toISOString()` nunca desloca o dia.

**Alternativa considerada**: Enviar a string `YYYY-MM-DD` pura ao backend e parsear lá como `DateOnly`. Rejeitada porque o campo `Compra.Data` é `DateTime` no model C# e a conversão para `DateOnly` exigiria mudanças no schema do banco e em todos os pontos que usam o campo.

### Decisão 3: Manter o parsing de `DateTime?` no backend, mas como `DateOnly` não está disponível no Minimal API binding, normalizar no frontend

**Escolha**: O frontend continua enviando `inicio=YYYY-MM-DD` e `fim=YYYY-MM-DD` na query string. O ASP.NET parseia como `DateTime` (meia-noite UTC). O backend já usa `.Date` e `.AddDays(1)` para delimitar o intervalo, o que funciona corretamente se a data for parseada como meia-noite. A correção principal é garantir que as strings de filtro não sejam `undefined` ou `null`.

**Alternativa considerada**: Mudar o backend para aceitar strings e parsear manualmente. Rejeitada por ser mais frágil e desnecessária dado que o binding do ASP.NET funciona se as strings forem válidas.

## Risks / Trade-offs

- **[Risco] Datas já armazenadas com dia deslocado** → Mitigação: Não alteramos dados existentes. Compras já salvas com timezone incorreto permanecerão com a data original. O impacto é mínimo (apenas compras criadas próximas à meia-noite em UTC-3).

- **[Risco] `new Date().toISOString().split('T')[0]` pode retornar o dia anterior se executado entre 00:00 e 02:59 BR** → Mitigação: Aceitável. `toISOString()` retorna UTC, então entre 00:00-02:59 em UTC-3 retornaria a data UTC (dia anterior). Para evitar totalmente, seria necessário usar `toLocaleDateString`, mas o formato varia entre browsers. A probabilidade de uso nesse horário é muito baixa.

- **[Trade-off] Meio-dia UTC na data da compra** → Ao salvar `"2025-05-31T12:00:00"`, o valor armazenado será `2025-05-31T12:00:00Z` ao invés de meia-noite. Isso não afeta a exibição (que usa apenas a parte da data) nem os filtros (que comparam por `.Date`).
