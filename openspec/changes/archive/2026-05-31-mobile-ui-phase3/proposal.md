## Why

Atualmente, embora as melhorias funcionais e interações da Fase 2 tenham sido finalizadas com sucesso, o aplicativo ainda carece do visual premium definitivo proposto no relatório UI/UX: a paleta de cores ainda utiliza tons de cinza escuro genéricos de baixo contraste que geram fadiga visual em sessões longas, e a tabela de compras em telas de celulares menores que 640px continua espremida e exige rolagem horizontal desconfortável. Esta proposta inicia a "Fase 3 (Paleta de Cores Premium Índigo & Tabela Mobile em Cards)" para atualizar as variáveis globais de estilização estritamente para o modelo HSL Índigo e transicionar o layout da tabela móvel para cartões individuais fluidos.

## What Changes

- **Refatoração da Paleta de Cores HSL (Índigo Premium):** Atualização da variável base `--hue-base` para `238` (Índigo) e calibração das cores de superfície de 3 níveis (`#12132a`, `#1e2040`, `#252641` e `#2e2e55` para bordas), garantindo um fundo escuro e suave, menor fadiga ocular e excelente contraste de legibilidade sob luz solar ou ambientes escuros.
- **Substituição da Tabela por Cards no Mobile (< 640px):** Redesenho da exibição de compras em telas menores que 640px, ocultando a tabela horizontal tradicional e renderizando cartões `.purchase-card` verticais independentes com as informações de descrição, valor em destaque, tag de categoria colorida, data e botões táteis de ação rápida.
- **Grade CSS Grid com Auto-Fit Dinâmico:** Ajuste da disposição dos painéis no dashboard para utilizar `repeat(auto-fit, minmax(320px, 1fr))` garantindo uma reorganização automática perfeita em qualquer tamanho de celular e tablet.

## Capabilities

### New Capabilities

### Modified Capabilities
- `shopping-dashboard`: Atualizar as variáveis de paleta de cores em múltiplos níveis no `style.css` e reestruturar o layout de exibição da lista de compras para renderizar cartões responsivos no celular (< 640px).

## Impact

- **Frontend:** Atualização dos estilos globais do tema no `:root` de `style.css`. Inserção de uma seção `@media (max-width: 640px)` na renderização da lista de compras em `DashboardView.vue` para alternar dinamicamente a visualização de tabela para cartões individuais.
- **APIs:** Sem impacto.
- **Dependencies:** Sem novos pacotes necessários.
