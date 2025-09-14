<!-- Copyright (c) 2025 Alexander Sinapov | Simeon Petkov -->

<!-- All rights reserved. -->
<!-- This code is proprietary and confidential.   -->
<!-- Unauthorized copying, modification, distribution, or use is strictly prohibited. -->

<template>
  <div class="py-6 mx-auto max-w-7xl sm:px-6 lg:px-8">
    <!-- Header Section -->
    <div class="mb-8 md:flex md:items-center md:justify-between">
      <div class="flex-1 min-w-0">
        <h2 class="text-2xl font-bold leading-7 text-gray-900 dark:text-white sm:text-3xl sm:truncate">
          Billing & Subscriptions
        </h2>
        <p class="mt-1 text-sm text-gray-500 dark:text-gray-400">
          Manage your subscription, view invoices, and track payments
        </p>
      </div>
      <div class="flex mt-4 space-x-3 md:mt-0 md:ml-4">
        <button
          @click="handleCreateInvoice"
          class="inline-flex items-center px-4 py-2 text-sm font-medium text-white bg-purple-600 border border-transparent rounded-md shadow-sm hover:bg-purple-700"
        >
          Create Invoice
        </button>
        <button
          @click="handleExportData"
          class="inline-flex items-center px-4 py-2 text-sm font-medium text-gray-700 bg-white border border-gray-300 rounded-md shadow-sm hover:bg-gray-50 dark:bg-gray-800 dark:text-gray-200 dark:border-gray-600 dark:hover:bg-gray-700"
        >
          Export Data
        </button>
      </div>
    </div>

    <!-- Main Content Grid -->
    <div class="grid grid-cols-1 gap-8 lg:grid-cols-3">
      <!-- Left Column: Subscription Management -->
      <div class="space-y-6 lg:col-span-2">
        <!-- Subscription Management Section -->
        <div v-if="hasActiveSubscription" class="bg-white border border-gray-200 rounded-lg shadow-sm dark:bg-gray-800 dark:border-gray-700">
        <div class="px-6 py-4 border-b border-gray-200 dark:border-gray-700">
          <div class="flex items-center justify-between">
            <h3 class="text-lg font-medium text-gray-900 dark:text-white">Current Subscription</h3>
            <span class="inline-flex items-center px-3 py-1 text-xs font-medium text-green-800 bg-green-100 rounded-full dark:bg-green-900 dark:text-green-300">
              Active
            </span>
          </div>
        </div>
        <div class="p-6">
          <div class="flex flex-col gap-6 lg:flex-row lg:items-center lg:justify-between">
            <div class="flex-1">
              <div class="flex items-center gap-4">
                <div class="flex-shrink-0">
                  <div class="flex items-center justify-center w-12 h-12 bg-purple-100 rounded-lg dark:bg-purple-900">
                    <svg class="w-6 h-6 text-purple-600 dark:text-purple-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 3v4M3 5h4M6 17v4m-2-2h4m5-16l2.286 6.857L21 12l-5.714 2.143L13 21l-2.286-6.857L5 12l5.714-2.143L13 3z"></path>
                    </svg>
                  </div>
                </div>
                <div>
                  <h4 class="text-xl font-semibold text-gray-900 dark:text-white">
                    {{ currentPlanName }} Plan
                  </h4>
                  <p class="text-lg font-medium text-purple-600 dark:text-purple-400">
                    ${{ currentPlanPrice }}/{{ currentPlanInterval }}
                  </p>
                  <p class="mt-1 text-sm text-gray-500 dark:text-gray-400">
                    Next billing: {{ formatDate(nextBillingDate) }}
                  </p>
                  <span 
                    v-if="isCancellingAtPeriodEnd"
                    class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-300 mt-2"
                  >
                    <svg class="w-4 h-4 mr-1" fill="currentColor" viewBox="0 0 20 20">
                      <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zM8.707 7.293a1 1 0 00-1.414 1.414L8.586 10l-1.293 1.293a1 1 0 101.414 1.414L10 11.414l1.293 1.293a1 1 0 001.414-1.414L11.414 10l1.293-1.293a1 1 0 00-1.414-1.414L10 8.586 8.707 7.293z" clip-rule="evenodd"></path>
                    </svg>
                    Cancelling at period end
                  </span>
                </div>
              </div>
              
              <!-- Plan Features -->
              <div class="grid grid-cols-1 gap-3 mt-4 sm:grid-cols-2">
                <div class="flex items-center text-sm text-gray-600 dark:text-gray-300">
                  <svg class="w-4 h-4 mr-2 text-green-500" fill="currentColor" viewBox="0 0 20 20">
                    <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd"></path>
                  </svg>
                  {{ maxPatientsText }} patients
                </div>
                <div class="flex items-center text-sm text-gray-600 dark:text-gray-300">
                  <svg class="w-4 h-4 mr-2 text-green-500" fill="currentColor" viewBox="0 0 20 20">
                    <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd"></path>
                  </svg>
                  {{ aiAnalysisLimitText }} AI analyses
                </div>
              </div>
            </div>
            
            <div class="flex flex-col gap-3 sm:flex-row">
              <button
                v-if="!isCancellingAtPeriodEnd"
                @click="showChangePlan = true"
                class="inline-flex items-center justify-center px-4 py-2 text-sm font-medium text-purple-700 transition-colors border border-purple-200 rounded-md bg-purple-50 hover:bg-purple-100 dark:bg-purple-900/20 dark:text-purple-300 dark:border-purple-800 dark:hover:bg-purple-900/40"
              >
                <svg class="w-4 h-4 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M7 16V4m0 0L3 8m4-4l4 4m6 0v12m0 0l4-4m-4 4l-4-4"></path>
                </svg>
                Change Plan
              </button>
              <button
                v-if="!isCancellingAtPeriodEnd"
                @click="handleCancelSubscription"
                class="inline-flex items-center justify-center px-4 py-2 text-sm font-medium text-red-700 transition-colors border border-red-200 rounded-md bg-red-50 hover:bg-red-100 dark:bg-red-900/20 dark:text-red-300 dark:border-red-800 dark:hover:bg-red-900/40"
              >
                <svg class="w-4 h-4 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"></path>
                </svg>
                Cancel Subscription
              </button>
              <button
                v-else
                @click="handleReactivateSubscription"
                class="inline-flex items-center justify-center px-4 py-2 text-sm font-medium text-green-700 transition-colors border border-green-200 rounded-md bg-green-50 hover:bg-green-100 dark:bg-green-900/20 dark:text-green-300 dark:border-green-800 dark:hover:bg-green-900/40"
              >
                <svg class="w-4 h-4 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15"></path>
                </svg>
                Reactivate
              </button>
            </div>
          </div>
        </div>
      </div>

        <!-- No Subscription State -->
        <div v-else class="bg-white border border-gray-200 rounded-lg shadow-sm dark:bg-gray-800 dark:border-gray-700">
        <div class="p-8 text-center">
          <div class="flex items-center justify-center w-16 h-16 mx-auto mb-4 bg-purple-100 rounded-full dark:bg-purple-900/30">
            <svg class="w-8 h-8 text-purple-600 dark:text-purple-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 10h18M7 15h1m4 0h1m-7 4h12a3 3 0 003-3V8a3 3 0 00-3-3H6a3 3 0 00-3 3v8a3 3 0 003 3z"></path>
            </svg>
          </div>
          <h3 class="mb-2 text-lg font-medium text-gray-900 dark:text-white">No Active Subscription</h3>
          <p class="max-w-md mx-auto mb-6 text-gray-500 dark:text-gray-400">
            Subscribe to unlock all features and get unlimited access to AI-powered medical analysis, patient management, and advanced reporting.
          </p>
          <div class="flex flex-col justify-center gap-3 sm:flex-row">
            <button
              @click="showSubscriptionPlans = true"
              class="inline-flex items-center justify-center px-6 py-3 text-base font-medium text-white transition-colors bg-purple-600 border border-transparent rounded-md shadow-sm hover:bg-purple-700"
            >
              <svg class="w-5 h-5 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 7h8m0 0v8m0-8l-8 8-4-4-6 6"></path>
              </svg>
              View Plans & Pricing
            </button>
            <button
              class="inline-flex items-center justify-center px-6 py-3 text-base font-medium text-purple-700 transition-colors border border-purple-200 rounded-md bg-purple-50 hover:bg-purple-100 dark:bg-purple-900/20 dark:text-purple-300 dark:border-purple-800 dark:hover:bg-purple-900/40"
            >
              Learn More
            </button>
          </div>
        </div>
      </div>
      </div>

      <!-- Right Column: Financial Overview -->
      <div class="space-y-6 lg:col-span-1">
        <!-- Quick Stats -->
        <div class="space-y-4">
          <h3 class="text-lg font-medium text-gray-900 dark:text-white">Financial Overview</h3>
          
          <!-- Total Revenue -->
          <div class="overflow-hidden bg-white border border-gray-200 rounded-lg shadow-sm dark:bg-gray-800 dark:border-gray-700">
            <div class="p-4">
              <div class="flex items-center">
                <div class="flex-shrink-0">
                  <DollarIcon class="w-6 h-6 text-green-500" />
                </div>
                <div class="flex-1 w-0 ml-3">
                  <dl>
                    <dt class="text-sm font-medium text-gray-500 truncate dark:text-gray-400">
                      Total Revenue
                    </dt>
                    <dd class="text-lg font-medium text-gray-900 dark:text-white">
                      ${{ totalRevenue.toLocaleString() }}
                    </dd>
                  </dl>
                </div>
              </div>
            </div>
          </div>

          <!-- Pending Invoices -->
          <div class="overflow-hidden bg-white border border-gray-200 rounded-lg shadow-sm dark:bg-gray-800 dark:border-gray-700">
            <div class="p-4">
              <div class="flex items-center">
                <div class="flex-shrink-0">
                  <ClockIcon class="w-6 h-6 text-yellow-500" />
                </div>
                <div class="flex-1 w-0 ml-3">
                  <dl>
                    <dt class="text-sm font-medium text-gray-500 truncate dark:text-gray-400">
                      Pending Invoices
                    </dt>
                    <dd class="text-lg font-medium text-gray-900 dark:text-white">
                      {{ pendingInvoicesCount }}
                    </dd>
                  </dl>
                </div>
              </div>
            </div>
          </div>

          <!-- Monthly Revenue -->
          <div class="overflow-hidden bg-white border border-gray-200 rounded-lg shadow-sm dark:bg-gray-800 dark:border-gray-700">
            <div class="p-4">
              <div class="flex items-center">
                <div class="flex-shrink-0">
                  <CheckIcon class="w-6 h-6 text-green-500" />
                </div>
                <div class="flex-1 w-0 ml-3">
                  <dl>
                    <dt class="text-sm font-medium text-gray-500 truncate dark:text-gray-400">
                      This Month
                    </dt>
                    <dd class="text-lg font-medium text-gray-900 dark:text-white">
                      ${{ monthlyRevenue.toLocaleString() }}
                    </dd>
                  </dl>
                </div>
              </div>
            </div>
          </div>

          <!-- Outstanding Balance -->
          <div class="overflow-hidden bg-white border border-gray-200 rounded-lg shadow-sm dark:bg-gray-800 dark:border-gray-700">
            <div class="p-4">
              <div class="flex items-center">
                <div class="flex-shrink-0">
                  <ExclamationIcon class="w-6 h-6 text-red-500" />
                </div>
                <div class="flex-1 w-0 ml-3">
                  <dl>
                    <dt class="text-sm font-medium text-gray-500 truncate dark:text-gray-400">
                      Outstanding
                    </dt>
                    <dd class="text-lg font-medium text-gray-900 dark:text-white">
                      ${{ outstandingBalance.toLocaleString() }}
                    </dd>
                  </dl>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Billing Management Section (Full Width) -->
    <!-- Filters and View Options -->
    <div class="bg-white border border-gray-200 rounded-lg shadow-sm dark:bg-gray-800 dark:border-gray-700">
      <div class="p-6">
        <div class="flex flex-wrap items-center justify-between gap-4">
          <!-- View Toggle -->
          <div class="flex items-center space-x-2">
            <button
              @click="currentView = 'invoices'"
              :class="currentView === 'invoices' 
                ? 'bg-purple-100 text-purple-700 dark:bg-purple-900 dark:text-purple-200' 
                : 'text-gray-500 hover:text-gray-700 dark:text-gray-400 dark:hover:text-gray-200'"
              class="px-3 py-2 text-sm font-medium rounded-md"
            >
              <DocumentTextIcon class="inline w-4 h-4 mr-2" />
              Invoices
            </button>
            <button
              @click="currentView = 'payments'"
              :class="currentView === 'payments' 
                ? 'bg-purple-100 text-purple-700 dark:bg-purple-900 dark:text-purple-200' 
                : 'text-gray-500 hover:text-gray-700 dark:text-gray-400 dark:hover:text-gray-200'"
              class="px-3 py-2 text-sm font-medium rounded-md"
            >
              <CreditCardIcon class="inline w-4 h-4 mr-2" />
              Payments
            </button>
            <button
              @click="currentView = 'reports'"
              :class="currentView === 'reports' 
                ? 'bg-purple-100 text-purple-700 dark:bg-purple-900 dark:text-purple-200' 
                : 'text-gray-500 hover:text-gray-700 dark:text-gray-400 dark:hover:text-gray-200'"
              class="px-3 py-2 text-sm font-medium rounded-md"
            >
              <ChartBarIcon class="inline w-4 h-4 mr-2" />
              Reports
            </button>
          </div>

          <!-- Filters -->
          <div class="flex items-center space-x-4">
            <select
              v-model="statusFilter"
              class="block px-3 py-2 text-gray-900 bg-white border border-gray-300 rounded-md dark:border-gray-600 dark:bg-gray-700 dark:text-white focus:outline-none focus:ring-1 focus:ring-purple-500 focus:border-purple-500 sm:text-sm"
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
              class="block px-3 py-2 text-gray-900 bg-white border border-gray-300 rounded-md dark:border-gray-600 dark:bg-gray-700 dark:text-white focus:outline-none focus:ring-1 focus:ring-purple-500 focus:border-purple-500 sm:text-sm"
            />
          </div>
        </div>
      </div>
    </div>

    <!-- Invoices View -->
    <div v-if="currentView === 'invoices'" class="bg-white border border-gray-200 rounded-lg shadow-sm dark:bg-gray-800 dark:border-gray-700">
      <div class="px-6 py-4 border-b border-gray-200 dark:border-gray-700">
        <h3 class="text-lg font-medium text-gray-900 dark:text-white">
          Invoices ({{ filteredInvoices.length }})
        </h3>
      </div>
      
      <div class="overflow-x-auto">
        <table class="min-w-full divide-y divide-gray-200 dark:divide-gray-700">
          <thead class="bg-gray-50 dark:bg-gray-900">
            <tr>
              <th class="px-6 py-3 text-xs font-medium tracking-wider text-left text-gray-500 uppercase dark:text-gray-400">
                Invoice & Patient
              </th>
              <th class="px-6 py-3 text-xs font-medium tracking-wider text-left text-gray-500 uppercase dark:text-gray-400">
                Services
              </th>
              <th class="px-6 py-3 text-xs font-medium tracking-wider text-left text-gray-500 uppercase dark:text-gray-400">
                Amount
              </th>
              <th class="px-6 py-3 text-xs font-medium tracking-wider text-left text-gray-500 uppercase dark:text-gray-400">
                Status
              </th>
              <th class="px-6 py-3 text-xs font-medium tracking-wider text-left text-gray-500 uppercase dark:text-gray-400">
                Due Date
              </th>
              <th class="relative px-6 py-3">
                <span class="sr-only">Actions</span>
              </th>
            </tr>
          </thead>
          <tbody class="bg-white divide-y divide-gray-200 dark:bg-gray-800 dark:divide-gray-700">
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
              <td class="px-6 py-4 text-sm font-medium text-gray-900 whitespace-nowrap dark:text-white">
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
              <td class="px-6 py-4 text-sm text-gray-900 whitespace-nowrap dark:text-white">
                {{ formatDate(invoice.dueDate) }}
              </td>
              <td class="px-6 py-4 text-sm font-medium text-right whitespace-nowrap">
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
    <div v-else-if="currentView === 'payments'" class="bg-white border border-gray-200 rounded-lg shadow-sm dark:bg-gray-800 dark:border-gray-700">
      <div class="px-6 py-4 border-b border-gray-200 dark:border-gray-700">
        <h3 class="text-lg font-medium text-gray-900 dark:text-white">
          Payment Transactions ({{ paymentTransactions.length }})
        </h3>
      </div>
      
      <div class="overflow-x-auto">
        <table class="min-w-full divide-y divide-gray-200 dark:divide-gray-700">
          <thead class="bg-gray-50 dark:bg-gray-900">
            <tr>
              <th class="px-6 py-3 text-xs font-medium tracking-wider text-left text-gray-500 uppercase dark:text-gray-400">
                Transaction
              </th>
              <th class="px-6 py-3 text-xs font-medium tracking-wider text-left text-gray-500 uppercase dark:text-gray-400">
                Patient
              </th>
              <th class="px-6 py-3 text-xs font-medium tracking-wider text-left text-gray-500 uppercase dark:text-gray-400">
                Amount
              </th>
              <th class="px-6 py-3 text-xs font-medium tracking-wider text-left text-gray-500 uppercase dark:text-gray-400">
                Method
              </th>
              <th class="px-6 py-3 text-xs font-medium tracking-wider text-left text-gray-500 uppercase dark:text-gray-400">
                Date
              </th>
              <th class="px-6 py-3 text-xs font-medium tracking-wider text-left text-gray-500 uppercase dark:text-gray-400">
                Status
              </th>
            </tr>
          </thead>
          <tbody class="bg-white divide-y divide-gray-200 dark:bg-gray-800 dark:divide-gray-700">
            <tr v-for="payment in paymentTransactions" :key="payment.id" class="hover:bg-gray-50 dark:hover:bg-gray-700">
              <td class="px-6 py-4 whitespace-nowrap">
                <div class="text-sm font-medium text-gray-900 dark:text-white">
                  {{ payment.transactionId }}
                </div>
              </td>
              <td class="px-6 py-4 text-sm text-gray-900 whitespace-nowrap dark:text-white">
                {{ payment.patientName }}
              </td>
              <td class="px-6 py-4 text-sm font-medium text-gray-900 whitespace-nowrap dark:text-white">
                ${{ payment.amount.toLocaleString() }}
              </td>
              <td class="px-6 py-4 text-sm text-gray-900 whitespace-nowrap dark:text-white">
                {{ payment.paymentMethod }}
              </td>
              <td class="px-6 py-4 text-sm text-gray-900 whitespace-nowrap dark:text-white">
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
      <div class="bg-white border border-gray-200 rounded-lg shadow-sm dark:bg-gray-800 dark:border-gray-700">
        <div class="px-6 py-4 border-b border-gray-200 dark:border-gray-700">
          <h3 class="text-lg font-medium text-gray-900 dark:text-white">Revenue Overview</h3>
        </div>
        <div class="p-6">
          <div class="py-8 text-center">
            <ChartBarIcon class="w-12 h-12 mx-auto text-gray-400" />
            <h3 class="mt-2 text-sm font-medium text-gray-900 dark:text-white">Revenue Charts</h3>
            <p class="mt-1 text-sm text-gray-500 dark:text-gray-400">
              Financial charts and analytics coming soon.
            </p>
          </div>
        </div>
      </div>

      <!-- Department Revenue Breakdown -->
      <div class="bg-white border border-gray-200 rounded-lg shadow-sm dark:bg-gray-800 dark:border-gray-700">
        <div class="px-6 py-4 border-b border-gray-200 dark:border-gray-700">
          <h3 class="text-lg font-medium text-gray-900 dark:text-white">Revenue by Department</h3>
        </div>
        <div class="p-6">
          <div class="grid grid-cols-1 gap-4 md:grid-cols-2 lg:grid-cols-3">
            <div v-for="dept in departmentRevenue" :key="dept.name" class="p-4 rounded-lg bg-gray-50 dark:bg-gray-900">
              <div class="flex items-center justify-between">
                <div>
                  <div class="text-sm font-medium text-gray-900 capitalize dark:text-white">{{ dept.name }}</div>
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
      @plan-changed="handlePlanChanged"
    />
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
const initializationInProgress = ref(false)

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

