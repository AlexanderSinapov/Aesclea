<!-- Copyright (c) 2025 Alexander Sinapov | Simeon Petkov -->

<!-- All rights reserved. -->
<!-- This code is proprietary and confidential.   -->
<!-- Unauthorized copying, modification, distribution, or use is strictly prohibited. -->

<template>
  <div class="flex items-center justify-center min-h-screen px-4 py-12 bg-gradient-to-br from-green-50 to-teal-100 dark:from-gray-900 dark:to-gray-800 sm:px-6 lg:px-8">
    <div class="w-full max-w-md p-8 space-y-8 bg-white shadow-lg dark:bg-gray-800 rounded-xl">
      <!-- Loading state -->
      <div v-if="isVerifying" class="text-center">
        <div class="flex items-center justify-center w-12 h-12 mx-auto bg-blue-600 rounded-full">
          <svg class="w-8 h-8 text-white animate-spin" fill="none" viewBox="0 0 24 24">
            <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
            <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
          </svg>
        </div>
        <h2 class="mt-6 text-3xl font-extrabold text-center text-gray-900 dark:text-white">
          Verifying Email...
        </h2>
        <p class="mt-2 text-sm text-center text-gray-600 dark:text-gray-400">
          Please wait while we verify your email address
        </p>
      </div>

      <!-- Success state -->
      <div v-else-if="verificationSuccess" class="text-center">
        <div class="flex items-center justify-center w-12 h-12 mx-auto bg-green-600 rounded-full">
          <svg class="w-8 h-8 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 13l4 4L19 7" />
          </svg>
        </div>
        <h2 class="mt-6 text-3xl font-extrabold text-center text-gray-900 dark:text-white">
          Email Verified! 🎉
        </h2>
        <p class="mt-2 text-sm text-center text-gray-600 dark:text-gray-400">
          Your email has been successfully verified. You can now access all features.
        </p>
        
        <div class="mt-8">
          <button
            @click="continueToDashboard"
            class="flex justify-center w-full px-4 py-3 text-sm font-medium text-white transition-colors bg-green-600 border border-transparent rounded-md shadow-sm hover:bg-green-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-green-500"
          >
            Continue to Dashboard
          </button>
        </div>
      </div>

      <!-- Error state -->
      <div v-else class="text-center">
        <div class="flex items-center justify-center w-12 h-12 mx-auto bg-red-600 rounded-full">
          <svg class="w-8 h-8 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
          </svg>
        </div>
        <h2 class="mt-6 text-3xl font-extrabold text-center text-gray-900 dark:text-white">
          Verification Failed
        </h2>
        <p class="mt-2 text-sm text-center text-gray-600 dark:text-gray-400">
          {{ verificationError || 'The verification link is invalid or has expired.' }}
        </p>
        
        <div class="mt-8 space-y-4">
          <button
            @click="resendVerification"
            :disabled="isResending"
            class="flex justify-center w-full px-4 py-3 text-sm font-medium text-white transition-colors bg-blue-600 border border-transparent rounded-md shadow-sm hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500 disabled:opacity-50"
          >
            <svg v-if="isResending" class="w-5 h-5 mr-3 -ml-1 text-white animate-spin" fill="none" viewBox="0 0 24 24">
              <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
              <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
            </svg>
            {{ isResending ? 'Sending...' : 'Resend Verification Email' }}
          </button>
          
          <button
            @click="goToLogin"
            class="flex justify-center w-full px-4 py-2 text-sm font-medium text-gray-700 transition-colors bg-white border border-gray-300 rounded-md shadow-sm dark:border-gray-600 dark:text-gray-300 dark:bg-gray-700 hover:bg-gray-50 dark:hover:bg-gray-600 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500"
          >
            Back to Login
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { useAuthStore } from '../stores/auth'

const router = useRouter()
const route = useRoute()
const authStore = useAuthStore()

const isVerifying = ref(true)
const verificationSuccess = ref(false)
const verificationError = ref<string | null>(null)
const isResending = ref(false)

const verifyEmailToken = async (token: string) => {
  try {
    await authStore.verifyEmail(token)
    verificationSuccess.value = true
  } catch (error: any) {
    verificationError.value = error.message || 'Verification failed'
  } finally {
    isVerifying.value = false
  }
}

const continueToDashboard = () => {
  router.push('/dashboard')
}

const resendVerification = async () => {
  if (!authStore.user?.email) {
    router.push('/login')
    return
  }

  isResending.value = true
  try {
    await authStore.sendVerificationEmail(authStore.user.email)
    router.push(`/email-verification?email=${encodeURIComponent(authStore.user.email)}`)
  } catch (error) {
    console.error('Failed to resend verification email:', error)
  } finally {
    isResending.value = false
  }
}

const goToLogin = () => {
  router.push('/login')
}

onMounted(async () => {
  const token = route.query.token as string
  
  if (!token) {
    verificationError.value = 'No verification token provided'
    isVerifying.value = false
    return
  }

  await verifyEmailToken(token)
})
</script>
