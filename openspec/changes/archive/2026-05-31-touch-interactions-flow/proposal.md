## Why

Embora o ControleCompras tenha recebido as otimizações de tipografia e área física de toque na Fase 1, o aplicativo ainda carece de interações táteis refinadas e de um fluxo de feedback de ações robusto: o usuário não tem feedback visual ao salvar ou deletar compras (o que gera incerteza se a ação deu certo), os campos de formulário não possuem validação visual inline ou textos de ajuda (aumentando a taxa de erros antes do envio), e o gráfico donut de categorias é estático (sem tooltips detalhados ou animações fluidas de carregamento). Esta proposta inicia a "Fase 2 (Interações Touch e Fluxo)" para elevar a usabilidade do aplicativo ao nível de um produto maduro e premium, focando em feedbacks reativos instantâneos e segurança de fluxo.

## What Changes

- **Rótulos, Validadores e Dicas nos Inputs de Formulário:** Adição de texto de dica (hint) abaixo de cada campo do formulário de compras especificando o formato ideal, e ativação de estados visuais de validação inline (bordas em verde para dados corretos e vermelho em caso de erro).
- **Feedback de Ações e Prevenção de Duplo Envio:** Implementação de um componente de notificações toast reativo (sucesso/erro/info) para ações de salvar, editar e deletar compras, e exibição de estado de carregamento visual (spinner de processamento + botão desabilitado) no botão de envio para evitar requisições duplicadas.
- **Gráfico Donut Interativo (Tooltips & Animações):** Adição de uma animação suave de entrada (crescimento gradual das fatias usando `stroke-dasharray`) no Donut Chart e tooltips flutuantes interativos ao passar o dedo/cursor por cima de cada segmento (mostrando a categoria, valor em R$ e a porcentagem).

## Capabilities

### New Capabilities

### Modified Capabilities
- `shopping-dashboard`: Otimizar o formulário de lançamento com feedbacks de validação e hints, adicionar tooltips interativos e animações ao Donut Chart, e implementar notificações toast reativas para o fluxo de ações.

## Impact

- **Frontend:** Atualização de `DashboardView.vue` para incluir lógica de tooltips reativos e SVG animations nos segmentos do gráfico donut. Implementação de validações inline no formulário e estados reativos de submissão. Criação de um sistema de notificações Toast (via composable simples ou componente dinâmico inline).
- **APIs:** Sem impacto.
- **Dependencies:** Sem novos pacotes necessários (tudo resolvido de forma otimizada com recursos nativos Vue 3).
