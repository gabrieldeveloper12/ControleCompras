import { createApp } from 'vue'
import { createPinia } from 'pinia'
import router from './router'
import '@fontsource/inter/400.css'
import '@fontsource/inter/500.css'
import '@fontsource/inter/600.css'
import '@fontsource/inter/700.css'
import './style.css'
import App from './App.vue'

const app = createApp(App)

window.showToast = (message, type = 'success') => {
  window.dispatchEvent(new CustomEvent('cc-toast', {
    detail: { message, type }
  }));
};

app.use(createPinia())
app.use(router)

app.mount('#app')
