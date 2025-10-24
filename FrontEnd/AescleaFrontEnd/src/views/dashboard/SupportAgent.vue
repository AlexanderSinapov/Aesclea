<template>
  <div class="min-h-screen bg-gray-50 dark:bg-gray-900">
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <!-- Header -->
      <div class="mb-8">
        <h1 class="text-3xl font-bold text-gray-900 dark:text-white">Support Agent Dashboard</h1>
        <p class="mt-2 text-gray-600 dark:text-gray-400">Manage support tickets and help customers</p>
      </div>

      <!-- Stats Cards -->
      <div class="mb-8 grid grid-cols-1 md:grid-cols-5 gap-6">
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
          <div class="flex items-center">
            <div class="flex-shrink-0">
              <div class="w-8 h-8 bg-blue-100 dark:bg-blue-900/20 rounded-md flex items-center justify-center">
                <svg class="w-5 h-5 text-blue-600 dark:text-blue-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M7 8h10M7 12h4m1 8l-4-4H5a2 2 0 01-2-2V6a2 2 0 012-2h14a2 2 0 012 2v8a2 2 0 01-2 2h-3l-4 4z"></path>
                </svg>
              </div>
            </div>
            <div class="ml-4">
              <p class="text-sm font-medium text-gray-600 dark:text-gray-400">Total Tickets</p>
              <p class="text-2xl font-semibold text-gray-900 dark:text-white">{{ stats?.totalTickets || 0 }}</p>
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
          <div class="flex items-center">
            <div class="flex-shrink-0">
              <div class="w-8 h-8 bg-red-100 dark:bg-red-900/20 rounded-md flex items-center justify-center">
                <svg class="w-5 h-5 text-red-600 dark:text-red-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"></path>
                </svg>
              </div>
            </div>
            <div class="ml-4">
              <p class="text-sm font-medium text-gray-600 dark:text-gray-400">Open</p>
              <p class="text-2xl font-semibold text-gray-900 dark:text-white">{{ stats?.openTickets || 0 }}</p>
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
          <div class="flex items-center">
            <div class="flex-shrink-0">
              <div class="w-8 h-8 bg-yellow-100 dark:bg-yellow-900/20 rounded-md flex items-center justify-center">
                <svg class="w-5 h-5 text-yellow-600 dark:text-yellow-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z"></path>
                </svg>
              </div>
            </div>
            <div class="ml-4">
              <p class="text-sm font-medium text-gray-600 dark:text-gray-400">In Progress</p>
              <p class="text-2xl font-semibold text-gray-900 dark:text-white">{{ stats?.inProgressTickets || 0 }}</p>
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
          <div class="flex items-center">
            <div class="flex-shrink-0">
              <div class="w-8 h-8 bg-green-100 dark:bg-green-900/20 rounded-md flex items-center justify-center">
                <svg class="w-5 h-5 text-green-600 dark:text-green-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z"></path>
                </svg>
              </div>
            </div>
            <div class="ml-4">
              <p class="text-sm font-medium text-gray-600 dark:text-gray-400">Resolved</p>
              <p class="text-2xl font-semibold text-gray-900 dark:text-white">{{ stats?.resolvedTickets || 0 }}</p>
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
          <div class="flex items-center">
            <div class="flex-shrink-0">
              <div class="w-8 h-8 bg-purple-100 dark:bg-purple-900/20 rounded-md flex items-center justify-center">
                <svg class="w-5 h-5 text-purple-600 dark:text-purple-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z"></path>
                </svg>
              </div>
            </div>
            <div class="ml-4">
              <p class="text-sm font-medium text-gray-600 dark:text-gray-400">My Assigned</p>
              <p class="text-2xl font-semibold text-gray-900 dark:text-white">{{ stats?.myAssignedTickets || 0 }}</p>
            </div>
          </div>
        </div>
      </div>

      <!-- Filters and Actions -->
      <div class="mb-6 bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
        <div class="flex flex-wrap items-center justify-between gap-4">
          <div class="flex flex-wrap items-center space-x-4">
            <select
              v-model="statusFilter"
              @change="loadTickets"
              class="px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
            >
              <option value="">All Status</option>
              <option value="Open">Open</option>
              <option value="InProgress">In Progress</option>
              <option value="Waiting">Waiting</option>
              <option value="Resolved">Resolved</option>
              <option value="Closed">Closed</option>
            </select>

            <select
              v-model="assignedFilter"
              @change="loadTickets"
              class="px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
            >
              <option value="">All Agents</option>
              <option value="me">My Tickets</option>
              <option value="unassigned">Unassigned</option>
              <option
                v-for="agent in agents"
                :key="agent.id"
                :value="agent.id"
              >
                {{ agent.firstName }} {{ agent.lastName }}
              </option>
            </select>

            <button
              @click="loadTickets"
              class="px-4 py-2 text-blue-600 dark:text-blue-400 hover:bg-blue-50 dark:hover:bg-blue-900/20 rounded-md transition-colors"
            >
              Refresh
            </button>
          </div>

          <div class="flex items-center space-x-3">
            <span class="text-sm text-gray-600 dark:text-gray-400">
              {{ tickets.length }} tickets found
            </span>
          </div>
        </div>
      </div>

      <!-- Tickets Table -->
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 overflow-hidden">
        <div class="px-6 py-4 border-b border-gray-200 dark:border-gray-700">
          <h2 class="text-xl font-semibold text-gray-900 dark:text-white">Support Tickets</h2>
        </div>

        <!-- Loading -->
        <div v-if="loading" class="p-8 text-center">
          <div class="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600 mx-auto"></div>
          <p class="mt-2 text-gray-600 dark:text-gray-400">Loading tickets...</p>
        </div>

        <!-- No tickets -->
        <div v-else-if="tickets.length === 0" class="p-8 text-center">
          <div class="text-gray-400 mb-4">
            <svg class="w-12 h-12 mx-auto" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M20 13V6a2 2 0 00-2-2H6a2 2 0 00-2 2v7m16 0v5a2 2 0 01-2 2H6a2 2 0 01-2-2v-5m16 0h-2M4 13h2m13-8V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v1M7 8h10"></path>
            </svg>
          </div>
          <h3 class="text-lg font-semibold text-gray-900 dark:text-white mb-2">No tickets found</h3>
          <p class="text-gray-600 dark:text-gray-400">No tickets match your current filters.</p>
        </div>

        <!-- Tickets list -->
        <div v-else class="overflow-x-auto">
          <table class="min-w-full divide-y divide-gray-200 dark:divide-gray-700">
            <thead class="bg-gray-50 dark:bg-gray-700">
              <tr>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                  Ticket
                </th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                  Customer
                </th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                  Status
                </th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                  Priority
                </th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                  Assigned To
                </th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                  Created
                </th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                  Actions
                </th>
              </tr>
            </thead>
            <tbody class="bg-white dark:bg-gray-800 divide-y divide-gray-200 dark:divide-gray-700">
              <tr
                v-for="ticket in tickets"
                :key="ticket.id"
                class="hover:bg-gray-50 dark:hover:bg-gray-700/50 transition-colors"
              >
                <td class="px-6 py-4 whitespace-nowrap">
                  <div class="flex flex-col">
                    <button
                      @click="openTicket(ticket)"
                      class="text-sm font-medium text-blue-600 dark:text-blue-400 hover:text-blue-800 dark:hover:text-blue-300 text-left"
                    >
                      {{ ticket.subject }}
                    </button>
                    <p class="text-xs text-gray-500 dark:text-gray-400 mt-1">{{ ticket.category }}</p>
                  </div>
                </td>
                <td class="px-6 py-4 whitespace-nowrap">
                  <div class="flex flex-col">
                    <div class="text-sm font-medium text-gray-900 dark:text-white">
                      {{ ticket.user?.firstName }} {{ ticket.user?.lastName }}
                    </div>
                    <div class="text-sm text-gray-500 dark:text-gray-400">{{ ticket.user?.email }}</div>
                  </div>
                </td>
                <td class="px-6 py-4 whitespace-nowrap">
                  <span
                    :class="getStatusBadgeClass(ticket.status)"
                    class="px-2 inline-flex text-xs leading-5 font-semibold rounded-full"
                  >
                    {{ formatStatus(ticket.status) }}
                  </span>
                </td>
                <td class="px-6 py-4 whitespace-nowrap">
                  <span
                    :class="getPriorityBadgeClass(ticket.priority)"
                    class="px-2 inline-flex text-xs leading-5 font-semibold rounded-full"
                  >
                    {{ ticket.priority }}
                  </span>
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900 dark:text-white">
                  <div v-if="ticket.assignedAgent" class="flex items-center">
                    <div class="flex-shrink-0 h-6 w-6">
                      <div class="h-6 w-6 rounded-full bg-gray-300 dark:bg-gray-600 flex items-center justify-center">
                        <span class="text-xs font-medium text-gray-700 dark:text-gray-300">
                          {{ ticket.assignedAgent.firstName?.charAt(0) }}{{ ticket.assignedAgent.lastName?.charAt(0) }}
                        </span>
                      </div>
                    </div>
                    <div class="ml-2">
                      <div class="text-sm font-medium text-gray-900 dark:text-white">
                        {{ ticket.assignedAgent.firstName }} {{ ticket.assignedAgent.lastName }}
                      </div>
                    </div>
                  </div>
                  <span v-else class="text-gray-400 dark:text-gray-500 italic">Unassigned</span>
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500 dark:text-gray-400">
                  {{ formatDate(ticket.createdAt) }}
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-right text-sm font-medium space-x-2">
                  <button
                    @click="openTicket(ticket)"
                    class="text-blue-600 dark:text-blue-400 hover:text-blue-900 dark:hover:text-blue-300"
                  >
                    View
                  </button>
                  <button
                    v-if="!ticket.assignedAgent"
                    @click="assignToMe(ticket)"
                    class="text-green-600 dark:text-green-400 hover:text-green-900 dark:hover:text-green-300"
                  >
                    Take
                  </button>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>

    <!-- Agent Ticket Detail Modal -->
    <AgentTicketDetailModal
      :show="showTicketDetailModal"
      :ticket="selectedTicket"
      :agents="agents"
      @close="closeTicketDetail"
      @updated="onTicketUpdated"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useAuthStore } from '../../stores/auth'
