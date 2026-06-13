import { createRouter, createWebHistory } from 'vue-router'
import LoginView from '../views/LoginView.vue'
import SignupView from '../views/SignupView.vue'
import ForgotPasswordView from '../views/ForgotPasswordView.vue'
const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      redirect: '/login'
    },
    {
      path: '/dashboard',
      name: 'dashboard',
      component: () => import('../views/DashboardView.vue'),
      meta: { layout: 'main' }
    },
    {
      path: '/compras',
      name: 'compras',
      component: () => import('../views/ComprasView.vue'),
      meta: { layout: 'main' }
    },
    {
      path: '/despesas-fixas',
      name: 'despesas-fixas',
      component: () => import('../views/DespesasFixasView.vue'),
      meta: { layout: 'main' }
    },
    {
      path: '/login',
      name: 'login',
      component: LoginView
    },
    {
      path: '/signup',
      name: 'signup',
      component: SignupView
    },
    {
      path: '/forgot-password',
      name: 'forgot-password',
      component: ForgotPasswordView
    }
  ]
})

export default router
