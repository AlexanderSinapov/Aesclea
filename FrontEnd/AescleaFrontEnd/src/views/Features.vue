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
              <span class="block">Powerful Features for</span>
              <span class="block text-transparent bg-clip-text bg-gradient-to-r from-purple-600 to-indigo-600">
                Modern Healthcare
              </span>
            </h1>
            <p class="max-w-3xl mx-auto mb-10 text-xl text-gray-600 dark:text-gray-300">
              Discover the comprehensive suite of tools designed to streamline your medical practice 
              and enhance patient care with cutting-edge technology.
            </p>
          </div>
        </div>
      </section>

      <!-- Main Features Grid -->
      <section class="py-20 bg-white dark:bg-gray-800">
        <div class="px-4 mx-auto max-w-7xl sm:px-6 lg:px-8">
          <div class="grid grid-cols-1 gap-8 md:grid-cols-2 lg:grid-cols-3">
            <!-- Feature 1: AI-Powered Diagnostics -->
            <div class="p-8 transition-all duration-300 transform bg-gradient-to-br from-blue-50 to-indigo-50 dark:from-gray-700 dark:to-gray-600 rounded-2xl hover:shadow-xl hover:-translate-y-1">
              <div class="flex items-center justify-center w-16 h-16 mb-6 bg-blue-100 dark:bg-blue-900 rounded-xl">
                <svg class="w-8 h-8 text-blue-600 dark:text-blue-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9.663 17h4.673M12 3v1m6.364 1.636l-.707.707M21 12h-1M4 12H3m3.343-5.657l-.707-.707m2.828 9.9a5 5 0 117.072 0l-.548.547A3.374 3.374 0 0014 18.469V19a2 2 0 11-4 0v-.531c0-.895-.356-1.754-.988-2.386l-.548-.547z" />
                </svg>
              </div>
              <h3 class="mb-4 text-2xl font-bold text-gray-900 dark:text-white">AI-Powered Diagnostics</h3>
              <p class="mb-6 text-gray-600 dark:text-gray-300">Advanced machine learning algorithms assist in tumor detection and medical diagnosis with high accuracy rates.</p>
              <ul class="space-y-2 text-sm text-gray-600 dark:text-gray-300">
                <li class="flex items-center">
                  <svg class="w-4 h-4 mr-2 text-green-500" fill="currentColor" viewBox="0 0 20 20">
                    <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd" />
                  </svg>
                  Neural network-based tumor classification
                </li>
                <li class="flex items-center">
                  <svg class="w-4 h-4 mr-2 text-green-500" fill="currentColor" viewBox="0 0 20 20">
                    <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd" />
                  </svg>
                  Real-time analysis results
                </li>
                <li class="flex items-center">
                  <svg class="w-4 h-4 mr-2 text-green-500" fill="currentColor" viewBox="0 0 20 20">
                    <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd" />
                  </svg>
                  Confidence scoring system
                </li>
              </ul>
            </div>

            <!-- Feature 2: Patient Management -->
            <div class="p-8 transition-all duration-300 transform bg-gradient-to-br from-green-50 to-emerald-50 dark:from-gray-700 dark:to-gray-600 rounded-2xl hover:shadow-xl hover:-translate-y-1">
              <div class="flex items-center justify-center w-16 h-16 mb-6 bg-green-100 dark:bg-green-900 rounded-xl">
                <svg class="w-8 h-8 text-green-600 dark:text-green-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857M7 20H2v-2a3 3 0 015.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0zm6 3a2 2 0 11-4 0 2 2 0 014 0zM7 10a2 2 0 11-4 0 2 2 0 014 0z" />
                </svg>
              </div>
              <h3 class="mb-4 text-2xl font-bold text-gray-900 dark:text-white">Patient Management</h3>
              <p class="mb-6 text-gray-600 dark:text-gray-300">Comprehensive patient records system with intuitive search and organization capabilities.</p>
              <ul class="space-y-2 text-sm text-gray-600 dark:text-gray-300">
                <li class="flex items-center">
                  <svg class="w-4 h-4 mr-2 text-green-500" fill="currentColor" viewBox="0 0 20 20">
                    <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd" />
                  </svg>
                  Complete medical history tracking
                </li>
                <li class="flex items-center">
                  <svg class="w-4 h-4 mr-2 text-green-500" fill="currentColor" viewBox="0 0 20 20">
                    <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd" />
                  </svg>
                  Advanced search and filtering
                </li>
                <li class="flex items-center">
                  <svg class="w-4 h-4 mr-2 text-green-500" fill="currentColor" viewBox="0 0 20 20">
                    <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd" />
                  </svg>
                  Automated data backup
                </li>
              </ul>
            </div>

            <!-- Feature 3: Real-time Analytics -->
            <div class="p-8 transition-all duration-300 transform bg-gradient-to-br from-purple-50 to-pink-50 dark:from-gray-700 dark:to-gray-600 rounded-2xl hover:shadow-xl hover:-translate-y-1">
              <div class="flex items-center justify-center w-16 h-16 mb-6 bg-purple-100 dark:bg-purple-900 rounded-xl">
                <svg class="w-8 h-8 text-purple-600 dark:text-purple-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z" />
                </svg>
              </div>
              <h3 class="mb-4 text-2xl font-bold text-gray-900 dark:text-white">Real-time Analytics</h3>
              <p class="mb-6 text-gray-600 dark:text-gray-300">Powerful dashboard with real-time insights and customizable reports for data-driven decisions.</p>
              <ul class="space-y-2 text-sm text-gray-600 dark:text-gray-300">
                <li class="flex items-center">
                  <svg class="w-4 h-4 mr-2 text-green-500" fill="currentColor" viewBox="0 0 20 20">
                    <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd" />
                  </svg>
                  Interactive dashboard widgets
                </li>
                <li class="flex items-center">
                  <svg class="w-4 h-4 mr-2 text-green-500" fill="currentColor" viewBox="0 0 20 20">
                    <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd" />
                  </svg>
                  Custom report generation
                </li>
                <li class="flex items-center">
                  <svg class="w-4 h-4 mr-2 text-green-500" fill="currentColor" viewBox="0 0 20 20">
                    <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd" />
                  </svg>
                  Export to multiple formats
                </li>
              </ul>
            </div>

            <!-- Feature 4: Secure Data Storage -->
            <div class="p-8 transition-all duration-300 transform bg-gradient-to-br from-red-50 to-orange-50 dark:from-gray-700 dark:to-gray-600 rounded-2xl hover:shadow-xl hover:-translate-y-1">
              <div class="flex items-center justify-center w-16 h-16 mb-6 bg-red-100 dark:bg-red-900 rounded-xl">
                <svg class="w-8 h-8 text-red-600 dark:text-red-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 15v2m-6 4h12a2 2 0 002-2v-6a2 2 0 00-2-2H6a2 2 0 00-2 2v6a2 2 0 002 2zm10-10V7a4 4 0 00-8 0v4h8z" />
                </svg>
              </div>
              <h3 class="mb-4 text-2xl font-bold text-gray-900 dark:text-white">Enterprise Security</h3>
              <p class="mb-6 text-gray-600 dark:text-gray-300">HIPAA-compliant security with end-to-end encryption and multi-factor authentication.</p>
              <ul class="space-y-2 text-sm text-gray-600 dark:text-gray-300">
                <li class="flex items-center">
                  <svg class="w-4 h-4 mr-2 text-green-500" fill="currentColor" viewBox="0 0 20 20">
                    <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd" />
                  </svg>
                  256-bit AES encryption
                </li>
                <li class="flex items-center">
                  <svg class="w-4 h-4 mr-2 text-green-500" fill="currentColor" viewBox="0 0 20 20">
                    <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd" />
                  </svg>
                  Multi-factor authentication
                </li>
                <li class="flex items-center">
                  <svg class="w-4 h-4 mr-2 text-green-500" fill="currentColor" viewBox="0 0 20 20">
                    <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd" />
                  </svg>
                  Regular security audits
                </li>
              </ul>
            </div>

            <!-- Feature 5: Mobile Accessibility -->
            <div class="p-8 transition-all duration-300 transform bg-gradient-to-br from-yellow-50 to-amber-50 dark:from-gray-700 dark:to-gray-600 rounded-2xl hover:shadow-xl hover:-translate-y-1">
              <div class="flex items-center justify-center w-16 h-16 mb-6 bg-yellow-100 dark:bg-yellow-900 rounded-xl">
                <svg class="w-8 h-8 text-yellow-600 dark:text-yellow-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 18h.01M8 21h8a2 2 0 002-2V5a2 2 0 00-2-2H8a2 2 0 00-2 2v14a2 2 0 002 2z" />
                </svg>
              </div>
              <h3 class="mb-4 text-2xl font-bold text-gray-900 dark:text-white">Mobile Ready</h3>
              <p class="mb-6 text-gray-600 dark:text-gray-300">Fully responsive design that works seamlessly across all devices and platforms.</p>
              <ul class="space-y-2 text-sm text-gray-600 dark:text-gray-300">
                <li class="flex items-center">
                  <svg class="w-4 h-4 mr-2 text-green-500" fill="currentColor" viewBox="0 0 20 20">
                    <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd" />
                  </svg>
                  Progressive Web App support
                </li>
                <li class="flex items-center">
                  <svg class="w-4 h-4 mr-2 text-green-500" fill="currentColor" viewBox="0 0 20 20">
                    <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd" />
                  </svg>
                  Offline functionality
                </li>
                <li class="flex items-center">
                  <svg class="w-4 h-4 mr-2 text-green-500" fill="currentColor" viewBox="0 0 20 20">
                    <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd" />
                  </svg>
                  Touch-optimized interface
                </li>
              </ul>
            </div>

            <!-- Feature 6: Integration Hub -->
            <div class="p-8 transition-all duration-300 transform bg-gradient-to-br from-indigo-50 to-blue-50 dark:from-gray-700 dark:to-gray-600 rounded-2xl hover:shadow-xl hover:-translate-y-1">
              <div class="flex items-center justify-center w-16 h-16 mb-6 bg-indigo-100 dark:bg-indigo-900 rounded-xl">
                <svg class="w-8 h-8 text-indigo-600 dark:text-indigo-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8.111 16.404a5.5 5.5 0 017.778 0M12 20h.01m-7.08-7.071c3.904-3.905 10.236-3.905 14.141 0M1.394 9.393c5.857-5.857 15.355-5.857 21.213 0" />
                </svg>
              </div>
              <h3 class="mb-4 text-2xl font-bold text-gray-900 dark:text-white">API Integration</h3>
              <p class="mb-6 text-gray-600 dark:text-gray-300">Seamless integration with existing healthcare systems and third-party applications.</p>
              <ul class="space-y-2 text-sm text-gray-600 dark:text-gray-300">
                <li class="flex items-center">
                  <svg class="w-4 h-4 mr-2 text-green-500" fill="currentColor" viewBox="0 0 20 20">
                    <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd" />
                  </svg>
                  RESTful API endpoints
                </li>
                <li class="flex items-center">
                  <svg class="w-4 h-4 mr-2 text-green-500" fill="currentColor" viewBox="0 0 20 20">
                    <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd" />
                  </svg>
                  HL7 FHIR compliance
                </li>
                <li class="flex items-center">
                  <svg class="w-4 h-4 mr-2 text-green-500" fill="currentColor" viewBox="0 0 20 20">
                    <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd" />
                  </svg>
                  Webhook support
                </li>
              </ul>
            </div>
          </div>
        </div>
      </section>

      <!-- Advanced Features Section -->
      <section class="py-20 bg-gradient-to-r from-purple-600 to-indigo-600">
        <div class="px-4 mx-auto max-w-7xl sm:px-6 lg:px-8">
          <div class="mb-16 text-center">
            <h2 class="mb-4 text-3xl font-extrabold text-white">
              Advanced Capabilities
            </h2>
            <p class="max-w-3xl mx-auto text-xl text-purple-100">
              Cutting-edge features that set Aesclea apart from traditional healthcare management systems
            </p>
          </div>
          
          <div class="grid grid-cols-1 gap-12 md:grid-cols-2">
            <div class="text-white">
              <h3 class="mb-6 text-2xl font-bold">Machine Learning Models</h3>
              <div class="space-y-4">
                <div class="flex items-start">
                  <div class="flex items-center justify-center flex-shrink-0 w-6 h-6 mt-1 mr-4 bg-white rounded-full">
                    <div class="w-2 h-2 bg-purple-600 rounded-full"></div>
                  </div>
                  <div>
                    <h4 class="mb-1 font-semibold">Neural Network Classification</h4>
                    <p class="text-purple-100">Advanced deep learning models for accurate tumor detection and classification.</p>
                  </div>
                </div>
                <div class="flex items-start">
                  <div class="flex items-center justify-center flex-shrink-0 w-6 h-6 mt-1 mr-4 bg-white rounded-full">
                    <div class="w-2 h-2 bg-purple-600 rounded-full"></div>
                  </div>
                  <div>
                    <h4 class="mb-1 font-semibold">Recurrent Networks</h4>
                    <p class="text-purple-100">Time-series analysis for patient monitoring and trend prediction.</p>
                  </div>
                </div>
                <div class="flex items-start">
                  <div class="flex items-center justify-center flex-shrink-0 w-6 h-6 mt-1 mr-4 bg-white rounded-full">
                    <div class="w-2 h-2 bg-purple-600 rounded-full"></div>
                  </div>
                  <div>
                    <h4 class="mb-1 font-semibold">Continuous Learning</h4>
                    <p class="text-purple-100">Models that improve accuracy over time with new data inputs.</p>
                  </div>
                </div>
              </div>
            </div>
            
            <div class="text-white">
              <h3 class="mb-6 text-2xl font-bold">Image Processing</h3>
              <div class="space-y-4">
                <div class="flex items-start">
                  <div class="flex items-center justify-center flex-shrink-0 w-6 h-6 mt-1 mr-4 bg-white rounded-full">
                    <div class="w-2 h-2 bg-purple-600 rounded-full"></div>
                  </div>
                  <div>
                    <h4 class="mb-1 font-semibold">Medical Image Analysis</h4>
                    <p class="text-purple-100">Automated analysis of MRI, CT, and X-ray images with AI assistance.</p>
                  </div>
                </div>
                <div class="flex items-start">
                  <div class="flex items-center justify-center flex-shrink-0 w-6 h-6 mt-1 mr-4 bg-white rounded-full">
                    <div class="w-2 h-2 bg-purple-600 rounded-full"></div>
                  </div>
                  <div>
                    <h4 class="mb-1 font-semibold">Annotation Tools</h4>
                    <p class="text-purple-100">Professional annotation interface for marking and documenting findings.</p>
                  </div>
                </div>
                <div class="flex items-start">
                  <div class="flex items-center justify-center flex-shrink-0 w-6 h-6 mt-1 mr-4 bg-white rounded-full">
                    <div class="w-2 h-2 bg-purple-600 rounded-full"></div>
                  </div>
                  <div>
                    <h4 class="mb-1 font-semibold">DICOM Support</h4>
                    <p class="text-purple-100">Full compatibility with medical imaging standards and formats.</p>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </section>

      <!-- CTA Section -->
      <section class="py-16 bg-white dark:bg-gray-800">
        <div class="px-4 mx-auto text-center max-w-7xl sm:px-6 lg:px-8">
          <h2 class="mb-4 text-3xl font-extrabold text-gray-900 dark:text-white">
            Ready to experience these features?
          </h2>
          <p class="max-w-2xl mx-auto mb-8 text-xl text-gray-600 dark:text-gray-300">
            Start your free trial today and discover how Aesclea can transform your healthcare practice.
          </p>
          <div class="flex flex-col justify-center gap-4 sm:flex-row">
            <router-link
              to="/register"
              class="inline-flex items-center px-8 py-4 text-lg font-medium text-white transition-all duration-200 border border-transparent rounded-lg shadow-lg bg-gradient-to-r from-purple-600 to-indigo-600 hover:from-purple-700 hover:to-indigo-700 hover:shadow-xl"
            >
              Start Free Trial
              <svg class="w-5 h-5 ml-2 -mr-1" fill="currentColor" viewBox="0 0 20 20">
                <path fill-rule="evenodd" d="M10.293 3.293a1 1 0 011.414 0l6 6a1 1 0 010 1.414l-6 6a1 1 0 01-1.414-1.414L14.586 11H3a1 1 0 110-2h11.586l-4.293-4.293a1 1 0 010-1.414z" clip-rule="evenodd" />
              </svg>
            </router-link>
            <router-link
              to="/pricing"
              class="inline-flex items-center px-8 py-4 text-lg font-medium text-indigo-600 transition-all duration-200 border-2 border-indigo-600 rounded-lg dark:text-indigo-400 hover:bg-indigo-50 dark:hover:bg-gray-700"
            >
              View Pricing
            </router-link>
          </div>
        </div>
      </section>

      <!-- Footer -->
      <SiteFooter />
    </div>
  </div>
</template>

<script setup lang="ts">
import SiteNavbar from '../components/SiteNavbar.vue'
import SiteFooter from '../components/SiteFooter.vue'
</script>