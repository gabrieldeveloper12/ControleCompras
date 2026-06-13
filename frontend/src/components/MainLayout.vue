<template>
  <div class="app-layout" :class="{ 'sidebar-collapsed': isCollapsed }">
    <!-- Sidebar Desktop -->
    <aside class="sidebar glass-panel">
      <div class="sidebar-header">
        <span class="logo-emoji">💸</span>
        <transition name="fade">
          <div v-if="!isCollapsed" class="logo-text">
            <h2>Controle</h2>
            <p>Compras &amp; Gastos</p>
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

    <!-- Page Content Wrapper -->
    <div class="content-wrapper">
      <slot></slot>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'

const isCollapsed = ref(localStorage.getItem('cc-sidebar-collapsed') === 'true')

function toggleCollapse() {
  isCollapsed.value = !isCollapsed.value
  localStorage.setItem('cc-sidebar-collapsed', isCollapsed.value)
}
</script>

<style scoped>
/* Scoped styles are minimal, most tokens are mapped in global style.css */
</style>
