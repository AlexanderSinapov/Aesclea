<!-- Copyright (c) 2025 Alexander Sinapov | Simeon Petkov -->

<!-- All rights reserved. -->
<!-- This code is proprietary and confidential.   -->
<!-- Unauthorized copying, modification, distribution, or use is strictly prohibited. -->

<template>
  <div v-if="isOpen" class="fixed inset-0 z-50 w-full h-full overflow-y-auto bg-gray-600 bg-opacity-50">
    <div class="relative w-11/12 max-w-2xl p-5 mx-auto bg-white border rounded-md shadow-lg top-20 dark:bg-gray-800">
      <!-- Modal Header -->
      <div class="flex items-center justify-between pb-4 border-b border-gray-200 dark:border-gray-700">
        <h3 class="text-xl font-semibold text-gray-900 dark:text-white">
          {{ mode === 'subscription' ? 'Complete Subscription' : 'Payment Details' }}
        </h3>
        <button @click="$emit('close')" class="text-gray-400 hover:text-gray-600 dark:hover:text-gray-300">
          <span class="sr-only">Close</span>
          ✕
        </button>
      </div>

      <!-- Test Mode Warning -->
      <div class="py-4 border-b border-gray-200 dark:border-gray-700">
        <div class="flex items-center p-3 bg-green-50 border border-green-200 rounded-lg dark:bg-green-900/20 dark:border-green-800">
          <svg class="w-5 h-5 mr-2 text-green-600 dark:text-green-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z"></path>
          </svg>
          <div>
            <p class="text-sm font-medium text-green-800 dark:text-green-200">
              Safe Test Mode
            </p>
            <p class="text-sm text-green-700 dark:text-green-300">
              No real payments will be charged. This is a demo environment.
            </p>
          </div>
        </div>
      </div>

      <!-- Plan Summary (for subscription mode) -->
      <div v-if="mode === 'subscription' && selectedPlan" class="py-4 border-b border-gray-200 dark:border-gray-700">
        <div class="p-4 rounded-lg bg-gray-50 dark:bg-gray-700">
          <h4 class="mb-2 text-lg font-medium text-gray-900 dark:text-white">
            {{ selectedPlan.name }} Plan
          </h4>
          <div class="flex items-center justify-between">
            <div>
              <p class="text-2xl font-bold text-gray-900 dark:text-white">
                ${{ selectedPlan.price }}
                <span class="text-sm font-normal text-gray-500 dark:text-gray-400">
                  /{{ selectedPlan.interval }}
                </span>
              </p>
              <p class="mt-1 text-sm text-gray-600 dark:text-gray-400">
                14-day free trial included
              </p>
            </div>
            <div class="text-right">
              <p class="text-sm text-gray-600 dark:text-gray-400">Due today</p>
              <p class="text-lg font-semibold text-green-600">$0.00</p>
            </div>
          </div>
        </div>
      </div>

      <!-- Payment Form -->
      <form @submit.prevent="processPayment" class="py-6">
        <!-- Saved Payment Methods -->
        <div v-if="savedPaymentMethods.length > 0" class="mb-6">
          <h4 class="mb-3 text-lg font-medium text-gray-900 dark:text-white">
            Saved Payment Methods
          </h4>
          <div class="space-y-3">
            <label
              v-for="method in savedPaymentMethods"
              :key="method.id"
              class="flex items-center p-3 border border-gray-200 rounded-lg cursor-pointer dark:border-gray-600 hover:bg-gray-50 dark:hover:bg-gray-700"
            >
              <input
                type="radio"
                :value="method.id"
                v-model="selectedPaymentMethod"
                class="w-4 h-4 text-purple-600 border-gray-300 focus:ring-purple-500"
              />
              <div class="flex-1 ml-3">
                <div class="flex items-center justify-between">
                  <div class="flex items-center">
                    <span class="text-sm font-medium text-gray-900 dark:text-white">
                      •••• •••• •••• {{ method.last4 }}
                    </span>
                    <span class="ml-2 text-xs text-gray-500 uppercase dark:text-gray-400">
                      {{ method.brand }}
                    </span>
                  </div>
                  <span v-if="method.isDefault" class="px-2 py-1 text-xs text-green-800 bg-green-100 rounded">
                    Default
                  </span>
                </div>
                <p class="text-xs text-gray-500 dark:text-gray-400">
                  Expires {{ method.expiryMonth?.toString().padStart(2, '0') }}/{{ method.expiryYear }}
                </p>
              </div>
            </label>
          </div>
          
          <div class="mt-4">
            <button
              type="button"
              @click="showNewCardForm = true"
              class="text-sm text-purple-600 hover:text-purple-500"
            >
              + Add new payment method
            </button>
          </div>
        </div>

        <!-- New Card Form -->
        <div v-if="savedPaymentMethods.length === 0 || showNewCardForm" class="space-y-4">
          <h4 class="mb-3 text-lg font-medium text-gray-900 dark:text-white">
            {{ savedPaymentMethods.length > 0 ? 'Add New Payment Method' : 'Payment Information' }}
          </h4>
          
          <!-- Card Number -->
          <div>
            <label for="cardNumber" class="block mb-1 text-sm font-medium text-gray-700 dark:text-gray-300">
              Card Number
            </label>
            <input
              id="cardNumber"
              v-model="cardForm.number"
              type="text"
              placeholder="1234 5678 9012 3456"
              maxlength="19"
              @input="formatCardNumber"
              class="w-full px-3 py-2 placeholder-gray-400 border border-gray-300 rounded-md shadow-sm dark:border-gray-600 focus:outline-none focus:ring-purple-500 focus:border-purple-500 dark:bg-gray-700 dark:text-white"
              :class="{ 'border-red-300': errors.cardNumber }"
            />
            <p v-if="errors.cardNumber" class="mt-1 text-sm text-red-600">{{ errors.cardNumber }}</p>
          </div>

          <!-- Expiry and CVC -->
          <div class="grid grid-cols-2 gap-4">
            <div>
              <label for="expiry" class="block mb-1 text-sm font-medium text-gray-700 dark:text-gray-300">
                Expiry Date
              </label>
              <input
                id="expiry"
                v-model="cardForm.expiry"
                type="text"
                placeholder="MM/YY"
                maxlength="5"
                @input="formatExpiry"
                class="w-full px-3 py-2 placeholder-gray-400 border border-gray-300 rounded-md shadow-sm dark:border-gray-600 focus:outline-none focus:ring-purple-500 focus:border-purple-500 dark:bg-gray-700 dark:text-white"
                :class="{ 'border-red-300': errors.expiry }"
              />
              <p v-if="errors.expiry" class="mt-1 text-sm text-red-600">{{ errors.expiry }}</p>
            </div>
            <div>
              <label for="cvc" class="block mb-1 text-sm font-medium text-gray-700 dark:text-gray-300">
                CVC
              </label>
              <input
                id="cvc"
                v-model="cardForm.cvc"
                type="text"
                placeholder="123"
                maxlength="4"
                @input="formatCVC"
                class="w-full px-3 py-2 placeholder-gray-400 border border-gray-300 rounded-md shadow-sm dark:border-gray-600 focus:outline-none focus:ring-purple-500 focus:border-purple-500 dark:bg-gray-700 dark:text-white"
                :class="{ 'border-red-300': errors.cvc }"
              />
              <p v-if="errors.cvc" class="mt-1 text-sm text-red-600">{{ errors.cvc }}</p>
            </div>
          </div>

          <!-- Cardholder Name -->
          <div>
            <label for="cardholderName" class="block mb-1 text-sm font-medium text-gray-700 dark:text-gray-300">
              Cardholder Name
            </label>
            <input
              id="cardholderName"
              v-model="cardForm.name"
              type="text"
              placeholder="John Doe"
              class="w-full px-3 py-2 placeholder-gray-400 border border-gray-300 rounded-md shadow-sm dark:border-gray-600 focus:outline-none focus:ring-purple-500 focus:border-purple-500 dark:bg-gray-700 dark:text-white"
              :class="{ 'border-red-300': errors.cardholderName }"
            />
            <p v-if="errors.cardholderName" class="mt-1 text-sm text-red-600">{{ errors.cardholderName }}</p>
          </div>

          <!-- Billing Address -->
          <div class="pt-4 border-t border-gray-200 dark:border-gray-600">
            <h5 class="mb-3 font-medium text-gray-900 text-md dark:text-white">Billing Address</h5>
            
            <div class="space-y-4">
              <div>
                <label for="address" class="block mb-1 text-sm font-medium text-gray-700 dark:text-gray-300">
                  Address
                </label>
                <input
                  id="address"
                  v-model="billingAddress.line1"
                  type="text"
                  placeholder="123 Main Street"
                  class="w-full px-3 py-2 placeholder-gray-400 border border-gray-300 rounded-md shadow-sm dark:border-gray-600 focus:outline-none focus:ring-purple-500 focus:border-purple-500 dark:bg-gray-700 dark:text-white"
                />
              </div>
              
              <div class="grid grid-cols-2 gap-4">
                <div>
                  <label for="city" class="block mb-1 text-sm font-medium text-gray-700 dark:text-gray-300">
                    City
                  </label>
                  <input
                    id="city"
                    v-model="billingAddress.city"
                    type="text"
                    placeholder="New York"
                    class="w-full px-3 py-2 placeholder-gray-400 border border-gray-300 rounded-md shadow-sm dark:border-gray-600 focus:outline-none focus:ring-purple-500 focus:border-purple-500 dark:bg-gray-700 dark:text-white"
                  />
                </div>
                <div>
                  <label for="state" class="block mb-1 text-sm font-medium text-gray-700 dark:text-gray-300">
                    State
                  </label>
                  <input
                    id="state"
                    v-model="billingAddress.state"
                    type="text"
                    placeholder="NY"
                    class="w-full px-3 py-2 placeholder-gray-400 border border-gray-300 rounded-md shadow-sm dark:border-gray-600 focus:outline-none focus:ring-purple-500 focus:border-purple-500 dark:bg-gray-700 dark:text-white"
                  />
                </div>
              </div>
              
              <div class="grid grid-cols-2 gap-4">
                <div>
                  <label for="postalCode" class="block mb-1 text-sm font-medium text-gray-700 dark:text-gray-300">
                    Postal Code
                  </label>
                  <input
                    id="postalCode"
                    v-model="billingAddress.postalCode"
                    type="text"
                    placeholder="10001"
                    class="w-full px-3 py-2 placeholder-gray-400 border border-gray-300 rounded-md shadow-sm dark:border-gray-600 focus:outline-none focus:ring-purple-500 focus:border-purple-500 dark:bg-gray-700 dark:text-white"
                  />
                </div>
                <div>
                  <label for="country" class="block mb-1 text-sm font-medium text-gray-700 dark:text-gray-300">
                    Country
                  </label>
                  <select
                    id="country"
                    v-model="billingAddress.country"
                    class="w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm dark:border-gray-600 focus:outline-none focus:ring-purple-500 focus:border-purple-500 dark:bg-gray-700 dark:text-white"
                  >
                    <option value="US">United States</option>
                    <option value="CA">Canada</option>
                    <option value="GB">United Kingdom</option>
                    <option value="AU">Australia</option>
                    <option value="DE">Germany</option>
                    <option value="FR">France</option>
                    <option value="IT">Italy</option>
                    <option value="ES">Spain</option>
                    <option value="NL">Netherlands</option>
                    <option value="SE">Sweden</option>
                    <option value="NO">Norway</option>
                    <option value="DK">Denmark</option>
                  </select>
                </div>
              </div>
            </div>
          </div>

          <!-- Save Payment Method -->
          <div class="flex items-center">
            <input
              id="savePaymentMethod"
              v-model="savePaymentMethod"
              type="checkbox"
              class="w-4 h-4 text-purple-600 border-gray-300 rounded focus:ring-purple-500"
            />
            <label for="savePaymentMethod" class="block ml-2 text-sm text-gray-900 dark:text-gray-300">
              Save this payment method for future use
            </label>
          </div>
        </div>

        <!-- Security Notice -->
        <div class="p-4 mt-6 rounded-lg bg-gray-50 dark:bg-gray-700">
          <div class="flex items-start">
            <svg class="w-5 h-5 text-green-500 mt-0.5 mr-3 flex-shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m5.618-4.016A11.955 11.955 0 0112 2.944a11.955 11.955 0 01-8.618 3.04A12.02 12.02 0 003 9c0 5.591 3.824 10.29 9 11.622 5.176-1.332 9-6.031 9-11.622 0-1.042-.133-2.052-.382-3.016z" />
            </svg>
            <div>
              <h4 class="text-sm font-medium text-gray-900 dark:text-white">Secure Payment</h4>
              <p class="mt-1 text-sm text-gray-600 dark:text-gray-400">
                Your payment information is encrypted and processed securely. We never store your full card details.
              </p>
            </div>
          </div>
        </div>

        <!-- Error Display -->
        <div v-if="error" class="p-3 mt-4 border border-red-200 rounded-md bg-red-50 dark:bg-red-900/20 dark:border-red-800">
          <div class="flex">
            <svg class="w-5 h-5 mr-2 text-red-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
            </svg>
            <p class="text-sm text-red-800 dark:text-red-200">{{ error }}</p>
          </div>
        </div>

        <!-- Action Buttons -->
        <div class="flex justify-end pt-4 mt-6 space-x-3 border-t border-gray-200 dark:border-gray-700">
          <button
            type="button"
            @click="$emit('close')"
            class="px-4 py-2 text-sm font-medium text-gray-700 bg-white border border-gray-300 rounded-md shadow-sm dark:border-gray-600 dark:text-gray-300 dark:bg-gray-700 hover:bg-gray-50 dark:hover:bg-gray-600 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-purple-500"
          >
            Cancel
          </button>
          <button
            type="submit"
            :disabled="isProcessing || !isFormValid"
            class="px-4 py-2 text-sm font-medium text-white bg-purple-600 border border-transparent rounded-md shadow-sm hover:bg-purple-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-purple-500 disabled:opacity-50 disabled:cursor-not-allowed"
          >
            <span v-if="isProcessing" class="flex items-center">
              <svg class="w-4 h-4 mr-2 -ml-1 text-white animate-spin" fill="none" viewBox="0 0 24 24">
                <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
                <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
              </svg>
              Processing...
            </span>
            <span v-else>
              {{ mode === 'subscription' ? 'Start Subscription' : 'Complete Payment' }}
            </span>
          </button>
        </div>
      </form>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, computed, onMounted } from 'vue'
