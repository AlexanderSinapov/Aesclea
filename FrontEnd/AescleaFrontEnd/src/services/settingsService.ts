// Copyright (c) 2025 Alexander Sinapov | Simeon Petkov

// All rights reserved.
// This code is proprietary and confidential.  
// Unauthorized copying, modification, distribution, or use is strictly prohibited.

import api from './api'

export interface UpdateProfileRequest {
  firstName?: string
  lastName?: string
  phone?: string
  department?: string
  role?: string
}

export interface ChangePasswordRequest {
  currentPassword: string
  newPassword: string
}

export interface UpdatePreferencesRequest {
  theme?: string
  language?: string
  timezone?: string
}

export interface UpdateNotificationSettingsRequest {
  notifyAppointments?: boolean
  notifyPatientUpdates?: boolean
  notifyAnalysisResults?: boolean
  notifyBilling?: boolean
  notifySystem?: boolean
  notifyAppointmentsPush?: boolean
  notifyPatientUpdatesPush?: boolean
  notifyAnalysisResultsPush?: boolean
  notifyBillingPush?: boolean
  notifySystemPush?: boolean
}

export interface UpdateAvatarRequest {
  avatar: string // Base64 encoded image
}

export interface SettingsResponse {
  success: boolean
  message: string
  user?: any
}

class SettingsService {
  async updateProfile(data: UpdateProfileRequest): Promise<SettingsResponse> {
    try {
      const response = await api.put('/auth/profile', data)
      
      if (response.data.success) {
        console.log('Profile updated successfully:', response.data)
        return response.data
      }
      
      throw new Error(response.data.message || 'Failed to update profile')
    } catch (error: any) {
      console.error('Settings service - Update profile error:', error)
      
      const errorMessage = error.response?.data?.message || error.message || 'Failed to update profile'
      
      return {
        success: false,
        message: errorMessage
      }
    }
  }

  async changePassword(data: ChangePasswordRequest): Promise<SettingsResponse> {
    try {
      const response = await api.post('/auth/change-password', data)
      
      if (response.data.success) {
        console.log('Password changed successfully')
        return response.data
      }
      
      throw new Error(response.data.message || 'Failed to change password')
    } catch (error: any) {
      console.error('Settings service - Change password error:', error)
      
      const errorMessage = error.response?.data?.message || error.message || 'Failed to change password'
      
      return {
        success: false,
        message: errorMessage
      }
    }
  }

  async updatePreferences(data: UpdatePreferencesRequest): Promise<SettingsResponse> {
    try {
      const response = await api.put('/auth/preferences', data)
      
      if (response.data.success) {
        console.log('Preferences updated successfully:', response.data)
        return response.data
      }
      
      throw new Error(response.data.message || 'Failed to update preferences')
    } catch (error: any) {
      console.error('Settings service - Update preferences error:', error)
      
      const errorMessage = error.response?.data?.message || error.message || 'Failed to update preferences'
      
      return {
        success: false,
        message: errorMessage
      }
    }
  }

  async updateNotificationSettings(data: UpdateNotificationSettingsRequest): Promise<SettingsResponse> {
    try {
      const response = await api.put('/auth/notifications', data)
      
      if (response.data.success) {
        console.log('Notification settings updated successfully:', response.data)
        return response.data
      }
      
      throw new Error(response.data.message || 'Failed to update notification settings')
    } catch (error: any) {
      console.error('Settings service - Update notification settings error:', error)
      
      const errorMessage = error.response?.data?.message || error.message || 'Failed to update notification settings'
      
      return {
        success: false,
        message: errorMessage
      }
    }
  }

  async updateAvatar(data: UpdateAvatarRequest): Promise<SettingsResponse> {
    try {
      const response = await api.post('/auth/avatar', data)
      
      if (response.data.success) {
        console.log('Avatar updated successfully:', response.data)
        return response.data
      }
      
      throw new Error(response.data.message || 'Failed to update avatar')
    } catch (error: any) {
      console.error('Settings service - Update avatar error:', error)
      
      const errorMessage = error.response?.data?.message || error.message || 'Failed to update avatar'
      
      return {
        success: false,
        message: errorMessage
      }
    }
  }

  async toggleTwoFactor(): Promise<SettingsResponse> {
    try {
      const response = await api.post('/auth/toggle-2fa')
      
      if (response.data.success) {
        console.log('Two-factor authentication toggled successfully:', response.data)
        return response.data
      }
      
      throw new Error(response.data.message || 'Failed to toggle two-factor authentication')
    } catch (error: any) {
      console.error('Settings service - Toggle two-factor error:', error)
      
      const errorMessage = error.response?.data?.message || error.message || 'Failed to toggle two-factor authentication'
      
      return {
        success: false,
        message: errorMessage
      }
    }
  }

  // Helper method to convert file to base64
  fileToBase64(file: File): Promise<string> {
    return new Promise((resolve, reject) => {
      const reader = new FileReader()
      
      reader.onload = () => {
        const result = reader.result as string
        resolve(result)
      }
      
      reader.onerror = () => {
        reject(new Error('Failed to read file'))
      }
      
      reader.readAsDataURL(file)
    })
  }
}

export default new SettingsService()