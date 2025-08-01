<template>
  <div class="space-y-6">
    <!-- Page Header -->
    <div class="sm:flex sm:items-center sm:justify-between">
      <div>
        <h1 class="text-2xl font-bold text-gray-900 dark:text-white">Admin Panel</h1>
        <p class="mt-1 text-sm text-gray-500 dark:text-gray-400">
          System administration and management tools
        </p>
      </div>
      <div class="mt-4 sm:mt-0 flex space-x-3">
        <button
          @click="showCreateUserModal = true"
          class="inline-flex items-center px-4 py-2 border border-transparent text-sm font-medium rounded-md shadow-sm text-white bg-purple-600 hover:bg-purple-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-purple-500"
        >
          <PlusIcon class="w-4 h-4 mr-2" />
          Add User
        </button>
      </div>
    </div>

    <!-- Admin Navigation Tabs -->
    <div class="border-b border-gray-200 dark:border-gray-700">
      <nav class="-mb-px flex space-x-8">
        <button
          v-for="tab in adminTabs"
          :key="tab.id"
          @click="activeTab = tab.id"
          :class="activeTab === tab.id 
            ? 'border-purple-500 text-purple-600 dark:text-purple-400' 
            : 'border-transparent text-gray-500 dark:text-gray-400 hover:text-gray-700 dark:hover:text-gray-300 hover:border-gray-300'"
          class="whitespace-nowrap py-2 px-1 border-b-2 font-medium text-sm"
        >
          {{ tab.name }}
        </button>
      </nav>
    </div>

    <!-- System Overview -->
    <div v-if="activeTab === 'overview'" class="space-y-6">
      <!-- System Stats -->
      <div class="grid grid-cols-1 md:grid-cols-4 gap-6">
        <div class="bg-white dark:bg-gray-800 overflow-hidden shadow-sm rounded-lg border border-gray-200 dark:border-gray-700">
          <div class="p-5">
            <div class="flex items-center">
              <div class="flex-shrink-0">
                <UsersIcon class="h-6 w-6 text-blue-400" />
              </div>
              <div class="ml-5 w-0 flex-1">
                <dl>
                  <dt class="text-sm font-medium text-gray-500 dark:text-gray-400 truncate">Total Users</dt>
                  <dd class="text-lg font-medium text-gray-900 dark:text-white">{{ adminStore.systemStats?.totalUsers || 0 }}</dd>
                </dl>
              </div>
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-gray-800 overflow-hidden shadow-sm rounded-lg border border-gray-200 dark:border-gray-700">
          <div class="p-5">
            <div class="flex items-center">
              <div class="flex-shrink-0">
                <UserGroupIcon class="h-6 w-6 text-green-400" />
              </div>
              <div class="ml-5 w-0 flex-1">
                <dl>
                  <dt class="text-sm font-medium text-gray-500 dark:text-gray-400 truncate">Active Sessions</dt>
                  <dd class="text-lg font-medium text-gray-900 dark:text-white">{{ adminStore.systemStats?.activeSessions || 0 }}</dd>
                </dl>
              </div>
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-gray-800 overflow-hidden shadow-sm rounded-lg border border-gray-200 dark:border-gray-700">
          <div class="p-5">
            <div class="flex items-center">
              <div class="flex-shrink-0">
                <ServerIcon class="h-6 w-6 text-yellow-400" />
              </div>
              <div class="ml-5 w-0 flex-1">
                <dl>
                  <dt class="text-sm font-medium text-gray-500 dark:text-gray-400 truncate">System Load</dt>
                  <dd class="text-lg font-medium text-gray-900 dark:text-white">{{ adminStore.systemStats?.systemLoad || 0 }}%</dd>
                </dl>
              </div>
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-gray-800 overflow-hidden shadow-sm rounded-lg border border-gray-200 dark:border-gray-700">
          <div class="p-5">
            <div class="flex items-center">
              <div class="flex-shrink-0">
                <CircleStackIcon class="h-6 w-6 text-purple-400" />
              </div>
              <div class="ml-5 w-0 flex-1">
                <dl>
                  <dt class="text-sm font-medium text-gray-500 dark:text-gray-400 truncate">Database Size</dt>
                  <dd class="text-lg font-medium text-gray-900 dark:text-white">{{ adminStore.systemStats?.databaseSize || 0 }}GB</dd>
                </dl>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Quick Actions -->
      <div class="bg-white dark:bg-gray-800 shadow-sm rounded-lg border border-gray-200 dark:border-gray-700">
        <div class="px-6 py-4 border-b border-gray-200 dark:border-gray-700">
          <h3 class="text-lg font-medium text-gray-900 dark:text-white">Quick Actions</h3>
        </div>
        <div class="p-6">
          <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
            <button
              @click="runSystemBackup"
              class="flex items-center justify-center px-4 py-3 border border-gray-300 dark:border-gray-600 rounded-md text-sm font-medium text-gray-700 dark:text-gray-300 bg-white dark:bg-gray-700 hover:bg-gray-50 dark:hover:bg-gray-600 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-purple-500"
            >
              <CircleStackIcon class="w-5 h-5 mr-2" />
              Run Backup
            </button>
            
            <button
              @click="clearSystemCache"
              class="flex items-center justify-center px-4 py-3 border border-gray-300 dark:border-gray-600 rounded-md text-sm font-medium text-gray-700 dark:text-gray-300 bg-white dark:bg-gray-700 hover:bg-gray-50 dark:hover:bg-gray-600 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-purple-500"
            >
              <TrashIcon class="w-5 h-5 mr-2" />
              Clear Cache
            </button>
            
            <button
              @click="generateReport"
              class="flex items-center justify-center px-4 py-3 border border-gray-300 dark:border-gray-600 rounded-md text-sm font-medium text-gray-700 dark:text-gray-300 bg-white dark:bg-gray-700 hover:bg-gray-50 dark:hover:bg-gray-600 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-purple-500"
            >
              <DocumentTextIcon class="w-5 h-5 mr-2" />
              Generate Report
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- User Management -->
    <div v-else-if="activeTab === 'users'" class="space-y-6">
      <!-- Users Table -->
      <div class="bg-white dark:bg-gray-800 shadow-sm rounded-lg border border-gray-200 dark:border-gray-700">
        <div class="px-6 py-4 border-b border-gray-200 dark:border-gray-700">
          <div class="flex justify-between items-center">
            <h3 class="text-lg font-medium text-gray-900 dark:text-white">Users ({{ adminStore.adminUsers.length }})</h3>
            <div class="flex items-center space-x-4">
              <input
                v-model="userSearchQuery"
                type="text"
                placeholder="Search users..."
                class="block px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md bg-white dark:bg-gray-700 text-gray-900 dark:text-white focus:outline-none focus:ring-1 focus:ring-purple-500 focus:border-purple-500 sm:text-sm"
              />
              <select
                v-model="userRoleFilter"
                class="block px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md bg-white dark:bg-gray-700 text-gray-900 dark:text-white focus:outline-none focus:ring-1 focus:ring-purple-500 focus:border-purple-500 sm:text-sm"
              >
                <option value="">All Roles</option>
                <option value="admin">Admin</option>
                <option value="doctor">Doctor</option>
                <option value="nurse">Nurse</option>
                <option value="technician">Technician</option>
              </select>
            </div>
          </div>
        </div>
        
        <div class="overflow-x-auto">
          <table class="min-w-full divide-y divide-gray-200 dark:divide-gray-700">
            <thead class="bg-gray-50 dark:bg-gray-900">
              <tr>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase tracking-wider">User</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase tracking-wider">Role</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase tracking-wider">Department</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase tracking-wider">Status</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase tracking-wider">Last Login</th>
                <th class="relative px-6 py-3"><span class="sr-only">Actions</span></th>
              </tr>
            </thead>
            <tbody class="bg-white dark:bg-gray-800 divide-y divide-gray-200 dark:divide-gray-700">
              <tr v-for="user in filteredUsers" :key="user.id" class="hover:bg-gray-50 dark:hover:bg-gray-700">
                <td class="px-6 py-4 whitespace-nowrap">
                  <div class="flex items-center">
                    <div class="h-10 w-10 flex-shrink-0">
                      <div class="h-10 w-10 rounded-full bg-purple-100 dark:bg-purple-900 flex items-center justify-center">
                        <span class="text-sm font-medium text-purple-600 dark:text-purple-300">
                          {{ user.firstName[0] }}{{ user.lastName[0] }}
                        </span>
                      </div>
                    </div>
                    <div class="ml-4">
                      <div class="text-sm font-medium text-gray-900 dark:text-white">
                        {{ user.firstName }} {{ user.lastName }}
                      </div>
                      <div class="text-sm text-gray-500 dark:text-gray-400">{{ user.email }}</div>
                    </div>
                  </div>
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900 dark:text-white capitalize">{{ user.role }}</td>
                <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900 dark:text-white capitalize">{{ user.department }}</td>
                <td class="px-6 py-4 whitespace-nowrap">
                  <span 
                    class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium"
                    :class="user.isActive 
                      ? 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-200' 
                      : 'bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-200'"
                  >
                    {{ user.isActive ? 'Active' : 'Inactive' }}
                  </span>
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900 dark:text-white">
                  {{ user.lastLogin ? formatDate(user.lastLogin) : 'Never' }}
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                  <div class="flex items-center space-x-2">
                    <button
                      @click="editUser(user)"
                      class="text-purple-600 hover:text-purple-900 dark:text-purple-400 dark:hover:text-purple-300"
                    >
                      Edit
                    </button>
                    <button
                      @click="toggleUserStatus(user)"
                      :class="user.isActive ? 'text-red-600 hover:text-red-900 dark:text-red-400 dark:hover:text-red-300' : 'text-green-600 hover:text-green-900 dark:text-green-400 dark:hover:text-green-300'"
                    >
                      {{ user.isActive ? 'Deactivate' : 'Activate' }}
                    </button>
                  </div>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>

    <!-- System Logs -->
    <div v-else-if="activeTab === 'logs'" class="space-y-6">
      <div class="bg-white dark:bg-gray-800 shadow-sm rounded-lg border border-gray-200 dark:border-gray-700">
        <div class="px-6 py-4 border-b border-gray-200 dark:border-gray-700">
          <div class="flex justify-between items-center">
            <h3 class="text-lg font-medium text-gray-900 dark:text-white">System Audit Logs</h3>
            <div class="flex items-center space-x-4">
              <select
                v-model="logLevelFilter"
                class="block px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md bg-white dark:bg-gray-700 text-gray-900 dark:text-white focus:outline-none focus:ring-1 focus:ring-purple-500 focus:border-purple-500 sm:text-sm"
              >
                <option value="">All Levels</option>
                <option value="info">Info</option>
                <option value="warning">Warning</option>
                <option value="error">Error</option>
                <option value="critical">Critical</option>
              </select>
              <button
                @click="refreshLogs"
                class="inline-flex items-center px-3 py-2 border border-gray-300 dark:border-gray-600 text-sm font-medium rounded-md text-gray-700 dark:text-gray-300 bg-white dark:bg-gray-700 hover:bg-gray-50 dark:hover:bg-gray-600 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-purple-500"
              >
                <ArrowPathIcon class="w-4 h-4 mr-1" />
                Refresh
              </button>
            </div>
          </div>
        </div>
        
        <div class="max-h-96 overflow-y-auto">
          <div class="divide-y divide-gray-200 dark:divide-gray-700">
            <div v-for="log in filteredLogs" :key="log.id" class="px-6 py-4">
              <div class="flex items-start justify-between">
                <div class="flex-1">
                  <div class="flex items-center space-x-2">
                    <span 
                      class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium"
                      :class="getLogLevelClass(log.level)"
                    >
                      {{ log.level }}
                    </span>
                    <span class="text-sm text-gray-500 dark:text-gray-400">{{ formatDateTime(log.timestamp) }}</span>
                  </div>
                  <div class="mt-1 text-sm font-medium text-gray-900 dark:text-white">{{ log.message }}</div>
                  <div v-if="log.details" class="mt-1 text-sm text-gray-600 dark:text-gray-400">{{ log.details }}</div>
                  <div class="mt-1 text-xs text-gray-500 dark:text-gray-400">User: {{ log.userId || 'System' }}</div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- User Creation Modal -->
    <UserCreateModal
      v-if="showCreateUserModal"
      @close="showCreateUserModal = false"
      @save="handleCreateUser"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useAdminStore, type AdminUser } from '../../stores/admin'
