// Copyright (c) 2025 Alexander Sinapov | Simeon Petkov

// All rights reserved.
// This code is proprietary and confidential.  
// Unauthorized copying, modification, distribution, or use is strictly prohibited.

import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import api from '../services/api'

export interface SupportTicket {
  id: string
  userId: string
  assignedAgentId?: string
  subject: string
  description: string
  status: 'Open' | 'InProgress' | 'Waiting' | 'Resolved' | 'Closed'
  priority: 'Low' | 'Medium' | 'High' | 'Critical'
  category: 'Technical' | 'Billing' | 'General' | 'Feature' | 'Bug'
  createdAt: string
  updatedAt: string
  resolvedAt?: string
  resolutionNote?: string
  user?: any
  assignedAgent?: any
  messages?: TicketMessage[]
  attachments?: any[]
}

export interface TicketMessage {
  id: string
  ticketId: string
  senderId: string
  content: string
  isFromAgent: boolean
  createdAt: string
  sender?: any
}

export interface CreateTicketRequest {
  subject: string
  description: string
  priority: number // 0=Low, 1=Medium, 2=High, 3=Critical
  category: number // 0=Technical, 1=Billing, 2=General, 3=Feature, 4=Bug
}

export interface UpdateTicketRequest {
  status?: 'Open' | 'InProgress' | 'Waiting' | 'Resolved' | 'Closed'
  priority?: 'Low' | 'Medium' | 'High' | 'Critical'
  category?: 'Technical' | 'Billing' | 'General' | 'Feature' | 'Bug'
  assignedAgentId?: string
  resolutionNote?: string
}

export interface TicketStats {
  totalTickets: number
  openTickets: number
  inProgressTickets: number
  resolvedTickets: number
  closedTickets: number
  myAssignedTickets: number
  averageResolutionTime: number
}

