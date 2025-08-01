<template>
  <div class="fixed inset-0 z-50 overflow-y-auto">
    <div class="flex items-end justify-center min-h-screen pt-4 px-4 pb-20 text-center sm:block sm:p-0">
      <!-- Background overlay -->
      <div 
        class="fixed inset-0 bg-gray-500 bg-opacity-75 transition-opacity" 
        @click="$emit('close')"
      ></div>

      <!-- Modal panel -->
      <div class="inline-block align-bottom bg-white dark:bg-gray-800 rounded-lg text-left overflow-hidden shadow-xl transform transition-all sm:my-8 sm:align-middle sm:max-w-lg sm:w-full">
        <!-- Header -->
        <div class="bg-white dark:bg-gray-800 px-4 pt-5 pb-4 sm:p-6 sm:pb-4">
          <div class="flex items-center justify-between">
            <h3 class="text-lg font-medium text-gray-900 dark:text-white">
              {{ isEdit ? 'Edit Patient' : 'Add New Patient' }}
            </h3>
            <button
              @click="$emit('close')"
              class="text-gray-400 hover:text-gray-500 dark:hover:text-gray-300"
            >
              <XMarkIcon class="h-6 w-6" />
            </button>
          </div>
        </div>

        <!-- Form -->
        <form @submit.prevent="handleSubmit">
          <div class="px-4 pb-4 sm:px-6">
            <div class="space-y-4">
              <!-- Name Fields -->
              <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
                <div>
                  <label for="firstName" class="block text-sm font-medium text-gray-700 dark:text-gray-300">
                    First Name *
                  </label>
                  <input
                    id="firstName"
                    v-model="formData.firstName"
                    type="text"
                    required
                    class="mt-1 block w-full border border-gray-300 dark:border-gray-600 rounded-md shadow-sm py-2 px-3 bg-white dark:bg-gray-700 text-gray-900 dark:text-white focus:outline-none focus:ring-purple-500 focus:border-purple-500 sm:text-sm"
                  />
                </div>
                <div>
                  <label for="lastName" class="block text-sm font-medium text-gray-700 dark:text-gray-300">
                    Last Name *
                  </label>
                  <input
                    id="lastName"
                    v-model="formData.lastName"
                    type="text"
                    required
                    class="mt-1 block w-full border border-gray-300 dark:border-gray-600 rounded-md shadow-sm py-2 px-3 bg-white dark:bg-gray-700 text-gray-900 dark:text-white focus:outline-none focus:ring-purple-500 focus:border-purple-500 sm:text-sm"
                  />
                </div>
              </div>

              <!-- Contact Fields -->
              <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
                <div>
                  <label for="email" class="block text-sm font-medium text-gray-700 dark:text-gray-300">
                    Email *
                  </label>
                  <input
                    id="email"
                    v-model="formData.email"
                    type="email"
                    required
                    class="mt-1 block w-full border border-gray-300 dark:border-gray-600 rounded-md shadow-sm py-2 px-3 bg-white dark:bg-gray-700 text-gray-900 dark:text-white focus:outline-none focus:ring-purple-500 focus:border-purple-500 sm:text-sm"
                  />
                </div>
                <div>
                  <label for="phone" class="block text-sm font-medium text-gray-700 dark:text-gray-300">
                    Phone *
                  </label>
                  <input
                    id="phone"
                    v-model="formData.phone"
                    type="tel"
                    required
                    class="mt-1 block w-full border border-gray-300 dark:border-gray-600 rounded-md shadow-sm py-2 px-3 bg-white dark:bg-gray-700 text-gray-900 dark:text-white focus:outline-none focus:ring-purple-500 focus:border-purple-500 sm:text-sm"
                  />
                </div>
              </div>

              <!-- Personal Info -->
              <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
                <div>
                  <label for="dateOfBirth" class="block text-sm font-medium text-gray-700 dark:text-gray-300">
                    Date of Birth
                  </label>
                  <input
                    id="dateOfBirth"
                    v-model="formData.dateOfBirth"
                    type="date"
                    class="mt-1 block w-full border border-gray-300 dark:border-gray-600 rounded-md shadow-sm py-2 px-3 bg-white dark:bg-gray-700 text-gray-900 dark:text-white focus:outline-none focus:ring-purple-500 focus:border-purple-500 sm:text-sm"
                  />
                </div>
                <div>
                  <label for="gender" class="block text-sm font-medium text-gray-700 dark:text-gray-300">
                    Gender
                  </label>
                  <select
                    id="gender"
                    v-model="formData.gender"
                    class="mt-1 block w-full border border-gray-300 dark:border-gray-600 rounded-md shadow-sm py-2 px-3 bg-white dark:bg-gray-700 text-gray-900 dark:text-white focus:outline-none focus:ring-purple-500 focus:border-purple-500 sm:text-sm"
                  >
                    <option value="">Select gender</option>
                    <option value="male">Male</option>
                    <option value="female">Female</option>
                    <option value="other">Other</option>
                  </select>
                </div>
              </div>

              <!-- Department and Status -->
              <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
                <div>
                  <label for="department" class="block text-sm font-medium text-gray-700 dark:text-gray-300">
                    Department
                  </label>
                  <select
                    id="department"
                    v-model="formData.department"
                    class="mt-1 block w-full border border-gray-300 dark:border-gray-600 rounded-md shadow-sm py-2 px-3 bg-white dark:bg-gray-700 text-gray-900 dark:text-white focus:outline-none focus:ring-purple-500 focus:border-purple-500 sm:text-sm"
                  >
                    <option value="">Select department</option>
                    <option value="cardiology">Cardiology</option>
                    <option value="neurology">Neurology</option>
                    <option value="oncology">Oncology</option>
                    <option value="radiology">Radiology</option>
                    <option value="emergency">Emergency</option>
                    <option value="pediatrics">Pediatrics</option>
                  </select>
                </div>
                <div>
                  <label for="status" class="block text-sm font-medium text-gray-700 dark:text-gray-300">
                    Status
                  </label>
                  <select
                    id="status"
                    v-model="formData.status"
                    class="mt-1 block w-full border border-gray-300 dark:border-gray-600 rounded-md shadow-sm py-2 px-3 bg-white dark:bg-gray-700 text-gray-900 dark:text-white focus:outline-none focus:ring-purple-500 focus:border-purple-500 sm:text-sm"
                  >
                    <option value="active">Active</option>
                    <option value="inactive">Inactive</option>
                    <option value="critical">Critical</option>
                  </select>
                </div>
              </div>

              <!-- Medical History -->
              <div>
                <label for="medicalHistory" class="block text-sm font-medium text-gray-700 dark:text-gray-300">
                  Medical History
                </label>
                <textarea
                  id="medicalHistory"
                  v-model="formData.medicalHistory"
                  rows="4"
                  class="mt-1 block w-full border border-gray-300 dark:border-gray-600 rounded-md shadow-sm py-2 px-3 bg-white dark:bg-gray-700 text-gray-900 dark:text-white focus:outline-none focus:ring-purple-500 focus:border-purple-500 sm:text-sm"
                  placeholder="Enter patient's medical history, allergies, current medications, etc."
                ></textarea>
              </div>
            </div>
          </div>

          <!-- Footer -->
          <div class="bg-gray-50 dark:bg-gray-700 px-4 py-3 sm:px-6 sm:flex sm:flex-row-reverse">
            <button
              type="submit"
              :disabled="isSubmitting"
              class="w-full inline-flex justify-center rounded-md border border-transparent shadow-sm px-4 py-2 bg-purple-600 text-base font-medium text-white hover:bg-purple-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-purple-500 sm:ml-3 sm:w-auto sm:text-sm disabled:opacity-50 disabled:cursor-not-allowed"
            >
              <svg 
                v-if="isSubmitting" 
                class="animate-spin -ml-1 mr-3 h-5 w-5 text-white" 
                fill="none" 
                viewBox="0 0 24 24"
              >
                <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
                <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
              </svg>
              {{ isSubmitting ? 'Saving...' : (isEdit ? 'Update Patient' : 'Add Patient') }}
            </button>
            <button
              type="button"
              @click="$emit('close')"
              class="mt-3 w-full inline-flex justify-center rounded-md border border-gray-300 dark:border-gray-600 shadow-sm px-4 py-2 bg-white dark:bg-gray-800 text-base font-medium text-gray-700 dark:text-gray-300 hover:bg-gray-50 dark:hover:bg-gray-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-purple-500 sm:mt-0 sm:ml-3 sm:w-auto sm:text-sm"
            >
              Cancel
            </button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, watch } from 'vue'
