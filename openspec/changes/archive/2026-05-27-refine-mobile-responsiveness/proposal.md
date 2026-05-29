## Why

Atualmente, embora a interface de Controle de Compras possua uma estrutura intuitiva e muito bem disposta, alguns detalhes como a exibição dos cartões de estatísticas empilhados de forma gigante e os formulários internos do modal de categorias apresentam quebras e dificuldades de uso no celular. Esta proposta visa refinar finamente as regras de CSS e responsividade para garantir uma experiência premium e perfeita em dispositivos móveis, sem alterar a estrutura lógica que o usuário aprovou.

## What Changes

- Melhoria na disposição dos cartões de estatísticas (stats sidebar) para que fiquem dispostos em 3 colunas em telas de tablets e 2 colunas/empilhados de forma compacta em celulares.
- Reestruturação responsiva do formulário inline do modal de categorias para que se reorganize verticalmente em telas menores que 600px, eliminando estouros de linha.
- Ajuste fino de fontes e paddings das tabelas de lançamentos de compras e listagem de categorias no celular para otimização do espaço físico.
- Empilhamento automático inteligente dos campos de "Valor" e "Data" em celulares muito estreitos.

## Capabilities

### New Capabilities

*(Nenhuma nova capacidade é adicionada, pois refinamos a existente).*

### Modified Capabilities

- `shopping-dashboard`: Aprimoramento dos estilos CSS e comportamento reativo responsivo das telas para dispositivos móveis de telas pequenas e tablets.

## Impact

- **Código Afetado**: Arquivo principal `App.vue` no front-end (seção de estilos e estrutura interna de grid/modal).
- **Sem impactos de banco ou back-end**: A mudança é estritamente de front-end.
