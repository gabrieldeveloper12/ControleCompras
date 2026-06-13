<template>
  <MainLayout v-if="route.meta.layout === 'main'">
    <router-view />
  </MainLayout>
  <router-view v-else />

  <!-- Premium Toast Notifications Overlay -->
  <div class="toast-container">
    <transition-group name="toast">
      <div 
        v-for="toast in toasts" 
        :key="toast.id" 
        class="toast-alert" 
        :class="toast.type"
      >
        <span class="toast-icon">
          <span v-if="toast.type === 'success'">✅</span>
          <span v-else-if="toast.type === 'error'">❌</span>
          <span v-else-if="toast.type === 'warning'">⚠️</span>
          <span v-else>ℹ️</span>
        </span>
        <span class="toast-message">{{ toast.message }}</span>
      </div>
    </transition-group>
  </div>
</template>

<script setup>
import { ref, onMounted, onBeforeUnmount } from 'vue'
import { useRoute } from 'vue-router'
import MainLayout from './components/MainLayout.vue'

const route = useRoute()
const toasts = ref([])


function handleToast(event) {
  const { message, type = 'success', duration = 3000 } = event.detail
  const id = Date.now() + Math.random()
  toasts.value.push({ id, message, type })
  
  setTimeout(() => {
    toasts.value = toasts.value.filter(t => t.id !== id)
  }, duration)
}

onMounted(() => {
  window.addEventListener('cc-toast', handleToast)
})

onBeforeUnmount(() => {
  window.removeEventListener('cc-toast', handleToast)
})
</script>

<style>
/* Os estilos globais e utilitários são gerenciados centralmente pelo src/style.css */
</style>

