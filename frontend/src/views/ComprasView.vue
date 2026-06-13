<template>
  <div class="page-content">
    <main class="main-content">
      <!-- DATA INTERACTION ROW (FORM & FILTERS) -->
      <section class="interaction-grid animate-fade-in" style="animation-delay: 0.1s">
        <!-- Form Cadastro/Edição de Compra -->
        <div class="glass-panel form-card">
          <h3 class="card-title">{{ editMode ? 'Editar Compra' : 'Registrar Compra' }}</h3>
          
          <form @submit.prevent="saveCompra" class="purchase-form">
            <div class="form-group">
              <label class="input-label" for="descricao">Descrição</label>
              <input 
                id="descricao"
                type="text" 
                v-model="formCompra.descricao" 
                class="input-control" 
                :class="{ 'valid': isDescricaoValid, 'invalid': isDescricaoInvalid }"
                :disabled="isSubmitting"
                placeholder="Ex: Supermercado do mês"
                required
              />
              <span class="input-hint" :class="{ 'error': isDescricaoInvalid }">
                {{ isDescricaoInvalid ? 'Mínimo de 3 caracteres.' : 'Mínimo de 3 caracteres (Ex: Supermercado do mês).' }}
              </span>
            </div>

            <div class="form-row">
              <div class="form-group">
                <label class="input-label" for="valor">Valor (R$)</label>
                <input 
                  id="valor"
                  type="text" 
                  inputmode="decimal"
                  v-model="valorExibido" 
                  @input="onValorInput($event)"
                  @click="onValorClick($event)"
                  @keyup="onValorKeyup($event)"
                  class="input-control" 
                  :class="{ 'valid': isValorValid, 'invalid': isValorInvalid }"
                  :disabled="isSubmitting"
                  placeholder="0,00"
                  required
                  @focus="onValorFocus($event)"
                  @blur="onValorBlur"
                />
                <span class="input-hint" :class="{ 'error': isValorInvalid }">
                  {{ isValorInvalid ? 'Insira um valor numérico válido.' : 'Valor da compra (Ex: 29,90).' }}
                </span>
              </div>

              <div class="form-group">
                <label class="input-label" for="data">Data</label>
                <input 
                  id="data"
                  type="date" 
                  v-model="formCompra.data" 
                  class="input-control" 
                  :class="{ 'valid': isDataValid }"
                  :disabled="isSubmitting"
                  required
                />
                <span class="input-hint">Data da compra (Dia/Mês/Ano).</span>
              </div>
            </div>

            <div class="form-group">
              <label class="input-label" for="categoria">Categoria</label>
              <select 
                id="categoria"
                v-model="formCompra.categoriaId" 
                class="input-control select-control"
                :class="{ 'valid': isCategoriaValid }"
                :disabled="isSubmitting"
                required
              >
                <option value="" disabled>Selecione uma categoria</option>
                <option 
                  v-for="cat in categorias" 
                  :key="cat.id" 
                  :value="cat.id"
                >
                  {{ cat.icone }} {{ cat.nome }}
                </option>
              </select>
              <span class="input-hint">Selecione o grupo do gasto.</span>
            </div>

            <div class="form-group">
              <label class="input-label">Forma de Pagamento</label>
              <div v-if="!editMode" class="payment-methods-grid">
                <button 
                  type="button" 
                  class="payment-method-btn" 
                  :class="{ active: formCompra.formaPagamento === 'debito' }"
                  @click="formCompra.formaPagamento = 'debito'; formCompra.totalParcelas = null"
                  :disabled="isSubmitting"
                >
                  💳 Débito
                </button>
                <button 
                  type="button" 
                  class="payment-method-btn" 
                  :class="{ active: formCompra.formaPagamento === 'pix' }"
                  @click="formCompra.formaPagamento = 'pix'; formCompra.totalParcelas = null"
                  :disabled="isSubmitting"
                >
                  📱 Pix
                </button>
                <button 
                  type="button" 
                  class="payment-method-btn" 
                  :class="{ active: formCompra.formaPagamento === 'cartao_avista' }"
                  @click="formCompra.formaPagamento = 'cartao_avista'; formCompra.totalParcelas = null"
                  :disabled="isSubmitting"
                >
                  💳 Cartão à Vista
                </button>
                <button 
                  type="button" 
                  class="payment-method-btn" 
                  :class="{ active: formCompra.formaPagamento === 'cartao_parcelado' }"
                  @click="formCompra.formaPagamento = 'cartao_parcelado'; formCompra.totalParcelas = ''"
                  :disabled="isSubmitting"
                >
                  💳 Parcelado
                </button>
              </div>
              <div v-else-if="formCompra.formaPagamento" class="payment-method-readonly">
                {{ getFormaPagamentoIcon(formCompra.formaPagamento) }} {{ getFormaPagamentoLabel(formCompra.formaPagamento) }}
                <span v-if="formCompra.formaPagamento === 'cartao_parcelado'">
                  — Parcela {{ formCompra.numeroParcela }} de {{ formCompra.totalParcelas }}
                </span>
              </div>
              <div v-else class="payment-method-readonly">
                Nenhuma forma de pagamento registrada
              </div>
            </div>

            <div v-if="formCompra.formaPagamento === 'cartao_parcelado' && !editMode" class="form-group animate-fade-in">
              <label class="input-label" for="totalParcelas">Quantidade de Parcelas</label>
              <select 
                id="totalParcelas" 
                v-model="formCompra.totalParcelas" 
                class="input-control select-control"
                :disabled="isSubmitting"
                required
              >
                <option value="" disabled>Selecione o número de parcelas</option>
                <option v-for="n in 11" :key="n + 1" :value="n + 1">{{ n + 1 }}x</option>
              </select>
              <span v-if="formCompra.valor && formCompra.totalParcelas" class="input-hint" style="color: var(--success); font-weight: 500;">
                Preview: {{ formCompra.totalParcelas }}x de {{ formatCurrency(calcularParcelaPreview()) }}
              </span>
            </div>

            <div class="form-actions">
              <button 
                type="button" 
                class="btn btn-secondary" 
                :disabled="isSubmitting"
                @click="cancelEdit"
              >
                {{ editMode ? 'Cancelar' : 'Limpar' }}
              </button>
              <button 
                type="submit" 
                class="btn btn-primary submit-btn"
                :disabled="isSubmitting || !isDescricaoValid || !isValorValid || !isCategoriaValid || !isDataValid || (formCompra.formaPagamento === 'cartao_parcelado' && !formCompra.totalParcelas && !editMode)"
              >
                <span v-if="isSubmitting" class="spinner-icon">🔄</span>
                <span>{{ isSubmitting ? 'Salvando...' : (editMode ? 'Salvar Alterações' : 'Salvar Compra') }}</span>
              </button>
            </div>
          </form>
        </div>

        <!-- Filtros de Consulta -->
        <div class="glass-panel filters-card">
          <h3 class="card-title">Filtros e Consultas</h3>
          
          <div class="filters-container">
            <!-- Segmented Control Tabs -->
            <div class="filter-tabs">
              <button 
                type="button" 
                class="tab-btn" 
                :class="{ active: filtroTipo === 'mes' }"
                @click="setFiltroTipo('mes')"
              >
                📅 Mês Específico
              </button>
              <button 
                type="button" 
                class="tab-btn" 
                :class="{ active: filtroTipo === 'periodo' }"
                @click="setFiltroTipo('periodo')"
              >
                📆 Intervalo de Datas
              </button>
            </div>

            <!-- Tab 1: Filtro por Mês/Ano -->
            <div v-if="filtroTipo === 'mes'" class="form-group animate-fade-in">
              <label class="input-label">Selecionar Mês/Ano</label>
              <input 
                type="month" 
                v-model="filtroMesAno" 
                class="input-control"
              />
            </div>

            <!-- Tab 2: Filtro por Intervalo de Datas -->
            <div v-if="filtroTipo === 'periodo'" class="form-row animate-fade-in">
              <div class="form-group">
                <label class="input-label">Data Inicial</label>
                <input 
                  type="date" 
                  v-model="filtroDataInicio" 
                  class="input-control"
                  :class="{ 'invalid': isPeriodoInvalido }"
                />
              </div>

              <div class="form-group">
                <label class="input-label">Data Final</label>
                <input 
                  type="date" 
                  v-model="filtroDataFim" 
                  class="input-control"
                  :class="{ 'invalid': isPeriodoInvalido }"
                />
              </div>
            </div>

            <!-- Error message for invalid period -->
            <div v-if="filtroTipo === 'periodo' && isPeriodoInvalido" class="periodo-error-message animate-fade-in">
              ⚠️ A data final não pode ser menor que a data inicial.
            </div>

            <div class="filter-actions-row">
              <button 
                type="button" 
                class="btn btn-primary filter-submit-btn" 
                :disabled="isPeriodoInvalido"
                @click="fetchCompras"
              >
                🔍 Consultar
              </button>
              <button type="button" class="btn btn-secondary clear-filters-btn" @click="resetFilters">
                🧹 Limpar Filtros
              </button>
            </div>

            <!-- Filter Status Active Card -->
            <div class="filter-status-card">
              <span class="filter-status-icon">🔍</span>
              <span class="filter-status-text">{{ periodoAtivoTexto }}</span>
            </div>
          </div>
        </div>
      </section>

      <!-- PURCHASES LIST TABLE -->
      <section class="glass-panel list-section animate-fade-in" style="animation-delay: 0.2s">
        <div class="list-header">
          <h3 class="card-title">Registros de Compras</h3>
          <div class="list-controls">
            <input 
              type="text" 
              v-model="searchQuery" 
              class="input-control search-bar" 
              placeholder="Buscar por descrição..." 
            />
            <select v-model.number="pageSize" class="select-control page-size-select" title="Registros por página">
              <option :value="5">5 por pág.</option>
              <option :value="10">10 por pág.</option>
              <option :value="20">20 por pág.</option>
              <option :value="50">50 por pág.</option>
            </select>
          </div>
          <span class="count-badge">{{ filteredCountText }}</span>
        </div>

        <div class="table-container">
          <div v-if="isLoading" class="skeleton-list">
            <div class="skeleton-block skeleton-row" v-for="i in 5" :key="i"></div>
          </div>
          <div v-else-if="filteredCompras.length === 0" class="empty-table-state">
            <span>📝</span>
            <p>Nenhuma compra encontrada.</p>
          </div>
          
          <template v-else>
            <!-- Desktop Table View -->
            <table class="purchase-table">
              <thead>
                <tr>
                  <th @click="sortBy('descricao')" class="sortable">Descrição <span v-if="sortKey === 'descricao'">{{ sortAsc ? '▲' : '▼' }}</span></th>
                  <th @click="sortBy('categoria')" class="sortable">Categoria <span v-if="sortKey === 'categoria'">{{ sortAsc ? '▲' : '▼' }}</span></th>
                  <th @click="sortBy('data')" class="sortable">Data <span v-if="sortKey === 'data'">{{ sortAsc ? '▲' : '▼' }}</span></th>
                  <th @click="sortBy('valor')" class="sortable text-right">Valor <span v-if="sortKey === 'valor'">{{ sortAsc ? '▲' : '▼' }}</span></th>
                  <th class="text-center">Ações</th>
                </tr>
              </thead>
              <tbody>
                <template v-for="grupo in paginatedComprasAgrupadas" :key="grupo.id || grupo.grupoParcelaId">
                  <!-- Row: Normal purchase (not a group) -->
                  <tr v-if="!grupo.isGroup" class="table-row">
                    <td class="col-desc">
                      <strong>{{ grupo.descricao }}</strong>
                      <span v-if="grupo.formaPagamento" class="count-badge" style="margin-left: 0.5rem; font-size: 0.72rem; padding: 0.1rem 0.4rem;">
                        {{ getFormaPagamentoIcon(grupo.formaPagamento) }} {{ getFormaPagamentoLabel(grupo.formaPagamento) }}
                      </span>
                    </td>
                    <td class="col-cat">
                      <span 
                        class="category-tag" 
                        :style="{ 
                          borderLeft: `4px solid ${grupo.categoria?.corHex || '#999'}`,
                          backgroundColor: `${grupo.categoria?.corHex || '#999'}15`
                        }"
                      >
                        <span class="tag-icon">{{ grupo.categoria?.icone }}</span>
                        <span class="tag-name">{{ grupo.categoria?.nome }}</span>
                      </span>
                    </td>
                    <td class="col-date">{{ formatDate(grupo.data) }}</td>
                    <td class="col-val text-right">
                      <strong>{{ formatCurrency(grupo.valor) }}</strong>
                    </td>
                    <td class="col-actions text-center">
                      <button class="action-btn edit" @click="startEdit(grupo)" title="Editar">✏️</button>
                      <button class="action-btn delete" @click="deleteCompra(grupo)" title="Excluir">❌</button>
                    </td>
                  </tr>

                  <!-- Row: Group header (installment group) -->
                  <template v-else>
                    <tr class="table-row group-row">
                      <td class="col-desc">
                        <button type="button" class="group-toggle-btn" :class="{ expanded: isGroupExpanded(grupo.id) }" @click="toggleGroup(grupo.id)">
                          ▶
                        </button>
                        <strong>{{ grupo.descricao }}</strong>
                        <span class="count-badge" style="margin-left: 0.5rem; font-size: 0.72rem; padding: 0.1rem 0.4rem; background: var(--primary-glow); border-color: var(--primary);">
                          💳 {{ getFormaPagamentoLabel(grupo.formaPagamento) }} ({{ grupo.totalParcelas }}x)
                        </span>
                      </td>
                      <td class="col-cat">
                        <span 
                          class="category-tag" 
                          :style="{ 
                            borderLeft: `4px solid ${grupo.categoria?.corHex || '#999'}`,
                            backgroundColor: `${grupo.categoria?.corHex || '#999'}15`
                          }"
                        >
                          <span class="tag-icon">{{ grupo.categoria?.icone }}</span>
                          <span class="tag-name">{{ grupo.categoria?.nome }}</span>
                        </span>
                      </td>
                      <td class="col-date">{{ formatDate(grupo.data) }}</td>
                      <td class="col-val text-right">
                        <strong>{{ formatCurrency(grupo.valor) }}</strong>
                      </td>
                      <td class="col-actions text-center">
                        <button class="action-btn delete" @click="deleteCompra(grupo)" title="Excluir Parcelamento Completo">❌</button>
                      </td>
                    </tr>

                    <!-- Rows: Group children (installments) -->
                    <tr 
                      v-if="isGroupExpanded(grupo.id)" 
                      v-for="item in grupo.items" 
                      :key="item.id" 
                      class="table-row sub-row animate-fade-in"
                    >
                      <td class="col-desc indent-cell">
                        <span>↳ Parcela {{ item.numeroParcela }} de {{ item.totalParcelas }}</span>
                      </td>
                      <td class="col-cat">
                        <span 
                          class="category-tag" 
                          :style="{ 
                            borderLeft: `4px solid ${item.categoria?.corHex || '#999'}`,
                            backgroundColor: `${item.categoria?.corHex || '#999'}10`
                          }"
                        >
                          <span class="tag-icon">{{ item.categoria?.icone }}</span>
                          <span class="tag-name">{{ item.categoria?.nome }}</span>
                        </span>
                      </td>
                      <td class="col-date">{{ formatDate(item.data) }}</td>
                      <td class="col-val text-right">
                        <strong>{{ formatCurrency(item.valor) }}</strong>
                      </td>
                      <td class="col-actions text-center">
                        <button class="action-btn edit" @click="startEdit(item)" title="Editar Parcela">✏️</button>
                      </td>
                    </tr>
                  </template>
                </template>
              </tbody>
            </table>

            <!-- Mobile Cards View -->
            <div class="purchase-cards-mobile">
              <template v-for="grupo in paginatedComprasAgrupadas" :key="grupo.id || grupo.grupoParcelaId">
                <!-- Card: Normal purchase -->
                <div v-if="!grupo.isGroup" class="purchase-card glass-panel">
                  <div class="card-header-row">
                    <strong class="card-description">{{ grupo.descricao }}</strong>
                    <span v-if="grupo.formaPagamento" class="count-badge" style="font-size: 0.72rem; padding: 0.1rem 0.4rem;">
                      {{ getFormaPagamentoIcon(grupo.formaPagamento) }} {{ getFormaPagamentoLabel(grupo.formaPagamento) }}
                    </span>
                    <span 
                      class="category-tag" 
                      :style="{ 
                        borderLeft: `4px solid ${grupo.categoria?.corHex || '#999'}`,
                        backgroundColor: `${grupo.categoria?.corHex || '#999'}15`
                      }"
                    >
                      <span class="tag-icon">{{ grupo.categoria?.icone }}</span>
                      <span class="tag-name">{{ grupo.categoria?.nome }}</span>
                    </span>
                  </div>
                  
                  <div class="card-body-row">
                    <div class="card-info-item">
                      <span class="card-label">Data</span>
                      <span class="card-value">{{ formatDate(grupo.data) }}</span>
                    </div>
                    <div class="card-info-item text-right">
                      <span class="card-label">Valor</span>
                      <strong class="card-value highlight">{{ formatCurrency(grupo.valor) }}</strong>
                    </div>
                  </div>
                  
                  <div class="card-actions-row">
                    <button class="btn btn-secondary action-btn-tátil" @click="startEdit(grupo)">
                      <span>✏️ Editar</span>
                    </button>
                    <button class="btn btn-danger action-btn-tátil" @click="deleteCompra(grupo)">
                      <span>❌ Excluir</span>
                    </button>
                  </div>
                </div>

                <!-- Card: Group (Installment purchase) -->
                <div v-else class="purchase-card glass-panel purchase-card-group-header">
                  <div class="card-header-row">
                    <div style="display: flex; align-items: center; gap: 0.5rem;">
                      <button type="button" class="group-toggle-btn" :class="{ expanded: isGroupExpanded(grupo.id) }" @click="toggleGroup(grupo.id)">
                        ▶
                      </button>
                      <strong class="card-description">{{ grupo.descricao }}</strong>
                    </div>
                    <span class="count-badge" style="font-size: 0.72rem; padding: 0.1rem 0.4rem; background: var(--primary-glow); border-color: var(--primary);">
                      💳 {{ getFormaPagamentoLabel(grupo.formaPagamento) }} ({{ grupo.totalParcelas }}x)
                    </span>
                    <span 
                      class="category-tag" 
                      :style="{ 
                        borderLeft: `4px solid ${grupo.categoria?.corHex || '#999'}`,
                        backgroundColor: `${grupo.categoria?.corHex || '#999'}15`
                      }"
                    >
                      <span class="tag-icon">{{ grupo.categoria?.icone }}</span>
                      <span class="tag-name">{{ grupo.categoria?.nome }}</span>
                    </span>
                  </div>
                  
                  <div class="card-body-row">
                    <div class="card-info-item">
                      <span class="card-label">Data Inicial</span>
                      <span class="card-value">{{ formatDate(grupo.data) }}</span>
                    </div>
                    <div class="card-info-item text-right">
                      <span class="card-label">Valor do Grupo</span>
                      <strong class="card-value highlight">{{ formatCurrency(grupo.valor) }}</strong>
                    </div>
                  </div>
                  
                  <div class="card-actions-row">
                    <button class="btn btn-danger action-btn-tátil" @click="deleteCompra(grupo)">
                      <span>❌ Excluir Grupo</span>
                    </button>
                  </div>

                  <!-- Children installment list inside group -->
                  <div v-if="isGroupExpanded(grupo.id)" class="purchase-card-group-children animate-fade-in">
                    <div 
                      v-for="item in grupo.items" 
                      :key="item.id" 
                      class="purchase-card sub-card glass-panel"
                      style="padding: 0.75rem;"
                    >
                      <div class="card-header-row">
                        <strong>Parcela {{ item.numeroParcela }} de {{ item.totalParcelas }}</strong>
                        <span class="card-value">{{ formatDate(item.data) }}</span>
                      </div>
                      <div class="card-body-row" style="border: none; padding: 0.25rem 0 0.5rem 0;">
                        <div class="card-info-item">
                          <span class="card-label">Valor Parcela</span>
                          <strong class="card-value highlight" style="font-size: 0.95rem;">{{ formatCurrency(item.valor) }}</strong>
                        </div>
                      </div>
                      <div class="card-actions-row">
                        <button class="btn btn-secondary action-btn-tátil" style="min-height: 32px !important; padding: 0.25rem 0.5rem;" @click="startEdit(item)">
                          <span>✏️ Editar Parcela</span>
                        </button>
                      </div>
                    </div>
                  </div>
                </div>
              </template>
            </div>
          </template>

          <!-- Paginação -->
          <div v-if="totalPaginas > 1" class="pagination-container">
            <button
              class="pagination-btn"
              :disabled="currentPage === 1"
              @click="currentPage = 1"
              title="Primeira página"
            >«</button>
            <button
              class="pagination-btn"
              :disabled="currentPage === 1"
              @click="currentPage--"
              title="Página anterior"
            >‹</button>

            <template v-for="pg in totalPaginas" :key="pg">
              <button
                v-if="pg === 1 || pg === totalPaginas || (pg >= currentPage - 1 && pg <= currentPage + 1)"
                class="pagination-btn"
                :class="{ active: pg === currentPage }"
                @click="currentPage = pg"
              >{{ pg }}</button>
              <span
                v-else-if="pg === currentPage - 2 || pg === currentPage + 2"
                class="pagination-ellipsis"
              >…</span>
            </template>

            <button
              class="pagination-btn"
              :disabled="currentPage === totalPaginas"
              @click="currentPage++"
              title="Próxima página"
            >›</button>
            <button
              class="pagination-btn"
              :disabled="currentPage === totalPaginas"
              @click="currentPage = totalPaginas"
              title="Última página"
            >»</button>
          </div>
        </div>
      </section>
    </main>

    <!-- CATEGORIES MANAGEMENT MODAL -->
    <div v-if="categoryModalOpen" class="modal-overlay" @click.self="closeCategoryModal">
      <div class="modal-content glass-panel animate-fade-in">
        <div class="modal-header">
          <h2>Gerenciamento de Categorias</h2>
          <button class="close-modal-btn" @click="closeCategoryModal">&times;</button>
        </div>

        <div class="modal-body">
          <!-- Form Criar/Editar Categoria -->
          <div class="modal-form-box">
            <h4>{{ editCatMode ? 'Editar Categoria' : 'Nova Categoria' }}</h4>
            <form @submit.prevent="saveCategoria" class="cat-form">
              <div class="cat-form-grid">
                <!-- Coluna Esquerda: Nome, Cor e Preview -->
                <div class="cat-form-left">
                  <div class="form-group">
                    <label class="input-label">Nome da Categoria</label>
                    <input 
                      type="text" 
                      v-model="formCat.nome" 
                      class="input-control" 
                      placeholder="Nome (ex: Lazer)"
                      required
                    />
                  </div>
                  
                  <!-- Seletor de Cores -->
                  <div class="color-selector-group">
                    <label class="input-label">Cor da Categoria</label>
                    <div class="color-grid">
                      <button
                        v-for="color in colorOptions"
                        :key="color"
                        type="button"
                        class="color-btn"
                        :style="{ backgroundColor: color }"
                        :class="{ active: formCat.corHex === color }"
                        @click="formCat.corHex = color"
                        title="Selecionar cor"
                      ></button>
                      
                      <div class="custom-color-picker-wrapper">
                        <button 
                          type="button" 
                          class="color-btn custom-color-trigger" 
                          :style="{ background: !colorOptions.includes(formCat.corHex) ? formCat.corHex : 'conic-gradient(red, yellow, green, cyan, blue, magenta, red)' }"
                          :class="{ active: !colorOptions.includes(formCat.corHex) }"
                          title="Outra cor..."
                        >
                          🎨
                        </button>
                        <input 
                          type="color" 
                          v-model="formCat.corHex" 
                          class="custom-color-input"
                        />
                      </div>
                    </div>
                  </div>

                  <!-- Preview da Categoria -->
                  <div class="cat-preview">
                    <span class="cat-preview-label">Preview:</span>
                    <span 
                      class="category-tag cat-preview-badge" 
                      :style="{ 
                        borderLeft: `4px solid ${previewCategoria.corHex}`,
                        backgroundColor: `${previewCategoria.corHex}15`
                      }"
                    >
                      <span class="tag-icon">{{ previewCategoria.icone }}</span>
                      <span class="tag-name">{{ previewCategoria.nome }}</span>
                    </span>
                  </div>
                </div>

                <!-- Coluna Direita: Seletor de Emoji -->
                <div class="cat-form-right">
                  <label class="input-label">Ícone da Categoria</label>
                  <div class="emoji-container">
                    <div class="emoji-grid">
                      <button
                        v-for="emoji in emojiOptions"
                        :key="emoji"
                        type="button"
                        class="emoji-btn"
                        :class="{ active: formCat.icone === emoji }"
                        :style="formCat.icone === emoji ? { borderColor: formCat.corHex, boxShadow: `0 0 8px ${formCat.corHex}` } : {}"
                        @click="formCat.icone = emoji"
                      >
                        {{ emoji }}
                      </button>
                    </div>
                    <div class="emoji-fallback-box">
                      <span class="emoji-fallback-icon">✏️</span>
                      <input 
                        type="text" 
                        v-model="formCat.icone" 
                        class="input-control emoji-fallback-input" 
                        placeholder="Outro emoji..."
                        maxlength="2"
                        required
                      />
                    </div>
                  </div>
                </div>
              </div>

              <div class="cat-form-actions" style="margin-top: 1.25rem; justify-content: flex-end;">
                <button v-if="editCatMode" type="button" class="btn btn-secondary btn-sm" :disabled="isSubmittingCat" @click="cancelCatEdit">
                  Cancelar
                </button>
                <button type="submit" class="btn btn-primary btn-sm" :disabled="isSubmittingCat">
                  {{ isSubmittingCat ? 'Salvando...' : (editCatMode ? 'Salvar' : 'Adicionar') }}
                </button>
              </div>
            </form>
          </div>

          <!-- List of Existing Categories -->
          <div class="modal-cat-list">
            <h4>Categorias Ativas</h4>
            <div class="cat-list-container">
              <div 
                v-for="cat in categorias" 
                :key="cat.id" 
                class="cat-item"
                :style="{ borderLeft: `4px solid ${cat.corHex}` }"
              >
                <div class="cat-item-info">
                  <span class="cat-item-icon">{{ cat.icone }}</span>
                  <span class="cat-item-name">{{ cat.nome }}</span>
                  <span class="cat-color-dot" :style="{ backgroundColor: cat.corHex }"></span>
                  <span class="cat-item-color-label">{{ cat.corHex }}</span>
                </div>
                <div class="cat-item-actions">
                  <button class="action-btn edit" @click="startCatEdit(cat)">✏️</button>
                  <button class="action-btn delete" @click="deleteCategoria(cat.id)">❌</button>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- CONFIRM MODAL -->
    <Teleport to="body">
      <div v-if="confirmModal.show" class="confirm-modal-overlay" @click.self="confirmModal.resolve(false); confirmModal.show = false">
        <div class="confirm-modal-box glass-panel animate-fade-in">
          <div class="confirm-modal-icon">⚠️</div>
          <h3 class="confirm-modal-title">Confirmação</h3>
          <p class="confirm-modal-message">{{ confirmModal.message }}</p>
          <div class="confirm-modal-actions">
            <button class="btn btn-secondary action-btn-tátil confirm-btn" @click="confirmModal.resolve(false); confirmModal.show = false">
              Cancelar
            </button>
            <button class="btn btn-danger action-btn-tátil confirm-btn" @click="confirmModal.resolve(true); confirmModal.show = false">
              Confirmar
            </button>
          </div>
        </div>
      </div>
    </Teleport>

  </div>
