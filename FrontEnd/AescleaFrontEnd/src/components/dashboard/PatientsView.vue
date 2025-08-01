<template>
  <div class="space-y-6">
    <!-- Page Header -->
    <div class="sm:flex sm:items-center sm:justify-between">
      <div>
        <h1 class="text-2xl font-bold text-gray-900 dark:text-white">Patients</h1>
        <p class="mt-1 text-sm text-gray-500 dark:text-gray-400">
          Manage patient records and information
          <span v-if="selectedDepartment" class="capitalize">
            - {{ selectedDepartment }} Department
          </span>
        </p>
      </div>
      <div class="mt-4 sm:mt-0 flex space-x-3">
        <button
          @click="showAddPatientModal = true"
          class="inline-flex items-center px-4 py-2 border border-transparent text-sm font-medium rounded-md shadow-sm text-white bg-purple-600 hover:bg-purple-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-purple-500"
        >
          <PlusIcon class="w-4 h-4 mr-2" />
          Add Patient
        </button>
      </div>
    </div>

    <!-- Filters and Search -->
    <div class="bg-white dark:bg-gray-800 shadow-sm rounded-lg border border-gray-200 dark:border-gray-700">
      <div class="p-6">
        <div class="grid grid-cols-1 md:grid-cols-4 gap-4">
          <!-- Search -->
          <div class="md:col-span-2">
            <div class="relative">
              <div class="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
                <MagnifyingGlassIcon class="h-5 w-5 text-gray-400" />
              </div>
              <input
                v-model="searchQuery"
                type="text"
                placeholder="Search patients..."
                class="block w-full pl-10 pr-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md leading-5 bg-white dark:bg-gray-700 text-gray-900 dark:text-white placeholder-gray-500 dark:placeholder-gray-400 focus:outline-none focus:ring-1 focus:ring-purple-500 focus:border-purple-500 sm:text-sm"
              />
            </div>
          </div>

          <!-- Department Filter -->
          <div>
            <select
              v-model="departmentFilter"
              class="block w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md bg-white dark:bg-gray-700 text-gray-900 dark:text-white focus:outline-none focus:ring-1 focus:ring-purple-500 focus:border-purple-500 sm:text-sm"
            >
              <option value="">All Departments</option>
              <option value="cardiology">Cardiology</option>
              <option value="neurology">Neurology</option>
              <option value="oncology">Oncology</option>
              <option value="radiology">Radiology</option>
              <option value="emergency">Emergency</option>
              <option value="pediatrics">Pediatrics</option>
            </select>
          </div>

          <!-- Status Filter -->
          <div>
            <select
              v-model="statusFilter"
              class="block w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md bg-white dark:bg-gray-700 text-gray-900 dark:text-white focus:outline-none focus:ring-1 focus:ring-purple-500 focus:border-purple-500 sm:text-sm"
            >
              <option value="">All Statuses</option>
              <option value="active">Active</option>
              <option value="inactive">Inactive</option>
              <option value="critical">Critical</option>
            </select>
          </div>
        </div>
      </div>
    </div>

    <!-- Patients Table -->
    <div class="bg-white dark:bg-gray-800 shadow-sm rounded-lg border border-gray-200 dark:border-gray-700">
      <div class="px-6 py-4 border-b border-gray-200 dark:border-gray-700">
        <h3 class="text-lg font-medium text-gray-900 dark:text-white">
          Patient List ({{ filteredPatients.length }})
        </h3>
      </div>
      
      <div class="overflow-x-auto">
        <table class="min-w-full divide-y divide-gray-200 dark:divide-gray-700">
          <thead class="bg-gray-50 dark:bg-gray-900">
            <tr>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase tracking-wider">
                Patient
              </th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase tracking-wider">
                Contact
              </th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase tracking-wider">
                Department
              </th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase tracking-wider">
                Status
              </th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase tracking-wider">
                Last Visit
              </th>
              <th class="relative px-6 py-3">
                <span class="sr-only">Actions</span>
              </th>
            </tr>
          </thead>
          <tbody class="bg-white dark:bg-gray-800 divide-y divide-gray-200 dark:divide-gray-700">
            <tr v-for="patient in paginatedPatients" :key="patient.id" class="hover:bg-gray-50 dark:hover:bg-gray-700">
              <td class="px-6 py-4 whitespace-nowrap">
                <div class="flex items-center">
                  <div class="flex-shrink-0 h-10 w-10">
                    <div class="h-10 w-10 rounded-full bg-purple-100 dark:bg-purple-900 flex items-center justify-center">
                      <span class="text-sm font-medium text-purple-600 dark:text-purple-300">
                        {{ patient.firstName.charAt(0) }}{{ patient.lastName.charAt(0) }}
                      </span>
                    </div>
                  </div>
                  <div class="ml-4">
                    <div class="text-sm font-medium text-gray-900 dark:text-white">
                      {{ patient.firstName }} {{ patient.lastName }}
                    </div>
                    <div class="text-sm text-gray-500 dark:text-gray-400">
                      {{ patient.gender }} • {{ calculateAge(patient.dateOfBirth) }} years old
                    </div>
                  </div>
                </div>
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <div class="text-sm text-gray-900 dark:text-white">{{ patient.email }}</div>
                <div class="text-sm text-gray-500 dark:text-gray-400">{{ patient.phone }}</div>
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <span 
                  class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium capitalize"
                  :class="getDepartmentBadgeClass(patient.department)"
                >
                  {{ patient.department || 'Unassigned' }}
                </span>
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <span 
                  class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium"
                  :class="getStatusBadgeClass(patient.status)"
                >
                  {{ patient.status }}
                </span>
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500 dark:text-gray-400">
                {{ formatDate(patient.lastVisit) }}
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                <div class="flex items-center space-x-2">
                  <button
                    @click="viewPatient(patient)"
                    class="text-purple-600 hover:text-purple-900 dark:text-purple-400 dark:hover:text-purple-300"
                  >
                    View
                  </button>
                  <button
                    @click="editPatient(patient)"
                    class="text-blue-600 hover:text-blue-900 dark:text-blue-400 dark:hover:text-blue-300"
                  >
                    Edit
                  </button>
                  <button
                    @click="deletePatient(patient)"
                    class="text-red-600 hover:text-red-900 dark:text-red-400 dark:hover:text-red-300"
                  >
                    Delete
                  </button>
                </div>
              </td>
            </tr>
          </tbody>
        </table>
        
        <!-- Empty State -->
        <div v-if="filteredPatients.length === 0" class="text-center py-12">
          <UsersIcon class="mx-auto h-12 w-12 text-gray-400" />
          <h3 class="mt-2 text-sm font-medium text-gray-900 dark:text-white">No patients found</h3>
          <p class="mt-1 text-sm text-gray-500 dark:text-gray-400">
            {{ searchQuery ? 'Try adjusting your search criteria.' : 'Get started by adding a new patient.' }}
          </p>
          <div class="mt-6">
            <button
              @click="showAddPatientModal = true"
              class="inline-flex items-center px-4 py-2 border border-transparent shadow-sm text-sm font-medium rounded-md text-white bg-purple-600 hover:bg-purple-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-purple-500"
            >
              <PlusIcon class="w-4 h-4 mr-2" />
              Add Patient
            </button>
          </div>
        </div>
      </div>

      <!-- Pagination -->
      <div v-if="filteredPatients.length > patientsPerPage" class="px-6 py-4 border-t border-gray-200 dark:border-gray-700">
        <div class="flex items-center justify-between">
          <div class="text-sm text-gray-700 dark:text-gray-300">
            Showing {{ (currentPage - 1) * patientsPerPage + 1 }} to {{ Math.min(currentPage * patientsPerPage, filteredPatients.length) }} of {{ filteredPatients.length }} results
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

    <!-- Add/Edit Patient Modal -->
    <PatientModal
      v-if="showAddPatientModal || showEditPatientModal"
      :patient="selectedPatient"
      :is-edit="showEditPatientModal"
      @close="closeModals"
      @save="handleSavePatient"
    />

    <!-- Patient Details Modal -->
    <PatientDetailsModal
      v-if="showPatientDetailsModal"
      :patient="selectedPatient"
      @close="showPatientDetailsModal = false"
      @edit="editPatient"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch, onMounted, onUnmounted } from 'vue'
