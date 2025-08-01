import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import api from '../services/api'

export interface Patient {
  id: string
  firstName: string
  lastName: string
  email: string
  phone: string
  dateOfBirth?: string
  gender: string
  medicalHistory: string
  department?: string
  status: 'active' | 'inactive' | 'critical'
  createdAt: string
  updatedAt: string
  lastVisit?: string
  upcomingAppointments?: Appointment[]
  medicalRecords?: MedicalRecord[]
}

export interface Appointment {
  id: string
  patientId: string
  patientName: string
  department: string
  doctor: string
  appointmentType: string
  dateTime: string
  duration: number
  status: 'scheduled' | 'completed' | 'cancelled' | 'no-show'
  priority: 'normal' | 'urgent' | 'emergency'
  reason?: string
  symptoms?: string
  insurance?: string
  specialRequirements?: {
    wheelchairAccess: boolean
    interpreter: boolean
    followUp: boolean
  }
  notes?: string
}

export interface MedicalRecord {
  id: string
  patientId: string
  diagnosis: string
  treatment: string
  date: string
  department: string
  doctor: string
  notes?: string
}

export interface DashboardStats {
  totalPatients: number
  todayAppointments: number
  pendingAnalyses: number
  activeSubscriptions: number
  revenueThisMonth: number
  departmentStats: Record<string, number>
}

export const usePatientsStore = defineStore('patients', () => {
  const patients = ref<Patient[]>([])
  const appointments = ref<Appointment[]>([])
  const medicalRecords = ref<MedicalRecord[]>([])
  const isLoading = ref(false)
  const error = ref<string | null>(null)

  const totalPatients = computed(() => patients.value.length)
  const activePatients = computed(() => 
    patients.value.filter(p => p.status === 'active').length
  )
  const criticalPatients = computed(() => 
    patients.value.filter(p => p.status === 'critical').length
  )

  const todayAppointments = computed(() => {
    const today = new Date().toDateString()
    return appointments.value.filter(apt => 
      new Date(apt.dateTime).toDateString() === today
    )
  })

  const patientsByDepartment = computed(() => {
    const departments: Record<string, Patient[]> = {}
    patients.value.forEach(patient => {
      if (patient.department) {
        if (!departments[patient.department]) {
          departments[patient.department] = []
        }
        departments[patient.department].push(patient)
      }
    })
    return departments
  })

  const fetchPatients = async () => {
    isLoading.value = true
    error.value = null
    
    try {
      const response = await api.get('/patients')
      patients.value = response.data
    } catch (err: any) {
      error.value = err.response?.data?.message || err.message || 'Failed to fetch patients'
      console.error('Error fetching patients:', err)
    } finally {
      isLoading.value = false
    }
  }

  const addPatient = async (patient: Omit<Patient, 'id' | 'createdAt' | 'updatedAt'>) => {
    isLoading.value = true
    error.value = null
    
    try {
      const response = await api.post('/patients', patient)
      const newPatient = response.data
      patients.value.push(newPatient)
      return newPatient
    } catch (err: any) {
      error.value = err.response?.data?.message || err.message || 'Failed to add patient'
      throw err
    } finally {
      isLoading.value = false
    }
  }

  const updatePatient = async (id: string, updates: Partial<Patient>) => {
    isLoading.value = true
    error.value = null
    
    try {
      const response = await api.put(`/patients/${id}`, updates)
      const updatedPatient = response.data
      const index = patients.value.findIndex(p => p.id === id)
      if (index !== -1) {
        patients.value[index] = updatedPatient
      }
      return updatedPatient
    } catch (err: any) {
      error.value = err.response?.data?.message || err.message || 'Failed to update patient'
      throw err
    } finally {
      isLoading.value = false
    }
  }

  const deletePatient = async (id: string) => {
    isLoading.value = true
    error.value = null
    
    try {
      await api.delete(`/patients/${id}`)
      patients.value = patients.value.filter(p => p.id !== id)
    } catch (err: any) {
      error.value = err.response?.data?.message || err.message || 'Failed to delete patient'
      throw err
    } finally {
      isLoading.value = false
    }
  }

  const fetchAppointments = async () => {
    try {
      const response = await api.get('/appointments')
      appointments.value = response.data
    } catch (err) {
      console.error('Error fetching appointments:', err)
    }
  }

  const scheduleAppointment = async (appointment: Omit<Appointment, 'id'>) => {
    try {
      const response = await api.post('/appointments', appointment)
      const newAppointment = response.data
      appointments.value.push(newAppointment)
      return newAppointment
    } catch (err) {
      console.error('Error scheduling appointment:', err)
      throw err
    }
  }

  return {
    patients,
    appointments,
    medicalRecords,
    isLoading,
    error,
    totalPatients,
    activePatients,
    criticalPatients,
    todayAppointments,
    patientsByDepartment,
    fetchPatients,
    addPatient,
    updatePatient,
    deletePatient,
    fetchAppointments,
    scheduleAppointment
  }
})
