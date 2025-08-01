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
        const response = await api.get('/api/appointments')
        this.appointments = response.data
      } catch (error: any) {
        this.error = error.response?.data?.message || 'Failed to fetch appointments'
        console.error('Error fetching appointments:', error)
      } finally {
        this.loading = false
      }
    },

    async fetchAppointment(id: string) {
      this.loading = true
      this.error = null
      try {
        const response = await api.get(`/api/appointments/${id}`)
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
        const response = await api.post('/api/appointments', appointmentData)
        const newAppointment = response.data
        this.appointments.push(newAppointment)
        return newAppointment
      } catch (error: any) {
        this.error = error.response?.data?.message || 'Failed to create appointment'
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
        const response = await api.put(`/api/appointments/${id}`, appointmentData)
        const updatedAppointment = response.data
        const index = this.appointments.findIndex(a => a.id === id)
        if (index !== -1) {
          this.appointments[index] = updatedAppointment
        }
        return updatedAppointment
      } catch (error: any) {
        this.error = error.response?.data?.message || 'Failed to update appointment'
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
        await api.delete(`/api/appointments/${id}`)
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
        const response = await api.get(`/api/appointments/patient/${patientId}`)
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
        const response = await api.get('/api/appointments/today')
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
