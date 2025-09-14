<!-- Copyright (c) 2025 Alexander Sinapov | Simeon Petkov -->

<!-- All rights reserved. -->
<!-- This code is proprietary and confidential.   -->
<!-- Unauthorized copying, modification, distribution, or use is strictly prohibited. -->

<template>
  <div class="fixed inset-0 z-50 w-full h-full overflow-y-auto backdrop-blur-sm bg-black/20">
    <div class="relative w-11/12 max-w-2xl p-5 mx-auto bg-white border rounded-md shadow-lg top-20 dark:bg-gray-800">
      <!-- Header -->
      <div class="flex items-center justify-between pb-4 border-b border-gray-200 dark:border-gray-700">
        <h3 class="text-lg font-medium text-gray-900 dark:text-white">
          {{ isEdit ? 'Edit Appointment' : 'Schedule New Appointment' }}
        </h3>
        <button
          @click="$emit('close')"
          class="text-gray-400 hover:text-gray-600 dark:hover:text-gray-300"
        >
          <XMarkIcon class="w-5 h-5" />
        </button>
      </div>

      <!-- Form -->
      <form @submit.prevent="handleSubmit" class="mt-6 space-y-6">
        <!-- Patient Selection -->
        <div>
          <label for="patient" class="block text-sm font-medium text-gray-700 dark:text-gray-300">
            Patient <span class="text-red-500">*</span>
          </label>
          <select
            id="patient"
            v-model="form.patientId"
            required
            class="block w-full px-3 py-2 mt-1 text-gray-900 bg-white border border-gray-300 rounded-md shadow-sm dark:border-gray-600 dark:bg-gray-700 dark:text-white focus:outline-none focus:ring-1 focus:ring-purple-500 focus:border-purple-500"
          >
            <option value="">Select a patient</option>
            <option 
              v-for="patient in availablePatients" 
              :key="patient.id" 
              :value="patient.id"
            >
              {{ patient.firstName }} {{ patient.lastName }} - {{ patient.id }}
            </option>
          </select>
        </div>

        <!-- Department -->
        <div>
          <label for="department" class="block text-sm font-medium text-gray-700 dark:text-gray-300">
            Department <span class="text-red-500">*</span>
          </label>
          <select
            id="department"
            v-model="form.department"
            required
            @change="updateAppointmentTypes"
            class="block w-full px-3 py-2 mt-1 text-gray-900 bg-white border border-gray-300 rounded-md shadow-sm dark:border-gray-600 dark:bg-gray-700 dark:text-white focus:outline-none focus:ring-1 focus:ring-purple-500 focus:border-purple-500"
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

        <!-- Appointment Type -->
        <div>
          <label for="appointmentType" class="block text-sm font-medium text-gray-700 dark:text-gray-300">
            Appointment Type <span class="text-red-500">*</span>
          </label>
          <select
            id="appointmentType"
            v-model="form.appointmentType"
            required
            class="block w-full px-3 py-2 mt-1 text-gray-900 bg-white border border-gray-300 rounded-md shadow-sm dark:border-gray-600 dark:bg-gray-700 dark:text-white focus:outline-none focus:ring-1 focus:ring-purple-500 focus:border-purple-500"
          >
            <option value="">Select appointment type</option>
            <option 
              v-for="type in availableAppointmentTypes" 
              :key="type" 
              :value="type"
            >
              {{ type }}
            </option>
          </select>
        </div>

        <!-- Date and Time -->
        <div class="grid grid-cols-1 gap-4 md:grid-cols-2">
          <div>
            <label for="date" class="block text-sm font-medium text-gray-700 dark:text-gray-300">
              Date <span class="text-red-500">*</span>
            </label>
            <input
              id="date"
              v-model="form.date"
              type="date"
              required
              :min="minDate"
              class="block w-full px-3 py-2 mt-1 text-gray-900 bg-white border border-gray-300 rounded-md shadow-sm dark:border-gray-600 dark:bg-gray-700 dark:text-white focus:outline-none focus:ring-1 focus:ring-purple-500 focus:border-purple-500"
            />
          </div>

          <div>
            <label for="time" class="block text-sm font-medium text-gray-700 dark:text-gray-300">
              Time <span class="text-red-500">*</span>
            </label>
            <select
              id="time"
              v-model="form.time"
              required
              class="block w-full px-3 py-2 mt-1 text-gray-900 bg-white border border-gray-300 rounded-md shadow-sm dark:border-gray-600 dark:bg-gray-700 dark:text-white focus:outline-none focus:ring-1 focus:ring-purple-500 focus:border-purple-500"
            >
              <option value="">Select time</option>
              <option v-for="slot in availableTimeSlots" :key="slot" :value="slot">
                {{ slot }}
              </option>
            </select>
          </div>
        </div>

        <!-- Doctor -->
        <div>
          <label for="doctor" class="block text-sm font-medium text-gray-700 dark:text-gray-300">
            Doctor <span class="text-red-500">*</span>
          </label>
          <select
            id="doctor"
            v-model="form.doctor"
            required
            class="block w-full px-3 py-2 mt-1 text-gray-900 bg-white border border-gray-300 rounded-md shadow-sm dark:border-gray-600 dark:bg-gray-700 dark:text-white focus:outline-none focus:ring-1 focus:ring-purple-500 focus:border-purple-500"
          >
            <option value="">Select doctor</option>
            <option v-for="doctor in availableDoctors" :key="doctor" :value="doctor">
              {{ doctor }}
            </option>
          </select>
        </div>

        <!-- Duration -->
        <div>
          <label for="duration" class="block text-sm font-medium text-gray-700 dark:text-gray-300">
            Duration (minutes) <span class="text-red-500">*</span>
          </label>
          <select
            id="duration"
            v-model="form.duration"
            required
            class="block w-full px-3 py-2 mt-1 text-gray-900 bg-white border border-gray-300 rounded-md shadow-sm dark:border-gray-600 dark:bg-gray-700 dark:text-white focus:outline-none focus:ring-1 focus:ring-purple-500 focus:border-purple-500"
          >
            <option value="">Select duration</option>
            <option value="15">15 minutes</option>
            <option value="30">30 minutes</option>
            <option value="45">45 minutes</option>
            <option value="60">1 hour</option>
            <option value="90">1.5 hours</option>
            <option value="120">2 hours</option>
          </select>
        </div>

        <!-- Priority -->
        <div>
          <label for="priority" class="block text-sm font-medium text-gray-700 dark:text-gray-300">
            Priority
          </label>
          <select
            id="priority"
            v-model="form.priority"
            class="block w-full px-3 py-2 mt-1 text-gray-900 bg-white border border-gray-300 rounded-md shadow-sm dark:border-gray-600 dark:bg-gray-700 dark:text-white focus:outline-none focus:ring-1 focus:ring-purple-500 focus:border-purple-500"
          >
            <option value="normal">Normal</option>
            <option value="urgent">Urgent</option>
            <option value="emergency">Emergency</option>
          </select>
        </div>

        <!-- Reason/Notes -->
        <div>
          <label for="reason" class="block text-sm font-medium text-gray-700 dark:text-gray-300">
            Reason for Visit
          </label>
          <textarea
            id="reason"
            v-model="form.reason"
            rows="3"
            placeholder="Please describe the reason for this appointment..."
            class="block w-full px-3 py-2 mt-1 text-gray-900 bg-white border border-gray-300 rounded-md shadow-sm dark:border-gray-600 dark:bg-gray-700 dark:text-white focus:outline-none focus:ring-1 focus:ring-purple-500 focus:border-purple-500"
          />
        </div>

        <!-- Additional Notes -->
        <div>
          <label for="notes" class="block text-sm font-medium text-gray-700 dark:text-gray-300">
            Additional Notes
          </label>
          <textarea
            id="notes"
            v-model="form.notes"
            rows="3"
            placeholder="Any additional notes or special instructions..."
            class="block w-full px-3 py-2 mt-1 text-gray-900 bg-white border border-gray-300 rounded-md shadow-sm dark:border-gray-600 dark:bg-gray-700 dark:text-white focus:outline-none focus:ring-1 focus:ring-purple-500 focus:border-purple-500"
          />
        </div>

        <!-- Actions -->
        <div class="flex justify-end pt-6 space-x-3 border-t border-gray-200 dark:border-gray-700">
          <button
            type="button"
            @click="$emit('close')"
            class="px-4 py-2 text-sm font-medium text-gray-700 bg-white border border-gray-300 rounded-md dark:text-gray-300 dark:bg-gray-700 dark:border-gray-600 hover:bg-gray-50 dark:hover:bg-gray-600 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-gray-500"
          >
            Cancel
          </button>
          <button
            type="submit"
            :disabled="!isFormValid"
            class="px-4 py-2 text-sm font-medium text-white bg-purple-600 border border-transparent rounded-md hover:bg-purple-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-purple-500 disabled:opacity-50 disabled:cursor-not-allowed"
          >
            {{ isEdit ? 'Update Appointment' : 'Schedule Appointment' }}
          </button>
        </div>
      </form>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch, onMounted } from 'vue'
