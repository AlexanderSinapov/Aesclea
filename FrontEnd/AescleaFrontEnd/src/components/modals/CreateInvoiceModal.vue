<template>
  <div v-if="isOpen" class="fixed inset-0 bg-gray-600 bg-opacity-50 overflow-y-auto h-full w-full z-50">
    <div class="relative top-20 mx-auto p-5 border w-11/12 max-w-3xl shadow-lg rounded-md bg-white dark:bg-gray-800">
      <!-- Modal Header -->
      <div class="flex items-center justify-between pb-4 border-b border-gray-200 dark:border-gray-700">
        <h3 class="text-xl font-semibold text-gray-900 dark:text-white">
          Create New Invoice
        </h3>
        <button @click="$emit('close')" class="text-gray-400 hover:text-gray-600 dark:hover:text-gray-300">
          <span class="sr-only">Close</span>
          ✕
        </button>
      </div>

      <!-- Invoice Form -->
      <form @submit.prevent="handleSave" class="py-6">
        <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
          <!-- Patient Information -->
          <div>
            <label for="patientName" class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
              Patient Name *
            </label>
            <input
              id="patientName"
              v-model="invoiceForm.patientName"
              type="text"
              required
              class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md shadow-sm placeholder-gray-400 focus:outline-none focus:ring-purple-500 focus:border-purple-500 dark:bg-gray-700 dark:text-white"
              placeholder="Enter patient name"
            />
          </div>

          <!-- Due Date -->
          <div>
            <label for="dueDate" class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
              Due Date *
            </label>
            <input
              id="dueDate"
              v-model="invoiceForm.dueDate"
              type="date"
              required
              class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md shadow-sm focus:outline-none focus:ring-purple-500 focus:border-purple-500 dark:bg-gray-700 dark:text-white"
            />
          </div>
        </div>

        <!-- Services Section -->
        <div class="mt-6">
          <div class="flex items-center justify-between mb-4">
            <h4 class="text-lg font-medium text-gray-900 dark:text-white">Services</h4>
            <button
              type="button"
              @click="addService"
              class="inline-flex items-center px-3 py-2 border border-transparent text-sm font-medium rounded-md text-purple-600 bg-purple-100 hover:bg-purple-200 dark:bg-purple-900 dark:text-purple-300 dark:hover:bg-purple-800"
            >
              <svg class="w-4 h-4 mr-1" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 6v6m0 0v6m0-6h6m-6 0H6" />
              </svg>
              Add Service
            </button>
          </div>

          <div class="space-y-4">
            <div 
              v-for="(service, index) in invoiceForm.services" 
              :key="index"
              class="grid grid-cols-1 md:grid-cols-5 gap-4 p-4 border border-gray-200 dark:border-gray-600 rounded-lg"
            >
              <div>
                <label class="block text-xs font-medium text-gray-700 dark:text-gray-300 mb-1">
                  Service Name *
                </label>
                <input
                  v-model="service.name"
                  type="text"
                  required
                  placeholder="Service name"
                  class="w-full px-2 py-1 text-sm border border-gray-300 dark:border-gray-600 rounded-md focus:outline-none focus:ring-purple-500 focus:border-purple-500 dark:bg-gray-700 dark:text-white"
                />
              </div>
              
              <div>
                <label class="block text-xs font-medium text-gray-700 dark:text-gray-300 mb-1">
                  Department
                </label>
                <select
                  v-model="service.department"
                  class="w-full px-2 py-1 text-sm border border-gray-300 dark:border-gray-600 rounded-md focus:outline-none focus:ring-purple-500 focus:border-purple-500 dark:bg-gray-700 dark:text-white"
                >
                  <option value="">Select Department</option>
                  <option value="General Medicine">General Medicine</option>
                  <option value="Cardiology">Cardiology</option>
                  <option value="Neurology">Neurology</option>
                  <option value="Radiology">Radiology</option>
                  <option value="Laboratory">Laboratory</option>
                  <option value="Emergency">Emergency</option>
                </select>
              </div>
              
              <div>
                <label class="block text-xs font-medium text-gray-700 dark:text-gray-300 mb-1">
                  Quantity *
                </label>
                <input
                  v-model.number="service.quantity"
                  type="number"
                  min="1"
                  required
                  class="w-full px-2 py-1 text-sm border border-gray-300 dark:border-gray-600 rounded-md focus:outline-none focus:ring-purple-500 focus:border-purple-500 dark:bg-gray-700 dark:text-white"
                />
              </div>
              
              <div>
                <label class="block text-xs font-medium text-gray-700 dark:text-gray-300 mb-1">
                  Unit Price ($) *
                </label>
                <input
                  v-model.number="service.amount"
                  type="number"
                  step="0.01"
                  min="0"
                  required
                  class="w-full px-2 py-1 text-sm border border-gray-300 dark:border-gray-600 rounded-md focus:outline-none focus:ring-purple-500 focus:border-purple-500 dark:bg-gray-700 dark:text-white"
                />
              </div>
              
              <div class="flex items-end">
                <button
                  type="button"
                  @click="removeService(index)"
                  :disabled="invoiceForm.services.length === 1"
                  class="w-full px-2 py-1 text-sm text-red-600 border border-red-300 rounded-md hover:bg-red-50 disabled:opacity-50 disabled:cursor-not-allowed dark:text-red-400 dark:border-red-600 dark:hover:bg-red-900"
                >
                  Remove
                </button>
              </div>
            </div>
          </div>
        </div>

        <!-- Total Section -->
        <div class="mt-6 p-4 bg-gray-50 dark:bg-gray-700 rounded-lg">
          <div class="flex justify-between items-center">
            <span class="text-lg font-medium text-gray-900 dark:text-white">Total Amount:</span>
            <span class="text-2xl font-bold text-purple-600 dark:text-purple-400">
              ${{ totalAmount.toFixed(2) }}
            </span>
          </div>
        </div>

        <!-- Action Buttons -->
        <div class="flex justify-end space-x-3 mt-6 pt-4 border-t border-gray-200 dark:border-gray-700">
          <button
            type="button"
            @click="$emit('close')"
            class="px-4 py-2 border border-gray-300 dark:border-gray-600 rounded-md shadow-sm text-sm font-medium text-gray-700 dark:text-gray-300 bg-white dark:bg-gray-700 hover:bg-gray-50 dark:hover:bg-gray-600"
          >
            Cancel
          </button>
          <button
            type="submit"
            :disabled="!isFormValid || isLoading"
            class="px-4 py-2 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-purple-600 hover:bg-purple-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-purple-500 disabled:opacity-50 disabled:cursor-not-allowed"
          >
            <span v-if="isLoading">Creating...</span>
            <span v-else>Create Invoice</span>
          </button>
        </div>
      </form>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, computed } from 'vue'

