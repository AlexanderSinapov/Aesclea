<template>
  <div class="space-y-6">
    <!-- Page Header -->
    <div class="sm:flex sm:items-center sm:justify-between">
      <div>
        <h1 class="text-2xl font-bold text-gray-900 dark:text-white">Appointments</h1>
        <p class="mt-1 text-sm text-gray-500 dark:text-gray-400">
          Manage patient appointments and scheduling
          <span v-if="selectedDepartment" class="capitalize">
            - {{ selectedDepartment }} Department
          </span>
        </p>
      </div>
      <div class="mt-4 sm:mt-0 flex space-x-3">
        <button
          @click="showScheduleModal = true"
          class="inline-flex items-center px-4 py-2 border border-transparent text-sm font-medium rounded-md shadow-sm text-white bg-purple-600 hover:bg-purple-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-purple-500"
        >
          <PlusIcon class="w-4 h-4 mr-2" />
          Schedule Appointment
        </button>
      </div>
    </div>

    <!-- Calendar View Toggle and Filters -->
    <div class="bg-white dark:bg-gray-800 shadow-sm rounded-lg border border-gray-200 dark:border-gray-700">
      <div class="p-6">
        <div class="flex flex-wrap items-center justify-between gap-4">
          <!-- View Toggle -->
          <div class="flex items-center space-x-2">
            <button
              @click="viewMode = 'list'"
              :class="viewMode === 'list' 
                ? 'bg-purple-100 text-purple-700 dark:bg-purple-900 dark:text-purple-200' 
                : 'text-gray-500 hover:text-gray-700 dark:text-gray-400 dark:hover:text-gray-200'"
              class="px-3 py-2 rounded-md text-sm font-medium"
            >
              <ListBulletIcon class="w-4 h-4 mr-2 inline" />
              List View
            </button>
            <button
              @click="viewMode = 'calendar'"
              :class="viewMode === 'calendar' 
                ? 'bg-purple-100 text-purple-700 dark:bg-purple-900 dark:text-purple-200' 
                : 'text-gray-500 hover:text-gray-700 dark:text-gray-400 dark:hover:text-gray-200'"
              class="px-3 py-2 rounded-md text-sm font-medium"
            >
              <CalendarIcon class="w-4 h-4 mr-2 inline" />
              Calendar View
            </button>
          </div>

          <!-- Filters -->
          <div class="flex items-center space-x-4">
            <select
              v-model="statusFilter"
              class="block px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md bg-white dark:bg-gray-700 text-gray-900 dark:text-white focus:outline-none focus:ring-1 focus:ring-purple-500 focus:border-purple-500 sm:text-sm"
            >
              <option value="">All Statuses</option>
              <option value="scheduled">Scheduled</option>
              <option value="completed">Completed</option>
              <option value="cancelled">Cancelled</option>
              <option value="no-show">No Show</option>
            </select>

            <select
              v-model="departmentFilter"
              class="block px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md bg-white dark:bg-gray-700 text-gray-900 dark:text-white focus:outline-none focus:ring-1 focus:ring-purple-500 focus:border-purple-500 sm:text-sm"
            >
              <option value="">All Departments</option>
              <option value="cardiology">Cardiology</option>
              <option value="neurology">Neurology</option>
              <option value="oncology">Oncology</option>
              <option value="radiology">Radiology</option>
              <option value="emergency">Emergency</option>
              <option value="pediatrics">Pediatrics</option>
            </select>

            <input
              v-model="dateFilter"
              type="date"
              class="block px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md bg-white dark:bg-gray-700 text-gray-900 dark:text-white focus:outline-none focus:ring-1 focus:ring-purple-500 focus:border-purple-500 sm:text-sm"
            />
          </div>
        </div>
      </div>
    </div>

    <!-- Appointments Statistics -->
    <div class="grid grid-cols-1 md:grid-cols-4 gap-6">
      <div class="bg-white dark:bg-gray-800 overflow-hidden shadow-sm rounded-lg border border-gray-200 dark:border-gray-700">
        <div class="p-5">
          <div class="flex items-center">
            <div class="flex-shrink-0">
              <CalendarIcon class="h-6 w-6 text-blue-400" />
            </div>
            <div class="ml-5 w-0 flex-1">
              <dl>
                <dt class="text-sm font-medium text-gray-500 dark:text-gray-400 truncate">Today's Appointments</dt>
                <dd class="text-lg font-medium text-gray-900 dark:text-white">{{ todayAppointments.length }}</dd>
              </dl>
            </div>
          </div>
        </div>
      </div>

      <div class="bg-white dark:bg-gray-800 overflow-hidden shadow-sm rounded-lg border border-gray-200 dark:border-gray-700">
        <div class="p-5">
          <div class="flex items-center">
            <div class="flex-shrink-0">
              <ClockIcon class="h-6 w-6 text-yellow-400" />
            </div>
            <div class="ml-5 w-0 flex-1">
              <dl>
                <dt class="text-sm font-medium text-gray-500 dark:text-gray-400 truncate">Upcoming</dt>
                <dd class="text-lg font-medium text-gray-900 dark:text-white">{{ upcomingAppointments.length }}</dd>
              </dl>
            </div>
          </div>
        </div>
      </div>

      <div class="bg-white dark:bg-gray-800 overflow-hidden shadow-sm rounded-lg border border-gray-200 dark:border-gray-700">
        <div class="p-5">
          <div class="flex items-center">
            <div class="flex-shrink-0">
              <CheckCircleIcon class="h-6 w-6 text-green-400" />
            </div>
            <div class="ml-5 w-0 flex-1">
              <dl>
                <dt class="text-sm font-medium text-gray-500 dark:text-gray-400 truncate">Completed</dt>
                <dd class="text-lg font-medium text-gray-900 dark:text-white">{{ completedAppointments.length }}</dd>
              </dl>
            </div>
          </div>
        </div>
      </div>

      <div class="bg-white dark:bg-gray-800 overflow-hidden shadow-sm rounded-lg border border-gray-200 dark:border-gray-700">
        <div class="p-5">
          <div class="flex items-center">
            <div class="flex-shrink-0">
              <XCircleIcon class="h-6 w-6 text-red-400" />
            </div>
            <div class="ml-5 w-0 flex-1">
              <dl>
                <dt class="text-sm font-medium text-gray-500 dark:text-gray-400 truncate">Cancelled/No Show</dt>
                <dd class="text-lg font-medium text-gray-900 dark:text-white">{{ cancelledAppointments.length }}</dd>
              </dl>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- List View -->
    <div v-if="viewMode === 'list'" class="bg-white dark:bg-gray-800 shadow-sm rounded-lg border border-gray-200 dark:border-gray-700">
      <div class="px-6 py-4 border-b border-gray-200 dark:border-gray-700">
        <h3 class="text-lg font-medium text-gray-900 dark:text-white">
          Appointments ({{ filteredAppointments.length }})
        </h3>
      </div>
      
      <div class="overflow-x-auto">
        <table class="min-w-full divide-y divide-gray-200 dark:divide-gray-700">
          <thead class="bg-gray-50 dark:bg-gray-900">
            <tr>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase tracking-wider">
                Patient & Time
              </th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase tracking-wider">
                Appointment Type
              </th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase tracking-wider">
                Department
              </th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase tracking-wider">
                Doctor
              </th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase tracking-wider">
                Status
              </th>
              <th class="relative px-6 py-3">
                <span class="sr-only">Actions</span>
              </th>
            </tr>
          </thead>
          <tbody class="bg-white dark:bg-gray-800 divide-y divide-gray-200 dark:divide-gray-700">
            <tr v-for="appointment in paginatedAppointments" :key="appointment.id" class="hover:bg-gray-50 dark:hover:bg-gray-700">
              <td class="px-6 py-4 whitespace-nowrap">
                <div class="flex items-center">
                  <div class="flex-shrink-0 h-10 w-10">
                    <div class="h-10 w-10 rounded-full bg-purple-100 dark:bg-purple-900 flex items-center justify-center">
                      <span class="text-sm font-medium text-purple-600 dark:text-purple-300">
                        {{ appointment.patientName.split(' ').map((n: string) => n[0]).join('') }}
                      </span>
                    </div>
                  </div>
                  <div class="ml-4">
                    <div class="text-sm font-medium text-gray-900 dark:text-white">
                      {{ appointment.patientName }}
                    </div>
                    <div class="text-sm text-gray-500 dark:text-gray-400">
                      {{ formatDateTime(appointment.dateTime) }}
                    </div>
                  </div>
                </div>
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <div class="text-sm text-gray-900 dark:text-white">{{ appointment.appointmentType }}</div>
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <span 
                  class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium capitalize"
                  :class="getDepartmentBadgeClass(appointment.department)"
                >
                  {{ appointment.department }}
                </span>
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900 dark:text-white">
                {{ appointment.doctor }}
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <span 
                  class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium"
                  :class="getStatusBadgeClass(appointment.status)"
                >
                  {{ appointment.status }}
                </span>
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                <div class="flex items-center space-x-2">
                  <button
                    @click="viewAppointment(appointment)"
                    class="text-purple-600 hover:text-purple-900 dark:text-purple-400 dark:hover:text-purple-300"
                  >
                    View
                  </button>
                  <button
                    v-if="appointment.status === 'scheduled'"
                    @click="editAppointment(appointment)"
                    class="text-blue-600 hover:text-blue-900 dark:text-blue-400 dark:hover:text-blue-300"
                  >
                    Edit
                  </button>
                  <button
                    v-if="appointment.status === 'scheduled'"
                    @click="cancelAppointment(appointment)"
                    class="text-red-600 hover:text-red-900 dark:text-red-400 dark:hover:text-red-300"
                  >
                    Cancel
                  </button>
                </div>
              </td>
            </tr>
          </tbody>
        </table>
        
        <!-- Empty State -->
        <div v-if="filteredAppointments.length === 0" class="text-center py-12">
          <CalendarIcon class="mx-auto h-12 w-12 text-gray-400" />
          <h3 class="mt-2 text-sm font-medium text-gray-900 dark:text-white">No appointments found</h3>
          <p class="mt-1 text-sm text-gray-500 dark:text-gray-400">
            Get started by scheduling a new appointment.
          </p>
          <div class="mt-6">
            <button
              @click="showScheduleModal = true"
              class="inline-flex items-center px-4 py-2 border border-transparent shadow-sm text-sm font-medium rounded-md text-white bg-purple-600 hover:bg-purple-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-purple-500"
            >
              <PlusIcon class="w-4 h-4 mr-2" />
              Schedule Appointment
            </button>
          </div>
        </div>
      </div>

      <!-- Pagination -->
      <div v-if="filteredAppointments.length > appointmentsPerPage" class="px-6 py-4 border-t border-gray-200 dark:border-gray-700">
        <div class="flex items-center justify-between">
          <div class="text-sm text-gray-700 dark:text-gray-300">
            Showing {{ (currentPage - 1) * appointmentsPerPage + 1 }} to {{ Math.min(currentPage * appointmentsPerPage, filteredAppointments.length) }} of {{ filteredAppointments.length }} results
          </div>
          <div class="flex space-x-2">
            <button
              @click="currentPage--"
              :disabled="currentPage === 1"
              class="px-3 py-1 text-sm font-medium text-gray-500 bg-white border border-gray-300 rounded-md hover:bg-gray-50 disabled:opacity-50 disabled:cursor-not-allowed dark:bg-gray-800 dark:border-gray-600 dark:text-gray-400 dark:hover:bg-gray-700"
            >
              Previous
            </button>
            <button
              @click="currentPage++"
              :disabled="currentPage >= totalPages"
              class="px-3 py-1 text-sm font-medium text-gray-500 bg-white border border-gray-300 rounded-md hover:bg-gray-50 disabled:opacity-50 disabled:cursor-not-allowed dark:bg-gray-800 dark:border-gray-600 dark:text-gray-400 dark:hover:bg-gray-700"
            >
              Next
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Calendar View -->
    <div v-else class="bg-white dark:bg-gray-800 shadow-sm rounded-lg border border-gray-200 dark:border-gray-700">
      <div class="p-6">
        <div class="text-center py-8">
          <CalendarIcon class="mx-auto h-12 w-12 text-gray-400" />
          <h3 class="mt-2 text-sm font-medium text-gray-900 dark:text-white">Calendar View</h3>
          <p class="mt-1 text-sm text-gray-500 dark:text-gray-400">
            Calendar view implementation coming soon.
          </p>
        </div>
      </div>
    </div>

    <!-- Schedule Appointment Modal -->
    <AppointmentModal
      v-if="showScheduleModal || showEditModal"
      :appointment="selectedAppointment"
      :is-edit="showEditModal"
      @close="closeModals"
      @save="handleSaveAppointment"
    />

    <!-- Appointment Details Modal -->
    <AppointmentDetailsModal
      v-if="showDetailsModal && selectedAppointment"
      :appointment="selectedAppointment"
      @close="showDetailsModal = false"
      @edit="editAppointment"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch, onMounted, onUnmounted } from 'vue'
