<template>
  <div v-if="show" class="fixed inset-0 bg-black bg-opacity-30 flex items-center justify-center p-4 z-50">
    <div class="bg-white dark:bg-gray-800 rounded-lg shadow-xl w-full max-w-2xl max-h-[90vh] overflow-hidden">
      <!-- Header -->
      <div class="px-6 py-4 border-b border-gray-200 dark:border-gray-700 flex items-center justify-between">
        <h2 class="text-xl font-semibold text-gray-900 dark:text-white">Create Support Ticket</h2>
        <button
          @click="$emit('close')"
          class="text-gray-400 hover:text-gray-600 dark:hover:text-gray-300 transition-colors"
        >
          <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"></path>
          </svg>
        </button>
      </div>

      <!-- Form -->
      <form @submit.prevent="submitTicket" class="p-6 space-y-6 overflow-y-auto max-h-[calc(90vh-120px)]">
        <!-- Subject -->
        <div>
          <label for="subject" class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
            Subject *
          </label>
          <input
            id="subject"
            v-model="form.subject"
            type="text"
            required
            maxlength="200"
            class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md bg-white dark:bg-gray-700 text-gray-900 dark:text-white focus:ring-blue-500 focus:border-blue-500"
            placeholder="Brief description of your issue"
          />
        </div>

        <!-- Category -->
        <div>
          <label for="category" class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
            Category *
          </label>
          <select
            id="category"
            v-model.number="form.category"
            required
            class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md bg-white dark:bg-gray-700 text-gray-900 dark:text-white focus:ring-blue-500 focus:border-blue-500"
          >
            <option value="0">Technical Support</option>
            <option value="1">Billing & Subscription</option>
            <option value="2">General Question</option>
            <option value="3">Feature Request</option>
            <option value="4">Bug Report</option>
          </select>
        </div>

        <!-- Priority -->
        <div>
          <label for="priority" class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
            Priority *
          </label>
          <select
            id="priority"
            v-model.number="form.priority"
            required
            class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md bg-white dark:bg-gray-700 text-gray-900 dark:text-white focus:ring-blue-500 focus:border-blue-500"
          >
            <option value="0">Low - General inquiry</option>
            <option value="1">Medium - Non-critical issue</option>
            <option value="2">High - Important issue affecting work</option>
            <option value="3">Critical - Service is down or unusable</option>
          </select>
        </div>

        <!-- Description -->
        <div>
          <label for="description" class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
            Description *
          </label>
          <textarea
            id="description"
            v-model="form.description"
            required
            maxlength="2000"
            rows="6"
            class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md bg-white dark:bg-gray-700 text-gray-900 dark:text-white focus:ring-blue-500 focus:border-blue-500"
            placeholder="Please provide detailed information about your issue, including any error messages, steps to reproduce, or relevant context..."
          ></textarea>
          <p class="mt-1 text-sm text-gray-500 dark:text-gray-400">
            {{ form.description.length }}/2000 characters
          </p>
        </div>

        <!-- Tips -->
        <div class="bg-blue-50 dark:bg-blue-900/20 border border-blue-200 dark:border-blue-800 rounded-md p-4">
          <h4 class="text-sm font-medium text-blue-900 dark:text-blue-300 mb-2">Tips for better support:</h4>
          <ul class="text-sm text-blue-800 dark:text-blue-300 space-y-1">
            <li>• Include specific error messages if any</li>
            <li>• Describe the steps to reproduce the issue</li>
            <li>• Mention your browser and operating system</li>
            <li>• Attach screenshots if relevant</li>
          </ul>
        </div>

        <!-- Error Message -->
        <div v-if="error" class="bg-red-50 dark:bg-red-900/20 border border-red-200 dark:border-red-800 rounded-md p-4">
          <div class="flex">
            <div class="flex-shrink-0">
              <svg class="h-5 w-5 text-red-400" viewBox="0 0 20 20" fill="currentColor">
                <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zM8.707 7.293a1 1 0 00-1.414 1.414L8.586 10l-1.293 1.293a1 1 0 101.414 1.414L10 11.414l1.293 1.293a1 1 0 001.414-1.414L11.414 10l1.293-1.293a1 1 0 00-1.414-1.414L10 8.586 8.707 7.293z" clip-rule="evenodd" />
              </svg>
            </div>
            <div class="ml-3">
              <p class="text-sm text-red-800 dark:text-red-300">{{ error }}</p>
            </div>
          </div>
        </div>
      </form>

      <!-- Footer -->
      <div class="px-6 py-4 border-t border-gray-200 dark:border-gray-700 flex items-center justify-end space-x-3">
        <button
          type="button"
          @click="$emit('close')"
          class="px-4 py-2 text-gray-700 dark:text-gray-300 hover:bg-gray-100 dark:hover:bg-gray-700 rounded-md transition-colors"
          :disabled="loading"
        >
          Cancel
        </button>
        <button
          type="submit"
          @click="submitTicket"
          :disabled="loading || !isFormValid"
          class="px-4 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700 disabled:opacity-50 disabled:cursor-not-allowed transition-colors flex items-center space-x-2"
        >
          <div v-if="loading" class="animate-spin rounded-full h-4 w-4 border-b-2 border-white"></div>
          <span>{{ loading ? 'Creating...' : 'Create Ticket' }}</span>
        </button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import { useSupportStore, type CreateTicketRequest } from '../../stores/support'

interface Props {
  show: boolean
}

interface Emits {
  (e: 'close'): void
  (e: 'created', ticket: any): void
}

const props = defineProps<Props>()
const emit = defineEmits<Emits>()

const supportStore = useSupportStore()

const form = ref<CreateTicketRequest>({
  subject: '',
  description: '',
  priority: 1, // Medium
  category: 2  // General
})

const loading = ref(false)
const error = ref('')

const isFormValid = computed(() => {
  return form.value.subject.trim() !== '' &&
         form.value.description.trim() !== ''
})

// Reset form when modal is opened
watch(() => props.show, (newShow) => {
  if (newShow) {
    resetForm()
  }
})

const resetForm = () => {
  form.value = {
    subject: '',
    description: '',
    priority: 1, // Medium
    category: 2  // General
  }
  error.value = ''
  loading.value = false
}

const submitTicket = async () => {
  if (!isFormValid.value) return

  loading.value = true
  error.value = ''

  try {
    console.log('Submitting ticket data:', form.value) // Debug log
    const ticket = await supportStore.createTicket(form.value)
    emit('created', ticket)
    resetForm()
  } catch (err: any) {
    console.error('Error creating ticket:', err) // Debug log
    error.value = err.message || 'Failed to create ticket'
  } finally {
    loading.value = false
  }
}
</script>