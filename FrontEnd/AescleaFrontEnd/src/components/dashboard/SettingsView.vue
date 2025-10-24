<!-- Copyright (c) 2025 Alexander Sinapov | Simeon Petkov -->

<!-- All rights reserved. -->
<!-- This code is proprietary and confidential.   -->
<!-- Unauthorized copying, modification, distribution, or use is strictly prohibited. -->

<template>
  <div class="space-y-6">
    <!-- Page Header -->
    <div>
      <h1 class="text-2xl font-bold text-gray-900 dark:text-white">Settings</h1>
      <p class="mt-1 text-sm text-gray-500 dark:text-gray-400">
        Manage your account settings, preferences, and system configuration
      </p>
    </div>

    <!-- Settings Navigation Tabs -->
    <div class="border-b border-gray-200 dark:border-gray-700">
      <nav class="flex -mb-px space-x-8">
        <button
          v-for="tab in settingsTabs"
          :key="tab.id"
          @click="activeTab = tab.id"
          :class="activeTab === tab.id 
            ? 'border-purple-500 text-purple-600 dark:text-purple-400' 
            : 'border-transparent text-gray-500 dark:text-gray-400 hover:text-gray-700 dark:hover:text-gray-300 hover:border-gray-300'"
          class="px-1 py-2 text-sm font-medium border-b-2 whitespace-nowrap"
        >
          {{ tab.name }}
        </button>
      </nav>
    </div>

    <!-- Profile Settings -->
    <div v-if="activeTab === 'profile'" class="space-y-6">
      <div class="bg-white border border-gray-200 rounded-lg shadow-sm dark:bg-gray-800 dark:border-gray-700">
        <div class="px-6 py-4 border-b border-gray-200 dark:border-gray-700">
          <h3 class="text-lg font-medium text-gray-900 dark:text-white">Profile Information</h3>
          <p class="text-sm text-gray-500 dark:text-gray-400">Update your personal information and profile settings.</p>
        </div>
        
        <form @submit.prevent="saveProfile" class="px-6 py-6 space-y-6">
          <!-- Profile Photo -->
          <div class="flex items-center space-x-6">
            <div class="shrink-0">
              <img class="object-cover w-16 h-16 rounded-full" :src="profileForm.avatar || '/api/placeholder/64/64'" alt="Profile photo" />
            </div>
            <div>
              <label for="photo" class="block text-sm font-medium text-gray-700 dark:text-gray-300">Profile Photo</label>
              <div class="flex items-center mt-1 space-x-4">
                <input
                  type="file"
                  id="photo"
                  accept="image/*"
                  @change="handlePhotoChange"
                  class="hidden"
                  ref="photoInput"
                />
                <button
                  type="button"
                  @click="photoInput?.click()"
                  class="px-3 py-2 text-sm font-medium leading-4 text-gray-700 border border-gray-300 rounded-md dark:border-gray-600 dark:text-gray-300 hover:bg-gray-50 dark:hover:bg-gray-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-purple-500"
                >
                  Change Photo
                </button>
              </div>
            </div>
          </div>

          <!-- Personal Information -->
          <div class="grid grid-cols-1 gap-6 md:grid-cols-2">
            <div>
              <label for="firstName" class="block text-sm font-medium text-gray-700 dark:text-gray-300">First Name</label>
              <input
                id="firstName"
                v-model="profileForm.firstName"
                type="text"
                required
                class="block w-full px-3 py-2 mt-1 text-gray-900 bg-white border border-gray-300 rounded-md shadow-sm dark:border-gray-600 dark:bg-gray-700 dark:text-white focus:outline-none focus:ring-1 focus:ring-purple-500 focus:border-purple-500"
              />
            </div>

            <div>
              <label for="lastName" class="block text-sm font-medium text-gray-700 dark:text-gray-300">Last Name</label>
              <input
                id="lastName"
                v-model="profileForm.lastName"
                type="text"
                required
                class="block w-full px-3 py-2 mt-1 text-gray-900 bg-white border border-gray-300 rounded-md shadow-sm dark:border-gray-600 dark:bg-gray-700 dark:text-white focus:outline-none focus:ring-1 focus:ring-purple-500 focus:border-purple-500"
              />
            </div>

            <div>
              <label for="email" class="block text-sm font-medium text-gray-700 dark:text-gray-300">Email</label>
              <input
                id="email"
                v-model="profileForm.email"
                type="email"
                required
                class="block w-full px-3 py-2 mt-1 text-gray-900 bg-white border border-gray-300 rounded-md shadow-sm dark:border-gray-600 dark:bg-gray-700 dark:text-white focus:outline-none focus:ring-1 focus:ring-purple-500 focus:border-purple-500"
              />
            </div>

            <div>
              <label for="phone" class="block text-sm font-medium text-gray-700 dark:text-gray-300">Phone</label>
              <input
                id="phone"
                v-model="profileForm.phone"
                type="tel"
                class="block w-full px-3 py-2 mt-1 text-gray-900 bg-white border border-gray-300 rounded-md shadow-sm dark:border-gray-600 dark:bg-gray-700 dark:text-white focus:outline-none focus:ring-1 focus:ring-purple-500 focus:border-purple-500"
              />
            </div>

            <div>
              <label for="department" class="block text-sm font-medium text-gray-700 dark:text-gray-300">Department</label>
              <select
                id="department"
                v-model="profileForm.department"
                class="block w-full px-3 py-2 mt-1 text-gray-900 bg-white border border-gray-300 rounded-md shadow-sm dark:border-gray-600 dark:bg-gray-700 dark:text-white focus:outline-none focus:ring-1 focus:ring-purple-500 focus:border-purple-500"
              >
                <option value="">Select Department</option>
                <option value="administration">Administration</option>
                <option value="cardiology">Cardiology</option>
                <option value="neurology">Neurology</option>
                <option value="oncology">Oncology</option>
                <option value="radiology">Radiology</option>
                <option value="emergency">Emergency</option>
                <option value="pediatrics">Pediatrics</option>
              </select>
            </div>

            <div>
              <label for="role" class="block text-sm font-medium text-gray-700 dark:text-gray-300">Role</label>
              <select
                id="role"
                v-model="profileForm.role"
                class="block w-full px-3 py-2 mt-1 text-gray-900 bg-white border border-gray-300 rounded-md shadow-sm dark:border-gray-600 dark:bg-gray-700 dark:text-white focus:outline-none focus:ring-1 focus:ring-purple-500 focus:border-purple-500"
              >
                <option value="doctor">Doctor</option>
                <option value="nurse">Nurse</option>
                <option value="admin">Administrator</option>
                <option value="technician">Technician</option>
                <option value="manager">Manager</option>
              </select>
            </div>
          </div>

          <div class="flex justify-end">
            <button
              type="submit"
              class="inline-flex items-center px-4 py-2 text-sm font-medium text-white bg-purple-600 border border-transparent rounded-md shadow-sm hover:bg-purple-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-purple-500"
            >
              Save Changes
            </button>
          </div>
        </form>
      </div>
    </div>

    <!-- Security Settings -->
    <div v-else-if="activeTab === 'security'" class="space-y-6">
      <!-- Change Password -->
      <div class="bg-white border border-gray-200 rounded-lg shadow-sm dark:bg-gray-800 dark:border-gray-700">
        <div class="px-6 py-4 border-b border-gray-200 dark:border-gray-700">
          <h3 class="text-lg font-medium text-gray-900 dark:text-white">Change Password</h3>
          <p class="text-sm text-gray-500 dark:text-gray-400">Update your account password.</p>
        </div>
        
        <form @submit.prevent="changePassword" class="px-6 py-6 space-y-6">
          <div>
            <label for="currentPassword" class="block text-sm font-medium text-gray-700 dark:text-gray-300">Current Password</label>
            <input
              id="currentPassword"
              v-model="passwordForm.currentPassword"
              type="password"
              required
              class="block w-full px-3 py-2 mt-1 text-gray-900 bg-white border border-gray-300 rounded-md shadow-sm dark:border-gray-600 dark:bg-gray-700 dark:text-white focus:outline-none focus:ring-1 focus:ring-purple-500 focus:border-purple-500"
            />
          </div>

          <div>
            <label for="newPassword" class="block text-sm font-medium text-gray-700 dark:text-gray-300">New Password</label>
            <input
              id="newPassword"
              v-model="passwordForm.newPassword"
              type="password"
              required
              minlength="8"
              class="block w-full px-3 py-2 mt-1 text-gray-900 bg-white border border-gray-300 rounded-md shadow-sm dark:border-gray-600 dark:bg-gray-700 dark:text-white focus:outline-none focus:ring-1 focus:ring-purple-500 focus:border-purple-500"
            />
          </div>

          <div>
            <label for="confirmPassword" class="block text-sm font-medium text-gray-700 dark:text-gray-300">Confirm New Password</label>
            <input
              id="confirmPassword"
              v-model="passwordForm.confirmPassword"
              type="password"
              required
              class="block w-full px-3 py-2 mt-1 text-gray-900 bg-white border border-gray-300 rounded-md shadow-sm dark:border-gray-600 dark:bg-gray-700 dark:text-white focus:outline-none focus:ring-1 focus:ring-purple-500 focus:border-purple-500"
            />
          </div>

          <div class="flex justify-end">
            <button
              type="submit"
              :disabled="!isPasswordFormValid"
              class="inline-flex items-center px-4 py-2 text-sm font-medium text-white bg-purple-600 border border-transparent rounded-md shadow-sm hover:bg-purple-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-purple-500 disabled:opacity-50 disabled:cursor-not-allowed"
            >
              Update Password
            </button>
          </div>
        </form>
      </div>

      <!-- Two-Factor Authentication -->
      <div class="bg-white border border-gray-200 rounded-lg shadow-sm dark:bg-gray-800 dark:border-gray-700">
        <div class="px-6 py-4 border-b border-gray-200 dark:border-gray-700">
          <h3 class="text-lg font-medium text-gray-900 dark:text-white">Two-Factor Authentication</h3>
          <p class="text-sm text-gray-500 dark:text-gray-400">Add an extra layer of security to your account.</p>
        </div>
        
        <div class="px-6 py-6">
          <div class="flex items-center justify-between">
            <div>
              <div class="text-sm font-medium text-gray-900 dark:text-white">
                Two-Factor Authentication
              </div>
              <div class="text-sm text-gray-500 dark:text-gray-400">
                {{ securitySettings.twoFactorEnabled ? 'Enabled' : 'Disabled' }}
              </div>
            </div>
            <button
              @click="toggleTwoFactor"
              :class="securitySettings.twoFactorEnabled 
                ? 'bg-purple-600 focus:ring-purple-500' 
                : 'bg-gray-200 focus:ring-gray-500'"
              class="relative inline-flex flex-shrink-0 h-6 transition-colors duration-200 ease-in-out border-2 border-transparent rounded-full cursor-pointer w-11 focus:outline-none focus:ring-2 focus:ring-offset-2"
            >
              <span
                :class="securitySettings.twoFactorEnabled ? 'translate-x-5' : 'translate-x-0'"
                class="inline-block w-5 h-5 transition duration-200 ease-in-out transform bg-white rounded-full shadow pointer-events-none ring-0"
              />
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Preferences -->
    <div v-else-if="activeTab === 'preferences'" class="space-y-6">
      <!-- Theme Settings -->
      <div class="bg-white border border-gray-200 rounded-lg shadow-sm dark:bg-gray-800 dark:border-gray-700">
        <div class="px-6 py-4 border-b border-gray-200 dark:border-gray-700">
          <h3 class="text-lg font-medium text-gray-900 dark:text-white">Appearance</h3>
          <p class="text-sm text-gray-500 dark:text-gray-400">Customize how the application looks and feels.</p>
        </div>
        
        <div class="px-6 py-6 space-y-6">
          <div>
            <label class="block mb-3 text-sm font-medium text-gray-700 dark:text-gray-300">Theme</label>
            <div class="grid grid-cols-3 gap-3">
              <label class="relative flex p-4 bg-white border rounded-lg shadow-sm cursor-pointer focus:outline-none">
                <input type="radio" v-model="preferences.theme" value="light" class="sr-only" />
                <span class="flex flex-1">
                  <span class="flex flex-col">
                    <span class="block text-sm font-medium text-gray-900">Light</span>
                    <span class="flex items-center mt-1 text-sm text-gray-500">Always light theme</span>
                  </span>
                </span>
                <CheckCircleIcon v-if="preferences.theme === 'light'" class="w-5 h-5 text-purple-600" />
              </label>

              <label class="relative flex p-4 text-white bg-gray-900 border rounded-lg shadow-sm cursor-pointer focus:outline-none">
                <input type="radio" v-model="preferences.theme" value="dark" class="sr-only" />
                <span class="flex flex-1">
                  <span class="flex flex-col">
                    <span class="block text-sm font-medium text-white">Dark</span>
                    <span class="flex items-center mt-1 text-sm text-gray-300">Always dark theme</span>
                  </span>
                </span>
                <CheckCircleIcon v-if="preferences.theme === 'dark'" class="w-5 h-5 text-purple-400" />
              </label>

              <label class="relative flex p-4 bg-white border rounded-lg shadow-sm cursor-pointer focus:outline-none">
                <input type="radio" v-model="preferences.theme" value="system" class="sr-only" />
                <span class="flex flex-1">
                  <span class="flex flex-col">
                    <span class="block text-sm font-medium text-gray-900">System</span>
                    <span class="flex items-center mt-1 text-sm text-gray-500">Follow system setting</span>
                  </span>
                </span>
                <CheckCircleIcon v-if="preferences.theme === 'system'" class="w-5 h-5 text-purple-600" />
              </label>
            </div>
          </div>

          <!-- Language -->
          <div>
            <label for="language" class="block text-sm font-medium text-gray-700 dark:text-gray-300">Language</label>
            <select
              id="language"
              v-model="preferences.language"
              class="block w-full px-3 py-2 mt-1 text-gray-900 bg-white border border-gray-300 rounded-md shadow-sm dark:border-gray-600 dark:bg-gray-700 dark:text-white focus:outline-none focus:ring-1 focus:ring-purple-500 focus:border-purple-500"
            >
              <option value="en">English</option>
              <option value="es">Español</option>
              <option value="fr">Français</option>
              <option value="de">Deutsch</option>
              <option value="it">Italiano</option>
            </select>
          </div>

          <!-- Timezone -->
          <div>
            <label for="timezone" class="block text-sm font-medium text-gray-700 dark:text-gray-300">Timezone</label>
            <select
              id="timezone"
              v-model="preferences.timezone"
              class="block w-full px-3 py-2 mt-1 text-gray-900 bg-white border border-gray-300 rounded-md shadow-sm dark:border-gray-600 dark:bg-gray-700 dark:text-white focus:outline-none focus:ring-1 focus:ring-purple-500 focus:border-purple-500"
            >
              <option value="UTC">UTC</option>
              <option value="America/New_York">Eastern Time</option>
              <option value="America/Chicago">Central Time</option>
              <option value="America/Denver">Mountain Time</option>
              <option value="America/Los_Angeles">Pacific Time</option>
              <option value="Europe/London">London</option>
              <option value="Europe/Paris">Paris</option>
              <option value="Asia/Tokyo">Tokyo</option>
            </select>
          </div>

          <div class="flex justify-end">
            <button
              @click="savePreferences"
              class="inline-flex items-center px-4 py-2 text-sm font-medium text-white bg-purple-600 border border-transparent rounded-md shadow-sm hover:bg-purple-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-purple-500"
            >
              Save Preferences
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Notifications -->
    <div v-else-if="activeTab === 'notifications'" class="space-y-6">
      <div class="bg-white border border-gray-200 rounded-lg shadow-sm dark:bg-gray-800 dark:border-gray-700">
        <div class="px-6 py-4 border-b border-gray-200 dark:border-gray-700">
          <h3 class="text-lg font-medium text-gray-900 dark:text-white">Notification Settings</h3>
          <p class="text-sm text-gray-500 dark:text-gray-400">Configure how you receive notifications.</p>
        </div>
        
        <div class="px-6 py-6 space-y-6">
          <div v-for="notification in notificationSettings" :key="notification.id" class="flex items-center justify-between">
            <div>
              <div class="text-sm font-medium text-gray-900 dark:text-white">{{ notification.title }}</div>
              <div class="text-sm text-gray-500 dark:text-gray-400">{{ notification.description }}</div>
            </div>
            <div class="flex space-x-6">
              <label class="flex items-center">
                <input
                  v-model="notification.email"
                  type="checkbox"
                  class="text-purple-600 border-gray-300 rounded focus:ring-purple-500"
                />
                <span class="ml-2 text-sm text-gray-700 dark:text-gray-300">Email</span>
              </label>
              <label class="flex items-center">
                <input
                  v-model="notification.push"
                  type="checkbox"
                  class="text-purple-600 border-gray-300 rounded focus:ring-purple-500"
                />
                <span class="ml-2 text-sm text-gray-700 dark:text-gray-300">Push</span>
              </label>
            </div>
          </div>

          <div class="flex justify-end pt-6 border-t border-gray-200 dark:border-gray-700">
            <button
              @click="saveNotifications"
              class="inline-flex items-center px-4 py-2 text-sm font-medium text-white bg-purple-600 border border-transparent rounded-md shadow-sm hover:bg-purple-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-purple-500"
            >
              Save Notifications
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import { useAuthStore } from '../../stores/auth'

