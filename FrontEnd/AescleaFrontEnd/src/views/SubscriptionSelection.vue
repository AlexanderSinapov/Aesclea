<template>
  <div class="min-h-screen bg-gradient-to-br from-purple-50 via-blue-50 to-indigo-100 dark:from-gray-900 dark:via-gray-800 dark:to-gray-900">
    <!-- Header -->
    <header class="bg-white dark:bg-gray-800 shadow-sm border-b border-gray-200 dark:border-gray-700">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="flex justify-between items-center py-4">
          <div class="flex items-center">
            <div class="h-8 w-8 bg-gradient-to-r from-purple-600 to-indigo-600 rounded-lg flex items-center justify-center">
              <svg class="h-5 w-5 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4.318 6.318a4.5 4.5 0 000 6.364L12 20.364l7.682-7.682a4.5 4.5 0 00-6.364-6.364L12 7.636l-1.318-1.318a4.5 4.5 0 00-6.364 0z" />
              </svg>
            </div>
            <h1 class="ml-3 text-xl font-semibold text-gray-900 dark:text-white">Aesclea</h1>
          </div>
          
          <div class="flex items-center space-x-4">
            <div class="flex items-center space-x-3">
              <div class="h-8 w-8 bg-gray-300 dark:bg-gray-600 rounded-full flex items-center justify-center">
                <span class="text-sm font-medium text-gray-700 dark:text-gray-300">
                  {{ userInitials }}
                </span>
              </div>
              <div class="hidden md:block">
                <p class="text-sm font-medium text-gray-900 dark:text-white">{{ userDisplayName }}</p>
                <p class="text-xs text-gray-500 dark:text-gray-400">New User</p>
              </div>
            </div>
            
            <button @click="handleLogout" class="text-gray-400 hover:text-gray-500 dark:hover:text-gray-300" title="Logout">
              <svg class="h-5 w-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17 16l4-4m0 0l-4-4m4 4H7m6 4v1a3 3 0 01-3 3H6a3 3 0 01-3-3V7a3 3 0 013-3h4a3 3 0 013 3v1" />
              </svg>
            </button>
          </div>
        </div>
      </div>
    </header>

    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-12">
      <!-- Welcome Section -->
      <div class="text-center mb-12">
        <h2 class="text-4xl font-bold text-gray-900 dark:text-white mb-4">
          Welcome to Aesclea, {{ authStore.user?.firstName }}! 🎉
        </h2>
        <p class="text-xl text-gray-600 dark:text-gray-300 mb-6">
          To get started with AI-powered medical analysis, please choose a subscription plan.
        </p>
        <div class="bg-blue-50 dark:bg-blue-900/20 border border-blue-200 dark:border-blue-800 rounded-lg p-4 max-w-2xl mx-auto">
          <div class="flex items-center justify-center space-x-2">
            <svg class="h-5 w-5 text-blue-500" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
            </svg>
            <p class="text-sm text-blue-700 dark:text-blue-300">
              <strong>Note:</strong> AI analysis features require an active subscription. You can explore other features with any plan.
            </p>
          </div>
        </div>
      </div>

      <!-- Pricing Plans -->
      <div class="grid grid-cols-1 md:grid-cols-3 gap-8 mb-12">
        <div 
          v-for="plan in subscriptionStore.availablePlans" 
          :key="plan.id"
          class="bg-white dark:bg-gray-800 rounded-2xl shadow-lg border border-gray-200 dark:border-gray-700 hover:shadow-xl transition-all duration-300 relative"
          :class="{ 'ring-2 ring-purple-500 scale-105': plan.recommended }"
        >
          <!-- Recommended badge -->
          <div v-if="plan.recommended" class="absolute -top-4 left-1/2 transform -translate-x-1/2">
            <span class="bg-purple-500 text-white px-4 py-1 rounded-full text-sm font-medium">
              Recommended
            </span>
          </div>

          <div class="p-8">
            <!-- Plan header -->
            <div class="text-center mb-6">
              <h3 class="text-2xl font-bold text-gray-900 dark:text-white mb-2">{{ plan.name }}</h3>
              <div class="mb-4">
                <span class="text-4xl font-extrabold text-gray-900 dark:text-white">${{ plan.price }}</span>
                <span class="text-gray-600 dark:text-gray-300">/{{ plan.interval }}</span>
              </div>
            </div>

            <!-- Features list -->
            <ul class="space-y-3 mb-8">
              <li v-for="feature in plan.features" :key="feature" class="flex items-center">
                <svg class="w-5 h-5 text-green-500 mr-3 flex-shrink-0" fill="currentColor" viewBox="0 0 20 20">
                  <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd" />
                </svg>
                <span class="text-gray-600 dark:text-gray-300">{{ feature }}</span>
              </li>
            </ul>

            <!-- CTA Button -->
            <button
              @click="selectPlan(plan)"
              :disabled="subscriptionStore.isLoading"
              class="w-full py-3 px-4 rounded-lg text-sm font-medium transition-colors focus:outline-none focus:ring-2 focus:ring-offset-2"
              :class="plan.recommended 
                ? 'bg-purple-600 hover:bg-purple-700 text-white focus:ring-purple-500' 
                : 'bg-gray-100 hover:bg-gray-200 text-gray-900 dark:bg-gray-700 dark:hover:bg-gray-600 dark:text-white focus:ring-gray-500'"
            >
              <span v-if="subscriptionStore.isLoading" class="flex items-center justify-center">
                <svg class="animate-spin -ml-1 mr-3 h-5 w-5" fill="none" viewBox="0 0 24 24">
                  <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
                  <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                </svg>
                Processing...
              </span>
              <span v-else>Choose {{ plan.name }}</span>
            </button>
          </div>
        </div>
      </div>

      <p class="text-sm text-gray-600 dark:text-gray-400 mb-4 text-center">
        *If you are a medical facility or organization, please contact us for custom plans and pricing.
      </p>

      <!-- Skip option -->
      <div class="text-center">
        <div class="bg-gray-50 dark:bg-gray-800 rounded-lg p-6 max-w-md mx-auto">
          <h3 class="text-lg font-medium text-gray-900 dark:text-white mb-2">
            Not ready to subscribe?
          </h3>
          <p class="text-sm text-gray-600 dark:text-gray-400 mb-4">
            You can explore the dashboard and other features, but AI analysis will be unavailable until you subscribe.
          </p>
          <button
            @click="skipForNow"
            class="w-full py-2 px-4 border border-gray-300 dark:border-gray-600 rounded-md text-sm font-medium text-gray-700 dark:text-gray-300 hover:bg-gray-50 dark:hover:bg-gray-700 transition-colors"
          >
            Continue without subscription
          </button>
        </div>
      </div>

      <!-- Error display -->
      <div v-if="subscriptionStore.error" class="mt-8 bg-red-50 dark:bg-red-900/20 border border-red-200 dark:border-red-800 rounded-md p-4 max-w-md mx-auto">
        <div class="flex">
          <svg class="h-5 w-5 text-red-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
          </svg>
          <div class="ml-3">
            <p class="text-sm text-red-600 dark:text-red-400">{{ subscriptionStore.error }}</p>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '../stores/auth'
