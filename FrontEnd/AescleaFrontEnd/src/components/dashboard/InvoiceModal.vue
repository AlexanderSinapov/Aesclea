<!-- Copyright (c) 2025 Alexander Sinapov | Simeon Petkov -->

<!-- All rights reserved. -->
<!-- This code is proprietary and confidential.   -->
<!-- Unauthorized copying, modification, distribution, or use is strictly prohibited. -->

<template>
  <div class="fixed inset-0 z-50 w-full h-full overflow-y-auto bg-gray-600 bg-opacity-50">
    <div class="relative w-11/12 max-w-4xl p-5 mx-auto bg-white border rounded-md shadow-lg top-20 dark:bg-gray-800">
      <!-- Header -->
      <div class="flex items-center justify-between pb-4 border-b border-gray-200 dark:border-gray-700">
        <h3 class="text-lg font-medium text-gray-900 dark:text-white">
          {{ isView ? 'Invoice Details' : 'Create New Invoice' }}
        </h3>
        <button
          @click="$emit('close')"
          class="text-gray-400 hover:text-gray-600 dark:hover:text-gray-300"
        >
          <XMarkIcon class="w-5 h-5" />
        </button>
      </div>

      <!-- Content -->
      <div v-if="isView && invoice" class="mt-6">
        <!-- View Mode - Invoice Details -->
        <div class="space-y-6">
          <!-- Invoice Header -->
          <div class="p-6 rounded-lg bg-gray-50 dark:bg-gray-900">
            <div class="flex items-start justify-between">
              <div>
                <h2 class="text-2xl font-bold text-gray-900 dark:text-white">
                  Invoice #{{ invoice.invoiceNumber }}
                </h2>
                <p class="mt-1 text-sm text-gray-500 dark:text-gray-400">
                  Created: {{ formatDate(invoice.createdAt) }}
                </p>
              </div>
              <div class="text-right">
                <div class="text-3xl font-bold text-purple-600 dark:text-purple-400">
                  ${{ invoice.totalAmount.toLocaleString() }}
                </div>
                <span 
                  class="inline-flex items-center px-3 py-1 mt-2 text-sm font-medium rounded-full"
                  :class="getStatusBadgeClass(invoice.status)"
                >
                  {{ invoice.status }}
                </span>
              </div>
            </div>
          </div>

          <!-- Patient Information -->
          <div class="grid grid-cols-1 gap-6 md:grid-cols-2">
            <div class="p-4 bg-white border border-gray-200 rounded-lg dark:bg-gray-800 dark:border-gray-700">
              <h4 class="mb-3 text-sm font-medium text-gray-900 dark:text-white">Patient Information</h4>
              <div class="space-y-2">
                <div>
                  <div class="text-xs text-gray-500 dark:text-gray-400">Patient Name</div>
                  <div class="text-sm font-medium text-gray-900 dark:text-white">{{ invoice.patientName }}</div>
                </div>
                <div>
                  <div class="text-xs text-gray-500 dark:text-gray-400">Patient ID</div>
                  <div class="text-sm font-medium text-gray-900 dark:text-white">{{ invoice.patientId }}</div>
                </div>
              </div>
            </div>

            <div class="p-4 bg-white border border-gray-200 rounded-lg dark:bg-gray-800 dark:border-gray-700">
              <h4 class="mb-3 text-sm font-medium text-gray-900 dark:text-white">Invoice Details</h4>
              <div class="space-y-2">
                <div>
                  <div class="text-xs text-gray-500 dark:text-gray-400">Due Date</div>
                  <div class="text-sm font-medium text-gray-900 dark:text-white">{{ formatDate(invoice.dueDate) }}</div>
                </div>
                <div v-if="invoice.paidAt">
                  <div class="text-xs text-gray-500 dark:text-gray-400">Paid Date</div>
                  <div class="text-sm font-medium text-gray-900 dark:text-white">{{ formatDate(invoice.paidAt) }}</div>
                </div>
              </div>
            </div>
          </div>

          <!-- Services -->
          <div class="bg-white border border-gray-200 rounded-lg dark:bg-gray-800 dark:border-gray-700">
            <div class="px-6 py-4 border-b border-gray-200 dark:border-gray-700">
              <h4 class="text-sm font-medium text-gray-900 dark:text-white">Services & Charges</h4>
            </div>
            <div class="overflow-x-auto">
              <table class="min-w-full divide-y divide-gray-200 dark:divide-gray-700">
                <thead class="bg-gray-50 dark:bg-gray-900">
                  <tr>
                    <th class="px-6 py-3 text-xs font-medium tracking-wider text-left text-gray-500 uppercase dark:text-gray-400">Service</th>
                    <th class="px-6 py-3 text-xs font-medium tracking-wider text-left text-gray-500 uppercase dark:text-gray-400">Department</th>
                    <th class="px-6 py-3 text-xs font-medium tracking-wider text-left text-gray-500 uppercase dark:text-gray-400">Quantity</th>
                    <th class="px-6 py-3 text-xs font-medium tracking-wider text-left text-gray-500 uppercase dark:text-gray-400">Unit Price</th>
                    <th class="px-6 py-3 text-xs font-medium tracking-wider text-left text-gray-500 uppercase dark:text-gray-400">Total</th>
                  </tr>
                </thead>
                <tbody class="bg-white divide-y divide-gray-200 dark:bg-gray-800 dark:divide-gray-700">
                  <tr v-for="service in invoice.services" :key="service.id">
                    <td class="px-6 py-4 whitespace-nowrap">
                      <div class="text-sm font-medium text-gray-900 dark:text-white">{{ service.name }}</div>
                      <div v-if="service.description" class="text-xs text-gray-500 dark:text-gray-400">{{ service.description }}</div>
                    </td>
                    <td class="px-6 py-4 text-sm text-gray-900 capitalize whitespace-nowrap dark:text-white">{{ service.department }}</td>
                    <td class="px-6 py-4 text-sm text-gray-900 whitespace-nowrap dark:text-white">{{ service.quantity }}</td>
                    <td class="px-6 py-4 text-sm text-gray-900 whitespace-nowrap dark:text-white">${{ service.amount.toLocaleString() }}</td>
                    <td class="px-6 py-4 text-sm font-medium text-gray-900 whitespace-nowrap dark:text-white">${{ (service.amount * service.quantity).toLocaleString() }}</td>
                  </tr>
                </tbody>
                <tfoot class="bg-gray-50 dark:bg-gray-900">
                  <tr>
                    <td colspan="4" class="px-6 py-3 text-sm font-medium text-right text-gray-900 dark:text-white">Total Amount:</td>
                    <td class="px-6 py-3 text-sm font-bold text-purple-600 dark:text-purple-400">${{ invoice.totalAmount.toLocaleString() }}</td>
                  </tr>
                </tfoot>
              </table>
            </div>
          </div>

          <!-- Actions -->
          <div class="flex justify-end space-x-3">
            <button
              @click="downloadInvoice"
              class="inline-flex items-center px-4 py-2 text-sm font-medium text-gray-700 bg-white border border-gray-300 rounded-md dark:border-gray-600 dark:text-gray-300 dark:bg-gray-700 hover:bg-gray-50 dark:hover:bg-gray-600 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-gray-500"
            >
              <DocumentArrowDownIcon class="w-4 h-4 mr-2" />
              Download PDF
            </button>
          </div>
        </div>
      </div>

      <!-- Create Mode - Invoice Form -->
      <form v-else @submit.prevent="handleSubmit" class="mt-6 space-y-6">
        <!-- Patient Selection -->
        <div>
          <label for="patient" class="block text-sm font-medium text-gray-700 dark:text-gray-300">
            Patient <span class="text-red-500">*</span>
          </label>
          <select
            id="patient"
            v-model="form.patientId"
            required
            @change="updatePatientInfo"
            class="block w-full px-3 py-2 mt-1 text-gray-900 bg-white border border-gray-300 rounded-md shadow-sm dark:border-gray-600 dark:bg-gray-700 dark:text-white focus:outline-none focus:ring-1 focus:ring-purple-500 focus:border-purple-500"
          >
            <option value="">Select a patient</option>
            <option 
              v-for="patient in availablePatients" 
              :key="patient.id" 
              :value="patient.id"
            >
              {{ patient.firstName }} {{ patient.lastName }} - {{ patient.id }}
            </option>
          </select>
        </div>

        <!-- Invoice Details -->
        <div class="grid grid-cols-1 gap-4 md:grid-cols-2">
          <div>
            <label for="invoiceNumber" class="block text-sm font-medium text-gray-700 dark:text-gray-300">
              Invoice Number <span class="text-red-500">*</span>
            </label>
            <input
              id="invoiceNumber"
              v-model="form.invoiceNumber"
              type="text"
              required
              placeholder="INV-001"
              class="block w-full px-3 py-2 mt-1 text-gray-900 bg-white border border-gray-300 rounded-md shadow-sm dark:border-gray-600 dark:bg-gray-700 dark:text-white focus:outline-none focus:ring-1 focus:ring-purple-500 focus:border-purple-500"
            />
          </div>

          <div>
            <label for="dueDate" class="block text-sm font-medium text-gray-700 dark:text-gray-300">
              Due Date <span class="text-red-500">*</span>
            </label>
            <input
              id="dueDate"
              v-model="form.dueDate"
              type="date"
              required
              :min="minDate"
              class="block w-full px-3 py-2 mt-1 text-gray-900 bg-white border border-gray-300 rounded-md shadow-sm dark:border-gray-600 dark:bg-gray-700 dark:text-white focus:outline-none focus:ring-1 focus:ring-purple-500 focus:border-purple-500"
            />
          </div>
        </div>

        <!-- Services -->
        <div>
          <div class="flex items-center justify-between mb-4">
            <label class="block text-sm font-medium text-gray-700 dark:text-gray-300">
              Services <span class="text-red-500">*</span>
            </label>
            <button
              type="button"
              @click="addService"
              class="inline-flex items-center px-3 py-2 text-sm font-medium leading-4 text-purple-700 bg-purple-100 border border-transparent rounded-md hover:bg-purple-200 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-purple-500"
            >
              <PlusIcon class="w-4 h-4 mr-1" />
              Add Service
            </button>
          </div>

          <div class="space-y-4">
            <div
              v-for="(service, index) in form.services"
              :key="service.id"
              class="p-4 rounded-lg bg-gray-50 dark:bg-gray-900"
            >
              <div class="flex items-start justify-between mb-4">
                <h4 class="text-sm font-medium text-gray-900 dark:text-white">Service {{ index + 1 }}</h4>
                <button
                  type="button"
                  @click="removeService(index)"
                  class="text-red-600 hover:text-red-900 dark:text-red-400 dark:hover:text-red-300"
                >
                  <XMarkIcon class="w-4 h-4" />
                </button>
              </div>

              <div class="grid grid-cols-1 gap-4 md:grid-cols-2">
                <div>
                  <label class="block text-xs font-medium text-gray-700 dark:text-gray-300">Service Name</label>
                  <input
                    v-model="service.name"
                    type="text"
                    required
                    placeholder="Enter service name"
                    class="block w-full px-3 py-2 mt-1 text-sm text-gray-900 bg-white border border-gray-300 rounded-md dark:border-gray-600 dark:bg-gray-700 dark:text-white focus:outline-none focus:ring-1 focus:ring-purple-500 focus:border-purple-500"
                  />
                </div>

                <div>
                  <label class="block text-xs font-medium text-gray-700 dark:text-gray-300">Department</label>
                  <select
                    v-model="service.department"
                    required
                    class="block w-full px-3 py-2 mt-1 text-sm text-gray-900 bg-white border border-gray-300 rounded-md dark:border-gray-600 dark:bg-gray-700 dark:text-white focus:outline-none focus:ring-1 focus:ring-purple-500 focus:border-purple-500"
                  >
                    <option value="">Select department</option>
                    <option value="cardiology">Cardiology</option>
                    <option value="neurology">Neurology</option>
                    <option value="oncology">Oncology</option>
                    <option value="radiology">Radiology</option>
                    <option value="emergency">Emergency</option>
                    <option value="pediatrics">Pediatrics</option>
                  </select>
                </div>

                <div>
                  <label class="block text-xs font-medium text-gray-700 dark:text-gray-300">Quantity</label>
                  <input
                    v-model.number="service.quantity"
                    type="number"
                    min="1"
                    required
                    class="block w-full px-3 py-2 mt-1 text-sm text-gray-900 bg-white border border-gray-300 rounded-md dark:border-gray-600 dark:bg-gray-700 dark:text-white focus:outline-none focus:ring-1 focus:ring-purple-500 focus:border-purple-500"
                  />
                </div>

                <div>
                  <label class="block text-xs font-medium text-gray-700 dark:text-gray-300">Unit Price ($)</label>
                  <input
                    v-model.number="service.amount"
                    type="number"
                    min="0"
                    step="0.01"
                    required
                    class="block w-full px-3 py-2 mt-1 text-sm text-gray-900 bg-white border border-gray-300 rounded-md dark:border-gray-600 dark:bg-gray-700 dark:text-white focus:outline-none focus:ring-1 focus:ring-purple-500 focus:border-purple-500"
                  />
                </div>
              </div>

              <div class="mt-4">
                <label class="block text-xs font-medium text-gray-700 dark:text-gray-300">Description (Optional)</label>
                <textarea
                  v-model="service.description"
                  rows="2"
                  placeholder="Service description..."
                  class="block w-full px-3 py-2 mt-1 text-sm text-gray-900 bg-white border border-gray-300 rounded-md dark:border-gray-600 dark:bg-gray-700 dark:text-white focus:outline-none focus:ring-1 focus:ring-purple-500 focus:border-purple-500"
                />
              </div>

              <div class="mt-2 text-right">
                <span class="text-sm font-medium text-gray-900 dark:text-white">
                  Total: ${{ (service.quantity * service.amount).toLocaleString() }}
                </span>
              </div>
            </div>
          </div>

          <!-- Total -->
          <div class="p-4 rounded-lg bg-purple-50 dark:bg-purple-900/20">
            <div class="flex items-center justify-between">
              <span class="text-lg font-medium text-gray-900 dark:text-white">Invoice Total:</span>
              <span class="text-2xl font-bold text-purple-600 dark:text-purple-400">
                ${{ calculateTotal().toLocaleString() }}
              </span>
            </div>
          </div>
        </div>

        <!-- Actions -->
        <div class="flex justify-end pt-6 space-x-3 border-t border-gray-200 dark:border-gray-700">
          <button
            type="button"
            @click="$emit('close')"
            class="px-4 py-2 text-sm font-medium text-gray-700 bg-white border border-gray-300 rounded-md dark:text-gray-300 dark:bg-gray-700 dark:border-gray-600 hover:bg-gray-50 dark:hover:bg-gray-600 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-gray-500"
          >
            Cancel
          </button>
          <button
            type="submit"
            :disabled="!isFormValid"
            class="px-4 py-2 text-sm font-medium text-white bg-purple-600 border border-transparent rounded-md hover:bg-purple-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-purple-500 disabled:opacity-50 disabled:cursor-not-allowed"
          >
            Create Invoice
          </button>
        </div>
      </form>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import { usePatientsStore } from '../../stores/patients'
