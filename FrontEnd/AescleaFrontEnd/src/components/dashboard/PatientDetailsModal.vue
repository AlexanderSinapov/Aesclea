<template>
  <div class="fixed inset-0 z-[9999] overflow-y-auto">
    <div class="flex items-end justify-center min-h-screen pt-4 px-4 pb-20 text-center sm:block sm:p-0">
      <!-- Background overlay with blur effect -->
      <div 
        class="fixed inset-0 backdrop-blur-sm bg-black/20 transition-opacity" 
        @click="$emit('close')"
      ></div>

      <!-- Modal panel -->
      <div class="relative inline-block align-bottom bg-white dark:bg-gray-800 rounded-lg text-left overflow-hidden shadow-xl transform transition-all sm:my-8 sm:align-middle sm:max-w-4xl sm:w-full z-10">
        <!-- Header -->
        <div class="bg-white dark:bg-gray-800 px-4 pt-5 pb-4 sm:p-6 border-b border-gray-200 dark:border-gray-700">
          <div class="flex items-center justify-between">
            <div class="flex items-center space-x-4">
              <div class="flex-shrink-0 h-16 w-16">
                <div class="h-16 w-16 rounded-full bg-purple-100 dark:bg-purple-900 flex items-center justify-center">
                  <span class="text-xl font-medium text-purple-600 dark:text-purple-300">
                    {{ patient?.firstName.charAt(0) }}{{ patient?.lastName.charAt(0) }}
                  </span>
                </div>
              </div>
              <div>
                <h3 class="text-2xl font-bold text-gray-900 dark:text-white">
                  {{ patient?.firstName }} {{ patient?.lastName }}
                </h3>
                <div class="flex items-center space-x-4 mt-1">
                  <span 
                    class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium"
                    :class="getStatusBadgeClass(patient?.status || 'active')"
                  >
                    {{ patient?.status }}
                  </span>
                  <span 
                    v-if="patient?.department"
                    class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium capitalize"
                    :class="getDepartmentBadgeClass(patient?.department)"
                  >
                    {{ patient?.department }}
                  </span>
                </div>
              </div>
            </div>
            <div class="flex items-center space-x-2">
              <button
                @click="$emit('edit', patient)"
                class="inline-flex items-center px-3 py-2 border border-transparent text-sm font-medium rounded-md text-white bg-purple-600 hover:bg-purple-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-purple-500"
              >
                <PencilIcon class="w-4 h-4 mr-2" />
                Edit
              </button>
              <button
                @click="$emit('close')"
                class="text-gray-400 hover:text-gray-500 dark:hover:text-gray-300"
              >
                <XMarkIcon class="h-6 w-6" />
              </button>
            </div>
          </div>
        </div>

        <!-- Content -->
        <div class="px-4 py-6 sm:px-6">
          <div class="grid grid-cols-1 lg:grid-cols-3 gap-6">
            <!-- Patient Information -->
            <div class="lg:col-span-2 space-y-6">
              <!-- Basic Information -->
              <div class="bg-gray-50 dark:bg-gray-700 rounded-lg p-6">
                <h4 class="text-lg font-medium text-gray-900 dark:text-white mb-4">Basic Information</h4>
                <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
                  <div>
                    <dt class="text-sm font-medium text-gray-500 dark:text-gray-400">Email</dt>
                    <dd class="mt-1 text-sm text-gray-900 dark:text-white">{{ patient?.email }}</dd>
                  </div>
                  <div>
                    <dt class="text-sm font-medium text-gray-500 dark:text-gray-400">Phone</dt>
                    <dd class="mt-1 text-sm text-gray-900 dark:text-white">{{ patient?.phone }}</dd>
                  </div>
                  <div>
                    <dt class="text-sm font-medium text-gray-500 dark:text-gray-400">Date of Birth</dt>
                    <dd class="mt-1 text-sm text-gray-900 dark:text-white">
                      {{ formatDate(patient?.dateOfBirth) }} ({{ calculateAge(patient?.dateOfBirth) }} years old)
                    </dd>
                  </div>
                  <div>
                    <dt class="text-sm font-medium text-gray-500 dark:text-gray-400">Gender</dt>
                    <dd class="mt-1 text-sm text-gray-900 dark:text-white capitalize">{{ patient?.gender }}</dd>
                  </div>
                  <div>
                    <dt class="text-sm font-medium text-gray-500 dark:text-gray-400">Patient Since</dt>
                    <dd class="mt-1 text-sm text-gray-900 dark:text-white">{{ formatDate(patient?.createdAt) }}</dd>
                  </div>
                  <div>
                    <dt class="text-sm font-medium text-gray-500 dark:text-gray-400">Last Visit</dt>
                    <dd class="mt-1 text-sm text-gray-900 dark:text-white">{{ formatDate(patient?.lastVisit) }}</dd>
                  </div>
                </div>
              </div>

              <!-- Medical History -->
              <div class="bg-gray-50 dark:bg-gray-700 rounded-lg p-6">
                <h4 class="text-lg font-medium text-gray-900 dark:text-white mb-4">Medical History</h4>
                <div class="text-sm text-gray-700 dark:text-gray-300">
                  {{ patient?.medicalHistory || 'No medical history available.' }}
                </div>
              </div>

              <!-- Recent Appointments -->
              <div class="bg-gray-50 dark:bg-gray-700 rounded-lg p-6">
                <div class="flex items-center justify-between mb-4">
                  <h4 class="text-lg font-medium text-gray-900 dark:text-white">Recent Appointments</h4>
                  <button
                    @click="scheduleAppointment"
                    class="text-sm text-purple-600 hover:text-purple-700 dark:text-purple-400 dark:hover:text-purple-300"
                  >
                    Schedule New
                  </button>
                </div>
                <div v-if="patientAppointments.length === 0" class="text-sm text-gray-500 dark:text-gray-400">
                  No appointments scheduled.
                </div>
                <div v-else class="space-y-3">
                  <div 
                    v-for="appointment in patientAppointments.slice(0, 3)" 
                    :key="appointment.id"
                    class="flex items-center justify-between p-3 bg-white dark:bg-gray-800 rounded-lg border border-gray-200 dark:border-gray-600"
                  >
                    <div>
                      <p class="text-sm font-medium text-gray-900 dark:text-white">{{ appointment.appointmentType }}</p>
                      <p class="text-sm text-gray-500 dark:text-gray-400">{{ formatDateTime(appointment.dateTime) }}</p>
                    </div>
                    <span 
                      class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium"
                      :class="getAppointmentStatusClass(appointment.status)"
                    >
                      {{ appointment.status }}
                    </span>
                  </div>
                </div>
              </div>

              <!-- AI Analysis Results -->
              <div class="bg-gray-50 dark:bg-gray-700 rounded-lg p-6">
                <div class="flex items-center justify-between mb-4">
                  <h4 class="text-lg font-medium text-gray-900 dark:text-white">AI Analysis Results</h4>
                  <button
                    @click="requestAnalysis"
                    class="text-sm text-purple-600 hover:text-purple-700 dark:text-purple-400 dark:hover:text-purple-300"
                  >
                    Request Analysis
                  </button>
                </div>
                <div v-if="patientAnalyses.length === 0" class="text-sm text-gray-500 dark:text-gray-400">
                  No analysis results available.
                </div>
                <div v-else class="space-y-3">
                  <div 
                    v-for="analysis in patientAnalyses.slice(0, 3)" 
                    :key="analysis.id"
                    class="flex items-center justify-between p-3 bg-white dark:bg-gray-800 rounded-lg border border-gray-200 dark:border-gray-600"
                  >
                    <div>
                      <p class="text-sm font-medium text-gray-900 dark:text-white capitalize">{{ analysis.analysisType.replace('-', ' ') }}</p>
                      <p class="text-sm text-gray-500 dark:text-gray-400">{{ formatDateTime(analysis.createdAt) }}</p>
                    </div>
                    <div class="flex items-center space-x-2">
                      <span 
                        v-if="analysis.confidence"
                        class="text-xs text-gray-500 dark:text-gray-400"
                      >
                        {{ Math.round(analysis.confidence * 100) }}% confidence
                      </span>
                      <span 
                        class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium"
                        :class="getAnalysisStatusClass(analysis.status)"
                      >
                        {{ analysis.status }}
                      </span>
                    </div>
                  </div>
                </div>
              </div>
            </div>

            <!-- Quick Actions Sidebar -->
            <div class="space-y-6">
              <!-- Quick Actions -->
              <div class="bg-gray-50 dark:bg-gray-700 rounded-lg p-6">
                <h4 class="text-lg font-medium text-gray-900 dark:text-white mb-4">Quick Actions</h4>
                <div class="space-y-3">
                  <button
                    @click="scheduleAppointment"
                    class="w-full flex items-center px-3 py-2 text-sm font-medium text-gray-700 dark:text-gray-300 bg-white dark:bg-gray-800 border border-gray-300 dark:border-gray-600 rounded-md hover:bg-gray-50 dark:hover:bg-gray-700"
                  >
                    <CalendarIcon class="w-4 h-4 mr-2" />
                    Schedule Appointment
                  </button>
                  <button
                    @click="requestAnalysis"
                    class="w-full flex items-center px-3 py-2 text-sm font-medium text-gray-700 dark:text-gray-300 bg-white dark:bg-gray-800 border border-gray-300 dark:border-gray-600 rounded-md hover:bg-gray-50 dark:hover:bg-gray-700"
                  >
                    <ChartBarIcon class="w-4 h-4 mr-2" />
                    Request AI Analysis
                  </button>
                  <button
                    @click="viewMedicalRecords"
                    class="w-full flex items-center px-3 py-2 text-sm font-medium text-gray-700 dark:text-gray-300 bg-white dark:bg-gray-800 border border-gray-300 dark:border-gray-600 rounded-md hover:bg-gray-50 dark:hover:bg-gray-700"
                  >
                    <DocumentTextIcon class="w-4 h-4 mr-2" />
                    Medical Records
                  </button>
                  <button
                    @click="sendMessage"
                    class="w-full flex items-center px-3 py-2 text-sm font-medium text-gray-700 dark:text-gray-300 bg-white dark:bg-gray-800 border border-gray-300 dark:border-gray-600 rounded-md hover:bg-gray-50 dark:hover:bg-gray-700"
                  >
                    <EnvelopeIcon class="w-4 h-4 mr-2" />
                    Send Message
                  </button>
                </div>
              </div>

              <!-- Emergency Contacts -->
              <div class="bg-gray-50 dark:bg-gray-700 rounded-lg p-6">
                <h4 class="text-lg font-medium text-gray-900 dark:text-white mb-4">Emergency Contacts</h4>
                <div class="text-sm text-gray-500 dark:text-gray-400">
                  No emergency contacts on file.
                </div>
              </div>

              <!-- Insurance Information -->
              <div class="bg-gray-50 dark:bg-gray-700 rounded-lg p-6">
                <h4 class="text-lg font-medium text-gray-900 dark:text-white mb-4">Insurance</h4>
                <div class="text-sm text-gray-500 dark:text-gray-400">
                  No insurance information available.
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { usePatientsStore } from '../../stores/patients'
import { useAnalysisStore } from '../../stores/analysis'
import type { Patient } from '../../stores/patients'