import { usePatientsStore } from '../../stores/patients'
import type { Patient } from '../../stores/patients'

// Define props
interface Props {
  patient?: Patient | null
  isEdit?: boolean
}

const props = defineProps<Props>()

// Define emits
const emit = defineEmits(['close', 'save'])

// Store
const patientsStore = usePatientsStore()

// Local state
const isSubmitting = ref(false)
const formData = ref({
  firstName: '',
  lastName: '',
  email: '',
  phone: '',
  dateOfBirth: '',
  gender: '',
  department: '',
  status: 'active' as 'active' | 'inactive' | 'critical',
  medicalHistory: ''
})

// Icon component
const XMarkIcon = {
  template: `<svg fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" d="M6 18L18 6M6 6l12 12" /></svg>`
}

// Watch for patient changes
watch(() => props.patient, (newPatient) => {
  if (newPatient && props.isEdit) {
    formData.value = {
      firstName: newPatient.firstName,
      lastName: newPatient.lastName,
      email: newPatient.email,
      phone: newPatient.phone,
      dateOfBirth: newPatient.dateOfBirth || '',
      gender: newPatient.gender,
      department: newPatient.department || '',
      status: newPatient.status,
      medicalHistory: newPatient.medicalHistory
    }
  }
}, { immediate: true })

// Methods
const handleSubmit = async () => {
  if (isSubmitting.value) return
  
  isSubmitting.value = true
  
  try {
    if (props.isEdit && props.patient?.id) {
      await patientsStore.updatePatient(props.patient.id, formData.value)
    } else {
      await patientsStore.addPatient(formData.value)
    }
    
    emit('close')
    
    // Show success message
    console.log(`Patient ${props.isEdit ? 'updated' : 'added'} successfully`)
    
  } catch (error) {
    console.error('Error saving patient:', error)
    // You could emit an error event here or show a toast notification
  } finally {
    isSubmitting.value = false
  }
}

onMounted(() => {
  // Reset form when modal opens
  if (!props.isEdit) {
    formData.value = {
      firstName: '',
      lastName: '',
      email: '',
      phone: '',
      dateOfBirth: '',
      gender: '',
      department: '',
      status: 'active',
      medicalHistory: ''
    }
  }
})
</script>
