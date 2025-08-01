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
      <nav class="-mb-px flex space-x-8">
        <button
          v-for="tab in settingsTabs"
          :key="tab.id"
          @click="activeTab = tab.id"
          :class="activeTab === tab.id 
            ? 'border-purple-500 text-purple-600 dark:text-purple-400' 
            : 'border-transparent text-gray-500 dark:text-gray-400 hover:text-gray-700 dark:hover:text-gray-300 hover:border-gray-300'"
          class="whitespace-nowrap py-2 px-1 border-b-2 font-medium text-sm"
        >
          {{ tab.name }}
        </button>
      </nav>
    </div>

    <!-- Profile Settings -->
    <div v-if="activeTab === 'profile'" class="space-y-6">
      <div class="bg-white dark:bg-gray-800 shadow-sm rounded-lg border border-gray-200 dark:border-gray-700">
        <div class="px-6 py-4 border-b border-gray-200 dark:border-gray-700">
          <h3 class="text-lg font-medium text-gray-900 dark:text-white">Profile Information</h3>
          <p class="text-sm text-gray-500 dark:text-gray-400">Update your personal information and profile settings.</p>
        </div>
        
        <form @submit.prevent="saveProfile" class="px-6 py-6 space-y-6">
          <!-- Profile Photo -->
          <div class="flex items-center space-x-6">
            <div class="shrink-0">
              <img class="h-16 w-16 object-cover rounded-full" :src="profileForm.avatar || '/api/placeholder/64/64'" alt="Profile photo" />
            </div>
            <div>
              <label for="photo" class="block text-sm font-medium text-gray-700 dark:text-gray-300">Profile Photo</label>
              <div class="mt-1 flex items-center space-x-4">
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
                  class="px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md text-sm leading-4 font-medium text-gray-700 dark:text-gray-300 hover:bg-gray-50 dark:hover:bg-gray-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-purple-500"
                >
                  Change Photo
                </button>
              </div>
            </div>
          </div>

          <!-- Personal Information -->
          <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
            <div>
              <label for="firstName" class="block text-sm font-medium text-gray-700 dark:text-gray-300">First Name</label>
              <input
                id="firstName"
                v-model="profileForm.firstName"
                type="text"
                required
                class="mt-1 block w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md shadow-sm bg-white dark:bg-gray-700 text-gray-900 dark:text-white focus:outline-none focus:ring-1 focus:ring-purple-500 focus:border-purple-500"
              />
            </div>

            <div>
              <label for="lastName" class="block text-sm font-medium text-gray-700 dark:text-gray-300">Last Name</label>
              <input
                id="lastName"
                v-model="profileForm.lastName"
                type="text"
                required
                class="mt-1 block w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md shadow-sm bg-white dark:bg-gray-700 text-gray-900 dark:text-white focus:outline-none focus:ring-1 focus:ring-purple-500 focus:border-purple-500"
              />
            </div>

            <div>
              <label for="email" class="block text-sm font-medium text-gray-700 dark:text-gray-300">Email</label>
              <input
                id="email"
                v-model="profileForm.email"
                type="email"
                required
                class="mt-1 block w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md shadow-sm bg-white dark:bg-gray-700 text-gray-900 dark:text-white focus:outline-none focus:ring-1 focus:ring-purple-500 focus:border-purple-500"
              />
            </div>

            <div>
              <label for="phone" class="block text-sm font-medium text-gray-700 dark:text-gray-300">Phone</label>
              <input
                id="phone"
                v-model="profileForm.phone"
                type="tel"
                class="mt-1 block w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md shadow-sm bg-white dark:bg-gray-700 text-gray-900 dark:text-white focus:outline-none focus:ring-1 focus:ring-purple-500 focus:border-purple-500"
              />
            </div>

            <div>
              <label for="department" class="block text-sm font-medium text-gray-700 dark:text-gray-300">Department</label>
              <select
                id="department"
                v-model="profileForm.department"
                class="mt-1 block w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md shadow-sm bg-white dark:bg-gray-700 text-gray-900 dark:text-white focus:outline-none focus:ring-1 focus:ring-purple-500 focus:border-purple-500"
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
                class="mt-1 block w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md shadow-sm bg-white dark:bg-gray-700 text-gray-900 dark:text-white focus:outline-none focus:ring-1 focus:ring-purple-500 focus:border-purple-500"
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
              class="inline-flex items-center px-4 py-2 border border-transparent text-sm font-medium rounded-md shadow-sm text-white bg-purple-600 hover:bg-purple-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-purple-500"
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
      <div class="bg-white dark:bg-gray-800 shadow-sm rounded-lg border border-gray-200 dark:border-gray-700">
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
              class="mt-1 block w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md shadow-sm bg-white dark:bg-gray-700 text-gray-900 dark:text-white focus:outline-none focus:ring-1 focus:ring-purple-500 focus:border-purple-500"
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
              class="mt-1 block w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md shadow-sm bg-white dark:bg-gray-700 text-gray-900 dark:text-white focus:outline-none focus:ring-1 focus:ring-purple-500 focus:border-purple-500"
            />
          </div>

          <div>
            <label for="confirmPassword" class="block text-sm font-medium text-gray-700 dark:text-gray-300">Confirm New Password</label>
            <input
              id="confirmPassword"
              v-model="passwordForm.confirmPassword"
              type="password"
              required
              class="mt-1 block w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md shadow-sm bg-white dark:bg-gray-700 text-gray-900 dark:text-white focus:outline-none focus:ring-1 focus:ring-purple-500 focus:border-purple-500"
            />
          </div>

          <div class="flex justify-end">
            <button
              type="submit"
              :disabled="!isPasswordFormValid"
              class="inline-flex items-center px-4 py-2 border border-transparent text-sm font-medium rounded-md shadow-sm text-white bg-purple-600 hover:bg-purple-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-purple-500 disabled:opacity-50 disabled:cursor-not-allowed"
            >
              Update Password
            </button>
          </div>
        </form>
      </div>

      <!-- Two-Factor Authentication -->
      <div class="bg-white dark:bg-gray-800 shadow-sm rounded-lg border border-gray-200 dark:border-gray-700">
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
              class="relative inline-flex h-6 w-11 flex-shrink-0 cursor-pointer rounded-full border-2 border-transparent transition-colors duration-200 ease-in-out focus:outline-none focus:ring-2 focus:ring-offset-2"
            >
              <span
                :class="securitySettings.twoFactorEnabled ? 'translate-x-5' : 'translate-x-0'"
                class="pointer-events-none inline-block h-5 w-5 rounded-full bg-white shadow transform ring-0 transition duration-200 ease-in-out"
              />
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Preferences -->
    <div v-else-if="activeTab === 'preferences'" class="space-y-6">
      <!-- Theme Settings -->
      <div class="bg-white dark:bg-gray-800 shadow-sm rounded-lg border border-gray-200 dark:border-gray-700">
        <div class="px-6 py-4 border-b border-gray-200 dark:border-gray-700">
          <h3 class="text-lg font-medium text-gray-900 dark:text-white">Appearance</h3>
          <p class="text-sm text-gray-500 dark:text-gray-400">Customize how the application looks and feels.</p>
        </div>
        
        <div class="px-6 py-6 space-y-6">
          <div>
            <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-3">Theme</label>
            <div class="grid grid-cols-3 gap-3">
              <label class="relative flex cursor-pointer rounded-lg border bg-white p-4 shadow-sm focus:outline-none">
                <input type="radio" v-model="preferences.theme" value="light" class="sr-only" />
                <span class="flex flex-1">
                  <span class="flex flex-col">
                    <span class="block text-sm font-medium text-gray-900">Light</span>
                    <span class="mt-1 flex items-center text-sm text-gray-500">Always light theme</span>
                  </span>
                </span>
                <CheckCircleIcon v-if="preferences.theme === 'light'" class="h-5 w-5 text-purple-600" />
              </label>

              <label class="relative flex cursor-pointer rounded-lg border bg-gray-900 p-4 text-white shadow-sm focus:outline-none">
                <input type="radio" v-model="preferences.theme" value="dark" class="sr-only" />
                <span class="flex flex-1">
                  <span class="flex flex-col">
                    <span class="block text-sm font-medium text-white">Dark</span>
                    <span class="mt-1 flex items-center text-sm text-gray-300">Always dark theme</span>
                  </span>
                </span>
                <CheckCircleIcon v-if="preferences.theme === 'dark'" class="h-5 w-5 text-purple-400" />
              </label>

              <label class="relative flex cursor-pointer rounded-lg border bg-white p-4 shadow-sm focus:outline-none">
                <input type="radio" v-model="preferences.theme" value="system" class="sr-only" />
                <span class="flex flex-1">
                  <span class="flex flex-col">
                    <span class="block text-sm font-medium text-gray-900">System</span>
                    <span class="mt-1 flex items-center text-sm text-gray-500">Follow system setting</span>
                  </span>
                </span>
                <CheckCircleIcon v-if="preferences.theme === 'system'" class="h-5 w-5 text-purple-600" />
              </label>
            </div>
          </div>

          <!-- Language -->
          <div>
            <label for="language" class="block text-sm font-medium text-gray-700 dark:text-gray-300">Language</label>
            <select
              id="language"
              v-model="preferences.language"
              class="mt-1 block w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md shadow-sm bg-white dark:bg-gray-700 text-gray-900 dark:text-white focus:outline-none focus:ring-1 focus:ring-purple-500 focus:border-purple-500"
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
              class="mt-1 block w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md shadow-sm bg-white dark:bg-gray-700 text-gray-900 dark:text-white focus:outline-none focus:ring-1 focus:ring-purple-500 focus:border-purple-500"
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
              class="inline-flex items-center px-4 py-2 border border-transparent text-sm font-medium rounded-md shadow-sm text-white bg-purple-600 hover:bg-purple-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-purple-500"
            >
              Save Preferences
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Notifications -->
    <div v-else-if="activeTab === 'notifications'" class="space-y-6">
      <div class="bg-white dark:bg-gray-800 shadow-sm rounded-lg border border-gray-200 dark:border-gray-700">
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
                  class="rounded border-gray-300 text-purple-600 focus:ring-purple-500"
                />
                <span class="ml-2 text-sm text-gray-700 dark:text-gray-300">Email</span>
              </label>
              <label class="flex items-center">
                <input
                  v-model="notification.push"
                  type="checkbox"
                  class="rounded border-gray-300 text-purple-600 focus:ring-purple-500"
                />
                <span class="ml-2 text-sm text-gray-700 dark:text-gray-300">Push</span>
              </label>
            </div>
          </div>

          <div class="flex justify-end pt-6 border-t border-gray-200 dark:border-gray-700">
            <button
              @click="saveNotifications"
              class="inline-flex items-center px-4 py-2 border border-transparent text-sm font-medium rounded-md shadow-sm text-white bg-purple-600 hover:bg-purple-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-purple-500"
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
const handlePhotoChange = (event: Event) => {
  const file = (event.target as HTMLInputElement).files?.[0]
  if (file) {
    const reader = new FileReader()
    reader.onload = (e) => {
      profileForm.value.avatar = e.target?.result as string
    }
    reader.readAsDataURL(file)
  }
}