// Define props
interface Props {
  patient?: Patient | null
}

const props = defineProps<Props>()

// Define emits
const emit = defineEmits(['close', 'edit'])

// Stores
const patientsStore = usePatientsStore()
const analysisStore = useAnalysisStore()

// Computed properties
const patientAppointments = computed(() => {
  if (!props.patient) return []
  return patientsStore.appointments.filter(apt => apt.patientId === props.patient?.id)
})

const patientAnalyses = computed(() => {
  if (!props.patient) return []
  return analysisStore.analyses.filter(analysis => analysis.patientId === props.patient?.id)
})

// Icon components
const XMarkIcon = {
  template: `<svg fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" d="M6 18L18 6M6 6l12 12" /></svg>`
}

const PencilIcon = {
  template: `<svg fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" d="m16.862 4.487 1.687-1.688a1.875 1.875 0 1 1 2.652 2.652L10.582 16.07a4.5 4.5 0 0 1-1.897 1.13L6 18l.8-2.685a4.5 4.5 0 0 1 1.13-1.897l8.932-8.931Zm0 0L19.5 7.125M18 14v4.75A2.25 2.25 0 0 1 15.75 21H5.25A2.25 2.25 0 0 1 3 18.75V8.25A2.25 2.25 0 0 1 5.25 6H10" /></svg>`
}

