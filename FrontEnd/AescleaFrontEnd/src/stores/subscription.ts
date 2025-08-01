import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import api from '../services/api'

export interface SubscriptionPlan {
  id: string
  name: string
  price: number
  interval: 'month' | 'year'
  features: string[]
  maxPatients: number
  aiAnalysisLimit: number
  priority: number
  recommended?: boolean
}

export interface UserSubscription {
  id: string
  planId: string
  status: 'active' | 'canceled' | 'past_due' | 'trialing' | 'incomplete'
  currentPeriodStart: string
  currentPeriodEnd: string
  cancelAtPeriodEnd: boolean
  plan: SubscriptionPlan
}

export const useSubscriptionStore = defineStore('subscription', () => {
  const userSubscription = ref<UserSubscription | null>(null)
  const availablePlans = ref<SubscriptionPlan[]>([])
  const isLoading = ref(false)
  const error = ref<string | null>(null)

  const hasActiveSubscription = computed(() => {
    return userSubscription.value?.status === 'active' || userSubscription.value?.status === 'trialing'
  })

  const canAccessAI = computed(() => {
    if (!hasActiveSubscription.value) return false
    const subscription = userSubscription.value
    if (!subscription) return false
    
    // Check if user has remaining AI analyses
    return subscription.plan.aiAnalysisLimit === -1 || subscription.plan.aiAnalysisLimit > 0
  })

  const currentPlan = computed(() => {
    return userSubscription.value?.plan || null
  })

  const needsSubscription = computed(() => {
    return !hasActiveSubscription.value
  })

  const subscribeToPlan = async (planId: string, paymentMethodId: string = 'pm_test_card') => {
    isLoading.value = true
    error.value = null

    try {
      const response = await api.post('/api/subscriptions/subscribe', {
        planId,
        paymentMethodId
      })
      
      userSubscription.value = response.data
      
      return { success: true }
    } catch (err: any) {
      error.value = err.response?.data?.message || err.message || 'Failed to subscribe'
      throw err
    } finally {
      isLoading.value = false
    }
  }

  const cancelSubscription = async () => {
    if (!userSubscription.value) {
      throw new Error('No active subscription found')
    }

    isLoading.value = true
    error.value = null

    try {
      const response = await api.post(`/api/subscriptions/cancel/${userSubscription.value.id}`)
      
      userSubscription.value.cancelAtPeriodEnd = true
      
      return response.data
    } catch (err: any) {
      error.value = err.response?.data?.message || err.message || 'Failed to cancel subscription'
      throw err
    } finally {
      isLoading.value = false
    }
  }

  const loadUserSubscription = async () => {
    isLoading.value = true
    error.value = null

    try {
      const response = await api.get('/api/subscriptions/user/current-user')
      userSubscription.value = response.data
    } catch (err: any) {
      error.value = err.response?.data?.message || err.message || 'Failed to load subscription'
      userSubscription.value = null
    } finally {
      isLoading.value = false
    }
  }

  const clearError = () => {
    error.value = null
  }

  const loadAvailablePlans = async () => {
    isLoading.value = true
    error.value = null

    try {
      const response = await api.get('/api/subscriptions/plans')
      availablePlans.value = response.data
    } catch (err: any) {
      error.value = err.response?.data?.message || err.message || 'Failed to load plans'
    } finally {
      isLoading.value = false
    }
  }

  const reactivateSubscription = async () => {
    if (!userSubscription.value) {
      throw new Error('No subscription found')
    }

    isLoading.value = true
    error.value = null

    try {
      const response = await api.post(`/api/subscriptions/reactivate/${userSubscription.value.id}`)
      userSubscription.value = response.data
      return response.data
    } catch (err: any) {
      error.value = err.response?.data?.message || err.message || 'Failed to reactivate subscription'
      throw err
    } finally {
      isLoading.value = false
    }
  }

  const changePlan = async (planId: string, paymentMethodId: string = 'pm_test_change') => {
    isLoading.value = true
    error.value = null

    try {
      const response = await api.put('/api/subscriptions/change-plan', {
        planId,
        paymentMethodId
      })
      
      userSubscription.value = response.data
      return response.data
    } catch (err: any) {
      error.value = err.response?.data?.message || err.message || 'Failed to change plan'
      throw err
    } finally {
      isLoading.value = false
    }
  }

  return {
    userSubscription,
    availablePlans,
    isLoading,
    error,
    hasActiveSubscription,
    canAccessAI,
    currentPlan,
    needsSubscription,
    subscribeToPlan,
    cancelSubscription,
    loadUserSubscription,
    loadAvailablePlans,
    reactivateSubscription,
    changePlan,
    clearError
  }
})
