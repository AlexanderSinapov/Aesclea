<template>
  <div v-if="isOpen" class="fixed inset-0 bg-gray-600 bg-opacity-50 overflow-y-auto h-full w-full z-50">
    <div class="relative top-20 mx-auto p-5 border w-11/12 max-w-3xl shadow-lg rounded-md bg-white dark:bg-gray-800">
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
        <h4 class="text-lg font-medium text-gray-900 dark:text-white mb-4">Available Plans</h4>
        <div class="space-y-4">
          <div
            v-for="plan in otherPlans"
            :key="plan.id"
            class="border rounded-lg p-4 hover:border-purple-300 transition-colors cursor-pointer"
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
            <div v-if="selectedPlanId === plan.id && priceComparison" class="mt-3 p-3 bg-gray-50 dark:bg-gray-700 rounded">
              <p class="text-sm" :class="priceComparison.isUpgrade ? 'text-blue-600 dark:text-blue-400' : 'text-green-600 dark:text-green-400'">
                {{ priceComparison.isUpgrade ? 'Upgrade' : 'Downgrade' }}: 
                {{ priceComparison.isUpgrade ? '+' : '-' }}${{ Math.abs(priceComparison.difference) }}/{{ plan.interval }}
              </p>
              <p class="text-xs text-gray-500 dark:text-gray-400 mt-1">
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
      <div class="flex justify-end space-x-3 pt-4 border-t border-gray-200 dark:border-gray-700">
        <button
          @click="$emit('close')"
          class="px-4 py-2 border border-gray-300 dark:border-gray-600 rounded-md text-sm font-medium text-gray-700 dark:text-gray-300 bg-white dark:bg-gray-700 hover:bg-gray-50 dark:hover:bg-gray-600"
        >
          Cancel
        </button>
        <button
          @click="changePlan"
          :disabled="!selectedPlanId || loading"
          class="px-4 py-2 border border-transparent rounded-md text-sm font-medium text-white bg-purple-600 hover:bg-purple-700 disabled:opacity-50 disabled:cursor-not-allowed"
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

defineProps<Props>()
defineEmits<{
  close: []
}>()

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
