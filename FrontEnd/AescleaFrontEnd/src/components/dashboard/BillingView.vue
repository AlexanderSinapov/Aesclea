<template>
  <div class="space-y-6">
    <!-- Page Header -->
    <div class="sm:flex sm:items-center sm:justify-between">
      <div>
        <h1 class="text-2xl font-bold text-gray-900 dark:text-white">Billing & Subscriptions</h1>
        <p class="mt-1 text-sm text-gray-500 dark:text-gray-400">
          Manage your subscription, billing, payments, and financial reports
        </p>
      </div>
      <div class="mt-4 sm:mt-0 flex space-x-3">
        <button
          v-if="!subscriptionStore.hasActiveSubscription"
          @click="showSubscriptionPlans = true"
          class="inline-flex items-center px-4 py-2 border border-transparent text-sm font-medium rounded-md shadow-sm text-white bg-purple-600 hover:bg-purple-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-purple-500"
        >
          <CreditCardIcon class="w-4 h-4 mr-2" />
          Subscribe Now
        </button>
        <button
          @click="showCreateInvoiceModal = true"
          class="inline-flex items-center px-4 py-2 border border-transparent text-sm font-medium rounded-md shadow-sm text-white bg-purple-600 hover:bg-purple-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-purple-500"
        >
          <PlusIcon class="w-4 h-4 mr-2" />
          Create Invoice
        </button>
        <button
          @click="exportReports"
          class="inline-flex items-center px-4 py-2 border border-gray-300 dark:border-gray-600 text-sm font-medium rounded-md text-gray-700 dark:text-gray-300 bg-white dark:bg-gray-700 hover:bg-gray-50 dark:hover:bg-gray-600 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-gray-500"
        >
          <DocumentArrowDownIcon class="w-4 h-4 mr-2" />
          Export Reports
        </button>
      </div>
    </div>

    <!-- Subscription Management Section -->
    <div v-if="subscriptionStore.hasActiveSubscription" class="bg-white dark:bg-gray-800 shadow-sm rounded-lg border border-gray-200 dark:border-gray-700">
      <div class="px-6 py-4 border-b border-gray-200 dark:border-gray-700">
        <h3 class="text-lg font-medium text-gray-900 dark:text-white">Current Subscription</h3>
      </div>
      <div class="p-6">
        <div class="flex items-center justify-between">
          <div>
            <h4 class="text-xl font-semibold text-gray-900 dark:text-white">
              {{ subscriptionStore.currentPlan?.name }} Plan
            </h4>
            <p class="text-sm text-gray-500 dark:text-gray-400">
              ${{ subscriptionStore.currentPlan?.price }}/{{ subscriptionStore.currentPlan?.interval }}
            </p>
            <p class="text-sm text-gray-500 dark:text-gray-400 mt-1">
              Next billing: {{ formatDate(subscriptionStore.userSubscription?.currentPeriodEnd) }}
            </p>
            <span 
              v-if="subscriptionStore.userSubscription?.cancelAtPeriodEnd"
              class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-300 mt-2"
            >
              Cancelling at period end
            </span>
          </div>
          <div class="flex space-x-3">
            <button
              v-if="!subscriptionStore.userSubscription?.cancelAtPeriodEnd"
              @click="showChangePlan = true"
              class="inline-flex items-center px-3 py-2 border border-gray-300 dark:border-gray-600 text-sm font-medium rounded-md text-gray-700 dark:text-gray-300 bg-white dark:bg-gray-700 hover:bg-gray-50 dark:hover:bg-gray-600"
            >
              Change Plan
            </button>
            <button
              v-if="!subscriptionStore.userSubscription?.cancelAtPeriodEnd"
              @click="cancelSubscription"
              class="inline-flex items-center px-3 py-2 border border-red-300 text-sm font-medium rounded-md text-red-700 bg-white hover:bg-red-50"
            >
              Cancel Subscription
            </button>
            <button
              v-else
              @click="reactivateSubscription"
              class="inline-flex items-center px-3 py-2 border border-green-300 text-sm font-medium rounded-md text-green-700 bg-white hover:bg-green-50"
            >
              Reactivate
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- No Subscription State -->
    <div v-else class="bg-white dark:bg-gray-800 shadow-sm rounded-lg border border-gray-200 dark:border-gray-700">
      <div class="p-6 text-center">
        <CreditCardIcon class="mx-auto h-12 w-12 text-gray-400" />
        <h3 class="mt-2 text-sm font-medium text-gray-900 dark:text-white">No Active Subscription</h3>
        <p class="mt-1 text-sm text-gray-500 dark:text-gray-400">
          Subscribe to unlock all features and get unlimited access to AI analysis.
        </p>
        <div class="mt-6">
          <button
            @click="showSubscriptionPlans = true"
            class="inline-flex items-center px-4 py-2 border border-transparent shadow-sm text-sm font-medium rounded-md text-white bg-purple-600 hover:bg-purple-700"
          >
            View Plans
          </button>
        </div>
      </div>
    </div>

    <!-- Financial Overview Cards -->
    <div class="grid grid-cols-1 md:grid-cols-4 gap-6">
      <div class="bg-white dark:bg-gray-800 overflow-hidden shadow-sm rounded-lg border border-gray-200 dark:border-gray-700">
        <div class="p-5">
          <div class="flex items-center">
            <div class="flex-shrink-0">
              <CurrencyDollarIcon class="h-6 w-6 text-green-400" />
            </div>
            <div class="ml-5 w-0 flex-1">
              <dl>
                <dt class="text-sm font-medium text-gray-500 dark:text-gray-400 truncate">Monthly Revenue</dt>
                <dd class="text-lg font-medium text-gray-900 dark:text-white">${{ monthlyRevenue.toLocaleString() }}</dd>
              </dl>
            </div>
          </div>
        </div>
      </div>

      <div class="bg-white dark:bg-gray-800 overflow-hidden shadow-sm rounded-lg border border-gray-200 dark:border-gray-700">
        <div class="p-5">
          <div class="flex items-center">
            <div class="flex-shrink-0">
              <DocumentTextIcon class="h-6 w-6 text-blue-400" />
            </div>
            <div class="ml-5 w-0 flex-1">
              <dl>
                <dt class="text-sm font-medium text-gray-500 dark:text-gray-400 truncate">Pending Invoices</dt>
                <dd class="text-lg font-medium text-gray-900 dark:text-white">{{ pendingInvoices.length }}</dd>
              </dl>
            </div>
          </div>
        </div>
      </div>

      <div class="bg-white dark:bg-gray-800 overflow-hidden shadow-sm rounded-lg border border-gray-200 dark:border-gray-700">
        <div class="p-5">
          <div class="flex items-center">
            <div class="flex-shrink-0">
              <ClockIcon class="h-6 w-6 text-yellow-400" />
            </div>
            <div class="ml-5 w-0 flex-1">
              <dl>
                <dt class="text-sm font-medium text-gray-500 dark:text-gray-400 truncate">Overdue Payments</dt>
                <dd class="text-lg font-medium text-gray-900 dark:text-white">${{ overdueAmount.toLocaleString() }}</dd>
              </dl>
            </div>
          </div>
        </div>
      </div>

      <div class="bg-white dark:bg-gray-800 overflow-hidden shadow-sm rounded-lg border border-gray-200 dark:border-gray-700">
        <div class="p-5">
          <div class="flex items-center">
            <div class="flex-shrink-0">
              <CreditCardIcon class="h-6 w-6 text-purple-400" />
            </div>
            <div class="ml-5 w-0 flex-1">
              <dl>
                <dt class="text-sm font-medium text-gray-500 dark:text-gray-400 truncate">Processed Today</dt>
                <dd class="text-lg font-medium text-gray-900 dark:text-white">${{ todayPayments.toLocaleString() }}</dd>
              </dl>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Filters and View Options -->
    <div class="bg-white dark:bg-gray-800 shadow-sm rounded-lg border border-gray-200 dark:border-gray-700">
      <div class="p-6">
        <div class="flex flex-wrap items-center justify-between gap-4">
          <!-- View Toggle -->
          <div class="flex items-center space-x-2">
            <button
              @click="currentView = 'invoices'"
              :class="currentView === 'invoices' 
                ? 'bg-purple-100 text-purple-700 dark:bg-purple-900 dark:text-purple-200' 
                : 'text-gray-500 hover:text-gray-700 dark:text-gray-400 dark:hover:text-gray-200'"
              class="px-3 py-2 rounded-md text-sm font-medium"
            >
              <DocumentTextIcon class="w-4 h-4 mr-2 inline" />
              Invoices
            </button>
            <button
              @click="currentView = 'payments'"
              :class="currentView === 'payments' 
                ? 'bg-purple-100 text-purple-700 dark:bg-purple-900 dark:text-purple-200' 
                : 'text-gray-500 hover:text-gray-700 dark:text-gray-400 dark:hover:text-gray-200'"
              class="px-3 py-2 rounded-md text-sm font-medium"
            >
              <CreditCardIcon class="w-4 h-4 mr-2 inline" />
              Payments
            </button>
            <button
              @click="currentView = 'reports'"
              :class="currentView === 'reports' 
                ? 'bg-purple-100 text-purple-700 dark:bg-purple-900 dark:text-purple-200' 
                : 'text-gray-500 hover:text-gray-700 dark:text-gray-400 dark:hover:text-gray-200'"
              class="px-3 py-2 rounded-md text-sm font-medium"
            >
              <ChartBarIcon class="w-4 h-4 mr-2 inline" />
              Reports
            </button>
          </div>

          <!-- Filters -->
          <div class="flex items-center space-x-4">
            <select
              v-model="statusFilter"
              class="block px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md bg-white dark:bg-gray-700 text-gray-900 dark:text-white focus:outline-none focus:ring-1 focus:ring-purple-500 focus:border-purple-500 sm:text-sm"
            >
              <option value="">All Statuses</option>
              <option value="paid">Paid</option>
              <option value="pending">Pending</option>
              <option value="overdue">Overdue</option>
              <option value="cancelled">Cancelled</option>
            </select>

            <input
              v-model="searchQuery"
              type="text"
              placeholder="Search patients..."
              class="block px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md bg-white dark:bg-gray-700 text-gray-900 dark:text-white focus:outline-none focus:ring-1 focus:ring-purple-500 focus:border-purple-500 sm:text-sm"
            />
          </div>
        </div>
      </div>
    </div>

    <!-- Invoices View -->
    <div v-if="currentView === 'invoices'" class="bg-white dark:bg-gray-800 shadow-sm rounded-lg border border-gray-200 dark:border-gray-700">
      <div class="px-6 py-4 border-b border-gray-200 dark:border-gray-700">
        <h3 class="text-lg font-medium text-gray-900 dark:text-white">
          Invoices ({{ filteredInvoices.length }})
        </h3>
      </div>
      
      <div class="overflow-x-auto">
        <table class="min-w-full divide-y divide-gray-200 dark:divide-gray-700">
          <thead class="bg-gray-50 dark:bg-gray-900">
            <tr>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase tracking-wider">
                Invoice & Patient
              </th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase tracking-wider">
                Services
              </th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase tracking-wider">
                Amount
              </th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase tracking-wider">
                Status
              </th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase tracking-wider">
                Due Date
              </th>
              <th class="relative px-6 py-3">
                <span class="sr-only">Actions</span>
              </th>
            </tr>
          </thead>
          <tbody class="bg-white dark:bg-gray-800 divide-y divide-gray-200 dark:divide-gray-700">
            <tr v-for="invoice in paginatedInvoices" :key="invoice.id" class="hover:bg-gray-50 dark:hover:bg-gray-700">
              <td class="px-6 py-4 whitespace-nowrap">
                <div>
                  <div class="text-sm font-medium text-gray-900 dark:text-white">
                    #{{ invoice.invoiceNumber }}
                  </div>
                  <div class="text-sm text-gray-500 dark:text-gray-400">
                    {{ invoice.patientName }}
                  </div>
                </div>
              </td>
              <td class="px-6 py-4">
                <div class="text-sm text-gray-900 dark:text-white">
                  <div v-for="service in invoice.services.slice(0, 2)" :key="service.id" class="truncate">
                    {{ service.name }}
                  </div>
                  <div v-if="invoice.services.length > 2" class="text-xs text-gray-500 dark:text-gray-400">
                    +{{ invoice.services.length - 2 }} more
                  </div>
                </div>
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900 dark:text-white">
                ${{ invoice.totalAmount.toLocaleString() }}
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <span 
                  class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium"
                  :class="getStatusBadgeClass(invoice.status)"
                >
                  {{ invoice.status }}
                </span>
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900 dark:text-white">
                {{ formatDate(invoice.dueDate) }}
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                <div class="flex items-center space-x-2">
                  <button
                    @click="viewInvoice(invoice)"
                    class="text-purple-600 hover:text-purple-900 dark:text-purple-400 dark:hover:text-purple-300"
                  >
                    View
                  </button>
                  <button
                    v-if="invoice.status === 'pending'"
                    @click="sendInvoice(invoice)"
                    class="text-blue-600 hover:text-blue-900 dark:text-blue-400 dark:hover:text-blue-300"
                  >
                    Send
                  </button>
                  <button
                    @click="downloadInvoice(invoice)"
                    class="text-green-600 hover:text-green-900 dark:text-green-400 dark:hover:text-green-300"
                  >
                    Download
                  </button>
                </div>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- Payments View -->
    <div v-else-if="currentView === 'payments'" class="bg-white dark:bg-gray-800 shadow-sm rounded-lg border border-gray-200 dark:border-gray-700">
      <div class="px-6 py-4 border-b border-gray-200 dark:border-gray-700">
        <h3 class="text-lg font-medium text-gray-900 dark:text-white">
          Payment Transactions ({{ paymentTransactions.length }})
        </h3>
      </div>
      
      <div class="overflow-x-auto">
        <table class="min-w-full divide-y divide-gray-200 dark:divide-gray-700">
          <thead class="bg-gray-50 dark:bg-gray-900">
            <tr>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase tracking-wider">
                Transaction
              </th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase tracking-wider">
                Patient
              </th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase tracking-wider">
                Amount
              </th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase tracking-wider">
                Method
              </th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase tracking-wider">
                Date
              </th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase tracking-wider">
                Status
              </th>
            </tr>
          </thead>
          <tbody class="bg-white dark:bg-gray-800 divide-y divide-gray-200 dark:divide-gray-700">
            <tr v-for="payment in paymentTransactions" :key="payment.id" class="hover:bg-gray-50 dark:hover:bg-gray-700">
              <td class="px-6 py-4 whitespace-nowrap">
                <div class="text-sm font-medium text-gray-900 dark:text-white">
                  {{ payment.transactionId }}
                </div>
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900 dark:text-white">
                {{ payment.patientName }}
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900 dark:text-white">
                ${{ payment.amount.toLocaleString() }}
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900 dark:text-white">
                {{ payment.paymentMethod }}
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900 dark:text-white">
                {{ formatDate(payment.date) }}
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <span 
                  class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium"
                  :class="getPaymentStatusBadgeClass(payment.status)"
                >
                  {{ payment.status }}
                </span>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- Reports View -->
    <div v-else-if="currentView === 'reports'" class="space-y-6">
      <!-- Revenue Chart -->
      <div class="bg-white dark:bg-gray-800 shadow-sm rounded-lg border border-gray-200 dark:border-gray-700">
        <div class="px-6 py-4 border-b border-gray-200 dark:border-gray-700">
          <h3 class="text-lg font-medium text-gray-900 dark:text-white">Revenue Overview</h3>
        </div>
        <div class="p-6">
          <div class="text-center py-8">
            <ChartBarIcon class="mx-auto h-12 w-12 text-gray-400" />
            <h3 class="mt-2 text-sm font-medium text-gray-900 dark:text-white">Revenue Charts</h3>
            <p class="mt-1 text-sm text-gray-500 dark:text-gray-400">
              Financial charts and analytics coming soon.
            </p>
          </div>
        </div>
      </div>

      <!-- Department Revenue Breakdown -->
      <div class="bg-white dark:bg-gray-800 shadow-sm rounded-lg border border-gray-200 dark:border-gray-700">
        <div class="px-6 py-4 border-b border-gray-200 dark:border-gray-700">
          <h3 class="text-lg font-medium text-gray-900 dark:text-white">Revenue by Department</h3>
        </div>
        <div class="p-6">
          <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
            <div v-for="dept in departmentRevenue" :key="dept.name" class="bg-gray-50 dark:bg-gray-900 rounded-lg p-4">
              <div class="flex justify-between items-center">
                <div>
                  <div class="text-sm font-medium text-gray-900 dark:text-white capitalize">{{ dept.name }}</div>
                  <div class="text-lg font-bold text-purple-600 dark:text-purple-400">${{ dept.revenue.toLocaleString() }}</div>
                </div>
                <div class="text-right">
                  <div class="text-xs text-gray-500 dark:text-gray-400">{{ dept.invoices }} invoices</div>
                  <div class="text-xs text-gray-500 dark:text-gray-400">{{ dept.patients }} patients</div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Create Invoice Modal -->
    <CreateInvoiceModal
      :is-open="showCreateInvoiceModal"
      @close="closeInvoiceModal"
      @save="handleSaveInvoice"
    />

    <!-- Subscription Plans Modal -->
    <SubscriptionPlansModal
      :is-open="showSubscriptionPlans"
      @close="showSubscriptionPlans = false"
    />

    <!-- Change Plan Modal -->
    <ChangePlanModal
      :is-open="showChangePlan"
      @close="showChangePlan = false"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { usePaymentStore, type Invoice } from '../../stores/payment'
