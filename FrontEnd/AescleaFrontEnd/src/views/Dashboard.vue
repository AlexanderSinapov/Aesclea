<!-- Copyright (c) 2025 Alexander Sinapov | Simeon Petkov -->

<!-- All rights reserved. -->
<!-- This code is proprietary and confidential.   -->
<!-- Unauthorized copying, modification, distribution, or use is strictly prohibited. -->

<template>
  <div class="min-h-screen bg-gray-50 dark:bg-gray-900">
    <!-- Sidebar Backdrop for Mobile -->
    <div 
      v-show="sidebarOpen"
      class="fixed inset-0 z-40 bg-gray-600 bg-opacity-75 lg:hidden"
      @click="sidebarOpen = false"
    ></div>

    <!-- Sidebar -->
    <div 
      class="fixed inset-y-0 left-0 z-50 w-64 transition-transform duration-300 ease-in-out transform bg-white border-r border-gray-200 dark:bg-gray-800 dark:border-gray-700 lg:translate-x-0"
      :class="sidebarOpen ? 'translate-x-0' : '-translate-x-full'"
    >
      <div class="flex items-center justify-between h-16 px-4 border-b border-gray-200 dark:border-gray-700">
        <div class="flex items-center">
          <div class="flex items-center justify-center w-8 h-8 rounded-lg bg-gradient-to-r from-purple-600 to-indigo-600">
            <svg class="w-5 h-5 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4.318 6.318a4.5 4.5 0 000 6.364L12 20.364l7.682-7.682a4.5 4.5 0 00-6.364-6.364L12 7.636l-1.318-1.318a4.5 4.5 0 00-6.364 0z" />
            </svg>
          </div>
          <span class="ml-2 text-xl font-semibold text-gray-900 dark:text-white">Aesclea</span>
        </div>
        <button @click="toggleSidebar" class="p-1 text-gray-400 hover:text-gray-500 dark:hover:text-gray-300 lg:hidden">
          <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
          </svg>
        </button>
      </div>

      <!-- Navigation -->
      <nav class="px-2 mt-5">
        <div class="space-y-1">
          <button
            v-for="(item, index) in navigationItems"
            :key="item.name"
            @click="handleNavigation(item)"
            :disabled="item.requiresSubscription && !subscriptionStore.hasActiveSubscription"
            :title="`${item.name} (Alt+${index + 1})`"
            class="flex items-center w-full px-2 py-2 text-sm font-medium transition-colors rounded-md"
            :class="[
              activeView === item.view 
                ? 'bg-purple-100 text-purple-900 dark:bg-purple-900 dark:text-purple-100' 
                : 'text-gray-600 hover:bg-gray-50 hover:text-gray-900 dark:text-gray-300 dark:hover:bg-gray-700 dark:hover:text-white',
              item.requiresSubscription && !subscriptionStore.hasActiveSubscription
                ? 'opacity-50 cursor-not-allowed'
                : ''
            ]"
            >
            <component :is="item.icon" class="flex-shrink-0 w-5 h-5 mr-3" />
            <span class="flex-1">{{ item.name }}</span>
            <div class="flex items-center space-x-1">
              <span v-if="index < 9" class="hidden text-xs text-gray-400 lg:inline">Alt+{{ index + 1 }}</span>
              <svg v-if="item.requiresSubscription && !subscriptionStore.hasActiveSubscription" 
                   class="w-3 h-3 text-yellow-500" 
                   fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 15v2m-6 4h12a2 2 0 002-2v-6a2 2 0 00-2-2H6a2 2 0 00-2 2v6a2 2 0 002 2zm10-10V7a4 4 0 00-8 0v4h8z" />
              </svg>
            </div>
          </button>
        </div>

        <!-- Department Quick Access -->
        <div class="mt-8">
          <h3 class="px-3 text-xs font-semibold tracking-wider text-gray-500 uppercase dark:text-gray-400">
            Departments
          </h3>
          <div class="mt-2 space-y-1">
            <button
              v-for="dept in departments"
              :key="dept.id"
              @click="selectDepartment(dept.id)"
              class="flex items-center w-full px-2 py-2 text-sm font-medium transition-colors rounded-md"
              :class="selectedDepartment === dept.id 
                ? 'bg-blue-100 text-blue-900 dark:bg-blue-900 dark:text-blue-100' 
                : 'text-gray-600 hover:bg-gray-50 hover:text-gray-900 dark:text-gray-300 dark:hover:bg-gray-700 dark:hover:text-white'"
            >
              <component :is="dept.icon" class="flex-shrink-0 w-4 h-4 mr-3" />
              {{ dept.name }}
              <span 
                v-if="dept.patientCount" 
                class="ml-auto inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium"
                :class="selectedDepartment === dept.id 
                  ? 'bg-blue-200 text-blue-800 dark:bg-blue-800 dark:text-blue-200' 
                  : 'bg-gray-100 text-gray-600 dark:bg-gray-700 dark:text-gray-300'"
              >
                {{ dept.patientCount }}
              </span>
            </button>
          </div>
        </div>
      </nav>

      <!-- Subscription Status -->
      <div v-if="!subscriptionStore.hasActiveSubscription" class="mx-2 mb-4">
        <div class="p-3 border border-yellow-200 rounded-lg bg-yellow-50 dark:bg-yellow-900/20 dark:border-yellow-800">
          <div class="flex items-center">
            <svg class="w-4 h-4 mr-2 text-yellow-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-2.5L13.732 4c-.77-.833-1.964-.833-2.732 0L3.732 16.5c-.77.833.192 2.5 1.732 2.5z" />
            </svg>
            <div class="flex-1">
              <p class="text-xs font-medium text-yellow-800 dark:text-yellow-200">Limited Access</p>
              <p class="text-xs text-yellow-600 dark:text-yellow-300">AI features require subscription</p>
            </div>
          </div>
          <button 
            @click="navigateToSubscription"
            class="w-full px-2 py-1 mt-2 text-xs text-white bg-yellow-600 rounded hover:bg-yellow-700"
          >
            Upgrade
          </button>
        </div>
      </div>

      <!-- User Info -->
      <div class="absolute bottom-0 left-0 right-0 p-4 border-t border-gray-200 dark:border-gray-700">
        <div class="flex items-center">
          <div class="flex items-center justify-center w-8 h-8 bg-gray-300 rounded-full dark:bg-gray-600">
            <span class="text-sm font-medium text-gray-700 dark:text-gray-300">
              {{ userInitials }}
            </span>
          </div>
          <div class="min-w-0 ml-3">
            <p class="text-sm font-medium text-gray-900 truncate dark:text-white">
              {{ userDisplayName }}
            </p>
            <p class="text-xs text-gray-500 dark:text-gray-400">
              {{ userRole }}
            </p>
            <p v-if="subscriptionStore.hasActiveSubscription" class="text-xs text-green-600 dark:text-green-400">
              {{ subscriptionStore.currentPlan?.name }} Plan
            </p>
          </div>
          <div class="flex items-center ml-auto space-x-2">
            <button @click="activeView = 'settings'" class="text-gray-400 hover:text-gray-500 dark:hover:text-gray-300" title="Settings">
              <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10.325 4.317c.426-1.756 2.924-1.756 3.35 0a1.724 1.724 0 002.573 1.066c1.543-.94 3.31.826 2.37 2.37a1.724 1.724 0 001.065 2.572c1.756.426 1.756 2.924 0 3.35a1.724 1.724 0 00-1.066 2.573c.94 1.543-.826 3.31-2.37 2.37a1.724 1.724 0 00-2.572 1.065c-.426 1.756-2.924 1.756-3.35 0a1.724 1.724 0 00-2.573-1.066c-1.543.94-3.31-.826-2.37-2.37a1.724 1.724 0 00-1.065-2.572c-1.756-.426-1.756-2.924 0-3.35a1.724 1.724 0 001.066-2.573c-.94-1.543.826-3.31 2.37-2.37.996.608 2.296.07 2.572-1.065z" />
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
              </svg>
            </button>
            <button @click="handleLogout" class="text-gray-400 hover:text-gray-500 dark:hover:text-gray-300" title="Logout">
              <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17 16l4-4m0 0l-4-4m4 4H7m6 4v1a3 3 0 01-3 3H6a3 3 0 01-3-3V7a3 3 0 013-3h4a3 3 0 013 3v1" />
              </svg>
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Main Content -->
    <div class="lg:pl-64">
      <!-- Mobile header -->
      <div class="sticky top-0 z-40 flex items-center h-16 px-4 bg-white border-b border-gray-200 shadow-sm shrink-0 gap-x-4 dark:border-gray-700 dark:bg-gray-800 sm:gap-x-6 sm:px-6 lg:hidden">
        <button @click="toggleSidebar" class="-m-2.5 p-2.5 text-gray-700 dark:text-gray-300 lg:hidden">
          <svg class="w-6 h-6" fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" d="M3.75 6.75h16.5M3.75 12h16.5m-16.5 5.25h16.5" />
          </svg>
        </button>
        <div class="w-px h-6 bg-gray-200 dark:bg-gray-700 lg:hidden" />
        <div class="flex self-stretch flex-1 gap-x-4 lg:gap-x-6">
          <div class="relative flex items-center flex-1">
            <h1 class="text-lg font-semibold text-gray-900 dark:text-white">
              {{ currentViewTitle }}
            </h1>
          </div>
          <div class="flex items-center gap-x-4 lg:gap-x-6">
            <!-- Quick Actions -->
            <div class="items-center hidden space-x-2 md:flex">
              <button
                v-if="activeView === 'patients'"
                @click="showAddPatientModal"
                class="inline-flex items-center px-3 py-1.5 border border-transparent text-xs font-medium rounded-md text-white bg-purple-600 hover:bg-purple-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-purple-500"
              >
                <svg class="w-3 h-3 mr-1" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 6v6m0 0v6m0-6h6m-6 0H6" />
                </svg>
                Add Patient
              </button>
              <button
                v-if="activeView === 'appointments'"
                @click="showNewAppointmentModal"
                class="inline-flex items-center px-3 py-1.5 border border-transparent text-xs font-medium rounded-md text-white bg-purple-600 hover:bg-purple-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-purple-500"
              >
                <svg class="w-3 h-3 mr-1" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 6v6m0 0v6m0-6h6m-6 0H6" />
                </svg>
                New Appointment
              </button>
            </div>

            <!-- User Menu -->
            <div class="relative">
              <button @click="activeView = 'settings'" class="p-1 text-gray-400 hover:text-gray-500 dark:hover:text-gray-300" title="Settings">
                <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10.325 4.317c.426-1.756 2.924-1.756 3.35 0a1.724 1.724 0 002.573 1.066c1.543-.94 3.31.826 2.37 2.37a1.724 1.724 0 001.065 2.572c1.756.426 1.756 2.924 0 3.35a1.724 1.724 0 00-1.066 2.573c.94 1.543-.826 3.31-2.37 2.37a1.724 1.724 0 00-2.572 1.065c-.426 1.756-2.924 1.756-3.35 0a1.724 1.724 0 00-2.573-1.066c-1.543.94-3.31-.826-2.37-2.37a1.724 1.724 0 00-1.065-2.572c-1.756-.426-1.756-2.924 0-3.35a1.724 1.724 0 001.066-2.573c-.94-1.543.826-3.31 2.37-2.37.996.608 2.296.07 2.572-1.065z" />
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
                </svg>
              </button>
            </div>
          </div>
        </div>
      </div>

      <!-- Dashboard Content -->
      <main class="py-6">
        <div class="px-4 mx-auto max-w-7xl sm:px-6 lg:px-8">
          <!-- Loading State -->
          <div v-if="subscriptionStore.isLoading" class="flex items-center justify-center py-12">
            <div class="text-center">
              <svg class="w-8 h-8 mx-auto mb-4 text-purple-600 animate-spin" fill="none" viewBox="0 0 24 24">
                <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
                <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
              </svg>
              <p class="text-gray-600 dark:text-gray-400">Loading dashboard...</p>
            </div>
          </div>

          <!-- Content -->
          <div v-else>
            <!-- Subscription Warning for AI Analysis -->
            <div v-if="activeView === 'analysis' && !subscriptionStore.hasActiveSubscription" 
                 class="p-4 mb-6 border border-yellow-200 rounded-lg bg-yellow-50 dark:bg-yellow-900/20 dark:border-yellow-800">
              <div class="flex items-center">
                <svg class="w-5 h-5 mr-3 text-yellow-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-2.5L13.732 4c-.77-.833-1.964-.833-2.732 0L3.732 16.5c-.77.833.192 2.5 1.732 2.5z" />
                </svg>
                <div class="flex-1">
                  <h3 class="text-sm font-medium text-yellow-800 dark:text-yellow-200">Subscription Required</h3>
                  <p class="mt-1 text-sm text-yellow-600 dark:text-yellow-300">
                    AI Analysis features require an active subscription. Limited functionality is available.
                  </p>
                </div>
                <button 
                  @click="navigateToSubscription"
                  class="px-3 py-1 ml-4 text-sm text-white bg-yellow-600 rounded hover:bg-yellow-700"
                >
                  Upgrade Now
                </button>
              </div>
            </div>

            <!-- Overview -->
            <DashboardOverview v-if="activeView === 'overview'" 
                               :selected-department="selectedDepartment" 
                               @navigate="handleNavigation" />
            
            <!-- Patients -->
            <PatientsView v-else-if="activeView === 'patients'" :selected-department="selectedDepartment" />
            
            <!-- AI Medical Assistant -->
            <div v-else-if="activeView === 'assistant'" class="h-[calc(100vh-4rem)]">
              <MedicalAssistant />
            </div>
            
            <!-- AI Analysis -->
            <AnalysisView v-else-if="activeView === 'analysis'" 
                          :selected-department="selectedDepartment" 
                          :has-subscription="subscriptionStore.hasActiveSubscription" />
            
            <!-- Appointments -->
            <AppointmentsView v-else-if="activeView === 'appointments'" :selected-department="selectedDepartment" />
            
            <!-- Billing & Payments -->
            <BillingView v-else-if="activeView === 'billing'" />
            
            <!-- Settings -->
            <SettingsView v-else-if="activeView === 'settings'" />
            
            <!-- Admin Panel -->
            <AdminPanel v-else-if="activeView === 'admin'" />
          </div>
        </div>
      </main>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, onUnmounted, defineAsyncComponent, nextTick } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '../stores/auth'
