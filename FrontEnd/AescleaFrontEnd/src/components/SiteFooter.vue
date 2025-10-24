<!-- Copyright (c) 2025 Alexander Sinapov | Simeon Petkov -->
<!-- All rights reserved. -->
<!-- This code is proprietary and confidential. -->
<!-- Unauthorized copying, modification, distribution, or use is strictly prohibited. -->

<template>
  <footer class="bg-gray-900 dark:bg-black text-white py-12 px-4 sm:px-6 lg:px-8">
    <div class="max-w-7xl mx-auto">
      <div class="text-center">
        <div class="flex items-center justify-center space-x-2 mb-4">
          <div class="w-10 h-10 bg-gradient-to-br from-blue-600 to-indigo-600 rounded-lg flex items-center justify-center">
            <svg class="w-6 h-6 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9.663 17h4.673M12 3v1m6.364 1.636l-.707.707M21 12h-1M4 12H3m3.343-5.657l-.707-.707m2.828 9.9a5 5 0 117.072 0l-.548.547A3.374 3.374 0 0014 18.469V19a2 2 0 11-4 0v-.531c0-.895-.356-1.754-.988-2.386l-.548-.547z" />
            </svg>
          </div>
          <span class="text-2xl font-bold">Aesclea</span>
        </div>
        <p class="text-gray-400 mb-6">
          Pioneering the future of healthcare with AI-powered medical intelligence.
        </p>
        <div class="flex justify-center space-x-6 mb-6">
          <router-link to="/products" class="text-gray-400 hover:text-white transition-colors">Products</router-link>
          <router-link to="/pricing" class="text-gray-400 hover:text-white transition-colors">Pricing</router-link>
          <router-link to="/contact" class="text-gray-400 hover:text-white transition-colors">Contact</router-link>
          <router-link to="/privacy" class="text-gray-400 hover:text-white transition-colors">Privacy</router-link>
          <router-link to="/terms" class="text-gray-400 hover:text-white transition-colors">Terms</router-link>
        </div>
        <div class="border-t border-gray-800 pt-6">
          <p class="text-gray-400">&copy; 2025 Aesclea. All rights reserved.</p>
        </div>
      </div>
    </div>
  </footer>
</template>

<script setup lang="ts">
// Site Footer Component
</script>
