<!-- Copyright (c) 2025 Alexander Sinapov | Simeon Petkov -->

<!-- All rights reserved. -->
<!-- This code is proprietary and confidential.   -->
<!-- Unauthorized copying, modification, distribution, or use is strictly prohibited. -->

<template>
  <div class="space-y-6">
    <!-- Page Header -->
    <div class="sm:flex sm:items-center sm:justify-between">
      <div>
        <h1 class="text-2xl font-bold text-gray-900 dark:text-white">Dashboard Overview</h1>
        <p class="mt-1 text-sm text-gray-500 dark:text-gray-400">
          Welcome back, {{ authStore.user?.firstName }}! Here's what's happening today.
        </p>
      </div>
      <div class="mt-4 sm:mt-0">
        <button
          @click="refreshData"
          :disabled="isRefreshing"
          class="inline-flex items-center px-4 py-2 text-sm font-medium text-white bg-purple-600 border border-transparent rounded-md shadow-sm hover:bg-purple-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-purple-500 disabled:opacity-50"
        >
          <svg 
            class="w-4 h-4 mr-2" 
            :class="{ 'animate-spin': isRefreshing }"
            fill="none" 
            stroke="currentColor" 
            viewBox="0 0 24 24"
          >
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15" />
          </svg>
          Refresh
        </button>
      </div>
    </div>

    <!-- Stats Cards -->
    <div class="grid grid-cols-1 gap-5 sm:grid-cols-2 lg:grid-cols-4">
      <div class="overflow-hidden bg-white border border-gray-200 rounded-lg shadow-sm dark:bg-gray-800 dark:border-gray-700">
        <div class="p-5">
          <div class="flex items-center">
            <div class="flex-shrink-0">
              <UsersIcon class="w-6 h-6 text-gray-400" />
            </div>
            <div class="flex-1 w-0 ml-5">
              <dl>
                <dt class="text-sm font-medium text-gray-500 truncate dark:text-gray-400">Total Patients</dt>
                <dd class="flex items-baseline">
                  <div class="text-2xl font-semibold text-gray-900 dark:text-white">{{ patientsStore.totalPatients }}</div>
                  <div class="flex items-baseline ml-2 text-sm font-semibold text-green-600">
                    <ArrowUpIcon class="self-center flex-shrink-0 w-3 h-3 text-green-500" />
                    <span class="sr-only">Increased by</span>
                    12%
                  </div>
                </dd>
              </dl>
            </div>
          </div>
        </div>
      </div>

      <div class="overflow-hidden bg-white border border-gray-200 rounded-lg shadow-sm dark:bg-gray-800 dark:border-gray-700">
        <div class="p-5">
          <div class="flex items-center">
            <div class="flex-shrink-0">
              <CalendarIcon class="w-6 h-6 text-gray-400" />
            </div>
            <div class="flex-1 w-0 ml-5">
              <dl>
                <dt class="text-sm font-medium text-gray-500 truncate dark:text-gray-400">Today's Appointments</dt>
                <dd class="flex items-baseline">
                  <div class="text-2xl font-semibold text-gray-900 dark:text-white">{{ patientsStore.todayAppointments.length }}</div>
                  <div class="flex items-baseline ml-2 text-sm font-semibold text-blue-600">
                    <span class="text-xs">{{ upcomingAppointments }} upcoming</span>
                  </div>
                </dd>
              </dl>
            </div>
          </div>
        </div>
      </div>

      <div class="overflow-hidden bg-white border border-gray-200 rounded-lg shadow-sm dark:bg-gray-800 dark:border-gray-700">
        <div class="p-5">
          <div class="flex items-center">
            <div class="flex-shrink-0">
              <ChartBarIcon class="w-6 h-6 text-gray-400" />
            </div>
            <div class="flex-1 w-0 ml-5">
              <dl>
                <dt class="text-sm font-medium text-gray-500 truncate dark:text-gray-400">Pending Analyses</dt>
                <dd class="flex items-baseline">
                  <div class="text-2xl font-semibold text-gray-900 dark:text-white">{{ analysisStore.pendingAnalyses.length }}</div>
                  <div class="flex items-baseline ml-2 text-sm font-semibold text-orange-600">
                    <span class="text-xs">{{ completedToday }} completed today</span>
                  </div>
                </dd>
              </dl>
            </div>
          </div>
        </div>
      </div>

      <div class="overflow-hidden bg-white border border-gray-200 rounded-lg shadow-sm dark:bg-gray-800 dark:border-gray-700">
        <div class="p-5">
          <div class="flex items-center">
            <div class="flex-shrink-0">
              <ExclamationTriangleIcon class="w-6 h-6 text-gray-400" />
            </div>
            <div class="flex-1 w-0 ml-5">
              <dl>
                <dt class="text-sm font-medium text-gray-500 truncate dark:text-gray-400">Critical Patients</dt>
                <dd class="flex items-baseline">
                  <div class="text-2xl font-semibold text-gray-900 dark:text-white">{{ patientsStore.criticalPatients }}</div>
                  <div class="flex items-baseline ml-2 text-sm font-semibold text-red-600">
                    <span class="text-xs">Requires attention</span>
                  </div>
                </dd>
              </dl>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Department Overview -->
    <div v-if="!selectedDepartment" class="grid grid-cols-1 gap-6 lg:grid-cols-2">
      <!-- Department Stats -->
      <div class="bg-white border border-gray-200 rounded-lg shadow-sm dark:bg-gray-800 dark:border-gray-700">
        <div class="px-6 py-4 border-b border-gray-200 dark:border-gray-700">
          <h3 class="text-lg font-medium text-gray-900 dark:text-white">Department Overview</h3>
        </div>
        <div class="p-6">
          <div class="space-y-4">
            <div v-for="(patients, department) in patientsStore.patientsByDepartment" :key="department" class="flex items-center justify-between">
              <div class="flex items-center">
                <div class="w-3 h-3 mr-3 rounded-full" :class="getDepartmentColor(department)"></div>
                <span class="text-sm font-medium text-gray-900 capitalize dark:text-white">{{ department }}</span>
              </div>
              <div class="text-sm text-gray-500 dark:text-gray-400">{{ patients.length }} patients</div>
            </div>
          </div>
        </div>
      </div>

      <!-- Recent Activity -->
      <div class="bg-white border border-gray-200 rounded-lg shadow-sm dark:bg-gray-800 dark:border-gray-700">
        <div class="px-6 py-4 border-b border-gray-200 dark:border-gray-700">
          <h3 class="text-lg font-medium text-gray-900 dark:text-white">Recent Activity</h3>
        </div>
        <div class="p-6">
          <div class="flow-root">
            <ul class="-mb-8">
              <li v-for="(activity, index) in recentActivity" :key="activity.id" class="relative pb-8">
                <div v-if="index !== recentActivity.length - 1" class="absolute top-5 left-5 -ml-px h-full w-0.5 bg-gray-200 dark:bg-gray-700"></div>
                <div class="relative flex items-start space-x-3">
                  <div>
                    <div class="relative px-1">
                      <div class="flex items-center justify-center w-8 h-8 bg-gray-100 rounded-full dark:bg-gray-700 ring-8 ring-white dark:ring-gray-800">
                        <component :is="getActivityIcon(activity.type)" class="w-4 h-4 text-gray-500 dark:text-gray-400" />
                      </div>
                    </div>
                  </div>
                  <div class="flex-1 min-w-0">
                    <div>
                      <div class="text-sm">
                        <span class="font-medium text-gray-900 dark:text-white">{{ activity.title }}</span>
                      </div>
                      <p class="mt-0.5 text-sm text-gray-500 dark:text-gray-400">{{ activity.description }}</p>
                    </div>
                    <div class="mt-2 text-sm text-gray-500 dark:text-gray-400">
                      <time>{{ formatTime(activity.timestamp) }}</time>
                    </div>
                  </div>
                </div>
              </li>
            </ul>
          </div>
        </div>
      </div>
    </div>

    <!-- Quick Actions -->
    <div class="bg-white border border-gray-200 rounded-lg shadow-sm dark:bg-gray-800 dark:border-gray-700">
      <div class="px-6 py-4 border-b border-gray-200 dark:border-gray-700">
        <h3 class="text-lg font-medium text-gray-900 dark:text-white">Quick Actions</h3>
      </div>
      <div class="p-6">
        <div class="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-4">
          <button 
            @click="$emit('navigate', 'patients')"
            class="flex items-center p-4 transition-colors border border-gray-200 rounded-lg dark:border-gray-700 hover:bg-gray-50 dark:hover:bg-gray-700"
          >
            <PlusIcon class="w-5 h-5 mr-3 text-purple-600" />
            <span class="text-sm font-medium text-gray-900 dark:text-white">Add Patient</span>
          </button>
          
          <button 
            @click="$emit('navigate', 'appointments')"
            class="flex items-center p-4 transition-colors border border-gray-200 rounded-lg dark:border-gray-700 hover:bg-gray-50 dark:hover:bg-gray-700"
          >
            <CalendarIcon class="w-5 h-5 mr-3 text-blue-600" />
            <span class="text-sm font-medium text-gray-900 dark:text-white">Schedule Appointment</span>
          </button>
          
          <button 
            @click="$emit('navigate', 'analysis')"
            class="flex items-center p-4 transition-colors border border-gray-200 rounded-lg dark:border-gray-700 hover:bg-gray-50 dark:hover:bg-gray-700"
          >
            <ChartBarIcon class="w-5 h-5 mr-3 text-green-600" />
            <span class="text-sm font-medium text-gray-900 dark:text-white">Run AI Analysis</span>
          </button>
          
          <button 
            @click="$emit('navigate', 'billing')"
            class="flex items-center p-4 transition-colors border border-gray-200 rounded-lg dark:border-gray-700 hover:bg-gray-50 dark:hover:bg-gray-700"
          >
            <CreditCardIcon class="w-5 h-5 mr-3 text-orange-600" />
            <span class="text-sm font-medium text-gray-900 dark:text-white">View Billing</span>
          </button>
        </div>
      </div>
    </div>

    <!-- Today's Schedule -->
    <div class="bg-white border border-gray-200 rounded-lg shadow-sm dark:bg-gray-800 dark:border-gray-700">
      <div class="px-6 py-4 border-b border-gray-200 dark:border-gray-700">
        <h3 class="text-lg font-medium text-gray-900 dark:text-white">Today's Schedule</h3>
      </div>
      <div class="p-6">
        <div v-if="patientsStore.todayAppointments.length === 0" class="py-8 text-center">
          <CalendarIcon class="w-12 h-12 mx-auto text-gray-400" />
          <h3 class="mt-2 text-sm font-medium text-gray-900 dark:text-white">No appointments today</h3>
          <p class="mt-1 text-sm text-gray-500 dark:text-gray-400">Your schedule is clear for today.</p>
        </div>
        
        <div v-else class="space-y-4">
          <div 
            v-for="appointment in patientsStore.todayAppointments.slice(0, 5)" 
            :key="appointment.id"
            class="flex items-center justify-between p-4 border border-gray-200 rounded-lg dark:border-gray-700"
          >
            <div class="flex items-center space-x-4">
              <div class="flex-shrink-0">
                <div class="flex items-center justify-center w-10 h-10 bg-purple-100 rounded-full dark:bg-purple-900">
                  <span class="text-sm font-medium text-purple-600 dark:text-purple-300">
                    {{ appointment.patientName.split(' ').map(n => n[0]).join('') }}
                  </span>
                </div>
              </div>
              <div>
                <p class="text-sm font-medium text-gray-900 dark:text-white">{{ appointment.patientName }}</p>
                <p class="text-sm text-gray-500 dark:text-gray-400">{{ appointment.appointmentType }} - {{ appointment.department }}</p>
              </div>
            </div>
            <div class="text-right">
              <p class="text-sm font-medium text-gray-900 dark:text-white">{{ formatAppointmentTime(appointment.dateTime) }}</p>
              <span 
                class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium"
                :class="getAppointmentStatusClass(appointment.status)"
              >
                {{ appointment.status }}
              </span>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useAuthStore } from '../../stores/auth'