import { useSubscriptionStore } from '../stores/subscription'
import { usePatientsStore } from '../stores/patients'

// Import dashboard components asynchronously
const DashboardOverview = defineAsyncComponent(() => import('../components/dashboard/DashboardOverview.vue'))
const PatientsView = defineAsyncComponent(() => import('../components/dashboard/PatientsView.vue'))
const MedicalAssistant = defineAsyncComponent(() => import('../components/MedicalAssistant.vue'))
const AnalysisView = defineAsyncComponent(() => import('../components/dashboard/AnalysisView.vue'))
const AppointmentsView = defineAsyncComponent(() => import('../components/dashboard/AppointmentsView.vue'))
const BillingView = defineAsyncComponent(() => import('../components/dashboard/BillingView.vue'))
const SettingsView = defineAsyncComponent(() => import('../components/dashboard/SettingsView.vue'))
const AdminPanel = defineAsyncComponent(() => import('../components/dashboard/AdminPanel.vue'))

const router = useRouter()
const authStore = useAuthStore()
const subscriptionStore = useSubscriptionStore()
const patientsStore = usePatientsStore()

const sidebarOpen = ref(false)
const activeView = ref('overview')
const selectedDepartment = ref<string | null>(null)

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

const userRole = computed(() => {
  // This would come from user data or be determined by subscription/permissions
  return 'Medical Professional'
})