import { usePaymentStore, type Invoice } from '../../stores/payment'

interface Props {
  invoice?: Invoice | null
  isView?: boolean
}

interface ServiceForm {
  id: string
  name: string
  department: string
  quantity: number
  amount: number
  description?: string
}

interface InvoiceForm {
  patientId: string
  patientName: string
  invoiceNumber: string
  dueDate: string
  services: ServiceForm[]
}

const props = defineProps<Props>()
const emit = defineEmits<{
  close: []
  save: [data: Omit<Invoice, 'id'>]
}>()

// Stores
const patientsStore = usePatientsStore()
const paymentStore = usePaymentStore()

// Form data
const form = ref<InvoiceForm>({
  patientId: '',
  patientName: '',
  invoiceNumber: '',
  dueDate: '',
  services: []
})

// Computed properties
const minDate = computed(() => {
  return new Date().toISOString().split('T')[0]
})

const availablePatients = computed(() => {
  return patientsStore.patients
})

const isFormValid = computed(() => {
  return form.value.patientId &&
         form.value.invoiceNumber &&
         form.value.dueDate &&
         form.value.services.length > 0 &&
         form.value.services.every(s => s.name && s.department && s.quantity > 0 && s.amount > 0)
})

// Icon components
const XMarkIcon = {
  template: `<svg fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" d="M6 18L18 6M6 6l12 12" /></svg>`
}