import { usePatientsStore } from '../../stores/patients'
import { useAppointmentsStore, type Appointment } from '../../stores/appointments'

interface Props {
  appointment?: Appointment | null
  isEdit?: boolean
}

interface AppointmentForm {
  patientId: string
  department: string
  appointmentType: string
  date: string
  time: string
  doctor: string
  duration: string
  priority: string
  reason: string
  notes: string
}

const props = defineProps<Props>()
const emit = defineEmits<{
  close: []
  save: [data: Omit<Appointment, 'id' | 'createdAt' | 'updatedAt'>]
}>()

// Stores
const patientsStore = usePatientsStore()
const appointmentsStore = useAppointmentsStore()

// Local state
const isSubmitting = ref(false)

// Form data
const form = ref<AppointmentForm>({
  patientId: '',
  department: '',
  appointmentType: '',
  date: '',
  time: '',
  doctor: '',
  duration: '30',
  priority: 'normal',
  reason: '',
  notes: ''
})

// Computed properties
const minDate = computed(() => {
  return new Date().toISOString().split('T')[0]
})

const availablePatients = computed(() => {
  return patientsStore.patients
})

const availableAppointmentTypes = computed(() => {
  const typesByDepartment: Record<string, string[]> = {
    cardiology: [
      'Consultation',
      'ECG Test',
      'Echocardiogram',
      'Stress Test',
      'Follow-up',
      'Cardiac Catheterization'
    ],
    neurology: [
      'Consultation',
      'EEG Test',
      'MRI Review',
      'Neurological Assessment',
      'Follow-up',
      'EMG Test'
    ],
    oncology: [
      'Consultation',
      'Chemotherapy',
      'Radiation Planning',
      'Follow-up',
      'Biopsy Review',
      'Treatment Planning'
    ],
    radiology: [
      'X-Ray',
      'CT Scan',
      'MRI',
      'Ultrasound',
      'Mammography',
      'Nuclear Medicine'
    ],
    emergency: [
      'Urgent Care',
      'Emergency Consultation',
      'Trauma Assessment',
      'Critical Care'
    ],
    pediatrics: [
      'Well-child Visit',
      'Vaccination',
      'Sick Visit',
      'Development Assessment',
      'Follow-up'
    ]
  }
  return typesByDepartment[form.value.department] || []
})