// Store
const authStore = useAuthStore()

// Local state
const activeTab = ref('profile')
const photoInput = ref<HTMLInputElement>()

// Settings tabs
const settingsTabs = [
  { id: 'profile', name: 'Profile' },
  { id: 'security', name: 'Security' },
  { id: 'preferences', name: 'Preferences' },
  { id: 'notifications', name: 'Notifications' }
]

// Form data
const profileForm = ref({
  firstName: authStore.user?.firstName || '',
  lastName: authStore.user?.lastName || '',
  email: authStore.user?.email || '',
  phone: authStore.user?.phone || '',
  department: (authStore.user as any)?.department || '',
  role: authStore.user?.role || 'doctor',
  avatar: (authStore.user as any)?.avatar || ''
})

const passwordForm = ref({
  currentPassword: '',
  newPassword: '',
  confirmPassword: ''
})

const preferences = ref({
  theme: 'system',
  language: 'en',
  timezone: 'UTC'
})

const securitySettings = ref({
  twoFactorEnabled: false
})

const notificationSettings = ref([
  {
    id: 'appointments',
    title: 'Appointment Reminders',
    description: 'Get notified about upcoming appointments',
    email: true,
    push: true
  },
  {
    id: 'patient-updates',
    title: 'Patient Updates',
    description: 'Notifications when patient information changes',
    email: true,
    push: false
  },
  {
    id: 'analysis-results',
    title: 'Analysis Results',
    description: 'Get notified when AI analysis results are ready',
    email: true,
    push: true
  },
  {
    id: 'billing',
    title: 'Billing Notifications',
    description: 'Updates about payments and invoices',
    email: false,
    push: false
  },
  {
    id: 'system',
    title: 'System Updates',
    description: 'Maintenance and system update notifications',
    email: true,
    push: false
  }
])

