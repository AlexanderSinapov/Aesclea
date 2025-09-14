<!-- Copyright (c) 2025 Alexander Sinapov | Simeon Petkov -->

<!-- All rights reserved. -->
<!-- This code is proprietary and confidential.   -->
<!-- Unauthorized copying, modification, distribution, or use is strictly prohibited. -->

<template>
  <div class="min-h-screen bg-gray-50">
    <!-- Header -->
    <header class="bg-white border-b border-gray-200 shadow-sm">
      <div class="px-4 mx-auto max-w-7xl sm:px-6 lg:px-8">
        <div class="flex items-center justify-between py-4">
          <div class="flex items-center">
            <div class="flex items-center justify-center w-8 h-8 rounded-lg bg-gradient-to-r from-blue-600 to-indigo-600">
              <svg class="w-5 h-5 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
              </svg>
            </div>
            <h1 class="ml-3 text-xl font-semibold text-gray-900">Aesclea Dashboard</h1>
          </div>
          
          <div class="flex items-center space-x-4">
            <button class="relative p-2 text-gray-400 hover:text-gray-500">
              <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 17h5l-5 5l-5-5h5v-5a7.5 7.5 0 1 0-15 0v5h5l-5 5l-5-5h5v-5" />
              </svg>
              <span class="absolute top-0 right-0 block w-2 h-2 bg-red-400 rounded-full ring-2 ring-white"></span>
            </button>
            
            <div class="flex items-center space-x-3">
              <div class="flex items-center justify-center w-8 h-8 bg-gray-300 rounded-full">
                <span class="text-sm font-medium text-gray-700">
                  {{ userInitials }}
                </span>
              </div>
              <div class="hidden md:block">
                <p class="text-sm font-medium text-gray-900">{{ userDisplayName }}</p>
                <div class="flex items-center space-x-2">
                  <p class="text-xs text-gray-500 capitalize">{{ authStore.user?.role || 'User' }}</p>
                  <span v-if="subscriptionStore.currentPlan" class="inline-flex items-center px-2 py-0.5 rounded text-xs font-medium bg-green-100 text-green-800">
                    {{ subscriptionStore.currentPlan.name }}
                  </span>
                </div>
              </div>
            </div>
            
            <button @click="handleLogout" class="ml-4 text-gray-400 hover:text-gray-500" title="Logout">
              <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17 16l4-4m0 0l-4-4m4 4H7m6 4v1a3 3 0 01-3 3H6a3 3 0 01-3-3V7a3 3 0 013-3h4a3 3 0 013 3v1" />
              </svg>
            </button>
          </div>
        </div>
      </div>
    </header>

    <div class="px-4 py-8 mx-auto max-w-7xl sm:px-6 lg:px-8">
      <!-- Welcome Section -->
      <div class="mb-8">
        <h2 class="mb-2 text-2xl font-bold text-gray-900">
          Good {{ getTimeOfDay() }}, {{ authStore.user?.firstName || 'User' }}
        </h2>
        <p class="text-gray-600">Here's what's happening at {{ authStore.user?.hospital || 'your hospital' }} today.</p>
        
        <!-- Subscription Banner -->
        <div v-if="!subscriptionStore.hasActiveSubscription" class="p-4 mt-6 border border-purple-200 rounded-lg bg-gradient-to-r from-purple-50 to-indigo-50 dark:from-purple-900/20 dark:to-indigo-900/20 dark:border-purple-800">
          <div class="flex items-center justify-between">
            <div class="flex items-center space-x-3">
              <div class="flex-shrink-0">
                <svg class="w-6 h-6 text-purple-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 10V3L4 14h7v7l9-11h-7z" />
                </svg>
              </div>
              <div>
                <h3 class="text-sm font-medium text-purple-900 dark:text-purple-100">
                  Unlock AI-Powered Medical Analysis
                </h3>
                <p class="text-sm text-purple-700 dark:text-purple-300">
                  Subscribe to access advanced AI diagnostics and unlimited patient records.
                </p>
              </div>
            </div>
            <div class="flex-shrink-0">
              <button
                @click="router.push('/subscription-selection')"
                class="px-4 py-2 text-sm font-medium text-white transition-colors bg-purple-600 rounded-md hover:bg-purple-700"
              >
                Upgrade Now
              </button>
            </div>
          </div>
        </div>
      </div>

      <!-- Stats Cards -->
      <div class="grid grid-cols-1 gap-6 mb-8 md:grid-cols-2 lg:grid-cols-4">
        <div class="p-6 bg-white border border-gray-200 rounded-lg shadow-sm">
          <div class="flex items-center">
            <div class="flex-shrink-0">
              <div class="flex items-center justify-center w-12 h-12 bg-blue-100 rounded-lg">
                <svg class="w-6 h-6 text-blue-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857M7 20H2v-2a3 3 0 015.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0zm6 3a2 2 0 11-4 0 2 2 0 014 0zM7 10a2 2 0 11-4 0 2 2 0 014 0z" />
                </svg>
              </div>
            </div>
            <div class="ml-4">
              <p class="text-sm font-medium text-gray-500">Total Patients</p>
              <p class="text-2xl font-semibold text-gray-900">1,247</p>
              <p class="text-sm text-green-600">+12% from last month</p>
            </div>
          </div>
        </div>

        <div class="p-6 bg-white border border-gray-200 rounded-lg shadow-sm">
          <div class="flex items-center">
            <div class="flex-shrink-0">
              <div class="flex items-center justify-center w-12 h-12 bg-green-100 rounded-lg">
                <svg class="w-6 h-6 text-green-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5H7a2 2 0 00-2 2v10a2 2 0 002 2h8a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2" />
                </svg>
              </div>
            </div>
            <div class="ml-4">
              <p class="text-sm font-medium text-gray-500">Today's Appointments</p>
              <p class="text-2xl font-semibold text-gray-900">32</p>
              <p class="text-sm text-blue-600">8 pending</p>
            </div>
          </div>
        </div>

        <div class="p-6 bg-white border border-gray-200 rounded-lg shadow-sm">
          <div class="flex items-center">
            <div class="flex-shrink-0">
              <div class="flex items-center justify-center w-12 h-12 bg-yellow-100 rounded-lg">
                <svg class="w-6 h-6 text-yellow-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
                </svg>
              </div>
            </div>
            <div class="ml-4">
              <p class="text-sm font-medium text-gray-500">Critical Alerts</p>
              <p class="text-2xl font-semibold text-gray-900">3</p>
              <p class="text-sm text-red-600">Requires attention</p>
            </div>
          </div>
        </div>

        <div class="p-6 bg-white border border-gray-200 rounded-lg shadow-sm">
          <div class="flex items-center">
            <div class="flex-shrink-0">
              <div class="flex items-center justify-center w-12 h-12 bg-purple-100 rounded-lg">
                <svg class="w-6 h-6 text-purple-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
                </svg>
              </div>
            </div>
            <div class="ml-4">
              <p class="text-sm font-medium text-gray-500">Reports Generated</p>
              <p class="text-2xl font-semibold text-gray-900">89</p>
              <p class="text-sm text-purple-600">This week</p>
            </div>
          </div>
        </div>
      </div>

      <!-- Main Content Grid -->
      <div class="grid grid-cols-1 gap-8 lg:grid-cols-3">
        <!-- Recent Appointments -->
        <div class="lg:col-span-2">
          <div class="bg-white border border-gray-200 rounded-lg shadow-sm">
            <div class="px-6 py-4 border-b border-gray-200">
              <h3 class="text-lg font-medium text-gray-900">Recent Appointments</h3>
            </div>
            <div class="p-6">
              <div class="space-y-4">
                <div v-for="appointment in recentAppointments" :key="appointment.id" class="flex items-center justify-between p-4 rounded-lg bg-gray-50">
                  <div class="flex items-center space-x-4">
                    <div class="flex items-center justify-center w-10 h-10 bg-gray-300 rounded-full">
                      <span class="text-sm font-medium text-gray-700">{{ appointment.patient.initials }}</span>
                    </div>
                    <div>
                      <p class="text-sm font-medium text-gray-900">{{ appointment.patient.name }}</p>
                      <p class="text-sm text-gray-500">{{ appointment.type }} - {{ appointment.time }}</p>
                    </div>
                  </div>
                  <div class="flex items-center space-x-2">
                    <span class="px-2 py-1 text-xs font-medium rounded-full">
                      {{ appointment.status }}
                    </span>
                  </div>
                </div>
              </div>
              <div class="mt-6">
                <button class="w-full text-sm font-medium text-center text-indigo-600 hover:text-indigo-500">
                  View all appointments
                </button>
              </div>
            </div>
          </div>
        </div>

        <!-- Quick Actions & Alerts -->
        <div class="space-y-6">
          <!-- Quick Actions -->
          <div class="bg-white border border-gray-200 rounded-lg shadow-sm">
            <div class="px-6 py-4 border-b border-gray-200">
              <h3 class="text-lg font-medium text-gray-900">Quick Actions</h3>
            </div>
            <div class="p-6">
              <div class="space-y-3">
                <button class="flex items-center w-full px-4 py-3 text-sm font-medium text-left text-gray-700 transition-colors rounded-lg hover:bg-gray-50"
                  :class="{ 'opacity-50 cursor-not-allowed': !subscriptionStore.canAccessAI }"
                  :disabled="!subscriptionStore.canAccessAI"
                  @click="subscriptionStore.canAccessAI ? null : showUpgradeModal()"
                >
                  <svg class="w-5 h-5 mr-3 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9.663 17h4.673M12 3v1m6.364 1.636l-.707.707M21 12h-1M4 12H3m3.343-5.657l-.707-.707m2.828 9.9a5 5 0 117.072 0l-.548.547A3.374 3.374 0 0014 18.469V19a2 2 0 11-4 0v-.531c0-.895-.356-1.754-.988-2.386l-.548-.547z" />
                  </svg>
                  <div class="flex-1">
                    <span>AI Medical Analysis</span>
                    <span v-if="!subscriptionStore.canAccessAI" class="block mt-1 text-xs text-amber-600">
                      🔒 Subscription required
                    </span>
                  </div>
                </button>
                <button class="flex items-center w-full px-4 py-3 text-sm font-medium text-left text-gray-700 transition-colors rounded-lg hover:bg-gray-50">
                  <svg class="w-5 h-5 mr-3 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 6v6m0 0v6m0-6h6m-6 0H6" />
                  </svg>
                  Add New Patient
                </button>
                <button class="flex items-center w-full px-4 py-3 text-sm font-medium text-left text-gray-700 transition-colors rounded-lg hover:bg-gray-50">
                  <svg class="w-5 h-5 mr-3 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 7V3a4 4 0 118 0v4m-4 12v-8m0 0V7a2 2 0 012-2h4a2 2 0 012 2v2m-6 0h8" />
                  </svg>
                  Schedule Appointment
                </button>
                <button class="flex items-center w-full px-4 py-3 text-sm font-medium text-left text-gray-700 transition-colors rounded-lg hover:bg-gray-50">
                  <svg class="w-5 h-5 mr-3 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
                  </svg>
                  Generate Report
                </button>
                <button class="flex items-center w-full px-4 py-3 text-sm font-medium text-left text-gray-700 transition-colors rounded-lg hover:bg-gray-50">
                  <svg class="w-5 h-5 mr-3 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
                  </svg>
                  Search Records
                </button>
                <button class="flex items-center w-full px-4 py-3 text-sm font-medium text-left text-gray-700 transition-colors rounded-lg hover:bg-gray-50">
                  <svg class="w-5 h-5 mr-3 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 7V3a4 4 0 118 0v4m-4 12v-8m0 0V7a2 2 0 012-2h4a2 2 0 012 2v2m-6 0h8" />
                  </svg>
                  Schedule Appointment
                </button>
                <button class="flex items-center w-full px-4 py-3 text-sm font-medium text-left text-gray-700 transition-colors rounded-lg hover:bg-gray-50">
                  <svg class="w-5 h-5 mr-3 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
                  </svg>
                  Generate Report
                </button>
                <button class="flex items-center w-full px-4 py-3 text-sm font-medium text-left text-gray-700 transition-colors rounded-lg hover:bg-gray-50">
                  <svg class="w-5 h-5 mr-3 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
                  </svg>
                  Search Records
                </button>
              </div>
            </div>
          </div>

          <!-- Critical Alerts -->
          <div class="bg-white border border-gray-200 rounded-lg shadow-sm">
            <div class="px-6 py-4 border-b border-gray-200">
              <h3 class="text-lg font-medium text-gray-900">Critical Alerts</h3>
            </div>
            <div class="p-6">
              <div class="space-y-4">
                <div v-for="alert in criticalAlerts" :key="alert.id" class="flex items-start p-3 space-x-3 border border-red-200 rounded-lg bg-red-50">
                  <div class="flex-shrink-0">
                    <svg class="w-5 h-5 text-red-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
                    </svg>
                  </div>
                  <div class="flex-1 min-w-0">
                    <p class="text-sm font-medium text-red-800">{{ alert.title }}</p>
                    <p class="text-sm text-red-600">{{ alert.description }}</p>
                    <p class="mt-1 text-xs text-red-500">{{ alert.time }}</p>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Recent Activity -->
      <div class="mt-8">
        <div class="bg-white border border-gray-200 rounded-lg shadow-sm">
          <div class="px-6 py-4 border-b border-gray-200">
            <h3 class="text-lg font-medium text-gray-900">Recent Activity</h3>
          </div>
          <div class="p-6">
            <div class="flow-root">
              <ul class="-mb-8">
                <li v-for="(activity, activityIdx) in recentActivity" :key="activity.id">
                  <div class="relative pb-8">
                    <span v-if="activityIdx !== recentActivity.length - 1" class="absolute top-4 left-4 -ml-px h-full w-0.5 bg-gray-200"></span>
                    <div class="relative flex space-x-3">
                      <div>
                        <span :class="activity.iconBackground" class="flex items-center justify-center w-8 h-8 rounded-full ring-8 ring-white">
                          <svg class="w-5 h-5 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" :d="activity.iconPath" />
                          </svg>
                        </span>
                      </div>
                      <div class="min-w-0 flex-1 pt-1.5 flex justify-between space-x-4">
                        <div>
                          <p class="text-sm text-gray-500">{{ activity.content }}</p>
                        </div>
                        <div class="text-sm text-right text-gray-500 whitespace-nowrap">
                          {{ activity.time }}
                        </div>
                      </div>
                    </div>
                  </div>
                </li>
              </ul>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '../stores/auth'