const PlusIcon = {
  template: `<svg fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" d="M12 4.5v15m7.5-7.5h-15" /></svg>`
}

const DocumentArrowDownIcon = {
  template: `<svg fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" d="M19.5 14.25v-2.625a3.375 3.375 0 00-3.375-3.375h-4.5A1.125 1.125 0 0010.5 9h-4.5a3.375 3.375 0 00-3.375 3.375v8.25a3.375 3.375 0 003.375 3.375h9a3.375 3.375 0 003.375-3.375zM9 16.5v-4.5m1.5 0L9 10.5l-1.5 1.5" /></svg>`
}

// Methods
const formatDate = (date: string) => {
  return new Date(date).toLocaleDateString()
}

const getStatusBadgeClass = (status: string) => {
  const classes: Record<string, string> = {
    paid: 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-200',
    pending: 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900 dark:text-yellow-200',
    overdue: 'bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-200',
    draft: 'bg-gray-100 text-gray-800 dark:bg-gray-900 dark:text-gray-200',
    open: 'bg-blue-100 text-blue-800 dark:bg-blue-900 dark:text-blue-200',
    void: 'bg-gray-100 text-gray-800 dark:bg-gray-900 dark:text-gray-200',
    uncollectible: 'bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-200',
    cancelled: 'bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-200'
  }
  return classes[status] || classes.pending
}