import AgentTicketDetailModal from '../../components/support/AgentTicketDetailModal.vue'

// Mock data and functions - replace with actual store implementation
const authStore = useAuthStore()

const showTicketDetailModal = ref(false)
const selectedTicket = ref(null)
const statusFilter = ref('')
const assignedFilter = ref('')
const loading = ref(false)

const tickets = ref<any[]>([])
const agents = ref<any[]>([])
const stats = ref<any>(null)

onMounted(async () => {
  // Check if user has support agent role
  const user = authStore.user
  if (!user || (user.role !== 'supportagent' && user.role !== 'admin')) {
    // Redirect to dashboard or show error
    return
  }

  await loadData()
})

const loadData = async () => {
  await Promise.all([
    loadTickets(),
    loadAgents(),
    loadStats()
  ])
}

const loadTickets = async () => {
  loading.value = true
  try {
    // Mock implementation - replace with actual API call
    // const response = await supportStore.loadAllTickets(statusFilter.value, assignedFilter.value)
    // tickets.value = response
    console.log('Loading tickets with filters:', { status: statusFilter.value, assigned: assignedFilter.value })
  } catch (error) {
    console.error('Error loading tickets:', error)
  } finally {
    loading.value = false
  }
}

const loadAgents = async () => {
  try {
    // Mock implementation - replace with actual API call
    // agents.value = await supportStore.loadSupportAgents()
    console.log('Loading support agents')
  } catch (error) {
    console.error('Error loading agents:', error)
  }
}

const loadStats = async () => {
  try {
    // Mock implementation - replace with actual API call
    // stats.value = await supportStore.loadTicketStats()
    console.log('Loading ticket stats')
  } catch (error) {
    console.error('Error loading stats:', error)
  }
}

const openTicket = (ticket: any) => {
  selectedTicket.value = ticket
  showTicketDetailModal.value = true
}

const closeTicketDetail = () => {
  showTicketDetailModal.value = false
  selectedTicket.value = null
}

const assignToMe = async (ticket: any) => {
  try {
    // Mock implementation - replace with actual API call
    // await supportStore.updateTicket(ticket.id, { assignedAgentId: authStore.user.id })
    console.log('Assigning ticket to me:', ticket.id)
    await loadTickets()
  } catch (error) {
    console.error('Error assigning ticket:', error)
  }
}

const onTicketUpdated = () => {
  loadTickets()
  loadStats()
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
  return status.replace(/([A-Z])/g, ' $1').trim()
}

const formatDate = (dateString: string) => {
  return new Date(dateString).toLocaleDateString() + ' ' + new Date(dateString).toLocaleTimeString()
}
</script>