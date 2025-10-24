<!-- Copyright (c) 2025 Alexander Sinapov | Simeon Petkov -->
<!-- All rights reserved. -->
<!-- This code is proprietary and confidential. -->
<!-- Unauthorized copying, modification, distribution, or use is strictly prohibited. -->

<template>
  <div class="min-h-screen bg-white dark:bg-gray-900">
    <!-- Navigation -->
    <SiteNavbar />

    <!-- Hero Section -->
    <section class="pt-32 pb-20 px-4 sm:px-6 lg:px-8 bg-gradient-to-br from-blue-50 via-indigo-50 to-purple-50 dark:from-gray-900 dark:via-gray-800 dark:to-gray-900">
      <div class="max-w-7xl mx-auto">
        <div class="text-center">
          <div class="inline-flex items-center px-4 py-2 bg-blue-100 dark:bg-blue-900/30 rounded-full text-blue-700 dark:text-blue-300 text-sm font-medium mb-8">
            <svg class="w-4 h-4 mr-2" fill="currentColor" viewBox="0 0 20 20">
              <path fill-rule="evenodd" d="M11.3 1.046A1 1 0 0112 2v5h4a1 1 0 01.82 1.573l-7 10A1 1 0 018 18v-5H4a1 1 0 01-.82-1.573l7-10a1 1 0 011.12-.38z" clip-rule="evenodd" />
            </svg>
            Powered by AI • Trusted by Healthcare Professionals
          </div>
          
          <h1 class="text-5xl md:text-7xl font-extrabold text-gray-900 dark:text-white mb-6">
            The Future of
            <span class="block mt-2 bg-gradient-to-r from-blue-600 via-indigo-600 to-purple-600 bg-clip-text text-transparent">
              Medical Intelligence
            </span>
          </h1>
          
          <p class="max-w-3xl mx-auto text-xl md:text-2xl text-gray-600 dark:text-gray-300 mb-12">
            Aesclea combines cutting-edge AI technology with comprehensive healthcare management 
            to revolutionize patient care and medical diagnostics.
          </p>

          <div class="flex flex-col sm:flex-row gap-4 justify-center items-center mb-16">
            <router-link 
              to="/register" 
              class="px-8 py-4 bg-gradient-to-r from-blue-600 to-indigo-600 text-white rounded-lg hover:from-blue-700 hover:to-indigo-700 transition-all shadow-lg hover:shadow-xl font-medium text-lg flex items-center"
            >
              Start Free Trial
              <svg class="w-5 h-5 ml-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 7l5 5m0 0l-5 5m5-5H6" />
              </svg>
            </router-link>
            <router-link 
              to="/products" 
              class="px-8 py-4 bg-white dark:bg-gray-800 text-gray-900 dark:text-white border-2 border-gray-300 dark:border-gray-700 rounded-lg hover:border-blue-600 dark:hover:border-blue-500 transition-all font-medium text-lg flex items-center"
            >
              <svg class="w-5 h-5 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 10V3L4 14h7v7l9-11h-7z" />
              </svg>
              Explore Products
            </router-link>
          </div>
        </div>
      </div>
    </section>

    <!-- Features Section -->
    <section id="features" class="py-20 px-4 sm:px-6 lg:px-8 bg-white dark:bg-gray-800">
      <div class="max-w-7xl mx-auto">
        <div class="text-center mb-16">
          <h2 class="text-4xl md:text-5xl font-extrabold text-gray-900 dark:text-white mb-4">
            Intelligent Healthcare Solutions
          </h2>
          <p class="text-xl text-gray-600 dark:text-gray-300 max-w-3xl mx-auto">
            Our AI-powered platform provides comprehensive tools for modern healthcare delivery
          </p>
        </div>

        <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-8">
          <!-- Feature 1: AI Diagnostics -->
          <div class="p-8 bg-gradient-to-br from-blue-50 to-indigo-50 dark:from-gray-700 dark:to-gray-800 rounded-2xl hover:shadow-xl transition-all">
            <div class="w-14 h-14 bg-gradient-to-br from-blue-600 to-indigo-600 rounded-xl flex items-center justify-center mb-6">
              <svg class="w-8 h-8 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9.663 17h4.673M12 3v1m6.364 1.636l-.707.707M21 12h-1M4 12H3m3.343-5.657l-.707-.707m2.828 9.9a5 5 0 117.072 0l-.548.547A3.374 3.374 0 0014 18.469V19a2 2 0 11-4 0v-.531c0-.895-.356-1.754-.988-2.386l-.548-.547z" />
              </svg>
            </div>
            <h3 class="text-2xl font-bold text-gray-900 dark:text-white mb-3">AI-Powered Diagnostics</h3>
            <p class="text-gray-600 dark:text-gray-300">
              Advanced neural networks analyze medical imaging and patient data to provide accurate diagnoses 
              for tumors, diseases, and medical conditions.
            </p>
          </div>

          <!-- Feature 2: Patient Management -->
          <div class="p-8 bg-gradient-to-br from-purple-50 to-pink-50 dark:from-gray-700 dark:to-gray-800 rounded-2xl hover:shadow-xl transition-all">
            <div class="w-14 h-14 bg-gradient-to-br from-purple-600 to-pink-600 rounded-xl flex items-center justify-center mb-6">
              <svg class="w-8 h-8 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857M7 20H2v-2a3 3 0 015.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0zm6 3a2 2 0 11-4 0 2 2 0 014 0zM7 10a2 2 0 11-4 0 2 2 0 014 0z" />
              </svg>
            </div>
            <h3 class="text-2xl font-bold text-gray-900 dark:text-white mb-3">Smart Patient Records</h3>
            <p class="text-gray-600 dark:text-gray-300">
              Comprehensive patient management system with intelligent record keeping, 
              automated insights, and seamless integration with medical devices.
            </p>
          </div>

          <!-- Feature 3: Real-time Analytics -->
          <div class="p-8 bg-gradient-to-br from-green-50 to-teal-50 dark:from-gray-700 dark:to-gray-800 rounded-2xl hover:shadow-xl transition-all">
            <div class="w-14 h-14 bg-gradient-to-br from-green-600 to-teal-600 rounded-xl flex items-center justify-center mb-6">
              <svg class="w-8 h-8 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z" />
              </svg>
            </div>
            <h3 class="text-2xl font-bold text-gray-900 dark:text-white mb-3">Real-Time Analytics</h3>
            <p class="text-gray-600 dark:text-gray-300">
              Monitor patient vitals, track health trends, and get predictive insights 
              with our advanced analytics dashboard powered by machine learning.
            </p>
          </div>

          <!-- Feature 4: Secure Cloud Storage -->
          <div class="p-8 bg-gradient-to-br from-orange-50 to-red-50 dark:from-gray-700 dark:to-gray-800 rounded-2xl hover:shadow-xl transition-all">
            <div class="w-14 h-14 bg-gradient-to-br from-orange-600 to-red-600 rounded-xl flex items-center justify-center mb-6">
              <svg class="w-8 h-8 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 15v2m-6 4h12a2 2 0 002-2v-6a2 2 0 00-2-2H6a2 2 0 00-2 2v6a2 2 0 002 2zm10-10V7a4 4 0 00-8 0v4h8z" />
              </svg>
            </div>
            <h3 class="text-2xl font-bold text-gray-900 dark:text-white mb-3">Enterprise Security</h3>
            <p class="text-gray-600 dark:text-gray-300">
              HIPAA-compliant cloud storage with end-to-end encryption, 
              multi-factor authentication, and comprehensive audit trails.
            </p>
          </div>

          <!-- Feature 5: Appointment Scheduling -->
          <div class="p-8 bg-gradient-to-br from-yellow-50 to-amber-50 dark:from-gray-700 dark:to-gray-800 rounded-2xl hover:shadow-xl transition-all">
            <div class="w-14 h-14 bg-gradient-to-br from-yellow-600 to-amber-600 rounded-xl flex items-center justify-center mb-6">
              <svg class="w-8 h-8 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z" />
              </svg>
            </div>
            <h3 class="text-2xl font-bold text-gray-900 dark:text-white mb-3">Smart Scheduling</h3>
            <p class="text-gray-600 dark:text-gray-300">
              AI-optimized appointment scheduling with automated reminders, 
              calendar sync, and intelligent resource allocation.
            </p>
          </div>

          <!-- Feature 6: Telemedicine -->
          <div class="p-8 bg-gradient-to-br from-cyan-50 to-blue-50 dark:from-gray-700 dark:to-gray-800 rounded-2xl hover:shadow-xl transition-all">
            <div class="w-14 h-14 bg-gradient-to-br from-cyan-600 to-blue-600 rounded-xl flex items-center justify-center mb-6">
              <svg class="w-8 h-8 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 10l4.553-2.276A1 1 0 0121 8.618v6.764a1 1 0 01-1.447.894L15 14M5 18h8a2 2 0 002-2V8a2 2 0 00-2-2H5a2 2 0 00-2 2v8a2 2 0 002 2z" />
              </svg>
            </div>
            <h3 class="text-2xl font-bold text-gray-900 dark:text-white mb-3">Telemedicine Platform</h3>
            <p class="text-gray-600 dark:text-gray-300">
              Integrated video consultations, remote patient monitoring, 
              and digital prescriptions for modern healthcare delivery.
            </p>
          </div>
        </div>
      </div>
    </section>

    <!-- About Section -->
    <section id="about" class="py-20 px-4 sm:px-6 lg:px-8 bg-gradient-to-br from-gray-50 to-blue-50 dark:from-gray-900 dark:to-gray-800">
      <div class="max-w-7xl mx-auto">
        <div class="grid grid-cols-1 lg:grid-cols-2 gap-12 items-center">
          <div>
            <h2 class="text-4xl md:text-5xl font-extrabold text-gray-900 dark:text-white mb-6">
              About Aesclea
            </h2>
            <p class="text-xl text-gray-600 dark:text-gray-300 mb-6">
              Aesclea is a pioneering medical technology company dedicated to transforming healthcare 
              through artificial intelligence and innovative software solutions.
            </p>
            <p class="text-lg text-gray-600 dark:text-gray-300 mb-6">
              Founded by healthcare professionals and AI experts, we combine deep medical knowledge 
              with cutting-edge technology to create tools that empower doctors, improve patient outcomes, 
              and advance medical science.
            </p>
            <div class="space-y-4">
              <div class="flex items-start">
                <svg class="w-6 h-6 text-blue-600 dark:text-blue-400 mr-3 flex-shrink-0 mt-1" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 13l4 4L19 7" />
                </svg>
                <div>
                  <h4 class="font-semibold text-gray-900 dark:text-white">Evidence-Based AI</h4>
                  <p class="text-gray-600 dark:text-gray-300">Built on peer-reviewed research and validated clinical data</p>
                </div>
              </div>
              <div class="flex items-start">
                <svg class="w-6 h-6 text-blue-600 dark:text-blue-400 mr-3 flex-shrink-0 mt-1" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 13l4 4L19 7" />
                </svg>
                <div>
                  <h4 class="font-semibold text-gray-900 dark:text-white">Global Reach</h4>
                  <p class="text-gray-600 dark:text-gray-300">Serving healthcare institutions across 50+ countries</p>
                </div>
              </div>
              <div class="flex items-start">
                <svg class="w-6 h-6 text-blue-600 dark:text-blue-400 mr-3 flex-shrink-0 mt-1" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 13l4 4L19 7" />
                </svg>
                <div>
                  <h4 class="font-semibold text-gray-900 dark:text-white">Continuous Innovation</h4>
                  <p class="text-gray-600 dark:text-gray-300">Regular updates with the latest medical AI research</p>
                </div>
              </div>
            </div>
          </div>
          <div class="relative">
            <div class="aspect-square bg-gradient-to-br from-blue-600 to-indigo-600 rounded-2xl shadow-2xl"></div>
            <div class="absolute inset-0 flex items-center justify-center">
              <svg class="w-48 h-48 text-white opacity-20" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="1" d="M9.663 17h4.673M12 3v1m6.364 1.636l-.707.707M21 12h-1M4 12H3m3.343-5.657l-.707-.707m2.828 9.9a5 5 0 117.072 0l-.548.547A3.374 3.374 0 0014 18.469V19a2 2 0 11-4 0v-.531c0-.895-.356-1.754-.988-2.386l-.548-.547z" />
              </svg>
            </div>
          </div>
        </div>
      </div>
    </section>

    <!-- CTA Section -->
    <section class="py-20 px-4 sm:px-6 lg:px-8 bg-gradient-to-r from-blue-600 via-indigo-600 to-purple-600">
      <div class="max-w-4xl mx-auto text-center">
        <h2 class="text-4xl md:text-5xl font-extrabold text-white mb-6">
          Ready to Transform Your Practice?
        </h2>
        <p class="text-xl text-blue-100 mb-10">
          Join thousands of healthcare professionals who trust Aesclea for intelligent medical solutions.
        </p>
        <div class="flex flex-col sm:flex-row gap-4 justify-center">
          <router-link 
            to="/register" 
            class="px-8 py-4 bg-white text-blue-600 rounded-lg hover:bg-gray-100 transition-all shadow-lg hover:shadow-xl font-bold text-lg"
          >
            Start Your Free Trial
          </router-link>
          <router-link 
            to="/products" 
            class="px-8 py-4 bg-transparent text-white border-2 border-white rounded-lg hover:bg-white/10 transition-all font-bold text-lg"
          >
            Explore Products
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