const currentViewTitle = computed(() => {
  const item = navigationItems.find(item => item.view === activeView.value)
  return item?.name || 'Dashboard'
})

// Define SVG icons as inline components for simplicity
const HomeIcon = {
  template: `<svg fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" d="M2.25 12l8.954-8.955c.44-.439 1.152-.439 1.591 0L21.75 12M4.5 9.75v10.125c0 .621.504 1.125 1.125 1.125H9.75v-4.875c0-.621.504-1.125 1.125-1.125h2.25c.621 0 1.125.504 1.125 1.125V21h4.125c.621 0 1.125-.504 1.125-1.125V9.75M8.25 21h8.25" /></svg>`
}

const UsersIcon = {
  template: `<svg fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" d="M15 19.128a9.38 9.38 0 002.625.372 9.337 9.337 0 004.121-.952 4.125 4.125 0 00-7.533-2.493M15 19.128v-.003c0-1.113-.285-2.16-.786-3.07M15 19.128v.106A12.318 12.318 0 018.624 21c-2.331 0-4.512-.645-6.374-1.766l-.001-.109a6.375 6.375 0 0111.964-3.07M12 6.375a3.375 3.375 0 11-6.75 0 3.375 3.375 0 016.75 0zm8.25 2.25a2.625 2.625 0 11-5.25 0 2.625 2.625 0 015.25 0z" /></svg>`
}