import UserCreateModal from './UserCreateModal.vue'

// Store
const adminStore = useAdminStore()

// Local state
const activeTab = ref('overview')
const showCreateUserModal = ref(false)
const userSearchQuery = ref('')
const userRoleFilter = ref('')
const logLevelFilter = ref('')

// Admin tabs
const adminTabs = [
  { id: 'overview', name: 'Overview' },
  { id: 'users', name: 'Users' },
  { id: 'logs', name: 'System Logs' }
]

// Computed properties
const filteredUsers = computed(() => {
  let users = adminStore.adminUsers

  if (userSearchQuery.value) {
    const query = userSearchQuery.value.toLowerCase()
    users = users.filter(user => 
      user.firstName.toLowerCase().includes(query) ||
      user.lastName.toLowerCase().includes(query) ||
      user.email.toLowerCase().includes(query)
    )
  }

  if (userRoleFilter.value) {
    users = users.filter(user => user.role === userRoleFilter.value)
  }

  return users
})

const filteredLogs = computed(() => {
  let logs = adminStore.auditLogs

  if (logLevelFilter.value) {
    logs = logs.filter(log => log.level === logLevelFilter.value)
  }

  return logs.slice(0, 100) // Show only latest 100 logs
})

// Icon components
const PlusIcon = {
  template: `<svg fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" d="M12 4.5v15m7.5-7.5h-15" /></svg>`
}