import { useAppointmentsStore, type Appointment } from '../../stores/appointments'
import AppointmentModal from './AppointmentModal.vue'
import AppointmentDetailsModal from './AppointmentDetailsModal.vue'

// Define props
interface Props {
  selectedDepartment?: string | null
}

const props = defineProps<Props>()

// Store
const appointmentsStore = useAppointmentsStore()

// Local state
const viewMode = ref<'list' | 'calendar'>('list')
const statusFilter = ref('')
const departmentFilter = ref('')
const dateFilter = ref('')
const currentPage = ref(1)
const appointmentsPerPage = 20

const showScheduleModal = ref(false)
const showEditModal = ref(false)
const showDetailsModal = ref(false)
const selectedAppointment = ref<Appointment | null>(null)

// Watch for department changes from parent
watch(() => props.selectedDepartment, (newDepartment) => {
  if (newDepartment) {
    departmentFilter.value = newDepartment
  }
}, { immediate: true })

// Computed properties
const filteredAppointments = computed(() => {
  let appointments = appointmentsStore.appointments

  // Apply status filter
  if (statusFilter.value) {
    appointments = appointments.filter((apt: Appointment) => apt.status === statusFilter.value)
  }

  // Apply department filter
  if (departmentFilter.value) {
    appointments = appointments.filter((apt: Appointment) => apt.department === departmentFilter.value)
  }

  // Apply date filter
  if (dateFilter.value) {
    const filterDate = new Date(dateFilter.value).toDateString()
    appointments = appointments.filter((apt: Appointment) => 
      new Date(apt.dateTime).toDateString() === filterDate
    )
  }

  return appointments.sort((a: Appointment, b: Appointment) => new Date(a.dateTime).getTime() - new Date(b.dateTime).getTime())
})