import { usePatientsStore } from '../../stores/patients'
import { useAnalysisStore } from '../../stores/analysis'

// Define props
interface Props {
  selectedDepartment?: string | null
}

const props = defineProps<Props>()

// Define emits
const emit = defineEmits(['navigate'])

// Stores
const authStore = useAuthStore()
const patientsStore = usePatientsStore()
const analysisStore = useAnalysisStore()

// Local state
const isRefreshing = ref(false)

// Computed properties
const upcomingAppointments = computed(() => {
  const now = new Date()
  return patientsStore.todayAppointments.filter(apt => new Date(apt.dateTime) > now).length
})

const completedToday = computed(() => {
  const today = new Date().toDateString()
  return analysisStore.completedAnalyses.filter(analysis => 
    new Date(analysis.completedAt || '').toDateString() === today
  ).length
})

const recentActivity = computed(() => [
  {
    id: '1',
    type: 'patient',
    title: 'New patient registered',
    description: 'John Doe was added to Cardiology department',
    timestamp: new Date(Date.now() - 30 * 60 * 1000).toISOString()
  },
  {
    id: '2',
    type: 'analysis',
    title: 'AI Analysis completed',
    description: 'Tumor analysis for Jane Smith shows benign results',
    timestamp: new Date(Date.now() - 2 * 60 * 60 * 1000).toISOString()
  },
  {
    id: '3',
    type: 'appointment',
    title: 'Appointment scheduled',
    description: 'Follow-up appointment for Mike Johnson',
    timestamp: new Date(Date.now() - 4 * 60 * 60 * 1000).toISOString()
  }
])

