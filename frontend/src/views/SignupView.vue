<template>
  <AuthLayout title="Criar Conta" subtitle="Junte-se a nós hoje">
    <form @submit.prevent="handleSignup" class="auth-form">
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
          minlength="6"
        />
      </div>

      <div class="form-group">
        <label for="confirmPassword" class="input-label">Confirmar Senha</label>
        <input 
          id="confirmPassword" 
          v-model="confirmPassword" 
          type="password" 
          class="input-control" 
          placeholder="••••••••" 
          required 
        />
      </div>

      <div v-if="errorMsg" class="error-msg">{{ errorMsg }}</div>
      <div v-if="successMsg" class="success-msg">{{ successMsg }}</div>

      <button type="submit" class="btn btn-primary w-full" :disabled="isLoading">
        {{ isLoading ? 'Criando conta...' : 'Cadastrar' }}
      </button>
    </form>

    <template #footer>
      <div class="footer-links">
        <p>
          Já tem uma conta? <router-link to="/login" class="text-link highlight">Fazer login</router-link>
        </p>
      </div>
    </template>
  </AuthLayout>
</template>

<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import AuthLayout from '../components/AuthLayout.vue'
import { AuthService } from '../services/authService'

const email = ref('')
const password = ref('')
const confirmPassword = ref('')
const isLoading = ref(false)
const errorMsg = ref('')
const successMsg = ref('')

const router = useRouter()

async function handleSignup() {
  errorMsg.value = ''
  successMsg.value = ''
  
  if (!email.value || !password.value || !confirmPassword.value) {
    errorMsg.value = 'Preencha todos os campos.'
    return
  }

  if (password.value !== confirmPassword.value) {
    errorMsg.value = 'As senhas não coincidem.'
    return
  }

  try {
    isLoading.value = true
    await AuthService.register(email.value, password.value)
    successMsg.value = 'Conta criada com sucesso! Redirecionando...'
    
    setTimeout(() => {
      router.push('/login')
    }, 1500)
  } catch (err) {
    errorMsg.value = err.message || 'Erro ao criar conta. Tente novamente.'
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

.success-msg {
  color: var(--success);
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
</style>
