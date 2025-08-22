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
      // For demo purposes, create sample data if none exists
      if (patients.value.length === 0) {
        patients.value = [
          {
            id: '1',
            firstName: 'John',
            lastName: 'Smith',
            email: 'john.smith@email.com',
            phone: '+1-555-0123',
            dateOfBirth: '1980-05-15',
            gender: 'Male',
            medicalHistory: 'Hypertension, Diabetes Type 2',
            department: 'cardiology',
            status: 'active',
            createdAt: '2024-01-15T10:00:00Z',
            updatedAt: '2024-01-15T10:00:00Z',
            lastVisit: '2024-01-10T14:30:00Z'
          },
          {
            id: '2',
            firstName: 'Emily',
            lastName: 'Johnson',
            email: 'emily.johnson@email.com',
            phone: '+1-555-0124',
            dateOfBirth: '1975-03-22',
            gender: 'Female',
            medicalHistory: 'Breast cancer survivor, Currently in remission',
            department: 'oncology',
            status: 'active',
            createdAt: '2024-01-12T09:00:00Z',
            updatedAt: '2024-01-12T09:00:00Z',
            lastVisit: '2024-01-08T11:00:00Z'
          },
          {
            id: '3',
            firstName: 'Michael',
            lastName: 'Davis',
            email: 'michael.davis@email.com',
            phone: '+1-555-0125',
            dateOfBirth: '1965-11-08',
            gender: 'Male',
            medicalHistory: 'Stroke history, Ongoing rehabilitation',
            department: 'neurology',
            status: 'critical',
            createdAt: '2024-01-10T08:00:00Z',
            updatedAt: '2024-01-10T08:00:00Z',
            lastVisit: '2024-01-09T16:45:00Z'
          },
          {
            id: '4',
            firstName: 'Sarah',
            lastName: 'Wilson',
            email: 'sarah.wilson@email.com',
            phone: '+1-555-0126',
            dateOfBirth: '1992-07-18',
            gender: 'Female',
            medicalHistory: 'No significant medical history',
            department: 'emergency',
            status: 'active',
            createdAt: '2024-01-14T12:00:00Z',
            updatedAt: '2024-01-14T12:00:00Z',
            lastVisit: '2024-01-13T20:15:00Z'
          },
          {
            id: '5',
            firstName: 'David',
            lastName: 'Brown',
            email: 'david.brown@email.com',
            phone: '+1-555-0127',
            dateOfBirth: '2010-02-14',
            gender: 'Male',
            medicalHistory: 'Asthma, Regular check-ups',
            department: 'pediatrics',
            status: 'active',
            createdAt: '2024-01-11T14:00:00Z',
            updatedAt: '2024-01-11T14:00:00Z',
            lastVisit: '2024-01-07T10:30:00Z'
          }
        ]
      }
      
      // Try to fetch from API if available
      try {
        const response = await api.get('/patients')
        if (response.data && response.data.length > 0) {
          patients.value = response.data
        }
      } catch (apiError) {
        console.log('API not available, using sample data')
      }
    } catch (err: any) {
      error.value = err.response?.data?.message || err.message || 'Failed to fetch patients'
      console.error('Error fetching patients:', err)
    } finally {
      isLoading.value = false
    }
  }

  const fetchAppointments = async () => {
    isLoading.value = true
    error.value = null
    
    try {
      // For demo purposes, create sample appointments data
      if (appointments.value.length === 0) {
        appointments.value = [
          {
            id: '1',
            patientId: '1',
            patientName: 'John Smith',
            department: 'cardiology',
            doctor: 'Dr. Sarah Johnson',
            appointmentType: 'Follow-up',
            dateTime: '2024-01-20T10:00:00Z',
            duration: 30,
            status: 'scheduled',
            priority: 'normal',
            reason: 'Blood pressure check',
            symptoms: 'Shortness of breath, chest pain',
            insurance: 'Blue Cross Blue Shield',
            specialRequirements: {
              wheelchairAccess: false,
              interpreter: false,
              followUp: true
            },
            notes: 'Patient reports improvement in symptoms'
          },
          {
            id: '2',
            patientId: '2',
            patientName: 'Emily Johnson',
            department: 'oncology',
            doctor: 'Dr. Michael Chen',
            appointmentType: 'Consultation',
            dateTime: '2024-01-19T14:00:00Z',
            duration: 60,
            status: 'scheduled',
            priority: 'urgent',
            reason: 'Routine cancer screening',
            symptoms: 'None reported',
            insurance: 'Aetna',
            specialRequirements: {
              wheelchairAccess: false,
              interpreter: false,
              followUp: true
            }
          }
        ]
      }
      
      try {
        const response = await api.get('/appointments')
        if (response.data && response.data.length > 0) {
          appointments.value = response.data
        }
      } catch (apiError) {
        console.log('API not available, using sample appointments data')
      }
    } catch (err: any) {
      error.value = err.response?.data?.message || err.message || 'Failed to fetch appointments'
      console.error('Error fetching appointments:', err)
    } finally {
      isLoading.value = false
    }
  }

  const addPatient = async (patient: Omit<Patient, 'id' | 'createdAt' | 'updatedAt'>) => {
    isLoading.value = true
    error.value = null
    
    try {
      const newPatient: Patient = {
        ...patient,
        id: crypto.randomUUID(),
        createdAt: new Date().toISOString(),
        updatedAt: new Date().toISOString()
      }
      
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
      const index = patients.value.findIndex(p => p.id === id)
      if (index !== -1) {
        patients.value[index] = {
          ...patients.value[index],
          ...updates,
          updatedAt: new Date().toISOString()
        }
      }
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
      const index = patients.value.findIndex(p => p.id === id)
      if (index !== -1) {
        patients.value.splice(index, 1)
      }
    } catch (err: any) {
      error.value = err.response?.data?.message || err.message || 'Failed to delete patient'
      throw err
    } finally {
      isLoading.value = false
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
    fetchAppointments,
    addPatient,
    updatePatient,
    deletePatient
  }
})
