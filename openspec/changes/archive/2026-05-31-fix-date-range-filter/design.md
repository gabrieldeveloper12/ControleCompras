## Context

O componente de filtro de datas atual (`DashboardView.vue`) possui duas abas: Filtro por Mês/Ano e Filtro por Intervalo. Atualmente, o filtro por intervalo inicializa os campos de Data Inicial e Data Final preenchidos com o dia atual ("Hoje"). Isso impede que o usuário faça buscas mais flexíveis (unilaterais) e gera confusão, pois, se todos os dados foram criados recentemente, o filtro inicializado com "Hoje" continuará exibindo todas as compras. Além disso, não há indicação clara na interface sobre qual período específico está sendo filtrado no momento.

## Goals / Non-Goals

**Goals:**
- Inicializar campos de data inicial e final como vazios (`''`) por padrão no filtro por período.
- Modificar o frontend para enviar os parâmetros de data (`inicio` e `fim`) apenas se eles possuírem valor de fato.
- Adicionar suporte a buscas unilaterais na API (ex: somente `inicio` ou somente `fim`), o que já foi implementado no backend mas requer testes e sintonia com o frontend.
- Implementar um card de status dinâmico (estilo glassmorphism) mostrando amigavelmente o período ativo (ex: "Exibindo compras a partir de 29/05/2026").

**Non-Goals:**
- Criar novos endpoints na API de Backend (utilizaremos o `/api/compras` já existente e ajustado).
- Adicionar novos modais de configurações de filtro.

## Decisions

- **Inicialização Limpa (data e resetFilters)**:
  - Definiremos `filtroDataInicio = ''` e `filtroDataFim = ''` no estado inicial e na função `resetFilters()`.
- **Card de Período Ativo (Computed Property)**:
  - Criar `periodoAtivoTexto` em `DashboardView.vue` que gera uma string legível a partir de `filtroTipo`, `filtroMesAno`, `filtroDataInicio` e `filtroDataFim`.
  - Exemplos:
    - Mês específico: *"Exibindo compras de Maio/2026"*
    - Dia Único: *"Exibindo compras do dia 29/05/2026"*
    - Intervalo Completo: *"Exibindo compras de 29/05/2026 a 31/05/2026"*
    - Apenas Inicial: *"Exibindo compras realizadas a partir de 29/05/2026"*
    - Apenas Final: *"Exibindo compras realizadas até 29/05/2026"*
    - Nenhum filtro: *"Exibindo todos os registros de compras"*
- **Design do Card (CSS)**:
  - Utilizaremos os tokens HSL definidos no sistema e criaremos uma classe `.filter-status-card` em `style.css` com efeito de blur (`backdrop-filter`) e borda suave translúcida, de acordo com o design glassmorphic atual.

## Risks / Trade-offs

- **[Risco]** Parâmetros vazios podem ser enviados como strings `"null"` ou `"undefined"` se não tratados.
  - *Mitigação*: Fazer a verificação explícita `if (this.filtroDataInicio)` antes de dar `.push()` nos parâmetros do array.
- **[Risco]** Diferença de Timezone (UTC vs Local) ao processar a data no input.
  - *Mitigação*: O input `type="date"` entrega o formato "YYYY-MM-DD" e a API no backend extrai o `.Date` neutro em relação ao fuso horário, o que resolve o problema.
