## Context

O ControleCompras possui todas as funcionalidades funcionais ativas e os ajustes básicos de toque/fontes concluídos na Fase 1. Porém, os formulários carecem de feedbacks imediatos pré-envio, as submissões não impedem cliques duplos em conexões móveis lentas, as ações de salvamento e exclusão são silenciosas (sem toasts flutuantes), e o Donut Chart é um elemento visual puramente estático. Este documento detalha como implementar interações premium reativas, segurança de fluxo e visualizações animadas usando recursos nativos do Vue 3 para manter a performance ultra-veloz.

## Goals / Non-Goals

**Goals:**
* **Validação Inline e Textos de Dica (Hints):** Inserir validação visual inline sob as caixas de texto com bordas coloridas de acordo com o estado (`--success` e `--danger`) e dicas de ajuda (`.input-hint`).
* **Prevenção de Duplo Envio:** Desabilitar o botão de submissão e exibir um spinner animado reativo durante o tráfego da API.
* **Componente de Toast Dinâmico:** Criar um gerenciador simples de notificações Toast inline no `App.vue` acessível de qualquer view para manter consistência sem pacotes npm pesados adicionais.
* **Animações e Tooltips no Donut Chart:** Implementar transições SVG suaves na renderização e tooltips flutuantes interativos (`hoveredSlice`) com dados de porcentagem e valor absoluto.

**Non-Goals:**
* Instalar pacotes pesados de terceiros para notificações ou validação de formulários (ex: Formkit, Vuelidate, vue-toastification) — tudo será construído com Vue 3 nativo de forma leve e performática.
* Alterar o modelo de dados ou rotas da API back-end.

## Decisions

### 1. Sistema Toast Baseado em Eventos Customizados ou State Global
* **Decisão:** Implementar um Toast Manager centralizado no componente principal `App.vue` que escuta eventos globais de notificação disparados pelas views ou exposto via um store simples Pinia (se aplicável), ou criar um composable leve.
* **Razão:** Permite exibir belos toasts no canto inferior de forma centralizada e sem dependências, casando perfeitamente com a paleta HSL e o design de glassmorphism da aplicação.

### 2. Validação Visual Inline via Computed Properties
* **Decisão:** Ligar classes de validação `.valid` e `.invalid` nos inputs do formulário com base no estado dinâmico dos campos em `DashboardView.vue`.
* **Razão:** Dá feedback tátil instantâneo enquanto o usuário digita no celular, reduzindo erros de preenchimento antes do clique final.

### 3. Estados de Loading Nativos nos Botões de Ação
* **Decisão:** Usar variáveis reativas de `isLoading` locais para gerenciar o estado dos botões nas views.
* **Razão:** Abordagem nativa Vue 3 direta que garante o bloqueio imediato do botão e renderiza o feedback de processamento de forma síncrona.

### 4. Donut Chart Premium Reativo com SVG Tooltips
* **Decisão:** Adicionar ouvintes de evento `mouseenter` / `mouseleave` (e equivalentes de touch `touchstart` / `touchend`) nos elementos `<path>` do Donut SVG para armazenar o segmento ativo em uma propriedade reativa e renderizar uma caixa de tooltip flutuante centralizada ou posicionada.
* **Razão:** Transforma o Donut estático em uma ferramenta de exploração de dados altamente premium sem a necessidade de importar bibliotecas de gráficos complexas.

## Risks / Trade-offs

* **[Risco]** Tooltips flutuantes em mobile podem se comportar de forma instável devido a eventos de toque persistentes.
  * **Mitigação:** Adicionar tratamento para limpar o estado do tooltip ao tocar fora ou fechar automaticamente após uma mudança de toque.
* **[Risco]** Muitos alertas toast disparados ao mesmo tempo podem poluir a tela.
  * **Mitigação:** Limitar a fila de exibição do Toast Manager para no máximo 3 notificações simultâneas, descartando as mais antigas.