// Computed properties
const isPasswordFormValid = computed(() => {
  return passwordForm.value.currentPassword &&
         passwordForm.value.newPassword &&
         passwordForm.value.confirmPassword &&
         passwordForm.value.newPassword === passwordForm.value.confirmPassword &&
         passwordForm.value.newPassword.length >= 8
})

// Icon component
const CheckCircleIcon = {
  template: `<svg fill="currentColor" viewBox="0 0 20 20"><path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z" clip-rule="evenodd" /></svg>`
}

// Methods
const handlePhotoChange = async (event: Event) => {
  const file = (event.target as HTMLInputElement).files?.[0]
  if (file) {
    try {
      const reader = new FileReader()
      reader.onload = async (e) => {
        const base64Avatar = e.target?.result as string
        profileForm.value.avatar = base64Avatar
        
        // Automatically upload the new avatar
        try {
          await authStore.updateAvatar({ avatar: base64Avatar })
          alert('Profile photo updated successfully!')
        } catch (error) {
          console.error('Error updating avatar:', error)
          alert('Failed to update profile photo. Please try again.')
        }
      }
      reader.readAsDataURL(file)
    } catch (error) {
      console.error('Error reading file:', error)
      alert('Failed to read the selected file.')
    }
  }
}

const saveProfile = async () => {
  try {
    await authStore.updateProfile({
      firstName: profileForm.value.firstName,
      lastName: profileForm.value.lastName,
      phone: profileForm.value.phone,
      department: profileForm.value.department,
      role: profileForm.value.role
    })
    
    alert('Profile updated successfully!')
  } catch (error) {
    console.error('Error saving profile:', error)
    alert('Failed to update profile. Please try again.')
  }
}