const updatePatientInfo = () => {
  const selectedPatient = availablePatients.value.find(p => p.id === form.value.patientId)
  if (selectedPatient) {
    form.value.patientName = `${selectedPatient.firstName} ${selectedPatient.lastName}`
  }
}

const addService = () => {
  form.value.services.push({
    id: `service-${Date.now()}`,
    name: '',
    department: '',
    quantity: 1,
    amount: 0,
    description: ''
  })
}

const removeService = (index: number) => {
  form.value.services.splice(index, 1)
}

const calculateTotal = () => {
  return form.value.services.reduce((total, service) => {
    return total + (service.quantity * service.amount)
  }, 0)
}

const downloadInvoice = async () => {
  if (props.invoice) {
    try {
      await paymentStore.downloadInvoice(props.invoice.id)
    } catch (error) {
      console.error('Error downloading invoice:', error)
      alert('Failed to download invoice. Please try again.')
    }
  }
}

const handleSubmit = () => {
  if (!isFormValid.value) return

  const invoiceData: Omit<Invoice, 'id'> = {
    patientId: form.value.patientId,
    patientName: form.value.patientName,
    invoiceNumber: form.value.invoiceNumber,
    dueDate: form.value.dueDate,
    amount: calculateTotal(),
    totalAmount: calculateTotal(),
    currency: 'USD',
    status: 'pending',
    createdAt: new Date().toISOString(),
    services: form.value.services.map(service => ({
      id: service.id,
      name: service.name,
      department: service.department,
      quantity: service.quantity,
      amount: service.amount,
      description: service.description
    }))
  }

  emit('save', invoiceData)
}

// Initialize form if needed
if (props.isView && props.invoice && !form.value.services.length) {
  // Add a default service if creating a new invoice
  addService()
}
</script>