const CalendarIcon = {
  template: `<svg fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" d="M6.75 3v2.25M17.25 3v2.25M3 18.75V7.5a2.25 2.25 0 012.25-2.25h13.5A2.25 2.25 0 0121 7.5v11.25m-18 0A2.25 2.25 0 005.25 21h13.5A2.25 2.25 0 0021 18.75m-18 0v-7.5A2.25 2.25 0 015.25 9h13.5A2.25 2.25 0 0121 11.25v7.5" /></svg>`
}

const ChartBarIcon = {
  template: `<svg fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" d="M3 13.125C3 12.504 3.504 12 4.125 12h2.25c.621 0 1.125.504 1.125 1.125v6.75C7.5 20.496 6.996 21 6.375 21h-2.25A1.125 1.125 0 013 19.875v-6.75zM9.75 8.625c0-.621.504-1.125 1.125-1.125h2.25c.621 0 1.125.504 1.125 1.125v11.25c0 .621-.504 1.125-1.125 1.125h-2.25a1.125 1.125 0 01-1.125-1.125V8.625zM16.5 4.125c0-.621.504-1.125 1.125-1.125h2.25C20.496 3 21 3.504 21 4.125v15.75c0 .621-.504 1.125-1.125 1.125h-2.25a1.125 1.125 0 01-1.125-1.125V4.125z" /></svg>`
}

