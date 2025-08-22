<template>
  <div v-if="isOpen" class="fixed inset-0 z-50 w-full h-full overflow-y-auto bg-gray-600 bg-opacity-50">
    <div class="relative w-11/12 max-w-3xl p-5 mx-auto bg-white border rounded-md shadow-lg top-20 dark:bg-gray-800">
      <!-- Modal Header -->
      <div class="flex items-center justify-between pb-4 border-b border-gray-200 dark:border-gray-700">
        <h3 class="text-xl font-semibold text-gray-900 dark:text-white">
          Change Your Plan
        </h3>
        <button
          @click="$emit('close')"
          class="text-gray-400 hover:text-gray-600 dark:hover:text-gray-300"
        >
          <span class="sr-only">Close</span>
          ✕
        </button>
      </div>

      <!-- Current Plan -->
      <div class="py-4 border-b border-gray-200 dark:border-gray-700">
        <p class="text-sm text-gray-600 dark:text-gray-400">Current Plan:</p>
        <p class="text-lg font-semibold text-gray-900 dark:text-white">
          {{ subscriptionStore.currentPlan?.name }} - ${{ subscriptionStore.currentPlan?.price }}/{{ subscriptionStore.currentPlan?.interval }}
        </p>
      </div>

      <!-- Available Plans -->
      <div class="py-6">
        <h4 class="mb-4 text-lg font-medium text-gray-900 dark:text-white">Available Plans</h4>
        <div class="space-y-4">
          <div
            v-for="plan in otherPlans"
            :key="plan.id"
            class="p-4 transition-colors border rounded-lg cursor-pointer hover:border-purple-300"
            :class="[
              selectedPlanId === plan.id 
                ? 'border-purple-500 ring-2 ring-purple-500 ring-opacity-50' 
                : 'border-gray-200 dark:border-gray-700'
            ]"
            @click="selectedPlanId = plan.id"
          >
            <div class="flex items-center justify-between">
              <div>
                <h5 class="font-semibold text-gray-900 dark:text-white">{{ plan.name }}</h5>
                <p class="text-sm text-gray-500 dark:text-gray-400">
                  Up to {{ plan.maxPatients }} patients • {{ plan.aiAnalysisLimit }} AI analyses/month
                </p>
              </div>
              <div class="text-right">
                <p class="text-xl font-bold text-gray-900 dark:text-white">
                  ${{ plan.price }}
                </p>
                <p class="text-sm text-gray-500 dark:text-gray-400">
                  /{{ plan.interval }}
                </p>
              </div>
            </div>
            
            <!-- Price Difference -->
            <div v-if="selectedPlanId === plan.id && priceComparison" class="p-3 mt-3 rounded bg-gray-50 dark:bg-gray-700">
              <p class="text-sm" :class="priceComparison.isUpgrade ? 'text-blue-600 dark:text-blue-400' : 'text-green-600 dark:text-green-400'">
                {{ priceComparison.isUpgrade ? 'Upgrade' : 'Downgrade' }}: 
                {{ priceComparison.isUpgrade ? '+' : '-' }}${{ Math.abs(priceComparison.difference) }}/{{ plan.interval }}
              </p>
              <p class="mt-1 text-xs text-gray-500 dark:text-gray-400">
                {{ priceComparison.isUpgrade 
                  ? 'You will be charged the prorated amount immediately' 
                  : 'Credit will be applied to your next billing cycle' 
                }}
              </p>
            </div>
          </div>
        </div>
      </div>

      <!-- Actions -->
      <div class="flex justify-end pt-4 space-x-3 border-t border-gray-200 dark:border-gray-700">
        <button
          @click="$emit('close')"
          class="px-4 py-2 text-sm font-medium text-gray-700 bg-white border border-gray-300 rounded-md dark:border-gray-600 dark:text-gray-300 dark:bg-gray-700 hover:bg-gray-50 dark:hover:bg-gray-600"
        >
          Cancel
        </button>
        <button
          @click="changePlan"
          :disabled="!selectedPlanId || loading"
          class="px-4 py-2 text-sm font-medium text-white bg-purple-600 border border-transparent rounded-md hover:bg-purple-700 disabled:opacity-50 disabled:cursor-not-allowed"
        >
          <span v-if="loading">Changing Plan...</span>
          <span v-else>Change Plan</span>
        </button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import { useSubscriptionStore } from '../../stores/subscription'

interface Props {
  isOpen: boolean
}

const subscriptionStore = useSubscriptionStore()
const selectedPlanId = ref<string>('')
const loading = ref(false)

// Computed properties
const otherPlans = computed(() => {
  return subscriptionStore.availablePlans.filter(plan => 
    plan.id !== subscriptionStore.currentPlan?.id
  )
})

const priceComparison = computed(() => {
  if (!selectedPlanId.value || !subscriptionStore.currentPlan) return null
  
  const selectedPlan = subscriptionStore.availablePlans.find(p => p.id === selectedPlanId.value)
  if (!selectedPlan) return null
  
  const currentPrice = subscriptionStore.currentPlan.price
  const newPrice = selectedPlan.price
  const difference = newPrice - currentPrice
  
  return {
    difference,
    isUpgrade: difference > 0
  }
})

const changePlan = async () => {
  if (!selectedPlanId.value) return
  
  loading.value = true
  try {
    // For plan changes, we'll use a test payment method
    await subscriptionStore.changePlan(selectedPlanId.value, 'pm_test_change')
    
    // Show success message
    const selectedPlan = subscriptionStore.availablePlans.find(p => p.id === selectedPlanId.value)
    const message = `Successfully changed to ${selectedPlan?.name || 'new'} plan!`
    
    // You could show a toast notification here instead of alert
    if (window.confirm(`${message}\n\nClick OK to continue.`)) {
      emit('close')
    }
  } catch (error: any) {
    console.error('Error changing plan:', error)
    const errorMessage = error.response?.data?.message || error.message || 'Failed to change plan'
    alert(`Error: ${errorMessage}`)
  } finally {
    loading.value = false
  }
}

const emit = defineEmits<{
  close: []
}>()

// Reset selection when modal opens
watch(() => props.isOpen, (isOpen) => {
  if (isOpen) {
    selectedPlanId.value = ''
  }
})

const props = defineProps<Props>()
</script>
