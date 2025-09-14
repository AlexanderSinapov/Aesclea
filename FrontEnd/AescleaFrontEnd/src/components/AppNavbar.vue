<!-- Copyright (c) 2025 Alexander Sinapov | Simeon Petkov -->

<!-- All rights reserved. -->
<!-- This code is proprietary and confidential.   -->
<!-- Unauthorized copying, modification, distribution, or use is strictly prohibited. -->

<template>
  <header class="relative transition-colors duration-300 bg-white shadow-sm dark:bg-gray-800">
    <div class="px-4 mx-auto max-w-7xl sm:px-6 lg:px-8">
      <div class="flex items-center justify-between py-6">
        <!-- Logo and Title -->
        <div class="flex items-center">
          <div class="flex items-center justify-center w-10 h-10 rounded-full bg-gradient-to-r from-purple-600 to-indigo-600">
            <svg class="w-6 h-6 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4.318 6.318a4.5 4.5 0 000 6.364L12 20.364l7.682-7.682a4.5 4.5 0 00-6.364-6.364L12 7.636l-1.318-1.318a4.5 4.5 0 00-6.364 0z" />
            </svg>
          </div>
          <h1 class="ml-3 text-2xl font-bold text-gray-900 dark:text-white">Aesclea</h1>
        </div>

        <!-- Navigation Links -->
        <nav class="hidden space-x-8 md:flex">
          <router-link to="/features" class="text-gray-700 transition-colors dark:text-gray-300 hover:text-indigo-600 dark:hover:text-indigo-400">Features</router-link>
          <router-link to="/pricing" class="text-gray-700 transition-colors dark:text-gray-300 hover:text-indigo-600 dark:hover:text-indigo-400">Pricing</router-link>
          <router-link to="/security" class="text-gray-700 transition-colors dark:text-gray-300 hover:text-indigo-600 dark:hover:text-indigo-400">Security</router-link>
          <router-link to="/updates" class="text-gray-700 transition-colors dark:text-gray-300 hover:text-indigo-600 dark:hover:text-indigo-400">Updates</router-link>
          <router-link to="/help" class="text-gray-700 transition-colors dark:text-gray-300 hover:text-indigo-600 dark:hover:text-indigo-400">Help</router-link>
          <router-link to="/contact" class="text-gray-700 transition-colors dark:text-gray-300 hover:text-indigo-600 dark:hover:text-indigo-400">Contact</router-link>
        </nav>

        <!-- Right Side Actions -->
        <div class="flex items-center space-x-4">
          <!-- Theme Toggle Button -->
          <div class="relative group">
            <button 
              @click="handleToggleClick"
              @dblclick="handleDoubleClick"
              class="relative flex items-center justify-center w-12 h-6 transition-all duration-300 ease-in-out bg-gray-200 rounded-full theme-toggle-button dark:bg-gray-700 focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:ring-offset-2 dark:focus:ring-offset-gray-800 hover:shadow-md hover:scale-105"
              :title="isDark ? 'Switch to light mode' : 'Switch to dark mode'"
            >
              <!-- Toggle Track -->
              <div class="absolute inset-0 transition-all duration-300 rounded-full bg-gradient-to-r from-gray-300 to-gray-400 dark:from-gray-600 dark:to-gray-500"
              ></div>
              
              <!-- Toggle Circle -->
              <div id = "theme-change-btn"
                class="relative z-10 flex items-center justify-center w-5 h-5 transition-all duration-500 ease-in-out transform bg-white rounded-full shadow-md dark:bg-gray-800"
                :class="[
                  isDark ? 'translate-x-3 rotate-270' : '-translate-x-3 rotate-0'
                ]"
              >
                <!-- Sun Icon (Light Mode) -->
                <svg 
                  v-if="!isDark" 
                  class="w-3 h-3 text-yellow-500 transition-all duration-700 ease-in-out transform" 
                  :class="isDark ? 'rotate-180' : 'rotate-0'"
                  fill="currentColor" 
                  viewBox="0 0 20 20"
                >
                  <path fill-rule="evenodd" d="M10 2a1 1 0 011 1v1a1 1 0 11-2 0V3a1 1 0 011-1zm4 8a4 4 0 11-8 0 4 4 0 018 0zm-.464 4.95l.707.707a1 1 0 001.414-1.414l-.707-.707a1 1 0 00-1.414 1.414zm2.12-10.607a1 1 0 010 1.414l-.706.707a1 1 0 11-1.414-1.414l.707-.707a1 1 0 011.414 0zM17 11a1 1 0 100-2h-1a1 1 0 100 2h1zm-7 4a1 1 0 011 1v1a1 1 0 11-2 0v-1a1 1 0 011-1zM5.05 6.464A1 1 0 106.465 5.05l-.708-.707a1 1 0 00-1.414 1.414l.707.707zm1.414 8.486l-.707.707a1 1 0 01-1.414-1.414l.707-.707a1 1 0 011.414 1.414zM4 11a1 1 0 100-2H3a1 1 0 000 2h1z" clip-rule="evenodd" />
                </svg>
                
                <!-- Moon Icon (Dark Mode) -->
                <svg 
                  v-else 
                  class="w-3 h-3 text-indigo-400 transition-all duration-700 ease-in-out transform" 
                  :class="!isDark ? 'rotate-180' : 'rotate-0'"
                  fill="currentColor" 
                  viewBox="0 0 20 20"
                >
                  <path d="M17.293 13.293A8 8 0 016.707 2.707a8.001 8.001 0 1010.586 10.586z" />
                </svg>
              </div>
            </button>
            
            <!-- Theme Label -->
            <div class="absolute text-xs text-gray-500 transition-opacity duration-200 transform -translate-x-1/2 opacity-0 pointer-events-none -bottom-8 left-1/2 dark:text-gray-400 whitespace-nowrap group-hover:opacity-100">
              {{ isDark ? 'Dark Mode' : 'Light Mode' }} ({{ isDark ? 'ON' : 'OFF' }})
            </div>
          </div>
          
          <!-- Auth Links -->
          <router-link
            to="/login"
            class="px-3 py-2 text-sm font-medium text-gray-700 transition-colors rounded-md dark:text-gray-300 hover:text-indigo-600 dark:hover:text-indigo-400"
          >
            Login
          </router-link>
          <router-link
            to="/register"
            class="px-4 py-2 text-sm font-medium text-white transition-colors bg-indigo-600 rounded-md hover:bg-indigo-700"
          >
            Sign Up
          </router-link>
        </div>
      </div>
    </div>
  </header>
</template>

<script setup lang="ts">
import { useTheme } from '../composables/useTheme'

const { isDark, toggleTheme, forceReset } = useTheme()

// Enhanced toggle handler with debugging
const handleToggleClick = () => {
  console.log('Toggle button clicked! Current state:', isDark.value)
  toggleTheme()
  console.log('After toggle call, new state:', isDark.value)
}

// Double-click to force reset (debugging feature)
const handleDoubleClick = () => {
  console.log('Double-click detected! Force resetting theme...')
  forceReset()
}
</script>

<style scoped>
.theme-toggle-button {
  position: relative;
  overflow: hidden;
}

.theme-toggle-button::before {
  content: '';
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: radial-gradient(circle, rgba(99, 102, 241, 0.1) 0%, transparent 70%);
  opacity: 0;
  transition: opacity 0.3s ease;
  pointer-events: none;
}

.theme-toggle-button:hover::before {
  opacity: 1;
}
</style>
