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

      <!-- BOTTOM ROW (WIDGETS: UPCOMING BILLS & RECENT PURCHASES) -->
      <section class="bottom-grid animate-fade-in" style="animation-delay: 0.1s">
        <!-- Widget: Próximos Vencimentos -->
        <div class="glass-panel upcoming-bills-card" style="padding: 1.5rem;">
          <div class="card-header-row-dashboard">
            <h3 class="card-title" style="border: none; margin: 0; padding: 0;">Próximos Vencimentos</h3>
            <router-link to="/despesas-fixas" class="btn-link">Ver Tudo ➔</router-link>
          </div>
          
          <div v-if="loadingVencimentos" class="skeleton-list">
            <div class="skeleton-block skeleton-row" v-for="i in 3" :key="i"></div>
          </div>
          <div v-else-if="vencimentos.length === 0" class="empty-widget-state">
            <span>🎉</span>
            <p>Nenhuma conta pendente para este mês!</p>
          </div>
          <div v-else class="upcoming-list">
            <div v-for="item in vencimentos.slice(0, 4)" :key="item.id" class="upcoming-item">
              <div class="upcoming-info">
                <span class="upcoming-emoji">{{ item.categoria?.icone || '📌' }}</span>
                <div class="upcoming-details">
                  <strong>{{ item.descricao }}</strong>
                  <span>Vence dia {{ item.diaVencimento }}</span>
                </div>
              </div>
              <strong class="upcoming-value text-danger">{{ formatCurrency(item.valor) }}</strong>
            </div>
          </div>
        </div>

        <!-- Widget: Últimos Lançamentos -->
        <div class="glass-panel recent-purchases-card" style="padding: 1.5rem;">
          <div class="card-header-row-dashboard">
            <h3 class="card-title" style="border: none; margin: 0; padding: 0;">Últimos Lançamentos</h3>
            <router-link to="/compras" class="btn-link">Ver Todos ➔</router-link>
          </div>
          
          <div v-if="isLoading" class="skeleton-list">
            <div class="skeleton-block skeleton-row" v-for="i in 3" :key="i"></div>
          </div>
          <div v-else-if="compras.length === 0" class="empty-widget-state">
            <span>🛒</span>
            <p>Nenhum lançamento registrado ainda.</p>
          </div>
          <div v-else class="recent-list">
            <div v-for="item in compras.slice(0, 4)" :key="item.id" class="recent-item">
              <div class="recent-info">
                <span class="recent-emoji">{{ item.categoria?.icone || '📦' }}</span>
                <div class="recent-details">
                  <strong>{{ item.descricao }}</strong>
                  <span>{{ formatDate(item.data) }}</span>
                </div>
              </div>
              <strong class="recent-value">{{ formatCurrency(item.valor) }}</strong>
            </div>
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
import Header from '../components/Header.vue';
import { useAuthStore } from '../stores/auth';
import { DespesasFixasService } from '../services/despesasFixasService';

const BASE_URL = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5076';
const API_BASE = `${BASE_URL}/api`;

export default {
  name: 'DashboardView',
  components: {
    Header
  },
  data() {
    return {
      isOnline: false,
      categorias: [],
      compras: [],
      vencimentos: [],
      loadingVencimentos: true,
      isLoading: true,
      hoveredSlice: null,
      animateChart: false,

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

      // Modal confirmação
      confirmModal: {
        show: false,
        message: '',
        resolve: null
      }
    }
  },
  watch: {
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
    chartLegendItems() {
      if (this.compras.length === 0) return [];
      
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
      const CIRCUMFERENCE = 314.16;

      legend.forEach(item => {
        const pct = item.pct;
        const size = (pct * CIRCUMFERENCE) / 100;
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
    async checkApiStatus() {
      try {
        const res = await fetch(BASE_URL);
        this.isOnline = res.ok;
      } catch (err) {
        this.isOnline = false;
      }
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
        
        // No dashboard, buscamos as compras do mês atual por padrão
        const agora = new Date();
        const mes = agora.getMonth() + 1;
        const ano = agora.getFullYear();
        
        const res = await this.fetchWithAuth(`${API_BASE}/compras?mes=${mes}&ano=${ano}`);
        if (res.ok) {
          this.compras = await res.json();
        }
      } catch (err) {
        console.error('Erro ao buscar compras:', err);
      } finally {
        this.isLoading = false;
      }
    },
    async fetchProximosVencimentos() {
      try {
        this.loadingVencimentos = true;
        this.vencimentos = await DespesasFixasService.listProximosVencimentos();
      } catch (err) {
        console.error('Erro ao buscar próximos vencimentos:', err);
      } finally {
        this.loadingVencimentos = false;
      }
    },
    async showConfirm(message) {
      return new Promise((resolve) => {
        this.confirmModal.message = message;
        this.confirmModal.resolve = resolve;
        this.confirmModal.show = true;
      });
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
    await this.checkApiStatus();
    if (this.isOnline) {
      await this.fetchCategorias();
      await this.fetchCompras();
      await this.fetchProximosVencimentos();
    }
    this.isLoading = false;
    
    setTimeout(() => {
      this.animateChart = true;
    }, 150);

    setInterval(async () => {
      await this.checkApiStatus();
    }, 10000);
  },
  beforeUnmount() {
    document.body.classList.remove('modal-open');
  }
}
</script>

<style>
/* Os estilos globais e utilitários são gerenciados centralmente pelo src/style.css */
</style>