const changePassword = async () => {
  try {
    if (!isPasswordFormValid.value) {
      alert('Please check your password inputs.')
      return
    }

    await authStore.changePassword({
      currentPassword: passwordForm.value.currentPassword,
      newPassword: passwordForm.value.newPassword
    })
    
    // Reset form
    passwordForm.value = {
      currentPassword: '',
      newPassword: '',
      confirmPassword: ''
    }
    
    alert('Password updated successfully!')
  } catch (error) {
    console.error('Error changing password:', error)
    alert('Failed to update password. Please try again.')
  }
}

const toggleTwoFactor = async () => {
  try {
    const response = await authStore.toggleTwoFactor()
    securitySettings.value.twoFactorEnabled = !securitySettings.value.twoFactorEnabled
    alert(response.message || `Two-factor authentication ${securitySettings.value.twoFactorEnabled ? 'enabled' : 'disabled'}`)
  } catch (error) {
    console.error('Error toggling two-factor authentication:', error)
    alert('Failed to toggle two-factor authentication. Please try again.')
  }
}

const savePreferences = async () => {
  try {
    await authStore.updatePreferences({
      theme: preferences.value.theme,
      language: preferences.value.language,
      timezone: preferences.value.timezone
    })
    
    // Also save to localStorage for immediate UI updates
    localStorage.setItem('preferences', JSON.stringify(preferences.value))
    alert('Preferences saved successfully!')
  } catch (error) {
    console.error('Error saving preferences:', error)
    alert('Failed to save preferences. Please try again.')
  }
}

