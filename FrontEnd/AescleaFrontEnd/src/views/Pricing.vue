<!-- Copyright (c) 2025 Alexander Sinapov | Simeon Petkov -->

<!-- All rights reserved. -->
<!-- This code is proprietary and confidential.   -->
<!-- Unauthorized copying, modification, distribution, or use is strictly prohibited. -->

<template>
  <div class="min-h-screen transition-colors duration-300">
    <div class="min-h-screen bg-gradient-to-br from-purple-50 via-blue-50 to-indigo-100 dark:from-gray-900 dark:via-gray-800 dark:to-gray-900">
      <!-- Navigation -->
      <SiteNavbar />

      <!-- Hero Section -->
      <section class="relative overflow-hidden">
        <div class="px-4 py-16 mx-auto max-w-7xl sm:px-6 lg:px-8">
          <div class="text-center">
            <h1 class="mb-6 text-4xl font-extrabold text-gray-900 sm:text-5xl dark:text-white">
              <span class="block">Simple, Transparent</span>
              <span class="block text-transparent bg-clip-text bg-gradient-to-r from-purple-600 to-indigo-600">
                Pricing Plans
              </span>
            </h1>
            <p class="max-w-3xl mx-auto mb-10 text-xl text-gray-600 dark:text-gray-300">
              Choose the perfect plan for your healthcare practice. No hidden fees, no surprises.
              All plans include our core features with flexible scaling options.
            </p>
          </div>
        </div>
      </section>

      <!-- Pricing Cards -->
      <section class="py-20 bg-white dark:bg-gray-800">
        <div class="px-4 mx-auto max-w-7xl sm:px-6 lg:px-8">
          <div class="grid grid-cols-1 gap-8 md:grid-cols-3">
            
            <!-- Starter Plan -->
            <div class="p-8 transition-shadow duration-300 bg-white border border-gray-200 shadow-lg dark:bg-gray-700 rounded-2xl dark:border-gray-600 hover:shadow-xl">
              <div class="text-center">
                <h3 class="mb-4 text-2xl font-bold text-gray-900 dark:text-white">Starter</h3>
                <div class="mb-6">
                  <span class="text-4xl font-extrabold text-gray-900 dark:text-white">$49</span>
                  <span class="text-gray-600 dark:text-gray-300">/month</span>
                </div>
                <p class="mb-8 text-gray-600 dark:text-gray-300">Perfect for small practices and individual practitioners getting started.</p>
              </div>
              
              <ul class="mb-8 space-y-4">
                <li class="flex items-center">
                  <svg class="w-5 h-5 mr-3 text-green-500" fill="currentColor" viewBox="0 0 20 20">
                    <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd" />
                  </svg>
                  <span class="text-gray-600 dark:text-gray-300">Unlimited patients</span>
                </li>
                <li class="flex items-center">
                  <svg class="w-5 h-5 mr-3 text-green-500" fill="currentColor" viewBox="0 0 20 20">
                    <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd" />
                  </svg>
                  <span class="text-gray-600 dark:text-gray-300">Basic AI diagnostics</span>
                </li>
                <li class="flex items-center">
                  <svg class="w-5 h-5 mr-3 text-green-500" fill="currentColor" viewBox="0 0 20 20">
                    <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd" />
                  </svg>
                  <span class="text-gray-600 dark:text-gray-300">Standard reports</span>
                </li>
                <li class="flex items-center">
                  <svg class="w-5 h-5 mr-3 text-green-500" fill="currentColor" viewBox="0 0 20 20">
                    <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd" />
                  </svg>
                  <span class="text-gray-600 dark:text-gray-300">Email support</span>
                </li>
                <li class="flex items-center">
                  <svg class="w-5 h-5 mr-3 text-green-500" fill="currentColor" viewBox="0 0 20 20">
                    <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd" />
                  </svg>
                  <span class="text-gray-600 dark:text-gray-300">5GB storage</span>
                </li>
              </ul>
              
              <button 
                @click="selectPlan('starter')"
                :disabled="subscriptionStore?.isLoading"
                class="w-full px-6 py-3 font-medium text-white transition-colors bg-gray-900 rounded-lg dark:bg-gray-600 hover:bg-gray-800 dark:hover:bg-gray-500 disabled:opacity-50"
              >
                <span v-if="subscriptionStore?.isLoading" class="flex items-center justify-center">
                  <svg class="w-5 h-5 mr-3 -ml-1 animate-spin" fill="none" viewBox="0 0 24 24">
                    <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
                    <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                  </svg>
                  Processing...
                </span>
                <span v-else>Get Started</span>
              </button>
            </div>

            <!-- Professional Plan (Most Popular) -->
            <div class="relative p-8 text-white transform scale-105 shadow-xl bg-gradient-to-br from-purple-600 to-indigo-600 rounded-2xl">
              <div class="absolute top-0 transform -translate-x-1/2 -translate-y-1/2 left-1/2">
                <span class="px-4 py-1 text-sm font-medium text-gray-900 bg-yellow-400 rounded-full">Most Popular</span>
              </div>
              
              <div class="text-center">
                <h3 class="mb-4 text-2xl font-bold">Professional</h3>
                <div class="mb-6">
                  <span class="text-4xl font-extrabold">$149</span>
                  <span class="text-purple-100">/month</span>
                </div>
                <p class="mb-8 text-purple-100">Ideal for growing practices and clinics with advanced needs.</p>
              </div>
              
              <ul class="mb-8 space-y-4">
                <li class="flex items-center">
                  <svg class="w-5 h-5 mr-3 text-green-300" fill="currentColor" viewBox="0 0 20 20">
                    <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd" />
                  </svg>
                  <span class="text-purple-100">Unlimited patients</span>
                </li>
                <li class="flex items-center">
                  <svg class="w-5 h-5 mr-3 text-green-300" fill="currentColor" viewBox="0 0 20 20">
                    <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd" />
                  </svg>
                  <span class="text-purple-100">Advanced AI diagnostics</span>
                </li>
                <li class="flex items-center">
                  <svg class="w-5 h-5 mr-3 text-green-300" fill="currentColor" viewBox="0 0 20 20">
                    <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd" />
                  </svg>
                  <span class="text-purple-100">Custom reports & analytics</span>
                </li>
                <li class="flex items-center">
                  <svg class="w-5 h-5 mr-3 text-green-300" fill="currentColor" viewBox="0 0 20 20">
                    <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd" />
                  </svg>
                  <span class="text-purple-100">Priority support</span>
                </li>
                <li class="flex items-center">
                  <svg class="w-5 h-5 mr-3 text-green-300" fill="currentColor" viewBox="0 0 20 20">
                    <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd" />
                  </svg>
                  <span class="text-purple-100">50GB storage</span>
                </li>
                <li class="flex items-center">
                  <svg class="w-5 h-5 mr-3 text-green-300" fill="currentColor" viewBox="0 0 20 20">
                    <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd" />
                  </svg>
                  <span class="text-purple-100">API access</span>
                </li>
              </ul>
              
              <button 
                @click="selectPlan('professional')"
                :disabled="subscriptionStore?.isLoading"
                class="w-full px-6 py-3 font-medium text-purple-600 transition-colors bg-white rounded-lg hover:bg-gray-50 disabled:opacity-50"
              >
                <span v-if="subscriptionStore?.isLoading" class="flex items-center justify-center">
                  <svg class="w-5 h-5 mr-3 -ml-1 animate-spin" fill="none" viewBox="0 0 24 24">
                    <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
                    <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                  </svg>
                  Processing...
                </span>
                <span v-else>Start Free Trial</span>
              </button>
            </div>

            <!-- Enterprise Plan -->
            <div class="p-8 transition-shadow duration-300 bg-white border border-gray-200 shadow-lg dark:bg-gray-700 rounded-2xl dark:border-gray-600 hover:shadow-xl">
              <div class="text-center">
                <h3 class="mb-4 text-2xl font-bold text-gray-900 dark:text-white">Enterprise</h3>
                <div class="mb-6">
                  <span class="text-4xl font-extrabold text-gray-900 dark:text-white">$399</span>
                  <span class="text-gray-600 dark:text-gray-300">/month/user</span>
                </div>
                <p class="mb-8 text-gray-600 dark:text-gray-300">For large hospitals and healthcare networks requiring maximum scalability.</p>
              </div>
              
              <ul class="mb-8 space-y-4">
                <li class="flex items-center">
                  <svg class="w-5 h-5 mr-3 text-green-500" fill="currentColor" viewBox="0 0 20 20">
                    <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd" />
                  </svg>
                  <span class="text-gray-600 dark:text-gray-300">Unlimited patients</span>
                </li>
                <li class="flex items-center">
                  <svg class="w-5 h-5 mr-3 text-green-500" fill="currentColor" viewBox="0 0 20 20">
                    <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd" />
                  </svg>
                  <span class="text-gray-600 dark:text-gray-300">Enterprise AI features</span>
                </li>
                <li class="flex items-center">
                  <svg class="w-5 h-5 mr-3 text-green-500" fill="currentColor" viewBox="0 0 20 20">
                    <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd" />
                  </svg>
                  <span class="text-gray-600 dark:text-gray-300">White-label solutions</span>
                </li>
                <li class="flex items-center">
                  <svg class="w-5 h-5 mr-3 text-green-500" fill="currentColor" viewBox="0 0 20 20">
                    <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd" />
                  </svg>
                  <span class="text-gray-600 dark:text-gray-300">24/7 dedicated support</span>
                </li>
                <li class="flex items-center">
                  <svg class="w-5 h-5 mr-3 text-green-500" fill="currentColor" viewBox="0 0 20 20">
                    <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd" />
                  </svg>
                  <span class="text-gray-600 dark:text-gray-300">Unlimited storage</span>
                </li>
                <li class="flex items-center">
                  <svg class="w-5 h-5 mr-3 text-green-500" fill="currentColor" viewBox="0 0 20 20">
                    <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd" />
                  </svg>
                  <span class="text-gray-600 dark:text-gray-300">Custom integrations</span>
                </li>
              </ul>
              
              <button 
                @click="selectPlan('enterprise')"
                :disabled="subscriptionStore?.isLoading"
                class="w-full px-6 py-3 font-medium text-white transition-colors bg-gray-900 rounded-lg dark:bg-gray-600 hover:bg-gray-800 dark:hover:bg-gray-500 disabled:opacity-50"
              >
                <span v-if="subscriptionStore?.isLoading" class="flex items-center justify-center">
                  <svg class="w-5 h-5 mr-3 -ml-1 animate-spin" fill="none" viewBox="0 0 24 24">
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
        <div class="px-4 mx-auto max-w-7xl sm:px-6 lg:px-8">
          <div class="mb-16 text-center">
            <h2 class="mb-4 text-3xl font-extrabold text-gray-900 dark:text-white">
              Compare Features
            </h2>
            <p class="max-w-3xl mx-auto text-xl text-gray-600 dark:text-gray-300">
              See what's included in each plan to find the perfect fit for your practice
            </p>
          </div>
          
          <div class="overflow-x-auto">
            <table class="w-full bg-white rounded-lg shadow-lg dark:bg-gray-800">
              <thead class="bg-gray-50 dark:bg-gray-700">
                <tr>
                  <th class="px-6 py-3 text-xs font-medium tracking-wider text-left text-gray-500 uppercase dark:text-gray-300">Feature</th>
                  <th class="px-6 py-3 text-xs font-medium tracking-wider text-center text-gray-500 uppercase dark:text-gray-300">Starter</th>
                  <th class="px-6 py-3 text-xs font-medium tracking-wider text-center text-gray-500 uppercase dark:text-gray-300">Professional</th>
                  <th class="px-6 py-3 text-xs font-medium tracking-wider text-center text-gray-500 uppercase dark:text-gray-300">Enterprise</th>
                </tr>
              </thead>
              <tbody class="divide-y divide-gray-200 dark:divide-gray-600">
                <tr>
                  <td class="px-6 py-4 text-sm font-medium text-gray-900 whitespace-nowrap dark:text-white">Patient Records</td>
                  <td class="px-6 py-4 text-center whitespace-nowrap">
                    <svg class="w-5 h-5 mx-auto text-green-500" fill="currentColor" viewBox="0 0 20 20">
                      <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd" />
                    </svg>
                  </td>
                  <td class="px-6 py-4 text-center whitespace-nowrap">
                    <svg class="w-5 h-5 mx-auto text-green-500" fill="currentColor" viewBox="0 0 20 20">
                      <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd" />
                    </svg>
                  </td>
                  <td class="px-6 py-4 text-center whitespace-nowrap">
                    <svg class="w-5 h-5 mx-auto text-green-500" fill="currentColor" viewBox="0 0 20 20">
                      <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd" />
                    </svg>
                  </td>
                </tr>
                <tr>
                  <td class="px-6 py-4 text-sm font-medium text-gray-900 whitespace-nowrap dark:text-white">AI Diagnostics</td>
                  <td class="px-6 py-4 text-sm text-center text-gray-500 whitespace-nowrap dark:text-gray-400">Basic</td>
                  <td class="px-6 py-4 text-sm text-center text-gray-500 whitespace-nowrap dark:text-gray-400">Advanced</td>
                  <td class="px-6 py-4 text-sm text-center text-gray-500 whitespace-nowrap dark:text-gray-400">Enterprise</td>
                </tr>
                <tr>
                  <td class="px-6 py-4 text-sm font-medium text-gray-900 whitespace-nowrap dark:text-white">API Access</td>
                  <td class="px-6 py-4 text-center whitespace-nowrap">
                    <svg class="w-5 h-5 mx-auto text-red-500" fill="currentColor" viewBox="0 0 20 20">
                      <path fill-rule="evenodd" d="M4.293 4.293a1 1 0 011.414 0L10 8.586l4.293-4.293a1 1 0 111.414 1.414L11.414 10l4.293 4.293a1 1 0 01-1.414 1.414L10 11.414l-4.293 4.293a1 1 0 01-1.414-1.414L8.586 10 4.293 5.707a1 1 0 010-1.414z" clip-rule="evenodd" />
                    </svg>
                  </td>
                  <td class="px-6 py-4 text-center whitespace-nowrap">
                    <svg class="w-5 h-5 mx-auto text-green-500" fill="currentColor" viewBox="0 0 20 20">
                      <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd" />
                    </svg>
                  </td>
                  <td class="px-6 py-4 text-center whitespace-nowrap">
                    <svg class="w-5 h-5 mx-auto text-green-500" fill="currentColor" viewBox="0 0 20 20">
                      <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd" />
                    </svg>
                  </td>
                </tr>
                <tr>
                  <td class="px-6 py-4 text-sm font-medium text-gray-900 whitespace-nowrap dark:text-white">Priority Support</td>
                  <td class="px-6 py-4 text-center whitespace-nowrap">
                    <svg class="w-5 h-5 mx-auto text-red-500" fill="currentColor" viewBox="0 0 20 20">
                      <path fill-rule="evenodd" d="M4.293 4.293a1 1 0 011.414 0L10 8.586l4.293-4.293a1 1 0 111.414 1.414L11.414 10l4.293 4.293a1 1 0 01-1.414 1.414L10 11.414l-4.293 4.293a1 1 0 01-1.414-1.414L8.586 10 4.293 5.707a1 1 0 010-1.414z" clip-rule="evenodd" />
                    </svg>
                  </td>
                  <td class="px-6 py-4 text-center whitespace-nowrap">
                    <svg class="w-5 h-5 mx-auto text-green-500" fill="currentColor" viewBox="0 0 20 20">
                      <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd" />
                    </svg>
                  </td>
                  <td class="px-6 py-4 text-sm text-center text-gray-500 whitespace-nowrap dark:text-gray-400">24/7</td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      </section>

      <!-- FAQ Section -->
      <section class="py-20 bg-white dark:bg-gray-800">
        <div class="max-w-4xl px-4 mx-auto sm:px-6 lg:px-8">
          <div class="mb-16 text-center">
            <h2 class="mb-4 text-3xl font-extrabold text-gray-900 dark:text-white">
              Frequently Asked Questions
            </h2>
            <p class="text-xl text-gray-600 dark:text-gray-300">
              Get answers to common questions about our pricing and plans
            </p>
          </div>
          
          <div class="space-y-8">
            <div class="p-6 rounded-lg bg-gray-50 dark:bg-gray-700">
              <h3 class="mb-2 text-lg font-semibold text-gray-900 dark:text-white">Can I change my plan later?</h3>
              <p class="text-gray-600 dark:text-gray-300">Yes, you can upgrade or downgrade your plan at any time. Changes take effect immediately, and we'll prorate the billing accordingly.</p>
            </div>
            
            <div class="p-6 rounded-lg bg-gray-50 dark:bg-gray-700">
              <h3 class="mb-2 text-lg font-semibold text-gray-900 dark:text-white">Is there a free trial available?</h3>
              <p class="text-gray-600 dark:text-gray-300">Yes, we offer a 14-day free trial for the Professional plan. No credit card required to start your trial.</p>
            </div>
            
            <div class="p-6 rounded-lg bg-gray-50 dark:bg-gray-700">
              <h3 class="mb-2 text-lg font-semibold text-gray-900 dark:text-white">What payment methods do you accept?</h3>
              <p class="text-gray-600 dark:text-gray-300">We accept all major credit cards (Visa, MasterCard, American Express) and offer annual billing options with discounts.</p>
            </div>
            
            <div class="p-6 rounded-lg bg-gray-50 dark:bg-gray-700">
              <h3 class="mb-2 text-lg font-semibold text-gray-900 dark:text-white">Is my data secure?</h3>
              <p class="text-gray-600 dark:text-gray-300">Absolutely. All plans include enterprise-grade security with HIPAA compliance, end-to-end encryption, and regular security audits.</p>
            </div>
          </div>
        </div>
      </section>

      <!-- CTA Section -->
      <section class="py-16 bg-gradient-to-r from-purple-600 to-indigo-600">
        <div class="px-4 mx-auto text-center max-w-7xl sm:px-6 lg:px-8">
          <h2 class="mb-4 text-3xl font-extrabold text-white">
            Ready to get started?
          </h2>
          <p class="max-w-2xl mx-auto mb-8 text-xl text-purple-100">
            Choose your plan and start transforming your healthcare practice today.
          </p>
          <div class="flex flex-col justify-center gap-4 sm:flex-row">
            <router-link
              to="/register"
              class="inline-flex items-center px-8 py-4 text-lg font-medium text-purple-600 transition-colors bg-white border border-transparent rounded-lg shadow-lg hover:bg-gray-50"
            >
              Start Free Trial
              <svg class="w-5 h-5 ml-2 -mr-1" fill="currentColor" viewBox="0 0 20 20">
                <path fill-rule="evenodd" d="M10.293 3.293a1 1 0 011.414 0l6 6a1 1 0 010 1.414l-6 6a1 1 0 01-1.414-1.414L14.586 11H3a1 1 0 110-2h11.586l-4.293-4.293a1 1 0 010-1.414z" clip-rule="evenodd" />
              </svg>
            </router-link>
            <router-link
              to="/contact"
              class="inline-flex items-center px-8 py-4 text-lg font-medium text-white transition-all duration-200 border-2 border-white rounded-lg hover:bg-white hover:text-purple-600"
            >
              Contact Sales
            </router-link>
          </div>
        </div>
      </section>

      <!-- Footer -->
    </div>

    <!-- Payment Modal -->
    <PaymentModal
      :is-open="showPaymentModal"
      mode="subscription"
      :selected-plan="selectedPlan"
      @close="showPaymentModal = false"
      @success="handlePaymentSuccess"
      @error="handlePaymentError"
    />

    <!-- Success Modal -->
    <div v-if="showSuccessModal" class="fixed inset-0 z-50 w-full h-full overflow-y-auto bg-gray-600 bg-opacity-50">
      <div class="relative w-11/12 max-w-md p-5 mx-auto transform -translate-y-1/2 bg-white border rounded-md shadow-lg top-1/2 dark:bg-gray-800">
        <div class="text-center">
          <div class="flex items-center justify-center w-12 h-12 mx-auto bg-green-100 rounded-full dark:bg-green-900">
            <svg class="w-6 h-6 text-green-600 dark:text-green-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 13l4 4L19 7"></path>
            </svg>
          </div>
          <h3 class="mt-4 text-lg font-medium text-gray-900 dark:text-white">Subscription Activated!</h3>
          <p class="mt-2 text-sm text-gray-500 dark:text-gray-400">
            Welcome to {{ selectedPlan?.name }}! Your subscription is now active and you have full access to all features.
          </p>
          <div class="mt-6">
            <button
              @click="closeSuccessModal"
              class="inline-flex justify-center w-full px-4 py-2 text-base font-medium text-white bg-purple-600 border border-transparent rounded-md shadow-sm hover:bg-purple-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-purple-500"
            >
              Get Started
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Footer -->
    <SiteFooter />
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '../stores/auth'
import { useSubscriptionStore } from '../stores/subscription'
import SiteNavbar from '../components/SiteNavbar.vue'
import SiteFooter from '../components/SiteFooter.vue'
import PaymentModal from '../components/modals/PaymentModal.vue'