// Icon components
const UsersIcon = {
  template: `<svg fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" d="M15 19.128a9.38 9.38 0 002.625.372 9.337 9.337 0 004.121-.952 4.125 4.125 0 00-7.533-2.493M15 19.128v-.003c0-1.113-.285-2.16-.786-3.07M15 19.128v.106A12.318 12.318 0 018.624 21c-2.331 0-4.512-.645-6.374-1.766l-.001-.109a6.375 6.375 0 0111.964-3.07M12 6.375a3.375 3.375 0 11-6.75 0 3.375 3.375 0 016.75 0zm8.25 2.25a2.625 2.625 0 11-5.25 0 2.625 2.625 0 015.25 0z" /></svg>`
}

const CalendarIcon = {
  template: `<svg fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" d="M6.75 3v2.25M17.25 3v2.25M3 18.75V7.5a2.25 2.25 0 012.25-2.25h13.5A2.25 2.25 0 0121 7.5v11.25m-18 0A2.25 2.25 0 005.25 21h13.5A2.25 2.25 0 0021 18.75m-18 0v-7.5A2.25 2.25 0 015.25 9h13.5A2.25 2.25 0 0121 11.25v7.5" /></svg>`
}

const ChartBarIcon = {
  template: `<svg fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" d="M3 13.125C3 12.504 3.504 12 4.125 12h2.25c.621 0 1.125.504 1.125 1.125v6.75C7.5 20.496 6.996 21 6.375 21h-2.25A1.125 1.125 0 013 19.875v-6.75zM9.75 8.625c0-.621.504-1.125 1.125-1.125h2.25c.621 0 1.125.504 1.125 1.125v11.25c0 .621-.504 1.125-1.125 1.125h-2.25a1.125 1.125 0 01-1.125-1.125V8.625zM16.5 4.125c0-.621.504-1.125 1.125-1.125h2.25C20.496 3 21 3.504 21 4.125v15.75c0 .621-.504 1.125-1.125 1.125h-2.25a1.125 1.125 0 01-1.125-1.125V4.125z" /></svg>`
}

