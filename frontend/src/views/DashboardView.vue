<template>
  <div class="app-container">
    <!-- Header Component -->
    <Header :is-online="isOnline" />

    <main class="main-content">
      <!-- DASHBOARD STATS & CHARTS SECTION -->
      <section class="dashboard-grid animate-fade-in">
        <!-- Stats Cards -->
        <div class="stats-sidebar">
          <div class="glass-panel glass-panel-hover stat-card">
            <span class="stat-icon">💰</span>
            <div class="stat-info">
              <span class="stat-label">Total de Gastos</span>
              <span v-if="isLoading" class="skeleton-block skeleton-text-lg"></span>
              <span v-else class="stat-value">{{ formatCurrency(totalGastos) }}</span>
            </div>
          </div>

          <div class="glass-panel glass-panel-hover stat-card">
            <span class="stat-icon">📊</span>
            <div class="stat-info">
              <span class="stat-label">Média por Compra</span>
              <span v-if="isLoading" class="skeleton-block skeleton-text-lg"></span>
              <span v-else class="stat-value">{{ formatCurrency(mediaPorCompra) }}</span>
            </div>
          </div>

          <div class="glass-panel glass-panel-hover stat-card">
            <span class="stat-icon">🏷️</span>
            <div class="stat-info">
              <span class="stat-label">Total de Lançamentos</span>
              <span v-if="isLoading" class="skeleton-block skeleton-text-lg"></span>
              <span v-else class="stat-value">{{ totalLancamentos }}</span>
            </div>
          </div>

          <button class="btn btn-secondary manage-categories-btn" @click="openCategoryModal">
            ⚙️ Gerenciar Categorias
          </button>
        </div>

        <!-- SVG Donut Chart (Zero Dependency Premium Chart) -->
        <div class="glass-panel chart-card">
          <h3 class="card-title">Distribuição por Categoria</h3>
          
          <div v-if="compras.length === 0" class="empty-chart-state">
            <span>📊</span>
            <p>Nenhuma compra registrada para exibir o gráfico.</p>
          </div>
          
          <div v-else class="chart-content">
            <!-- SVG Donut -->
            <div class="svg-container">
              <svg viewBox="0 0 140 140" class="donut-svg">
                <circle cx="70" cy="70" r="50" class="donut-bg" />
                
                <circle 
                  v-for="(slice, index) in chartSlices" 
                  :key="index"
                  cx="70" 
                  cy="70" 
                  r="50" 
                  class="donut-segment"
                  :class="{ active: hoveredSlice && hoveredSlice.id === slice.id }"
                  :stroke="slice.cor"
                  :stroke-dasharray="animateChart ? `${slice.size} 314.16` : `0 314.16`"
                  :stroke-dashoffset="slice.offset"
                  stroke-width="12"
                  fill="transparent"
                  @mouseenter="hoveredSlice = slice"
                  @mouseleave="hoveredSlice = null"
                  @touchstart.passive="hoveredSlice = slice"
                  @touchend.passive="hoveredSlice = null"
                />
                
                <!-- Central Text -->
                <g class="donut-center-text" :class="{ 'has-hover': hoveredSlice }">
                  <text x="70" y="65" class="donut-center-val" :style="{ fill: hoveredSlice ? hoveredSlice.cor : 'var(--text-primary)' }">
                    {{ hoveredSlice ? formatCurrency(hoveredSlice.valor) : formatCurrency(totalGastos) }}
                  </text>
                  <text x="70" y="79" class="donut-center-lbl">
                    {{ hoveredSlice ? `${hoveredSlice.icone} ${hoveredSlice.nome}` : 'Total Filtrado' }}
                  </text>
                  <text x="70" y="89" class="donut-center-pct">
                    {{ hoveredSlice ? `${hoveredSlice.pct}% do total` : `${compras.length} lançamentos` }}
                  </text>
                </g>
              </svg>
            </div>

            <!-- Legend Grid -->
            <div class="chart-legend">
              <div 
                v-for="item in chartLegendItems" 
                :key="item.id" 
                class="legend-item"
              >
                <span class="legend-dot" :style="{ backgroundColor: item.cor }"></span>
                <span class="legend-emoji">{{ item.icone }}</span>
                <span class="legend-name">{{ item.nome }}</span>
                <span class="legend-value">{{ formatCurrency(item.valor) }}</span>
                <span class="legend-pct">{{ item.pct }}%</span>
              </div>
            </div>
          </div>
        </div>
      </section>

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
                  :class="{ active: formCompra.formaPagamento === 'dinheiro' }"
                  @click="formCompra.formaPagamento = 'dinheiro'; formCompra.totalParcelas = null"
                  :disabled="isSubmitting"
                >
                  💵 Dinheiro
                </button>
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
          <input 
            type="text" 
            v-model="searchQuery" 
            class="input-control search-bar" 
            placeholder="Buscar por descrição..." 
          />
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
                  <!-- Do not allow editing/deleting standard seeded category names for safety if preferred, but we support full CRUD -->
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
import Header from '../components/Header.vue';
import { useAuthStore } from '../stores/auth';

const BASE_URL = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5076';
const API_BASE = `${BASE_URL}/api`;

