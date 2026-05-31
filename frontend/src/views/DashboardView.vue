<template>
  <div class="app-container">
    <!-- Header Component -->
    <Header :is-online="isOnline" />

    <main class="main-content">
      <!-- DASHBOARD STATS & CHARTS SECTION -->
      <section class="dashboard-grid animate-fade-in">
        <!-- Stats Cards -->
        <div class="stats-sidebar">
          <div class="glass-panel stat-card">
            <span class="stat-icon">💰</span>
            <div class="stat-info">
              <span class="stat-label">Total de Gastos</span>
              <span v-if="isLoading" class="skeleton-block skeleton-text-lg"></span>
              <span v-else class="stat-value">{{ formatCurrency(totalGastos) }}</span>
            </div>
          </div>

          <div class="glass-panel stat-card">
            <span class="stat-icon">📊</span>
            <div class="stat-info">
              <span class="stat-label">Média por Compra</span>
              <span v-if="isLoading" class="skeleton-block skeleton-text-lg"></span>
              <span v-else class="stat-value">{{ formatCurrency(mediaPorCompra) }}</span>
            </div>
          </div>

          <div class="glass-panel stat-card">
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
                  @input="onValorInput"
                  class="input-control" 
                  :class="{ 'valid': isValorValid, 'invalid': isValorInvalid }"
                  :disabled="isSubmitting"
                  placeholder="0,00"
                  required
                  @focus="onValorFocus"
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
                :disabled="isSubmitting || !isDescricaoValid || !isValorValid || !isCategoriaValid || !isDataValid"
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
                <tr v-for="compra in filteredCompras" :key="compra.id" class="table-row">
                  <td class="col-desc">
                    <strong>{{ compra.descricao }}</strong>
                  </td>
                  <td class="col-cat">
                    <span 
                      class="category-tag" 
                      :style="{ 
                        borderLeft: `4px solid ${compra.categoria?.corHex || '#999'}`,
                        backgroundColor: `${compra.categoria?.corHex || '#999'}15`
                      }"
                    >
                      <span class="tag-icon">{{ compra.categoria?.icone }}</span>
                      <span class="tag-name">{{ compra.categoria?.nome }}</span>
                    </span>
                  </td>
                  <td class="col-date">{{ formatDate(compra.data) }}</td>
                  <td class="col-val text-right">
                    <strong>{{ formatCurrency(compra.valor) }}</strong>
                  </td>
                  <td class="col-actions text-center">
                    <button class="action-btn edit" @click="startEdit(compra)" title="Editar">✏️</button>
                    <button class="action-btn delete" @click="deleteCompra(compra.id)" title="Excluir">❌</button>
                  </td>
                </tr>
              </tbody>
            </table>

            <!-- Mobile Cards View -->
            <div class="purchase-cards-mobile">
              <div 
                v-for="compra in filteredCompras" 
                :key="compra.id" 
                class="purchase-card glass-panel"
              >
                <div class="card-header-row">
                  <strong class="card-description">{{ compra.descricao }}</strong>
                  <span 
                    class="category-tag" 
                    :style="{ 
                      borderLeft: `4px solid ${compra.categoria?.corHex || '#999'}`,
                      backgroundColor: `${compra.categoria?.corHex || '#999'}15`
                    }"
                  >
                    <span class="tag-icon">{{ compra.categoria?.icone }}</span>
                    <span class="tag-name">{{ compra.categoria?.nome }}</span>
                  </span>
                </div>
                
                <div class="card-body-row">
                  <div class="card-info-item">
                    <span class="card-label">Data</span>
                    <span class="card-value">{{ formatDate(compra.data) }}</span>
                  </div>
                  <div class="card-info-item text-right">
                    <span class="card-label">Valor</span>
                    <strong class="card-value highlight">{{ formatCurrency(compra.valor) }}</strong>
                  </div>
                </div>
                
                <div class="card-actions-row">
                  <button class="btn btn-secondary action-btn-tátil" @click="startEdit(compra)">
                    <span>✏️ Editar</span>
                  </button>
                  <button class="btn btn-danger action-btn-tátil" @click="deleteCompra(compra.id)">
                    <span>❌ Excluir</span>
                  </button>
                </div>
              </div>
            </div>
          </template>
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
              <div class="form-row-cat">
                <input 
                  type="text" 
                  v-model="formCat.nome" 
                  class="input-control" 
                  placeholder="Nome (ex: Lazer)"
                  required
                />
                
                <input 
                  type="text" 
                  v-model="formCat.icone" 
                  class="input-control icon-input" 
                  placeholder="Ícone (ex: 🎮)"
                  required
                />
                
                <input 
                  type="color" 
                  v-model="formCat.corHex" 
                  class="input-control color-input" 
                  title="Cor da Categoria"
                />

                <div class="cat-form-actions">
                  <button v-if="editCatMode" type="button" class="btn btn-secondary btn-sm" @click="cancelCatEdit">
                    Cancelar
                  </button>
                  <button type="submit" class="btn btn-primary btn-sm">
                    {{ editCatMode ? 'Salvar' : 'Adicionar' }}
                  </button>
                </div>
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
        categoriaId: ''
      },
      valorExibido: '',
      editMode: false,

      // Modal Categorias
      categoryModalOpen: false,
      formCat: {
        id: null,
        nome: '',
        icone: '📦',
        corHex: '#9E9E9E'
      },
      editCatMode: false,
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
      isLoading: true
    }
  },
  watch: {
    // 1.2: Debounce de 300ms para searchQuery
    searchQuery(newVal) {
      clearTimeout(this.searchDebounceTimer);
      this.searchDebounceTimer = setTimeout(() => {
        this.debouncedSearchQuery = newVal;
      }, 300);
    }
  },
  computed: {
    isDescricaoValid() {
      return this.formCompra.descricao.trim().length >= 3;
    },
    isDescricaoInvalid() {
      return this.formCompra.descricao.trim().length > 0 && this.formCompra.descricao.trim().length < 3;
    },
    isValorValid() {
      if (!this.valorExibido) return false;
      const valorCru = this.valorExibido.replace(',', '.');
      const val = parseFloat(valorCru);
      return !isNaN(val) && val > 0;
    },
    isValorInvalid() {
      if (!this.valorExibido) return false;
      const valorCru = this.valorExibido.replace(',', '.');
      const val = parseFloat(valorCru);
      return isNaN(val) || val <= 0;
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
      const cnt = this.filteredCompras.length;
      if (cnt === 0) return 'Nenhum lançamento';
      if (cnt === 1) return '1 lançamento';
      return `${cnt} lançamentos`;
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

    // Currency Input Formatter Helpers
    formatDecimalBrl(val) {
      if (val === null || val === undefined) return '';
      return val.toLocaleString('pt-BR', {
        minimumFractionDigits: 2,
        maximumFractionDigits: 2
      });
    },
    onValorFocus() {
      if (this.valorExibido) {
        // Clean display value to make it edit-friendly (remove R$ or thousands dots)
        let cleaned = this.valorExibido
          .replace(/[R$\s]/g, '') // Remove currency symbol and spaces
          .replace(/\./g, ''); // Remove thousands separator dots
        this.valorExibido = cleaned;
      }
    },
    onValorBlur() {
      if (this.valorExibido) {
        // Standardise separator and format with 2 decimals
        const rawVal = this.valorExibido.replace(',', '.');
        const parsed = parseFloat(rawVal);
        if (!isNaN(parsed) && parsed > 0) {
          this.valorExibido = this.formatDecimalBrl(parsed);
        }
      }
    },
    onValorInput() {
      let val = this.valorExibido || '';
      let cleaned = val.replace(/[^0-9.,]/g, '');
      
      let firstSeparatorIndex = cleaned.search(/[.,]/);
      if (firstSeparatorIndex !== -1) {
        let separator = cleaned[firstSeparatorIndex];
        let before = cleaned.substring(0, firstSeparatorIndex);
        let after = cleaned.substring(firstSeparatorIndex + 1).replace(/[.,]/g, '');
        cleaned = before + separator + after;
      }
      
      this.valorExibido = cleaned;
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
        const valorCru = this.valorExibido.replace(',', '.');
        const valorParseado = parseFloat(valorCru);
        
        if (isNaN(valorParseado) || valorParseado <= 0) {
          alert('Por favor, insira um valor numérico válido maior que zero.');
          return;
        }

        this.isSubmitting = true;

        const url = this.editMode 
          ? `${API_BASE}/compras/${this.formCompra.id}`
          : `${API_BASE}/compras`;
        
        const method = this.editMode ? 'PUT' : 'POST';
        
        const res = await this.fetchWithAuth(url, {
          method: method,
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify({
            descricao: this.formCompra.descricao,
            valor: valorParseado,
            data: this.formCompra.data + "T12:00:00",
            categoriaId: this.formCompra.categoriaId
          })
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
      this.valorExibido = this.formatDecimalBrl(compra.valor);
      this.formCompra.data = compra.data ? compra.data.split('T')[0] : this.getTodayDateString();
      this.formCompra.categoriaId = compra.categoriaId;

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
      this.valorExibido = '';
      this.formCompra.data = new Date().toISOString().split('T')[0];
      this.formCompra.categoriaId = '';
    },

    async deleteCompra(id) {
      const confirmed = await this.showConfirm('Deseja realmente excluir esta compra?');
      if (!confirmed) return;
      
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
      try {
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
  grid-template-columns: 180px 1fr;
  gap: 2rem;
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
  gap: 0.75rem;
  max-height: 200px;
  overflow-y: auto;
  padding-right: 0.5rem;
}

.legend-item {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  font-size: 0.9rem;
  padding: 0.5rem;
  border-radius: var(--radius-sm);
  background: var(--surface-1);
  border: 1px solid var(--border-glass);
}

.legend-dot {
  width: 10px;
  height: 10px;
  border-radius: 50%;
  flex-shrink: 0;
}

.legend-emoji {
  font-size: 1.1rem;
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
  font-size: 0.75rem;
  background: var(--surface-3);
  padding: 0.1rem 0.3rem;
  border-radius: 4px;
  color: var(--text-secondary);
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

.select-control {
  cursor: pointer;
  appearance: none;
  background-image: url("data:image/svg+xml;charset=utf-8,%3Csvg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 24 24' fill='none' stroke='rgba(255,255,255,0.6)' stroke-width='2' stroke-linecap='round' stroke-linejoin='round'%3E%3Cpath d='m6 9 6 6 6-6'/%3E%3C/svg%3E");
  background-repeat: no-repeat;
  background-position: right 1rem center;
  background-size: 1.2rem;
  padding-right: 2.5rem;
}

[data-theme="light"] .select-control {
  background-image: url("data:image/svg+xml;charset=utf-8,%3Csvg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 24 24' fill='none' stroke='rgba(0,0,0,0.45)' stroke-width='2' stroke-linecap='round' stroke-linejoin='round'%3E%3Cpath d='m6 9 6 6 6-6'/%3E%3C/svg%3E");
}

[data-theme="light"] .btn-secondary {
  background: var(--surface-3);
  border-color: var(--border-glass);
}

[data-theme="light"] .btn-secondary:hover {
  background: var(--surface-4);
  border-color: hsla(var(--hue-base) 10% 50% / 0.15);
}

.select-control option {
  background: var(--bg-input);
  color: var(--text-primary);
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

/* RESPONSIVE DESIGN */
@media (max-width: 900px) {
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

  .chart-legend {
    grid-template-columns: 1fr !important;
  }
}
</style>