</template>

<script>
import { useAuthStore } from '../stores/auth';

const BASE_URL = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5076';
const API_BASE = `${BASE_URL}/api`;

export default {
  name: 'ComprasView',
  data() {
    return {
      categorias: [],
      compras: [],
      
      // Filtros
      filtroTipo: 'mes',
      filtroMesAno: new Date(Date.now() - new Date().getTimezoneOffset() * 60000).toISOString().substring(0, 7),
      filtroDataInicio: new Date(Date.now() - new Date().getTimezoneOffset() * 60000).toISOString().split('T')[0],
      filtroDataFim: new Date(Date.now() - new Date().getTimezoneOffset() * 60000).toISOString().split('T')[0],
      
      // Form Compra
      formCompra: {
        id: null,
        descricao: '',
        valor: null,
        data: new Date(Date.now() - new Date().getTimezoneOffset() * 60000).toISOString().split('T')[0],
        categoriaId: '',
        formaPagamento: 'pix',
        totalParcelas: null,
        numeroParcela: null,
        grupoParcelaId: null
      },
      valorExibido: 'R$ 0,00',
      editMode: false,
      expandedGroups: {},

      // Modal Categorias
      categoryModalOpen: false,
      formCat: {
        id: null,
        nome: '',
        icone: '📦',
        corHex: '#9E9E9E'
      },
      editCatMode: false,
      isSubmittingCat: false,
      emojiOptions: ['🛒','💊','🚗','🍔','🍕','🍧','💧','🏠','🎮','✈️','👕','📱','💰','🏋️','📚','🎬','🐾','🎵','💄','🍷','☕','🍺','⛽','✂️','🛠️','💳','🎁','🏥','🐶','📦','🍦','🥖'],
      colorOptions: ['#FF6B6B', '#FF8E53', '#FFD043', '#10B981', '#06B6D4', '#3B82F6', '#6366F1', '#8B5CF6', '#EC4899', '#F59E0B', '#84CC16', '#14B8A6', '#A855F7', '#6B7280'],
      isSubmitting: false,

      // Busca debounced
      searchQuery: '',
      debouncedSearchQuery: '',
      searchDebounceTimer: null,

      // Ordenação
      sortKey: 'data',
      sortAsc: false,

      // Modal de confirmação customizado
      confirmModal: {
        show: false,
        message: '',
        resolve: null
      },

      // Skeleton loader
      isLoading: true,

      // Paginação
      currentPage: 1,
      pageSize: parseInt(localStorage.getItem('controle_compras_pageSize') || '10', 10) || 10
    }
  },
  watch: {
    searchQuery(newVal) {
      clearTimeout(this.searchDebounceTimer);
      this.searchDebounceTimer = setTimeout(() => {
        this.debouncedSearchQuery = newVal;
        this.currentPage = 1;
      }, 300);
    },
    compras() {
      this.currentPage = 1;
    },
    pageSize(newVal) {
      localStorage.setItem('controle_compras_pageSize', newVal.toString());
      this.currentPage = 1;
    },
    categoryModalOpen(isOpen) {
      if (isOpen || this.confirmModal.show) {
        document.body.classList.add('modal-open');
      } else {
        document.body.classList.remove('modal-open');
      }
    },
    'confirmModal.show'(isOpen) {
      if (isOpen || this.categoryModalOpen) {
        document.body.classList.add('modal-open');
      } else {
        document.body.classList.remove('modal-open');
      }
    }
  },
  computed: {
    previewCategoria() {
      return {
        nome: this.formCat.nome || 'Nova Categoria',
        icone: this.formCat.icone || '📦',
        corHex: this.formCat.corHex || '#9E9E9E'
      };
    },
    isDescricaoValid() {
      return this.formCompra.descricao.trim().length >= 3;
    },
    isDescricaoInvalid() {
      return this.formCompra.descricao.trim().length > 0 && this.formCompra.descricao.trim().length < 3;
    },
    isValorValid() {
      const val = this.formCompra.valor;
      return typeof val === 'number' && !isNaN(val) && val > 0;
    },
    isValorInvalid() {
      if (this.valorExibido === 'R$ 0,00') return false;
      const val = this.formCompra.valor;
      return val !== null && val !== undefined && (isNaN(val) || val <= 0);
    },
    isCategoriaValid() {
      return this.formCompra.categoriaId !== '';
    },
    isDataValid() {
      return this.formCompra.data !== '';
    },
    isPeriodoInvalido() {
      if (this.filtroTipo !== 'periodo') return false;
      if (!this.filtroDataInicio || !this.filtroDataFim) return false;
      return this.filtroDataFim < this.filtroDataInicio;
    },
    filteredCompras() {
      let list = this.editMode && this.formCompra.id
        ? this.compras.filter(c => c.id !== this.formCompra.id)
        : [...this.compras];

      const q = (this.debouncedSearchQuery || '').trim().toLowerCase();
      if (q) {
        list = list.filter(c => c.descricao && c.descricao.toLowerCase().includes(q));
      }

      if (this.sortKey) {
        list = list.slice().sort((a, b) => {
          let va = a[this.sortKey];
          let vb = b[this.sortKey];
          if (this.sortKey === 'categoria') {
            va = a.categoria?.nome || '';
            vb = b.categoria?.nome || '';
          }
          if (va === null || va === undefined) va = '';
          if (vb === null || vb === undefined) vb = '';
          if (typeof va === 'string') va = va.toLowerCase();
          if (typeof vb === 'string') vb = vb.toLowerCase();
          if (va < vb) return this.sortAsc ? -1 : 1;
          if (va > vb) return this.sortAsc ? 1 : -1;
          return 0;
        });
      }

      return list;
    },
    periodoAtivoTexto() {
      if (this.filtroTipo === 'mes') {
        if (!this.filtroMesAno) return 'Exibindo todos os registros';
        const [ano, mes] = this.filtroMesAno.split('-');
        const meses = [
          'Janeiro', 'Fevereiro', 'Março', 'Abril', 'Maio', 'Junho',
          'Julho', 'Agosto', 'Setembro', 'Outubro', 'Novembro', 'Dezembro'
        ];
        const nomeMes = meses[parseInt(mes) - 1];
        return `Exibindo compras de ${nomeMes} de ${ano}`;
      } else if (this.filtroTipo === 'periodo') {
        const formatarData = (dt) => {
          if (!dt) return '';
          const [ano, mes, dia] = dt.split('-');
          return `${dia}/${mes}/${ano}`;
        };
        
        const ini = formatarData(this.filtroDataInicio);
        const fim = formatarData(this.filtroDataFim);
        
        if (ini && fim) {
          if (this.filtroDataInicio === this.filtroDataFim) {
            return `Exibindo compras do dia ${ini}`;
          }
          return `Exibindo compras de ${ini} até ${fim}`;
        } else if (ini) {
          return `Exibindo compras realizadas a partir de ${ini}`;
        } else if (fim) {
          return `Exibindo compras realizadas até ${fim}`;
        }
        return 'Exibindo todos os registros de compras';
      }
      return 'Exibindo compras';
    },
    filteredCountText() {
      const total = this.comprasAgrupadas.length;
      if (total === 0) return 'Nenhum lançamento';
      const inicio = (this.currentPage - 1) * this.pageSize + 1;
      const fim = Math.min(this.currentPage * this.pageSize, total);
      if (total === 1) return '1 lançamento';
      return `${inicio}–${fim} de ${total} lançamentos`;
    },
    totalPaginas() {
      return Math.ceil(this.comprasAgrupadas.length / this.pageSize);
    },
    paginatedComprasAgrupadas() {
      const start = (this.currentPage - 1) * this.pageSize;
      return this.comprasAgrupadas.slice(start, start + this.pageSize);
    },
    comprasAgrupadas() {
      const groups = [];
      const groupMap = {};

      this.filteredCompras.forEach(compra => {
        if (compra.grupoParcelaId) {
          if (!groupMap[compra.grupoParcelaId]) {
            groupMap[compra.grupoParcelaId] = {
              id: compra.grupoParcelaId,
              isGroup: true,
              descricao: compra.descricao,
              categoria: compra.categoria,
              categoriaId: compra.categoriaId,
              formaPagamento: compra.formaPagamento,
              totalParcelas: compra.totalParcelas,
              grupoParcelaId: compra.grupoParcelaId,
              data: compra.data,
              items: [],
              get valor() {
                return this.items.reduce((sum, item) => sum + item.valor, 0);
              }
            };
            groups.push(groupMap[compra.grupoParcelaId]);
          }
          groupMap[compra.grupoParcelaId].items.push(compra);
          if (compra.data < groupMap[compra.grupoParcelaId].data) {
            groupMap[compra.grupoParcelaId].data = compra.data;
          }
        } else {
          groups.push({
            ...compra,
            isGroup: false
          });
        }
      });

      return groups;
    }
  },
  methods: {
    sortBy(key) {
      if (this.sortKey === key) {
        this.sortAsc = !this.sortAsc;
      } else {
        this.sortKey = key;
        this.sortAsc = false;
      }
    },
    async showConfirm(message) {
      return new Promise((resolve) => {
        this.confirmModal.message = message;
        this.confirmModal.resolve = resolve;
        this.confirmModal.show = true;
      });
    },
    calcularParcelaPreview() {
      if (!this.formCompra.valor || !this.formCompra.totalParcelas) return 0;
      return this.formCompra.valor / this.formCompra.totalParcelas;
    },
    onValorFocus(event) {
      if (this.valorExibido === 'R$ 0,00' || this.formCompra.valor === 0) {
        this.valorExibido = 'R$ ';
        this.formCompra.valor = null;
      }
      this.$nextTick(() => {
        if (event && event.target) {
          const len = this.valorExibido.length;
          event.target.setSelectionRange(len, len);
        }
      });
    },
    onValorBlur() {
      if (this.valorExibido.trim() === 'R$' || this.formCompra.valor === null || this.formCompra.valor === 0) {
        this.valorExibido = 'R$ 0,00';
        this.formCompra.valor = null;
      } else {
        this.valorExibido = this.formatCurrency(this.formCompra.valor);
      }
    },
    onValorInput(event) {
      let valStr = event.target.value.replace(/\D/g, '');
      if (!valStr) {
        this.valorExibido = 'R$ ';
        this.formCompra.valor = null;
        return;
      }
      let centavos = parseInt(valStr, 10);
      let floatVal = centavos / 100;
      this.formCompra.valor = floatVal;
      this.valorExibido = 'R$ ' + floatVal.toLocaleString('pt-BR', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
    },
    onValorClick(event) {
      if (event && event.target) {
        const len = this.valorExibido.length;
        event.target.setSelectionRange(len, len);
      }
    },
    onValorKeyup(event) {
      if (event && event.target) {
        const len = this.valorExibido.length;
        event.target.setSelectionRange(len, len);
      }
    },
    getTodayDateString() {
      return new Date(Date.now() - new Date().getTimezoneOffset() * 60000).toISOString().split('T')[0];
    },
    formatDate(dateStr) {
      if (!dateStr) return '';
      const datePart = dateStr.split('T')[0];
      const [year, month, day] = datePart.split('-');
      return `${day}/${month}/${year}`;
    },
    formatCurrency(val) {
      return new Intl.NumberFormat('pt-BR', {
        style: 'currency',
        currency: 'BRL'
      }).format(val);
    },
    async fetchWithAuth(url, options = {}) {
      const authStore = useAuthStore();
      const headers = { ...options.headers };
      if (authStore.token) {
        headers['Authorization'] = `Bearer ${authStore.token}`;
      }
      
      const response = await fetch(url, { ...options, headers });
      if (response.status === 401) {
        authStore.clearToken();
        this.$router.push('/login');
      }
      return response;
    },
    async fetchCategorias() {
      try {
        const res = await this.fetchWithAuth(`${API_BASE}/categorias`);
        if (res.ok) {
          this.categorias = await res.json();
        }
      } catch (err) {
        console.error('Erro ao buscar categorias:', err);
      }
    },
    async fetchCompras() {
      try {
        this.isLoading = true;
        let url = `${API_BASE}/compras`;
        const params = [];
        
        if (this.filtroTipo === 'mes' && this.filtroMesAno) {
          const [ano, mes] = this.filtroMesAno.split('-');
          params.push(`mes=${parseInt(mes, 10)}`);
          params.push(`ano=${parseInt(ano, 10)}`);
        } else if (this.filtroTipo === 'periodo') {
          if (this.filtroDataInicio) params.push(`inicio=${this.filtroDataInicio}`);
          if (this.filtroDataFim) params.push(`fim=${this.filtroDataFim}`);
        }
        
        if (params.length > 0) {
          url += '?' + params.join('&');
        }
        
        const res = await this.fetchWithAuth(url);
        if (res.ok) {
          this.compras = await res.json();
        }
      } catch (err) {
        console.error('Erro ao buscar compras:', err);
      } finally {
        this.isLoading = false;
      }
    },
    setFiltroTipo(tipo) {
      this.filtroTipo = tipo;
      this.fetchCompras();
    },
    resetFilters() {
      this.filtroTipo = 'mes';
      this.filtroMesAno = new Date(Date.now() - new Date().getTimezoneOffset() * 60000).toISOString().substring(0, 7);
      this.filtroDataInicio = this.getTodayDateString();
      this.filtroDataFim = this.getTodayDateString();
      this.fetchCompras();
    },
    isGroupExpanded(id) {
      return !!this.expandedGroups[id];
    },
    toggleGroup(id) {
      this.expandedGroups[id] = !this.expandedGroups[id];
    },
    getFormaPagamentoLabel(fp) {
      const labels = {
        'debito': 'Débito',
        'pix': 'Pix',
        'cartao_avista': 'Cartão à Vista',
        'cartao_parcelado': 'Parcelado'
      };
      return labels[fp] || fp;
    },
    getFormaPagamentoIcon(fp) {
      const icons = {
        'debito': '💳',
        'pix': '📱',
        'cartao_avista': '💳',
        'cartao_parcelado': '💳'
      };
      return icons[fp] || '💰';
    },
    async saveCompra() {
      if (!this.isDescricaoValid || !this.isValorValid || !this.isCategoriaValid || !this.isDataValid) {
        window.showToast('Por favor, preencha o formulário corretamente.', 'warning');
        return;
      }

      try {
        this.isSubmitting = true;
        const payload = {
          descricao: this.formCompra.descricao.trim(),
          valorTotal: this.formCompra.valor,
          data: this.formCompra.data,
          categoriaId: parseInt(this.formCompra.categoriaId, 10),
          formaPagamento: this.formCompra.formaPagamento,
          totalParcelas: this.formCompra.formaPagamento === 'cartao_parcelado' ? parseInt(this.formCompra.totalParcelas, 10) : null
        };

        let res;
        if (this.editMode) {
          // Edição
          const url = `${API_BASE}/compras/${this.formCompra.id}`;
          res = await this.fetchWithAuth(url, {
            method: 'PUT',
            body: JSON.stringify({
              ...payload,
              id: this.formCompra.id,
              valor: payload.valorTotal,
              data: payload.data
            })
          });
        } else {
          // Criação
          const url = `${API_BASE}/compras`;
          res = await this.fetchWithAuth(url, {
            method: 'POST',
            body: JSON.stringify(payload)
          });
        }

        if (res.ok) {
          window.showToast(this.editMode ? 'Compra atualizada com sucesso!' : 'Compra registrada com sucesso!', 'success');
          this.cancelEdit();
          await this.fetchCompras();
        } else {
          const errMsg = await res.text();
          window.showToast(errMsg || 'Erro ao salvar a compra.', 'error');
        }
      } catch (err) {
        console.error('Erro ao salvar compra:', err);
        window.showToast('Erro de conexão com o servidor.', 'error');
      } finally {
        this.isSubmitting = false;
      }
    },
    startEdit(compra) {
      this.editMode = true;
      this.formCompra.id = compra.id;
      this.formCompra.descricao = compra.descricao;
      this.formCompra.valor = compra.valor;
      this.formCompra.data = compra.data.split('T')[0];
      this.formCompra.categoriaId = compra.categoriaId;
      this.formCompra.formaPagamento = compra.formaPagamento;
      this.formCompra.totalParcelas = compra.totalParcelas;
      this.formCompra.numeroParcela = compra.numeroParcela;
      this.formCompra.grupoParcelaId = compra.grupoParcelaId;
      this.valorExibido = this.formatCurrency(compra.valor);

      this.$nextTick(() => {
        const inputDesc = document.getElementById('descricao');
        if (inputDesc) {
          inputDesc.focus();
        }
        window.scrollTo({
          top: 0,
          behavior: 'smooth'
        });
      });
    },
    cancelEdit() {
      this.editMode = false;
      this.formCompra.id = null;
      this.formCompra.descricao = '';
      this.formCompra.valor = null;
      this.formCompra.data = this.getTodayDateString();
      this.formCompra.categoriaId = '';
      this.formCompra.formaPagamento = 'pix';
      this.formCompra.totalParcelas = null;
      this.formCompra.numeroParcela = null;
      this.formCompra.grupoParcelaId = null;
      this.valorExibido = 'R$ 0,00';
    },
    async deleteCompra(compra) {
      let msg = 'Tem certeza que deseja excluir esta compra?';
      if (compra.isGroup) {
        msg = `Tem certeza que deseja excluir todas as ${compra.totalParcelas} parcelas deste parcelamento?`;
      }
      const confirmed = await this.showConfirm(msg);
      if (!confirmed) return;

      try {
        const id = compra.isGroup ? compra.items[0].id : compra.id;
        const res = await this.fetchWithAuth(`${API_BASE}/compras/${id}`, {
          method: 'DELETE'
        });

        if (res.ok) {
          window.showToast(compra.isGroup ? 'Parcelamento excluído com sucesso!' : 'Compra excluída com sucesso!', 'success');
          await this.fetchCompras();
        } else {
          const errMsg = await res.text();
          window.showToast(errMsg || 'Erro ao excluir a compra.', 'error');
        }
      } catch (err) {
        console.error('Erro ao excluir compra:', err);
        window.showToast('Erro de conexão ao excluir.', 'error');
      }
    },
    async saveCategoria() {
      if (!this.formCat.nome.trim()) return;
      try {
        this.isSubmittingCat = true;
        const payload = {
          nome: this.formCat.nome.trim(),
          icone: this.formCat.icone,
          corHex: this.formCat.corHex
        };

        let res;
        if (this.editCatMode) {
          res = await this.fetchWithAuth(`${API_BASE}/categorias/${this.formCat.id}`, {
            method: 'PUT',
            body: JSON.stringify(payload)
          });
        } else {
          res = await this.fetchWithAuth(`${API_BASE}/categorias`, {
            method: 'POST',
            body: JSON.stringify(payload)
          });
        }

        if (res.ok) {
          window.showToast(this.editCatMode ? 'Categoria atualizada!' : 'Categoria criada!', 'success');
          this.cancelCatEdit();
          await this.fetchCategorias();
          await this.fetchCompras();
        } else {
          const errMsg = await res.text();
          window.showToast(errMsg || 'Erro ao salvar categoria.', 'error');
        }
      } catch (err) {
        console.error('Erro ao salvar categoria:', err);
      } finally {
        this.isSubmittingCat = false;
      }
    },
    startCatEdit(cat) {
      this.editCatMode = true;
      this.formCat.id = cat.id;
      this.formCat.nome = cat.nome;
      this.formCat.icone = cat.icone;
      this.formCat.corHex = cat.corHex;
    },
    cancelCatEdit() {
      this.editCatMode = false;
      this.formCat.id = null;
      this.formCat.nome = '';
      this.formCat.icone = '📦';
      this.formCat.corHex = '#9E9E9E';
    },
    async deleteCategoria(id) {
      const confirmed = await this.showConfirm('Tem certeza que deseja excluir esta categoria? Apenas categorias sem lançamentos vinculados podem ser excluídas.');
      if (!confirmed) return;

      try {
        const res = await this.fetchWithAuth(`${API_BASE}/categorias/${id}`, {
          method: 'DELETE'
        });
        if (res.ok) {
          window.showToast('Categoria excluída com sucesso!', 'success');
          await this.fetchCategorias();
        } else {
          const errMsg = await res.text();
          window.showToast(errMsg || 'Não é possível excluir esta categoria.', 'warning');
        }
      } catch (err) {
        console.error('Erro ao excluir categoria:', err);
        window.showToast('Erro ao excluir a categoria.', 'error');
      }
    },
    openCategoryModal() {
      this.categoryModalOpen = true;
    },
    closeCategoryModal() {
      this.categoryModalOpen = false;
      this.cancelCatEdit();
    }
  },
  async mounted() {
    await this.fetchCategorias();
    await this.fetchCompras();
    this.isLoading = false;

    this.$nextTick(() => {
      const inputDesc = document.getElementById('descricao');
      if (inputDesc) {
        inputDesc.focus();
      }
    });
  },
  beforeUnmount() {
    document.body.classList.remove('modal-open');
  }
}
</script>

<style>
/* Os estilos globais e utilitários são gerenciados centralmente pelo src/style.css */
</style>
