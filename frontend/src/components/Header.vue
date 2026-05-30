<template>
  <header class="header glass-panel animate-fade-in">
    <div class="logo">
      <span class="logo-emoji">💸</span>
      <div class="logo-text">
        <h1>ControleCompras</h1>
        <p>Gestão Pessoal de Gastos</p>
      </div>
    </div>

    <div class="header-right">
      <div class="api-status">
        <span class="status-indicator" :class="{ online: isOnline }"></span>
        <span class="status-text">{{ isOnline ? 'Servidor Conectado' : 'Conectando...' }}</span>
      </div>

      <!-- Settings gear with teleported theme dropdown -->
      <div class="settings-wrapper" ref="settingsWrapperRef">
        <button
          class="settings-btn"
          :class="{ active: dropdownOpen }"
          @click="toggleDropdown"
          title="Configurações de Tema"
        >
          ⚙️
        </button>

        <transition name="dropdown">
          <div
            v-if="dropdownOpen"
            class="theme-dropdown"
          >
            <p class="dropdown-label">Aparência</p>

            <button
              class="theme-option"
              :class="{ selected: currentTheme === 'dark' }"
              @click="setTheme('dark')"
            >
              <span class="theme-icon">🌙</span>
              <span class="theme-name">Escuro</span>
              <span v-if="currentTheme === 'dark'" class="theme-check">✓</span>
            </button>

            <button
              class="theme-option"
              :class="{ selected: currentTheme === 'light' }"
              @click="setTheme('light')"
            >
              <span class="theme-icon">☀️</span>
              <span class="theme-name">Claro</span>
              <span v-if="currentTheme === 'light'" class="theme-check">✓</span>
            </button>
          </div>
        </transition>
      </div>

      <!-- User Avatar with dropdown -->
      <div class="user-wrapper" ref="userWrapperRef">
        <div class="user-avatar" title="Opções de Usuário" @click="toggleUserDropdown">
          <span class="avatar-icon">👤</span>
        </div>

        <transition name="dropdown">
          <div
            v-if="userDropdownOpen"
            class="theme-dropdown"
          >
            <p class="dropdown-label">Sua Conta</p>
            <button class="theme-option text-danger" @click="logout">
              <span class="theme-icon">🚪</span>
              <span class="theme-name">Sair do Sistema</span>
            </button>
          </div>
        </transition>
      </div>
    </div>
  </header>
</template>

<script>
import { useAuthStore } from '../stores/auth'

export default {
  name: 'Header',

  props: {
    isOnline: {
      type: Boolean,
      default: false
    }
  },

  data() {
    return {
      dropdownOpen: false,
      userDropdownOpen: false,
      currentTheme: localStorage.getItem('cc-theme') || 'dark'
    }
  },

  mounted() {
    this.applyTheme(this.currentTheme)
    document.addEventListener('click', this.handleOutsideClick)
  },

  beforeUnmount() {
    document.removeEventListener('click', this.handleOutsideClick)
  },

  methods: {
    toggleDropdown() {
      this.dropdownOpen = !this.dropdownOpen
      this.userDropdownOpen = false // close other
    },
    toggleUserDropdown() {
      this.userDropdownOpen = !this.userDropdownOpen
      this.dropdownOpen = false // close other
    },

    handleOutsideClick(event) {
      if (this.dropdownOpen) {
        const wrapper = this.$refs.settingsWrapperRef
        if (wrapper && !wrapper.contains(event.target)) {
          this.dropdownOpen = false
        }
      }
      if (this.userDropdownOpen) {
        const wrapper = this.$refs.userWrapperRef
        if (wrapper && !wrapper.contains(event.target)) {
          this.userDropdownOpen = false
        }
      }
    },

    setTheme(theme) {
      this.currentTheme = theme
      this.applyTheme(theme)
      localStorage.setItem('cc-theme', theme)
      this.dropdownOpen = false
    },

    applyTheme(theme) {
      if (theme === 'light') {
        document.documentElement.setAttribute('data-theme', 'light')
      } else {
        document.documentElement.removeAttribute('data-theme')
      }
    },
    logout() {
      const authStore = useAuthStore()
      authStore.clearToken()
      this.userDropdownOpen = false
      this.$router.push('/login')
    }
  }
}
</script>

<style scoped>
.header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1rem 2rem;
  border-radius: var(--radius-md);
  margin-bottom: 2rem;
  background: var(--bg-card);
  position: relative;
  z-index: 1000;
}

.logo {
  display: flex;
  align-items: center;
  gap: 1rem;
}

.logo-emoji {
  font-size: 2.2rem;
  filter: drop-shadow(0 0 10px var(--primary-glow));
}

