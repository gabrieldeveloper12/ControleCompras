<template>
  <AuthLayout title="Bem-vindo de volta" subtitle="Faça login para continuar">
    <form @submit.prevent="handleLogin" class="auth-form">
      <div class="form-group">
        <label for="email" class="input-label">E-mail</label>
        <input 
          id="email" 
          v-model="email" 
          type="email" 
          class="input-control" 
          placeholder="seu@email.com" 
          required 
        />
      </div>

      <div class="form-group">
        <label for="password" class="input-label">Senha</label>
        <input 
          id="password" 
          v-model="password" 
          type="password" 
          class="input-control" 
          placeholder="••••••••" 
          required 
        />
      </div>

      <div v-if="errorMsg" class="error-msg">{{ errorMsg }}</div>

      <button type="submit" class="btn btn-primary w-full" :disabled="isLoading">
        {{ isLoading ? 'Entrando...' : 'Entrar' }}
      </button>
    </form>

    <template #footer>
      <div class="footer-links">
        <router-link to="/forgot-password" class="text-link">Esqueceu sua senha?</router-link>
        <p class="mt-2">
          Não tem uma conta? <router-link to="/signup" class="text-link highlight">Cadastre-se</router-link>
        </p>
      </div>
    </template>
  </AuthLayout>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import AuthLayout from '../components/AuthLayout.vue'
import { useAuthStore } from '../stores/auth'
import { AuthService } from '../services/authService'

const email = ref('')
const password = ref('')
const isLoading = ref(false)
const errorMsg = ref('')

const authStore = useAuthStore()
const router = useRouter()

onMounted(() => {
  const baseUrl = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5076'
  fetch(`${baseUrl}/health`).catch(() => {})
})

async function handleLogin() {
  if (!email.value || !password.value) {
    errorMsg.value = 'Preencha todos os campos.'
    window.showToast('Preencha todos os campos.', 'warning')
    return
  }

  try {
    isLoading.value = true
    errorMsg.value = ''
    const response = await AuthService.login(email.value, password.value)
    
    authStore.setToken(response.token)
    authStore.user = response.user
    
    window.showToast('Login realizado com sucesso! Bem-vindo.', 'success')
    // Redirect to dashboard
    router.push('/dashboard')
  } catch (err) {
    errorMsg.value = err.message || 'Credenciais inválidas. Tente novamente.'
    window.showToast(errorMsg.value, 'error')
  } finally {
    isLoading.value = false
  }
}
</script>

<style scoped>
.auth-form {
  display: flex;
  flex-direction: column;
  gap: 1.25rem;
}

.w-full {
  width: 100%;
}

.error-msg {
  color: var(--danger);
  font-size: 0.85rem;
  text-align: center;
}

.footer-links {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.text-link {
  color: var(--text-secondary);
  text-decoration: none;
  transition: color var(--transition-fast);
}

.text-link:hover {
  color: var(--text-primary);
}

.text-link.highlight {
  color: var(--primary);
  font-weight: 500;
}

.text-link.highlight:hover {
  filter: brightness(1.2);
}

.mt-2 {
  margin-top: 0.75rem;
}
</style>
