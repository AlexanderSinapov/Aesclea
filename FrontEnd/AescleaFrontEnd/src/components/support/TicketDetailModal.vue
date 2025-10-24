<template>
  <div v-if="show" class="fixed inset-0 bg-black bg-opacity-30 flex items-center justify-center p-4 z-50">
    <div class="bg-white dark:bg-gray-800 rounded-lg shadow-xl w-full max-w-4xl max-h-[90vh] overflow-hidden flex flex-col">
      <!-- Header -->
      <div class="px-6 py-4 border-b border-gray-200 dark:border-gray-700 flex items-center justify-between flex-shrink-0">
        <div class="flex items-center space-x-4">
          <h2 class="text-xl font-semibold text-gray-900 dark:text-white">{{ ticket?.subject }}</h2>
          <span
            :class="getStatusBadgeClass(ticket?.status)"
            class="px-2 py-1 text-xs font-medium rounded-full"
          >
            {{ formatStatus(ticket?.status) }}
          </span>
          <span
            :class="getPriorityBadgeClass(ticket?.priority)"
            class="px-2 py-1 text-xs font-medium rounded-full"
          >
            {{ ticket?.priority }}
          </span>
        </div>
        <button
          @click="$emit('close')"
          class="text-gray-400 hover:text-gray-600 dark:hover:text-gray-300 transition-colors"
        >
          <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"></path>
          </svg>
        </button>
      </div>

      <!-- Content -->
      <div class="flex flex-1 overflow-hidden">
        <!-- Ticket Details -->
        <div class="w-1/3 border-r border-gray-200 dark:border-gray-700 p-6 overflow-y-auto">
          <h3 class="text-lg font-medium text-gray-900 dark:text-white mb-4">Ticket Details</h3>
          
          <div class="space-y-4">
            <div>
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">Ticket ID</label>
              <p class="text-sm text-gray-900 dark:text-white font-mono">{{ ticket?.id }}</p>
            </div>

            <div>
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">Category</label>
              <p class="text-sm text-gray-900 dark:text-white">{{ ticket?.category }}</p>
            </div>

            <div>
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">Created</label>
              <p class="text-sm text-gray-900 dark:text-white">{{ formatDate(ticket?.createdAt) }}</p>
            </div>

            <div>
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">Last Updated</label>
              <p class="text-sm text-gray-900 dark:text-white">{{ formatDate(ticket?.updatedAt) }}</p>
            </div>

            <div v-if="ticket?.assignedAgent">
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">Assigned Agent</label>
              <p class="text-sm text-gray-900 dark:text-white">
                {{ ticket.assignedAgent.firstName }} {{ ticket.assignedAgent.lastName }}
              </p>
            </div>

            <div v-if="ticket?.resolvedAt">
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">Resolved</label>
              <p class="text-sm text-gray-900 dark:text-white">{{ formatDate(ticket.resolvedAt) }}</p>
            </div>

            <div v-if="ticket?.resolutionNote">
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">Resolution Note</label>
              <p class="text-sm text-gray-900 dark:text-white">{{ ticket.resolutionNote }}</p>
            </div>
          </div>
        </div>

        <!-- Messages -->
        <div class="flex-1 flex flex-col">
          <!-- Messages Header -->
          <div class="px-6 py-4 border-b border-gray-200 dark:border-gray-700 flex-shrink-0">
            <h3 class="text-lg font-medium text-gray-900 dark:text-white">Conversation</h3>
          </div>

          <!-- Messages List -->
          <div class="flex-1 overflow-y-auto p-6 space-y-4">
            <!-- Initial Description -->
            <div class="bg-blue-50 dark:bg-blue-900/20 border border-blue-200 dark:border-blue-800 rounded-lg p-4">
              <div class="flex items-start space-x-3">
                <div class="flex-shrink-0">
                  <div class="w-8 h-8 bg-blue-600 rounded-full flex items-center justify-center">
                    <span class="text-white text-sm font-medium">
                      {{ ticket?.user?.firstName?.charAt(0) }}{{ ticket?.user?.lastName?.charAt(0) }}
                    </span>
                  </div>
                </div>
                <div class="flex-1">
                  <div class="flex items-center space-x-2 mb-2">
                    <p class="text-sm font-medium text-gray-900 dark:text-white">
                      {{ ticket?.user?.firstName }} {{ ticket?.user?.lastName }}
                    </p>
                    <span class="text-xs text-gray-500 dark:text-gray-400">
                      {{ formatDate(ticket?.createdAt) }}
                    </span>
                  </div>
                  <div class="text-sm text-gray-800 dark:text-gray-200 whitespace-pre-wrap">{{ ticket?.description }}</div>
                </div>
              </div>
            </div>

            <!-- Messages -->
            <div
              v-for="message in messages"
              :key="message.id"
              :class="[
                'rounded-lg p-4',
                message.isFromAgent
                  ? 'bg-green-50 dark:bg-green-900/20 border border-green-200 dark:border-green-800'
                  : 'bg-gray-50 dark:bg-gray-700 border border-gray-200 dark:border-gray-600'
              ]"
            >
              <div class="flex items-start space-x-3">
                <div class="flex-shrink-0">
                  <div
                    :class="[
                      'w-8 h-8 rounded-full flex items-center justify-center text-sm font-medium',
                      message.isFromAgent
                        ? 'bg-green-600 text-white'
                        : 'bg-gray-600 text-white'
                    ]"
                  >
                    {{ message.sender?.firstName?.charAt(0) }}{{ message.sender?.lastName?.charAt(0) }}
                  </div>
                </div>
                <div class="flex-1">
                  <div class="flex items-center space-x-2 mb-2">
                    <p class="text-sm font-medium text-gray-900 dark:text-white">
                      {{ message.sender?.firstName }} {{ message.sender?.lastName }}
                      <span v-if="message.isFromAgent" class="text-green-600 dark:text-green-400">(Support Agent)</span>
                    </p>
                    <span class="text-xs text-gray-500 dark:text-gray-400">
                      {{ formatDate(message.createdAt) }}
                    </span>
                  </div>
                  <div class="text-sm text-gray-800 dark:text-gray-200 whitespace-pre-wrap">{{ message.content }}</div>
                </div>
              </div>
            </div>

            <!-- Loading Messages -->
            <div v-if="loadingMessages" class="text-center py-4">
              <div class="animate-spin rounded-full h-6 w-6 border-b-2 border-blue-600 mx-auto"></div>
              <p class="mt-2 text-sm text-gray-600 dark:text-gray-400">Loading messages...</p>
            </div>
          </div>

          <!-- Message Input -->
          <div class="border-t border-gray-200 dark:border-gray-700 p-6 flex-shrink-0">
            <form @submit.prevent="sendMessage" class="flex space-x-4">
              <div class="flex-1">
                <textarea
                  v-model="newMessage"
                  rows="3"
                  placeholder="Type your message..."
                  class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md bg-white dark:bg-gray-700 text-gray-900 dark:text-white focus:ring-blue-500 focus:border-blue-500 resize-none"
                  :disabled="sendingMessage || ticket?.status === 'Closed'"
                ></textarea>
              </div>
              <button
                type="submit"
                :disabled="!newMessage.trim() || sendingMessage || ticket?.status === 'Closed'"
                class="px-4 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700 disabled:opacity-50 disabled:cursor-not-allowed transition-colors flex items-center space-x-2"
              >
                <div v-if="sendingMessage" class="animate-spin rounded-full h-4 w-4 border-b-2 border-white"></div>
                <span>{{ sendingMessage ? 'Sending...' : 'Send' }}</span>
              </button>
            </form>
            
            <p v-if="ticket?.status === 'Closed'" class="mt-2 text-sm text-gray-500 dark:text-gray-400">
              This ticket is closed. Contact support to reopen if needed.
            </p>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue'
