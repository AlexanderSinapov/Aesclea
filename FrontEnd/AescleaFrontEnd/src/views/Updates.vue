<!-- Copyright (c) 2025 Alexander Sinapov | Simeon Petkov -->

<!-- All rights reserved. -->
<!-- This code is proprietary and confidential.   -->
<!-- Unauthorized copying, modification, distribution, or use is strictly prohibited. -->

<template>
  <div class="min-h-screen transition-colors duration-300">
    <div class="min-h-screen bg-gradient-to-br from-purple-50 via-blue-50 to-indigo-100 dark:from-gray-900 dark:via-gray-800 dark:to-gray-900">
      <!-- Header -->
      <AppNavbar />

      <!-- Hero Section -->
      <section class="relative overflow-hidden">
        <div class="px-4 py-16 mx-auto max-w-7xl sm:px-6 lg:px-8">
          <div class="text-center">
            <div class="flex items-center justify-center w-20 h-20 mx-auto mb-8 rounded-full bg-gradient-to-r from-blue-500 to-purple-600">
              <svg class="w-10 h-10 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 16v1a3 3 0 003 3h10a3 3 0 003-3v-1m-4-8l-4-4m0 0L8 8m4-4v12" />
              </svg>
            </div>
            <h1 class="mb-6 text-4xl font-extrabold text-gray-900 sm:text-5xl dark:text-white">
              <span class="block">Platform Updates</span>
              <span class="block text-transparent bg-clip-text bg-gradient-to-r from-blue-600 to-purple-600">
                & Release Notes
              </span>
            </h1>
            <p class="max-w-3xl mx-auto mb-10 text-xl text-gray-600 dark:text-gray-300">
              Stay informed about the latest features, improvements, and enhancements to the Aesclea platform.
              We're constantly evolving to serve you better.
            </p>
          </div>
        </div>
      </section>

      <!-- Recent Updates -->
      <section class="py-20 bg-white dark:bg-gray-800">
        <div class="max-w-4xl px-4 mx-auto sm:px-6 lg:px-8">
          <div class="mb-16 text-center">
            <h2 class="mb-4 text-3xl font-extrabold text-gray-900 dark:text-white">
              Latest Updates
            </h2>
            <p class="text-xl text-gray-600 dark:text-gray-300">
              Recent improvements and new features
            </p>
          </div>
          
          <div class="space-y-12">
            <!-- Update 1 -->
            <div class="relative">
              <div class="absolute top-0 bottom-0 left-0 w-1 rounded-full bg-gradient-to-b from-blue-500 to-purple-600"></div>
              <div class="ml-8">
                <div class="flex items-center mb-4">
                  <div class="px-3 py-1 mr-4 text-sm font-medium text-white rounded-full bg-gradient-to-r from-blue-500 to-purple-600">
                    v2.1.0
                  </div>
                  <span class="text-gray-500 dark:text-gray-400">January 15, 2025</span>
                </div>
                <h3 class="mb-4 text-2xl font-bold text-gray-900 dark:text-white">Enhanced AI Diagnostics Engine</h3>
                <div class="p-6 mb-6 bg-gradient-to-br from-blue-50 to-purple-50 dark:from-gray-700 dark:to-gray-600 rounded-xl">
                  <h4 class="mb-3 font-semibold text-gray-900 dark:text-white">🎉 New Features</h4>
                  <ul class="space-y-2 text-gray-600 dark:text-gray-300">
                    <li class="flex items-start">
                      <svg class="w-5 h-5 text-green-500 mr-2 mt-0.5 flex-shrink-0" fill="currentColor" viewBox="0 0 20 20">
                        <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd" />
                      </svg>
                      <span>Improved neural network accuracy by 15% for tumor classification</span>
                    </li>
                    <li class="flex items-start">
                      <svg class="w-5 h-5 text-green-500 mr-2 mt-0.5 flex-shrink-0" fill="currentColor" viewBox="0 0 20 20">
                        <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd" />
                      </svg>
                      <span>New confidence scoring system with detailed explanation</span>
                    </li>
                    <li class="flex items-start">
                      <svg class="w-5 h-5 text-green-500 mr-2 mt-0.5 flex-shrink-0" fill="currentColor" viewBox="0 0 20 20">
                        <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd" />
                      </svg>
                      <span>Support for additional medical imaging formats (DICOM, NIFTI)</span>
                    </li>
                    <li class="flex items-start">
                      <svg class="w-5 h-5 text-green-500 mr-2 mt-0.5 flex-shrink-0" fill="currentColor" viewBox="0 0 20 20">
                        <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd" />
                      </svg>
                      <span>Real-time batch processing for multiple images</span>
                    </li>
                  </ul>
                </div>
                <div class="p-6 bg-gradient-to-br from-yellow-50 to-orange-50 dark:from-gray-700 dark:to-gray-600 rounded-xl">
                  <h4 class="mb-3 font-semibold text-gray-900 dark:text-white">🔧 Improvements</h4>
                  <ul class="space-y-2 text-gray-600 dark:text-gray-300">
                    <li class="flex items-start">
                      <svg class="w-5 h-5 text-blue-500 mr-2 mt-0.5 flex-shrink-0" fill="currentColor" viewBox="0 0 20 20">
                        <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd" />
                      </svg>
                      <span>Reduced processing time by 30% for image analysis</span>
                    </li>
                    <li class="flex items-start">
                      <svg class="w-5 h-5 text-blue-500 mr-2 mt-0.5 flex-shrink-0" fill="currentColor" viewBox="0 0 20 20">
                        <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd" />
                      </svg>
                      <span>Enhanced user interface for annotation tools</span>
                    </li>
                  </ul>
                </div>
              </div>
            </div>

            <!-- Update 2 -->
            <div class="relative">
              <div class="absolute top-0 bottom-0 left-0 w-1 rounded-full bg-gradient-to-b from-green-500 to-emerald-600"></div>
              <div class="ml-8">
                <div class="flex items-center mb-4">
                  <div class="px-3 py-1 mr-4 text-sm font-medium text-white rounded-full bg-gradient-to-r from-green-500 to-emerald-600">
                    v2.0.5
                  </div>
                  <span class="text-gray-500 dark:text-gray-400">December 28, 2024</span>
                </div>
                <h3 class="mb-4 text-2xl font-bold text-gray-900 dark:text-white">Security & Performance Updates</h3>
                <div class="p-6 mb-6 bg-gradient-to-br from-green-50 to-emerald-50 dark:from-gray-700 dark:to-gray-600 rounded-xl">
                  <h4 class="mb-3 font-semibold text-gray-900 dark:text-white">🔒 Security Enhancements</h4>
                  <ul class="space-y-2 text-gray-600 dark:text-gray-300">
                    <li class="flex items-start">
                      <svg class="w-5 h-5 text-green-500 mr-2 mt-0.5 flex-shrink-0" fill="currentColor" viewBox="0 0 20 20">
                        <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd" />
                      </svg>
                      <span>Enhanced two-factor authentication with TOTP support</span>
                    </li>
                    <li class="flex items-start">
                      <svg class="w-5 h-5 text-green-500 mr-2 mt-0.5 flex-shrink-0" fill="currentColor" viewBox="0 0 20 20">
                        <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd" />
                      </svg>
                      <span>Updated encryption protocols to latest standards</span>
                    </li>
                    <li class="flex items-start">
                      <svg class="w-5 h-5 text-green-500 mr-2 mt-0.5 flex-shrink-0" fill="currentColor" viewBox="0 0 20 20">
                        <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd" />
                      </svg>
                      <span>Improved audit logging and compliance reporting</span>
                    </li>
                  </ul>
                </div>
                <div class="p-6 bg-gradient-to-br from-blue-50 to-indigo-50 dark:from-gray-700 dark:to-gray-600 rounded-xl">
                  <h4 class="mb-3 font-semibold text-gray-900 dark:text-white">⚡ Performance Improvements</h4>
                  <ul class="space-y-2 text-gray-600 dark:text-gray-300">
                    <li class="flex items-start">
                      <svg class="w-5 h-5 text-blue-500 mr-2 mt-0.5 flex-shrink-0" fill="currentColor" viewBox="0 0 20 20">
                        <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd" />
                      </svg>
                      <span>Database query optimization for faster patient searches</span>
                    </li>
                    <li class="flex items-start">
                      <svg class="w-5 h-5 text-blue-500 mr-2 mt-0.5 flex-shrink-0" fill="currentColor" viewBox="0 0 20 20">
                        <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd" />
                      </svg>
                      <span>Optimized image loading and caching mechanisms</span>
                    </li>
                  </ul>
                </div>
              </div>
            </div>

            <!-- Update 3 -->
            <div class="relative">
              <div class="absolute top-0 bottom-0 left-0 w-1 rounded-full bg-gradient-to-b from-purple-500 to-pink-600"></div>
              <div class="ml-8">
                <div class="flex items-center mb-4">
                  <div class="px-3 py-1 mr-4 text-sm font-medium text-white rounded-full bg-gradient-to-r from-purple-500 to-pink-600">
                    v2.0.0
                  </div>
                  <span class="text-gray-500 dark:text-gray-400">December 10, 2024</span>
                </div>
                <h3 class="mb-4 text-2xl font-bold text-gray-900 dark:text-white">Major Platform Overhaul</h3>
                <div class="p-6 mb-6 bg-gradient-to-br from-purple-50 to-pink-50 dark:from-gray-700 dark:to-gray-600 rounded-xl">
                  <h4 class="mb-3 font-semibold text-gray-900 dark:text-white">🚀 Major Features</h4>
                  <ul class="space-y-2 text-gray-600 dark:text-gray-300">
                    <li class="flex items-start">
                      <svg class="w-5 h-5 text-purple-500 mr-2 mt-0.5 flex-shrink-0" fill="currentColor" viewBox="0 0 20 20">
                        <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd" />
                      </svg>
                      <span>Complete UI/UX redesign with modern, accessible interface</span>
                    </li>
                    <li class="flex items-start">
                      <svg class="w-5 h-5 text-purple-500 mr-2 mt-0.5 flex-shrink-0" fill="currentColor" viewBox="0 0 20 20">
                        <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd" />
                      </svg>
                      <span>New dashboard with customizable widgets and analytics</span>
                    </li>
                    <li class="flex items-start">
                      <svg class="w-5 h-5 text-purple-500 mr-2 mt-0.5 flex-shrink-0" fill="currentColor" viewBox="0 0 20 20">
                        <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd" />
                      </svg>
                      <span>Mobile-first responsive design for all devices</span>
                    </li>
                    <li class="flex items-start">
                      <svg class="w-5 h-5 text-purple-500 mr-2 mt-0.5 flex-shrink-0" fill="currentColor" viewBox="0 0 20 20">
                        <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd" />
                      </svg>
                      <span>Advanced search and filtering capabilities</span>
                    </li>
                  </ul>
                </div>
                <div class="p-6 bg-gradient-to-br from-red-50 to-pink-50 dark:from-gray-700 dark:to-gray-600 rounded-xl">
                  <h4 class="mb-3 font-semibold text-gray-900 dark:text-white">⚠️ Breaking Changes</h4>
                  <ul class="space-y-2 text-gray-600 dark:text-gray-300">
                    <li class="flex items-start">
                      <svg class="w-5 h-5 text-red-500 mr-2 mt-0.5 flex-shrink-0" fill="currentColor" viewBox="0 0 20 20">
                        <path fill-rule="evenodd" d="M8.257 3.099c.765-1.36 2.722-1.36 3.486 0l5.58 9.92c.75 1.334-.213 2.98-1.742 2.98H4.42c-1.53 0-2.493-1.646-1.743-2.98l5.58-9.92zM11 13a1 1 0 11-2 0 1 1 0 012 0zm-1-8a1 1 0 00-1 1v3a1 1 0 002 0V6a1 1 0 00-1-1z" clip-rule="evenodd" />
                      </svg>
                      <span>Legacy API endpoints deprecated (migration guide available)</span>
                    </li>
                    <li class="flex items-start">
                      <svg class="w-5 h-5 text-red-500 mr-2 mt-0.5 flex-shrink-0" fill="currentColor" viewBox="0 0 20 20">
                        <path fill-rule="evenodd" d="M8.257 3.099c.765-1.36 2.722-1.36 3.486 0l5.58 9.92c.75 1.334-.213 2.98-1.742 2.98H4.42c-1.53 0-2.493-1.646-1.743-2.98l5.58-9.92zM11 13a1 1 0 11-2 0 1 1 0 012 0zm-1-8a1 1 0 00-1 1v3a1 1 0 002 0V6a1 1 0 00-1-1z" clip-rule="evenodd" />
                      </svg>
                      <span>Updated minimum browser requirements</span>
                    </li>
                  </ul>
                </div>
              </div>
            </div>
          </div>
        </div>
      </section>

      <!-- Update Categories -->
      <section class="py-20 bg-gray-50 dark:bg-gray-900">
        <div class="px-4 mx-auto max-w-7xl sm:px-6 lg:px-8">
          <div class="mb-16 text-center">
            <h2 class="mb-4 text-3xl font-extrabold text-gray-900 dark:text-white">
              Update Categories
            </h2>
            <p class="text-xl text-gray-600 dark:text-gray-300">
              Different types of updates we regularly deliver
            </p>
          </div>
          
          <div class="grid grid-cols-1 gap-8 md:grid-cols-2 lg:grid-cols-4">
            <div class="p-6 transition-shadow bg-white shadow-lg dark:bg-gray-800 rounded-xl hover:shadow-xl">
              <div class="flex items-center justify-center w-12 h-12 mb-4 rounded-lg bg-gradient-to-r from-blue-500 to-indigo-600">
                <svg class="w-6 h-6 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 10V3L4 14h7v7l9-11h-7z" />
                </svg>
              </div>
              <h3 class="mb-2 text-lg font-semibold text-gray-900 dark:text-white">Feature Updates</h3>
              <p class="text-sm text-gray-600 dark:text-gray-300">New functionality and capabilities added to enhance your workflow.</p>
            </div>
            
            <div class="p-6 transition-shadow bg-white shadow-lg dark:bg-gray-800 rounded-xl hover:shadow-xl">
              <div class="flex items-center justify-center w-12 h-12 mb-4 rounded-lg bg-gradient-to-r from-green-500 to-emerald-600">
                <svg class="w-6 h-6 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m5.618-4.016A11.955 11.955 0 0112 2.944a11.955 11.955 0 01-8.618 3.04A12.02 12.02 0 003 9c0 5.591 3.824 10.29 9 11.622 5.176-1.332 9-6.03 9-11.622 0-1.042-.133-2.052-.382-3.016z" />
                </svg>
              </div>
              <h3 class="mb-2 text-lg font-semibold text-gray-900 dark:text-white">Security Patches</h3>
              <p class="text-sm text-gray-600 dark:text-gray-300">Critical security updates to keep your data protected and compliant.</p>
            </div>
            
            <div class="p-6 transition-shadow bg-white shadow-lg dark:bg-gray-800 rounded-xl hover:shadow-xl">
              <div class="flex items-center justify-center w-12 h-12 mb-4 rounded-lg bg-gradient-to-r from-purple-500 to-pink-600">
                <svg class="w-6 h-6 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 6V4m0 2a2 2 0 100 4m0-4a2 2 0 110 4m-6 8a2 2 0 100-4m0 4a2 2 0 100 4m0-4v2m0-6V4m6 6v10m6-2a2 2 0 100-4m0 4a2 2 0 100 4m0-4v2m0-6V4" />
                </svg>
              </div>
              <h3 class="mb-2 text-lg font-semibold text-gray-900 dark:text-white">Performance</h3>
              <p class="text-sm text-gray-600 dark:text-gray-300">Optimizations to improve speed, reliability, and system efficiency.</p>
            </div>
            
            <div class="p-6 transition-shadow bg-white shadow-lg dark:bg-gray-800 rounded-xl hover:shadow-xl">
              <div class="flex items-center justify-center w-12 h-12 mb-4 rounded-lg bg-gradient-to-r from-orange-500 to-red-600">
                <svg class="w-6 h-6 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 9v3m0 0v3m0-3h3m-3 0H9m12 0a9 9 0 11-18 0 9 9 0 0118 0z" />
                </svg>
              </div>
              <h3 class="mb-2 text-lg font-semibold text-gray-900 dark:text-white">Bug Fixes</h3>
              <p class="text-sm text-gray-600 dark:text-gray-300">Resolving issues and improving overall system stability and reliability.</p>
            </div>
          </div>
        </div>
      </section>

      <!-- Roadmap -->
      <section class="py-20 bg-white dark:bg-gray-800">
        <div class="px-4 mx-auto max-w-7xl sm:px-6 lg:px-8">
          <div class="mb-16 text-center">
            <h2 class="mb-4 text-3xl font-extrabold text-gray-900 dark:text-white">
              What's Coming Next
            </h2>
            <p class="text-xl text-gray-600 dark:text-gray-300">
              Upcoming features and improvements in our development pipeline
            </p>
          </div>
          
          <div class="grid grid-cols-1 gap-8 md:grid-cols-3">
            <div class="relative p-8 overflow-hidden bg-gradient-to-br from-blue-50 to-indigo-50 dark:from-gray-700 dark:to-gray-600 rounded-xl">
              <div class="absolute px-3 py-1 text-sm font-medium text-white bg-blue-500 rounded-full top-4 right-4">
                Q1 2025
              </div>
              <div class="flex items-center justify-center w-16 h-16 mb-6 bg-blue-500 rounded-xl">
                <svg class="w-8 h-8 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9.663 17h4.673M12 3v1m6.364 1.636l-.707.707M21 12h-1M4 12H3m3.343-5.657l-.707-.707m2.828 9.9a5 5 0 117.072 0l-.548.547A3.374 3.374 0 0014 18.469V19a2 2 0 11-4 0v-.531c0-.895-.356-1.754-.988-2.386l-.548-.547z" />
                </svg>
              </div>
              <h3 class="mb-4 text-2xl font-bold text-gray-900 dark:text-white">Advanced AI Models</h3>
              <p class="mb-6 text-gray-600 dark:text-gray-300">Enhanced machine learning capabilities with support for more medical conditions and improved accuracy.</p>
              <ul class="space-y-2 text-sm text-gray-600 dark:text-gray-300">
                <li class="flex items-center">
                  <div class="w-2 h-2 mr-2 bg-blue-500 rounded-full"></div>
                  Multi-organ tumor detection
                </li>
                <li class="flex items-center">
                  <div class="w-2 h-2 mr-2 bg-blue-500 rounded-full"></div>
                  Predictive analytics
                </li>
                <li class="flex items-center">
                  <div class="w-2 h-2 mr-2 bg-blue-500 rounded-full"></div>
                  Real-time risk assessment
                </li>
              </ul>
            </div>
            
            <div class="relative p-8 overflow-hidden bg-gradient-to-br from-green-50 to-emerald-50 dark:from-gray-700 dark:to-gray-600 rounded-xl">
              <div class="absolute px-3 py-1 text-sm font-medium text-white bg-green-500 rounded-full top-4 right-4">
                Q2 2025
              </div>
              <div class="flex items-center justify-center w-16 h-16 mb-6 bg-green-500 rounded-xl">
                <svg class="w-8 h-8 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 18h.01M8 21h8a2 2 0 002-2V5a2 2 0 00-2-2H8a2 2 0 00-2 2v14a2 2 0 002 2z" />
                </svg>
              </div>
              <h3 class="mb-4 text-2xl font-bold text-gray-900 dark:text-white">Mobile App</h3>
              <p class="mb-6 text-gray-600 dark:text-gray-300">Native mobile applications for iOS and Android with offline capabilities and push notifications.</p>
              <ul class="space-y-2 text-sm text-gray-600 dark:text-gray-300">
                <li class="flex items-center">
                  <div class="w-2 h-2 mr-2 bg-green-500 rounded-full"></div>
                  Offline patient records
                </li>
                <li class="flex items-center">
                  <div class="w-2 h-2 mr-2 bg-green-500 rounded-full"></div>
                  Push notifications
                </li>
                <li class="flex items-center">
                  <div class="w-2 h-2 mr-2 bg-green-500 rounded-full"></div>
                  Biometric authentication
                </li>
              </ul>
            </div>
            
            <div class="relative p-8 overflow-hidden bg-gradient-to-br from-purple-50 to-pink-50 dark:from-gray-700 dark:to-gray-600 rounded-xl">
              <div class="absolute px-3 py-1 text-sm font-medium text-white bg-purple-500 rounded-full top-4 right-4">
                Q3 2025
              </div>
              <div class="flex items-center justify-center w-16 h-16 mb-6 bg-purple-500 rounded-xl">
                <svg class="w-8 h-8 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8.111 16.404a5.5 5.5 0 017.778 0M12 20h.01m-7.08-7.071c3.904-3.905 10.236-3.905 14.141 0M1.394 9.393c5.857-5.857 15.355-5.857 21.213 0" />
                </svg>
              </div>
              <h3 class="mb-4 text-2xl font-bold text-gray-900 dark:text-white">API Expansion</h3>
              <p class="mb-6 text-gray-600 dark:text-gray-300">Comprehensive API suite for third-party integrations and custom healthcare solutions.</p>
              <ul class="space-y-2 text-sm text-gray-600 dark:text-gray-300">
                <li class="flex items-center">
                  <div class="w-2 h-2 mr-2 bg-purple-500 rounded-full"></div>
                  HL7 FHIR compliance
                </li>
                <li class="flex items-center">
                  <div class="w-2 h-2 mr-2 bg-purple-500 rounded-full"></div>
                  Webhook integrations
                </li>
                <li class="flex items-center">
                  <div class="w-2 h-2 mr-2 bg-purple-500 rounded-full"></div>
                  GraphQL endpoints
                </li>
              </ul>
            </div>
          </div>
        </div>
      </section>

      <!-- Newsletter Signup -->
      <section class="py-20 bg-gradient-to-r from-blue-600 to-purple-600">
        <div class="max-w-4xl px-4 mx-auto text-center sm:px-6 lg:px-8">
          <h2 class="mb-4 text-3xl font-extrabold text-white">
            Never Miss an Update
          </h2>
          <p class="mb-8 text-xl text-blue-100">
            Subscribe to our newsletter to get notified about new features, security updates, and important announcements.
          </p>
          
          <div class="max-w-md mx-auto">
            <div class="flex">
              <input
                type="email"
                placeholder="Enter your email address"
                class="flex-1 px-4 py-3 border-0 rounded-l-lg focus:outline-none focus:ring-2 focus:ring-white focus:ring-opacity-50"
              />
              <button class="px-6 py-3 font-medium text-blue-600 transition-colors bg-white rounded-r-lg hover:bg-gray-50">
                Subscribe
              </button>
            </div>
            <p class="mt-3 text-sm text-blue-100">
              We respect your privacy. Unsubscribe at any time.
            </p>
          </div>
        </div>
      </section>

      <!-- Footer -->
      <footer class="py-12 text-white bg-gray-900">
        <div class="px-4 mx-auto max-w-7xl sm:px-6 lg:px-8">
          <div class="grid grid-cols-1 gap-8 md:grid-cols-4">
            <div class="col-span-1 md:col-span-2">
              <div class="flex items-center mb-4">
                <div class="flex items-center justify-center w-8 h-8 rounded-full bg-gradient-to-r from-purple-600 to-indigo-600">
                  <svg class="w-5 h-5 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4.318 6.318a4.5 4.5 0 000 6.364L12 20.364l7.682-7.682a4.5 4.5 0 00-6.364-6.364L12 7.636l-1.318-1.318a4.5 4.5 0 00-6.364 0z" />
                  </svg>
                </div>
                <h3 class="ml-2 text-xl font-bold">Aesclea</h3>
              </div>
              <p class="max-w-md mb-4 text-gray-400">
                Empowering healthcare professionals with modern, efficient, and secure practice management solutions.
              </p>
            </div>
            
            <div>
              <h4 class="mb-4 text-lg font-semibold">Product</h4>
              <ul class="space-y-2 text-gray-400">
                <li><router-link to="/features" class="transition-colors hover:text-white">Features</router-link></li>
                <li><router-link to="/pricing" class="transition-colors hover:text-white">Pricing</router-link></li>
                <li><router-link to="/security" class="transition-colors hover:text-white">Security</router-link></li>
                <li><router-link to="/updates" class="transition-colors hover:text-white">Updates</router-link></li>
              </ul>
            </div>
            
            <div>
              <h4 class="mb-4 text-lg font-semibold">Support</h4>
              <ul class="space-y-2 text-gray-400">
                <li><router-link to="/help" class="transition-colors hover:text-white">Help Center</router-link></li>
                <li><router-link to="/contact" class="transition-colors hover:text-white">Contact Us</router-link></li>
                <li><router-link to="/privacy" class="transition-colors hover:text-white">Privacy Policy</router-link></li>
                <li><router-link to="/terms" class="transition-colors hover:text-white">Terms of Service</router-link></li>
              </ul>
            </div>
          </div>
          
          <div class="pt-8 mt-8 text-center text-gray-400 border-t border-gray-800">
            <p>&copy; 2025 Aesclea Medical Management System. All rights reserved.</p>
          </div>
        </div>
      </footer>
    </div>
  </div>
</template>

<script setup lang="ts">
import { onMounted } from 'vue'
import { useTheme } from '../composables/useTheme'
import AppNavbar from '../components/AppNavbar.vue'

const { isDark, toggleTheme, initializeTheme } = useTheme()

onMounted(() => {
  initializeTheme()
})
</script>
