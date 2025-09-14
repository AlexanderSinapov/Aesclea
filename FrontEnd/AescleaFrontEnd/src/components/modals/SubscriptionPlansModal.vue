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
        <div class="grid grid-cols-1 gap-6 md:grid-cols-3">
          <div
            v-for="plan in subscriptionStore.availablePlans"
            :key="plan.id"
            class="relative p-6 transition-shadow border rounded-lg shadow-sm hover:shadow-md"
            :class="[
              plan.recommended 
                ? 'border-purple-500 ring-2 ring-purple-500 ring-opacity-50' 
                : 'border-gray-200 dark:border-gray-700'
            ]"
          >
            <div v-if="plan.recommended" class="absolute transform -translate-x-1/2 -top-3 left-1/2">
              <span class="px-3 py-1 text-sm font-medium text-white bg-purple-500 rounded-full">
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
                class="w-full px-4 py-3 text-sm font-medium transition-colors rounded-md"
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
        <p class="text-sm text-center text-gray-500 dark:text-gray-400">
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
    <div v-if="showSuccessModal" class="fixed inset-0 z-50 w-full h-full overflow-y-auto bg-gray-600 bg-opacity-50">
      <div class="relative w-11/12 max-w-md p-5 mx-auto transform -translate-y-1/2 bg-white border rounded-md shadow-lg top-1/2 dark:bg-gray-800">
        <div class="text-center">
          <div class="flex items-center justify-center w-12 h-12 mx-auto bg-green-100 rounded-full dark:bg-green-900">
            <svg class="w-6 h-6 text-green-600 dark:text-green-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
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
              class="inline-flex justify-center w-full px-4 py-2 text-base font-medium text-white bg-purple-600 border border-transparent rounded-md shadow-sm hover:bg-purple-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-purple-500"
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
