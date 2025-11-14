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
            Department / Specialization <span class="text-red-500">*</span>
          </label>
          <select
            id="department"
            v-model="form.department"
            required
            @change="updateAppointmentTypes"
            class="block w-full px-3 py-2 mt-1 text-gray-900 bg-white border border-gray-300 rounded-md shadow-sm dark:border-gray-600 dark:bg-gray-700 dark:text-white focus:outline-none focus:ring-1 focus:ring-purple-500 focus:border-purple-500"
          >
            <option value="">Select department or specialization</option>
            <option value="cardiology">Кардиология (Cardiology)</option>
            <option value="neurology">Неврология (Neurology)</option>
            <option value="oncology">Онкология (Oncology)</option>
            <option value="pediatrics">Педиатрия (Pediatrics)</option>
            <option value="psychiatry">Психиатрия (Psychiatry)</option>
            <option value="radiology">Радиология (Radiology)</option>
            <option value="surgery">Хирургия (Surgery)</option>
            <option value="orthopedics">Ортопедия (Orthopedics)</option>
            <option value="dermatology">Дерматология (Dermatology)</option>
            <option value="obstetrics-gynecology">Акушерство и гинекология (Obstetrics & Gynecology)</option>
            <option value="anesthesiology">Анестезиология (Anesthesiology)</option>
            <option value="ophthalmology">Офталмология (Ophthalmology)</option>
            <option value="otolaryngology">Оториноларингология (Otolaryngology)</option>
            <option value="urology">Урология (Urology)</option>
            <option value="endocrinology">Ендокринология (Endocrinology)</option>
            <option value="gastroenterology">Гастроентерология (Gastroenterology)</option>
            <option value="nephrology">Нефрология (Nephrology)</option>
            <option value="pulmonology">Пулмология (Pulmonology)</option>
            <option value="rheumatology">Ревматология (Rheumatology)</option>
            <option value="infectious-diseases">Инфекциозни болести (Infectious Diseases)</option>
            <option value="emergency">Спешна медицина (Emergency Medicine)</option>
            <option value="general-practice">Обща медицина (General Practice)</option>
            <option value="administration">Administration</option>
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
            :disabled="loadingDoctors || availableDoctors.length === 0"
            class="block w-full px-3 py-2 mt-1 text-gray-900 bg-white border border-gray-300 rounded-md shadow-sm dark:border-gray-600 dark:bg-gray-700 dark:text-white focus:outline-none focus:ring-1 focus:ring-purple-500 focus:border-purple-500 disabled:opacity-50"
          >
            <option value="">{{ loadingDoctors ? 'Loading doctors...' : availableDoctors.length === 0 ? 'No doctors available for this department' : 'Select doctor' }}</option>
            <option v-for="doctor in availableDoctors" :key="doctor.id" :value="doctor.value">
              {{ doctor.name }}
            </option>
          </select>
          <p v-if="!loadingDoctors && form.department && availableDoctors.length === 0" class="mt-1 text-sm text-gray-500 dark:text-gray-400">
            No doctors found for {{ form.department }}. Please contact admin to assign doctors to this department.
          </p>
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
import { useAdminStore } from '../../stores/admin'
import { useAuthStore } from '../../stores/auth'

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
const adminStore = useAdminStore()
const authStore = useAuthStore()

