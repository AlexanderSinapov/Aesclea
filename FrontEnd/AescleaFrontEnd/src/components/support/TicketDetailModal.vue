<template>
  <div v-if="show" class="fixed inset-0 z-50 flex items-center justify-center p-4 bg-black bg-opacity-30">
    <div class="bg-white dark:bg-gray-800 rounded-lg shadow-xl w-full max-w-4xl max-h-[90vh] overflow-hidden flex flex-col">
      <!-- Header -->
      <div class="flex items-center justify-between flex-shrink-0 px-6 py-4 border-b border-gray-200 dark:border-gray-700">
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
          class="text-gray-400 transition-colors hover:text-gray-600 dark:hover:text-gray-300"
        >
          <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"></path>
          </svg>
        </button>
      </div>

      <!-- Content -->
      <div class="flex flex-1 overflow-hidden">
        <!-- Ticket Details -->
        <div class="w-1/3 p-6 overflow-y-auto border-r border-gray-200 dark:border-gray-700">
          <h3 class="mb-4 text-lg font-medium text-gray-900 dark:text-white">Ticket Details</h3>
          
          <div class="space-y-4">
            <div>
              <label class="block mb-1 text-sm font-medium text-gray-700 dark:text-gray-300">Ticket ID</label>
              <p class="font-mono text-sm text-gray-900 dark:text-white">{{ ticket?.id }}</p>
            </div>

            <div>
              <label class="block mb-1 text-sm font-medium text-gray-700 dark:text-gray-300">Category</label>
              <p class="text-sm text-gray-900 dark:text-white">{{ ticket?.category }}</p>
            </div>

            <div>
              <label class="block mb-1 text-sm font-medium text-gray-700 dark:text-gray-300">Created</label>
              <p class="text-sm text-gray-900 dark:text-white">{{ formatDate(ticket?.createdAt) }}</p>
            </div>

            <div>
              <label class="block mb-1 text-sm font-medium text-gray-700 dark:text-gray-300">Last Updated</label>
              <p class="text-sm text-gray-900 dark:text-white">{{ formatDate(ticket?.updatedAt) }}</p>
            </div>

            <div v-if="ticket?.assignedAgent">
              <label class="block mb-1 text-sm font-medium text-gray-700 dark:text-gray-300">Assigned Agent</label>
              <p class="text-sm text-gray-900 dark:text-white">
                {{ ticket.assignedAgent.firstName }} {{ ticket.assignedAgent.lastName }}
              </p>
            </div>

            <div v-if="ticket?.resolvedAt">
              <label class="block mb-1 text-sm font-medium text-gray-700 dark:text-gray-300">Resolved</label>
              <p class="text-sm text-gray-900 dark:text-white">{{ formatDate(ticket.resolvedAt) }}</p>
            </div>

            <div v-if="ticket?.resolutionNote">
              <label class="block mb-1 text-sm font-medium text-gray-700 dark:text-gray-300">Resolution Note</label>
              <p class="text-sm text-gray-900 dark:text-white">{{ ticket.resolutionNote }}</p>
            </div>
          </div>
        </div>

        <!-- Messages -->
        <div class="flex flex-col flex-1">
          <!-- Messages Header -->
          <div class="flex-shrink-0 px-6 py-4 border-b border-gray-200 dark:border-gray-700">
            <h3 class="text-lg font-medium text-gray-900 dark:text-white">Conversation</h3>
          </div>

          <!-- Messages List -->
          <div class="flex-1 p-6 space-y-4 overflow-y-auto">
            <!-- Initial Description -->
            <div class="p-4 border border-blue-200 rounded-lg bg-blue-50 dark:bg-blue-900/20 dark:border-blue-800">
              <div class="flex items-start space-x-3">
                <div class="flex-shrink-0">
                  <div class="flex items-center justify-center w-8 h-8 bg-blue-600 rounded-full">
                    <span class="text-sm font-medium text-white">
                      {{ ticket?.user?.firstName?.charAt(0) }}{{ ticket?.user?.lastName?.charAt(0) }}
                    </span>
                  </div>
                </div>
                <div class="flex-1">
                  <div class="flex items-center mb-2 space-x-2">
                    <p class="text-sm font-medium text-gray-900 dark:text-white">
                      {{ ticket?.user?.firstName }} {{ ticket?.user?.lastName }}
                    </p>
                    <span class="text-xs text-gray-500 dark:text-gray-400">
                      {{ formatDate(ticket?.createdAt) }}
                    </span>
                  </div>
                  <div class="text-sm text-gray-800 whitespace-pre-wrap dark:text-gray-200">{{ ticket?.description }}</div>
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
                  <div class="flex items-center mb-2 space-x-2">
                    <p class="text-sm font-medium text-gray-900 dark:text-white">
                      {{ message.sender?.firstName }} {{ message.sender?.lastName }}
                      <span v-if="message.isFromAgent" class="text-green-600 dark:text-green-400">(Support Agent)</span>
                    </p>
                    <span class="text-xs text-gray-500 dark:text-gray-400">
                      {{ formatDate(message.createdAt) }}
                    </span>
                  </div>
                  <div class="text-sm text-gray-800 whitespace-pre-wrap dark:text-gray-200">{{ message.content }}</div>
                </div>
              </div>
            </div>

            <!-- Loading Messages -->
            <div v-if="loadingMessages" class="py-4 text-center">
              <div class="w-6 h-6 mx-auto border-b-2 border-blue-600 rounded-full animate-spin"></div>
              <p class="mt-2 text-sm text-gray-600 dark:text-gray-400">Loading messages...</p>
            </div>
          </div>

          <!-- Message Input -->
          <div class="flex-shrink-0 p-6 border-t border-gray-200 dark:border-gray-700">
            <form @submit.prevent="sendMessage" class="flex space-x-4">
              <div class="flex-1">
                <textarea
                  v-model="newMessage"
                  rows="3"
                  placeholder="Type your message..."
                  class="w-full px-3 py-2 text-gray-900 bg-white border border-gray-300 rounded-md resize-none dark:border-gray-600 dark:bg-gray-700 dark:text-white focus:ring-blue-500 focus:border-blue-500"
                  :disabled="sendingMessage || ticket?.status === 'Closed'"
                ></textarea>
              </div>
              <button
                type="submit"
                :disabled="!newMessage.trim() || sendingMessage || ticket?.status === 'Closed'"
                class="flex items-center px-4 py-2 space-x-2 text-white transition-colors bg-blue-600 rounded-md hover:bg-blue-700 disabled:opacity-50 disabled:cursor-not-allowed"
              >
                <div v-if="sendingMessage" class="w-4 h-4 border-b-2 border-white rounded-full animate-spin"></div>
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
  const statusStr = typeof status === 'string' ? status : String(status)
  return statusStr?.replace(/([A-Z])/g, ' $1').trim()
}

const formatDate = (dateString?: string) => {
  if (!dateString) return ''
  return new Date(dateString).toLocaleDateString() + ' ' + new Date(dateString).toLocaleTimeString()
}
</script>