import { usePaymentStore } from '../../stores/payment'
import { useSubscriptionStore } from '../../stores/subscription'

interface Props {
  isOpen: boolean
  mode: 'subscription' | 'payment'
  selectedPlan?: any
  amount?: number
  description?: string
}

const props = defineProps<Props>()

const emit = defineEmits<{
  close: []
  success: [paymentMethodId: string]
  error: [message: string]
}>()

const paymentStore = usePaymentStore()
const subscriptionStore = useSubscriptionStore()

const isProcessing = ref(false)
const error = ref<string | null>(null)
const showNewCardForm = ref(false)
const selectedPaymentMethod = ref<string>('')
const savePaymentMethod = ref(true)

const cardForm = reactive({
  number: '',
  expiry: '',
  cvc: '',
  name: ''
})

const billingAddress = reactive({
  line1: '',
  city: '',
  state: '',
  postalCode: '',
  country: 'US'
})

const errors = reactive({
  cardNumber: '',
  expiry: '',
  cvc: '',
  cardholderName: ''
})

const savedPaymentMethods = computed(() => paymentStore.paymentMethods || [])

const isFormValid = computed(() => {
  if (selectedPaymentMethod.value && !showNewCardForm.value) {
    return true
  }
  
  return (
    cardForm.number.replace(/\s/g, '').length >= 13 &&
    cardForm.expiry.length === 5 &&
    cardForm.cvc.length >= 3 &&
    cardForm.name.trim().length > 0 &&
    !Object.values(errors).some(error => error)
  )
})

