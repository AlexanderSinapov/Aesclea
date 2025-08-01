<template>
  <div class="fixed inset-0 bg-gray-600 bg-opacity-50 overflow-y-auto h-full w-full z-50">
    <div class="relative top-20 mx-auto p-5 border w-11/12 max-w-3xl shadow-lg rounded-md bg-white dark:bg-gray-800">
      <!-- Header -->
      <div class="flex items-center justify-between pb-4 border-b border-gray-200 dark:border-gray-700">
        <div>
          <h3 class="text-lg font-medium text-gray-900 dark:text-white">
            Appointment Details
          </h3>
          <p class="text-sm text-gray-500 dark:text-gray-400">
            {{ formatDateTime(appointment.dateTime) }}
          </p>
        </div>
        <button
          @click="$emit('close')"
          class="text-gray-400 hover:text-gray-600 dark:hover:text-gray-300"
        >
          <XMarkIcon class="w-5 h-5" />
        </button>
      </div>

      <!-- Content -->
      <div class="mt-6 space-y-6">
        <!-- Status and Priority -->
        <div class="flex items-center justify-between">
          <div class="flex items-center space-x-4">
            <span 
              class="inline-flex items-center px-3 py-1 rounded-full text-sm font-medium"
              :class="getStatusBadgeClass(appointment.status)"
            >
              {{ appointment.status }}
            </span>
            <span 
              class="inline-flex items-center px-3 py-1 rounded-full text-sm font-medium"
              :class="getPriorityBadgeClass(appointment.priority)"
            >
              {{ appointment.priority }} priority
            </span>
          </div>
          <div class="flex space-x-3">
            <button
              v-if="appointment.status === 'scheduled'"
              @click="$emit('edit', appointment)"
              class="inline-flex items-center px-3 py-2 border border-transparent text-sm leading-4 font-medium rounded-md text-white bg-blue-600 hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500"
            >
              <PencilIcon class="w-4 h-4 mr-1" />
              Edit
            </button>
            <button
              v-if="appointment.status === 'scheduled'"
              @click="markCompleted"
              class="inline-flex items-center px-3 py-2 border border-transparent text-sm leading-4 font-medium rounded-md text-white bg-green-600 hover:bg-green-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-green-500"
            >
              <CheckIcon class="w-4 h-4 mr-1" />
              Mark Complete
            </button>
          </div>
        </div>

        <!-- Patient Information -->
        <div class="bg-gray-50 dark:bg-gray-900 rounded-lg p-4">
          <h4 class="text-sm font-medium text-gray-900 dark:text-white mb-3">Patient Information</h4>
          <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div>
              <div class="text-sm text-gray-500 dark:text-gray-400">Patient Name</div>
              <div class="text-sm font-medium text-gray-900 dark:text-white">{{ appointment.patientName }}</div>
            </div>
            <div>
              <div class="text-sm text-gray-500 dark:text-gray-400">Patient ID</div>
              <div class="text-sm font-medium text-gray-900 dark:text-white">{{ appointment.patientId }}</div>
            </div>
          </div>
        </div>

        <!-- Appointment Information -->
        <div class="bg-white dark:bg-gray-800 border border-gray-200 dark:border-gray-700 rounded-lg p-4">
          <h4 class="text-sm font-medium text-gray-900 dark:text-white mb-3">Appointment Information</h4>
          <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div>
              <div class="text-sm text-gray-500 dark:text-gray-400">Department</div>
              <div class="text-sm font-medium text-gray-900 dark:text-white capitalize">{{ appointment.department }}</div>
            </div>
            <div>
              <div class="text-sm text-gray-500 dark:text-gray-400">Appointment Type</div>
              <div class="text-sm font-medium text-gray-900 dark:text-white">{{ appointment.appointmentType }}</div>
            </div>
            <div>
              <div class="text-sm text-gray-500 dark:text-gray-400">Doctor</div>
              <div class="text-sm font-medium text-gray-900 dark:text-white">{{ appointment.doctor }}</div>
            </div>
            <div>
              <div class="text-sm text-gray-500 dark:text-gray-400">Duration</div>
              <div class="text-sm font-medium text-gray-900 dark:text-white">{{ appointment.duration }} minutes</div>
            </div>
            <div>
              <div class="text-sm text-gray-500 dark:text-gray-400">Date & Time</div>
              <div class="text-sm font-medium text-gray-900 dark:text-white">{{ formatDateTime(appointment.dateTime) }}</div>
            </div>
          </div>
        </div>

        <!-- Reason and Notes -->
        <div v-if="appointment.reason || appointment.notes" class="space-y-4">
          <div v-if="appointment.reason" class="bg-blue-50 dark:bg-blue-900/20 rounded-lg p-4">
            <h4 class="text-sm font-medium text-gray-900 dark:text-white mb-2">Reason for Visit</h4>
            <p class="text-sm text-gray-700 dark:text-gray-300">{{ appointment.reason }}</p>
          </div>
          
          <div v-if="appointment.notes" class="bg-yellow-50 dark:bg-yellow-900/20 rounded-lg p-4">
            <h4 class="text-sm font-medium text-gray-900 dark:text-white mb-2">Additional Notes</h4>
            <p class="text-sm text-gray-700 dark:text-gray-300">{{ appointment.notes }}</p>
          </div>
        </div>

        <!-- Actions Section -->
        <div v-if="appointment.status === 'scheduled'" class="border-t border-gray-200 dark:border-gray-700 pt-6">
          <h4 class="text-sm font-medium text-gray-900 dark:text-white mb-3">Quick Actions</h4>
          <div class="grid grid-cols-1 md:grid-cols-3 gap-3">
            <button
              @click="sendReminder"
              class="inline-flex items-center justify-center px-4 py-2 border border-gray-300 dark:border-gray-600 text-sm font-medium rounded-md text-gray-700 dark:text-gray-300 bg-white dark:bg-gray-700 hover:bg-gray-50 dark:hover:bg-gray-600 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-gray-500"
            >
              <BellIcon class="w-4 h-4 mr-2" />
              Send Reminder
            </button>
            
            <button
              @click="reschedule"
              class="inline-flex items-center justify-center px-4 py-2 border border-gray-300 dark:border-gray-600 text-sm font-medium rounded-md text-gray-700 dark:text-gray-300 bg-white dark:bg-gray-700 hover:bg-gray-50 dark:hover:bg-gray-600 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-gray-500"
            >
              <CalendarIcon class="w-4 h-4 mr-2" />
              Reschedule
            </button>
            
            <button
              @click="cancel"
              class="inline-flex items-center justify-center px-4 py-2 border border-red-300 dark:border-red-600 text-sm font-medium rounded-md text-red-700 dark:text-red-300 bg-white dark:bg-gray-700 hover:bg-red-50 dark:hover:bg-red-900/20 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-red-500"
            >
              <XCircleIcon class="w-4 h-4 mr-2" />
              Cancel
            </button>
          </div>
        </div>

        <!-- Completed Appointment Summary -->
        <div v-if="appointment.status === 'completed'" class="bg-green-50 dark:bg-green-900/20 border border-green-200 dark:border-green-800 rounded-lg p-4">
          <div class="flex items-center">
            <CheckCircleIcon class="w-5 h-5 text-green-500 mr-2" />
            <h4 class="text-sm font-medium text-green-800 dark:text-green-200">Appointment Completed</h4>
          </div>
          <p class="text-sm text-green-700 dark:text-green-300 mt-1">
            This appointment was successfully completed. Check patient records for visit notes and follow-up instructions.
          </p>
        </div>

        <!-- Cancelled/Rescheduled Appointment Info -->
        <div v-if="appointment.status === 'cancelled' || appointment.status === 'rescheduled'" class="bg-red-50 dark:bg-red-900/20 border border-red-200 dark:border-red-800 rounded-lg p-4">
          <div class="flex items-center">
            <XCircleIcon class="w-5 h-5 text-red-500 mr-2" />
            <h4 class="text-sm font-medium text-red-800 dark:text-red-200">
              {{ appointment.status === 'cancelled' ? 'Appointment Cancelled' : 'Appointment Rescheduled' }}
            </h4>
          </div>
          <p class="text-sm text-red-700 dark:text-red-300 mt-1">
            {{ appointment.status === 'cancelled' 
              ? 'This appointment was cancelled.' 
              : 'This appointment was rescheduled.' }}
          </p>
        </div>
      </div>

      <!-- Footer Actions -->
      <div class="flex justify-end space-x-3 pt-6 mt-6 border-t border-gray-200 dark:border-gray-700">
        <button
          @click="$emit('close')"
          class="px-4 py-2 text-sm font-medium text-gray-700 dark:text-gray-300 bg-white dark:bg-gray-700 border border-gray-300 dark:border-gray-600 rounded-md hover:bg-gray-50 dark:hover:bg-gray-600 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-gray-500"
        >
          Close
        </button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import type { Appointment } from '../../stores/appointments'