const totalRevenue = computed(() => {
  return paymentStore.transactions
    .filter(t => t.status === 'completed')
    .reduce((sum, t) => sum + t.amount, 0)
})

const pendingInvoicesCount = computed(() => {
  return paymentStore.invoices.filter(inv => inv.status === 'pending').length
})

const outstandingBalance = computed(() => {
  return paymentStore.invoices
    .filter(inv => inv.status === 'pending' || inv.status === 'overdue')
    .reduce((sum, inv) => sum + inv.amount, 0)
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

const DollarIcon = {
  template: `<svg fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" d="M12 6v12m-3-2.818l.879.659c1.171.879 3.07.879 4.242 0 1.172-.879 1.172-2.303 0-3.182C13.536 12.219 12.768 12 12 12c-.725 0-1.467-.22-2.121-.659-1.172-.879-1.172-2.303 0-3.182C10.464 7.69 11.232 7.5 12 7.5c.768 0 1.536.22 2.121.659l.879-.659m-4.242 0V6m0 12v1.5m6-6.5h1.5m-7.5 0h-1.5" /></svg>`
}

const CheckIcon = {
  template: `<svg fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" d="m4.5 12.75 6 6 9-13.5" /></svg>`
}

const ExclamationIcon = {
  template: `<svg fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" d="M12 9v3.75m-9.303 3.376c-.866 1.5.217 3.374 1.948 3.374h14.71c1.73 0 2.813-1.874 1.948-3.374L13.949 3.378c-.866-1.5-3.032-1.5-3.898 0L2.697 16.126ZM12 15.75h.007v.008H12v-.008Z" /></svg>`
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

// EMERGENCY: Static mock data to prevent infinite loop
// TODO: Replace with subscription store once loop is fixed
const hasActiveSubscription = computed(() => {
  console.log('BillingView: Using mock hasActiveSubscription = true')
  return true // Mock active subscription
})

const currentPlanName = computed(() => {
  console.log('BillingView: Using mock plan name = Professional')
  return 'Professional'
})

const currentPlanPrice = computed(() => {
  console.log('BillingView: Using mock price = 29')
  return 29
})

const currentPlanInterval = computed(() => {
  console.log('BillingView: Using mock interval = month')
  return 'month'
})

const nextBillingDate = computed(() => {
  console.log('BillingView: Using mock billing date')
  return '2025-10-14'
})

const isCancellingAtPeriodEnd = computed(() => {
  console.log('BillingView: Using mock cancellation status = false')
  return false
})

const maxPatientsText = computed(() => {
  console.log('BillingView: Using mock max patients = 100')
  return '100'
})

const aiAnalysisLimitText = computed(() => {
  console.log('BillingView: Using mock AI limit = Unlimited')
  return 'Unlimited'
})

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

const handleCreateInvoice = () => {
  showCreateInvoiceModal.value = true
}

const handleExportData = async () => {
  try {
    // Simple CSV export of billing data
    const csvData = 'Invoice ID,Patient Name,Amount,Status,Date\n' +
      paymentStore.invoices.map(inv => 
        `${inv.invoiceNumber},${inv.patientName},${inv.amount},${inv.status},${inv.createdAt}`
      ).join('\n')
    
    const blob = new Blob([csvData], { type: 'text/csv;charset=utf-8;' })
    const link = document.createElement('a')
    const url = URL.createObjectURL(blob)
    link.setAttribute('href', url)
    link.setAttribute('download', `billing_data_${new Date().toISOString().split('T')[0]}.csv`)
    link.style.visibility = 'hidden'
    document.body.appendChild(link)
    link.click()
    document.body.removeChild(link)
  } catch (error) {
    console.error('Export failed:', error)
  }
}

const handleCancelSubscription = async () => {
  if (confirm('Are you sure you want to cancel your subscription?')) {
    try {
      console.log('BillingView: Cancelling subscription...')
      await subscriptionStore.cancelSubscription()
      console.log('BillingView: Subscription cancelled successfully')
    } catch (error) {
      console.error('BillingView: Cancellation failed:', error)
    }
  }
}

const handleReactivateSubscription = async () => {
  try {
    console.log('BillingView: Reactivating subscription...')
    await subscriptionStore.reactivateSubscription()
    console.log('BillingView: Subscription reactivated successfully')
  } catch (error) {
    console.error('BillingView: Reactivation failed:', error)
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

const handlePlanChanged = async () => {
  try {
    console.log('BillingView: Plan changed, reloading subscription data...')
    // Reload subscription data after plan change
    await subscriptionStore.loadUserSubscription()
    console.log('BillingView: Subscription data reloaded successfully after plan change')
  } catch (error) {
    console.error('BillingView: Error reloading subscription after plan change:', error)
  }
}

// Initialize data on component mount
onMounted(async () => {
  // Prevent multiple initializations
  if (initializationInProgress.value) {
    console.log('BillingView: Initialization already in progress, skipping...')
    return
  }
  
  initializationInProgress.value = true
  
  try {
    console.log('BillingView: Starting initialization...')
    
    // Initialize sample data for demo purposes
    paymentStore.initializeSampleData()
    console.log('BillingView: Sample data initialized')
    
    // TESTING: Re-enabling loadAvailablePlans only (with circuit breaker protection)
    console.log('BillingView: Testing loadAvailablePlans with circuit breaker protection')
    
    // Test loadAvailablePlans first
    try {
      console.log('BillingView: Loading available plans...')
      // Add timeout to prevent hanging
      const plansPromise = subscriptionStore.loadAvailablePlans()
      const plansTimeout = new Promise((_, reject) => 
        setTimeout(() => reject(new Error('Plans loading timeout')), 5000)
      )
      await Promise.race([plansPromise, plansTimeout])
      console.log('BillingView: Available plans loaded successfully - NO LOOP DETECTED')
    } catch (error) {
      console.error('BillingView: Failed to load available plans:', error)
      // Continue execution even if plans fail to load
    }
    
    // TESTING: Now re-enabling loadUserSubscription with circuit breaker protection
    console.log('BillingView: Testing loadUserSubscription with circuit breaker protection')
    try {
      console.log('BillingView: Loading user subscription...')
      // Add timeout to prevent hanging
      const subscriptionPromise = subscriptionStore.loadUserSubscription()
      const subscriptionTimeout = new Promise((_, reject) => 
        setTimeout(() => reject(new Error('Subscription loading timeout')), 5000)
      )
      await Promise.race([subscriptionPromise, subscriptionTimeout])
      console.log('BillingView: User subscription loaded successfully - NO LOOP DETECTED')
    } catch (error) {
      console.error('BillingView: Failed to load user subscription:', error)
      // Continue execution even if subscription fails to load
    }
    
    console.log('BillingView: Initialization complete')
  } catch (error) {
    console.error('BillingView: Critical error during initialization:', error)
  } finally {
    initializationInProgress.value = false
  }
})
</script>