const router = useRouter()
const authStore = useAuthStore()
const subscriptionStore = useSubscriptionStore()

const showPaymentModal = ref(false)
const selectedPlan = ref<any>(null)
const showSuccessModal = ref(false)

const selectPlan = async (planId: string) => {
  // Check if user is authenticated
  if (!authStore.isAuthenticated) {
    router.push('/register')
    return
  }

  // Check if email is verified
  if (!authStore.user?.isEmailVerified) {
    router.push(`/email-verification?email=${encodeURIComponent(authStore.user?.email || '')}`)
    return
  }

  // Find the plan details
  await subscriptionStore.loadAvailablePlans()
  const plan = subscriptionStore.availablePlans.find(p => p.id === planId)
  
  if (plan) {
    selectedPlan.value = plan
    showPaymentModal.value = true
  }
}

const handlePaymentSuccess = (paymentMethodId: string) => {
  showPaymentModal.value = false
  showSuccessModal.value = true
  console.log('Payment successful with method:', paymentMethodId)
}

const handlePaymentError = (message: string) => {
  console.error('Payment error:', message)
  // Error is already handled in PaymentModal
}

const closeSuccessModal = () => {
  showSuccessModal.value = false
  selectedPlan.value = null
  router.push('/dashboard')
}
</script>
