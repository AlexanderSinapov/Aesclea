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
  const isLoading = ref(false) // General loading state
  const isLoadingUserSubscription = ref(false) // Specific to user subscription
  const isLoadingAvailablePlans = ref(false) // Specific to available plans
  const error = ref<string | null>(null)
  
  // Circuit breaker to prevent infinite API loops
  const sessionId = Date.now() // Unique session identifier
  const apiCallCounts = ref({
    loadUserSubscription: 0,
    loadAvailablePlans: 0
  })
  const maxApiCalls = 2 // Very restrictive to prevent loops
  const lastResetTime = ref(Date.now())
  const lastApiCallTimes = ref({
    loadUserSubscription: 0,
    loadAvailablePlans: 0
  })
  const minTimeBetweenCalls = 1000 // Minimum 1 second between calls
  
  const resetCircuitBreaker = () => {
    apiCallCounts.value.loadUserSubscription = 0
    apiCallCounts.value.loadAvailablePlans = 0
    lastApiCallTimes.value.loadUserSubscription = 0
    lastApiCallTimes.value.loadAvailablePlans = 0
    lastResetTime.value = Date.now()
    console.log('Subscription store: Circuit breaker reset')
  }
  
  const canMakeApiCall = (apiName: keyof typeof apiCallCounts.value) => {
    const now = Date.now()
    
    // Auto-reset circuit breaker after 30 seconds
    if (now - lastResetTime.value > 30000) {
      resetCircuitBreaker()
    }
    
    // Check if we're calling too frequently (prevents rapid-fire loops)
    const lastCallTime = lastApiCallTimes.value[apiName]
    if (lastCallTime && (now - lastCallTime) < minTimeBetweenCalls) {
      console.warn(`Subscription store: ${apiName} blocked - called too recently (${now - lastCallTime}ms ago). Session: ${sessionId}`)
      return false
    }
    
    const count = apiCallCounts.value[apiName]
    if (count >= maxApiCalls) {
      console.warn(`Subscription store: ${apiName} blocked - too many calls (${count}). Session: ${sessionId}`)
      return false
    }
    
    // Record this call time
    lastApiCallTimes.value[apiName] = now
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
      
      // Directly set the subscription data from response
      userSubscription.value = response.data
      
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
      console.error(`Subscription store: loadUserSubscription blocked by circuit breaker (session: ${sessionId})`)
      return
    }
    
    // Prevent multiple concurrent calls of this specific method
    if (isLoadingUserSubscription.value) {
      console.log(`Subscription store: loadUserSubscription already in progress, skipping... (session: ${sessionId})`)
      return
    }
    
    console.log(`Subscription store: Loading user subscription... (attempt #${apiCallCounts.value.loadUserSubscription + 1}, session: ${sessionId})`)
    apiCallCounts.value.loadUserSubscription++
    isLoadingUserSubscription.value = true
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
      console.log(`Subscription store: User subscription loaded successfully (session: ${sessionId}):`, userSubscription.value)
      
      // Reset circuit breaker on successful load
      if (apiCallCounts.value.loadUserSubscription > 1) {
        console.log('Subscription store: Resetting loadUserSubscription counter after success')
        apiCallCounts.value.loadUserSubscription = 0
      }
    } catch (err: any) {
      console.error(`Subscription store: Error loading user subscription (session: ${sessionId}):`, err)
      error.value = err.response?.data?.message || err.message || 'Failed to load subscription'
      userSubscription.value = null
      
      // Don't throw error to prevent potential retry loops
    } finally {
      isLoadingUserSubscription.value = false
      isLoading.value = false
    }
  }

  const clearError = () => {
    error.value = null
  }

  const loadAvailablePlans = async () => {
    // Circuit breaker protection
    if (!canMakeApiCall('loadAvailablePlans')) {
      console.error(`Subscription store: loadAvailablePlans blocked by circuit breaker (session: ${sessionId})`)
      return
    }
    
    // Prevent multiple concurrent calls of this specific method
    if (isLoadingAvailablePlans.value) {
      console.log(`Subscription store: loadAvailablePlans already in progress, skipping... (session: ${sessionId})`)
      return
    }
    
    console.log(`Subscription store: Loading available plans... (attempt #${apiCallCounts.value.loadAvailablePlans + 1}, session: ${sessionId})`)
    apiCallCounts.value.loadAvailablePlans++
    isLoadingAvailablePlans.value = true
    isLoading.value = true
    error.value = null

    try {
      const response = await api.get('/subscriptions/plans')
      availablePlans.value = response.data
      console.log(`Subscription store: Available plans loaded successfully (session: ${sessionId}):`, availablePlans.value.length, 'plans')
      
      // Reset circuit breaker on successful load
      if (apiCallCounts.value.loadAvailablePlans > 1) {
        console.log('Subscription store: Resetting loadAvailablePlans counter after success')
        apiCallCounts.value.loadAvailablePlans = 0
      }
    } catch (err: any) {
      console.error(`Subscription store: Error loading available plans (session: ${sessionId}):`, err)
      error.value = err.response?.data?.message || err.message || 'Failed to load plans'
      
      // Don't throw error to prevent potential retry loops
    } finally {
      isLoadingAvailablePlans.value = false
      // Only set general loading to false if both specific loading states are false
      if (!isLoadingUserSubscription.value && !isLoadingAvailablePlans.value) {
        isLoading.value = false
      }
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
    isLoadingUserSubscription,
    isLoadingAvailablePlans,
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
    clearError,
    resetCircuitBreaker
  }
})
