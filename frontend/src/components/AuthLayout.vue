<template>
  <div class="auth-container">
    <div class="glass-panel auth-card animate-fade-in">
      <div class="auth-header">
        <h1 class="auth-title">{{ title }}</h1>
        <p v-if="subtitle" class="auth-subtitle">{{ subtitle }}</p>
      </div>
      
      <slot></slot>
      
      <div v-if="$slots.footer" class="auth-footer">
        <slot name="footer"></slot>
      </div>
    </div>
  </div>
</template>

<script setup>
defineProps({
  title: {
    type: String,
    required: true
  },
  subtitle: {
    type: String,
    default: ''
  }
})
</script>

<style scoped>
.auth-container {
  display: flex;
  align-items: center;
  justify-content: center;
  min-height: 100vh;
  padding: 1rem;
}

.auth-card {
  width: 100%;
  max-width: 420px;
  padding: 2.5rem;
}

.auth-header {
  text-align: center;
  margin-bottom: 2rem;
}

.auth-title {
  font-size: 1.75rem;
  color: var(--text-primary);
  margin-bottom: 0.5rem;
}

.auth-subtitle {
  font-size: 0.95rem;
  color: var(--text-secondary);
}

.auth-footer {
  margin-top: 2rem;
  text-align: center;
  font-size: 0.9rem;
  color: var(--text-secondary);
}
</style>