.logo-text h1 {
  font-size: 1.5rem;
  font-weight: 700;
  background: linear-gradient(135deg, var(--text-primary) 0%, var(--secondary) 100%);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
}

.logo-text p {
  font-size: 0.8rem;
  color: var(--text-secondary);
}

.api-status {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  font-size: 0.85rem;
  font-weight: 500;
  padding: 0.4rem 0.8rem;
  border-radius: var(--radius-sm);
  background: var(--surface-1);
  border: 1px solid var(--border-glass);
}

.status-indicator {
  display: inline-block;
  width: 8px;
  height: 8px;
  border-radius: 50%;
  background-color: var(--danger);
  box-shadow: 0 0 8px var(--danger);
  transition: all var(--transition-fast);
}

.status-indicator.online {
  background-color: var(--success);
  box-shadow: 0 0 8px var(--success);
}

.status-text {
  color: var(--text-secondary);
}

/* ── Header Right ── */
.header-right {
  display: flex;
  align-items: center;
  gap: 0.75rem;
}

/* ── Settings wrapper (relative anchor for dropdown) ── */
.settings-wrapper {
  position: relative;
}

.settings-btn {
  background: var(--surface-2);
  border: 1px solid var(--border-glass);
  border-radius: var(--radius-sm);
  padding: 0.4rem 0.55rem;
  font-size: 1.1rem;
  cursor: pointer;
  transition: transform 0.4s ease, background 0.2s ease, box-shadow 0.2s ease;
  line-height: 1;
  display: flex;
  align-items: center;
  justify-content: center;
}

.settings-btn:hover,
.settings-btn.active {
  transform: rotate(180deg);
  background: var(--surface-4);
  box-shadow: 0 0 12px var(--primary-glow);
}

/* ── Dropdown animation (scoped ok here) ── */
.dropdown-enter-active,
.dropdown-leave-active {
  transition: opacity 0.18s ease, transform 0.18s ease;
}
.dropdown-enter-from,
.dropdown-leave-to {
  opacity: 0;
  transform: translateY(-6px) scale(0.97);
}

/* ── User Avatar ── */
.user-wrapper {
  position: relative;
}

.user-avatar {
  width: 36px;
  height: 36px;
  border-radius: 50%;
  background: var(--surface-3);
  border: 2px solid var(--border-glass);
  box-shadow: 0 0 10px var(--primary-glow), inset 0 1px 0 var(--border-glass);
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 1rem;
  cursor: pointer;
  transition: box-shadow 0.25s ease, border-color 0.25s ease;
}

.user-avatar:hover {
  border-color: var(--primary);
  box-shadow: 0 0 18px var(--primary-glow);
}

/* ── Dropdown (nested absolutely inside settings-wrapper or user-wrapper) ── */
.theme-dropdown {
  position: absolute;
  top: calc(100% + 10px);
  right: 0;
  z-index: 99999;
  width: 175px;
  background: var(--bg-card);
  backdrop-filter: blur(20px);
  -webkit-backdrop-filter: blur(20px);
  border: 1px solid var(--border-glass);
  border-radius: var(--radius-md);
  box-shadow: 0 16px 48px rgba(0, 0, 0, 0.45), 0 0 0 1px rgba(255,255,255,0.05);
  padding: 0.6rem;
}

.dropdown-label {
  font-size: 0.7rem;
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: 0.08em;
  color: var(--text-muted);
  padding: 0.25rem 0.5rem 0.5rem;
}

.theme-option {
  width: 100%;
  display: flex;
  align-items: center;
  gap: 0.6rem;
  background: transparent;
  border: none;
  border-radius: var(--radius-sm);
  padding: 0.55rem 0.6rem;
  cursor: pointer;
  font-family: 'Outfit', 'Inter', sans-serif;
  font-size: 0.9rem;
  font-weight: 500;
  color: var(--text-secondary);
  transition: background 0.15s ease, color 0.15s ease;
  text-align: left;
}

.theme-option:hover {
  background: var(--surface-4);
  color: var(--text-primary);
}

.theme-option.selected {
  background: hsla(263, 80%, 62%, 0.15);
  color: var(--primary);
}

.theme-icon {
  font-size: 1rem;
  flex-shrink: 0;
}

.theme-name {
  flex: 1;
}

.theme-check {
  font-size: 0.8rem;
  color: var(--primary);
  font-weight: 700;
}

.text-danger {
  color: var(--danger) !important;
}
.text-danger:hover {
  background: rgba(255, 59, 48, 0.15) !important;
}

/* ── Responsive ── */
@media (max-width: 600px) {
  .header {
    flex-direction: column;
    gap: 1rem;
    padding: 1.2rem;
    align-items: flex-start;
  }

  .header-right {
    align-self: flex-end;
  }
}
</style>
