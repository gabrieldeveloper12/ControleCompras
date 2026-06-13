<template>
  <div class="page-content">
    <main class="main-content animate-fade-in">
      
      <!-- Topo: Formulário de Cadastro/Edição de Despesa Fixa -->
      <section class="glass-panel form-card" style="margin-bottom: 2rem;">
        <h3 class="card-title">{{ editMode ? 'Editar Despesa Fixa' : 'Cadastrar Despesa Fixa' }}</h3>
        
        <form @submit.prevent="saveDespesa" class="purchase-form">
          <div class="despesas-form-grid">
            <!-- Descrição -->
            <div class="form-group descricao-col">
              <label class="input-label" for="descricao">Descrição</label>
              <input 
                id="descricao"
                type="text" 
                v-model="formDespesa.descricao" 
                class="input-control" 
                :class="{ 'valid': isDescricaoValid, 'invalid': isDescricaoInvalid }"
                :disabled="isSubmitting"
                placeholder="Ex: Aluguel do Apartamento"
                required
              />
              <span class="input-hint" :class="{ 'error': isDescricaoInvalid }">
                {{ isDescricaoInvalid ? 'Mínimo de 3 caracteres.' : 'Mínimo de 3 caracteres (Ex: Internet).' }}
              </span>
            </div>

            <!-- Categoria -->
            <div class="form-group categoria-col">
              <label class="input-label" for="categoria">Categoria</label>
              <select 
                id="categoria"
                v-model="formDespesa.categoriaId" 
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
              <span class="input-hint">Grupo de gastos da despesa.</span>
            </div>

            <!-- Valor -->
            <div class="form-group valor-col">
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
                {{ isValorInvalid ? 'Valor numérico inválido.' : 'Valor da despesa.' }}
              </span>
            </div>

            <!-- Dia Vencimento -->
            <div class="form-group dia-col">
              <label class="input-label" for="diaVencimento">Dia Vencimento</label>
              <input 
                id="diaVencimento"
                type="number" 
                min="1"
                max="31"
                v-model.number="formDespesa.diaVencimento" 
                class="input-control" 
                :class="{ 'valid': isDiaVencimentoValid }"
                :disabled="isSubmitting"
                placeholder="10"
                required
              />
              <span class="input-hint">Dia do mês (1 a 31).</span>
            </div>
          </div>

          <div class="form-actions" style="margin-top: 1.5rem; justify-content: flex-end;">
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
              :disabled="isSubmitting || !isDescricaoValid || !isValorValid || !isCategoriaValid || !isDiaVencimentoValid"
            >
              <span v-if="isSubmitting" class="spinner-icon">🔄</span>
              <span>{{ isSubmitting ? 'Salvando...' : (editMode ? 'Salvar Alterações' : 'Salvar Despesa') }}</span>
            </button>
          </div>
        </form>
      </section>

      <!-- Grid Horizontal de Listas -->
      <section class="despesas-lists-grid">
        <!-- Coluna Esquerda: Listagem de Templates de Despesas Fixas -->
        <div class="glass-panel list-section" style="padding: 1.5rem;">
          <h3 class="card-title">Despesas Cadastradas</h3>
          
          <div v-if="loadingTemplates" class="skeleton-list">
            <div class="skeleton-block skeleton-row" v-for="i in 3" :key="i"></div>
          </div>
          <div v-else-if="templates.length === 0" class="empty-widget-state">
            <span>📌</span>
            <p>Nenhuma despesa fixa cadastrada.</p>
          </div>
          
          <div v-else class="upcoming-list">
            <div v-for="item in templates" :key="item.id" class="upcoming-item" style="padding: 0.85rem 1rem;">
              <div class="upcoming-info">
                <span class="upcoming-emoji">{{ item.categoria?.icone || '📌' }}</span>
                <div class="upcoming-details">
                  <strong>{{ item.descricao }}</strong>
                  <span>Vence todo dia {{ item.diaVencimento }}</span>
                </div>
              </div>
              <div style="display: flex; align-items: center; gap: 1rem;">
                <strong class="upcoming-value">{{ formatCurrency(item.valor) }}</strong>
                <div style="display: flex; gap: 0.25rem;">
                  <button class="action-btn edit" @click="startEdit(item)" title="Editar">✏️</button>
                  <button class="action-btn delete" @click="deleteDespesa(item)" title="Excluir">❌</button>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Coluna Direita: Controle e checklist de Pagamento do Mês -->
        <div class="glass-panel checklist-section" style="padding: 1.5rem;">
          <div class="card-header-row-dashboard" style="align-items: flex-start; flex-direction: column; gap: 0.5rem; border-bottom: none; margin-bottom: 1rem;">
            <h3 class="card-title" style="border: none; margin: 0; padding: 0;">Controle de Pagamentos</h3>
            <p style="font-size: 0.82rem; color: var(--text-secondary);">Marque e controle as contas pagas do mês.</p>
          </div>

          <!-- Seletor de Mês/Ano -->
          <div class="form-group" style="margin-bottom: 1.5rem;">
            <label class="input-label">Mês de Referência</label>
            <input 
              type="month" 
              v-model="filtroMesAno" 
              @change="fetchPagamentos"
              class="input-control"
              style="max-width: 200px;"
            />
          </div>

          <!-- Checklist List -->
          <div v-if="loadingPagamentos" class="skeleton-list">
            <div class="skeleton-block skeleton-row" v-for="i in 5" :key="i"></div>
          </div>
          <div v-else-if="pagamentos.length === 0" class="empty-widget-state" style="padding: 4rem 2rem;">
            <span>📅</span>
            <p>Nenhuma despesa fixa ativa para este mês.</p>
            <p style="font-size: 0.8rem; color: var(--text-muted);">Adicione despesas fixas na coluna ao lado para exibi-las aqui.</p>
          </div>
          
          <div v-else class="upcoming-list">
            <div 
              v-for="item in pagamentos" 
              :key="item.despesaFixaId" 
              class="upcoming-item"
              :style="item.pago ? { borderLeft: '4px solid var(--success)', background: 'hsla(142, 72%, 42%, 0.05)' } : { borderLeft: '4px solid var(--warning)' }"
            >
              <div class="upcoming-info">
                <span class="upcoming-emoji" :style="item.pago ? { borderColor: 'var(--success)' } : {}">
                  {{ item.categoria?.icone || '📌' }}
                </span>
                <div class="upcoming-details">
                  <strong :style="item.pago ? { textDecoration: 'line-through', opacity: 0.65 } : {}">
                    {{ item.descricao }}
                  </strong>
                  <span>Vencimento: Dia {{ item.diaVencimento }}</span>
                </div>
              </div>
              
              <div style="display: flex; align-items: center; gap: 1rem;">
                <strong class="upcoming-value" :style="item.pago ? { opacity: 0.65 } : {}">
                  {{ formatCurrency(item.valorOriginal) }}
                </strong>
                
                <!-- Toggle Button -->
                <button 
                  class="btn btn-sm action-btn-tátil"
                  :class="item.pago ? 'btn-secondary' : 'btn-primary'"
                  :disabled="togglingId === item.despesaFixaId"
                  @click="togglePagamento(item)"
                  style="min-height: 36px !important; font-size: 0.82rem; padding: 0.4rem 0.85rem;"
                >
                  <span v-if="togglingId === item.despesaFixaId" class="spinner-icon">🔄</span>
                  <span v-else-if="item.pago">✓ Pago</span>
                  <span v-else>Pagar</span>
                </button>
              </div>
            </div>
          </div>
        </div>
      </section>

    </main>

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
import { DespesasFixasService } from '../services/despesasFixasService';