const ExclamationTriangleIcon = {
  template: `<svg fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" d="M12 9v3.75m-9.303 3.376c-.866 1.5.217 3.374 1.948 3.374h14.71c1.73 0 2.813-1.874 1.948-3.374L13.949 3.378c-.866-1.5-3.032-1.5-3.898 0L2.697 16.126zM12 15.75h.007v.008H12v-.008z" /></svg>`
}

const ArrowUpIcon = {
  template: `<svg fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" d="M4.5 10.5L12 3m0 0l7.5 7.5M12 3v18" /></svg>`
}

const PlusIcon = {
  template: `<svg fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" d="M12 4.5v15m7.5-7.5h-15" /></svg>`
}

const CreditCardIcon = {
  template: `<svg fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" d="M2.25 8.25h19.5M2.25 9h19.5m-16.5 5.25h6m-6 2.25h3m-3.75 3h15a2.25 2.25 0 002.25-2.25V6.75A2.25 2.25 0 0019.5 4.5h-15a2.25 2.25 0 00-2.25 2.25v10.5A2.25 2.25 0 004.5 19.5z" /></svg>`
}

// Methods
const refreshData = async () => {
  isRefreshing.value = true
  try {
    await Promise.all([
      patientsStore.fetchPatients(),
      patientsStore.fetchAppointments(),
      analysisStore.fetchAnalyses()
    ])
  } catch (error) {
    console.error('Error refreshing data:', error)
  } finally {
    isRefreshing.value = false
  }
}