import { useSupportStore, type SupportTicket, type TicketMessage } from '../../stores/support'

interface Props {
  show: boolean
  ticket: SupportTicket | null
}

const props = defineProps<Props>()

const supportStore = useSupportStore()

const messages = ref<TicketMessage[]>([])
const newMessage = ref('')
const loadingMessages = ref(false)
const sendingMessage = ref(false)

// Load messages when ticket changes
watch(() => props.ticket, async (newTicket) => {
  if (newTicket) {
    await loadMessages(newTicket.id)
  }
}, { immediate: true })

// Load messages when modal opens
watch(() => props.show, async (newShow) => {
  if (newShow && props.ticket) {
    await loadMessages(props.ticket.id)
  }
})

const loadMessages = async (ticketId: string) => {
  loadingMessages.value = true
  try {
    await supportStore.loadTicketMessages(ticketId)
    messages.value = supportStore.currentMessages
  } catch (error) {
    console.error('Error loading messages:', error)
  } finally {
    loadingMessages.value = false
  }
}

const sendMessage = async () => {
  if (!newMessage.value.trim() || !props.ticket) return

  sendingMessage.value = true
  try {
    const message = await supportStore.sendMessage(props.ticket.id, newMessage.value.trim())
    messages.value.push(message)
    newMessage.value = ''
  } catch (error) {
    console.error('Error sending message:', error)
  } finally {
    sendingMessage.value = false
  }
}

const getStatusBadgeClass = (status?: string) => {
  const classes = {
    'Open': 'bg-blue-100 text-blue-800 dark:bg-blue-900/20 dark:text-blue-300',
    'InProgress': 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900/20 dark:text-yellow-300',
    'Waiting': 'bg-orange-100 text-orange-800 dark:bg-orange-900/20 dark:text-orange-300',
    'Resolved': 'bg-green-100 text-green-800 dark:bg-green-900/20 dark:text-green-300',
    'Closed': 'bg-gray-100 text-gray-800 dark:bg-gray-700 dark:text-gray-300'
  }
  return classes[status as keyof typeof classes] || 'bg-gray-100 text-gray-800 dark:bg-gray-700 dark:text-gray-300'
}

const getPriorityBadgeClass = (priority?: string) => {
  const classes = {
    'Low': 'bg-green-100 text-green-800 dark:bg-green-900/20 dark:text-green-300',
    'Medium': 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900/20 dark:text-yellow-300',
    'High': 'bg-orange-100 text-orange-800 dark:bg-orange-900/20 dark:text-orange-300',
    'Critical': 'bg-red-100 text-red-800 dark:bg-red-900/20 dark:text-red-300'
  }
  return classes[priority as keyof typeof classes] || 'bg-gray-100 text-gray-800 dark:bg-gray-700 dark:text-gray-300'
}

const formatStatus = (status?: string) => {
  return status?.replace(/([A-Z])/g, ' $1').trim()
}

const formatDate = (dateString?: string) => {
  if (!dateString) return ''
  return new Date(dateString).toLocaleDateString() + ' ' + new Date(dateString).toLocaleTimeString()
}
</script>