const totalPages = computed(() => Math.ceil(filteredAppointments.value.length / appointmentsPerPage))

const paginatedAppointments = computed(() => {
  const start = (currentPage.value - 1) * appointmentsPerPage
  const end = start + appointmentsPerPage
  return filteredAppointments.value.slice(start, end)
})

const todayAppointments = computed(() => {
  const today = new Date().toDateString()
  return appointmentsStore.appointments.filter((apt: Appointment) => 
    new Date(apt.dateTime).toDateString() === today
  )
})

const upcomingAppointments = computed(() => {
  const now = new Date()
  return appointmentsStore.appointments.filter((apt: Appointment) => 
    new Date(apt.dateTime) > now && apt.status === 'scheduled'
  )
})

const completedAppointments = computed(() => 
  appointmentsStore.appointments.filter((apt: Appointment) => apt.status === 'completed')
)

const cancelledAppointments = computed(() => 
  appointmentsStore.appointments.filter((apt: Appointment) => 
    apt.status === 'cancelled' || apt.status === 'rescheduled'
  )
)

// Icon components
const PlusIcon = {
  template: `<svg fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" d="M12 4.5v15m7.5-7.5h-15" /></svg>`
}

const CalendarIcon = {
  template: `<svg fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" d="M6.75 3v2.25M17.25 3v2.25M3 18.75V7.5a2.25 2.25 0 012.25-2.25h13.5A2.25 2.25 0 0121 7.5v11.25m-18 0A2.25 2.25 0 005.25 21h13.5A2.25 2.25 0 0021 18.75m-18 0v-7.5A2.25 2.25 0 015.25 9h13.5A2.25 2.25 0 0121 11.25v7.5" /></svg>`
}