const availableTimeSlots = computed(() => {
  const slots = []
  const startHour = form.value.department === 'emergency' ? 0 : 8
  const endHour = form.value.department === 'emergency' ? 24 : 18
  
  for (let hour = startHour; hour < endHour; hour++) {
    for (let minute = 0; minute < 60; minute += 30) {
      const timeString = `${hour.toString().padStart(2, '0')}:${minute.toString().padStart(2, '0')}`
      slots.push(timeString)
    }
  }
  return slots
})

const availableDoctors = computed(() => {
  const doctorsByDepartment: Record<string, string[]> = {
    cardiology: [
      'Dr. Smith (Cardiologist)',
      'Dr. Johnson (Interventional Cardiologist)',
      'Dr. Williams (Electrophysiologist)'
    ],
    neurology: [
      'Dr. Brown (Neurologist)',
      'Dr. Davis (Neurosurgeon)',
      'Dr. Miller (Movement Disorders Specialist)'
    ],
    oncology: [
      'Dr. Wilson (Medical Oncologist)',
      'Dr. Moore (Radiation Oncologist)',
      'Dr. Taylor (Surgical Oncologist)'
    ],
    radiology: [
      'Dr. Anderson (Radiologist)',
      'Dr. Thomas (Interventional Radiologist)',
      'Dr. Jackson (Nuclear Medicine)'
    ],
    emergency: [
      'Dr. White (Emergency Medicine)',
      'Dr. Harris (Trauma Specialist)',
      'Dr. Martin (Critical Care)'
    ],
    pediatrics: [
      'Dr. Thompson (Pediatrician)',
      'Dr. Garcia (Pediatric Specialist)',
      'Dr. Martinez (Adolescent Medicine)'
    ]
  }
  return doctorsByDepartment[form.value.department] || []
})

