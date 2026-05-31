## 1. Corrigir inicialização de datas no `data()`

- [x] 1.1 Substituir `this.getTodayDateString()` por `new Date().toISOString().split('T')[0]` nas propriedades `filtroDataInicio` e `filtroDataFim` dentro de `data()` em `DashboardView.vue`
- [x] 1.2 Substituir `this.getTodayDateString()` por `new Date().toISOString().split('T')[0]` na propriedade `formCompra.data` dentro de `data()` em `DashboardView.vue`

## 2. Corrigir normalização de timezone ao salvar compras

- [x] 2.1 Alterar a construção da data em `saveCompra()` de `new Date(this.formCompra.data).toISOString()` para `this.formCompra.data + "T12:00:00"` para preservar o dia selecionado independente do fuso horário

## 3. Corrigir `resetFilters()` e `cancelEdit()`

- [x] 3.1 Alterar `resetFilters()` para usar `new Date().toISOString().split('T')[0]` ao invés de `this.getTodayDateString()` ao redefinir `filtroDataInicio` e `filtroDataFim`
- [x] 3.2 Alterar `cancelEdit()` para usar `new Date().toISOString().split('T')[0]` ao invés de `this.getTodayDateString()` ao redefinir `formCompra.data`

## 4. Verificação

- [x] 4.1 Executar `npm run build` no frontend e confirmar que compila sem erros
- [x] 4.2 Verificar que os filtros de "Mês Específico" e "Intervalo de Datas" retornam dados corretamente quando testados com o backend ativo