const formatCardNumber = (event: Event) => {
  const input = event.target as HTMLInputElement
  let value = input.value.replace(/\s/g, '').replace(/[^0-9]/gi, '')
  
  // Add spaces every 4 digits
  const matches = value.match(/\d{4,16}/g)
  const match = matches && matches[0] || ''
  const parts = []
  
  for (let i = 0, len = match.length; i < len; i += 4) {
    parts.push(match.substring(i, i + 4))
  }
  
  if (parts.length) {
    cardForm.number = parts.join(' ')
  } else {
    cardForm.number = value
  }
  
  // Validate card number
  const cleanNumber = cardForm.number.replace(/\s/g, '')
  if (cleanNumber.length > 0 && cleanNumber.length < 13) {
    errors.cardNumber = 'Card number must be at least 13 digits'
  } else if (cleanNumber.length > 19) {
    errors.cardNumber = 'Card number is too long'
  } else {
    errors.cardNumber = ''
  }
}

const formatExpiry = (event: Event) => {
  const input = event.target as HTMLInputElement
  let value = input.value.replace(/\D/g, '')
  
  if (value.length >= 2) {
    value = value.substring(0, 2) + '/' + value.substring(2, 4)
  }
  
  cardForm.expiry = value
  
  // Validate expiry
  if (value.length === 5) {
    const [month, year] = value.split('/')
    const currentYear = new Date().getFullYear() % 100
    const currentMonth = new Date().getMonth() + 1
    
    const monthNum = parseInt(month)
    const yearNum = parseInt(year)
    
    if (monthNum < 1 || monthNum > 12) {
      errors.expiry = 'Invalid month'
    } else if (yearNum < currentYear || (yearNum === currentYear && monthNum < currentMonth)) {
      errors.expiry = 'Card has expired'
    } else {
      errors.expiry = ''
    }
  } else if (value.length > 0) {
    errors.expiry = 'Enter MM/YY format'
  } else {
    errors.expiry = ''
  }
}

