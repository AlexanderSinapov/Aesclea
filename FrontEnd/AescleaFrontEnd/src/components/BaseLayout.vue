<template>
  <div class="min-h-screen bg-white dark:bg-gray-900 text-gray-900 dark:text-gray-100 transition-colors duration-300">
    <AppNavbar v-if="showNavbar" />
    <main>
      <slot />
    </main>
  </div>
</template>

<script setup lang="ts">
import AppNavbar from './AppNavbar.vue'

interface Props {
  showNavbar?: boolean
}

withDefaults(defineProps<Props>(), {
  showNavbar: true
})
</script>