const DocumentTextIcon = {
  template: `<svg fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" d="M19.5 14.25v-2.625a3.375 3.375 0 0 0-3.375-3.375h-1.5A1.125 1.125 0 0 1 13.5 7.125v-1.5a3.375 3.375 0 0 0-3.375-3.375H8.25m2.25 0H5.625c-.621 0-1.125.504-1.125 1.125v17.25c0 .621.504 1.125 1.125 1.125h12.75c.621 0 1.125-.504 1.125-1.125V11.25a9 9 0 0 0-9-9Z" /></svg>`
}

const EnvelopeIcon = {
  template: `<svg fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" d="M21.75 6.75v10.5a2.25 2.25 0 0 1-2.25 2.25h-15a2.25 2.25 0 0 1-2.25-2.25V6.75m19.5 0A2.25 2.25 0 0 0 19.5 4.5h-15a2.25 2.25 0 0 0-2.25 2.25m19.5 0v.243a2.25 2.25 0 0 1-1.07 1.916l-7.5 4.615a2.25 2.25 0 0 1-2.36 0L3.32 8.91a2.25 2.25 0 0 1-1.07-1.916V6.75" /></svg>`
}

// Methods
const calculateAge = (dateOfBirth?: string) => {
  if (!dateOfBirth) return 'Unknown'
  const today = new Date()
  const birthDate = new Date(dateOfBirth)
  let age = today.getFullYear() - birthDate.getFullYear()
  const monthDiff = today.getMonth() - birthDate.getMonth()
  if (monthDiff < 0 || (monthDiff === 0 && today.getDate() < birthDate.getDate())) {
    age--
  }
  return age
}

const formatDate = (date?: string) => {
  if (!date) return 'Not specified'
  return new Date(date).toLocaleDateString()
}

const formatDateTime = (dateTime: string) => {
  return new Date(dateTime).toLocaleString()
}

const getStatusBadgeClass = (status: string) => {
  const classes: Record<string, string> = {
    active: 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-200',
    inactive: 'bg-gray-100 text-gray-800 dark:bg-gray-900 dark:text-gray-200',
    critical: 'bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-200'
  }
  return classes[status] || classes.active
}

const getDepartmentBadgeClass = (department?: string) => {
  const classes: Record<string, string> = {
    cardiology: 'bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-200',
    neurology: 'bg-blue-100 text-blue-800 dark:bg-blue-900 dark:text-blue-200',
    oncology: 'bg-purple-100 text-purple-800 dark:bg-purple-900 dark:text-purple-200',
    radiology: 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-200',
    emergency: 'bg-orange-100 text-orange-800 dark:bg-orange-900 dark:text-orange-200',
    pediatrics: 'bg-pink-100 text-pink-800 dark:bg-pink-900 dark:text-pink-200'
  }
  return classes[department || ''] || 'bg-gray-100 text-gray-800 dark:bg-gray-900 dark:text-gray-200'
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

const getAnalysisStatusClass = (status: string) => {
  const classes: Record<string, string> = {
    pending: 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900 dark:text-yellow-200',
    processing: 'bg-blue-100 text-blue-800 dark:bg-blue-900 dark:text-blue-200',
    completed: 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-200',
    failed: 'bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-200'
  }
  return classes[status] || classes.pending
}

const scheduleAppointment = () => {
  console.log('Schedule appointment for', props.patient?.firstName, props.patient?.lastName)
  // Implement navigation to appointment scheduling
}

const requestAnalysis = () => {
  console.log('Request analysis for', props.patient?.firstName, props.patient?.lastName)
  // Implement navigation to analysis request
}

const viewMedicalRecords = () => {
  console.log('View medical records for', props.patient?.firstName, props.patient?.lastName)
  // Implement navigation to medical records
}

const sendMessage = () => {
  console.log('Send message to', props.patient?.firstName, props.patient?.lastName)
  // Implement messaging functionality
}
</script>