import { usePatientsStore, type Patient } from '../../stores/patients'
import PatientModal from './PatientModal.vue'
import PatientDetailsModal from './PatientDetailsModal.vue'

// Define props
interface Props {
  selectedDepartment?: string | null
}

const props = defineProps<Props>()

// Store
const patientsStore = usePatientsStore()

// Local state
const searchQuery = ref('')
const departmentFilter = ref('')
const statusFilter = ref('')
const currentPage = ref(1)
const patientsPerPage = 20

const showAddPatientModal = ref(false)
const showEditPatientModal = ref(false)
const showPatientDetailsModal = ref(false)
const selectedPatient = ref<Patient | null>(null)

// Watch for department changes from parent
watch(() => props.selectedDepartment, (newDepartment) => {
  if (newDepartment) {
    departmentFilter.value = newDepartment
  }
}, { immediate: true })

// Computed properties
const filteredPatients = computed(() => {
  let patients = patientsStore.patients

  // Apply search filter
  if (searchQuery.value) {
    const query = searchQuery.value.toLowerCase()
    patients = patients.filter(patient =>
      patient.firstName.toLowerCase().includes(query) ||
      patient.lastName.toLowerCase().includes(query) ||
      patient.email.toLowerCase().includes(query) ||
      patient.phone.includes(query)
    )
  }

  // Apply department filter
  if (departmentFilter.value) {
    patients = patients.filter(patient => patient.department === departmentFilter.value)
  }

  // Apply status filter
  if (statusFilter.value) {
    patients = patients.filter(patient => patient.status === statusFilter.value)
  }

  return patients
})

