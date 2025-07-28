<template>
  <div class="p-8 bg-white dark:bg-black text-black dark:text-white transition-all duration-500">
    <h1 class="text-4xl font-bold mb-4">Theme Test</h1>
    <p class="mb-4">This should change colors when you toggle dark mode.</p>
    
    <!-- Direct HTML class manipulation test -->
    <button 
      @click="testDarkClass"
      class="px-4 py-2 bg-blue-500 hover:bg-blue-600 text-white rounded mr-4"
    >
      Force Dark Class
    </button>
    
    <button 
      @click="testLightClass" 
      class="px-4 py-2 bg-gray-500 hover:bg-gray-600 text-white rounded mr-4"
    >
      Force Light Class
    </button>
    
    <!-- Theme composable test -->
    <button 
      @click="toggleTheme"
      class="px-4 py-2 bg-green-500 hover:bg-green-600 text-white rounded"
    >
      Toggle Theme ({{ isDark ? 'Dark' : 'Light' }})
    </button>
    
    <div class="mt-8 p-4 border border-gray-300 dark:border-gray-700 rounded">
      <h3 class="font-bold mb-2">Debug Info:</h3>
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
