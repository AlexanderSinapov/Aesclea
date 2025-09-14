<!-- Copyright (c) 2025 Alexander Sinapov | Simeon Petkov -->

<!-- All rights reserved. -->
<!-- This code is proprietary and confidential.   -->
<!-- Unauthorized copying, modification, distribution, or use is strictly prohibited. -->

<template>
  <div v-if="isOpen" class="fixed inset-0 z-50 w-full h-full overflow-y-auto bg-gray-600 bg-opacity-50">
    <div class="relative w-11/12 max-w-4xl p-5 mx-auto bg-white border rounded-md shadow-lg top-20 dark:bg-gray-800">
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

      <!-- Test Mode Warning -->
      <div class="py-4 border-b border-gray-200 dark:border-gray-700">
        <div class="flex items-center p-3 bg-yellow-50 border border-yellow-200 rounded-lg dark:bg-yellow-900/20 dark:border-yellow-800">
          <svg class="w-5 h-5 mr-2 text-yellow-600 dark:text-yellow-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-2.5L13.732 4c-.77-.833-1.964-.833-2.732 0L3.732 16.5c-.77.833.192 2.5 1.732 2.5z"></path>
          </svg>
          <div>
            <p class="text-sm font-medium text-yellow-800 dark:text-yellow-200">
              Test Mode Active
            </p>
            <p class="text-sm text-yellow-700 dark:text-yellow-300">
              No real payments will be processed. This is for testing purposes only.
            </p>
          </div>
        </div>
      </div>

      <!-- Current Plan Info -->
      <div class="py-4 border-b border-gray-200 dark:border-gray-700">
        <div class="flex items-center justify-between p-4 bg-blue-50 border border-blue-200 rounded-lg dark:bg-blue-900/20 dark:border-blue-800">
          <div>
            <h4 class="text-sm font-medium text-blue-900 dark:text-blue-100">
              Current Plan
            </h4>
            <p class="text-lg font-semibold text-blue-900 dark:text-blue-100">
              {{ subscriptionStore.currentPlan?.name }}
            </p>
            <p class="text-sm text-blue-700 dark:text-blue-300">
              ${{ subscriptionStore.currentPlan?.price }}/{{ subscriptionStore.currentPlan?.interval }}
            </p>
          </div>
          <div class="text-right">
            <p class="text-sm text-blue-700 dark:text-blue-300">
              Next billing: {{ formatDate(subscriptionStore.userSubscription?.currentPeriodEnd) }}
            </p>
          </div>
        </div>
      </div>

      <!-- Plans Grid -->
      <div class="py-6">
        <h4 class="mb-4 text-lg font-medium text-gray-900 dark:text-white">
          Select New Plan
        </h4>
        <div class="grid grid-cols-1 gap-6 md:grid-cols-3">
          <div
            v-for="plan in availablePlansFiltered"
            :key="plan.id"
            class="relative p-6 transition-all border rounded-lg shadow-sm hover:shadow-md"
            :class="[
              plan.id === subscriptionStore.currentPlan?.id
                ? 'border-blue-500 ring-2 ring-blue-500 ring-opacity-50 bg-blue-50 dark:bg-blue-900/20' 
                : 'border-gray-200 dark:border-gray-700 hover:border-purple-300 dark:hover:border-purple-600',
              plan.recommended 
                ? 'border-purple-500 ring-2 ring-purple-500 ring-opacity-50' 
                : ''
            ]"
          >
            <!-- Current Plan Badge -->
            <div v-if="plan.id === subscriptionStore.currentPlan?.id" class="absolute transform -translate-x-1/2 -top-3 left-1/2">
              <span class="px-3 py-1 text-sm font-medium text-white bg-blue-500 rounded-full">
                Current Plan
              </span>
            </div>

            <!-- Recommended Badge -->
            <div v-else-if="plan.recommended" class="absolute transform -translate-x-1/2 -top-3 left-1/2">
              <span class="px-3 py-1 text-sm font-medium text-white bg-purple-500 rounded-full">
                Recommended
              </span>
            </div>

            <!-- Upgrade/Downgrade Badge -->
            <div v-else-if="getPlanComparison(plan) === 'upgrade'" class="absolute transform -translate-x-1/2 -top-3 left-1/2">
              <span class="px-3 py-1 text-sm font-medium text-white bg-green-500 rounded-full">
                Upgrade
              </span>
            </div>
            <div v-else-if="getPlanComparison(plan) === 'downgrade'" class="absolute transform -translate-x-1/2 -top-3 left-1/2">
              <span class="px-3 py-1 text-sm font-medium text-white bg-orange-500 rounded-full">
                Downgrade
              </span>
            </div>

            <div class="text-center">
              <h4 class="text-xl font-semibold text-gray-900 dark:text-white">
                {{ plan.name }}
              </h4>
              <div class="mt-4">
                <span class="text-4xl font-bold text-gray-900 dark:text-white">
                  ${{ plan.price }}
                </span>
                <span class="text-base font-medium text-gray-500 dark:text-gray-400">
                  /{{ plan.interval }}
                </span>
              </div>
            </div>

            <!-- Features -->
            <ul class="mt-6 space-y-3">
              <li v-for="feature in plan.features" :key="feature" class="flex items-start">
                <svg class="flex-shrink-0 w-5 h-5 text-purple-500" fill="currentColor" viewBox="0 0 20 20">
                  <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd"></path>
                </svg>
                <span class="ml-3 text-sm text-gray-600 dark:text-gray-300">{{ feature }}</span>
              </li>
            </ul>

            <!-- Plan Details -->
            <div class="mt-6 space-y-2">
              <div class="flex justify-between text-sm">
                <span class="text-gray-500 dark:text-gray-400">Max Patients:</span>
                <span class="font-medium text-gray-900 dark:text-white">
                  {{ plan.maxPatients === -1 ? 'Unlimited' : plan.maxPatients }}
                </span>
              </div>
              <div class="flex justify-between text-sm">
                <span class="text-gray-500 dark:text-gray-400">AI Analyses:</span>
                <span class="font-medium text-gray-900 dark:text-white">
                  {{ plan.aiAnalysisLimit === -1 ? 'Unlimited' : plan.aiAnalysisLimit }}
                </span>
              </div>
            </div>

            <!-- Action Button -->
            <div class="mt-6">
              <button
                v-if="plan.id === subscriptionStore.currentPlan?.id"
                disabled
                class="w-full px-4 py-2 text-sm font-medium text-gray-500 bg-gray-100 border border-gray-300 rounded-md cursor-not-allowed dark:bg-gray-700 dark:text-gray-400 dark:border-gray-600"
              >
                Current Plan
              </button>
              <button
                v-else
                @click="selectPlan(plan)"
                :disabled="loading"
                class="w-full px-4 py-2 text-sm font-medium text-white transition-colors border border-transparent rounded-md shadow-sm focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-purple-500"
                :class="[
                  getPlanComparison(plan) === 'upgrade'
                    ? 'bg-green-600 hover:bg-green-700'
                    : getPlanComparison(plan) === 'downgrade'
                    ? 'bg-orange-600 hover:bg-orange-700'
                    : 'bg-purple-600 hover:bg-purple-700'
                ]"
              >
                <span v-if="loading && selectedPlan?.id === plan.id" class="flex items-center justify-center">
                  <svg class="w-4 h-4 mr-2 animate-spin" fill="none" viewBox="0 0 24 24">
                    <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
                    <path class="opacity-75" fill="currentColor" d="m4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                  </svg>
                  Processing...
                </span>
                <span v-else>
                  {{ getPlanComparison(plan) === 'upgrade' ? 'Upgrade' : getPlanComparison(plan) === 'downgrade' ? 'Downgrade' : 'Switch' }} to {{ plan.name }}
                </span>
              </button>
            </div>
          </div>
        </div>
      </div>

      <!-- Change Plan Confirmation -->
      <div class="pt-4 border-t border-gray-200 dark:border-gray-700">
        <div class="p-4 bg-yellow-50 border border-yellow-200 rounded-lg dark:bg-yellow-900/20 dark:border-yellow-800">
          <div class="flex">
            <svg class="flex-shrink-0 w-5 h-5 text-yellow-400" viewBox="0 0 20 20" fill="currentColor">
              <path fill-rule="evenodd" d="M8.257 3.099c.765-1.36 2.722-1.36 3.486 0l5.58 9.92c.75 1.334-.213 2.98-1.742 2.98H4.42c-1.53 0-2.493-1.646-1.743-2.98l5.58-9.92zM11 13a1 1 0 11-2 0 1 1 0 012 0zm-1-8a1 1 0 00-1 1v3a1 1 0 002 0V6a1 1 0 00-1-1z" clip-rule="evenodd" />
            </svg>
            <div class="ml-3">
              <h3 class="text-sm font-medium text-yellow-800 dark:text-yellow-200">
                Plan Change Information
              </h3>
              <div class="mt-2 text-sm text-yellow-700 dark:text-yellow-300">
                <ul class="pl-5 list-disc space-y-1">
                  <li>Plan changes take effect immediately</li>
                  <li>You will be prorated for the time remaining in your current billing cycle</li>
                  <li>Your next billing date will remain the same</li>
                  <li>Downgrades may affect access to certain features</li>
                </ul>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>

  <!-- Payment Modal -->
  <PaymentModal
    v-if="showPaymentModal"
    :isOpen="showPaymentModal"
    mode="payment"
    :amount="selectedPlan?.price || 0"
    :description="`Change plan to ${selectedPlan?.name}`"
    @close="cancelPlanChange"
    @success="handlePaymentSuccess"
    @error="handlePaymentError"
  />

  <!-- Success Modal -->
  <div v-if="showSuccessModal" class="fixed inset-0 z-50 flex items-center justify-center bg-gray-600 bg-opacity-50">
    <div class="relative max-w-md mx-auto bg-white border rounded-lg shadow-lg dark:bg-gray-800">
      <div class="p-6">
        <div class="flex items-center justify-center w-12 h-12 mx-auto bg-green-100 rounded-full dark:bg-green-900">
          <svg class="w-6 h-6 text-green-600 dark:text-green-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 13l4 4L19 7"></path>
          </svg>
        </div>
        <h3 class="mt-4 text-lg font-medium text-center text-gray-900 dark:text-white">Plan Changed Successfully!</h3>
        <p class="mt-2 text-sm text-center text-gray-500 dark:text-gray-400">
          Your subscription has been updated to {{ selectedPlan?.name }}. The changes are now active.
        </p>
        <div class="mt-6">
          <button
            @click="closeSuccessModal"
            class="inline-flex justify-center w-full px-4 py-2 text-base font-medium text-white bg-green-600 border border-transparent rounded-md shadow-sm hover:bg-green-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-green-500"
          >
            Continue
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useSubscriptionStore, type SubscriptionPlan } from '../../stores/subscription'
import PaymentModal from './PaymentModal.vue'

