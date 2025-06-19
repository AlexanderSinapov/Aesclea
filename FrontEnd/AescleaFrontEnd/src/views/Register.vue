<template>
  <div class="min-h-screen bg-gradient-to-br from-green-50 to-teal-100 flex items-center justify-center py-12 px-4 sm:px-6 lg:px-8">
    <div class="max-w-md w-full space-y-8 bg-white p-8 rounded-xl shadow-lg">
      <div>
        <div class="mx-auto h-12 w-12 bg-green-600 rounded-full flex items-center justify-center">
          <svg class="h-8 w-8 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4.354a4 4 0 110 5.292M15 21H3v-1a6 6 0 0112 0v1zm0 0h6v-1a6 6 0 00-9-5.197m13.5-9a2.5 2.5 0 11-5 0 2.5 2.5 0 015 0z" />
          </svg>
        </div>
        <h2 class="mt-6 text-center text-3xl font-extrabold text-gray-900">
          Create your account
        </h2>
        <p class="mt-2 text-center text-sm text-gray-600">
          Join Aesclea Medical Management System
        </p>
      </div>

      <!-- Error message -->
      <div v-if="error" class="bg-red-50 border border-red-200 rounded-md p-4">
        <div class="flex">
          <svg class="h-5 w-5 text-red-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
          </svg>
          <div class="ml-3">
            <p class="text-sm text-red-600">{{ error }}</p>
          </div>
        </div>
      </div>

      <!-- Success message -->
      <div v-if="registrationSuccess" class="bg-green-50 border border-green-200 rounded-md p-4">
        <div class="flex">
          <svg class="h-5 w-5 text-green-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z" />
          </svg>
          <div class="ml-3">
            <p class="text-sm text-green-600">Registration successful! Redirecting to dashboard...</p>
          </div>
        </div>
      </div>

      <form class="mt-8 space-y-6" @submit.prevent="handleRegister">
        <div class="space-y-4">
          <div class="grid grid-cols-2 gap-4">
            <div>
              <label for="firstName" class="block text-sm font-medium text-gray-700">First Name</label>
              <input
                id="firstName"
                name="firstName"
                type="text"
                required
                v-model="registerForm.firstName"
                :disabled="isLoading"
                class="mt-1 appearance-none relative block w-full px-3 py-2 border border-gray-300 placeholder-gray-500 text-gray-900 rounded-md focus:outline-none focus:ring-green-500 focus:border-green-500 focus:z-10 sm:text-sm disabled:opacity-50"
                placeholder="First name"
              />
            </div>
            <div>
              <label for="lastName" class="block text-sm font-medium text-gray-700">Last Name</label>
              <input
                id="lastName"
                name="lastName"
                type="text"
                required
                v-model="registerForm.lastName"
                :disabled="isLoading"
                class="mt-1 appearance-none relative block w-full px-3 py-2 border border-gray-300 placeholder-gray-500 text-gray-900 rounded-md focus:outline-none focus:ring-green-500 focus:border-green-500 focus:z-10 sm:text-sm disabled:opacity-50"
                placeholder="Last name"
              />
            </div>
          </div>
          
          <div>
            <label for="email" class="block text-sm font-medium text-gray-700">Email address</label>
            <input
              id="email"
              name="email"
              type="email"
              autocomplete="email"
              required
              v-model="registerForm.email"
              :disabled="isLoading"
              class="mt-1 appearance-none relative block w-full px-3 py-2 border border-gray-300 placeholder-gray-500 text-gray-900 rounded-md focus:outline-none focus:ring-green-500 focus:border-green-500 focus:z-10 sm:text-sm disabled:opacity-50"
              placeholder="Enter your email"
            />
          </div>

          <div>
            <label for="phone" class="block text-sm font-medium text-gray-700">Phone Number</label>
            <input
              id="phone"
              name="phone"
              type="tel"
              required
              v-model="registerForm.phone"
              :disabled="isLoading"
              class="mt-1 appearance-none relative block w-full px-3 py-2 border border-gray-300 placeholder-gray-500 text-gray-900 rounded-md focus:outline-none focus:ring-green-500 focus:border-green-500 focus:z-10 sm:text-sm disabled:opacity-50"
              placeholder="Enter your phone number"
            />
          </div>

          <div>
            <label for="role" class="block text-sm font-medium text-gray-700">Role</label>
            <select
              id="role"
              name="role"
              required
              v-model="registerForm.role"
              :disabled="isLoading"
              class="mt-1 block w-full px-3 py-2 border border-gray-300 bg-white rounded-md shadow-sm focus:outline-none focus:ring-green-500 focus:border-green-500 sm:text-sm disabled:opacity-50"
            >
              <option value="">Select your role</option>
              <option value="doctor">Doctor</option>
              <option value="nurse">Nurse</option>
              <option value="admin">Administrator</option>
              <option value="patient">Patient</option>
            </select>
          </div>

          <div>
            <label for="hospital" class="block text-sm font-medium text-gray-700">Hospital</label>
            <input
              type="text"
              id="hospital"
              name="hospital"
              required
              v-model="registerForm.hospital"
              :disabled="isLoading"
              class="mt-1 appearance-none relative block w-full px-3 py-2 border border-gray-300 placeholder-gray-500 text-gray-900 rounded-md focus:outline-none focus:ring-green-500 focus:border-green-500 focus:z-10 sm:text-sm disabled:opacity-50"
              placeholder="Enter your hospital name"
            />
          </div>
          
          <div>
            <label for="password" class="block text-sm font-medium text-gray-700">Password</label>
            <input
              id="password"
              name="password"
              type="password"
              autocomplete="new-password"
              required
              v-model="registerForm.password"
              :disabled="isLoading"
              class="mt-1 appearance-none relative block w-full px-3 py-2 border border-gray-300 placeholder-gray-500 text-gray-900 rounded-md focus:outline-none focus:ring-green-500 focus:border-green-500 focus:z-10 sm:text-sm disabled:opacity-50"
              placeholder="Enter your password"
            />
          </div>
          
          <div>
            <label for="confirmPassword" class="block text-sm font-medium text-gray-700">Confirm Password</label>
            <input
              id="confirmPassword"
              name="confirmPassword"
              type="password"
              autocomplete="new-password"
              required
              v-model="registerForm.confirmPassword"
              :disabled="isLoading"
              class="mt-1 appearance-none relative block w-full px-3 py-2 border border-gray-300 placeholder-gray-500 text-gray-900 rounded-md focus:outline-none focus:ring-green-500 focus:border-green-500 focus:z-10 sm:text-sm disabled:opacity-50"
              placeholder="Confirm your password"
            />
          </div>
        </div>

        <div class="flex items-center">
          <input
            id="agree-terms"
            name="agree-terms"
            type="checkbox"
            required
            v-model="agreeToTerms"
            :disabled="isLoading"
            class="h-4 w-4 text-green-600 focus:ring-green-500 border-gray-300 rounded disabled:opacity-50"
          />
          <label for="agree-terms" class="ml-2 block text-sm text-gray-900">
            I agree to the <a href="#" class="text-green-600 hover:text-green-500">Terms and Conditions</a> and <a href="#" class="text-green-600 hover:text-green-500">Privacy Policy</a>
          </label>
        </div>

        <div>
          <button
            type="submit"
            :disabled="isLoading || !agreeToTerms"
            class="group relative w-full flex justify-center py-2 px-4 border border-transparent text-sm font-medium rounded-md text-white bg-green-600 hover:bg-green-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-green-500 transition duration-150 ease-in-out disabled:opacity-50 disabled:cursor-not-allowed"
          >
            <span class="absolute left-0 inset-y-0 flex items-center pl-3">
              <svg v-if="!isLoading" class="h-5 w-5 text-green-500 group-hover:text-green-400" fill="currentColor" viewBox="0 0 20 20">
                <path fill-rule="evenodd" d="M10 1a4.5 4.5 0 00-4.5 4.5V9H5a2 2 0 00-2 2v6a2 2 0 002 2h10a2 2 0 002-2v-6a2 2 0 00-2-2h-.5V5.5A4.5 4.5 0 0010 1zm3 8V5.5a3 3 0 10-6 0V9h6z" clip-rule="evenodd" />
              </svg>
              <svg v-else class="animate-spin h-5 w-5 text-green-500" fill="none" viewBox="0 0 24 24">
                <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
                <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
              </svg>
            </span>
            {{ isLoading ? 'Creating Account...' : 'Create Account' }}
          </button>
        </div>

        <div class="text-center">
          <span class="text-sm text-gray-600">
            Already have an account?
            <router-link to="/login" class="font-medium text-green-600 hover:text-green-500">
              Sign in
            </router-link>
          </span>
        </div>
      </form>
    </div>
  </div>
