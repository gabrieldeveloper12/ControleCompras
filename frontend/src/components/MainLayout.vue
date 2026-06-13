<template>
  <div class="app-layout" :class="{ 'sidebar-collapsed': isCollapsed }">
    <!-- Sidebar for Desktop -->
    <aside class="sidebar glass-panel">
      <div class="sidebar-header">
        <span class="logo-emoji">💸</span>
        <transition name="fade">
          <div v-if="!isCollapsed" class="logo-text">
            <h2>Controle</h2>
            <p>Compras & Gastos</p>
          </div>
        </transition>
      </div>

      <nav class="sidebar-nav">
        <router-link to="/dashboard" class="nav-item" active-class="active">
          <span class="nav-icon" title="Painel Geral">📊</span>
          <span v-if="!isCollapsed" class="nav-label">Painel Geral</span>
        </router-link>

        <router-link to="/compras" class="nav-item" active-class="active">
          <span class="nav-icon" title="Lançamentos">🛒</span>
          <span v-if="!isCollapsed" class="nav-label">Lançamentos</span>
        </router-link>

        <router-link to="/despesas-fixas" class="nav-item" active-class="active">
          <span class="nav-icon" title="Despesas Fixas">📌</span>
          <span v-if="!isCollapsed" class="nav-label">Despesas Fixas</span>
        </router-link>
      </nav>

      <div class="sidebar-footer">
        <button class="btn-collapse" @click="toggleCollapse" :title="isCollapsed ? 'Expandir Menu' : 'Colapsar Menu'">
          <span class="collapse-icon">{{ isCollapsed ? '▶' : '◀' }}</span>
        </button>
      </div>
    </aside>

    <!-- Slide-in Drawer for Mobile -->
    <transition name="drawer">
      <div v-if="isMobileMenuOpen" class="mobile-drawer-overlay" @click.self="closeMobileMenu">
        <div class="mobile-drawer glass-panel">
          <div class="drawer-header">
            <div class="logo">
              <span class="logo-emoji">💸</span>
              <div class="logo-text">
                <h2>Controle</h2>
                <p>Compras & Gastos</p>
              </div>
            </div>
            <button class="btn-close-drawer" @click="closeMobileMenu">&times;</button>
          </div>

          <nav class="drawer-nav">
            <router-link to="/dashboard" class="drawer-item" active-class="active" @click="closeMobileMenu">
              <span class="nav-icon">📊</span>
              <span class="nav-label">Painel Geral</span>
            </router-link>

            <router-link to="/compras" class="drawer-item" active-class="active" @click="closeMobileMenu">
              <span class="nav-icon">🛒</span>
              <span class="nav-label">Lançamentos</span>
            </router-link>

            <router-link to="/despesas-fixas" class="drawer-item" active-class="active" @click="closeMobileMenu">
              <span class="nav-icon">📌</span>
              <span class="nav-label">Despesas Fixas</span>
            </router-link>
          </nav>
        </div>
      </div>
    </transition>

    <!-- Page Content Wrapper -->
    <div class="content-wrapper">
      <slot></slot>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, onBeforeUnmount } from 'vue'

const isCollapsed = ref(localStorage.getItem('cc-sidebar-collapsed') === 'true')
const isMobileMenuOpen = ref(false)

function toggleCollapse() {
  isCollapsed.value = !isCollapsed.value
  localStorage.setItem('cc-sidebar-collapsed', isCollapsed.value)
}

function openMobileMenu() {
  isMobileMenuOpen.value = true
  document.body.classList.add('modal-open')
}

function closeMobileMenu() {
  isMobileMenuOpen.value = false
  document.body.classList.remove('modal-open')
}

function handleMobileToggle() {
  if (isMobileMenuOpen.value) {
    closeMobileMenu()
  } else {
    openMobileMenu()
  }
}

onMounted(() => {
  window.addEventListener('cc-toggle-mobile-menu', handleMobileToggle)
})

onBeforeUnmount(() => {
  window.removeEventListener('cc-toggle-mobile-menu', handleMobileToggle)
  document.body.classList.remove('modal-open')
})
</script>

<style scoped>
/* Scoped styles are minimal, most tokens are mapped in global style.css */
</style>