interface Props {
  isOpen: boolean
}

defineProps<Props>()

const emit = defineEmits<{
  close: []
  planChanged: []
}>()

const subscriptionStore = useSubscriptionStore()
const loading = ref(false)
const showPaymentModal = ref(false)
const showSuccessModal = ref(false)
const selectedPlan = ref<SubscriptionPlan | null>(null)

const availablePlansFiltered = computed(() => {
  // Show all plans for comparison
  return subscriptionStore.availablePlans
})

const getPlanComparison = (plan: SubscriptionPlan) => {
  const currentPlan = subscriptionStore.currentPlan
  if (!currentPlan || plan.id === currentPlan.id) return 'same'
  
  if (plan.price > currentPlan.price || plan.priority > currentPlan.priority) {
    return 'upgrade'
  } else if (plan.price < currentPlan.price || plan.priority < currentPlan.priority) {
    return 'downgrade'
  }
  return 'same'
}

const formatDate = (dateString: string | undefined) => {
  if (!dateString) return 'N/A'
  return new Date(dateString).toLocaleDateString()
}

const selectPlan = (plan: SubscriptionPlan) => {
  selectedPlan.value = plan
  showPaymentModal.value = true
}

const cancelPlanChange = () => {
  showPaymentModal.value = false
  selectedPlan.value = null
  loading.value = false
}

