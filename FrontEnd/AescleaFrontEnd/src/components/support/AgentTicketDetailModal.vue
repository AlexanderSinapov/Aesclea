<template>
  <div v-if="show" class="fixed inset-0 bg-black bg-opacity-30 flex items-center justify-center p-4 z-50">
    <div class="bg-white dark:bg-gray-800 rounded-lg shadow-xl w-full max-w-6xl max-h-[90vh] overflow-hidden flex flex-col">
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
        <!-- Ticket Management -->
        <div class="w-1/3 border-r border-gray-200 dark:border-gray-700 p-6 overflow-y-auto">
          <h3 class="text-lg font-medium text-gray-900 dark:text-white mb-4">Ticket Management</h3>
          
          <!-- Update Form -->
          <div class="space-y-4 mb-6">
            <div>
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Status</label>
              <select
                v-model="updateForm.status"
                class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md bg-white dark:bg-gray-700 text-gray-900 dark:text-white focus:ring-blue-500 focus:border-blue-500"
              >
                <option value="Open">Open</option>
                <option value="InProgress">In Progress</option>
                <option value="Waiting">Waiting for Customer</option>
                <option value="Resolved">Resolved</option>
                <option value="Closed">Closed</option>
              </select>
            </div>

            <div>
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Priority</label>
              <select
                v-model="updateForm.priority"
                class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md bg-white dark:bg-gray-700 text-gray-900 dark:text-white focus:ring-blue-500 focus:border-blue-500"
              >
                <option value="Low">Low</option>
                <option value="Medium">Medium</option>
                <option value="High">High</option>
                <option value="Critical">Critical</option>
              </select>
            </div>

            <div>
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Assign To</label>
              <select
                v-model="updateForm.assignedAgentId"
                class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md bg-white dark:bg-gray-700 text-gray-900 dark:text-white focus:ring-blue-500 focus:border-blue-500"
              >
                <option value="">Unassigned</option>
                <option
                  v-for="agent in agents"
                  :key="agent.id"
                  :value="agent.id"
                >
                  {{ agent.firstName }} {{ agent.lastName }}
                </option>
              </select>
            </div>

            <div v-if="updateForm.status === 'Resolved' || updateForm.status === 'Closed'">
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Resolution Note</label>
              <textarea
                v-model="updateForm.resolutionNote"
                rows="3"
                placeholder="Describe how this issue was resolved..."
                class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md bg-white dark:bg-gray-700 text-gray-900 dark:text-white focus:ring-blue-500 focus:border-blue-500 resize-none"
              ></textarea>
            </div>

            <button
              @click="updateTicket"
              :disabled="updating"
              class="w-full px-4 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700 disabled:opacity-50 disabled:cursor-not-allowed transition-colors flex items-center justify-center space-x-2"
            >
              <div v-if="updating" class="animate-spin rounded-full h-4 w-4 border-b-2 border-white"></div>
              <span>{{ updating ? 'Updating...' : 'Update Ticket' }}</span>
            </button>
          </div>

          <!-- Ticket Details -->
          <div class="border-t border-gray-200 dark:border-gray-700 pt-6">
            <h4 class="text-md font-medium text-gray-900 dark:text-white mb-4">Ticket Details</h4>
            
            <div class="space-y-3 text-sm">
              <div>
                <label class="block font-medium text-gray-700 dark:text-gray-300">ID</label>
                <p class="text-gray-900 dark:text-white font-mono">{{ ticket?.id }}</p>
              </div>

              <div>
                <label class="block font-medium text-gray-700 dark:text-gray-300">Customer</label>
                <p class="text-gray-900 dark:text-white">{{ ticket?.user?.firstName }} {{ ticket?.user?.lastName }}</p>
                <p class="text-gray-600 dark:text-gray-400">{{ ticket?.user?.email }}</p>
              </div>

              <div>
                <label class="block font-medium text-gray-700 dark:text-gray-300">Category</label>
                <p class="text-gray-900 dark:text-white">{{ ticket?.category }}</p>
              </div>

              <div>
                <label class="block font-medium text-gray-700 dark:text-gray-300">Created</label>
                <p class="text-gray-900 dark:text-white">{{ formatDate(ticket?.createdAt) }}</p>
              </div>

              <div>
                <label class="block font-medium text-gray-700 dark:text-gray-300">Last Updated</label>
                <p class="text-gray-900 dark:text-white">{{ formatDate(ticket?.updatedAt) }}</p>
              </div>

              <div v-if="ticket?.resolvedAt">
                <label class="block font-medium text-gray-700 dark:text-gray-300">Resolved</label>
                <p class="text-gray-900 dark:text-white">{{ formatDate(ticket.resolvedAt) }}</p>
              </div>
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
                    <span class="text-xs text-blue-600 dark:text-blue-400 font-medium">Customer</span>
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
                        : 'bg-blue-600 text-white'
                    ]"
                  >
                    {{ message.sender?.firstName?.charAt(0) }}{{ message.sender?.lastName?.charAt(0) }}
                  </div>
                </div>
                <div class="flex-1">
                  <div class="flex items-center space-x-2 mb-2">
                    <p class="text-sm font-medium text-gray-900 dark:text-white">
                      {{ message.sender?.firstName }} {{ message.sender?.lastName }}
                    </p>
                    <span
                      :class="[
                        'text-xs font-medium',
                        message.isFromAgent
                          ? 'text-green-600 dark:text-green-400'
                          : 'text-blue-600 dark:text-blue-400'
                      ]"
                    >
                      {{ message.isFromAgent ? 'Support Agent' : 'Customer' }}
                    </span>
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
            <form @submit.prevent="sendMessage" class="space-y-4">
              <div>
                <textarea
                  v-model="newMessage"
                  rows="4"
                  placeholder="Type your response to the customer..."
                  class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md bg-white dark:bg-gray-700 text-gray-900 dark:text-white focus:ring-blue-500 focus:border-blue-500 resize-none"
                  :disabled="sendingMessage || ticket?.status === 'Closed'"
                ></textarea>
              </div>
              
              <div class="flex items-center justify-between">
                <p v-if="ticket?.status === 'Closed'" class="text-sm text-gray-500 dark:text-gray-400">
                  This ticket is closed. Change status to continue the conversation.
                </p>
                <div v-else class="flex items-center space-x-3">
                  <label class="flex items-center space-x-2">
                    <input
                      v-model="markAsWaiting"
                      type="checkbox"
                      class="rounded border-gray-300 text-blue-600 focus:ring-blue-500"
                    />
                    <span class="text-sm text-gray-700 dark:text-gray-300">Mark as waiting for customer</span>
                  </label>
                </div>
                
                <button
                  type="submit"
                  :disabled="!newMessage.trim() || sendingMessage || ticket?.status === 'Closed'"
                  class="px-6 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700 disabled:opacity-50 disabled:cursor-not-allowed transition-colors flex items-center space-x-2"
                >
                  <div v-if="sendingMessage" class="animate-spin rounded-full h-4 w-4 border-b-2 border-white"></div>
                  <span>{{ sendingMessage ? 'Sending...' : 'Send Response' }}</span>
                </button>
              </div>
            </form>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue'