const saveProfile = async () => {
  try {
    // API call to update profile
    console.log('Saving profile:', profileForm.value)
    
    // Update auth store with new profile data
    if (authStore.user) {
      Object.assign(authStore.user, profileForm.value)
    }
    
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

    // API call to change password
    console.log('Changing password')
    
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

const toggleTwoFactor = () => {
  securitySettings.value.twoFactorEnabled = !securitySettings.value.twoFactorEnabled
  console.log('Two-factor authentication:', securitySettings.value.twoFactorEnabled ? 'enabled' : 'disabled')
  alert(`Two-factor authentication ${securitySettings.value.twoFactorEnabled ? 'enabled' : 'disabled'}`)
}

const savePreferences = () => {
  // Save preferences to localStorage or API
  localStorage.setItem('preferences', JSON.stringify(preferences.value))
  console.log('Preferences saved:', preferences.value)
  alert('Preferences saved successfully!')
}

const saveNotifications = () => {
  // Save notification settings to API
  console.log('Notification settings saved:', notificationSettings.value)
  alert('Notification settings saved successfully!')
}

// Load saved preferences on mount
const loadPreferences = () => {
  const saved = localStorage.getItem('preferences')
  if (saved) {
    preferences.value = { ...preferences.value, ...JSON.parse(saved) }
  }
}

loadPreferences()
</script>
