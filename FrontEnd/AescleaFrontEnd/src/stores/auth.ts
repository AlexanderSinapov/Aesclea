// Copyright (c) 2025 Alexander Sinapov | Simeon Petkov

// All rights reserved.
// This code is proprietary and confidential.  
// Unauthorized copying, modification, distribution, or use is strictly prohibited.

import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import authService, { type User, type LoginRequest, type RegisterRequest } from '../services/authService'
import settingsService, { 
  type UpdateProfileRequest, 
  type ChangePasswordRequest, 
  type UpdatePreferencesRequest,
  type UpdateNotificationSettingsRequest,
  type UpdateAvatarRequest 
} from '../services/settingsService'

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

  const updateProfile = async (data: UpdateProfileRequest) => {
    isLoading.value = true
    error.value = null

    try {
      const response = await settingsService.updateProfile(data)
      
      if (response.success && response.user) {
        // Update the user in the store with the new data
        user.value = { ...user.value, ...response.user }
        return response
      } else {
        throw new Error(response.message || 'Failed to update profile')
      }
    } catch (err: any) {
      error.value = err.message
      throw err
    } finally {
      isLoading.value = false
    }
  }

  const changePassword = async (data: ChangePasswordRequest) => {
    isLoading.value = true
    error.value = null

    try {
      const response = await settingsService.changePassword(data)
      
      if (response.success) {
        return response
      } else {
        throw new Error(response.message || 'Failed to change password')
      }
    } catch (err: any) {
      error.value = err.message
      throw err
    } finally {
      isLoading.value = false
    }
  }

  const updatePreferences = async (data: UpdatePreferencesRequest) => {
    isLoading.value = true
    error.value = null

    try {
      const response = await settingsService.updatePreferences(data)
      
      if (response.success && response.user) {
        // Update the user in the store with the new preferences
        user.value = { ...user.value, ...response.user }
        return response
      } else {
        throw new Error(response.message || 'Failed to update preferences')
      }
    } catch (err: any) {
      error.value = err.message
      throw err
    } finally {
      isLoading.value = false
    }
  }

  const updateNotificationSettings = async (data: UpdateNotificationSettingsRequest) => {
    isLoading.value = true
    error.value = null

    try {
      const response = await settingsService.updateNotificationSettings(data)
      
      if (response.success && response.user) {
        // Update the user in the store with the new notification settings
        user.value = { ...user.value, ...response.user }
        return response
      } else {
        throw new Error(response.message || 'Failed to update notification settings')
      }
    } catch (err: any) {
      error.value = err.message
      throw err
    } finally {
      isLoading.value = false
    }
  }

  const updateAvatar = async (data: UpdateAvatarRequest) => {
    isLoading.value = true
    error.value = null

    try {
      const response = await settingsService.updateAvatar(data)
      
      if (response.success && response.user) {
        // Update the user in the store with the new avatar
        user.value = { ...user.value, ...response.user }
        return response
      } else {
        throw new Error(response.message || 'Failed to update avatar')
      }
    } catch (err: any) {
      error.value = err.message
      throw err
    } finally {
      isLoading.value = false
    }
  }

  const toggleTwoFactor = async () => {
    isLoading.value = true
    error.value = null

    try {
      const response = await settingsService.toggleTwoFactor()
      
      if (response.success && response.user) {
        // Update the user in the store with the new 2FA status
        user.value = { ...user.value, ...response.user }
        return response
      } else {
        throw new Error(response.message || 'Failed to toggle two-factor authentication')
      }
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
    verifyEmail,
    updateProfile,
    changePassword,
    updatePreferences,
    updateNotificationSettings,
    updateAvatar,
    toggleTwoFactor
  }
})