export default {
  name: 'App',
  components: {
    Header
  },
  data() {
    return {
      isOnline: false,
      categorias: [],
      compras: [],
      
      // Filtros
      filtroTipo: 'mes',
      filtroMesAno: new Date().toISOString().substring(0, 7),
      filtroDataInicio: new Date().toISOString().split('T')[0],
      filtroDataFim: new Date().toISOString().split('T')[0],
      
      // Form Compra
      formCompra: {
        id: null,
        descricao: '',
        valor: null,
        data: new Date().toISOString().split('T')[0],
        categoriaId: '',
        formaPagamento: 'dinheiro',
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
      hoveredSlice: null,
      animateChart: false,

      // Phase 4: Busca debounced
      searchQuery: '',
      debouncedSearchQuery: '',
      searchDebounceTimer: null,

      // Phase 4: Ordenação
      sortKey: 'data',
      sortAsc: false,

      // Phase 4: Modal de confirmação customizado
      confirmModal: {
        show: false,
        message: '',
        resolve: null
      },

      // Phase 4: Skeleton loader
      isLoading: true,

      // Paginação
      currentPage: 1,
      pageSize: 10
    }
  },
  watch: {
    // 1.2: Debounce de 300ms para searchQuery
    searchQuery(newVal) {
      clearTimeout(this.searchDebounceTimer);
      this.searchDebounceTimer = setTimeout(() => {
        this.debouncedSearchQuery = newVal;
        this.currentPage = 1;
      }, 300);
    },
    // Resetar página ao mudar dados
    compras() {
      this.currentPage = 1;
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
      // 1. Exclude the item being edited
      let list = this.editMode && this.formCompra.id
        ? this.compras.filter(c => c.id !== this.formCompra.id)
        : [...this.compras];

      // 1.4: Filter by debounced search query (case-insensitive on descricao)
      const q = (this.debouncedSearchQuery || '').trim().toLowerCase();
      if (q) {
        list = list.filter(c => c.descricao && c.descricao.toLowerCase().includes(q));
      }

      // 2.3: Sort by sortKey in sortAsc direction
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
    totalGastos() {
      return this.compras.reduce((sum, c) => sum + c.valor, 0);
    },
    totalLancamentos() {
      return this.compras.length;
    },
    mediaPorCompra() {
      if (this.compras.length === 0) return 0;
      return this.totalGastos / this.compras.length;
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
    
    // SVG Donut Slices Calculations
    chartLegendItems() {
      if (this.compras.length === 0) return [];
      
      // Group by Category
      const groups = {};
      this.compras.forEach(compra => {
        const cat = compra.categoria || { id: 0, nome: 'Sem Categoria', icone: '❓', corHex: '#999' };
        if (!groups[cat.id]) {
          groups[cat.id] = {
            id: cat.id,
            nome: cat.nome,
            icone: cat.icone,
            cor: cat.corHex,
            valor: 0
          };
        }
        groups[cat.id].valor += compra.valor;
      });

      const total = this.totalGastos;
      return Object.values(groups)
        .map(g => {
          const pct = total > 0 ? ((g.valor / total) * 100).toFixed(1) : '0.0';
          return {
            ...g,
            pct: parseFloat(pct)
          };
        })
        .sort((a, b) => b.valor - a.valor);
    },
    chartSlices() {
      const legend = this.chartLegendItems;
      const slices = [];
      let cumulativePercentage = 0;
      
      // Circumference of our donut is 2 * PI * 50 = 314.16
      const CIRCUMFERENCE = 314.16;

      legend.forEach(item => {
        const pct = item.pct;
        const size = (pct * CIRCUMFERENCE) / 100;
        
        // Offset starts at 0, goes backwards around the circle
        const offset = -(cumulativePercentage * CIRCUMFERENCE) / 100;
        
        slices.push({
          id: item.id,
          nome: item.nome,
          icone: item.icone,
          valor: item.valor,
          pct: item.pct,
          cor: item.cor,
          size: size,
          offset: offset
        });
        
        cumulativePercentage += pct;
      });

      return slices;
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
    // Phase 4: Sorting
    sortBy(key) {
      if (this.sortKey === key) {
        this.sortAsc = !this.sortAsc;
      } else {
        this.sortKey = key;
        this.sortAsc = false; // default desc for new sort
      }
    },

    // Phase 4: Custom Confirm Modal
    showConfirm(message) {
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
    getFormaPagamentoLabel(type) {
      const labels = {
        dinheiro: 'Dinheiro',
        debito: 'Débito',
        pix: 'Pix',
        cartao_avista: 'Cartão à Vista',
        cartao_parcelado: 'Cartão Parcelado'
      };
      return labels[type] || 'Não Informado';
    },
    getFormaPagamentoIcon(type) {
      const icons = {
        dinheiro: '💵',
        debito: '💳',
        pix: '📱',
        cartao_avista: '💳',
        cartao_parcelado: '💳'
      };
      return icons[type] || '❓';
    },
    toggleGroup(groupId) {
      this.expandedGroups[groupId] = !this.expandedGroups[groupId];
    },
    isGroupExpanded(groupId) {
      return !!this.expandedGroups[groupId];
    },

    // Currency Input Formatter Helpers
    formatDecimalBrl(val) {
      if (val === null || val === undefined) return '';
      return val.toLocaleString('pt-BR', {
        minimumFractionDigits: 2,
        maximumFractionDigits: 2
      });
    },
    onValorFocus(event) {
      this.$nextTick(() => {
        if (event && event.target) {
          const len = this.valorExibido.length;
          event.target.setSelectionRange(len, len);
        }
      });
    },
    onValorBlur() {
      // O valor já está sempre perfeitamente formatado
    },
    onValorInput(event) {
      let val = this.valorExibido || '';
      
      // Extrai apenas os dígitos numéricos
      let cleaned = val.replace(/\D/g, '');
      
      // Remove zeros à esquerda adicionais
      cleaned = cleaned.replace(/^0+/, '');
      
      if (!cleaned) {
        this.formCompra.valor = 0;
        this.valorExibido = 'R$ 0,00';
      } else {
        const centavos = parseInt(cleaned, 10);
        this.formCompra.valor = centavos / 100;
        this.valorExibido = this.formatCurrency(this.formCompra.valor);
      }
      
      // Força a seleção do cursor a ficar sempre no final do texto
      this.$nextTick(() => {
        if (event && event.target) {
          const len = this.valorExibido.length;
          event.target.setSelectionRange(len, len);
        }
      });
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

    // Utility Dates Helper
    getTodayDateString() {
      const today = new Date();
      return today.toISOString().split('T')[0];
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

    // Check API Status
    async checkApiStatus() {
      try {
        const res = await fetch(BASE_URL);
        this.isOnline = res.ok;
      } catch (err) {
        this.isOnline = false;
      }
    },

    // Wrapper for authenticated fetch calls
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

    // Fetch Categories
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

    // Fetch Purchases
    async fetchCompras() {
      try {
        let url = `${API_BASE}/compras`;
        const params = [];
        
        if (this.filtroTipo === 'periodo') {
          if (this.filtroDataInicio && this.filtroDataInicio !== 'null' && this.filtroDataInicio !== 'undefined') {
            params.push(`inicio=${this.filtroDataInicio}`);
          }
          if (this.filtroDataFim && this.filtroDataFim !== 'null' && this.filtroDataFim !== 'undefined') {
            params.push(`fim=${this.filtroDataFim}`);
          }
        } else if (this.filtroTipo === 'mes' && this.filtroMesAno) {
          const [year, month] = this.filtroMesAno.split('-');
          params.push(`mes=${parseInt(month)}`);
          params.push(`ano=${parseInt(year)}`);
        }

        if (params.length > 0) {
          url += `?${params.join('&')}`;
        }

        const res = await this.fetchWithAuth(url);
        if (res.ok) {
          this.compras = await res.json();
        }
      } catch (err) {
        console.error('Erro ao buscar compras:', err);
      }
    },

    // Filter Change Event Handlers
    onFiltroMesAnoChange() {
      this.fetchCompras();
    },
    onFiltroDataChange() {
      this.fetchCompras();
    },
    setFiltroTipo(tipo) {
      this.filtroTipo = tipo;
    },
    resetFilters() {
      this.filtroTipo = 'mes';
      this.filtroMesAno = new Date().toISOString().substring(0, 7);
      this.filtroDataInicio = new Date().toISOString().split('T')[0];
      this.filtroDataFim = new Date().toISOString().split('T')[0];
      this.fetchCompras();
    },

    // Save Purchase (Add or Update)
    async saveCompra() {
      try {
        const valorParseado = this.formCompra.valor;
        
        if (typeof valorParseado !== 'number' || isNaN(valorParseado) || valorParseado <= 0) {
          alert('Por favor, insira um valor numérico válido maior que zero.');
          return;
        }

        this.isSubmitting = true;

        const url = this.editMode 
          ? `${API_BASE}/compras/${this.formCompra.id}`
          : `${API_BASE}/compras`;
        
        const method = this.editMode ? 'PUT' : 'POST';
        
        let bodyPayload;
        if (this.editMode) {
          bodyPayload = {
            id: this.formCompra.id,
            descricao: this.formCompra.descricao,
            valor: valorParseado,
            data: this.formCompra.data + "T12:00:00",
            categoriaId: this.formCompra.categoriaId
          };
        } else {
          bodyPayload = {
            descricao: this.formCompra.descricao,
            valorTotal: valorParseado,
            data: this.formCompra.data + "T12:00:00",
            categoriaId: this.formCompra.categoriaId,
            formaPagamento: this.formCompra.formaPagamento,
            totalParcelas: this.formCompra.totalParcelas ? parseInt(this.formCompra.totalParcelas, 10) : null
          };
        }

        const res = await this.fetchWithAuth(url, {
          method: method,
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify(bodyPayload)
        });

        if (res.ok) {
          const isEdit = this.editMode;
          this.cancelEdit();
          await this.fetchCompras();
          window.showToast(isEdit ? 'Compra atualizada com sucesso!' : 'Compra cadastrada com sucesso!', 'success');
        } else {
          const errMsg = await res.text();
          window.showToast(errMsg || 'Erro ao salvar a compra.', 'error');
        }
      } catch (err) {
        console.error('Erro ao salvar compra:', err);
        window.showToast('Erro ao salvar a compra.', 'error');
      } finally {
        this.isSubmitting = false;
      }
    },

    startEdit(compra) {
      this.editMode = true;
      this.formCompra.id = compra.id;
      this.formCompra.descricao = compra.descricao;
      this.formCompra.valor = compra.valor;
      this.valorExibido = this.formatCurrency(compra.valor);
      this.formCompra.data = compra.data ? compra.data.split('T')[0] : this.getTodayDateString();
      this.formCompra.categoriaId = compra.categoriaId;
      this.formCompra.formaPagamento = compra.formaPagamento;
      this.formCompra.totalParcelas = compra.totalParcelas;
      this.formCompra.numeroParcela = compra.numeroParcela;
      this.formCompra.grupoParcelaId = compra.grupoParcelaId;

      // Rolagem suave até o formulário e foco automático para feedback visual imediato
      this.$nextTick(() => {
        const inputDesc = document.getElementById('descricao');
        if (inputDesc) {
          inputDesc.scrollIntoView({ behavior: 'smooth', block: 'center' });
          inputDesc.focus();
        }
      });
    },

    cancelEdit() {
      this.editMode = false;
      this.formCompra.id = null;
      this.formCompra.descricao = '';
      this.formCompra.valor = null;
      this.valorExibido = 'R$ 0,00';
      this.formCompra.data = new Date().toISOString().split('T')[0];
      this.formCompra.categoriaId = '';
      this.formCompra.formaPagamento = 'dinheiro';
      this.formCompra.totalParcelas = null;
      this.formCompra.numeroParcela = null;
      this.formCompra.grupoParcelaId = null;
    },

    async deleteCompra(item) {
      let confirmed;
      if (item.isGroup || (item.formaPagamento === 'cartao_parcelado' && item.grupoParcelaId)) {
        confirmed = await this.showConfirm(`Esta compra é parcelada (${item.totalParcelas || item.items[0]?.totalParcelas}x). Deseja realmente excluir TODAS as parcelas deste grupo?`);
      } else {
        confirmed = await this.showConfirm('Deseja realmente excluir esta compra?');
      }
      
      if (!confirmed) return;
      
      const id = item.isGroup ? item.items[0].id : item.id;
      
      try {
        const res = await this.fetchWithAuth(`${API_BASE}/compras/${id}`, {
          method: 'DELETE'
        });

        if (res.ok) {
          await this.fetchCompras();
          window.showToast('Compra excluída com sucesso!', 'success');
        } else {
          window.showToast('Erro ao excluir a compra.', 'error');
        }
      } catch (err) {
        console.error('Erro ao excluir compra:', err);
        window.showToast('Erro ao excluir a compra.', 'error');
      }
    },

    // Category Management Methods
    openCategoryModal() {
      this.categoryModalOpen = true;
      this.cancelCatEdit();
    },
    closeCategoryModal() {
      this.categoryModalOpen = false;
    },
    async saveCategoria() {
      if (!this.formCat.icone || !this.formCat.icone.trim()) {
        window.showToast('O ícone da categoria é obrigatório.', 'error');
        return;
      }
      try {
        this.isSubmittingCat = true;
        const url = this.editCatMode 
          ? `${API_BASE}/categorias/${this.formCat.id}`
          : `${API_BASE}/categorias`;
        
        const method = this.editCatMode ? 'PUT' : 'POST';
        
        const res = await this.fetchWithAuth(url, {
          method: method,
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify({
            nome: this.formCat.nome,
            icone: this.formCat.icone,
            corHex: this.formCat.corHex
          })
        });

        if (res.ok) {
          const isEdit = this.editCatMode;
          this.cancelCatEdit();
          await this.fetchCategorias();
          await this.fetchCompras(); // Atualiza cores/ícones na lista e dashboard
          window.showToast(isEdit ? 'Categoria atualizada com sucesso!' : 'Categoria criada com sucesso!', 'success');
        } else {
          const errMsg = await res.text();
          window.showToast(errMsg || 'Erro ao salvar a categoria.', 'error');
        }
      } catch (err) {
        console.error('Erro ao salvar categoria:', err);
        window.showToast('Erro ao salvar a categoria.', 'error');
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
      const confirmed = await this.showConfirm('Deseja realmente excluir esta categoria?');
      if (!confirmed) return;

      try {
        const res = await this.fetchWithAuth(`${API_BASE}/categorias/${id}`, {
          method: 'DELETE'
        });

        if (res.ok) {
          await this.fetchCategorias();
          window.showToast('Categoria excluída com sucesso!', 'success');
        } else {
          const errMsg = await res.text();
          window.showToast(errMsg || 'Erro ao excluir a categoria.', 'error');
        }
      } catch (err) {
        console.error('Erro ao excluir categoria:', err);
        window.showToast('Erro ao excluir a categoria.', 'error');
      }
    }
  },
  async mounted() {
    // Inicialização da aplicação
    await this.checkApiStatus();
    if (this.isOnline) {
      await this.fetchCategorias();
      await this.fetchCompras();
    }
    this.isLoading = false;
    
    // Ativa animação de entrada do gráfico donut
    setTimeout(() => {
      this.animateChart = true;
    }, 150);
    // Loop de verificação de status do servidor a cada 10 segundos
    setInterval(async () => {
      await this.checkApiStatus();
    }, 10000);

    // Foco automático programático no campo de descrição para início rápido da digitação
    this.$nextTick(() => {
      const inputDesc = document.getElementById('descricao');
      if (inputDesc) {
        inputDesc.focus();
      }
    });
  }
}
</script>

<style>
/* Global layouts details */
.app-container {
  max-width: 1200px;
  margin: 0 auto;
  padding: 2rem 1.5rem;
  min-height: 100vh;
  display: flex;
  flex-direction: column;
}

.main-content {
  flex: 1;
  display: flex;
  flex-direction: column;
  gap: 2rem;
}

/* Common Components */
.card-title {
  font-size: 1.15rem;
  font-weight: 600;
  color: var(--text-primary);
  margin-bottom: 1.25rem;
  border-bottom: 1px solid var(--border-glass);
  padding-bottom: 0.5rem;
  text-transform: uppercase;
  letter-spacing: 0.05em;
}

/* SECTION 1: DASHBOARD GRID */
.dashboard-grid {
  display: grid;
  grid-template-columns: 320px 1fr;
  gap: 2rem;
}

.stats-sidebar {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.stat-card {
  display: flex;
  align-items: center;
  gap: 1.25rem;
  padding: 1.25rem;
  background: var(--bg-card);
}

.stat-icon {
  font-size: 2rem;
  background: var(--surface-2);
  padding: 0.5rem;
  border-radius: var(--radius-sm);
  border: 1px solid var(--border-glass);
}

.stat-info {
  display: flex;
  flex-direction: column;
}

.stat-label {
  font-size: 0.8rem;
  color: var(--text-secondary);
  text-transform: uppercase;
  letter-spacing: 0.05em;
}

.stat-value {
  font-size: 1.5rem;
  font-weight: 700;
  color: var(--text-primary);
}

.manage-categories-btn {
  width: 100%;
  padding: 0.85rem;
  font-size: 0.95rem;
  margin-top: 0.5rem;
}

/* SVG Chart Card */
.chart-card {
  padding: 1.5rem;
  min-height: 280px;
}

.empty-chart-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  height: 200px;
  color: var(--text-secondary);
  text-align: center;
}

.empty-chart-state span {
  font-size: 3rem;
  margin-bottom: 1rem;
  opacity: 0.5;
}

.chart-content {
  display: grid;
  grid-template-columns: 220px 1fr;
  gap: 2.5rem;
  align-items: center;
}

.svg-container {
  position: relative;
  width: 100%;
}

.donut-svg {
  transform: rotate(-90deg);
}

.donut-bg {
  stroke: var(--border-glass);
  stroke-width: 10;
  fill: transparent;
}

.donut-segment {
  stroke-linecap: round;
  transition: stroke-dasharray var(--transition-slow), stroke-width var(--transition-fast), filter var(--transition-fast);
  cursor: pointer;
}

.donut-segment:hover,
.donut-segment.active {
  stroke-width: 14;
  filter: drop-shadow(0 0 6px rgba(255,255,255,0.15));
}

.donut-center-text {
  transform: rotate(90deg);
  transform-origin: center;
  transition: all var(--transition-fast);
}

.donut-center-text.has-hover .donut-center-val {
  font-size: 11px;
}

.donut-center-val {
  fill: var(--text-primary);
  font-size: 10px;
  font-weight: 700;
  text-anchor: middle;
  transition: fill var(--transition-fast), font-size var(--transition-fast);
}

.donut-center-lbl {
  fill: var(--text-secondary);
  font-size: 6px;
  text-transform: uppercase;
  letter-spacing: 0.05em;
  text-anchor: middle;
  transition: color var(--transition-fast);
}

.donut-center-pct {
  fill: var(--text-muted);
  font-size: 4.5px;
  text-transform: uppercase;
  letter-spacing: 0.03em;
  text-anchor: middle;
}

.chart-legend {
  display: flex;
  flex-direction: column;
  gap: 0.6rem;
  max-height: 240px;
  overflow-y: auto;
  padding-right: 0.5rem;
}

.legend-item {
  display: flex;
  align-items: center;
  gap: 0.6rem;
  font-size: 0.85rem;
  padding: 0.45rem 0.75rem;
  border-radius: var(--radius-sm);
  background: var(--surface-1);
  border: 1px solid var(--border-glass);
  transition: background-color var(--transition-fast), border-color var(--transition-fast), transform var(--transition-fast);
}

.legend-item:hover {
  background: var(--surface-2);
  border-color: var(--border-glass-focus);
  transform: translateX(2px);
}

.legend-dot {
  width: 10px;
  height: 10px;
  border-radius: 50%;
  flex-shrink: 0;
}

.legend-emoji {
  font-size: 1.1rem;
  flex-shrink: 0;
}

.legend-name {
  color: var(--text-primary);
  font-weight: 500;
  margin-right: auto;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
  min-width: 0;
}

.legend-value {
  color: var(--text-secondary);
  font-weight: 600;
  margin-right: 0.5rem;
  white-space: nowrap;
  flex-shrink: 0;
}

.legend-pct {
  font-size: 0.72rem;
  background: var(--surface-3);
  padding: 0.12rem 0.35rem;
  border-radius: 4px;
  color: var(--text-secondary);
  flex-shrink: 0;
}

/* SECTION 2: INTERACTION GRID */
.interaction-grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 2rem;
}

.form-card, .filters-card {
  padding: 1.5rem;
}

.purchase-form {
  display: flex;
  flex-direction: column;
  gap: 1.25rem;
}

.form-group {
  display: flex;
  flex-direction: column;
}

.form-row {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 1rem;
}


[data-theme="light"] .btn-secondary {
  background: var(--surface-3);
  border-color: var(--border-glass);
}

[data-theme="light"] .btn-secondary:hover {
  background: var(--surface-4);
  border-color: hsla(var(--hue-base) 10% 50% / 0.15);
}

.form-actions {
  display: flex;
  justify-content: flex-end;
  gap: 1rem;
  margin-top: 0.5rem;
}

.submit-btn {
  flex: 1;
}

/* Filters Custom UI */
.filters-container {
  display: flex;
  flex-direction: column;
  gap: 1.25rem;
}

/* Segmented Control / Tabs */
.filter-tabs {
  display: flex;
  background: var(--bg-tabs);
  border: 1px solid var(--border-glass);
  padding: 4px;
  border-radius: var(--radius-sm);
  margin-bottom: 0.5rem;
}

.tab-btn {
  flex: 1;
  background: transparent;
  border: none;
  color: var(--text-secondary);
  font-family: inherit;
  font-size: 0.85rem;
  font-weight: 500;
  padding: 0.6rem 0.8rem;
  border-radius: calc(var(--radius-sm) - 4px);
  cursor: pointer;
  outline: none;
  transition: all var(--transition-fast);
  display: inline-flex;
  align-items: center;
  justify-content: center;
  gap: 0.4rem;
}

.tab-btn:hover {
  color: var(--text-primary);
  background: var(--surface-1);
}

.tab-btn.active {
  color: #fff;
  background: linear-gradient(135deg, var(--primary) 0%, hsl(263, 80%, 55%) 100%);
  box-shadow: 0 2px 10px hsla(263, 80%, 62%, 0.2);
}


.filter-actions-row {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 1rem;
}

.filter-submit-btn,
.clear-filters-btn {
  width: 100%;
  padding: 0.75rem;
}

.periodo-error-message {
  color: var(--danger, #ef4444);
  font-size: 0.8rem;
  margin-top: -0.5rem;
  margin-bottom: 0.5rem;
  font-weight: 500;
  display: flex;
  align-items: center;
  gap: 0.4rem;
}

.btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
  transform: none !important;
  box-shadow: none !important;
  filter: none !important;
}

/* SECTION 3: LIST SECTION */
.list-section {
  padding: 1.5rem;
}

.list-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1.5rem;
}

.list-header .card-title {
  margin-bottom: 0;
  border-bottom: none;
  padding-bottom: 0;
}

.count-badge {
  font-size: 0.8rem;
  font-weight: 600;
  padding: 0.35rem 0.75rem;
  border-radius: 99px;
  background: var(--surface-3);
  border: 1px solid var(--border-glass);
  color: var(--text-secondary);
}

.table-container {
  overflow-x: auto;
  border-radius: var(--radius-sm);
  border: 1px solid var(--border-glass);
}

.empty-table-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 4rem 2rem;
  color: var(--text-secondary);
  text-align: center;
}

.empty-table-state span {
  font-size: 3rem;
  margin-bottom: 1rem;
  opacity: 0.5;
}

.purchase-table {
  width: 100%;
  border-collapse: collapse;
  text-align: left;
}

.purchase-table th {
  background: var(--surface-1);
  padding: 1rem;
  font-size: 0.85rem;
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: 0.05em;
  color: var(--text-secondary);
  border-bottom: 1px solid var(--border-glass);
}

.purchase-table td {
  padding: 1rem;
  font-size: 0.95rem;
  border-bottom: 1px solid var(--border-glass);
  color: var(--text-primary);
  vertical-align: middle;
}

.table-row {
  transition: background-color var(--transition-fast);
}

.table-row:hover {
  background-color: var(--row-hover);
}

.category-tag {
  display: inline-flex;
  align-items: center;
  gap: 0.4rem;
  font-size: 0.8rem;
  font-weight: 500;
  padding: 0.35rem 0.75rem;
  border-radius: 6px;
  color: var(--text-primary);
}

.tag-icon {
  font-size: 1rem;
}

.text-right { text-align: right !important; }
.text-center { text-align: center !important; }

.col-actions {
  width: 100px;
}

.action-btn {
  background: transparent;
  border: none;
  cursor: pointer;
  font-size: 1rem;
  padding: 0.25rem;
  margin: 0 0.25rem;
  border-radius: 4px;
  transition: all var(--transition-fast);
}

.action-btn:hover {
  background: var(--surface-4);
  transform: scale(1.1);
}

/* SECTION 4: MODALS & OVERLAYS */
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  width: 100vw;
  height: 100vh;
  background: var(--overlay-bg);
  backdrop-filter: blur(8px);
  z-index: 1000;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 1rem;
}

.modal-content {
  width: 100%;
  max-width: 650px;
  background: var(--bg-card);
  border: 1px solid var(--border-glass);
  box-shadow: var(--shadow-glass);
  border-radius: var(--radius-md);
  padding: 2rem;
  position: relative;
}

.modal-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1.5rem;
  border-bottom: 1px solid var(--border-glass);
  padding-bottom: 0.75rem;
}

.modal-header h2 {
  font-size: 1.4rem;
  font-weight: 600;
  color: var(--text-primary);
}

.close-modal-btn {
  background: transparent;
  border: none;
  font-size: 2rem;
  cursor: pointer;
  color: var(--text-secondary);
  line-height: 1;
}

.close-modal-btn:hover {
  color: var(--text-primary);
}

.modal-body {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
}

.modal-form-box {
  background: var(--surface-1);
  border: 1px solid var(--border-glass);
  border-radius: var(--radius-sm);
  padding: 1.25rem;
}

.modal-form-box h4 {
  font-size: 0.95rem;
  color: var(--text-secondary);
  text-transform: uppercase;
  margin-bottom: 0.75rem;
  letter-spacing: 0.05em;
}

.form-row-cat {
  display: flex;
  gap: 0.75rem;
  align-items: center;
}

.form-row-cat .input-control {
  flex: 2;
}

.form-row-cat .icon-input {
  flex: 1;
  text-align: center;
}

.form-row-cat .color-input {
  width: 45px;
  height: 40px;
  padding: 2px;
  cursor: pointer;
  flex-shrink: 0;
}

.cat-form-actions {
  display: flex;
  gap: 0.5rem;
}

.btn-sm {
  padding: 0.5rem 1rem;
  font-size: 0.85rem;
}

.modal-cat-list h4 {
  font-size: 0.95rem;
  color: var(--text-secondary);
  text-transform: uppercase;
  margin-bottom: 0.75rem;
  letter-spacing: 0.05em;
}

.cat-list-container {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
  max-height: 200px;
  overflow-y: auto;
  padding-right: 0.25rem;
}

.cat-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 0.65rem 1rem;
  background: var(--surface-1);
  border: 1px solid var(--border-glass);
  border-radius: var(--radius-sm);
}

.cat-item-info {
  display: flex;
  align-items: center;
  gap: 0.75rem;
}

.cat-item-icon {
  font-size: 1.2rem;
}

.cat-item-name {
  font-weight: 500;
}

.cat-item-color-label {
  font-size: 0.75rem;
  color: var(--text-muted);
  font-family: monospace;
}

.cat-item-actions {
  display: flex;
  align-items: center;
}

/* Category Modal UX Enhancements */
.cat-form-grid {
  display: grid;
  grid-template-columns: 1.2fr 1fr;
  gap: 1.5rem;
}

@media (max-width: 600px) {
  .cat-form-grid {
    grid-template-columns: 1fr;
    gap: 1.25rem;
  }
}

.cat-form-left {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.cat-form-right {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.cat-preview {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  margin-top: auto;
  padding: 0.5rem 0.75rem;
  background: var(--surface-1);
  border: 1px dashed var(--border-glass);
  border-radius: var(--radius-sm);
}

.cat-preview-label {
  font-size: 0.8rem;
  color: var(--text-secondary);
  font-weight: 500;
  text-transform: uppercase;
  letter-spacing: 0.05em;
}

.color-selector-group {
  display: flex;
  flex-direction: column;
  gap: 0.4rem;
}

.color-grid {
  display: flex;
  flex-wrap: wrap;
  gap: 0.4rem;
  margin-top: 0.25rem;
}

.color-btn {
  width: 24px;
  height: 24px;
  border-radius: 50%;
  border: 2px solid transparent;
  cursor: pointer;
  transition: all var(--transition-fast);
  box-shadow: 0 1.5px 3px rgba(0, 0, 0, 0.15);
  display: flex;
  align-items: center;
  justify-content: center;
}

.color-btn:hover {
  transform: scale(1.15);
  box-shadow: 0 3px 6px rgba(0, 0, 0, 0.25);
}

.color-btn.active {
  border-color: #fff;
  transform: scale(1.15);
  box-shadow: 0 0 0 2px var(--primary);
}

.custom-color-picker-wrapper {
  position: relative;
  width: 24px;
  height: 24px;
  flex-shrink: 0;
}

.custom-color-input {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  opacity: 0;
  cursor: pointer;
  border: none;
  padding: 0;
}

.custom-color-trigger {
  font-size: 0.9rem;
  color: #fff;
  text-shadow: 0 1px 3px rgba(0, 0, 0, 0.5);
}

.emoji-selector-group {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.emoji-container {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
  background: hsla(var(--hue-base) 10% 20% / 0.15);
  border: 1px solid var(--border-glass);
  padding: 0.5rem;
  border-radius: var(--radius-sm);
  max-width: 280px;
}

.emoji-grid {
  display: grid;
  grid-template-columns: repeat(8, 1fr);
  gap: 0.4rem;
  justify-items: center;
}

.emoji-btn {
  width: 28px;
  height: 28px;
  font-size: 1.05rem;
  background: var(--surface-1);
  border: 1px solid var(--border-glass);
  border-radius: var(--radius-sm);
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: all var(--transition-fast);
}

.emoji-btn:hover {
  background: var(--surface-3);
  transform: scale(1.15) translateY(-1px);
}

.emoji-btn.active {
  background: var(--surface-4) !important;
  border-color: var(--primary) !important;
  transform: scale(1.15) translateY(-1px);
}

.emoji-fallback-box {
  display: flex;
  align-items: center;
  background: var(--surface-1);
  border: 1px solid var(--border-glass);
  border-radius: var(--radius-sm);
  padding: 0 0.5rem;
  gap: 0.35rem;
  max-width: 150px;
  align-self: center;
}

.emoji-fallback-icon {
  font-size: 0.8rem;
  opacity: 0.7;
}

.emoji-fallback-input {
  border: none !important;
  background: transparent !important;
  padding: 0.35rem 0 !important;
  font-size: 0.8rem;
  width: 100%;
  color: var(--text-primary);
  outline: none;
  box-shadow: none !important;
}

.cat-color-dot {
  display: inline-block;
  width: 14px;
  height: 14px;
  border-radius: 50%;
  margin-right: 6px;
  vertical-align: middle;
  border: 1px solid rgba(255,255,255,0.2);
}

/* RESPONSIVE DESIGN */
@media (max-width: 900px) {
  .app-container {
    padding: 1.25rem 0.75rem;
  }

  .dashboard-grid, .interaction-grid {
    grid-template-columns: 1fr;
    gap: 1.5rem;
  }

  .stats-sidebar {
    display: grid;
    grid-template-columns: repeat(3, 1fr);
    gap: 1rem;
    align-items: stretch;
  }

  .manage-categories-btn {
    grid-column: span 3;
    margin-top: 0;
  }

  .chart-content {
    grid-template-columns: 1fr;
    justify-items: center;
    gap: 1.5rem;
  }
  
  .chart-legend {
    width: 100%;
    max-height: none;
  }
}

@media (max-width: 720px) {
  .stats-sidebar {
    grid-template-columns: 1fr;
  }

  .manage-categories-btn {
    grid-column: span 1;
  }
}

@media (max-width: 600px) {
  .form-row-cat {
    flex-direction: column;
    align-items: stretch;
    gap: 0.85rem;
  }

  .form-row-cat .input-control,
  .form-row-cat .icon-input {
    width: 100%;
    flex: none;
  }

  .form-row-cat .color-input {
    width: 100%;
    height: 44px;
    flex-shrink: 0;
  }

  .cat-form-actions {
    display: flex;
    gap: 0.5rem;
    width: 100%;
  }

  .cat-form-actions .btn {
    flex: 1;
  }

  .modal-content {
    padding: 1.5rem;
  }
}

@media (max-width: 500px) {
  .purchase-table th, 
  .purchase-table td {
    padding: 0.65rem 0.5rem;
    font-size: 0.82rem;
  }

  .category-tag {
    padding: 0.25rem 0.5rem;
    font-size: 0.75rem;
  }

  .tag-icon {
    font-size: 0.85rem;
  }

  .action-btn {
    font-size: 0.9rem;
    padding: 0.15rem;
    margin: 0 0.15rem;
  }
}

@media (max-width: 400px) {
  .form-row {
    grid-template-columns: 1fr;
    gap: 0.85rem;
  }
}

@media (max-width: 480px) {
  .svg-container {
    max-width: 200px !important;
    margin: 0 auto !important;
  }

  /* O layout já é flex-col por padrão */
}
/* Payment Methods UI */
.payment-methods-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(110px, 1fr));
  gap: 0.5rem;
  margin-top: 0.25rem;
}

.payment-method-btn {
  background: var(--surface-1);
  border: 1px solid var(--border-glass);
  color: var(--text-secondary);
  font-family: inherit;
  font-size: 0.82rem;
  font-weight: 500;
  padding: 0.65rem 0.5rem;
  border-radius: var(--radius-sm);
  cursor: pointer;
  outline: none;
  transition: all var(--transition-fast);
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.35rem;
  text-align: center;
  user-select: none;
}

.payment-method-btn:hover:not(:disabled) {
  color: var(--text-primary);
  background: var(--surface-3);
  border-color: var(--border-glass-focus);
}

.payment-method-btn.active {
  color: #fff !important;
  background: linear-gradient(135deg, var(--primary) 0%, hsl(263, 80%, 55%) 100%) !important;
  border-color: var(--primary) !important;
  box-shadow: 0 2px 10px hsla(263, 80%, 62%, 0.2);
}

.payment-method-btn:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.payment-method-readonly {
  margin-top: 0.5rem;
  padding: 0.75rem 1rem;
  border-radius: var(--radius-sm);
  background: hsla(var(--hue-base) 30% 25% / 0.1);
  border: 1px dashed var(--border-glass);
  font-size: 0.88rem;
  color: var(--text-secondary);
}

.payment-method-readonly strong {
  color: var(--text-primary);
}

/* Group row styles */
.group-row {
  background-color: hsla(var(--hue-base) 30% 25% / 0.08) !important;
  border-left: 3px solid var(--primary) !important;
}

.group-toggle-btn {
  background: transparent;
  border: none;
  color: var(--text-secondary);
  cursor: pointer;
  font-size: 0.9rem;
  margin-right: 0.5rem;
  transition: transform var(--transition-fast);
  display: inline-block;
}

.group-toggle-btn.expanded {
  transform: rotate(90deg);
}

.sub-row {
  background-color: hsla(var(--hue-base) 30% 25% / 0.03) !important;
  opacity: 0.95;
}

.indent-cell {
  padding-left: 2rem !important;
}

/* Card hierarchy in mobile */
.purchase-card-group-header {
  border-left: 4px solid var(--primary) !important;
  background: hsla(var(--hue-base) 30% 25% / 0.08) !important;
}

.purchase-card-group-children {
  margin-left: 1rem;
  border-left: 1px dashed var(--border-glass);
  padding-left: 0.75rem;
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
}

/* ===== PAGINAÇÃO ===== */
.pagination-container {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.4rem;
  padding: 1.25rem 1rem 0.5rem;
  flex-wrap: wrap;
}

.pagination-btn {
  min-width: 36px;
  height: 36px;
  padding: 0 0.5rem;
  border-radius: var(--radius-sm);
  border: 1px solid var(--border-glass);
  background: var(--surface-1);
  color: var(--text-secondary);
  font-size: 0.9rem;
  font-weight: 500;
  cursor: pointer;
  transition: all var(--transition-fast);
  display: inline-flex;
  align-items: center;
  justify-content: center;
}

.pagination-btn:hover:not(:disabled) {
  background: var(--surface-2);
  border-color: var(--border-glass-focus);
  color: var(--text-primary);
  transform: translateY(-1px);
}

.pagination-btn.active {
  background: var(--primary);
  border-color: var(--primary);
  color: #fff;
  font-weight: 700;
  box-shadow: 0 0 12px var(--primary-glow);
}

.pagination-btn:disabled {
  opacity: 0.35;
  cursor: not-allowed;
  transform: none;
}

.pagination-ellipsis {
  color: var(--text-muted);
  font-size: 0.9rem;
  padding: 0 0.25rem;
  user-select: none;
}
</style>
