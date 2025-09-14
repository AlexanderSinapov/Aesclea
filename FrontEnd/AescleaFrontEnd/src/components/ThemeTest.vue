<!-- Copyright (c) 2025 Alexander Sinapov | Simeon Petkov -->

<!-- All rights reserved. -->
<!-- This code is proprietary and confidential.   -->
<!-- Unauthorized copying, modification, distribution, or use is strictly prohibited. -->

<template>
  <div class="p-8 text-black transition-all duration-500 bg-white dark:bg-black dark:text-white">
    <h1 class="mb-4 text-4xl font-bold">Theme Test</h1>
    <p class="mb-4">This should change colors when you toggle dark mode.</p>
    
    <!-- Direct HTML class manipulation test -->
    <button 
      @click="testDarkClass"
      class="px-4 py-2 mr-4 text-white bg-blue-500 rounded hover:bg-blue-600"
    >
      Force Dark Class
    </button>
    
    <button 
      @click="testLightClass" 
      class="px-4 py-2 mr-4 text-white bg-gray-500 rounded hover:bg-gray-600"
    >
      Force Light Class
    </button>
    
    <!-- Theme composable test -->
    <button 
      @click="toggleTheme"
      class="px-4 py-2 text-white bg-green-500 rounded hover:bg-green-600"
    >
      Toggle Theme ({{ isDark ? 'Dark' : 'Light' }})
    </button>
    
    <div class="p-4 mt-8 border border-gray-300 rounded dark:border-gray-700">
      <h3 class="mb-2 font-bold">Debug Info:</h3>
      <p>isDark ref: {{ isDark }}</p>
      <p>HTML has dark class: {{ htmlHasDark }}</p>
      <p>HTML classes: {{ htmlClasses }}</p>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, watch } from 'vue'
import { useTheme } from '../composables/useTheme'

const { isDark, toggleTheme } = useTheme()

const htmlHasDark = ref(false)
const htmlClasses = ref('')

const updateDebugInfo = () => {
  htmlHasDark.value = document.documentElement.classList.contains('dark')
  htmlClasses.value = document.documentElement.className
}

const testDarkClass = () => {
  console.log('🧪 FORCE DARK TEST')
  document.documentElement.classList.add('dark')
  updateDebugInfo()
}

const testLightClass = () => {
  console.log('🧪 FORCE LIGHT TEST')
  document.documentElement.classList.remove('dark')
  updateDebugInfo()
}

onMounted(() => {
  updateDebugInfo()
  // Update debug info every second to see changes
  setInterval(updateDebugInfo, 1000)
})

watch(isDark, () => {
  console.log('👀 isDark watch in test component:', isDark.value)
  setTimeout(updateDebugInfo, 100)
})
</script>