interface Service {
  id: string
  name: string
  department: string
  amount: number
  quantity: number
  description?: string
}

interface Props {
  isOpen: boolean
}

defineProps<Props>()

const emit = defineEmits<{
  close: []
  save: [invoiceData: any]
}>()

const isLoading = ref(false)

const invoiceForm = reactive({
  patientName: '',
  dueDate: '',
  services: [
    {
      id: '',
      name: '',
      department: '',
      amount: 0,
      quantity: 1,
      description: ''
    }
  ] as Service[]
})

const totalAmount = computed(() => {
  return invoiceForm.services.reduce((total, service) => {
    return total + (service.amount * service.quantity)
  }, 0)
})

const isFormValid = computed(() => {
  return (
    invoiceForm.patientName.trim() &&
    invoiceForm.dueDate &&
    invoiceForm.services.length > 0 &&
    invoiceForm.services.every(service => 
      service.name.trim() && 
      service.amount > 0 && 
      service.quantity > 0
    )
  )
})

const addService = () => {
  invoiceForm.services.push({
    id: '',
    name: '',
    department: '',
    amount: 0,
    quantity: 1,
    description: ''
  })
}

const removeService = (index: number) => {
  if (invoiceForm.services.length > 1) {
    invoiceForm.services.splice(index, 1)
  }
}

const handleSave = async () => {
  if (!isFormValid.value) return

  isLoading.value = true
  
  try {
    const invoiceData = {
      patientId: `patient_${Date.now()}`,
      patientName: invoiceForm.patientName,
      amount: totalAmount.value,
      totalAmount: totalAmount.value,
      currency: 'USD',
      status: 'open' as const,
      invoiceNumber: `INV-${new Date().getFullYear()}-${String(Date.now()).slice(-6)}`,
      dueDate: invoiceForm.dueDate,
      createdAt: new Date().toISOString(),
      services: invoiceForm.services.map((service, index) => ({
        ...service,
        id: `svc_${Date.now()}_${index}`
      }))
    }

    emit('save', invoiceData)
    
    // Reset form
    invoiceForm.patientName = ''
    invoiceForm.dueDate = ''
    invoiceForm.services = [{
      id: '',
      name: '',
      department: '',
      amount: 0,
      quantity: 1,
      description: ''
    }]
  } catch (error) {
    console.error('Error creating invoice:', error)
  } finally {
    isLoading.value = false
  }
}
</script>
