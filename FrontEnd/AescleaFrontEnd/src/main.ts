import { createApp } from 'vue'
import { createPinia } from 'pinia'
import App from './App.vue'
import router from './router'
import './style.css'

const app = createApp(App)
const pinia = createPinia()

// Important: Use Pinia before router
app.use(pinia)
app.use(router)

app.mount('#app')