const getDepartmentColor = (department: string) => {
  const colors: Record<string, string> = {
    cardiology: 'bg-red-500',
    neurology: 'bg-blue-500',
    oncology: 'bg-purple-500',
    radiology: 'bg-green-500',
    emergency: 'bg-orange-500',
    pediatrics: 'bg-pink-500'
  }
  return colors[department] || 'bg-gray-500'
}

const getActivityIcon = (type: string) => {
  const icons: Record<string, any> = {
    patient: UsersIcon,
    analysis: ChartBarIcon,
    appointment: CalendarIcon
  }
  return icons[type] || UsersIcon
}

const formatTime = (timestamp: string) => {
  const now = new Date()
  const time = new Date(timestamp)
  const diffInHours = (now.getTime() - time.getTime()) / (1000 * 60 * 60)
  
  if (diffInHours < 1) {
    return `${Math.floor(diffInHours * 60)} minutes ago`
  } else if (diffInHours < 24) {
    return `${Math.floor(diffInHours)} hours ago`
  } else {
    return time.toLocaleDateString()
  }
}

const formatAppointmentTime = (dateTime: string) => {
  return new Date(dateTime).toLocaleTimeString('en-US', {
    hour: '2-digit',
    minute: '2-digit'
  })
}

const getAppointmentStatusClass = (status: string) => {
  const classes: Record<string, string> = {
    scheduled: 'bg-blue-100 text-blue-800 dark:bg-blue-900 dark:text-blue-200',
    completed: 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-200',
    cancelled: 'bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-200',
    'no-show': 'bg-gray-100 text-gray-800 dark:bg-gray-900 dark:text-gray-200'
  }
  return classes[status] || classes.scheduled
}

onMounted(() => {
  // Load initial data if not already loaded
  if (patientsStore.patients.length === 0) {
    refreshData()
  }
})
</script>