import { useSubscriptionStore } from '../../stores/subscription'
import SubscriptionPlansModal from '../modals/SubscriptionPlansModal.vue'
import ChangePlanModal from '../modals/ChangePlanModal.vue'
import CreateInvoiceModal from '../modals/CreateInvoiceModal.vue'

// Define props
interface Props {
  selectedDepartment?: string | null
}

defineProps<Props>()

// Store
const paymentStore = usePaymentStore()
const subscriptionStore = useSubscriptionStore()

// Local state
const currentView = ref<'invoices' | 'payments' | 'reports'>('invoices')
const statusFilter = ref('')
const searchQuery = ref('')
const currentPage = ref(1)
const itemsPerPage = 20

const showCreateInvoiceModal = ref(false)
const showViewInvoiceModal = ref(false)
const showSubscriptionPlans = ref(false)
const showChangePlan = ref(false)
const selectedInvoice = ref<Invoice | null>(null)

// Computed properties
const filteredInvoices = computed(() => {
  let invoices = paymentStore.invoices

  // Apply status filter
  if (statusFilter.value) {
    invoices = invoices.filter(inv => inv.status === statusFilter.value)
  }

  // Apply search filter
  if (searchQuery.value) {
    const query = searchQuery.value.toLowerCase()
    invoices = invoices.filter(inv => 
      inv.patientName.toLowerCase().includes(query) ||
      inv.invoiceNumber.toLowerCase().includes(query)
    )
  }

  return invoices.sort((a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime())
})

const paginatedInvoices = computed(() => {
  const start = (currentPage.value - 1) * itemsPerPage
  const end = start + itemsPerPage
  return filteredInvoices.value.slice(start, end)
})

const paymentTransactions = computed(() => paymentStore.transactions)

const monthlyRevenue = computed(() => {
  const thisMonth = new Date().getMonth()
  const thisYear = new Date().getFullYear()
  
  return paymentStore.transactions
    .filter(t => {
      const date = new Date(t.date)
      return date.getMonth() === thisMonth && 
             date.getFullYear() === thisYear && 
             t.status === 'completed'
    })
    .reduce((sum, t) => sum + t.amount, 0)
})

const pendingInvoices = computed(() => 
  paymentStore.invoices.filter(inv => inv.status === 'pending')
)

const overdueAmount = computed(() => {
  const now = new Date()
  return paymentStore.invoices
    .filter(inv => inv.status === 'overdue' || (
      inv.status === 'pending' && new Date(inv.dueDate) < now
    ))
    .reduce((sum, inv) => sum + inv.totalAmount, 0)
})

const todayPayments = computed(() => {
  const today = new Date().toDateString()
  return paymentStore.transactions
    .filter(t => new Date(t.date).toDateString() === today && t.status === 'completed')
    .reduce((sum, t) => sum + t.amount, 0)
})

const departmentRevenue = computed(() => {
  const deptData: Record<string, { revenue: number, invoices: number, patients: Set<string> }> = {}
  
  paymentStore.invoices.forEach(invoice => {
    invoice.services.forEach(service => {
      if (!deptData[service.department]) {
        deptData[service.department] = { revenue: 0, invoices: 0, patients: new Set() }
      }
      deptData[service.department].revenue += service.amount
      deptData[service.department].invoices++
      deptData[service.department].patients.add(invoice.patientId)
    })
  })

  return Object.entries(deptData).map(([name, data]) => ({
    name,
    revenue: data.revenue,
    invoices: data.invoices,
    patients: data.patients.size
  }))
})

// Icon components
const PlusIcon = {
  template: `<svg fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" d="M12 4.5v15m7.5-7.5h-15" /></svg>`
}

const DocumentArrowDownIcon = {
  template: `<svg fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" d="M19.5 14.25v-2.625a3.375 3.375 0 00-3.375-3.375h-4.5A1.125 1.125 0 0010.5 9h-4.5a3.375 3.375 0 00-3.375 3.375v8.25a3.375 3.375 0 003.375 3.375h9a3.375 3.375 0 003.375-3.375zM9 16.5v-4.5m1.5 0L9 10.5l-1.5 1.5" /></svg>`
}

const CurrencyDollarIcon = {
  template: `<svg fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" d="M12 6v12m-3-2.818l.879.659c1.171.879 3.07.879 4.242 0 1.172-.879 1.172-2.303 0-3.182C13.536 12.219 12.768 12 12 12c-.725 0-1.467-.22-2.121-.659-1.172-.879-1.172-2.303 0-3.182C10.464 7.69 11.232 7.5 12 7.5c.768 0 1.536.22 2.121.659l.879-.659m-4.242 0V6m0 12v1.5m6-6.5h1.5m-7.5 0h-1.5" /></svg>`
}

const DocumentTextIcon = {
  template: `<svg fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" d="M19.5 14.25v-2.625a3.375 3.375 0 00-3.375-3.375h-4.5A1.125 1.125 0 0010.5 9h-4.5a3.375 3.375 0 00-3.375 3.375v8.25a3.375 3.375 0 003.375 3.375h9a3.375 3.375 0 003.375-3.375V14.25zM9.75 9.75l-1.5-1.5m0 0l1.5-1.5m-1.5 1.5h12" /></svg>`
}

const ClockIcon = {
  template: `<svg fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" d="M12 6v6h4.5m4.5 0a9 9 0 11-18 0 9 9 0 0118 0z" /></svg>`
}

const CreditCardIcon = {
  template: `<svg fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" d="M2.25 8.25h19.5M2.25 9h19.5m-16.5 5.25h6m-6 2.25h3m-3.75 3h15a2.25 2.25 0 002.25-2.25V6.75A2.25 2.25 0 0019.5 4.5h-15a2.25 2.25 0 00-2.25 2.25v10.5A2.25 2.25 0 004.5 19.5z" /></svg>`
}

const ChartBarIcon = {
  template: `<svg fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" d="M3 13.125C3 12.504 3.504 12 4.125 12h2.25c.621 0 1.125.504 1.125 1.125v6.75C7.5 20.496 6.996 21 6.375 21h-2.25A1.125 1.125 0 013 19.875v-6.75zM9.75 8.625c0-.621.504-1.125 1.125-1.125h2.25c.621 0 1.125.504 1.125 1.125v11.25c0 .621-.504 1.125-1.125 1.125h-2.25a1.125 1.125 0 01-1.125-1.125V8.625zM16.5 4.125c0-.621.504-1.125 1.125-1.125h2.25C20.496 3 21 3.504 21 4.125v15.75c0 .621-.504 1.125-1.125 1.125h-2.25a1.125 1.125 0 01-1.125-1.125V4.125z" /></svg>`
}

// Methods
const formatDate = (date?: string) => {
  if (!date) return ''
  return new Date(date).toLocaleDateString()
}

const getStatusBadgeClass = (status: string) => {
  const classes: Record<string, string> = {
    paid: 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-200',
    pending: 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900 dark:text-yellow-200',
    overdue: 'bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-200',
    cancelled: 'bg-gray-100 text-gray-800 dark:bg-gray-900 dark:text-gray-200'
  }
  return classes[status] || classes.pending
}

const getPaymentStatusBadgeClass = (status: string) => {
  const classes: Record<string, string> = {
    completed: 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-200',
    processing: 'bg-blue-100 text-blue-800 dark:bg-blue-900 dark:text-blue-200',
    failed: 'bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-200',
    refunded: 'bg-gray-100 text-gray-800 dark:bg-gray-900 dark:text-gray-200'
  }
  return classes[status] || classes.processing
}

const viewInvoice = (invoice: Invoice) => {
  selectedInvoice.value = invoice
  showViewInvoiceModal.value = true
}

const sendInvoice = async (invoice: Invoice) => {
  try {
    // Simulate sending invoice via email
    const confirmed = confirm(`Send invoice ${invoice.invoiceNumber} to ${invoice.patientName}?`)
    if (!confirmed) return
    
    // Simulate API call with loading state
    console.log('Sending invoice...')
    await new Promise(resolve => setTimeout(resolve, 1500))
    
    // Update invoice status to indicate it was sent
    const invoiceIndex = paymentStore.invoices.findIndex(inv => inv.id === invoice.id)
    if (invoiceIndex !== -1) {
      // Mark as sent (you might want to add a "sent" status to your Invoice interface)
      console.log(`Invoice ${invoice.invoiceNumber} sent successfully`)
    }
    
    alert(`✅ Invoice ${invoice.invoiceNumber} sent successfully to ${invoice.patientName}!`)
  } catch (error) {
    console.error('Error sending invoice:', error)
    alert('❌ Failed to send invoice. Please try again.')
  }
}

const downloadInvoice = async (invoice: Invoice) => {
  try {
    await paymentStore.downloadInvoice(invoice.id)
  } catch (error) {
    console.error('Error downloading invoice:', error)
    alert('Failed to download invoice. Please try again.')
  }
}

const exportReports = async () => {
  try {
    const reportType = prompt('Select report type:\n1. Invoice Summary\n2. Payment History\n3. Financial Overview\n\nEnter 1, 2, or 3:', '1')
    
    if (!reportType || !['1', '2', '3'].includes(reportType)) {
      return
    }
    
    const reportNames = {
      '1': 'Invoice Summary',
      '2': 'Payment History', 
      '3': 'Financial Overview'
    }
    
    // Simulate generating and downloading report
    console.log(`Generating ${reportNames[reportType as keyof typeof reportNames]} report...`)
    await new Promise(resolve => setTimeout(resolve, 2000))
    
    // Create a simple CSV data for demo
    const csvData = generateReportData(reportType)
    downloadCSV(csvData, `${reportNames[reportType as keyof typeof reportNames]}_${new Date().toISOString().split('T')[0]}.csv`)
    
    alert(`✅ ${reportNames[reportType as keyof typeof reportNames]} report exported successfully!`)
  } catch (error) {
    console.error('Error exporting reports:', error)
    alert('❌ Failed to export reports. Please try again.')
  }
}

const generateReportData = (type: string): string => {
  switch (type) {
    case '1':
      return 'Invoice Number,Patient Name,Amount,Status,Date\n' +
             paymentStore.invoices.map(inv => 
               `${inv.invoiceNumber},${inv.patientName},${inv.amount},${inv.status},${inv.createdAt}`
             ).join('\n')
    case '2':
      return 'Transaction ID,Amount,Type,Status,Date\n' +
             paymentStore.transactions.map(txn => 
               `${txn.transactionId},${txn.amount},${txn.type},${txn.status},${txn.date}`
             ).join('\n')
    case '3':
      const totalRevenue = paymentStore.transactions
        .filter(t => t.status === 'succeeded')
        .reduce((sum, t) => sum + t.amount, 0)
      return `Financial Overview Report\nGenerated: ${new Date().toISOString()}\n\nTotal Revenue: $${totalRevenue}\nTotal Invoices: ${paymentStore.invoices.length}\nPaid Invoices: ${paymentStore.paidInvoices.length}`
    default:
      return 'No data available'
  }
}

const downloadCSV = (csvContent: string, filename: string) => {
  const blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' })
  const link = document.createElement('a')
  if (link.download !== undefined) {
    const url = URL.createObjectURL(blob)
    link.setAttribute('href', url)
    link.setAttribute('download', filename)
    link.style.visibility = 'hidden'
    document.body.appendChild(link)
    link.click()
    document.body.removeChild(link)
  }
}

const closeInvoiceModal = () => {
  showCreateInvoiceModal.value = false
  showViewInvoiceModal.value = false
  selectedInvoice.value = null
}

const handleSaveInvoice = async (invoiceData: Omit<Invoice, 'id'>) => {
  try {
    await paymentStore.createInvoice(invoiceData)
    closeInvoiceModal()
  } catch (error) {
    console.error('Error saving invoice:', error)
    alert('Failed to save invoice. Please try again.')
  }
}

// Subscription management functions
const cancelSubscription = async () => {
  if (confirm('Are you sure you want to cancel your subscription? You will continue to have access until the end of your billing period.')) {
    try {
      await subscriptionStore.cancelSubscription()
      alert('Subscription cancelled successfully. You will continue to have access until the end of your billing period.')
    } catch (error) {
      console.error('Error cancelling subscription:', error)
      alert('Failed to cancel subscription. Please try again.')
    }
  }
}

const reactivateSubscription = async () => {
  try {
    await subscriptionStore.reactivateSubscription()
    alert('Subscription reactivated successfully!')
  } catch (error) {
    console.error('Error reactivating subscription:', error)
    alert('Failed to reactivate subscription. Please try again.')
  }
}

// Initialize data on component mount
onMounted(async () => {
  // Initialize sample data for demo purposes
  paymentStore.initializeSampleData()
  
  await subscriptionStore.loadAvailablePlans()
  await subscriptionStore.loadUserSubscription()
})
</script>