const BASE_URL = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5076';
const API_BASE = `${BASE_URL}/api`;

export default {
  name: 'DespesasFixasView',
  data() {
    return {
      categorias: [],
      templates: [],
      pagamentos: [],
      loadingTemplates: true,
      loadingPagamentos: true,
      togglingId: null,
      isSubmitting: false,
      editMode: false,

      // Form despesa
      formDespesa: {
        id: null,
        descricao: '',
        valor: null,
        diaVencimento: null,
        categoriaId: ''
      },
      valorExibido: 'R$ 0,00',
      filtroMesAno: new Date(Date.now() - new Date().getTimezoneOffset() * 60000).toISOString().substring(0, 7),

      // Confirm modal
      confirmModal: {
        show: false,
        message: '',
        resolve: null
      }
    }
  },
  watch: {
    'confirmModal.show'(isOpen) {
      if (isOpen) {
        document.body.classList.add('modal-open');
      } else {
        document.body.classList.remove('modal-open');
      }
    }
  },
  computed: {
    isDescricaoValid() {
      return this.formDespesa.descricao.trim().length >= 3;
    },
    isDescricaoInvalid() {
      return this.formDespesa.descricao.trim().length > 0 && this.formDespesa.descricao.trim().length < 3;
    },
    isValorValid() {
      const val = this.formDespesa.valor;
      return typeof val === 'number' && !isNaN(val) && val > 0;
    },
    isValorInvalid() {
      if (this.valorExibido === 'R$ 0,00') return false;
      const val = this.formDespesa.valor;
      return val !== null && val !== undefined && (isNaN(val) || val <= 0);
    },
    isCategoriaValid() {
      return this.formDespesa.categoriaId !== '';
    },
    isDiaVencimentoValid() {
      const dia = this.formDespesa.diaVencimento;
      return typeof dia === 'number' && dia >= 1 && dia <= 31;
    }
  },
  methods: {
    formatCurrency(val) {
      return new Intl.NumberFormat('pt-BR', {
        style: 'currency',
        currency: 'BRL'
      }).format(val);
    },
    onValorFocus(event) {
      if (this.valorExibido === 'R$ 0,00' || this.formDespesa.valor === 0) {
        this.valorExibido = 'R$ ';
        this.formDespesa.valor = null;
      }
      this.$nextTick(() => {
        if (event && event.target) {
          const len = this.valorExibido.length;
          event.target.setSelectionRange(len, len);
        }
      });
    },
    onValorBlur() {
      if (this.valorExibido.trim() === 'R$' || this.formDespesa.valor === null || this.formDespesa.valor === 0) {
        this.valorExibido = 'R$ 0,00';
        this.formDespesa.valor = null;
      } else {
        this.valorExibido = this.formatCurrency(this.formDespesa.valor);
      }
    },
    onValorInput(event) {
      let valStr = event.target.value.replace(/\D/g, '');
      if (!valStr) {
        this.valorExibido = 'R$ ';
        this.formDespesa.valor = null;
        return;
      }
      let centavos = parseInt(valStr, 10);
      let floatVal = centavos / 100;
      this.formDespesa.valor = floatVal;
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
    async showConfirm(message) {
      return new Promise((resolve) => {
        this.confirmModal.message = message;
        this.confirmModal.resolve = resolve;
        this.confirmModal.show = true;
      });
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
    async fetchTemplates() {
      try {
        this.loadingTemplates = true;
        this.templates = await DespesasFixasService.list();
      } catch (err) {
        console.error('Erro ao buscar templates:', err);
        window.showToast('Erro ao carregar despesas fixas.', 'error');
      } finally {
        this.loadingTemplates = false;
      }
    },
    async fetchPagamentos() {
      try {
        this.loadingPagamentos = true;
        if (!this.filtroMesAno) return;
        const [ano, mes] = this.filtroMesAno.split('-');
        this.pagamentos = await DespesasFixasService.listPagamentos(parseInt(mes, 10), parseInt(ano, 10));
      } catch (err) {
        console.error('Erro ao buscar pagamentos:', err);
        window.showToast('Erro ao carregar checklist de pagamentos.', 'error');
      } finally {
        this.loadingPagamentos = false;
      }
    },
    async saveDespesa() {
      if (!this.isDescricaoValid || !this.isValorValid || !this.isCategoriaValid || !this.isDiaVencimentoValid) {
        window.showToast('Preencha os dados corretamente.', 'warning');
        return;
      }

      try {
        this.isSubmitting = true;
        const payload = {
          descricao: this.formDespesa.descricao.trim(),
          valor: this.formDespesa.valor,
          diaVencimento: this.formDespesa.diaVencimento,
          categoriaId: this.formDespesa.categoriaId
        };

        if (this.editMode) {
          await DespesasFixasService.update(this.formDespesa.id, payload);
          window.showToast('Despesa fixa atualizada!', 'success');
        } else {
          await DespesasFixasService.create(payload);
          window.showToast('Despesa fixa criada com sucesso!', 'success');
        }
        
        this.cancelEdit();
        await this.fetchTemplates();
        await this.fetchPagamentos();
      } catch (err) {
        console.error('Erro ao salvar despesa:', err);
        window.showToast(err.message || 'Erro ao salvar despesa.', 'error');
      } finally {
        this.isSubmitting = false;
      }
    },
    startEdit(item) {
      this.editMode = true;
      this.formDespesa.id = item.id;
      this.formDespesa.descricao = item.descricao;
      this.formDespesa.valor = item.valor;
      this.formDespesa.diaVencimento = item.diaVencimento;
      this.formDespesa.categoriaId = item.categoriaId;
      this.valorExibido = this.formatCurrency(item.valor);
      
      this.$nextTick(() => {
        const inputDesc = document.getElementById('descricao');
        if (inputDesc) {
          inputDesc.focus();
        }
      });
    },
    cancelEdit() {
      this.editMode = false;
      this.formDespesa.id = null;
      this.formDespesa.descricao = '';
      this.formDespesa.valor = null;
      this.formDespesa.diaVencimento = null;
      this.formDespesa.categoriaId = '';
      this.valorExibido = 'R$ 0,00';
    },
    async deleteDespesa(item) {
      const confirmed = await this.showConfirm(`Tem certeza que deseja excluir a despesa fixa "${item.descricao}"? Ela não será cobrada nos meses futuros.`);
      if (!confirmed) return;

      try {
        await DespesasFixasService.delete(item.id);
        window.showToast('Despesa fixa excluída!', 'success');
        await this.fetchTemplates();
        await this.fetchPagamentos();
      } catch (err) {
        console.error('Erro ao excluir despesa fixa:', err);
        window.showToast('Erro ao excluir despesa.', 'error');
      }
    },
    async togglePagamento(item) {
      try {
        this.togglingId = item.despesaFixaId;
        if (!this.filtroMesAno) return;
        const [ano, mes] = this.filtroMesAno.split('-');
        
        const payload = {
          despesaFixaId: item.despesaFixaId,
          mes: parseInt(mes, 10),
          ano: parseInt(ano, 10),
          dataPagamento: item.pago ? null : new Date().toISOString(), // paga hoje
          valorPago: item.pago ? null : item.valorOriginal
        };

        const response = await DespesasFixasService.togglePagamento(payload);
        
        if (response.pago) {
          window.showToast(`Conta "${item.descricao}" marcada como paga!`, 'success');
        } else {
          window.showToast(`Pagamento da conta "${item.descricao}" estornado com sucesso!`, 'info');
        }
        
        await this.fetchPagamentos();
      } catch (err) {
        console.error('Erro ao alternar status do pagamento:', err);
        window.showToast(err.message || 'Erro ao alterar status de pagamento.', 'error');
      } finally {
        this.togglingId = null;
      }
    }
  },
  async mounted() {
    await this.fetchCategorias();
    await this.fetchTemplates();
    await this.fetchPagamentos();
  }
}
</script>

<style scoped>
/* Os estilos globais e utilitários são gerenciados centralmente pelo src/style.css */

/* Layout otimizado da View de Despesas Fixas */
.despesas-form-grid {
  display: grid;
  grid-template-columns: 2fr 2fr 1fr 1fr;
  gap: 1.5rem;
  align-items: start;
}

.despesas-lists-grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 2rem;
  align-items: start;
}

/* Responsividade (Caso a tela não seja ultra larga, pode empilhar o formulário melhor) */
@media (max-width: 1024px) {
  .despesas-form-grid {
    grid-template-columns: 1fr 1fr;
  }
}

@media (max-width: 768px) {
  .despesas-form-grid {
    grid-template-columns: 1fr;
  }
  .despesas-lists-grid {
    grid-template-columns: 1fr;
  }
}
</style>
