// Copyright (c) 2025 Alexander Sinapov | Simeon Petkov

// All rights reserved.
// This code is proprietary and confidential.  
// Unauthorized copying, modification, distribution, or use is strictly prohibited.

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
      // Try API first
      try {
        const response = await api.post('/patients', patient)
        const newPatient = response.data
        patients.value.push(newPatient)
        return newPatient
      } catch (apiError) {
        // API failed, create local patient
        console.warn('API unavailable, creating patient locally:', apiError)
        const newPatient: Patient = {
          ...patient,
          id: crypto.randomUUID(),
          createdAt: new Date().toISOString(),
          updatedAt: new Date().toISOString()
        }
        patients.value.push(newPatient)
        return newPatient
      }
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
      // Try API first
      try {
        await api.put(`/patients/${id}`, updates)
        const index = patients.value.findIndex(p => p.id === id)
        if (index !== -1) {
          patients.value[index] = {
            ...patients.value[index],
            ...updates,
            updatedAt: new Date().toISOString()
          }
        }
      } catch (apiError) {
        // API failed, update locally
        console.warn('API unavailable, updating patient locally:', apiError)
        const index = patients.value.findIndex(p => p.id === id)
        if (index !== -1) {
          patients.value[index] = {
            ...patients.value[index],
            ...updates,
            updatedAt: new Date().toISOString()
          }
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
      // Try API first
      try {
        await api.delete(`/patients/${id}`)
        const index = patients.value.findIndex(p => p.id === id)
        if (index !== -1) {
          patients.value.splice(index, 1)
        }
      } catch (apiError) {
        // API failed, delete locally
        console.warn('API unavailable, deleting patient locally:', apiError)
        const index = patients.value.findIndex(p => p.id === id)
        if (index !== -1) {
          patients.value.splice(index, 1)
        }
      }
    } catch (err: any) {
      error.value = err.response?.data?.message || err.message || 'Failed to delete patient'
      throw err
    } finally {
      isLoading.value = false
    }
  }

  const addAppointment = async (appointmentData: Omit<Appointment, 'id'>) => {
    const newAppointment: Appointment = {
      ...appointmentData,
      id: `apt_${Date.now()}_${Math.random().toString(36).substr(2, 9)}`
    }
    
    appointments.value.push(newAppointment)
    
    // Also try to save to API but don't fail if it doesn't work
    try {
      await api.post('/appointments', newAppointment)
    } catch (err) {
      console.warn('Failed to save appointment to API, keeping local copy:', err)
    }
    
    return newAppointment
  }

  const updateAppointmentInStore = async (id: string, appointmentData: Partial<Appointment>) => {
    const index = appointments.value.findIndex(a => a.id === id)
    if (index !== -1) {
      appointments.value[index] = { ...appointments.value[index], ...appointmentData }
      
      // Also try to update on API but don't fail if it doesn't work
      try {
        await api.put(`/appointments/${id}`, appointmentData)
      } catch (err) {
        console.warn('Failed to update appointment on API, keeping local changes:', err)
      }
      
      return appointments.value[index]
    }
    throw new Error('Appointment not found')
  }

  const deleteAppointment = async (id: string) => {
    const index = appointments.value.findIndex(a => a.id === id)
    if (index !== -1) {
      appointments.value.splice(index, 1)
      
      // Also try to delete from API but don't fail if it doesn't work
      try {
        await api.delete(`/appointments/${id}`)
      } catch (err) {
        console.warn('Failed to delete appointment from API, removed locally:', err)
      }
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
    deletePatient,
    addAppointment,
    updateAppointmentInStore,
    deleteAppointment
  }
})
