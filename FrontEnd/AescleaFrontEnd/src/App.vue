<!-- Copyright (c) 2025 Alexander Sinapov | Simeon Petkov -->

<!-- All rights reserved. -->
<!-- This code is proprietary and confidential.   -->
<!-- Unauthorized copying, modification, distribution, or use is strictly prohibited. -->

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
    <div class="min-h-screen text-gray-900 transition-colors duration-300 bg-white dark:bg-gray-900 dark:text-gray-100">
        <!-- Replace the Welcome component with router-view -->
        <router-view />
    </div>
</template>