interface Props {
  appointment: Appointment
}

const props = defineProps<Props>()
const emit = defineEmits<{
  close: []
  edit: [appointment: Appointment]
}>()

// Computed properties
// Icon components
const XMarkIcon = {
  template: `<svg fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" d="M6 18L18 6M6 6l12 12" /></svg>`
}

const PencilIcon = {
  template: `<svg fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" d="M16.862 4.487l1.687-1.688a1.875 1.875 0 112.652 2.652L10.582 16.07a4.5 4.5 0 01-1.897 1.13L6 18l.8-2.685a4.5 4.5 0 011.13-1.897l8.932-8.931zm0 0L19.5 7.125M18 14v4.75A2.25 2.25 0 0115.75 21H5.25A2.25 2.25 0 013 18.75V8.25A2.25 2.25 0 015.25 6H10" /></svg>`
}

const CheckIcon = {
  template: `<svg fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" d="M4.5 12.75l6 6 9-13.5" /></svg>`
}

const CheckCircleIcon = {
  template: `<svg fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" d="M9 12.75L11.25 15 15 9.75M21 12a9 9 0 11-18 0 9 9 0 0118 0z" /></svg>`
}

const BellIcon = {
  template: `<svg fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" d="M14.857 17.082a23.848 23.848 0 005.454-1.31A8.967 8.967 0 0118 9.75v-.7V9A6 6 0 006 9v.75a8.967 8.967 0 01-2.312 6.022c1.733.64 3.56 1.085 5.455 1.31m5.714 0a24.255 24.255 0 01-5.714 0m5.714 0a3 3 0 11-5.714 0" /></svg>`
}