import { useSubscriptionStore } from '../stores/subscription'

const router = useRouter()
const authStore = useAuthStore()
const subscriptionStore = useSubscriptionStore()

const userDisplayName = computed(() => {
  const user = authStore.user
  if (user) {
    return `${user.firstName} ${user.lastName}`
  }
  return 'User'
})

const userInitials = computed(() => {
  const user = authStore.user
  if (user) {
    return `${user.firstName.charAt(0)}${user.lastName.charAt(0)}`.toUpperCase()
  }
  return 'U'
})

const getTimeOfDay = () => {
  const hour = new Date().getHours()
  if (hour < 12) return 'morning'
  if (hour < 17) return 'afternoon'
  return 'evening'
}

const handleLogout = async () => {
  await authStore.logout()
  router.push('/login')
}

const showUpgradeModal = () => {
  // Navigate to subscription selection or pricing page
  router.push('/subscription-selection')
}

onMounted(async () => {
  // Redirect to login if not authenticated
  if (!authStore.isAuthenticated) {
    router.push('/login')
    return
  }

  // Load user's subscription status
  await subscriptionStore.loadUserSubscription()

  // If user doesn't have email verified, redirect to verification
  if (authStore.user && !authStore.user.isEmailVerified) {
    router.push(`/email-verification?email=${encodeURIComponent(authStore.user.email)}`)
    return
  }

  // If user doesn't have an active subscription, redirect to subscription selection
  if (!subscriptionStore.hasActiveSubscription) {
    router.push('/subscription-selection')
    return
  }
})