const ChartBarIcon = {
  template: `<svg fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" d="M3 13.125C3 12.504 3.504 12 4.125 12h2.25c.621 0 1.125.504 1.125 1.125v6.75C7.5 20.496 6.996 21 6.375 21h-2.25A1.125 1.125 0 013 19.875v-6.75zM9.75 8.625c0-.621.504-1.125 1.125-1.125h2.25c.621 0 1.125.504 1.125 1.125v11.25c0 .621-.504 1.125-1.125 1.125h-2.25a1.125 1.125 0 01-1.125-1.125V8.625zM16.5 4.125c0-.621.504-1.125 1.125-1.125h2.25C20.496 3 21 3.504 21 4.125v15.75c0 .621-.504 1.125-1.125 1.125h-2.25a1.125 1.125 0 01-1.125-1.125V4.125z" /></svg>`
}

const CalendarIcon = {
  template: `<svg fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" d="M6.75 3v2.25M17.25 3v2.25M3 18.75V7.5a2.25 2.25 0 012.25-2.25h13.5A2.25 2.25 0 0121 7.5v11.25m-18 0A2.25 2.25 0 005.25 21h13.5A2.25 2.25 0 0021 18.75m-18 0v-7.5A2.25 2.25 0 015.25 9h13.5A2.25 2.25 0 0121 11.25v7.5" /></svg>`
}

