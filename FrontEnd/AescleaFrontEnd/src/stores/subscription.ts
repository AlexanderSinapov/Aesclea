import { defineStore } from 'pinia'
import { ref, computed } from 'vue'

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
  const availablePlans = ref<SubscriptionPlan[]>([
    {
      id: 'starter',
      name: 'Starter',
      price: 49,
      interval: 'month',
      maxPatients: 50,
      aiAnalysisLimit: 10,
      priority: 1,
      features: [
        'Up to 50 patients',
        '10 AI analyses per month',
        'Basic reporting',
        'Email support',
        'Secure data storage'
      ]
    },
    {
      id: 'professional',
      name: 'Professional',
      price: 149,
      interval: 'month',
      maxPatients: 200,
      aiAnalysisLimit: 50,
      priority: 2,
      recommended: true,
      features: [
        'Up to 200 patients',
        '50 AI analyses per month',
        'Advanced reporting',
        'Priority email support',
        'Secure data storage',
        'Custom templates',
        'Team collaboration'
      ]
    },
    {
      id: 'enterprise',
      name: 'Enterprise',
      price: 399,
      interval: 'month',
      maxPatients: -1, // Unlimited
      aiAnalysisLimit: -1, // Unlimited
      priority: 3,
      features: [
        'Unlimited patients',
        'Unlimited AI analyses',
        'Advanced reporting & analytics',
        '24/7 phone & email support',
        'Secure data storage',
        'Custom templates',
        'Team collaboration',
        'API access',
      ]
    }
  ])
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

  const subscribeToPlan = async (planId: string) => {
    isLoading.value = true
    error.value = null

    try {
      // This would make an API call to your backend
      // For now, we'll simulate it
      const plan = availablePlans.value.find(p => p.id === planId)
      if (!plan) {
        throw new Error('Plan not found')
      }

      // Simulate API call
      await new Promise(resolve => setTimeout(resolve, 2000))
      
      userSubscription.value = {
        id: `sub_${Date.now()}`,
        planId: plan.id,
        status: 'active',
        currentPeriodStart: new Date().toISOString(),
        currentPeriodEnd: new Date(Date.now() + 30 * 24 * 60 * 60 * 1000).toISOString(),
        cancelAtPeriodEnd: false,
        plan
      }

      return { success: true }
    } catch (err: any) {
      error.value = err.message
      throw err
    } finally {
      isLoading.value = false
    }
  }

  const cancelSubscription = async () => {
    if (!userSubscription.value) return

    isLoading.value = true
    error.value = null

    try {
      // Make API call to cancel subscription
      await new Promise(resolve => setTimeout(resolve, 1000))
      
      userSubscription.value.cancelAtPeriodEnd = true
      
      return { success: true }
    } catch (err: any) {
      error.value = err.message
      throw err
    } finally {
      isLoading.value = false
    }
  }

  const loadUserSubscription = async () => {
    isLoading.value = true
    error.value = null

    try {
      // Make API call to get user's subscription
      // For now, simulate no subscription
      await new Promise(resolve => setTimeout(resolve, 500))
      userSubscription.value = null
    } catch (err: any) {
      error.value = err.message
    } finally {
      isLoading.value = false
    }
  }

  const clearError = () => {
    error.value = null
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
    clearError
  }
})
