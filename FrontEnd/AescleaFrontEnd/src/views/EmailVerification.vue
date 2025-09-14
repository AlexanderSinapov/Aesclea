<!-- Copyright (c) 2025 Alexander Sinapov | Simeon Petkov -->

<!-- All rights reserved. -->
<!-- This code is proprietary and confidential.   -->
<!-- Unauthorized copying, modification, distribution, or use is strictly prohibited. -->

<template>
  <div class="flex items-center justify-center min-h-screen px-4 py-12 bg-gradient-to-br from-blue-50 to-indigo-100 dark:from-gray-900 dark:to-gray-800 sm:px-6 lg:px-8">
    <div class="w-full max-w-md p-8 space-y-8 bg-white shadow-lg dark:bg-gray-800 rounded-xl">
      <!-- Header -->
      <div class="text-center">
        <div class="flex items-center justify-center w-12 h-12 mx-auto bg-blue-600 rounded-full">
          <svg class="w-8 h-8 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 8l7.89 4.26a2 2 0 002.22 0L21 8M5 19h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v10a2 2 0 002 2z" />
          </svg>
        </div>
        <h2 class="mt-6 text-3xl font-extrabold text-center text-gray-900 dark:text-white">
          Check Your Email
        </h2>
        <p class="mt-2 text-sm text-center text-gray-600 dark:text-gray-400">
          We've sent a verification link to your email address
        </p>
      </div>

      <!-- Content -->
      <div class="space-y-6">
        <!-- Email display -->
        <div class="p-4 text-center rounded-lg bg-gray-50 dark:bg-gray-700">
          <p class="text-sm text-gray-600 dark:text-gray-300">
            Verification email sent to:
          </p>
          <p class="mt-1 text-lg font-semibold text-gray-900 dark:text-white">
            {{ email }}
          </p>
        </div>

        <!-- Instructions -->
        <div class="space-y-4 text-center">
          <p class="text-sm text-gray-600 dark:text-gray-400">
            Please check your email and click the verification link to activate your account.
            The link will expire in 24 hours.
          </p>
          
          <!-- Resend button -->
          <div class="space-y-2">
            <button
              @click="resendVerificationEmail"
              :disabled="isResending || countdown > 0"
              class="flex justify-center w-full px-4 py-2 text-sm font-medium text-white transition-colors bg-blue-600 border border-transparent rounded-md shadow-sm hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500 disabled:opacity-50 disabled:cursor-not-allowed"
            >
              <svg v-if="isResending" class="w-5 h-5 mr-3 -ml-1 text-white animate-spin" fill="none" viewBox="0 0 24 24">
                <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
                <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
              </svg>
              {{ countdown > 0 ? `Resend in ${countdown}s` : isResending ? 'Sending...' : 'Resend Email' }}
            </button>
            
            <p v-if="resendSuccess" class="text-sm text-green-600 dark:text-green-400">
              Verification email sent successfully!
            </p>
            <p v-if="resendError" class="text-sm text-red-600 dark:text-red-400">
              {{ resendError }}
            </p>
          </div>
        </div>

        <!-- Alternative actions -->
        <div class="space-y-3">
          <hr class="border-gray-300 dark:border-gray-600">
          
          <div class="space-y-2 text-center">
            <p class="text-sm text-gray-600 dark:text-gray-400">
              Need help or want to use a different email?
            </p>
            
            <div class="flex justify-center space-x-4">
              <button
                @click="goToLogin"
                class="text-sm font-medium text-blue-600 hover:text-blue-500 dark:text-blue-400 dark:hover:text-blue-300"
              >
                Back to Login
              </button>
              <button
                @click="changeEmail"
                class="text-sm font-medium text-gray-600 hover:text-gray-500 dark:text-gray-400 dark:hover:text-gray-300"
              >
                Change Email
              </button>
            </div>
          </div>
        </div>

        <!-- Tips -->
        <div class="p-4 border border-yellow-200 rounded-lg bg-yellow-50 dark:bg-yellow-900/20 dark:border-yellow-800">
          <div class="flex">
            <svg class="w-5 h-5 text-yellow-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
            </svg>
            <div class="ml-3">
              <h3 class="text-sm font-medium text-yellow-800 dark:text-yellow-200">
                Email not received?
              </h3>
              <div class="mt-2 text-sm text-yellow-700 dark:text-yellow-300">
                <ul class="space-y-1 list-disc list-inside">
                  <li>Check your spam/junk folder</li>
                  <li>Make sure the email address is correct</li>
                  <li>Wait a few minutes for delivery</li>
                  <li>Try resending the email</li>
                </ul>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted } from 'vue'
import { useRouter, useRoute } from 'vue-router'

const router = useRouter()
const route = useRoute()

const email = ref(route.query.email as string || '')
const isResending = ref(false)
const resendSuccess = ref(false)
const resendError = ref<string | null>(null)
const countdown = ref(0)
let countdownInterval: number | null = null

const startCountdown = () => {
  countdown.value = 60
  countdownInterval = setInterval(() => {
    countdown.value--
    if (countdown.value <= 0) {
      clearInterval(countdownInterval!)
      countdownInterval = null
    }
  }, 1000)
}

const resendVerificationEmail = async () => {
  if (countdown.value > 0 || isResending.value) return

  isResending.value = true
  resendSuccess.value = false
  resendError.value = null

  try {
    // Import the auth store
    const { useAuthStore } = await import('../stores/auth')
    const authStore = useAuthStore()
    
    // Make actual API call to resend verification email
    await authStore.sendVerificationEmail(email.value)
    
    resendSuccess.value = true
    startCountdown()
  } catch (err: any) {
    resendError.value = err.message || 'Failed to resend email. Please try again.'
  } finally {
    isResending.value = false
  }
}

const goToLogin = () => {
  router.push('/login')
}

const changeEmail = () => {
  router.push('/register')
}

onMounted(() => {
  if (!email.value) {
    router.push('/register')
    return
  }
  startCountdown()
})

onUnmounted(() => {
  if (countdownInterval) {
    clearInterval(countdownInterval)
  }
})
</script>
