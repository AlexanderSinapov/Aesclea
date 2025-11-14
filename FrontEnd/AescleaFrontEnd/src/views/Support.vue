<template>
  <div class="min-h-screen bg-gray-50 dark:bg-gray-900">
    <div class="px-4 py-8 mx-auto max-w-7xl sm:px-6 lg:px-8">
      <!-- Header -->
      <div class="mb-8">
        <h1 class="text-3xl font-bold text-gray-900 dark:text-white">Support Center</h1>
        <p class="mt-2 text-gray-600 dark:text-gray-400">Get help with your account, report issues, or ask questions</p>
      </div>

      <!-- Quick Actions -->
      <div class="grid grid-cols-1 gap-6 mb-8 md:grid-cols-3">
        <button
          @click="showCreateTicketModal = true"
          class="p-6 transition-colors bg-white border border-gray-200 rounded-lg shadow-sm dark:bg-gray-800 dark:border-gray-700 hover:border-blue-500 dark:hover:border-blue-400"
        >
          <div class="mb-3 text-blue-600 dark:text-blue-400">
            <svg class="w-8 h-8" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4v16m8-8H4"></path>
            </svg>
          </div>
          <h3 class="mb-2 text-lg font-semibold text-gray-900 dark:text-white">Create New Ticket</h3>
          <p class="text-gray-600 dark:text-gray-400">Report an issue or ask for help</p>
        </button>

        <div class="p-6 bg-white border border-gray-200 rounded-lg shadow-sm dark:bg-gray-800 dark:border-gray-700">
          <div class="mb-3 text-green-600 dark:text-green-400">
            <svg class="w-8 h-8" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z"></path>
            </svg>
          </div>
          <h3 class="mb-2 text-lg font-semibold text-gray-900 dark:text-white">Knowledge Base</h3>
          <p class="text-gray-600 dark:text-gray-400">Find answers to common questions</p>
        </div>

        <div class="p-6 bg-white border border-gray-200 rounded-lg shadow-sm dark:bg-gray-800 dark:border-gray-700">
          <div class="mb-3 text-purple-600 dark:text-purple-400">
            <svg class="w-8 h-8" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 12h.01M12 12h.01M16 12h.01M21 12c0 4.418-4.03 8-9 8a9.863 9.863 0 01-4.255-.949L3 20l1.395-3.72C3.512 15.042 3 13.574 3 12c0-4.418 4.03-8 9-8s9 3.582 9 8z"></path>
            </svg>
          </div>
          <h3 class="mb-2 text-lg font-semibold text-gray-900 dark:text-white">Live Chat</h3>
          <p class="text-gray-600 dark:text-gray-400">Chat with our support team</p>
        </div>
      </div>

      <!-- My Tickets -->
      <div class="bg-white border border-gray-200 rounded-lg shadow-sm dark:bg-gray-800 dark:border-gray-700">
        <div class="p-6 border-b border-gray-200 dark:border-gray-700">
          <div class="flex items-center justify-between">
            <h2 class="text-xl font-semibold text-gray-900 dark:text-white">My Support Tickets</h2>
            <div class="flex items-center space-x-4">
              <select
                v-model="statusFilter"
                @change="loadTickets"
                class="px-3 py-2 text-gray-900 bg-white border border-gray-300 rounded-md dark:border-gray-600 dark:bg-gray-700 dark:text-white"
              >
                <option value="">All Status</option>
                <option value="Open">Open</option>
                <option value="InProgress">In Progress</option>
                <option value="Waiting">Waiting</option>
                <option value="Resolved">Resolved</option>
                <option value="Closed">Closed</option>
              </select>
              <button
                @click="loadTickets"
                class="px-4 py-2 text-blue-600 transition-colors rounded-md dark:text-blue-400 hover:bg-blue-50 dark:hover:bg-blue-900/20"
              >
                Refresh
              </button>
            </div>
          </div>
        </div>

        <!-- Tickets List -->
        <div v-if="loading" class="p-8 text-center">
          <div class="w-8 h-8 mx-auto border-b-2 border-blue-600 rounded-full animate-spin"></div>
          <p class="mt-2 text-gray-600 dark:text-gray-400">Loading tickets...</p>
        </div>

        <div v-else-if="tickets.length === 0" class="p-8 text-center">
          <div class="mb-4 text-gray-400">
            <svg class="w-12 h-12 mx-auto" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M20 13V6a2 2 0 00-2-2H6a2 2 0 00-2 2v7m16 0v5a2 2 0 01-2 2H6a2 2 0 01-2-2v-5m16 0h-2M4 13h2m13-8V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v1M7 8h10"></path>
            </svg>
          </div>
          <h3 class="mb-2 text-lg font-semibold text-gray-900 dark:text-white">No support tickets</h3>
          <p class="mb-4 text-gray-600 dark:text-gray-400">You haven't created any support tickets yet.</p>
          <button
            @click="showCreateTicketModal = true"
            class="px-4 py-2 text-white transition-colors bg-blue-600 rounded-md hover:bg-blue-700"
          >
            Create Your First Ticket
          </button>
        </div>

        <div v-else class="divide-y divide-gray-200 dark:divide-gray-700">
          <div
            v-for="ticket in tickets"
            :key="ticket.id"
            @click="openTicket(ticket)"
            class="p-6 transition-colors cursor-pointer hover:bg-gray-50 dark:hover:bg-gray-700/50"
          >
            <div class="flex items-start justify-between">
              <div class="flex-1">
                <div class="flex items-center mb-2 space-x-3">
                  <h3 class="text-lg font-medium text-gray-900 dark:text-white">{{ ticket.subject }}</h3>
                  <span
                    :class="getStatusBadgeClass(ticket.status)"
                    class="px-2 py-1 text-xs font-medium rounded-full"
                  >
                    {{ formatStatus(ticket.status) }}
                  </span>
                  <span
                    :class="getPriorityBadgeClass(ticket.priority)"
                    class="px-2 py-1 text-xs font-medium rounded-full"
                  >
                    {{ ticket.priority }}
                  </span>
                </div>
                <p class="mb-3 text-gray-600 dark:text-gray-400 line-clamp-2">{{ ticket.description }}</p>
                <div class="flex items-center space-x-4 text-sm text-gray-500 dark:text-gray-400">
                  <span>Created {{ formatDate(ticket.createdAt) }}</span>
                  <span>•</span>
                  <span>{{ ticket.category }}</span>
                  <span v-if="ticket.assignedAgent">•</span>
                  <span v-if="ticket.assignedAgent">Assigned to {{ ticket.assignedAgent.firstName }} {{ ticket.assignedAgent.lastName }}</span>
                </div>
              </div>
              <div class="ml-4">
                <svg class="w-5 h-5 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5l7 7-7 7"></path>
                </svg>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Create Ticket Modal -->
    <CreateTicketModal
      :show="showCreateTicketModal"
      @close="showCreateTicketModal = false"
      @created="onTicketCreated"
    />

    <!-- Ticket Detail Modal -->
    <TicketDetailModal
      :show="showTicketDetailModal"
      :ticket="selectedTicket"
      @close="closeTicketDetail"
      @updated="onTicketUpdated"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useSupportStore, type SupportTicket } from '../stores/support'