// Local state
const isSubmitting = ref(false)
const doctors = ref<any[]>([])
const loadingDoctors = ref(false)

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
    cardiology: ['Consultation', 'ECG Test', 'Echocardiogram', 'Stress Test', 'Follow-up', 'Cardiac Catheterization'],
    neurology: ['Consultation', 'EEG Test', 'MRI Review', 'Neurological Assessment', 'Follow-up', 'EMG Test'],
    oncology: ['Consultation', 'Chemotherapy', 'Radiation Planning', 'Follow-up', 'Biopsy Review', 'Treatment Planning'],
    pediatrics: ['Well-child Visit', 'Vaccination', 'Sick Visit', 'Development Assessment', 'Follow-up'],
    psychiatry: ['Consultation', 'Therapy Session', 'Medication Review', 'Crisis Intervention', 'Follow-up'],
    radiology: ['X-Ray', 'CT Scan', 'MRI', 'Ultrasound', 'Mammography', 'Nuclear Medicine'],
    surgery: ['Pre-operative Consultation', 'Post-operative Follow-up', 'Surgical Procedure', 'Consultation'],
    orthopedics: ['Consultation', 'Joint Injection', 'Post-operative Follow-up', 'Physical Therapy Review'],
    dermatology: ['Consultation', 'Skin Biopsy', 'Mole Check', 'Treatment Review', 'Follow-up'],
    'obstetrics-gynecology': ['Prenatal Visit', 'Gynecological Exam', 'Ultrasound', 'Consultation', 'Follow-up'],
    anesthesiology: ['Pre-operative Assessment', 'Post-operative Follow-up', 'Pain Management', 'Consultation'],
    ophthalmology: ['Eye Exam', 'Vision Test', 'Consultation', 'Follow-up', 'Surgery Consultation'],
    otolaryngology: ['Consultation', 'Hearing Test', 'Throat Examination', 'Follow-up', 'Procedure'],
    urology: ['Consultation', 'Examination', 'Follow-up', 'Procedure Planning', 'Post-operative Care'],
    endocrinology: ['Consultation', 'Diabetes Management', 'Hormone Assessment', 'Follow-up', 'Treatment Review'],
    gastroenterology: ['Consultation', 'Endoscopy', 'Colonoscopy', 'Follow-up', 'Treatment Planning'],
    nephrology: ['Consultation', 'Dialysis Planning', 'Kidney Function Assessment', 'Follow-up'],
    pulmonology: ['Consultation', 'Pulmonary Function Test', 'Sleep Study Review', 'Follow-up'],
    rheumatology: ['Consultation', 'Joint Assessment', 'Treatment Review', 'Follow-up', 'Injection'],
    'infectious-diseases': ['Consultation', 'Treatment Review', 'Follow-up', 'Laboratory Review'],
    emergency: ['Urgent Care', 'Emergency Consultation', 'Trauma Assessment', 'Critical Care'],
    'general-practice': ['General Consultation', 'Health Check-up', 'Vaccination', 'Follow-up', 'Referral'],
    administration: ['Administrative Meeting', 'Consultation', 'Review']
  }
  return typesByDepartment[form.value.department] || ['Consultation', 'Follow-up']
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
  console.log('Computing available doctors for department:', form.value.department)
  console.log('All doctors:', doctors.value)
  
  if (!form.value.department) return []
  
  // Show all doctors that match the selected department/specialization
  const matchedDoctors = doctors.value.filter(doctor => {
    // Always include if department matches exactly
    if (doctor.department?.toLowerCase() === form.value.department.toLowerCase()) {
      console.log('Doctor matched by department:', doctor)
      return true
    }
    
    // Include doctors whose specialization matches the department
    if (doctor.specialization) {
      const specialization = doctor.specialization.toLowerCase()
      const department = form.value.department.toLowerCase()
      
      // Comprehensive mapping of specializations to departments
      const specialtyMatches: Record<string, string[]> = {
        'cardiology': ['кардиология'],
        'neurology': ['неврология'],
        'oncology': ['онкология'],
        'pediatrics': ['педиатрия'],
        'psychiatry': ['психиатрия'],
        'radiology': ['радиология'],
        'surgery': ['хирургия'],
        'orthopedics': ['ортопедия'],
        'dermatology': ['дерматология'],
        'obstetrics-gynecology': ['акушерство и гинекология'],
        'anesthesiology': ['анестезиология'],
        'ophthalmology': ['офталмология'],
        'otolaryngology': ['оториноларингология'],
        'urology': ['урология'],
        'endocrinology': ['ендокринология'],
        'gastroenterology': ['гастроентерология'],
        'nephrology': ['нефрология'],
        'pulmonology': ['пулмология'],
        'rheumatology': ['ревматология'],
        'infectious-diseases': ['инфекциозни болести'],
        'emergency': ['спешна медицина'],
        'general-practice': ['обща медицина'],
        'administration': ['administration']
      }
      
      const matchingTerms = specialtyMatches[department] || [department]
      const isMatch = matchingTerms.some(term => specialization.includes(term.toLowerCase()))
      if (isMatch) {
        console.log('Doctor matched by specialization:', doctor)
      }
      return isMatch
    }
    
    // For general practice, show all doctors
    if (form.value.department === 'general-practice') {
      console.log('General practice - including all doctors:', doctor)
      return true
    }
    
    // For administration, show admin users
    if (form.value.department === 'administration' && doctor.role === 'admin') {
      console.log('Admin department - including admin:', doctor)
      return true
    }
    
    return false
  }).map(doctor => ({
    id: doctor.id,
    name: `${doctor.firstName} ${doctor.lastName}${doctor.specialization ? ` (${doctor.specialization})` : ''}`,
    value: doctor.id
  }))
  
  console.log('Matched doctors:', matchedDoctors)
  return matchedDoctors
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
const loadDoctors = async () => {
  try {
    loadingDoctors.value = true
    console.log('Loading doctors from API...')
    
    try {
      const response = await adminStore.fetchDoctors()
      console.log('Doctors loaded from API:', response)
      doctors.value = response || []
    } catch (apiError) {
      console.warn('API failed, using fallback:', apiError)
      doctors.value = []
    }
    
    // Always add current user as a doctor option if they have doctor role
    const currentUser = authStore.user
    if (currentUser && (currentUser.role === 'doctor' || currentUser.role === 'admin')) {
      const currentUserDoctor = {
        id: currentUser.id,
        firstName: currentUser.firstName,
        lastName: currentUser.lastName,
        email: currentUser.email,
        role: currentUser.role,
        department: (currentUser as any).department || 'general-practice',
        specialization: (currentUser as any).specialization || 'Обща медицина'
      }
      
      // Check if current user is already in the list
      const existingDoctor = doctors.value.find(d => d.id === currentUser.id)
      if (!existingDoctor) {
        console.log('Adding current user as doctor option:', currentUserDoctor)
        doctors.value.push(currentUserDoctor)
      }
    }
    
    // If still no doctors, add some fallback options
    if (doctors.value.length === 0) {
      console.log('No doctors available, creating fallback options')
      if (currentUser) {
        doctors.value = [{
          id: currentUser.id,
          firstName: currentUser.firstName,
          lastName: currentUser.lastName,
          email: currentUser.email,
          role: 'doctor',
          department: 'general-practice',
          specialization: 'Обща медицина'
        }]
      }
    }
    
    console.log('Final doctors list:', doctors.value)
  } catch (error) {
    console.error('Error loading doctors:', error)
    doctors.value = []
  } finally {
    loadingDoctors.value = false
  }
}

const updateAppointmentTypes = () => {
  // Clear doctor selection when department changes
  form.value.doctor = ''
  form.value.appointmentType = ''
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
  // Load doctors first
  await loadDoctors()
  
  // Load patients if not already loaded
  if (patientsStore.patients.length === 0) {
    await patientsStore.fetchPatients()
  }
  
  initializeForm()
})
</script>