const CreditCardIcon = {
  template: `<svg fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" d="M2.25 8.25h19.5M2.25 9h19.5m-16.5 5.25h6m-6 2.25h3m-3.75 3h15a2.25 2.25 0 002.25-2.25V6.75A2.25 2.25 0 0119.5 4.5h-15a2.25 2.25 0 00-2.25 2.25v10.5A2.25 2.25 0 004.5 19.5z" /></svg>`
}

const CogIcon = {
  template: `<svg fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" d="M9.594 3.94c.09-.542.56-.94 1.11-.94h2.593c.55 0 1.02.398 1.11.94l.213 1.281c.063.374.313.686.645.87.074.04.147.083.22.127.324.196.72.257 1.075.124l1.217-.456a1.125 1.125 0 011.37.49l1.296 2.247a1.125 1.125 0 01-.26 1.431l-1.003.827c-.293.24-.438.613-.431.992a6.759 6.759 0 010 .255c-.007.378.138.75.43.99l1.005.828c.424.35.534.954.26 1.43l-1.298 2.247a1.125 1.125 0 01-1.369.491l-1.217-.456c-.355-.133-.75-.072-1.076.124a6.57 6.57 0 01-.22.128c-.331.183-.581.495-.644.869l-.213 1.28c-.09.543-.56.941-1.11.941h-2.594c-.55 0-1.02-.398-1.11-.94l-.213-1.281c-.062-.374-.312-.686-.644-.87a6.52 6.52 0 01-.22-.127c-.325-.196-.72-.257-1.076-.124l-1.217.456a1.125 1.125 0 01-1.369-.49l-1.297-2.247a1.125 1.125 0 01.26-1.431l1.004-.827c.292-.24.437-.613.43-.992a6.932 6.932 0 010-.255c.007-.378-.138-.75-.43-.99l-1.004-.828a1.125 1.125 0 01-.26-1.43l1.297-2.247a1.125 1.125 0 011.37-.491l1.216.456c.356.133.751.072 1.076-.124.072-.044.146-.087.22-.128.332-.183.582-.495.644-.869l.214-1.281z" /><path stroke-linecap="round" stroke-linejoin="round" d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" /></svg>`
}

