// Copyright (c) 2025 Alexander Sinapov | Simeon Petkov

// All rights reserved.
// This code is proprietary and confidential.  
// Unauthorized copying, modification, distribution, or use is strictly prohibited.

import { createApp } from 'vue'
import { createPinia } from 'pinia'
import App from './App.vue'
import router from './router'
import './style.css'
// Import theme to ensure it's initialized early
import './composables/useTheme'

const app = createApp(App)
const pinia = createPinia()

// Important: Use Pinia before router
app.use(pinia)
app.use(router)

app.mount('#app')
