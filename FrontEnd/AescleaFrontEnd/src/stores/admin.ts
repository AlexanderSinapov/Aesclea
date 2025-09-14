// Copyright (c) 2025 Alexander Sinapov | Simeon Petkov

// All rights reserved.
// This code is proprietary and confidential.  
// Unauthorized copying, modification, distribution, or use is strictly prohibited.

import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import api from '../services/api'

export interface AdminUser {
  id: string
  firstName: string
  lastName: string
  email: string
  role: 'super_admin' | 'admin' | 'manager'
  department?: string
  isActive: boolean
  lastLogin?: string
  createdAt: string
}

export interface SystemStats {
  totalUsers: number
  activeSubscriptions: number
  totalRevenue: number
  monthlyRevenue: number
  systemHealth: 'healthy' | 'warning' | 'critical'
  serverLoad: number
  systemLoad: number
  activeSessions: number
  databaseSize: number
  databaseStatus: 'online' | 'offline' | 'maintenance'
  apiRequests24h: number
  errorRate: number
}

export interface AuditLog {
  id: string
  userId: string
  userName: string
  action: string
  resource: string
  timestamp: string
  ipAddress: string
  userAgent: string
  level: 'info' | 'warning' | 'error' | 'critical'
  message: string
  details?: any
}

export const useAdminStore = defineStore('admin', () => {
  const adminUsers = ref<AdminUser[]>([])
  const systemStats = ref<SystemStats | null>(null)
  const auditLogs = ref<AuditLog[]>([])
  const isLoading = ref(false)
  const error = ref<string | null>(null)

  const activeAdmins = computed(() => 
    adminUsers.value.filter(user => user.isActive)
  )

  const adminsByRole = computed(() => {
    const roles: Record<string, AdminUser[]> = {}
    adminUsers.value.forEach(user => {
      if (!roles[user.role]) {
        roles[user.role] = []
      }
      roles[user.role].push(user)
    })
    return roles
  })

  const fetchAdminUsers = async () => {
    isLoading.value = true
    error.value = null
    
    try {
      const response = await api.get('/admin/users')
      adminUsers.value = response.data
    } catch (err: any) {
      error.value = err.response?.data?.message || err.message || 'Failed to fetch admin users'
      console.error('Error fetching admin users:', err)
    } finally {
      isLoading.value = false
    }
  }

  const fetchSystemStats = async () => {
    isLoading.value = true
    error.value = null
    
    try {
      const response = await api.get('/admin/stats')
      systemStats.value = response.data
    } catch (err: any) {
      error.value = err.response?.data?.message || err.message || 'Failed to fetch system stats'
      console.error('Error fetching system stats:', err)
    } finally {
      isLoading.value = false
    }
  }

  const fetchAuditLogs = async (limit = 100) => {
    isLoading.value = true
    error.value = null
    
    try {
      const response = await api.get(`/admin/audit-logs?limit=${limit}`)
      auditLogs.value = response.data
    } catch (err: any) {
      error.value = err.response?.data?.message || err.message || 'Failed to fetch audit logs'
      console.error('Error fetching audit logs:', err)
    } finally {
      isLoading.value = false
    }
  }

  const createAdminUser = async (userData: Omit<AdminUser, 'id' | 'createdAt' | 'lastLogin'>) => {
    isLoading.value = true
    error.value = null
    
    try {
      const response = await api.post('/admin/users', userData)
      const newUser = response.data.user
      adminUsers.value.push(newUser)
      return newUser
    } catch (err: any) {
      error.value = err.response?.data?.message || err.message || 'Failed to create admin user'
      throw err
    } finally {
      isLoading.value = false
    }
  }

  const updateAdminUser = async (id: string, updates: Partial<AdminUser>) => {
    isLoading.value = true
    error.value = null
    
    try {
      const response = await api.put(`/admin/users/${id}`, updates)
      const updatedUser = response.data.user
      const index = adminUsers.value.findIndex(u => u.id === id)
      if (index !== -1) {
        adminUsers.value[index] = updatedUser
      }
      return updatedUser
    } catch (err: any) {
      error.value = err.response?.data?.message || err.message || 'Failed to update admin user'
      throw err
    } finally {
      isLoading.value = false
    }
  }

  const deleteAdminUser = async (id: string) => {
    isLoading.value = true
    error.value = null
    
    try {
      await api.delete(`/admin/users/${id}`)
      adminUsers.value = adminUsers.value.filter(u => u.id !== id)
    } catch (err: any) {
      error.value = err.response?.data?.message || err.message || 'Failed to delete admin user'
      throw err
    } finally {
      isLoading.value = false
    }
  }

  const getAllUsers = async () => {
    try {
      const response = await api.get('/admin/all-users')
      return response.data
    } catch (err: any) {
      console.error('Error fetching all users:', err)
      throw err
    }
  }

  const getAllSubscriptions = async () => {
    try {
      const response = await api.get('/admin/subscriptions')
      return response.data
    } catch (err: any) {
      console.error('Error fetching all subscriptions:', err)
      throw err
    }
  }

  const updateSystemSettings = async (settings: any) => {
    try {
      const response = await api.put('/admin/settings', settings)
      return response.data
    } catch (err: any) {
      console.error('Error updating system settings:', err)
      throw err
    }
  }

  return {
    adminUsers,
    systemStats,
    auditLogs,
    isLoading,
    error,
    activeAdmins,
    adminsByRole,
    fetchAdminUsers,
    fetchSystemStats,
    fetchAuditLogs,
    createAdminUser,
    updateAdminUser,
    deleteAdminUser,
    getAllUsers,
    getAllSubscriptions,
    updateSystemSettings
  }
})