const ShieldCheckIcon = {
  template: `<svg fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" d="M9 12.75L11.25 15 15 9.75m-3-7.036A11.959 11.959 0 013.598 6 11.99 11.99 0 003 9.749c0 5.592 3.824 10.29 9 11.623 5.176-1.332 9-6.03 9-11.622 0-1.31-.21-2.571-.598-3.751h-.152c-3.196 0-6.1-1.248-8.25-3.285z" /></svg>`
}

const SupportIcon = {
  template: `<svg fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" d="M8.625 12a.375.375 0 11-.75 0 .375.375 0 01.75 0zm0 0H8.25m4.125 0a.375.375 0 11-.75 0 .375.375 0 01.75 0zm0 0H12m4.125 0a.375.375 0 11-.75 0 .375.375 0 01.75 0zm0 0h-.375M21 12c0 4.556-4.03 8.25-9 8.25a9.764 9.764 0 01-2.555-.337A5.972 5.972 0 015.41 20.97a5.969 5.969 0 01-.474-.065 4.48 4.48 0 00.978-2.025c.09-.457-.133-.901-.467-1.226C3.93 16.178 3 14.189 3 12c0-4.556 4.03-8.25 9-8.25s9 3.694 9 8.25z" /></svg>`
}

const ChatIcon = {
  template: `<svg fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" d="M8 12h.01M12 12h.01M16 12h.01M21 12c0 4.418-4.03 8-9 8a9.863 9.863 0 01-4.255-.949L3 20l1.395-3.72C3.512 15.042 3 13.574 3 12c0-4.418 4.03-8 9-8s9 3.582 9 8z" /></svg>`
}

// Department icons
const HeartIcon = {
  template: `<svg fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" d="M21 8.25c0-2.485-2.099-4.5-4.688-4.5-1.935 0-3.597 1.126-4.312 2.733-.715-1.607-2.377-2.733-4.313-2.733C5.1 3.75 3 5.765 3 8.25c0 7.22 9 12 9 12s9-4.78 9-12z" /></svg>`
}

const BeakerIcon = {
  template: `<svg fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" d="M9.75 3.104v5.714a2.25 2.25 0 01-.659 1.591L5 14.5a2.25 2.25 0 00-.659 1.591v.035c0 .623.505 1.125 1.125 1.125h12.999a1.125 1.125 0 001.125-1.125v-.035c0-.592-.237-1.16-.659-1.591L14.25 10.409a2.25 2.25 0 01-.659-1.591V3.104z" /></svg>`
}

const navigationItems = [
  { name: 'Overview', view: 'overview', icon: HomeIcon, requiresSubscription: false },
  { name: 'Patients', view: 'patients', icon: UsersIcon, requiresSubscription: false },
  { name: 'AI Assistant', view: 'assistant', icon: ChatIcon, requiresSubscription: true },
  { name: 'AI Analysis', view: 'analysis', icon: ChartBarIcon, requiresSubscription: true },
  { name: 'Appointments', view: 'appointments', icon: CalendarIcon, requiresSubscription: false },
  { name: 'Billing', view: 'billing', icon: CreditCardIcon, requiresSubscription: false },
  { name: 'Support', view: 'support', icon: SupportIcon, requiresSubscription: false },
  { name: 'Settings', view: 'settings', icon: CogIcon, requiresSubscription: false },
  { name: 'Admin', view: 'admin', icon: ShieldCheckIcon, requiresSubscription: false },
]