const UsersIcon = {
  template: `<svg fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" d="M15 19.128a9.38 9.38 0 002.625.372 9.337 9.337 0 004.121-.952 4.125 4.125 0 00-7.533-2.493M15 19.128v-.003c0-1.113-.285-2.16-.786-3.07M15 19.128v.106A12.318 12.318 0 018.624 21c-2.331 0-4.512-.645-6.374-1.766l-.001-.109a6.375 6.375 0 0111.964-3.07M12 6.375a3.375 3.375 0 11-6.75 0 3.375 3.375 0 016.75 0zm8.25 2.25a2.625 2.625 0 11-5.25 0 2.625 2.625 0 015.25 0z" /></svg>`
}

const UserGroupIcon = {
  template: `<svg fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" d="M18 18.72a9.094 9.094 0 003.741-.479 3 3 0 00-4.682-2.72m.94 3.198l.001.031c0 .225-.012.447-.037.666A11.944 11.944 0 0112 21c-2.17 0-4.207-.576-5.963-1.584A6.062 6.062 0 016 18.719m12 0a5.971 5.971 0 00-.941-3.197m0 0A5.995 5.995 0 0012 12.75a5.995 5.995 0 00-5.058 2.772m0 0a3 3 0 00-4.681 2.72 8.986 8.986 0 003.74.477m.94-3.197a5.971 5.971 0 00-.94 3.197M15 6.75a3 3 0 11-6 0 3 3 0 016 0zm6 3a2.25 2.25 0 11-4.5 0 2.25 2.25 0 014.5 0zm-13.5 0a2.25 2.25 0 11-4.5 0 2.25 2.25 0 014.5 0z" /></svg>`
}

