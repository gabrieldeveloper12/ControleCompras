import { useAuthStore } from '../stores/auth'

const BASE_URL = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5076'
const API_BASE = `${BASE_URL}/api`

async function getHeaders() {
  const authStore = useAuthStore()
  const headers = { 'Content-Type': 'application/json' }
  if (authStore.token) {
    headers['Authorization'] = `Bearer ${authStore.token}`
  }
  return headers
}

export const DespesasFixasService = {
  async list() {
    const headers = await getHeaders()
    const res = await fetch(`${API_BASE}/despesas-fixas`, { headers })
    if (!res.ok) throw new Error('Erro ao listar despesas fixas')
    return await res.json()
  },

  async create(data) {
    const headers = await getHeaders()
    const res = await fetch(`${API_BASE}/despesas-fixas`, {
      method: 'POST',
      headers,
      body: JSON.stringify(data)
    })
    if (!res.ok) {
      const msg = await res.text()
      throw new Error(msg || 'Erro ao criar despesa fixa')
    }
    return await res.json()
  },

  async update(id, data) {
    const headers = await getHeaders()
    const res = await fetch(`${API_BASE}/despesas-fixas/${id}`, {
      method: 'PUT',
      headers,
      body: JSON.stringify(data)
    })
    if (!res.ok) {
      const msg = await res.text()
      throw new Error(msg || 'Erro ao atualizar despesa fixa')
    }
    return true
  },

  async delete(id) {
    const headers = await getHeaders()
    const res = await fetch(`${API_BASE}/despesas-fixas/${id}`, {
      method: 'DELETE',
      headers
    })
    if (!res.ok) throw new Error('Erro ao excluir despesa fixa')
    return true
  },

  async listPagamentos(mes, ano) {
    const headers = await getHeaders()
    const res = await fetch(`${API_BASE}/despesas-fixas/pagamentos?mes=${mes}&ano=${ano}`, { headers })
    if (!res.ok) throw new Error('Erro ao listar pagamentos mensais')
    return await res.json()
  },

  async togglePagamento(data) {
    const headers = await getHeaders()
    const res = await fetch(`${API_BASE}/despesas-fixas/pagamentos/toggle`, {
      method: 'POST',
      headers,
      body: JSON.stringify(data)
    })
    if (!res.ok) {
      const msg = await res.text()
      throw new Error(msg || 'Erro ao alterar status de pagamento')
    }
    return await res.json()
  },

  async listProximosVencimentos() {
    const headers = await getHeaders()
    const res = await fetch(`${API_BASE}/despesas-fixas/proximos-vencimentos`, { headers })
    if (!res.ok) throw new Error('Erro ao buscar próximos vencimentos')
    return await res.json()
  }
}