const ListBulletIcon = {
  template: `<svg fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" d="M8.25 6.75h12M8.25 12h12m-12 5.25h12M3.75 6.75h.007v.008H3.75V6.75zm.375 0a.375.375 0 11-.75 0 .375.375 0 01.75 0zM3.75 12h.007v.008H3.75V12zm.375 0a.375.375 0 11-.75 0 .375.375 0 01.75 0zM3.75 17.25h.007v.008H3.75v-.008zm.375 0a.375.375 0 11-.75 0 .375.375 0 01.75 0z" /></svg>`
}

const ClockIcon = {
  template: `<svg fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" d="M12 6v6h4.5m4.5 0a9 9 0 11-18 0 9 9 0 0118 0z" /></svg>`
}

const CheckCircleIcon = {
  template: `<svg fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" d="M9 12.75L11.25 15 15 9.75M21 12a9 9 0 11-18 0 9 9 0 0118 0z" /></svg>`
}

const XCircleIcon = {
  template: `<svg fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" d="M9.75 9.75l4.5 4.5m0-4.5l-4.5 4.5M21 12a9 9 0 11-18 0 9 9 0 0118 0z" /></svg>`
}

// Methods
const formatDateTime = (dateTime: string) => {
  return new Date(dateTime).toLocaleString()
}

