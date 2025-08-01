<template>
  <div v-if="isOpen" class="fixed inset-0 bg-gray-600 bg-opacity-50 overflow-y-auto h-full w-full z-50">
    <div class="relative top-20 mx-auto p-5 border w-11/12 max-w-4xl shadow-lg rounded-md bg-white dark:bg-gray-800">
      <!-- Modal Header -->
      <div class="flex items-center justify-between pb-4 border-b border-gray-200 dark:border-gray-700">
        <h3 class="text-xl font-semibold text-gray-900 dark:text-white">
          Choose Your Plan
        </h3>
        <button
          @click="$emit('close')"
          class="text-gray-400 hover:text-gray-600 dark:hover:text-gray-300"
        >
          <span class="sr-only">Close</span>
          ✕
        </button>
      </div>

      <!-- Plans Grid -->
      <div class="py-6">
        <div class="grid grid-cols-1 md:grid-cols-3 gap-6">
          <div
            v-for="plan in subscriptionStore.availablePlans"
            :key="plan.id"
            class="relative border rounded-lg p-6 shadow-sm hover:shadow-md transition-shadow"
            :class="[
              plan.recommended 
                ? 'border-purple-500 ring-2 ring-purple-500 ring-opacity-50' 
                : 'border-gray-200 dark:border-gray-700'
            ]"
          >
            <div v-if="plan.recommended" class="absolute -top-3 left-1/2 transform -translate-x-1/2">
              <span class="bg-purple-500 text-white px-3 py-1 text-sm font-medium rounded-full">
                Recommended
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
                <span class="text-gray-500 dark:text-gray-400">
                  /{{ plan.interval }}
                </span>
              </div>
              <p class="mt-2 text-sm text-gray-500 dark:text-gray-400">
                Up to {{ plan.maxPatients === -1 ? 'unlimited' : plan.maxPatients }} patients • 
                {{ plan.aiAnalysisLimit === -1 ? 'unlimited' : plan.aiAnalysisLimit }} AI analyses/month
              </p>
            </div>

            <!-- Features List -->
            <ul class="mt-6 space-y-3">
              <li
                v-for="feature in plan.features"
                :key="feature"
                class="flex items-start"
              >
                <span class="w-5 h-5 text-green-500 mr-3 mt-0.5 flex-shrink-0">✓</span>
                <span class="text-sm text-gray-700 dark:text-gray-300">
                  {{ feature }}
                </span>
              </li>
            </ul>

            <!-- Subscribe Button -->
            <div class="mt-8">
              <button
                @click="selectPlan(plan)"
                :disabled="loading"
                class="w-full py-3 px-4 rounded-md text-sm font-medium transition-colors"
                :class="[
                  plan.recommended
                    ? 'bg-purple-600 hover:bg-purple-700 text-white'
                    : 'bg-gray-100 hover:bg-gray-200 text-gray-900 dark:bg-gray-700 dark:hover:bg-gray-600 dark:text-white',
                  loading ? 'opacity-50 cursor-not-allowed' : ''
                ]"
              >
                <span v-if="loading">Processing...</span>
                <span v-else>Select {{ plan.name }}</span>
              </button>
            </div>
          </div>
        </div>
      </div>

      <!-- Footer -->
      <div class="pt-4 border-t border-gray-200 dark:border-gray-700">
        <p class="text-sm text-gray-500 dark:text-gray-400 text-center">
          All plans include a 14-day free trial. Cancel anytime.
        </p>
      </div>
    </div>

    <!-- Payment Modal -->
    <PaymentModal
      :is-open="showPaymentModal"
      mode="subscription"
      :selected-plan="selectedPlan"
      @close="showPaymentModal = false"
      @success="handlePaymentSuccess"
      @error="handlePaymentError"
    />

    <!-- Success Modal -->
    <div v-if="showSuccessModal" class="fixed inset-0 bg-gray-600 bg-opacity-50 overflow-y-auto h-full w-full z-50">
      <div class="relative top-1/2 transform -translate-y-1/2 mx-auto p-5 border w-11/12 max-w-md shadow-lg rounded-md bg-white dark:bg-gray-800">
        <div class="text-center">
          <div class="mx-auto flex items-center justify-center h-12 w-12 rounded-full bg-green-100 dark:bg-green-900">
            <svg class="h-6 w-6 text-green-600 dark:text-green-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 13l4 4L19 7"></path>
            </svg>
          </div>
          <h3 class="mt-4 text-lg font-medium text-gray-900 dark:text-white">Subscription Activated!</h3>
          <p class="mt-2 text-sm text-gray-500 dark:text-gray-400">
            Welcome to {{ selectedPlan?.name }}! Your subscription is now active and you have full access to all features.
          </p>
          <div class="mt-6">
            <button
              @click="closeSuccessModal"
              class="w-full inline-flex justify-center rounded-md border border-transparent shadow-sm px-4 py-2 bg-purple-600 text-base font-medium text-white hover:bg-purple-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-purple-500"
            >
              Get Started
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useSubscriptionStore } from '../../stores/subscription'
import PaymentModal from './PaymentModal.vue'

interface Props {
  isOpen: boolean
}

defineProps<Props>()

const emit = defineEmits<{
  close: []
}>()

const subscriptionStore = useSubscriptionStore()
const loading = ref(false)
const showPaymentModal = ref(false)
const showSuccessModal = ref(false)
const selectedPlan = ref<any>(null)

const selectPlan = (plan: any) => {
  selectedPlan.value = plan
  showPaymentModal.value = true
}

const handlePaymentSuccess = (paymentMethodId: string) => {
  showPaymentModal.value = false
  showSuccessModal.value = true
  console.log('Payment successful with method:', paymentMethodId)
}

const handlePaymentError = (message: string) => {
  console.error('Payment error:', message)
  // Error is already handled in PaymentModal
}

const closeSuccessModal = () => {
  showSuccessModal.value = false
  selectedPlan.value = null
  emit('close')
}
</script>
