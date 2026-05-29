## Context

A aplicação de Controle de Compras possui uma disposição visual robusta e intuitiva que agrada bastante ao usuário. Porém, alguns componentes do front-end (como cartões de estatísticas e formulários internos de modais) precisam de refinamentos de CSS Responsivo para celulares estreitos, garantindo usabilidade máxima sem alterar a estrutura global aprovada.

## Goals / Non-Goals

**Goals:**
- Ajustar a barra lateral de estatísticas para exibir em 3 colunas em tablets e empilhar de forma compacta em celulares.
- Reestruturar o formulário inline do modal de categorias para se reordenar verticalmente e com espaçamentos confortáveis no celular (<600px).
- Ajustar espaçamentos (paddings) e fontes de tabelas de compras e modais para encaixar melhor em telas estreitas.
- Manter o design 100% fiel à lógica e estética premium (Glassmorphism e HSL) atual.

**Non-Goals:**
- Criar novos componentes de navegação por abas ou menu fixo inferior (o usuário solicitou explicitamente manter a mesma estrutura de painel único atual).
- Alterar códigos e endpoints de back-end.

## Decisions

- **Media Queries CSS Avançadas**: Implementadas no bloco de estilos de `App.vue` para segmentar resoluções de forma limpa:
  - `900px` (Tablets / Telas médias): Reorganização dos cards de estatísticas em linha (`grid-template-columns: repeat(3, 1fr)`) poupando espaço vertical.
  - `600px` (Celulares): Empilhamento vertical do formulário do modal de categorias e flexibilidade dos inputs de data/valor.
- **Flexbox Wrappers e Grid Layouts**: Usar flex-direction column nos formulários inline menores em telas mobile para evitar estouros de margens e texto esmagado.

## Risks / Trade-offs

- **Tabelas de Dados em Telas Muito Pequenas**:
  - *Risco*: Tabelas HTML puras podem ficar difíceis de ler em telas com menos de 360px de largura.
  - *Mitigação*: Manter o `.table-container` com rolagem horizontal fluida nativa (`overflow-x: auto; -webkit-overflow-scrolling: touch`) e reduzir ligeiramente a fonte em celulares.