</template>

<script setup lang="ts">
import { reactive, ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '../stores/auth'
import { testApiConnection } from '../services/api'

const router = useRouter()
const authStore = useAuthStore()
const isLoading = ref(false)
const error = ref<string | null>(null)

const registerForm = reactive({
  firstName: '',
  lastName: '',
  email: '',
  phone: '',
  role: '',
  password: '',
  confirmPassword: '',
  hospital: '',
})

const agreeToTerms = ref(false)
const registrationSuccess = ref(false)

const handleRegister = async () => {
  console.log('Register: Form submitted')
  
  // Clear previous errors
  error.value = null
  authStore.clearError()
  
  // Client-side validation
  if (registerForm.password !== registerForm.confirmPassword) {
    error.value = 'Passwords do not match!'
    return
  }

  if (registerForm.password.length < 6) {
    error.value = 'Password must be at least 6 characters long!'
    return
  }

  isLoading.value = true

  try {
    console.log('Register: Starting registration process')
    const { confirmPassword, ...registrationData } = registerForm
    
    await authStore.register(registrationData)
    
    registrationSuccess.value = true
    console.log('Register: Registration successful!')
    
    // Redirect to dashboard after a short delay
    setTimeout(() => {
      router.push('/dashboard')
    }, 2000)
  } catch (err: any) {
    console.error('Register: Registration failed:', err)
    error.value = err.message || 'Registration failed. Please try again.'
  } finally {
    isLoading.value = false
  }
}

onMounted(async () => {
  console.log('Register: Component mounted')
  
  // Test API connection
  const isApiConnected = await testApiConnection()
  if (!isApiConnected) {
    error.value = 'Cannot connect to server. Please ensure the backend is running.'
    return
  }
  
  // Check authentication status when component mounts
  try {
    authStore.initializeAuth()
    
    if (authStore.isAuthenticated) {
      console.log('Register: User already authenticated, redirecting to dashboard')
      router.push('/dashboard')
    }
  } catch (err) {
    console.error('Auth check failed:', err)
  }
})
</script>