const ServerIcon = {
  template: `<svg fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" d="M21.75 17.25v-.228a4.5 4.5 0 00-.12-1.03l-2.268-9.64a3.375 3.375 0 00-3.285-2.602H7.923a3.375 3.375 0 00-3.285 2.602l-2.268 9.64a4.5 4.5 0 00-.12 1.03v.228m19.5 0a3 3 0 01-3 3H5.25a3 3 0 01-3-3m19.5 0a3 3 0 00-3-3H5.25a3 3 0 00-3 3m16.5 0h.008v.008h-.008V21M7.5 21h.008v.008H7.5V21zm9 0h.008v.008H16.5V21z" /></svg>`
}

const CircleStackIcon = {
  template: `<svg fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" d="M20.25 6.375c0 2.278-3.694 4.125-8.25 4.125S3.75 8.653 3.75 6.375m16.5 0c0-2.278-3.694-4.125-8.25-4.125S3.75 4.097 3.75 6.375m16.5 0v11.25c0 2.278-3.694 4.125-8.25 4.125s-8.25-1.847-8.25-4.125V6.375m16.5 0v3.75m-16.5-3.75v3.75m16.5 0v3.75C20.25 16.153 16.556 18 12 18s-8.25-1.847-8.25-4.125v-3.75m16.5-3.75c0 2.278-3.694 4.125-8.25 4.125s-8.25-1.847-8.25-4.125" /></svg>`
}

