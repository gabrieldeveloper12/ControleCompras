const API_BASE = `${import.meta.env.VITE_API_BASE_URL || 'http://localhost:5076'}/api/auth`;

export const AuthService = {
  async login(email, password) {
    const res = await fetch(`${API_BASE}/login`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ email, password })
    });

    if (!res.ok) {
      const errorMsg = await res.text();
      throw new Error(errorMsg || 'Erro ao realizar login');
    }

    return await res.json();
  },

  async register(email, password, nome = '') {
    const res = await fetch(`${API_BASE}/register`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ email, password, nome })
    });

    if (!res.ok) {
      const errorMsg = await res.text();
      throw new Error(errorMsg || 'Erro ao realizar cadastro');
    }

    return await res.json();
  },

  async requestPasswordReset(email) {
    // API não possui endpoint real de reset de senha ainda, vamos apenas simular sucesso
    return new Promise((resolve) => {
      setTimeout(() => {
        resolve({ success: true })
      }, 500)
    })
  }
}