const saveNotifications = async () => {
  try {
    // Convert notification settings array to the format expected by the API
    const notificationData = {
      notifyAppointments: notificationSettings.value.find(n => n.id === 'appointments')?.email || false,
      notifyPatientUpdates: notificationSettings.value.find(n => n.id === 'patient-updates')?.email || false,
      notifyAnalysisResults: notificationSettings.value.find(n => n.id === 'analysis-results')?.email || false,
      notifyBilling: notificationSettings.value.find(n => n.id === 'billing')?.email || false,
      notifySystem: notificationSettings.value.find(n => n.id === 'system')?.email || false,
      notifyAppointmentsPush: notificationSettings.value.find(n => n.id === 'appointments')?.push || false,
      notifyPatientUpdatesPush: notificationSettings.value.find(n => n.id === 'patient-updates')?.push || false,
      notifyAnalysisResultsPush: notificationSettings.value.find(n => n.id === 'analysis-results')?.push || false,
      notifyBillingPush: notificationSettings.value.find(n => n.id === 'billing')?.push || false,
      notifySystemPush: notificationSettings.value.find(n => n.id === 'system')?.push || false
    }
    
    await authStore.updateNotificationSettings(notificationData)
    alert('Notification settings saved successfully!')
  } catch (error) {
    console.error('Error saving notification settings:', error)
    alert('Failed to save notification settings. Please try again.')
  }
}

