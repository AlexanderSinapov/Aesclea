// Copyright (c) 2025 Alexander Sinapov | Simeon Petkov

// All rights reserved.
// This code is proprietary and confidential.  
// Unauthorized copying, modification, distribution, or use is strictly prohibited.

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
  
  // Circuit breaker to prevent infinite API loops
  const apiCallCounts = ref({
    loadUserSubscription: 0,
    loadAvailablePlans: 0
  })
  const maxApiCalls = 5 // Maximum calls per page load
  
  const canMakeApiCall = (apiName: keyof typeof apiCallCounts.value) => {
    const count = apiCallCounts.value[apiName]
    if (count >= maxApiCalls) {
      console.warn(`Subscription store: ${apiName} blocked - too many calls (${count})`)
      return false
    }
    return true
  }

  const hasActiveSubscription = computed(() => {
    return userSubscription.value?.status === 'active' || userSubscription.value?.status === 'trialing'
  })

  const canAccessAI = computed(() => {
    console.log('Checking AI access:', {
      hasActiveSubscription: hasActiveSubscription.value,
      userSubscription: userSubscription.value,
      aiAnalysisLimit: userSubscription.value?.plan?.aiAnalysisLimit
    })
    
    if (!hasActiveSubscription.value) {
      console.log('No active subscription, AI access denied')
      return false
    }
    
    const subscription = userSubscription.value
    if (!subscription || !subscription.plan) {
      console.log('No subscription or plan found, AI access denied')
      return false
    }
    
    // Check if user has remaining AI analyses
    const hasAccess = subscription.plan.aiAnalysisLimit === -1 || subscription.plan.aiAnalysisLimit > 0
    console.log('AI access result:', hasAccess, 'limit:', subscription.plan.aiAnalysisLimit)
    return hasAccess
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
      const response = await api.post('/subscriptions/subscribe', {
        planId,
        paymentMethodId
      })
      
      userSubscription.value = response.data
      
      // Also reload user subscription to ensure we have the latest data
      await loadUserSubscription()
      
      console.log('Subscription successful:', userSubscription.value)
      
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
      const response = await api.post(`/subscriptions/cancel/${userSubscription.value.id}`)
      
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
    // Circuit breaker protection
    if (!canMakeApiCall('loadUserSubscription')) {
      console.error('Subscription store: loadUserSubscription blocked by circuit breaker')
      return
    }
    
    // Prevent multiple concurrent calls
    if (isLoading.value) {
      console.log('Subscription store: loadUserSubscription already in progress, skipping...')
      return
    }
    
    console.log('Subscription store: Loading user subscription... (attempt #' + (apiCallCounts.value.loadUserSubscription + 1) + ')')
    apiCallCounts.value.loadUserSubscription++
    isLoading.value = true
    error.value = null

    try {
      const response = await api.get('/subscriptions/current')
      // Handle the nested response structure
      if (response.data && response.data.subscription) {
        userSubscription.value = response.data.subscription
      } else {
        userSubscription.value = null
      }
      console.log('Subscription store: User subscription loaded successfully:', userSubscription.value)
    } catch (err: any) {
      console.error('Subscription store: Error loading user subscription:', err)
      error.value = err.response?.data?.message || err.message || 'Failed to load subscription'
      userSubscription.value = null
      
      // Don't throw error to prevent potential retry loops
    } finally {
      isLoading.value = false
    }
  }

  const clearError = () => {
    error.value = null
  }

  const loadAvailablePlans = async () => {
    // Circuit breaker protection
    if (!canMakeApiCall('loadAvailablePlans')) {
      console.error('Subscription store: loadAvailablePlans blocked by circuit breaker')
      return
    }
    
    // Prevent multiple concurrent calls
    if (isLoading.value) {
      console.log('Subscription store: loadAvailablePlans already in progress, skipping...')
      return
    }
    
    console.log('Subscription store: Loading available plans... (attempt #' + (apiCallCounts.value.loadAvailablePlans + 1) + ')')
    apiCallCounts.value.loadAvailablePlans++
    isLoading.value = true
    error.value = null

    try {
      const response = await api.get('/subscriptions/plans')
      availablePlans.value = response.data
      console.log('Subscription store: Available plans loaded successfully:', availablePlans.value.length, 'plans')
      
      // Reset circuit breaker on success
      apiCallCounts.value.loadAvailablePlans = 0
    } catch (err: any) {
      console.error('Subscription store: Error loading available plans:', err)
      error.value = err.response?.data?.message || err.message || 'Failed to load plans'
      
      // Don't throw error to prevent potential retry loops
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
      const response = await api.post(`/subscriptions/reactivate/${userSubscription.value.id}`)
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
      const response = await api.put('/subscriptions/change-plan', {
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