interface Props {
  show: boolean
  ticket: any
  agents: any[]
}

const props = defineProps<Props>()

const messages = ref<any[]>([])
const newMessage = ref('')
const markAsWaiting = ref(false)
const loadingMessages = ref(false)
const sendingMessage = ref(false)
const updating = ref(false)

const updateForm = ref({
  status: '',
  priority: '',
  assignedAgentId: '',
  resolutionNote: ''
})

// Initialize form when ticket changes
watch(() => props.ticket, (newTicket) => {
  if (newTicket) {
    updateForm.value = {
      status: newTicket.status || '',
      priority: newTicket.priority || '',
      assignedAgentId: newTicket.assignedAgentId || '',
      resolutionNote: newTicket.resolutionNote || ''
    }
    loadMessages()
  }
}, { immediate: true })

const loadMessages = async () => {
  if (!props.ticket) return
  
  loadingMessages.value = true
  try {
    // Mock implementation - replace with actual API call
    console.log('Loading messages for ticket:', props.ticket.id)
    // messages.value = await supportStore.loadTicketMessages(props.ticket.id)
  } catch (error) {
    console.error('Error loading messages:', error)
  } finally {
    loadingMessages.value = false
  }
}

const updateTicket = async () => {
  if (!props.ticket) return

  updating.value = true
  try {
    // Mock implementation - replace with actual API call
    console.log('Updating ticket:', props.ticket.id, updateForm.value)
    // await supportStore.updateTicket(props.ticket.id, updateForm.value)
    // $emit('updated')
  } catch (error) {
    console.error('Error updating ticket:', error)
  } finally {
    updating.value = false
  }
}

const sendMessage = async () => {
  if (!newMessage.value.trim() || !props.ticket) return

  sendingMessage.value = true
  try {
    // Mock implementation - replace with actual API call
    console.log('Sending message:', {
      ticketId: props.ticket.id,
      content: newMessage.value,
      markAsWaiting: markAsWaiting.value
    })
    
    // const message = await supportStore.sendMessage(props.ticket.id, newMessage.value, true)
    // messages.value.push(message)
    
    newMessage.value = ''
    markAsWaiting.value = false
    
    // If marked as waiting, update ticket status
    if (markAsWaiting.value) {
      updateForm.value.status = 'Waiting'
      await updateTicket()
    }
  } catch (error) {
    console.error('Error sending message:', error)
  } finally {
    sendingMessage.value = false
  }
}

const getStatusBadgeClass = (status?: string) => {
  const classes: Record<string, string> = {
    'Open': 'bg-blue-100 text-blue-800 dark:bg-blue-900/20 dark:text-blue-300',
    'InProgress': 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900/20 dark:text-yellow-300',
    'Waiting': 'bg-orange-100 text-orange-800 dark:bg-orange-900/20 dark:text-orange-300',
    'Resolved': 'bg-green-100 text-green-800 dark:bg-green-900/20 dark:text-green-300',
    'Closed': 'bg-gray-100 text-gray-800 dark:bg-gray-700 dark:text-gray-300'
  }
  return classes[status || ''] || 'bg-gray-100 text-gray-800 dark:bg-gray-700 dark:text-gray-300'
}

const getPriorityBadgeClass = (priority?: string) => {
  const classes: Record<string, string> = {
    'Low': 'bg-green-100 text-green-800 dark:bg-green-900/20 dark:text-green-300',
    'Medium': 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900/20 dark:text-yellow-300',
    'High': 'bg-orange-100 text-orange-800 dark:bg-orange-900/20 dark:text-orange-300',
    'Critical': 'bg-red-100 text-red-800 dark:bg-red-900/20 dark:text-red-300'
  }
  return classes[priority || ''] || 'bg-gray-100 text-gray-800 dark:bg-gray-700 dark:text-gray-300'
}

const formatStatus = (status?: string) => {
  return status?.replace(/([A-Z])/g, ' $1').trim()
}

const formatDate = (dateString?: string) => {
  if (!dateString) return ''
  return new Date(dateString).toLocaleDateString() + ' ' + new Date(dateString).toLocaleTimeString()
}
</script>