const totalPages = computed(() => Math.ceil(filteredPatients.value.length / patientsPerPage))

const paginatedPatients = computed(() => {
  const start = (currentPage.value - 1) * patientsPerPage
  const end = start + patientsPerPage
  return filteredPatients.value.slice(start, end)
})

// Icon components
const PlusIcon = {
  template: `<svg fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" d="M12 4.5v15m7.5-7.5h-15" /></svg>`
}

const MagnifyingGlassIcon = {
  template: `<svg fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" d="m21 21-5.197-5.197m0 0A7.5 7.5 0 1 0 5.196 5.196a7.5 7.5 0 0 0 10.607 10.607Z" /></svg>`
}

const UsersIcon = {
  template: `<svg fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" d="M15 19.128a9.38 9.38 0 002.625.372 9.337 9.337 0 004.121-.952 4.125 4.125 0 00-7.533-2.493M15 19.128v-.003c0-1.113-.285-2.16-.786-3.07M15 19.128v.106A12.318 12.318 0 018.624 21c-2.331 0-4.512-.645-6.374-1.766l-.001-.109a6.375 6.375 0 0111.964-3.07M12 6.375a3.375 3.375 0 11-6.75 0 3.375 3.375 0 016.75 0zm8.25 2.25a2.625 2.625 0 11-5.25 0 2.625 2.625 0 015.25 0z" /></svg>`
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
  if (!date) return 'Never'
  return new Date(date).toLocaleDateString()
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

const getStatusBadgeClass = (status: string) => {
  const classes: Record<string, string> = {
    active: 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-200',
    inactive: 'bg-gray-100 text-gray-800 dark:bg-gray-900 dark:text-gray-200',
    critical: 'bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-200'
  }
  return classes[status] || classes.active
}

const viewPatient = (patient: Patient) => {
  selectedPatient.value = patient
  showPatientDetailsModal.value = true
}

const editPatient = (patient: Patient) => {
  selectedPatient.value = patient
  showEditPatientModal.value = true
  showPatientDetailsModal.value = false
}

const deletePatient = async (patient: Patient) => {
  if (confirm(`Are you sure you want to delete ${patient.firstName} ${patient.lastName}?`)) {
    try {
      await patientsStore.deletePatient(patient.id)
    } catch (error) {
      console.error('Error deleting patient:', error)
      alert('Failed to delete patient. Please try again.')
    }
  }
}

const closeModals = () => {
  showAddPatientModal.value = false
  showEditPatientModal.value = false
  selectedPatient.value = null
}

const handleSavePatient = async (patientData: Omit<Patient, 'id' | 'createdAt' | 'updatedAt'>) => {
  try {
    if (showEditPatientModal.value && selectedPatient.value) {
      await patientsStore.updatePatient(selectedPatient.value.id, patientData)
    } else {
      await patientsStore.addPatient(patientData)
    }
    closeModals()
  } catch (error) {
    console.error('Error saving patient:', error)
    alert('Failed to save patient. Please try again.')
  }
}

// Event listener for dashboard header button
const handleShowAddPatientModal = () => {
  showAddPatientModal.value = true
}

onMounted(() => {
  document.addEventListener('show-add-patient-modal', handleShowAddPatientModal)
})

onUnmounted(() => {
  document.removeEventListener('show-add-patient-modal', handleShowAddPatientModal)
})
</script>
