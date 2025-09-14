// Copyright (c) 2025 Alexander Sinapov | Simeon Petkov

// All rights reserved.
// This code is proprietary and confidential.  
// Unauthorized copying, modification, distribution, or use is strictly prohibited.

import { defineStore } from 'pinia'
import api from '../services/api'

export interface Appointment {
  id: string
  patientId: string
  patientName: string
  department: string
  doctor: string
  appointmentType: string
  dateTime: string
  duration: number
  status: 'scheduled' | 'completed' | 'cancelled' | 'rescheduled'
  priority: 'normal' | 'urgent' | 'emergency'
  reason: string
  notes?: string
  createdAt: string
  updatedAt?: string
}

export const useAppointmentsStore = defineStore('appointments', {
  state: () => ({
    appointments: [] as Appointment[],
    loading: false,
    error: null as string | null
  }),

  getters: {
    getAppointmentById: (state) => (id: string) => {
      return state.appointments.find(appointment => appointment.id === id)
    },
    
    getAppointmentsByPatient: (state) => (patientId: string) => {
      return state.appointments.filter(appointment => appointment.patientId === patientId)
    },

    getTodayAppointments: (state) => {
      const today = new Date().toDateString()
      return state.appointments.filter(appointment => 
        new Date(appointment.dateTime).toDateString() === today
      )
    },

    getUpcomingAppointments: (state) => {
      const now = new Date()
      return state.appointments.filter(appointment => 
        new Date(appointment.dateTime) > now && appointment.status === 'scheduled'
      ).sort((a, b) => new Date(a.dateTime).getTime() - new Date(b.dateTime).getTime())
    }
  },

  actions: {
    async fetchAppointments() {
      this.loading = true
      this.error = null
      try {
        const response = await api.get('/appointments')
        this.appointments = response.data
      } catch (error: any) {
        console.warn('API not available, using sample appointments data:', error)
        // Provide sample appointments data for demo
        if (this.appointments.length === 0) {
          const tomorrow = new Date()
          tomorrow.setDate(tomorrow.getDate() + 1)
          const nextWeek = new Date()
          nextWeek.setDate(nextWeek.getDate() + 7)
          
          this.appointments = [
            {
              id: 'apt_1',
              patientId: '1',
              patientName: 'John Doe',
              department: 'cardiology',
              doctor: 'Dr. Smith (Cardiologist)',
              appointmentType: 'Follow-up',
              dateTime: tomorrow.toISOString(),
              duration: 30,
              status: 'scheduled',
              priority: 'normal',
              reason: 'Regular cardiac check-up',
              createdAt: new Date().toISOString(),
              updatedAt: new Date().toISOString()
            },
            {
              id: 'apt_2',
              patientId: '2',
              patientName: 'Jane Smith',
              department: 'neurology',
              doctor: 'Dr. Brown (Neurologist)',
              appointmentType: 'Consultation',
              dateTime: nextWeek.toISOString(),
              duration: 60,
              status: 'scheduled',
              priority: 'urgent',
              reason: 'Headache evaluation',
              createdAt: new Date().toISOString(),
              updatedAt: new Date().toISOString()
            }
          ]
        }
      } finally {
        this.loading = false
      }
    },

    async fetchAppointment(id: string) {
      this.loading = true
      this.error = null
      try {
        const response = await api.get(`/appointments/${id}`)
        const appointment = response.data
        const index = this.appointments.findIndex(a => a.id === id)
        if (index !== -1) {
          this.appointments[index] = appointment
        } else {
          this.appointments.push(appointment)
        }
        return appointment
      } catch (error: any) {
        this.error = error.response?.data?.message || 'Failed to fetch appointment'
        console.error('Error fetching appointment:', error)
        throw error
      } finally {
        this.loading = false
      }
    },

    async addAppointment(appointmentData: Omit<Appointment, 'id' | 'createdAt' | 'updatedAt'>) {
      this.loading = true
      this.error = null
      try {
        // Try API first
        try {
          const response = await api.post('/appointments', appointmentData)
          const newAppointment = response.data
          this.appointments.push(newAppointment)
          return newAppointment
        } catch (apiError) {
          // API failed, create local appointment
          console.warn('API unavailable, creating appointment locally:', apiError)
          const newAppointment = {
            ...appointmentData,
            id: `apt_${Date.now()}_${Math.random().toString(36).substr(2, 9)}`,
            createdAt: new Date().toISOString(),
            updatedAt: new Date().toISOString()
          }
          this.appointments.push(newAppointment)
          return newAppointment
        }
      } catch (error: any) {
        this.error = error.message || 'Failed to create appointment'
        console.error('Error creating appointment:', error)
        throw error
      } finally {
        this.loading = false
      }
    },

    async updateAppointment(id: string, appointmentData: Partial<Appointment>) {
      this.loading = true
      this.error = null
      try {
        // Try API first
        try {
          const response = await api.put(`/appointments/${id}`, appointmentData)
          const updatedAppointment = response.data
          const index = this.appointments.findIndex(a => a.id === id)
          if (index !== -1) {
            this.appointments[index] = updatedAppointment
          }
          return updatedAppointment
        } catch (apiError) {
          // API failed, update locally
          console.warn('API unavailable, updating appointment locally:', apiError)
          const index = this.appointments.findIndex(a => a.id === id)
          if (index !== -1) {
            this.appointments[index] = { 
              ...this.appointments[index], 
              ...appointmentData,
              updatedAt: new Date().toISOString()
            }
            return this.appointments[index]
          }
          throw new Error('Appointment not found')
        }
      } catch (error: any) {
        this.error = error.message || 'Failed to update appointment'
        console.error('Error updating appointment:', error)
        throw error
      } finally {
        this.loading = false
      }
    },

    async deleteAppointment(id: string) {
      this.loading = true
      this.error = null
      try {
        await api.delete(`/appointments/${id}`)
        this.appointments = this.appointments.filter(appointment => appointment.id !== id)
      } catch (error: any) {
        this.error = error.response?.data?.message || 'Failed to delete appointment'
        console.error('Error deleting appointment:', error)
        throw error
      } finally {
        this.loading = false
      }
    },

    async fetchAppointmentsByPatient(patientId: string) {
      this.loading = true
      this.error = null
      try {
        const response = await api.get(`/appointments/patient/${patientId}`)
        return response.data
      } catch (error: any) {
        this.error = error.response?.data?.message || 'Failed to fetch patient appointments'
        console.error('Error fetching patient appointments:', error)
        throw error
      } finally {
        this.loading = false
      }
    },

    async fetchTodayAppointments() {
      this.loading = true
      this.error = null
      try {
        const response = await api.get('/appointments/today')
        return response.data
      } catch (error: any) {
        this.error = error.response?.data?.message || 'Failed to fetch today\'s appointments'
        console.error('Error fetching today\'s appointments:', error)
        throw error
      } finally {
        this.loading = false
      }
    },

    clearError() {
      this.error = null
    }
  }
})