import CreateTicketModal from '../components/support/CreateTicketModal.vue'
import TicketDetailModal from '../components/support/TicketDetailModal.vue'

const supportStore = useSupportStore()

const showCreateTicketModal = ref(false)
const showTicketDetailModal = ref(false)
const selectedTicket = ref<SupportTicket | null>(null)
const statusFilter = ref('')
const loading = ref(false)

const tickets = ref<SupportTicket[]>([])

onMounted(() => {
  loadTickets()
})

const loadTickets = async () => {
  loading.value = true
  try {
    await supportStore.loadUserTickets()
    tickets.value = supportStore.userTickets
  } catch (error) {
    console.error('Error loading tickets:', error)
  } finally {
    loading.value = false
  }
}

const openTicket = async (ticket: SupportTicket) => {
  selectedTicket.value = ticket
  showTicketDetailModal.value = true
}

const closeTicketDetail = () => {
  showTicketDetailModal.value = false
  selectedTicket.value = null
}

const onTicketCreated = (ticket: SupportTicket) => {
  showCreateTicketModal.value = false
  tickets.value.unshift(ticket)
}

const onTicketUpdated = (updatedTicket: SupportTicket) => {
  const index = tickets.value.findIndex((t: SupportTicket) => t.id === updatedTicket.id)
  if (index !== -1) {
    tickets.value[index] = updatedTicket
  }
}

const getStatusBadgeClass = (status: string) => {
  const classes: Record<string, string> = {
    'Open': 'bg-blue-100 text-blue-800 dark:bg-blue-900/20 dark:text-blue-300',
    'InProgress': 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900/20 dark:text-yellow-300',
    'Waiting': 'bg-orange-100 text-orange-800 dark:bg-orange-900/20 dark:text-orange-300',
    'Resolved': 'bg-green-100 text-green-800 dark:bg-green-900/20 dark:text-green-300',
    'Closed': 'bg-gray-100 text-gray-800 dark:bg-gray-700 dark:text-gray-300'
  }
  return classes[status] || 'bg-gray-100 text-gray-800 dark:bg-gray-700 dark:text-gray-300'
}

const getPriorityBadgeClass = (priority: string) => {
  const classes: Record<string, string> = {
    'Low': 'bg-green-100 text-green-800 dark:bg-green-900/20 dark:text-green-300',
    'Medium': 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900/20 dark:text-yellow-300',
    'High': 'bg-orange-100 text-orange-800 dark:bg-orange-900/20 dark:text-orange-300',
    'Critical': 'bg-red-100 text-red-800 dark:bg-red-900/20 dark:text-red-300'
  }
  return classes[priority] || 'bg-gray-100 text-gray-800 dark:bg-gray-700 dark:text-gray-300'
}

const formatStatus = (status: string) => {
  const statusStr = typeof status === 'string' ? status : String(status)
  return statusStr.replace(/([A-Z])/g, ' $1').trim()
}

const formatDate = (dateString: string) => {
  return new Date(dateString).toLocaleDateString() + ' ' + new Date(dateString).toLocaleTimeString()
}
</script>

<style scoped>
.line-clamp-2 {
  display: -webkit-box;
  line-clamp: 2;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}
</style>