const recentAppointments = ref([
  {
    id: 1,
    patient: { name: 'Sarah Johnson', initials: 'SJ' },
    type: 'Consultation',
    time: '10:00 AM',
    status: 'Confirmed'
  },
  {
    id: 2,
    patient: { name: 'Michael Brown', initials: 'MB' },
    type: 'Follow-up',
    time: '11:30 AM',
    status: 'Pending'
  },
  {
    id: 3,
    patient: { name: 'Emily Davis', initials: 'ED' },
    type: 'Check-up',
    time: '2:00 PM',
    status: 'Completed'
  },
  {
    id: 4,
    patient: { name: 'Robert Wilson', initials: 'RW' },
    type: 'Surgery Consultation',
    time: '3:30 PM',
    status: 'Confirmed'
  }
])

const criticalAlerts = ref([
  {
    id: 1,
    title: 'Patient Emergency',
    description: 'John Doe requires immediate attention in Room 204',
    time: '5 minutes ago'
  },
  {
    id: 2,
    title: 'Equipment Maintenance',
    description: 'MRI Machine #2 scheduled for maintenance in 1 hour',
    time: '15 minutes ago'
  },
  {
    id: 3,
    title: 'Staff Schedule',
    description: 'Dr. Smith has a scheduling conflict tomorrow',
    time: '1 hour ago'
  }
])

const recentActivity = ref([
  {
    id: 1,
    content: 'New patient Sarah Johnson registered',
    time: '2 hours ago',
    iconBackground: 'bg-green-500',
    iconPath: 'M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z'
  },
  {
    id: 2,
    content: 'Lab results uploaded for Michael Brown',
    time: '4 hours ago',
    iconBackground: 'bg-blue-500',
    iconPath: 'M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z'
  },
  {
    id: 3,
    content: 'Emergency appointment scheduled',
    time: '6 hours ago',
    iconBackground: 'bg-red-500',
    iconPath: 'M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z'
  },
  {
    id: 4,
    content: 'Weekly report generated',
    time: '1 day ago',
    iconBackground: 'bg-purple-500',
    iconPath: 'M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z'
  }
])
</script>