const isFormValid = computed(() => {
  return form.value.patientId &&
         form.value.department &&
         form.value.appointmentType &&
         form.value.date &&
         form.value.time &&
         form.value.doctor &&
         form.value.duration
})

// Icon component
const XMarkIcon = {
  template: `<svg fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" d="M6 18L18 6M6 6l12 12" /></svg>`
}

// Methods
const updateAppointmentTypes = () => {
  form.value.appointmentType = ''
  form.value.doctor = ''
}

const handleSubmit = async () => {
  if (!isFormValid.value || isSubmitting.value) return
  
  isSubmitting.value = true
  
  try {
    const dateTime = new Date(`${form.value.date}T${form.value.time}:00`)
    const selectedPatient = patientsStore.patients.find(p => p.id === form.value.patientId)
    
    const appointmentData = {
      patientId: form.value.patientId,
      patientName: selectedPatient ? `${selectedPatient.firstName} ${selectedPatient.lastName}` : '',
      department: form.value.department,
      doctor: form.value.doctor,
      appointmentType: form.value.appointmentType,
      dateTime: dateTime.toISOString(),
      duration: parseInt(form.value.duration),
      status: 'scheduled' as const,
      priority: form.value.priority as 'normal' | 'urgent' | 'emergency',
      reason: form.value.reason,
      notes: form.value.notes
    }

    if (props.isEdit && props.appointment?.id) {
      await appointmentsStore.updateAppointment(props.appointment.id, appointmentData)
    } else {
      await appointmentsStore.addAppointment(appointmentData)
    }
    
    emit('close')
    
    // Show success message
    alert(`Appointment ${props.isEdit ? 'updated' : 'scheduled'} successfully!`)
    
  } catch (error) {
    console.error('Error saving appointment:', error)
    alert('Failed to save appointment. Please try again.')
  } finally {
    isSubmitting.value = false
  }
}

const initializeForm = () => {
  if (props.isEdit && props.appointment) {
    const apt = props.appointment
    const appointmentDate = new Date(apt.dateTime)
    
    form.value = {
      patientId: apt.patientId,
      department: apt.department,
      appointmentType: apt.appointmentType,
      date: appointmentDate.toISOString().split('T')[0],
      time: appointmentDate.toTimeString().slice(0, 5),
      doctor: apt.doctor,
      duration: apt.duration.toString(),
      priority: apt.priority,
      reason: apt.reason || '',
      notes: apt.notes || ''
    }
  }
}

// Watchers
watch(() => props.appointment, initializeForm, { immediate: true })

// Lifecycle
onMounted(async () => {
  // Load patients if not already loaded
  if (patientsStore.patients.length === 0) {
    await patientsStore.fetchPatients()
  }
  
  initializeForm()
})
</script>
