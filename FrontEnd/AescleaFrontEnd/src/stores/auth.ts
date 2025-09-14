// Copyright (c) 2025 Alexander Sinapov | Simeon Petkov

// All rights reserved.
// This code is proprietary and confidential.  
// Unauthorized copying, modification, distribution, or use is strictly prohibited.

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

  // Initialize auth when store is created
  initializeAuth()

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

  const clearAllData = () => {
    // Clear all localStorage data
    localStorage.clear()
    
    // Reset store state
    user.value = null
    error.value = null
    isLoading.value = false
    
    console.log('All localStorage data cleared!')
  }

  const sendVerificationEmail = async (email: string) => {
    isLoading.value = true
    error.value = null

    try {
      const response = await authService.sendVerificationEmail(email)
      return response
    } catch (err: any) {
      error.value = err.message
      throw err
    } finally {
      isLoading.value = false
    }
  }

  const verifyEmail = async (token: string) => {
    isLoading.value = true
    error.value = null

    try {
      const response = await authService.verifyEmail(token)
      
      // Update user verification status in store
      if (response.success && user.value) {
        user.value.isEmailVerified = true
      }
      
      return response
    } catch (err: any) {
      error.value = err.message
      throw err
    } finally {
      isLoading.value = false
    }
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
    clearError,
    clearAllData,
    sendVerificationEmail,
    verifyEmail
  }
})