export const useSupportStore = defineStore('support', () => {
  // State
  const userTickets = ref<SupportTicket[]>([])
  const allTickets = ref<SupportTicket[]>([])
  const currentTicket = ref<SupportTicket | null>(null)
  const currentMessages = ref<TicketMessage[]>([])
  const supportAgents = ref<any[]>([])
  const ticketStats = ref<TicketStats | null>(null)
  const loading = ref(false)
  const error = ref<string | null>(null)

  // Computed
  const isLoading = computed(() => loading.value)
  const hasError = computed(() => error.value !== null)

  // Actions
  const clearError = () => {
    error.value = null
  }

  const setLoading = (isLoading: boolean) => {
    loading.value = isLoading
  }

  const setError = (errorMessage: string) => {
    error.value = errorMessage
    loading.value = false
  }

  // User ticket operations
  const createTicket = async (ticketData: CreateTicketRequest): Promise<SupportTicket> => {
    setLoading(true)
    clearError()

    try {
      const response = await api.post('/support/tickets', ticketData)
      
      if (response.data.success) {
        const newTicket = response.data.ticket
        userTickets.value.unshift(newTicket)
        return newTicket
      } else {
        throw new Error(response.data.message || 'Failed to create ticket')
      }
    } catch (err: any) {
      const errorMessage = err.response?.data?.message || err.message || 'Failed to create ticket'
      setError(errorMessage)
      throw new Error(errorMessage)
    } finally {
      setLoading(false)
    }
  }

  const loadUserTickets = async (): Promise<void> => {
    setLoading(true)
    clearError()

    try {
      const response = await api.get('/support/tickets')
      
      if (response.data.success) {
        userTickets.value = response.data.tickets || []
      } else {
        throw new Error(response.data.message || 'Failed to load tickets')
      }
    } catch (err: any) {
      const errorMessage = err.response?.data?.message || err.message || 'Failed to load tickets'
      setError(errorMessage)
    } finally {
      setLoading(false)
    }
  }

  const loadTicketById = async (ticketId: string): Promise<SupportTicket | null> => {
    setLoading(true)
    clearError()

    try {
      const response = await api.get(`/support/tickets/${ticketId}`)
      
      if (response.data.success) {
        currentTicket.value = response.data.ticket
        return response.data.ticket
      } else {
        throw new Error(response.data.message || 'Failed to load ticket')
      }
    } catch (err: any) {
      const errorMessage = err.response?.data?.message || err.message || 'Failed to load ticket'
      setError(errorMessage)
      return null
    } finally {
      setLoading(false)
    }
  }

  const sendMessage = async (ticketId: string, content: string): Promise<TicketMessage> => {
    setLoading(true)
    clearError()

    try {
      const response = await api.post(`/support/tickets/${ticketId}/messages`, { content })
      
      if (response.data.success) {
        const newMessage = response.data.ticketMessage
        currentMessages.value.push(newMessage)
        return newMessage
      } else {
        throw new Error(response.data.message || 'Failed to send message')
      }
    } catch (err: any) {
      const errorMessage = err.response?.data?.message || err.message || 'Failed to send message'
      setError(errorMessage)
      throw new Error(errorMessage)
    } finally {
      setLoading(false)
    }
  }

  const loadTicketMessages = async (ticketId: string): Promise<void> => {
    setLoading(true)
    clearError()

    try {
      const response = await api.get(`/support/tickets/${ticketId}/messages`)
      
      if (response.data.success) {
        currentMessages.value = response.data.messages || []
      } else {
        throw new Error(response.data.message || 'Failed to load messages')
      }
    } catch (err: any) {
      const errorMessage = err.response?.data?.message || err.message || 'Failed to load messages'
      setError(errorMessage)
    } finally {
      setLoading(false)
    }
  }

  // Agent operations
  const loadAllTickets = async (status?: string, assignedAgentId?: string): Promise<void> => {
    setLoading(true)
    clearError()

    try {
      const params = new URLSearchParams()
      if (status) params.append('status', status)
      if (assignedAgentId) params.append('assignedAgentId', assignedAgentId)

      const response = await api.get(`/support/agent/tickets?${params.toString()}`)
      
      if (response.data.success) {
        allTickets.value = response.data.tickets || []
      } else {
        throw new Error(response.data.message || 'Failed to load tickets')
      }
    } catch (err: any) {
      const errorMessage = err.response?.data?.message || err.message || 'Failed to load tickets'
      setError(errorMessage)
    } finally {
      setLoading(false)
    }
  }

  const updateTicket = async (ticketId: string, updates: UpdateTicketRequest): Promise<SupportTicket> => {
    setLoading(true)
    clearError()

    try {
      const response = await api.put(`/support/agent/tickets/${ticketId}`, updates)
      
      if (response.data.success) {
        const updatedTicket = response.data.ticket
        
        // Update in allTickets if present
        const allIndex = allTickets.value.findIndex(t => t.id === ticketId)
        if (allIndex !== -1) {
          allTickets.value[allIndex] = updatedTicket
        }
        
        // Update in userTickets if present
        const userIndex = userTickets.value.findIndex(t => t.id === ticketId)
        if (userIndex !== -1) {
          userTickets.value[userIndex] = updatedTicket
        }
        
        // Update current ticket if it's the same
        if (currentTicket.value?.id === ticketId) {
          currentTicket.value = updatedTicket
        }
        
        return updatedTicket
      } else {
        throw new Error(response.data.message || 'Failed to update ticket')
      }
    } catch (err: any) {
      const errorMessage = err.response?.data?.message || err.message || 'Failed to update ticket'
      setError(errorMessage)
      throw new Error(errorMessage)
    } finally {
      setLoading(false)
    }
  }

  const loadTicketStats = async (agentId?: string): Promise<void> => {
    setLoading(true)
    clearError()

    try {
      const params = agentId ? `?agentId=${agentId}` : ''
      const response = await api.get(`/support/agent/stats${params}`)
      
      ticketStats.value = response.data || null
    } catch (err: any) {
      const errorMessage = err.response?.data?.message || err.message || 'Failed to load stats'
      setError(errorMessage)
    } finally {
      setLoading(false)
    }
  }

  const loadSupportAgents = async (): Promise<void> => {
    setLoading(true)
    clearError()

    try {
      const response = await api.get('/support/agent/agents')
      
      supportAgents.value = response.data || []
    } catch (err: any) {
      const errorMessage = err.response?.data?.message || err.message || 'Failed to load agents'
      setError(errorMessage)
    } finally {
      setLoading(false)
    }
  }

  // Utility functions
  const getTicketById = (ticketId: string): SupportTicket | undefined => {
    return userTickets.value.find(t => t.id === ticketId) || 
           allTickets.value.find(t => t.id === ticketId)
  }

  const clearCurrentTicket = () => {
    currentTicket.value = null
    currentMessages.value = []
  }

  return {
    // State
    userTickets,
    allTickets,
    currentTicket,
    currentMessages,
    supportAgents,
    ticketStats,
    loading,
    error,

    // Computed
    isLoading,
    hasError,

    // Actions
    clearError,
    setLoading,
    setError,
    createTicket,
    loadUserTickets,
    loadTicketById,
    sendMessage,
    loadTicketMessages,
    loadAllTickets,
    updateTicket,
    loadTicketStats,
    loadSupportAgents,
    getTicketById,
    clearCurrentTicket
  }
})