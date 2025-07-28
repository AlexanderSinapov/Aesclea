<template>
  <div class="min-h-screen bg-gradient-to-br from-green-50 to-teal-100 dark:from-gray-900 dark:to-gray-800 flex items-center justify-center py-12 px-4 sm:px-6 lg:px-8">
    <div class="max-w-md w-full space-y-8 bg-white dark:bg-gray-800 p-8 rounded-xl shadow-lg">
      <!-- Loading state -->
      <div v-if="isVerifying" class="text-center">
        <div class="mx-auto h-12 w-12 bg-blue-600 rounded-full flex items-center justify-center">
          <svg class="animate-spin h-8 w-8 text-white" fill="none" viewBox="0 0 24 24">
            <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
            <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
          </svg>
        </div>
        <h2 class="mt-6 text-center text-3xl font-extrabold text-gray-900 dark:text-white">
          Verifying Email...
        </h2>
        <p class="mt-2 text-center text-sm text-gray-600 dark:text-gray-400">
          Please wait while we verify your email address
        </p>
      </div>

      <!-- Success state -->
      <div v-else-if="verificationSuccess" class="text-center">
        <div class="mx-auto h-12 w-12 bg-green-600 rounded-full flex items-center justify-center">
          <svg class="h-8 w-8 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 13l4 4L19 7" />
          </svg>
        </div>
        <h2 class="mt-6 text-center text-3xl font-extrabold text-gray-900 dark:text-white">
          Email Verified! 🎉
        </h2>
        <p class="mt-2 text-center text-sm text-gray-600 dark:text-gray-400">
          Your email has been successfully verified. You can now access all features.
        </p>
        
        <div class="mt-8">
          <button
            @click="continueToDashboard"
            class="w-full flex justify-center py-3 px-4 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-green-600 hover:bg-green-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-green-500 transition-colors"
          >
            Continue to Dashboard
          </button>
        </div>
      </div>

      <!-- Error state -->
      <div v-else class="text-center">
        <div class="mx-auto h-12 w-12 bg-red-600 rounded-full flex items-center justify-center">
          <svg class="h-8 w-8 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
          </svg>
        </div>
        <h2 class="mt-6 text-center text-3xl font-extrabold text-gray-900 dark:text-white">
          Verification Failed
        </h2>
        <p class="mt-2 text-center text-sm text-gray-600 dark:text-gray-400">
          {{ verificationError || 'The verification link is invalid or has expired.' }}
        </p>
        
        <div class="mt-8 space-y-4">
          <button
            @click="resendVerification"
            :disabled="isResending"
            class="w-full flex justify-center py-3 px-4 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-blue-600 hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500 disabled:opacity-50 transition-colors"
          >
            <svg v-if="isResending" class="animate-spin -ml-1 mr-3 h-5 w-5 text-white" fill="none" viewBox="0 0 24 24">
              <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
              <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
            </svg>
            {{ isResending ? 'Sending...' : 'Resend Verification Email' }}
          </button>
          
          <button
            @click="goToLogin"
            class="w-full flex justify-center py-2 px-4 border border-gray-300 dark:border-gray-600 rounded-md shadow-sm text-sm font-medium text-gray-700 dark:text-gray-300 bg-white dark:bg-gray-700 hover:bg-gray-50 dark:hover:bg-gray-600 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500 transition-colors"
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
