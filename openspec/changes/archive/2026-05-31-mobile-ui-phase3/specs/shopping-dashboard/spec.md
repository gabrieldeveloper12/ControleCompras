## ADDED Requirements

### Requirement: Paleta de Cores Premium HSL Índigo
A estilização global do aplicativo SHALL implementar um sistema profissional de cores baseado em tons de azul-escuro índigo com 3 níveis de profundidade utilizando variáveis CSS baseadas no matiz HSL `238` para redução extrema de fadiga ocular em sessões de uso longas.

#### Scenario: Visualização do tema escuro premium
- **WHEN** o aplicativo é carregado no tema escuro padrão
- **THEN** o fundo do app SHALL ser `hsl(238, 40%, 12%)` (#12132a), os cards `hsla(238, 36%, 18% / 0.65)` (#1e2040) e as bordas `hsla(238, 30%, 25% / 0.15)` (#2e2e55).

### Requirement: Visualização de Compras em Cards no Mobile
Em larguras de telas de celulares inferiores a 640px, a lista de lançamentos de compras SHALL ocultar completamente a tabela HTML tradicional e renderizar os dados em formato de cartões individuais (.purchase-card) verticais e ergonômicos para toques rápidos.

#### Scenario: Visualizando lançamentos no celular
- **WHEN** a largura de tela do navegador for menor que 640px
- **THEN** a tabela de registros SHALL ficar oculta e cada compra SHALL ser exibida como um card tátil contendo descrição, valor BRL realçado, tag da categoria com emoji, data e botões de ação (editar/deletar) com área de toque mínima de 44px.