const TrashIcon = {
  template: `<svg fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" d="M14.74 9l-.346 9m-4.788 0L9.26 9m9.968-3.21c.342.052.682.107 1.022.166m-1.022-.165L18.16 19.673a2.25 2.25 0 01-2.244 2.077H8.084a2.25 2.25 0 01-2.244-2.077L4.772 5.79m14.456 0a48.108 48.108 0 00-3.478-.397m-12 .562c.34-.059.68-.114 1.022-.165m0 0a48.11 48.11 0 013.478-.397m7.5 0v-.916c0-1.18-.91-2.164-2.09-2.201a51.964 51.964 0 00-3.32 0c-1.18.037-2.09 1.022-2.09 2.201v.916m7.5 0a48.667 48.667 0 00-7.5 0" /></svg>`
}

const DocumentTextIcon = {
  template: `<svg fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" d="M19.5 14.25v-2.625a3.375 3.375 0 00-3.375-3.375h-4.5A1.125 1.125 0 0010.5 9h-4.5a3.375 3.375 0 00-3.375 3.375v8.25a3.375 3.375 0 003.375 3.375h9a3.375 3.375 0 003.375-3.375V14.25zM9.75 9.75l-1.5-1.5m0 0l1.5-1.5m-1.5 1.5h12" /></svg>`
}

const ArrowPathIcon = {
  template: `<svg fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" d="M16.023 9.348h4.992v-.001M2.985 19.644v-4.992m0 0h4.992m-4.993 0l3.181 3.183a8.25 8.25 0 0013.803-3.7M4.031 9.865a8.25 8.25 0 0113.803-3.7l3.181 3.182m0-4.991v4.99" /></svg>`
}

// Methods
const formatDate = (date: string) => {
  return new Date(date).toLocaleDateString()
}

const formatDateTime = (date: string) => {
  return new Date(date).toLocaleString()
}

const getLogLevelClass = (level: string) => {
  const classes: Record<string, string> = {
    info: 'bg-blue-100 text-blue-800 dark:bg-blue-900 dark:text-blue-200',
    warning: 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900 dark:text-yellow-200',
    error: 'bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-200',
    critical: 'bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-200'
  }
  return classes[level] || classes.info
}

const editUser = (user: AdminUser) => {
  console.log('Editing user:', user.id)
  // Implement user editing logic
}

const toggleUserStatus = async (user: AdminUser) => {
  try {
    await adminStore.updateAdminUser(user.id, { isActive: !user.isActive })
    console.log(`User ${user.email} ${user.isActive ? 'deactivated' : 'activated'}`)
  } catch (error) {
    console.error('Error updating user status:', error)
    alert('Failed to update user status. Please try again.')
  }
}

const runSystemBackup = () => {
  console.log('Running system backup...')
  alert('System backup initiated. You will be notified when complete.')
}

const clearSystemCache = () => {
  console.log('Clearing system cache...')
  alert('System cache cleared successfully.')
}

const generateReport = () => {
  console.log('Generating system report...')
  alert('System report generation started. Check your email for the report.')
}

const refreshLogs = async () => {
  try {
    await adminStore.fetchAuditLogs()
    console.log('Logs refreshed')
  } catch (error) {
    console.error('Error refreshing logs:', error)
  }
}

const handleCreateUser = async (userData: Omit<AdminUser, 'id' | 'createdAt' | 'updatedAt'>) => {
  try {
    await adminStore.createAdminUser(userData)
    showCreateUserModal.value = false
  } catch (error) {
    console.error('Error creating user:', error)
    alert('Failed to create user. Please try again.')
  }
}

// Load initial data
onMounted(async () => {
  await Promise.all([
    adminStore.fetchSystemStats(),
    adminStore.fetchAdminUsers(),
    adminStore.fetchAuditLogs()
  ])
})
</script>