const formatCVC = (event: Event) => {
  const input = event.target as HTMLInputElement
  cardForm.cvc = input.value.replace(/\D/g, '').substring(0, 4)
  
  // Validate CVC
  if (cardForm.cvc.length > 0 && cardForm.cvc.length < 3) {
    errors.cvc = 'CVC must be at least 3 digits'
  } else {
    errors.cvc = ''
  }
}

const createPaymentMethod = async () => {
  // Use the payment store to create the payment method
  const paymentMethodId = await paymentStore.addPaymentMethod({
    number: cardForm.number.replace(/\s/g, ''),
    expiryMonth: parseInt(cardForm.expiry.split('/')[0]),
    expiryYear: 2000 + parseInt(cardForm.expiry.split('/')[1]),
    cvc: cardForm.cvc,
    name: cardForm.name,
    address: billingAddress
  })
  
  return paymentMethodId
}

const processPayment = async () => {
  if (!isFormValid.value) return
  
  isProcessing.value = true
  error.value = null
  
  try {
    let paymentMethodId: string
    
    if (selectedPaymentMethod.value && !showNewCardForm.value) {
      // Use saved payment method
      paymentMethodId = selectedPaymentMethod.value
    } else {
      // Create new payment method
      paymentMethodId = await createPaymentMethod()
    }
    
    if (props.mode === 'subscription' && props.selectedPlan) {
      // Process subscription
      await subscriptionStore.subscribeToPlan(props.selectedPlan.id, paymentMethodId)
    }
    
    emit('success', paymentMethodId)
  } catch (err: any) {
    error.value = err.message || 'Payment processing failed. Please try again.'
    emit('error', error.value || 'Payment processing failed. Please try again.')
  } finally {
    isProcessing.value = false
  }
}

onMounted(async () => {
  // Load saved payment methods
  try {
    await paymentStore.fetchPaymentMethods()
    if (savedPaymentMethods.value.length === 0) {
      showNewCardForm.value = true
    }
  } catch (err) {
    // Show new card form if can't load saved methods
    showNewCardForm.value = true
  }
})
</script>
