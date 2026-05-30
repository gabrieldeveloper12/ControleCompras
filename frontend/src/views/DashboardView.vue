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
              <span class="stat-value">{{ formatCurrency(totalGastos) }}</span>
            </div>
          </div>

          <div class="glass-panel stat-card">
            <span class="stat-icon">📊</span>
            <div class="stat-info">
              <span class="stat-label">Média por Compra</span>
              <span class="stat-value">{{ formatCurrency(mediaPorCompra) }}</span>
            </div>
          </div>

          <div class="glass-panel stat-card">
            <span class="stat-icon">🏷️</span>
            <div class="stat-info">
              <span class="stat-label">Total de Lançamentos</span>
              <span class="stat-value">{{ totalLancamentos }}</span>
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
                  :stroke="slice.cor"
                  :stroke-dasharray="`${slice.size} 314.16`"
                  :stroke-dashoffset="slice.offset"
                  stroke-width="12"
                  fill="transparent"
                />
                
                <!-- Central Text -->
                <g class="donut-center-text">
                  <text x="70" y="68" class="donut-center-val">{{ formatCurrency(totalGastos) }}</text>
                  <text x="70" y="82" class="donut-center-lbl">Total Filtrado</text>
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
                placeholder="Ex: Supermercado do mês"
                required
              />
            </div>

            <div class="form-row">
              <div class="form-group">
                <label class="input-label" for="valor">Valor (R$)</label>
                <input 
                  id="valor"
                  type="text" 
                  inputmode="decimal"
                  v-model="valorExibido" 
                  class="input-control" 
                  placeholder="0,00"
                  required
                  @focus="onValorFocus"
                  @blur="onValorBlur"
                />
              </div>

              <div class="form-group">
                <label class="input-label" for="data">Data</label>
                <input 
                  id="data"
                  type="date" 
                  v-model="formCompra.data" 
                  class="input-control" 
                  required
                />
              </div>
            </div>

            <div class="form-group">
              <label class="input-label" for="categoria">Categoria</label>
              <select 
                id="categoria"
                v-model="formCompra.categoriaId" 
                class="input-control select-control"
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
            </div>

            <div class="form-actions">
              <button 
                v-if="editMode" 
                type="button" 
                class="btn btn-secondary" 
                @click="cancelEdit"
              >
                Cancelar
              </button>
              <button type="submit" class="btn btn-primary submit-btn">
                {{ editMode ? 'Salvar Alterações' : 'Salvar Compra' }}
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
                @change="onFiltroMesAnoChange"
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
                  @change="onFiltroDataChange"
                />
              </div>

              <div class="form-group">
                <label class="input-label">Data Final</label>
                <input 
                  type="date" 
                  v-model="filtroDataFim" 
                  class="input-control"
                  @change="onFiltroDataChange"
                />
              </div>
            </div>

            <button class="btn btn-secondary clear-filters-btn" @click="resetFilters">
              🧹 Limpar Filtros
            </button>
          </div>
        </div>
      </section>

      <!-- PURCHASES LIST TABLE -->
      <section class="glass-panel list-section animate-fade-in" style="animation-delay: 0.2s">
        <div class="list-header">
          <h3 class="card-title">Registros de Compras</h3>
          <span class="count-badge">{{ filteredCountText }}</span>
        </div>

        <div class="table-container">
          <div v-if="filteredCompras.length === 0" class="empty-table-state">
            <span>📝</span>
            <p>Nenhuma compra registrada neste período.</p>
          </div>
          
          <table v-else class="purchase-table">
            <thead>
              <tr>
                <th>Descrição</th>
                <th>Categoria</th>
                <th>Data</th>
                <th class="text-right">Valor</th>
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
        data: this.getTodayDateString(),
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
      editCatMode: false
    }
  },
  computed: {
    filteredCompras() {
      if (!this.editMode || !this.formCompra.id) return this.compras;
      return this.compras.filter(c => c.id !== this.formCompra.id);
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

    // Utility Dates Helper
    getTodayDateString() {
      const today = new Date();
      return today.toISOString().split('T')[0];
    },
    formatDate(dateStr) {
      if (!dateStr) return '';
      const date = new Date(dateStr);
      // Ajuste de Timezone local
      return date.toLocaleDateString('pt-BR', { timeZone: 'UTC' });
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
          if (this.filtroDataInicio) params.push(`inicio=${this.filtroDataInicio}`);
          if (this.filtroDataFim) params.push(`fim=${this.filtroDataFim}`);
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
      this.fetchCompras();
    },
    resetFilters() {
      this.filtroTipo = 'mes';
      this.filtroMesAno = new Date().toISOString().substring(0, 7);
      this.filtroDataInicio = this.getTodayDateString();
      this.filtroDataFim = this.getTodayDateString();
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
            data: new Date(this.formCompra.data).toISOString(),
            categoriaId: this.formCompra.categoriaId
          })
        });

        if (res.ok) {
          this.cancelEdit();
          await this.fetchCompras();
        } else {
          const errMsg = await res.text();
          alert(`Erro: ${errMsg}`);
        }
      } catch (err) {
        console.error('Erro ao salvar compra:', err);
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
      this.formCompra.data = this.getTodayDateString();
      this.formCompra.categoriaId = '';
    },

    async deleteCompra(id) {
      if (!confirm('Deseja realmente excluir esta compra?')) return;
      
      try {
        const res = await this.fetchWithAuth(`${API_BASE}/compras/${id}`, {
          method: 'DELETE'
        });

        if (res.ok) {
          await this.fetchCompras();
        } else {
          alert('Erro ao excluir a compra.');
        }
      } catch (err) {
        console.error('Erro ao excluir compra:', err);
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
          this.cancelCatEdit();
          await this.fetchCategorias();
          await this.fetchCompras(); // Atualiza cores/ícones na lista e dashboard
        } else {
          const errMsg = await res.text();
          alert(`Erro: ${errMsg}`);
        }
      } catch (err) {
        console.error('Erro ao salvar categoria:', err);
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
      if (!confirm('Deseja realmente excluir esta categoria?')) return;

      try {
        const res = await this.fetchWithAuth(`${API_BASE}/categorias/${id}`, {
          method: 'DELETE'
        });

        if (res.ok) {
          await this.fetchCategorias();
        } else {
          const errMsg = await res.text();
          alert(`Erro: ${errMsg}`);
        }
      } catch (err) {
        console.error('Erro ao excluir categoria:', err);
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
  transition: stroke-dashoffset var(--transition-slow);
}

.donut-center-text {
  transform: rotate(90deg);
  transform-origin: center;
}

.donut-center-val {
  fill: var(--text-primary);
  font-size: 10px;
  font-weight: 700;
  text-anchor: middle;
}

.donut-center-lbl {
  fill: var(--text-secondary);
  font-size: 6px;
  text-transform: uppercase;
  letter-spacing: 0.05em;
  text-anchor: middle;
}

.chart-legend {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(200px, 1fr));
  gap: 1rem;
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
}

.legend-value {
  color: var(--text-secondary);
  font-weight: 600;
  margin-right: 0.5rem;
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


.clear-filters-btn {
  width: 100%;
  padding: 0.75rem;
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