const departments = ref([
  { id: 'cardiology', name: 'Cardiology', icon: HeartIcon, patientCount: 23 },
  { id: 'neurology', name: 'Neurology', icon: BeakerIcon, patientCount: 15 },
  { id: 'oncology', name: 'Oncology', icon: BeakerIcon, patientCount: 31 },
  { id: 'radiology', name: 'Radiology', icon: BeakerIcon, patientCount: 18 },
  { id: 'emergency', name: 'Emergency', icon: HeartIcon, patientCount: 7 },
  { id: 'pediatrics', name: 'Pediatrics', icon: HeartIcon, patientCount: 12 },
])

const handleNavigation = (itemOrView: any) => {
  // Handle string view names from emit events
  if (typeof itemOrView === 'string') {
    const navigationItem = navigationItems.find(item => item.view === itemOrView)
    if (navigationItem) {
      if (navigationItem.requiresSubscription && !subscriptionStore.hasActiveSubscription) {
        navigateToSubscription()
        return
      }
      // Handle special routes that should navigate to different pages
      if (itemOrView === 'support') {
        router.push('/support')
        return
      }
    }
    activeView.value = itemOrView
    return
  }
  
  // Handle navigation item objects
  if (itemOrView.requiresSubscription && !subscriptionStore.hasActiveSubscription) {
    // Show subscription required modal or navigate to subscription page
    navigateToSubscription()
    return
  }
  
  // Handle special routes that should navigate to different pages
  if (itemOrView.view === 'support') {
    router.push('/support')
    return
  }
  
  activeView.value = itemOrView.view
}

const navigateToSubscription = () => {
  router.push('/subscription-selection')
}

const toggleSidebar = () => {
  sidebarOpen.value = !sidebarOpen.value
}

const selectDepartment = (departmentId: string) => {
  selectedDepartment.value = selectedDepartment.value === departmentId ? null : departmentId
}

const handleLogout = async () => {
  await authStore.logout()
  router.push('/login')
}

// Quick action functions
const showAddPatientModal = () => {
  // Emit event or trigger modal in PatientsView
  // We'll communicate through the view component
  activeView.value = 'patients'
  // Use a custom event to trigger the add patient modal
  nextTick(() => {
    document.dispatchEvent(new CustomEvent('show-add-patient-modal'))
  })
}

const showNewAppointmentModal = () => {
  // Emit event or trigger modal in AppointmentsView
  activeView.value = 'appointments'
  // Use a custom event to trigger the new appointment modal
  nextTick(() => {
    document.dispatchEvent(new CustomEvent('show-new-appointment-modal'))
  })
}

// Keyboard shortcuts
const handleKeydown = (event: KeyboardEvent) => {
  if (event.altKey) {
    switch (event.key) {
      case '1':
        event.preventDefault()
        activeView.value = 'overview'
        break
      case '2':
        event.preventDefault()
        activeView.value = 'patients'
        break
      case '3':
        if (subscriptionStore.hasActiveSubscription) {
          event.preventDefault()
          activeView.value = 'analysis'
        }
        break
      case '4':
        event.preventDefault()
        activeView.value = 'appointments'
        break
      case 's':
        event.preventDefault()
        activeView.value = 'settings'
        break
    }
  }
  
  if (event.key === 'Escape') {
    sidebarOpen.value = false
  }
}

onMounted(async () => {
  // Check authentication
  if (!authStore.isAuthenticated) {
    router.push('/login')
    return
  }

  // Add keyboard shortcuts
  document.addEventListener('keydown', handleKeydown)

  // Load subscription status and initial data
  try {
    await Promise.all([
      subscriptionStore.loadUserSubscription(),
      subscriptionStore.loadAvailablePlans(),
      patientsStore.fetchPatients(),
      patientsStore.fetchAppointments(),
    ])
  } catch (error) {
    console.error('Error loading dashboard data:', error)
  }
})

onUnmounted(() => {
  document.removeEventListener('keydown', handleKeydown)
})
</script>
