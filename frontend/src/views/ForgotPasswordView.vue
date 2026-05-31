<template>
  <AuthLayout title="Recuperar Senha" subtitle="Enviaremos um link para redefinir sua senha">
    <form @submit.prevent="handleReset" class="auth-form">
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

      <div v-if="errorMsg" class="error-msg">{{ errorMsg }}</div>
      <div v-if="successMsg" class="success-msg">{{ successMsg }}</div>

      <button type="submit" class="btn btn-primary w-full" :disabled="isLoading || successMsg">
        {{ isLoading ? 'Enviando...' : 'Enviar Link' }}
      </button>
    </form>

    <template #footer>
      <div class="footer-links">
        <p>
          Lembrou a senha? <router-link to="/login" class="text-link highlight">Voltar ao login</router-link>
        </p>
      </div>
    </template>
  </AuthLayout>
</template>

<script setup>
import { ref } from 'vue'
import AuthLayout from '../components/AuthLayout.vue'
import { AuthService } from '../services/authService'

const email = ref('')
const isLoading = ref(false)
const errorMsg = ref('')
const successMsg = ref('')

async function handleReset() {
  if (!email.value) {
    errorMsg.value = 'Preencha seu e-mail.'
    window.showToast('Preencha seu e-mail.', 'warning')
    return
  }

  try {
    isLoading.value = true
    errorMsg.value = ''
    await AuthService.requestPasswordReset(email.value)
    successMsg.value = 'Link de recuperação enviado! Verifique seu e-mail.'
    window.showToast('Link de recuperação enviado! Verifique seu e-mail.', 'success')
  } catch (err) {
    errorMsg.value = err.message || 'Erro ao solicitar recuperação. Tente novamente.'
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