// Load user data and preferences on mount
const loadUserData = () => {
  if (authStore.user) {
    const user = authStore.user as any
    
    // Update profile form
    profileForm.value = {
      firstName: user.firstName || '',
      lastName: user.lastName || '',
      email: user.email || '',
      phone: user.phone || '',
      department: user.department || '',
      role: user.role || 'doctor',
      avatar: user.avatar || ''
    }
    
    // Update preferences
    preferences.value = {
      theme: user.theme || 'system',
      language: user.language || 'en',
      timezone: user.timezone || 'UTC'
    }
    
    // Update security settings
    securitySettings.value.twoFactorEnabled = user.twoFactorEnabled || false
    
    // Update notification settings from user data
    notificationSettings.value = [
      {
        id: 'appointments',
        title: 'Appointment Reminders',
        description: 'Get notified about upcoming appointments',
        email: user.notifyAppointments !== undefined ? user.notifyAppointments : true,
        push: user.notifyAppointmentsPush !== undefined ? user.notifyAppointmentsPush : true
      },
      {
        id: 'patient-updates',
        title: 'Patient Updates',
        description: 'Notifications when patient information changes',
        email: user.notifyPatientUpdates !== undefined ? user.notifyPatientUpdates : true,
        push: user.notifyPatientUpdatesPush !== undefined ? user.notifyPatientUpdatesPush : false
      },
      {
        id: 'analysis-results',
        title: 'Analysis Results',
        description: 'Get notified when AI analysis results are ready',
        email: user.notifyAnalysisResults !== undefined ? user.notifyAnalysisResults : true,
        push: user.notifyAnalysisResultsPush !== undefined ? user.notifyAnalysisResultsPush : true
      },
      {
        id: 'billing',
        title: 'Billing Notifications',
        description: 'Updates about payments and invoices',
        email: user.notifyBilling !== undefined ? user.notifyBilling : false,
        push: user.notifyBillingPush !== undefined ? user.notifyBillingPush : false
      },
      {
        id: 'system',
        title: 'System Updates',
        description: 'Maintenance and system update notifications',
        email: user.notifySystem !== undefined ? user.notifySystem : true,
        push: user.notifySystemPush !== undefined ? user.notifySystemPush : false
      }
    ]
  }
  
  // Also load saved preferences from localStorage as fallback
  const saved = localStorage.getItem('preferences')
  if (saved && (!authStore.user || !authStore.user.theme)) {
    preferences.value = { ...preferences.value, ...JSON.parse(saved) }
  }
}

loadUserData()
</script>
