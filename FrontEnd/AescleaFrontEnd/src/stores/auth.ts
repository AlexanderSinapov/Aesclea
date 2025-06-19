import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import authService, { type User, type LoginRequest, type RegisterRequest } from '../services/authService'

export const useAuthStore = defineStore('auth', () => {
  const user = ref<User | null>(null)
  const isLoading = ref(false)
  const error = ref<string | null>(null)

  const isAuthenticated = computed(() => {
    return !!user.value && authService.isAuthenticated()
  })

  const initializeAuth = () => {
    const storedUser = authService.getUser()
    if (storedUser && authService.isAuthenticated()) {
      user.value = storedUser
    }
  }

  const login = async (credentials: LoginRequest) => {
    isLoading.value = true
    error.value = null

    try {
      const response = await authService.login(credentials)
      
      if (response.success && response.user) {
        user.value = response.user
        return response
      } else {
        throw new Error(response.message || 'Login failed')
      }
    } catch (err: any) {
      error.value = err.message
      throw err
    } finally {
      isLoading.value = false
    }
  }

  const register = async (userData: RegisterRequest) => {
    isLoading.value = true
    error.value = null

    try {
      const response = await authService.register(userData)
      
      if (response.success && response.user) {
        user.value = response.user
        return response
      } else {
        throw new Error(response.message || 'Registration failed')
      }
    } catch (err: any) {
      error.value = err.message
      throw err
    } finally {
      isLoading.value = false
    }
  }

  const logout = async () => {
    isLoading.value = true
    
    try {
      await authService.logout()
      user.value = null
      error.value = null
    } catch (err: any) {
      console.error('Logout error:', err)
    } finally {
      isLoading.value = false
    }
  }

  const clearError = () => {
    error.value = null
  }

  return {
    user,
    isLoading,
    error,
    isAuthenticated,
    initializeAuth,
    login,
    register,
    logout,
    clearError
  }
})