const CalendarIcon = {
  template: `<svg fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" d="M6.75 3v2.25M17.25 3v2.25M3 18.75V7.5a2.25 2.25 0 012.25-2.25h13.5A2.25 2.25 0 0121 7.5v11.25m-18 0A2.25 2.25 0 005.25 21h13.5A2.25 2.25 0 0021 18.75m-18 0v-7.5A2.25 2.25 0 015.25 9h13.5A2.25 2.25 0 0121 11.25v7.5" /></svg>`
}

const XCircleIcon = {
  template: `<svg fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" d="M9.75 9.75l4.5 4.5m0-4.5l-4.5 4.5M21 12a9 9 0 11-18 0 9 9 0 0118 0z" /></svg>`
}

// Methods
const formatDateTime = (dateTime: string) => {
  return new Date(dateTime).toLocaleString('en-US', {
    weekday: 'long',
    year: 'numeric',
    month: 'long',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  })
}

const getStatusBadgeClass = (status: string) => {
  const classes: Record<string, string> = {
    scheduled: 'bg-blue-100 text-blue-800 dark:bg-blue-900 dark:text-blue-200',
    completed: 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-200',
    cancelled: 'bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-200',
    'no-show': 'bg-gray-100 text-gray-800 dark:bg-gray-900 dark:text-gray-200'
  }
  return classes[status] || classes.scheduled
}

const getPriorityBadgeClass = (priority: string) => {
  const classes: Record<string, string> = {
    normal: 'bg-gray-100 text-gray-800 dark:bg-gray-900 dark:text-gray-200',
    urgent: 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900 dark:text-yellow-200',
    emergency: 'bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-200'
  }
  return classes[priority] || classes.normal
}

const markCompleted = () => {
  // Update appointment status
  props.appointment.status = 'completed'
  alert('Appointment marked as completed!')
  emit('close')
}

const sendReminder = () => {
  alert(`Reminder sent to ${props.appointment.patientName}`)
}

const reschedule = () => {
  emit('edit', props.appointment)
}

const cancel = () => {
  if (confirm(`Are you sure you want to cancel the appointment for ${props.appointment.patientName}?`)) {
    props.appointment.status = 'cancelled'
    alert('Appointment cancelled!')
    emit('close')
  }
}
</script>
