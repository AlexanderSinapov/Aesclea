<template>
  <div class="min-h-screen transition-colors duration-300">
    <div class="min-h-screen bg-gradient-to-br from-purple-50 via-blue-50 to-indigo-100 dark:from-gray-900 dark:via-gray-800 dark:to-gray-900">
      <!-- Use AppNavbar for consistency -->
      <AppNavbar />

      <!-- Hero Section -->
      <section class="relative overflow-hidden">
        <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-16">
          <div class="text-center">
            <h1 class="text-4xl sm:text-5xl font-extrabold text-gray-900 dark:text-white mb-6">
              <span class="block">Simple, Transparent</span>
              <span class="block text-transparent bg-clip-text bg-gradient-to-r from-purple-600 to-indigo-600">
                Pricing Plans
              </span>
            </h1>
            <p class="max-w-3xl mx-auto text-xl text-gray-600 dark:text-gray-300 mb-10">
              Choose the perfect plan for your healthcare practice. No hidden fees, no surprises.
              All plans include our core features with flexible scaling options.
            </p>
          </div>
        </div>
      </section>

      <!-- Pricing Cards -->
      <section class="py-20 bg-white dark:bg-gray-800">
        <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div class="grid grid-cols-1 md:grid-cols-3 gap-8">
            
            <!-- Starter Plan -->
            <div class="bg-white dark:bg-gray-700 rounded-2xl shadow-lg p-8 border border-gray-200 dark:border-gray-600 hover:shadow-xl transition-shadow duration-300">
              <div class="text-center">
                <h3 class="text-2xl font-bold text-gray-900 dark:text-white mb-4">Starter</h3>
                <div class="mb-6">
                  <span class="text-4xl font-extrabold text-gray-900 dark:text-white">$49</span>
                  <span class="text-gray-600 dark:text-gray-300">/month</span>
                </div>
                <p class="text-gray-600 dark:text-gray-300 mb-8">Perfect for small practices and individual practitioners getting started.</p>
              </div>
              
              <ul class="space-y-4 mb-8">
                <li class="flex items-center">
                  <svg class="w-5 h-5 text-green-500 mr-3" fill="currentColor" viewBox="0 0 20 20">
                    <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd" />
                  </svg>
                  <span class="text-gray-600 dark:text-gray-300">Up to 50 patients</span>
                </li>
                <li class="flex items-center">
                  <svg class="w-5 h-5 text-green-500 mr-3" fill="currentColor" viewBox="0 0 20 20">
                    <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd" />
                  </svg>
                  <span class="text-gray-600 dark:text-gray-300">Basic AI diagnostics</span>
                </li>
                <li class="flex items-center">
                  <svg class="w-5 h-5 text-green-500 mr-3" fill="currentColor" viewBox="0 0 20 20">
                    <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd" />
                  </svg>
                  <span class="text-gray-600 dark:text-gray-300">Standard reports</span>
                </li>
                <li class="flex items-center">
                  <svg class="w-5 h-5 text-green-500 mr-3" fill="currentColor" viewBox="0 0 20 20">
                    <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd" />
                  </svg>
                  <span class="text-gray-600 dark:text-gray-300">Email support</span>
                </li>
                <li class="flex items-center">
                  <svg class="w-5 h-5 text-green-500 mr-3" fill="currentColor" viewBox="0 0 20 20">
                    <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd" />
                  </svg>
                  <span class="text-gray-600 dark:text-gray-300">5GB storage</span>
                </li>
              </ul>
              
              <button 
                @click="selectPlan('starter')"
                :disabled="subscriptionStore?.isLoading"
                class="w-full bg-gray-900 dark:bg-gray-600 text-white py-3 px-6 rounded-lg font-medium hover:bg-gray-800 dark:hover:bg-gray-500 transition-colors disabled:opacity-50"
              >
                <span v-if="subscriptionStore?.isLoading" class="flex items-center justify-center">
                  <svg class="animate-spin -ml-1 mr-3 h-5 w-5" fill="none" viewBox="0 0 24 24">
                    <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
                    <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                  </svg>
                  Processing...
                </span>
                <span v-else>Get Started</span>
              </button>
            </div>

            <!-- Professional Plan (Most Popular) -->
            <div class="bg-gradient-to-br from-purple-600 to-indigo-600 rounded-2xl shadow-xl p-8 text-white relative transform scale-105">
              <div class="absolute top-0 left-1/2 transform -translate-x-1/2 -translate-y-1/2">
                <span class="bg-yellow-400 text-gray-900 px-4 py-1 rounded-full text-sm font-medium">Most Popular</span>
              </div>
              
              <div class="text-center">
                <h3 class="text-2xl font-bold mb-4">Professional</h3>
                <div class="mb-6">
                  <span class="text-4xl font-extrabold">$149</span>
                  <span class="text-purple-100">/month</span>
                </div>
                <p class="text-purple-100 mb-8">Ideal for growing practices and clinics with advanced needs.</p>
              </div>
              
              <ul class="space-y-4 mb-8">
                <li class="flex items-center">
                  <svg class="w-5 h-5 text-green-300 mr-3" fill="currentColor" viewBox="0 0 20 20">
                    <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd" />
                  </svg>
                  <span class="text-purple-100">Up to 500 patients</span>
                </li>
                <li class="flex items-center">
                  <svg class="w-5 h-5 text-green-300 mr-3" fill="currentColor" viewBox="0 0 20 20">
                    <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd" />
                  </svg>
                  <span class="text-purple-100">Advanced AI diagnostics</span>
                </li>
                <li class="flex items-center">
                  <svg class="w-5 h-5 text-green-300 mr-3" fill="currentColor" viewBox="0 0 20 20">
                    <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd" />
                  </svg>
                  <span class="text-purple-100">Custom reports & analytics</span>
                </li>
                <li class="flex items-center">
                  <svg class="w-5 h-5 text-green-300 mr-3" fill="currentColor" viewBox="0 0 20 20">
                    <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd" />
                  </svg>
                  <span class="text-purple-100">Priority support</span>
                </li>
                <li class="flex items-center">
                  <svg class="w-5 h-5 text-green-300 mr-3" fill="currentColor" viewBox="0 0 20 20">
                    <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd" />
                  </svg>
                  <span class="text-purple-100">50GB storage</span>
                </li>
                <li class="flex items-center">
                  <svg class="w-5 h-5 text-green-300 mr-3" fill="currentColor" viewBox="0 0 20 20">
                    <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd" />
                  </svg>
                  <span class="text-purple-100">API access</span>
                </li>
              </ul>
              
              <button 
                @click="selectPlan('professional')"
                :disabled="subscriptionStore?.isLoading"
                class="w-full bg-white text-purple-600 py-3 px-6 rounded-lg font-medium hover:bg-gray-50 transition-colors disabled:opacity-50"
              >
                <span v-if="subscriptionStore?.isLoading" class="flex items-center justify-center">
                  <svg class="animate-spin -ml-1 mr-3 h-5 w-5" fill="none" viewBox="0 0 24 24">
                    <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
                    <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                  </svg>
                  Processing...
                </span>
                <span v-else>Start Free Trial</span>
              </button>
            </div>

            <!-- Enterprise Plan -->
            <div class="bg-white dark:bg-gray-700 rounded-2xl shadow-lg p-8 border border-gray-200 dark:border-gray-600 hover:shadow-xl transition-shadow duration-300">
              <div class="text-center">
                <h3 class="text-2xl font-bold text-gray-900 dark:text-white mb-4">Enterprise</h3>
                <div class="mb-6">
                  <span class="text-4xl font-extrabold text-gray-900 dark:text-white">$399</span>
                  <span class="text-gray-600 dark:text-gray-300">/month</span>
                </div>
                <p class="text-gray-600 dark:text-gray-300 mb-8">For large hospitals and healthcare networks requiring maximum scalability.</p>
              </div>
              
              <ul class="space-y-4 mb-8">
                <li class="flex items-center">
                  <svg class="w-5 h-5 text-green-500 mr-3" fill="currentColor" viewBox="0 0 20 20">
                    <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd" />
                  </svg>
                  <span class="text-gray-600 dark:text-gray-300">Unlimited patients</span>
                </li>
                <li class="flex items-center">
                  <svg class="w-5 h-5 text-green-500 mr-3" fill="currentColor" viewBox="0 0 20 20">
                    <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd" />
                  </svg>
                  <span class="text-gray-600 dark:text-gray-300">Enterprise AI features</span>
                </li>
                <li class="flex items-center">
                  <svg class="w-5 h-5 text-green-500 mr-3" fill="currentColor" viewBox="0 0 20 20">
                    <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd" />
                  </svg>
                  <span class="text-gray-600 dark:text-gray-300">White-label solutions</span>
                </li>
                <li class="flex items-center">
                  <svg class="w-5 h-5 text-green-500 mr-3" fill="currentColor" viewBox="0 0 20 20">
                    <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd" />
                  </svg>
                  <span class="text-gray-600 dark:text-gray-300">24/7 dedicated support</span>
                </li>
                <li class="flex items-center">
                  <svg class="w-5 h-5 text-green-500 mr-3" fill="currentColor" viewBox="0 0 20 20">
                    <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd" />
                  </svg>
                  <span class="text-gray-600 dark:text-gray-300">Unlimited storage</span>
                </li>
                <li class="flex items-center">
                  <svg class="w-5 h-5 text-green-500 mr-3" fill="currentColor" viewBox="0 0 20 20">
                    <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd" />
                  </svg>
                  <span class="text-gray-600 dark:text-gray-300">Custom integrations</span>
                </li>
              </ul>
              
              <button 
                @click="selectPlan('enterprise')"
                :disabled="subscriptionStore?.isLoading"
                class="w-full bg-gray-900 dark:bg-gray-600 text-white py-3 px-6 rounded-lg font-medium hover:bg-gray-800 dark:hover:bg-gray-500 transition-colors disabled:opacity-50"
              >
                <span v-if="subscriptionStore?.isLoading" class="flex items-center justify-center">
                  <svg class="animate-spin -ml-1 mr-3 h-5 w-5" fill="none" viewBox="0 0 24 24">
                    <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
                    <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                  </svg>
                  Processing...
                </span>
                <span v-else>Contact Sales</span>
              </button>
            </div>

          </div>
        </div>
      </section>

      <!-- Features Comparison -->
      <section class="py-20 bg-gray-50 dark:bg-gray-900">
        <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div class="text-center mb-16">
            <h2 class="text-3xl font-extrabold text-gray-900 dark:text-white mb-4">
              Compare Features
            </h2>
            <p class="text-xl text-gray-600 dark:text-gray-300 max-w-3xl mx-auto">
              See what's included in each plan to find the perfect fit for your practice
            </p>
          </div>
          
          <div class="overflow-x-auto">
            <table class="w-full bg-white dark:bg-gray-800 rounded-lg shadow-lg">
              <thead class="bg-gray-50 dark:bg-gray-700">
                <tr>
                  <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">Feature</th>
                  <th class="px-6 py-3 text-center text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">Starter</th>
                  <th class="px-6 py-3 text-center text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">Professional</th>
                  <th class="px-6 py-3 text-center text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">Enterprise</th>
                </tr>
              </thead>
              <tbody class="divide-y divide-gray-200 dark:divide-gray-600">
                <tr>
                  <td class="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900 dark:text-white">Patient Records</td>
                  <td class="px-6 py-4 whitespace-nowrap text-center">
                    <svg class="w-5 h-5 text-green-500 mx-auto" fill="currentColor" viewBox="0 0 20 20">
                      <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd" />
                    </svg>
                  </td>
                  <td class="px-6 py-4 whitespace-nowrap text-center">
                    <svg class="w-5 h-5 text-green-500 mx-auto" fill="currentColor" viewBox="0 0 20 20">
                      <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd" />
                    </svg>
                  </td>
                  <td class="px-6 py-4 whitespace-nowrap text-center">
                    <svg class="w-5 h-5 text-green-500 mx-auto" fill="currentColor" viewBox="0 0 20 20">
                      <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd" />
                    </svg>
                  </td>
                </tr>
                <tr>
                  <td class="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900 dark:text-white">AI Diagnostics</td>
                  <td class="px-6 py-4 whitespace-nowrap text-center text-sm text-gray-500 dark:text-gray-400">Basic</td>
                  <td class="px-6 py-4 whitespace-nowrap text-center text-sm text-gray-500 dark:text-gray-400">Advanced</td>
                  <td class="px-6 py-4 whitespace-nowrap text-center text-sm text-gray-500 dark:text-gray-400">Enterprise</td>
                </tr>
                <tr>
                  <td class="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900 dark:text-white">API Access</td>
                  <td class="px-6 py-4 whitespace-nowrap text-center">
                    <svg class="w-5 h-5 text-red-500 mx-auto" fill="currentColor" viewBox="0 0 20 20">
                      <path fill-rule="evenodd" d="M4.293 4.293a1 1 0 011.414 0L10 8.586l4.293-4.293a1 1 0 111.414 1.414L11.414 10l4.293 4.293a1 1 0 01-1.414 1.414L10 11.414l-4.293 4.293a1 1 0 01-1.414-1.414L8.586 10 4.293 5.707a1 1 0 010-1.414z" clip-rule="evenodd" />
                    </svg>
                  </td>
                  <td class="px-6 py-4 whitespace-nowrap text-center">
                    <svg class="w-5 h-5 text-green-500 mx-auto" fill="currentColor" viewBox="0 0 20 20">
                      <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd" />
                    </svg>
                  </td>
                  <td class="px-6 py-4 whitespace-nowrap text-center">
                    <svg class="w-5 h-5 text-green-500 mx-auto" fill="currentColor" viewBox="0 0 20 20">
                      <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd" />
                    </svg>
                  </td>
                </tr>
                <tr>
                  <td class="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900 dark:text-white">Priority Support</td>
                  <td class="px-6 py-4 whitespace-nowrap text-center">
                    <svg class="w-5 h-5 text-red-500 mx-auto" fill="currentColor" viewBox="0 0 20 20">
                      <path fill-rule="evenodd" d="M4.293 4.293a1 1 0 011.414 0L10 8.586l4.293-4.293a1 1 0 111.414 1.414L11.414 10l4.293 4.293a1 1 0 01-1.414 1.414L10 11.414l-4.293 4.293a1 1 0 01-1.414-1.414L8.586 10 4.293 5.707a1 1 0 010-1.414z" clip-rule="evenodd" />
                    </svg>
                  </td>
                  <td class="px-6 py-4 whitespace-nowrap text-center">
                    <svg class="w-5 h-5 text-green-500 mx-auto" fill="currentColor" viewBox="0 0 20 20">
                      <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd" />
                    </svg>
                  </td>
                  <td class="px-6 py-4 whitespace-nowrap text-center text-sm text-gray-500 dark:text-gray-400">24/7</td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      </section>

      <!-- FAQ Section -->
      <section class="py-20 bg-white dark:bg-gray-800">
        <div class="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8">
          <div class="text-center mb-16">
            <h2 class="text-3xl font-extrabold text-gray-900 dark:text-white mb-4">
              Frequently Asked Questions
            </h2>
            <p class="text-xl text-gray-600 dark:text-gray-300">
              Get answers to common questions about our pricing and plans
            </p>
          </div>
          
          <div class="space-y-8">
            <div class="bg-gray-50 dark:bg-gray-700 rounded-lg p-6">
              <h3 class="text-lg font-semibold text-gray-900 dark:text-white mb-2">Can I change my plan later?</h3>
              <p class="text-gray-600 dark:text-gray-300">Yes, you can upgrade or downgrade your plan at any time. Changes take effect immediately, and we'll prorate the billing accordingly.</p>
            </div>
            
            <div class="bg-gray-50 dark:bg-gray-700 rounded-lg p-6">
              <h3 class="text-lg font-semibold text-gray-900 dark:text-white mb-2">Is there a free trial available?</h3>
              <p class="text-gray-600 dark:text-gray-300">Yes, we offer a 14-day free trial for the Professional plan. No credit card required to start your trial.</p>
            </div>
            
            <div class="bg-gray-50 dark:bg-gray-700 rounded-lg p-6">
              <h3 class="text-lg font-semibold text-gray-900 dark:text-white mb-2">What payment methods do you accept?</h3>
              <p class="text-gray-600 dark:text-gray-300">We accept all major credit cards (Visa, MasterCard, American Express) and offer annual billing options with discounts.</p>
            </div>
            
            <div class="bg-gray-50 dark:bg-gray-700 rounded-lg p-6">
              <h3 class="text-lg font-semibold text-gray-900 dark:text-white mb-2">Is my data secure?</h3>
              <p class="text-gray-600 dark:text-gray-300">Absolutely. All plans include enterprise-grade security with HIPAA compliance, end-to-end encryption, and regular security audits.</p>
            </div>
          </div>
        </div>
      </section>

      <!-- CTA Section -->
      <section class="bg-gradient-to-r from-purple-600 to-indigo-600 py-16">
        <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 text-center">
          <h2 class="text-3xl font-extrabold text-white mb-4">
            Ready to get started?
          </h2>
          <p class="text-xl text-purple-100 mb-8 max-w-2xl mx-auto">
            Choose your plan and start transforming your healthcare practice today.
          </p>
          <div class="flex flex-col sm:flex-row gap-4 justify-center">
            <router-link
              to="/register"
              class="inline-flex items-center px-8 py-4 border border-transparent text-lg font-medium rounded-lg text-purple-600 bg-white hover:bg-gray-50 transition-colors shadow-lg"
            >
              Start Free Trial
              <svg class="ml-2 -mr-1 w-5 h-5" fill="currentColor" viewBox="0 0 20 20">
                <path fill-rule="evenodd" d="M10.293 3.293a1 1 0 011.414 0l6 6a1 1 0 010 1.414l-6 6a1 1 0 01-1.414-1.414L14.586 11H3a1 1 0 110-2h11.586l-4.293-4.293a1 1 0 010-1.414z" clip-rule="evenodd" />
              </svg>
            </router-link>
            <router-link
              to="/contact"
              class="inline-flex items-center px-8 py-4 border-2 border-white text-lg font-medium rounded-lg text-white hover:bg-white hover:text-purple-600 transition-all duration-200"
            >
              Contact Sales
            </router-link>
          </div>
        </div>
      </section>

      <!-- Footer -->
      <footer class="bg-gray-900 text-white py-12">
        <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div class="grid grid-cols-1 md:grid-cols-4 gap-8">
            <div class="col-span-1 md:col-span-2">
              <div class="flex items-center mb-4">
                <div class="h-8 w-8 bg-gradient-to-r from-purple-600 to-indigo-600 rounded-full flex items-center justify-center">
                  <svg class="h-5 w-5 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4.318 6.318a4.5 4.5 0 000 6.364L12 20.364l7.682-7.682a4.5 4.5 0 00-6.364-6.364L12 7.636l-1.318-1.318a4.5 4.5 0 00-6.364 0z" />
                  </svg>
                </div>
                <h3 class="ml-2 text-xl font-bold">Aesclea</h3>
              </div>
              <p class="text-gray-400 mb-4 max-w-md">
                Empowering healthcare professionals with modern, efficient, and secure practice management solutions.
              </p>
            </div>
            
            <div>
              <h4 class="text-lg font-semibold mb-4">Product</h4>
              <ul class="space-y-2 text-gray-400">
                <li><router-link to="/features" class="hover:text-white transition-colors">Features</router-link></li>
                <li><router-link to="/pricing" class="hover:text-white transition-colors">Pricing</router-link></li>
                <li><router-link to="/security" class="hover:text-white transition-colors">Security</router-link></li>
                <li><router-link to="/updates" class="hover:text-white transition-colors">Updates</router-link></li>
              </ul>
            </div>
            
            <div>
              <h4 class="text-lg font-semibold mb-4">Support</h4>
              <ul class="space-y-2 text-gray-400">
                <li><router-link to="/help" class="hover:text-white transition-colors">Help Center</router-link></li>
                <li><router-link to="/contact" class="hover:text-white transition-colors">Contact Us</router-link></li>
                <li><router-link to="/privacy" class="hover:text-white transition-colors">Privacy Policy</router-link></li>
                <li><router-link to="/terms" class="hover:text-white transition-colors">Terms of Service</router-link></li>
              </ul>
            </div>
          </div>
          
          <div class="border-t border-gray-800 mt-8 pt-8 text-center text-gray-400">
            <p>&copy; 2025 Aesclea Medical Management System. All rights reserved.</p>
          </div>
        </div>
      </footer>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useRouter } from 'vue-router'
import { useAuthStore } from '../stores/auth'
import { useSubscriptionStore } from '../stores/subscription'
import AppNavbar from '../components/AppNavbar.vue'

const router = useRouter()
const authStore = useAuthStore()
const subscriptionStore = useSubscriptionStore()

const selectPlan = async (planId: string) => {
  // Check if user is authenticated
  if (!authStore.isAuthenticated) {
    router.push('/register')
    return
  }

  // Check if email is verified
  if (!authStore.user?.emailVerified) {
    router.push(`/email-verification?email=${encodeURIComponent(authStore.user?.email || '')}`)
    return
  }

  try {
    await subscriptionStore.subscribeToPlan(planId)
    router.push('/dashboard')
  } catch (error) {
    console.error('Subscription failed:', error)
    // Handle error - could show a toast or modal
  }
}
</script>
