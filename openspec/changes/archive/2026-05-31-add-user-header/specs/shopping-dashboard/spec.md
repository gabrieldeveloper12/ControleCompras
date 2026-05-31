## ADDED Requirements

### Requirement: Cabeçalho com Configurações e Avatar do Usuário
O cabeçalho principal SHALL conter uma seção à direita agrupando o status de conexão da API, um botão de engrenagem (`⚙️`) interativo para configurações rápidas, e um avatar circular que simula o perfil do usuário logado contendo dicas acessíveis (ex: "Gabriel Ramos").

#### Scenario: Rotação interativa da engrenagem
- **WHEN** o usuário passa o mouse (hover) sobre o botão de engrenagem no cabeçalho
- **THEN** a engrenagem SHALL rotacionar suavemente em 180 graus e destacar sua cor utilizando o brilho primário.

#### Scenario: Integração do botão de engrenagem com modal de categorias
- **WHEN** o usuário clica no botão de engrenagem no cabeçalho
- **THEN** a interface SHALL disparar um evento customizado que abre o modal de gerenciamento de categorias na tela principal para centralizar as configurações do dashboard.
