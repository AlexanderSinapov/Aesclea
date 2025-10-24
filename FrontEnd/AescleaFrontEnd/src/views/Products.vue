<!-- Copyright (c) 2025 Alexander Sinapov | Simeon Petkov -->
<!-- All rights reserved. -->
<!-- This code is proprietary and confidential. -->
<!-- Unauthorized copying, modification, distribution, or use is strictly prohibited. -->

<template>
  <div class="min-h-screen bg-white dark:bg-gray-900">
    <!-- Navigation -->
    <SiteNavbar />

    <!-- Hero Section -->
    <section class="pt-32 pb-16 px-4 sm:px-6 lg:px-8 bg-gradient-to-br from-blue-50 via-indigo-50 to-purple-50 dark:from-gray-900 dark:via-gray-800 dark:to-gray-900">
      <div class="max-w-7xl mx-auto text-center">
        <h1 class="text-5xl md:text-6xl font-extrabold text-gray-900 dark:text-white mb-6">
          Our Products
        </h1>
        <p class="text-xl md:text-2xl text-gray-600 dark:text-gray-300 max-w-3xl mx-auto">
          Cutting-edge AI solutions designed to transform healthcare delivery and improve patient outcomes
        </p>
      </div>
    </section>

    <!-- Main Product -->
    <section class="py-20 px-4 sm:px-6 lg:px-8 bg-white dark:bg-gray-800">
      <div class="max-w-7xl mx-auto">
        <div class="grid grid-cols-1 lg:grid-cols-2 gap-12 items-center mb-20">
          <div>
            <div class="inline-flex items-center px-4 py-2 bg-blue-100 dark:bg-blue-900/30 rounded-full text-blue-700 dark:text-blue-300 text-sm font-medium mb-6">
              <svg class="w-4 h-4 mr-2" fill="currentColor" viewBox="0 0 20 20">
                <path fill-rule="evenodd" d="M11.3 1.046A1 1 0 0112 2v5h4a1 1 0 01.82 1.573l-7 10A1 1 0 018 18v-5H4a1 1 0 01-.82-1.573l7-10a1 1 0 011.12-.38z" clip-rule="evenodd" />
              </svg>
              Flagship Product
            </div>
            <h2 class="text-4xl md:text-5xl font-extrabold text-gray-900 dark:text-white mb-6">
              Aesclea AI
              <span class="block mt-2 bg-gradient-to-r from-blue-600 to-indigo-600 bg-clip-text text-transparent">
                Medical Intelligence Platform
              </span>
            </h2>
            <p class="text-xl text-gray-600 dark:text-gray-300 mb-8">
              The world's most advanced AI-powered medical diagnostic system. Aesclea AI leverages deep learning 
              and neural networks to analyze medical imaging, predict diagnoses, and provide decision support for healthcare professionals.
            </p>
            
            <div class="space-y-4 mb-8">
              <div class="flex items-start">
                <svg class="w-6 h-6 text-blue-600 dark:text-blue-400 mr-3 flex-shrink-0 mt-1" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z" />
                </svg>
                <div>
                  <h4 class="font-semibold text-gray-900 dark:text-white">Tumor Classification</h4>
                  <p class="text-gray-600 dark:text-gray-300">Detect and classify brain tumors with 99.9% accuracy using advanced CNN models</p>
                </div>
              </div>
              <div class="flex items-start">
                <svg class="w-6 h-6 text-blue-600 dark:text-blue-400 mr-3 flex-shrink-0 mt-1" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z" />
                </svg>
                <div>
                  <h4 class="font-semibold text-gray-900 dark:text-white">Multi-Disease Diagnosis</h4>
                  <p class="text-gray-600 dark:text-gray-300">Analyze patient symptoms and medical history for comprehensive diagnosis recommendations</p>
                </div>
              </div>
              <div class="flex items-start">
                <svg class="w-6 h-6 text-blue-600 dark:text-blue-400 mr-3 flex-shrink-0 mt-1" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z" />
                </svg>
                <div>
                  <h4 class="font-semibold text-gray-900 dark:text-white">Predictive Analytics</h4>
                  <p class="text-gray-600 dark:text-gray-300">Forecast patient outcomes and disease progression with machine learning models</p>
                </div>
              </div>
            </div>

            <div class="flex flex-col sm:flex-row gap-4">
              <router-link 
                to="/register" 
                class="px-8 py-4 bg-gradient-to-r from-blue-600 to-indigo-600 text-white rounded-lg hover:from-blue-700 hover:to-indigo-700 transition-all shadow-lg hover:shadow-xl font-medium text-center"
              >
                Start Free Trial
              </router-link>
              <router-link 
                to="/pricing" 
                class="px-8 py-4 border-2 border-gray-300 dark:border-gray-700 text-gray-700 dark:text-gray-300 rounded-lg hover:border-blue-600 dark:hover:border-blue-500 transition-all font-medium text-center"
              >
                View Pricing
              </router-link>
            </div>
          </div>

          <div class="relative">
            <div class="aspect-square bg-gradient-to-br from-blue-600 via-indigo-600 to-purple-600 rounded-2xl shadow-2xl p-8 flex items-center justify-center">
              <svg class="w-full h-full text-white opacity-20" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="0.5" d="M9.663 17h4.673M12 3v1m6.364 1.636l-.707.707M21 12h-1M4 12H3m3.343-5.657l-.707-.707m2.828 9.9a5 5 0 117.072 0l-.548.547A3.374 3.374 0 0014 18.469V19a2 2 0 11-4 0v-.531c0-.895-.356-1.754-.988-2.386l-.548-.547z" />
              </svg>
            </div>
          </div>
        </div>

        <!-- AI Features Grid -->
        <div class="mt-20">
          <h3 class="text-3xl font-bold text-gray-900 dark:text-white text-center mb-12">
            Comprehensive AI Capabilities
          </h3>
          
          <div class="grid grid-cols-1 md:grid-cols-3 gap-8">
            <!-- Feature 1 -->
            <div class="p-6 bg-gradient-to-br from-blue-50 to-indigo-50 dark:from-gray-700 dark:to-gray-800 rounded-xl">
              <div class="w-12 h-12 bg-blue-600 rounded-lg flex items-center justify-center mb-4">
                <svg class="w-6 h-6 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 16l4.586-4.586a2 2 0 012.828 0L16 16m-2-2l1.586-1.586a2 2 0 012.828 0L20 14m-6-6h.01M6 20h12a2 2 0 002-2V6a2 2 0 00-2-2H6a2 2 0 00-2 2v12a2 2 0 002 2z" />
                </svg>
              </div>
              <h4 class="text-xl font-semibold text-gray-900 dark:text-white mb-2">Medical Imaging Analysis</h4>
              <p class="text-gray-600 dark:text-gray-300">
                Process MRI, CT, and X-ray scans with state-of-the-art computer vision algorithms for instant insights
              </p>
            </div>

            <!-- Feature 2 -->
            <div class="p-6 bg-gradient-to-br from-purple-50 to-pink-50 dark:from-gray-700 dark:to-gray-800 rounded-xl">
              <div class="w-12 h-12 bg-purple-600 rounded-lg flex items-center justify-center mb-4">
                <svg class="w-6 h-6 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 6V4m0 2a2 2 0 100 4m0-4a2 2 0 110 4m-6 8a2 2 0 100-4m0 4a2 2 0 110-4m0 4v2m0-6V4m6 6v10m6-2a2 2 0 100-4m0 4a2 2 0 110-4m0 4v2m0-6V4" />
                </svg>
              </div>
              <h4 class="text-xl font-semibold text-gray-900 dark:text-white mb-2">Neural Network Training</h4>
              <p class="text-gray-600 dark:text-gray-300">
                Custom trainable models that adapt to your hospital's specific patient demographics and conditions
              </p>
            </div>

            <!-- Feature 3 -->
            <div class="p-6 bg-gradient-to-br from-green-50 to-teal-50 dark:from-gray-700 dark:to-gray-800 rounded-xl">
              <div class="w-12 h-12 bg-green-600 rounded-lg flex items-center justify-center mb-4">
                <svg class="w-6 h-6 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z" />
                </svg>
              </div>
              <h4 class="text-xl font-semibold text-gray-900 dark:text-white mb-2">Real-Time Monitoring</h4>
              <p class="text-gray-600 dark:text-gray-300">
                Continuous patient vitals monitoring with AI-powered anomaly detection and instant alerts
              </p>
            </div>

            <!-- Feature 4 -->
            <div class="p-6 bg-gradient-to-br from-orange-50 to-red-50 dark:from-gray-700 dark:to-gray-800 rounded-xl">
              <div class="w-12 h-12 bg-orange-600 rounded-lg flex items-center justify-center mb-4">
                <svg class="w-6 h-6 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5H7a2 2 0 00-2 2v10a2 2 0 002 2h8a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2m-3 7h3m-3 4h3m-6-4h.01M9 16h.01" />
                </svg>
              </div>
              <h4 class="text-xl font-semibold text-gray-900 dark:text-white mb-2">Treatment Recommendations</h4>
              <p class="text-gray-600 dark:text-gray-300">
                AI-generated treatment plans based on medical literature, patient history, and outcome data
              </p>
            </div>

            <!-- Feature 5 -->
            <div class="p-6 bg-gradient-to-br from-yellow-50 to-amber-50 dark:from-gray-700 dark:to-gray-800 rounded-xl">
              <div class="w-12 h-12 bg-yellow-600 rounded-lg flex items-center justify-center mb-4">
                <svg class="w-6 h-6 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 6.253v13m0-13C10.832 5.477 9.246 5 7.5 5S4.168 5.477 3 6.253v13C4.168 18.477 5.754 18 7.5 18s3.332.477 4.5 1.253m0-13C13.168 5.477 14.754 5 16.5 5c1.747 0 3.332.477 4.5 1.253v13C19.832 18.477 18.247 18 16.5 18c-1.746 0-3.332.477-4.5 1.253" />
                </svg>
              </div>
              <h4 class="text-xl font-semibold text-gray-900 dark:text-white mb-2">Medical Knowledge Base</h4>
              <p class="text-gray-600 dark:text-gray-300">
                Access to continuously updated medical literature and research integrated into diagnostic algorithms
              </p>
            </div>

            <!-- Feature 6 -->
            <div class="p-6 bg-gradient-to-br from-cyan-50 to-blue-50 dark:from-gray-700 dark:to-gray-800 rounded-xl">
              <div class="w-12 h-12 bg-cyan-600 rounded-lg flex items-center justify-center mb-4">
                <svg class="w-6 h-6 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m5.618-4.016A11.955 11.955 0 0112 2.944a11.955 11.955 0 01-8.618 3.04A12.02 12.02 0 003 9c0 5.591 3.824 10.29 9 11.622 5.176-1.332 9-6.03 9-11.622 0-1.042-.133-2.052-.382-3.016z" />
                </svg>
              </div>
              <h4 class="text-xl font-semibold text-gray-900 dark:text-white mb-2">HIPAA Compliant</h4>
              <p class="text-gray-600 dark:text-gray-300">
                Enterprise-grade security and privacy controls ensure full compliance with healthcare regulations
              </p>
            </div>
          </div>
        </div>
      </div>
    </section>

    <!-- Additional Products -->
    <section class="py-20 px-4 sm:px-6 lg:px-8 bg-gray-50 dark:bg-gray-900">
      <div class="max-w-7xl mx-auto">
        <h2 class="text-4xl font-extrabold text-gray-900 dark:text-white text-center mb-12">
          Complete Healthcare Ecosystem
        </h2>
        
        <div class="grid grid-cols-1 md:grid-cols-2 gap-8">
          <!-- Product 2 -->
          <div class="bg-white dark:bg-gray-800 p-8 rounded-2xl shadow-lg hover:shadow-xl transition-all">
            <div class="w-16 h-16 bg-gradient-to-br from-green-600 to-teal-600 rounded-xl flex items-center justify-center mb-6">
              <svg class="w-10 h-10 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857M7 20H2v-2a3 3 0 015.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0zm6 3a2 2 0 11-4 0 2 2 0 014 0zM7 10a2 2 0 11-4 0 2 2 0 014 0z" />
              </svg>
            </div>
            <h3 class="text-2xl font-bold text-gray-900 dark:text-white mb-4">
              Aesclea Practice Manager
            </h3>
            <p class="text-gray-600 dark:text-gray-300 mb-6">
              Comprehensive practice management system with patient records, appointment scheduling, 
              billing, and administrative tools designed for modern healthcare facilities.
            </p>
            <ul class="space-y-2 mb-6">
              <li class="flex items-center text-gray-600 dark:text-gray-300">
                <svg class="w-5 h-5 text-green-600 dark:text-green-400 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 13l4 4L19 7" />
                </svg>
                Electronic Health Records (EHR)
              </li>
              <li class="flex items-center text-gray-600 dark:text-gray-300">
                <svg class="w-5 h-5 text-green-600 dark:text-green-400 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 13l4 4L19 7" />
                </svg>
                Smart Appointment Scheduling
              </li>
              <li class="flex items-center text-gray-600 dark:text-gray-300">
                <svg class="w-5 h-5 text-green-600 dark:text-green-400 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 13l4 4L19 7" />
                </svg>
                Integrated Billing & Payments
              </li>
            </ul>
            <router-link 
              to="/register" 
              class="inline-block px-6 py-3 bg-green-600 text-white rounded-lg hover:bg-green-700 transition-colors font-medium"
            >
              Learn More
            </router-link>
          </div>

          <!-- Product 3 -->
          <div class="bg-white dark:bg-gray-800 p-8 rounded-2xl shadow-lg hover:shadow-xl transition-all">
            <div class="w-16 h-16 bg-gradient-to-br from-purple-600 to-pink-600 rounded-xl flex items-center justify-center mb-6">
              <svg class="w-10 h-10 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 10l4.553-2.276A1 1 0 0121 8.618v6.764a1 1 0 01-1.447.894L15 14M5 18h8a2 2 0 002-2V8a2 2 0 00-2-2H5a2 2 0 00-2 2v8a2 2 0 002 2z" />
              </svg>
            </div>
            <h3 class="text-2xl font-bold text-gray-900 dark:text-white mb-4">
              Aesclea Telemedicine
            </h3>
            <p class="text-gray-600 dark:text-gray-300 mb-6">
              HIPAA-compliant telehealth platform enabling secure video consultations, remote patient monitoring, 
              and digital prescription management.
            </p>
            <ul class="space-y-2 mb-6">
              <li class="flex items-center text-gray-600 dark:text-gray-300">
                <svg class="w-5 h-5 text-purple-600 dark:text-purple-400 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 13l4 4L19 7" />
                </svg>
                HD Video Consultations
              </li>
              <li class="flex items-center text-gray-600 dark:text-gray-300">
                <svg class="w-5 h-5 text-purple-600 dark:text-purple-400 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 13l4 4L19 7" />
                </svg>
                Remote Patient Monitoring
              </li>
              <li class="flex items-center text-gray-600 dark:text-gray-300">
                <svg class="w-5 h-5 text-purple-600 dark:text-purple-400 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 13l4 4L19 7" />
                </svg>
                Digital Prescriptions
              </li>
            </ul>
            <router-link 
              to="/register" 
              class="inline-block px-6 py-3 bg-purple-600 text-white rounded-lg hover:bg-purple-700 transition-colors font-medium"
            >
              Learn More
            </router-link>
          </div>
        </div>
      </div>
    </section>

    <!-- CTA Section -->
    <section class="py-20 px-4 sm:px-6 lg:px-8 bg-gradient-to-r from-blue-600 via-indigo-600 to-purple-600">
      <div class="max-w-4xl mx-auto text-center">
        <h2 class="text-4xl md:text-5xl font-extrabold text-white mb-6">
          Ready to Experience Aesclea AI?
        </h2>
        <p class="text-xl text-blue-100 mb-10">
          Start your free trial today and discover how AI can transform your medical practice
        </p>
        <div class="flex flex-col sm:flex-row gap-4 justify-center">
          <router-link 
            to="/register" 
            class="px-8 py-4 bg-white text-blue-600 rounded-lg hover:bg-gray-100 transition-all shadow-lg hover:shadow-xl font-bold text-lg"
          >
            Start Free Trial
          </router-link>
          <router-link 
            to="/contact" 
            class="px-8 py-4 bg-transparent text-white border-2 border-white rounded-lg hover:bg-white/10 transition-all font-bold text-lg"
          >
            Contact Sales
          </router-link>
        </div>
      </div>
    </section>

    <!-- Footer -->
    <SiteFooter />
  </div>
</template>

<script setup lang="ts">
import SiteNavbar from '../components/SiteNavbar.vue'
import SiteFooter from '../components/SiteFooter.vue'
</script>