import { useSubscriptionStore, type SubscriptionPlan } from '../stores/subscription'

const router = useRouter()
const authStore = useAuthStore()
const subscriptionStore = useSubscriptionStore()

const userDisplayName = computed(() => {
  const user = authStore.user
  if (user) {
    return `${user.firstName} ${user.lastName}`
  }
  return 'User'
})

const userInitials = computed(() => {
  const user = authStore.user
  if (user) {
    return `${user.firstName.charAt(0)}${user.lastName.charAt(0)}`.toUpperCase()
  }
  return 'U'
})

const selectPlan = async (plan: SubscriptionPlan) => {
  try {
    await subscriptionStore.subscribeToPlan(plan.id)
    // Redirect to dashboard after successful subscription
    router.push('/dashboard')
  } catch (error) {
    console.error('Subscription failed:', error)
  }
}

const skipForNow = () => {
  router.push('/dashboard')
}

const handleLogout = async () => {
  await authStore.logout()
  router.push('/login')
}

onMounted(() => {
  // Check if user is authenticated
  if (!authStore.isAuthenticated) {
    router.push('/login')
    return
  }

  // If user already has a subscription, redirect to dashboard
  if (subscriptionStore.hasActiveSubscription) {
    router.push('/dashboard')
  }
})
</script>
