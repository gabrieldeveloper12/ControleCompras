## Context

Atualmente, embora as correções do tema claro tenham sido finalizadas com sucesso, a interface móvel do ControleCompras apresenta limitações de usabilidade física que prejudicam a experiência tátil: botões e inputs possuem altura abaixo de 44px (o mínimo ideal recomendado pelo Apple HIG e Google Material Design), toques nos botões sofrem um atraso de 300ms em navegadores iOS devido ao double-tap zoom padrão, a fonte padrão do sistema carece de hierarquia visual e consistência entre sistemas operacionais, e o gráfico donut side-by-side fica excessivamente espremido em telas pequenas de 360px/480px. Esta proposta inicia a "Fase 1 (Design & Responsividade)" para elevar a experiência móvel a um patamar profissional de aplicativos nativos.

## Goals / Non-Goals

**Goals:**
* **Área de Toque Mínima de 44px:** Garantir que todos os botões (`.btn`, `.action-btn`), caixas de entrada de texto (`.input-control`) e seletores de categoria (`.select-control`) tenham uma altura mínima de 44px em resoluções móveis.
* **Remoção de Latência de Toque:** Adicionar `touch-action: manipulation` para desativar o delay de 300ms de duplo clique em navegadores iOS/Safari.
* **Tipografia Premium Inter:** Instalação e aplicação da tipografia Inter para corpo de texto, tabelas, rótulos e caixas de entrada para legibilidade aprimorada, mantendo a Outfit para cabeçalhos premium.
* **Ajuste de Breakpoint para Donut Chart (< 480px):** Empilhar verticalmente o gráfico donut de categoria e sua legenda em resoluções móveis, limitando o diâmetro do gráfico em 200px.
* **Normalização de Rótulos de Formulário:** Converter labels de autenticação de caixa alta para Sentence Case (ex. "E-mail" e "Senha" em vez de "E-MAIL" e "SENHA").

**Non-Goals:**
* Alterações de lógica de negócio no Pinia ou APIs de back-end.
* Modificação na paleta de cores ou lógica de mudança de tema já implementada.
* Redesenho completo das telas de login ou cadastro (apenas ajustes de fontes, labels e paddings).

## Decisions

### 1. Pacote `@fontsource/inter` vs CDN
* **Decisão:** Instalar o pacote `@fontsource/inter` localmente nas dependências do front-end em vez de carregar do Google Fonts via CDN.
* **Razão:** Garante que o aplicativo carregue a fonte offline/Vercel de forma rápida, previne bloqueio de rede por adblockers e melhora o tempo de carregamento inicial (LCP).

### 2. Aumento de Área de Toque via CSS Global
* **Decisão:** Atualizar as definições globais de `.btn`, `.action-btn`, `.input-control`, e `.select-control` em `style.css` usando `min-height: 44px`.
* **Razão:** Centralizar os estilos em `style.css` garante consistência sem a necessidade de modificar todos os arquivos Vue individuais de forma invasiva, reduzindo o risco de quebra de componentes.

### 3. Remoção de Latência de Clique
* **Decisão:** Aplicar `touch-action: manipulation` em seletores globais de botões.
* **Razão:** Esta propriedade CSS instrui os navegadores móveis (Safari, Chrome Mobile) que o duplo toque para zoom não é necessário nesse elemento, eliminando instantaneamente o atraso padrão de 300ms.

### 4. Responsividade do Gráfico Donut no CSS de Dashboard
* **Decisão:** Adicionar media query `@media (max-width: 480px)` no container do gráfico para forçar `flex-direction: column` e limitar a largura/altura máxima do SVG do Donut em 200px.
* **Razão:** Gráficos circulares tornam-se ilegíveis quando colocados lado a lado com suas legendas em telas menores que 400px. O empilhamento vertical oferece excelente visibilidade e aproveita melhor o espaço vertical dos aparelhos.

## Risks / Trade-offs

* **[Risco]** Tamanhos de inputs aumentados podem empurrar o conteúdo para fora da viewport em telas muito pequenas (como iPhone SE).
  * **Mitigação:** Certificar que o container principal permite rolagem vertical sem quebrar o layout e otimizar margens para telas pequenas.
* **[Risco]** A fonte Inter pode alterar levemente a largura ocupada pelo texto.
  * **Mitigação:** Verificar se nenhum layout depende de largura fixa baseada em caracteres de fontes anteriores.