const getDepartmentBadgeClass = (department: string) => {
  const classes: Record<string, string> = {
    cardiology: 'bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-200',
    neurology: 'bg-blue-100 text-blue-800 dark:bg-blue-900 dark:text-blue-200',
    oncology: 'bg-purple-100 text-purple-800 dark:bg-purple-900 dark:text-purple-200',
    radiology: 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-200',
    emergency: 'bg-orange-100 text-orange-800 dark:bg-orange-900 dark:text-orange-200',
    pediatrics: 'bg-pink-100 text-pink-800 dark:bg-pink-900 dark:text-pink-200'
  }
  return classes[department] || 'bg-gray-100 text-gray-800 dark:bg-gray-900 dark:text-gray-200'
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

const viewAppointment = (appointment: Appointment) => {
  selectedAppointment.value = appointment
  showDetailsModal.value = true
}

const editAppointment = (appointment: Appointment) => {
  selectedAppointment.value = appointment
  showEditModal.value = true
  showDetailsModal.value = false
}

const cancelAppointment = async (appointment: Appointment) => {
  if (confirm(`Are you sure you want to cancel the appointment for ${appointment.patientName}?`)) {
    try {
      // Update appointment status
      // This would typically call an API endpoint
      appointment.status = 'cancelled'
      console.log('Appointment cancelled:', appointment.id)
    } catch (error) {
      console.error('Error cancelling appointment:', error)
      alert('Failed to cancel appointment. Please try again.')
    }
  }
}

const closeModals = () => {
  showScheduleModal.value = false
  showEditModal.value = false
  selectedAppointment.value = null
}

const handleSaveAppointment = async (appointmentData: Omit<Appointment, 'id' | 'createdAt' | 'updatedAt'>) => {
  try {
    if (showEditModal.value && selectedAppointment.value) {
      // Update existing appointment
      await appointmentsStore.updateAppointment(selectedAppointment.value.id, appointmentData)
    } else {
      // Create new appointment
      await appointmentsStore.addAppointment(appointmentData)
    }
    closeModals()
  } catch (error) {
    console.error('Error saving appointment:', error)
    alert('Failed to save appointment. Please try again.')
  }
}

// Lifecycle
onMounted(async () => {
  await appointmentsStore.fetchAppointments()
  // Add event listener for dashboard header button
  document.addEventListener('show-new-appointment-modal', handleShowScheduleModal)
})

onUnmounted(() => {
  document.removeEventListener('show-new-appointment-modal', handleShowScheduleModal)
})

// Event listener for dashboard header button
const handleShowScheduleModal = () => {
  showScheduleModal.value = true
}
</script>
