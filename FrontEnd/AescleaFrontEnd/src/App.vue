<script setup lang="ts">
import { onMounted, nextTick } from 'vue'
import { useTheme } from './composables/useTheme'

const { initializeTheme } = useTheme()

// Ensure theme is initialized as early as possible
onMounted(async () => {
  console.log('App.vue mounted, initializing theme...')
  await nextTick()
  initializeTheme()
})

// Note: Removed duplicate watcher - the composable handles DOM updates
</script>

<template>
    <div class="min-h-screen bg-white dark:bg-gray-900 text-gray-900 dark:text-gray-100 transition-colors duration-300">
        <!-- Replace the Welcome component with router-view -->
        <router-view />
    </div>
</template>

