## 1. Dicas e Validação Inline nos Formulários

- [x] 1.1 Adicionar regras estéticas para `.input-hint` e classes de bordas de validação `.valid`/`.invalid` em style.css
- [x] 1.2 Inserir elementos de texto `.input-hint` abaixo dos campos de descrição, categoria e valor em DashboardView.vue
- [x] 1.3 Adicionar lógica de validação visual reativa nas classes dos inputs conforme digitação

## 2. Prevenção de Envio Duplo e Feedback de Submissão

- [x] 2.1 Criar controle de estado reativo `isSubmitting` na view de Dashboard
- [x] 2.2 Desabilitar elementos do formulário e exibir texto "Salvando..." com spinner animado no botão durante envio

## 3. Sistema de Notificações Toast Dinâmico

- [x] 3.1 Implementar contêiner Toast Manager reativo em App.vue integrado às variáveis HSL do tema
- [x] 3.2 Configurar o canal de eventos global ou store para permitir disparar mensagens de sucesso/erro/aviso
- [x] 3.3 Integrar chamadas de alerta toast nos fluxos de autenticação, inserção, edição e exclusão de compras e categorias

## 4. Animações e Tooltips Interativos no Donut Chart

- [x] 4.1 Implementar animação SVG de preenchimento radial gradual ao carregar as fatias do Donut Chart
- [x] 4.2 Adicionar listeners de hover/toque nos caminhos do SVG associando a fatia selecionada à reatividade do Vue
- [x] 4.3 Renderizar caixa de Tooltip flutuante reativa com categoria, valor absoluto em R$ e percentual