const handlePaymentSuccess = async (paymentMethodId: string) => {
  if (!selectedPlan.value) return
  
  loading.value = true
  try {
    await subscriptionStore.changePlan(selectedPlan.value.id, paymentMethodId)
    showPaymentModal.value = false
    showSuccessModal.value = true
  } catch (error) {
    console.error('Failed to change plan:', error)
    // Error handling is done in the store
    loading.value = false
  }
}

const handlePaymentError = (message: string) => {
  console.error('Payment error:', message)
  loading.value = false
  // Error is already handled in PaymentModal
}

const closeSuccessModal = () => {
  showSuccessModal.value = false
  selectedPlan.value = null
  loading.value = false
  emit('planChanged')
  emit('close')
}

onMounted(async () => {
  // EMERGENCY: Temporarily disabled to prevent infinite loop
  console.log('ChangePlanModal: onMounted disabled for debugging infinite loop')
  
  // TODO: Re-enable once root cause is identified
  /*
  try {
    console.log('ChangePlanModal: Checking if plans need to be loaded...')
    // Load available plans if not already loaded and not currently loading
    if (subscriptionStore.availablePlans.length === 0 && !subscriptionStore.isLoading) {
      console.log('ChangePlanModal: Loading available plans...')
      await subscriptionStore.loadAvailablePlans()
      console.log('ChangePlanModal: Plans loaded successfully')
    } else {
      console.log('ChangePlanModal: Plans already available or loading in progress, skipping load')
    }
  } catch (error) {
    console.error('ChangePlanModal: Error loading plans:', error)
  }
  */
})
</script>