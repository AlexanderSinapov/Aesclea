
<template>
  <div class="min-h-screen bg-gradient-to-br from-purple-50 via-blue-50 to-indigo-100   dark:from-gray-900 dark:via-gray-800 dark:to-gray-900 transition-colors duration-300">
    <!-- Use AppNavbar for consistency -->
    <AppNavbar />

    <!-- Hero Section -->
    <section class="relative overflow-hidden">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-24">
        <div class="text-center">
          <h1 class="text-4xl sm:text-6xl font-extrabold text-gray-900 dark:text-white mb-6 transition-colors duration-300">
            <span class="block">Modern Healthcare</span>
            <span class="block text-transparent bg-clip-text bg-gradient-to-r from-purple-600 to-indigo-600">
              Management System
            </span>
          </h1>
          <p class="max-w-3xl mx-auto text-xl text-gray-600 dark:text-gray-300 mb-10 transition-colors duration-300">
            Streamline your medical practice with our comprehensive management platform. 
            Designed for healthcare professionals to manage patients, appointments, and medical records efficiently.
          </p>
          <div class="flex flex-col sm:flex-row gap-4 justify-center">
            <router-link
              to="/register"
              class="inline-flex items-center px-8 py-4 border border-transparent text-lg font-medium rounded-lg text-white bg-gradient-to-r from-purple-600 to-indigo-600 hover:from-purple-700 hover:to-indigo-700 transition-all duration-200 shadow-lg hover:shadow-xl"
            >
              Get Started
              <svg class="ml-2 -mr-1 w-5 h-5" fill="currentColor" viewBox="0 0 20 20">
                <path fill-rule="evenodd" d="M10.293 3.293a1 1 0 011.414 0l6 6a1 1 0 010 1.414l-6 6a1 1 0 01-1.414-1.414L14.586 11H3a1 1 0 110-2h11.586l-4.293-4.293a1 1 0 010-1.414z" clip-rule="evenodd" />
              </svg>
            </router-link>
            <router-link
              to="/login"
              class="inline-flex items-center px-8 py-4 border-2 border-indigo-600 dark:border-indigo-400 text-lg font-medium rounded-lg text-indigo-600 dark:text-indigo-400 hover:bg-indigo-50 dark:hover:bg-indigo-900/20 transition-all duration-200"
            >
              Sign In
            </router-link>
          </div>
        </div>
      </div>
    </section>

    <!-- Features Section -->
    <section id="features" class="py-20 bg-white dark:bg-gray-800 transition-colors duration-300">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="text-center mb-16">
          <h2 class="text-3xl font-extrabold text-gray-900 dark:text-white mb-4 transition-colors duration-300">
            Everything you need to manage your practice
          </h2>
          <p class="text-xl text-gray-600 dark:text-gray-300 max-w-3xl mx-auto transition-colors duration-300">
            Our platform provides comprehensive tools for modern healthcare management
          </p>
        </div>
        
        <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-8">
          <!-- Feature 1 -->
          <div class="text-center p-6 rounded-xl hover:shadow-lg dark:hover:shadow-gray-700/50 transition-all duration-300 bg-white dark:bg-gray-700/50">
            <div class="w-16 h-16 bg-blue-100 dark:bg-blue-900/50 rounded-full flex items-center justify-center mx-auto mb-4 transition-colors duration-300">
              <svg class="w-8 h-8 text-blue-600 dark:text-blue-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857M7 20H2v-2a3 3 0 015.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0zm6 3a2 2 0 11-4 0 2 2 0 014 0zM7 10a2 2 0 11-4 0 2 2 0 014 0z" />
              </svg>
            </div>
            <h3 class="text-xl font-semibold text-gray-900 dark:text-white mb-2 transition-colors duration-300">Patient Management</h3>
            <p class="text-gray-600 dark:text-gray-300 transition-colors duration-300">Comprehensive patient records, history tracking, and seamless data management.</p>
          </div>

          <!-- Feature 2 -->
          <div class="text-center p-6 rounded-xl hover:shadow-lg dark:hover:shadow-gray-700/50 transition-all duration-300 bg-white dark:bg-gray-700/50">
            <div class="w-16 h-16 bg-green-100 dark:bg-green-900/50 rounded-full flex items-center justify-center mx-auto mb-4 transition-colors duration-300">
              <svg class="w-8 h-8 text-green-600 dark:text-green-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 7V3a1 1 0 011-1h6a1 1 0 011 1v4h3a1 1 0 011 1v8a1 1 0 01-1 1H3a1 1 0 01-1-1V8a1 1 0 011-1h3zm6-4H10v2h4zM6 10v6h12v-6z" />
              </svg>
            </div>
            <h3 class="text-xl font-semibold text-gray-900 dark:text-white mb-2 transition-colors duration-300">Appointment Scheduling</h3>
            <p class="text-gray-600 dark:text-gray-300 transition-colors duration-300">Smart scheduling system with automated reminders and calendar integration.</p>
          </div>

          <!-- Feature 3 -->
          <div class="text-center p-6 rounded-xl hover:shadow-lg dark:hover:shadow-gray-700/50 transition-all duration-300 bg-white dark:bg-gray-700/50">
            <div class="w-16 h-16 bg-purple-100 dark:bg-purple-900/50 rounded-full flex items-center justify-center mx-auto mb-4 transition-colors duration-300">
              <svg class="w-8 h-8 text-purple-600 dark:text-purple-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
              </svg>
            </div>
            <h3 class="text-xl font-semibold text-gray-900 dark:text-white mb-2 transition-colors duration-300">Medical Records</h3>
            <p class="text-gray-600 dark:text-gray-300 transition-colors duration-300">Secure digital medical records with easy access and comprehensive documentation.</p>
          </div>

          <!-- Feature 4 -->
          <div class="text-center p-6 rounded-xl hover:shadow-lg dark:hover:shadow-gray-700/50 transition-all duration-300 bg-white dark:bg-gray-700/50">
            <div class="w-16 h-16 bg-red-100 dark:bg-red-900/50 rounded-full flex items-center justify-center mx-auto mb-4 transition-colors duration-300">
              <svg class="w-8 h-8 text-red-600 dark:text-red-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z" />
              </svg>
            </div>
            <h3 class="text-xl font-semibold text-gray-900 dark:text-white mb-2 transition-colors duration-300">Analytics & Reports</h3>
            <p class="text-gray-600 dark:text-gray-300 transition-colors duration-300">Detailed analytics and customizable reports for better decision making.</p>
          </div>

          <!-- Feature 5 -->
          <div class="text-center p-6 rounded-xl hover:shadow-lg dark:hover:shadow-gray-700/50 transition-all duration-300 bg-white dark:bg-gray-700/50">
            <div class="w-16 h-16 bg-yellow-100 dark:bg-yellow-900/50 rounded-full flex items-center justify-center mx-auto mb-4 transition-colors duration-300">
              <svg class="w-8 h-8 text-yellow-600 dark:text-yellow-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 15v2m-6 4h12a2 2 0 002-2v-6a2 2 0 00-2-2H6a2 2 0 00-2 2v6a2 2 0 002 2zm10-10V7a4 4 0 00-8 0v4h8z" />
              </svg>
            </div>
            <h3 class="text-xl font-semibold text-gray-900 dark:text-white mb-2 transition-colors duration-300">Secure & Compliant</h3>
            <p class="text-gray-600 dark:text-gray-300 transition-colors duration-300">HIPAA compliant with enterprise-grade security and data protection.</p>
          </div>

          <!-- Feature 6 -->
          <div class="text-center p-6 rounded-xl hover:shadow-lg dark:hover:shadow-gray-700/50 transition-all duration-300 bg-white dark:bg-gray-700/50">
            <div class="w-16 h-16 bg-indigo-100 dark:bg-indigo-900/50 rounded-full flex items-center justify-center mx-auto mb-4 transition-colors duration-300">
              <svg class="w-8 h-8 text-indigo-600 dark:text-indigo-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 18h.01M8 21h8a2 2 0 002-2V5a2 2 0 00-2-2H8a2 2 0 00-2 2v14a2 2 0 002 2z" />
              </svg>
            </div>
            <h3 class="text-xl font-semibold text-gray-900 dark:text-white mb-2 transition-colors duration-300">Mobile Ready</h3>
            <p class="text-gray-600 dark:text-gray-300 transition-colors duration-300">Access your practice management tools anywhere with our mobile-responsive design.</p>
          </div>
        </div>
      </div>
    </section>

    <!-- CTA Section -->
    <section class="bg-gradient-to-r from-purple-600 to-indigo-600 py-16">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 text-center">
        <h2 class="text-3xl font-extrabold text-white mb-4">
          Ready to transform your practice?
        </h2>
        <p class="text-xl text-purple-100 dark:text-purple-200 mb-8 max-w-2xl mx-auto transition-colors duration-300">
          Join thousands of healthcare professionals who trust Aesclea for their practice management needs.
        </p>
        <router-link
          to="/register"
          class="inline-flex items-center px-8 py-4 border border-transparent text-lg font-medium rounded-lg text-purple-600 bg-white hover:bg-gray-50 dark:hover:bg-gray-100 transition-colors shadow-lg"
        >
          Start Your Free Trial
          <svg class="ml-2 -mr-1 w-5 h-5" fill="currentColor" viewBox="0 0 20 20">
            <path fill-rule="evenodd" d="M10.293 3.293a1 1 0 011.414 0l6 6a1 1 0 010 1.414l-6 6a1 1 0 01-1.414-1.414L14.586 11H3a1 1 0 110-2h11.586l-4.293-4.293a1 1 0 010-1.414z" clip-rule="evenodd" />
          </svg>
        </router-link>
      </div>
    </section>

    <!-- Footer -->
    <footer class="bg-gray-900 dark:bg-gray-950 text-white py-12 transition-colors duration-300">
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
            <p class="text-gray-400 dark:text-gray-500 mb-4 max-w-md transition-colors duration-300">
              Empowering healthcare professionals with modern, efficient, and secure practice management solutions.
            </p>
          </div>
          
          <div>
            <h4 class="text-lg font-semibold mb-4">Product</h4>
            <ul class="space-y-2 text-gray-400 dark:text-gray-500 transition-colors duration-300">
              <li><router-link to="/features" class="hover:text-white dark:hover:text-gray-300 transition-colors">Features</router-link></li>
              <li><router-link to="/pricing" class="hover:text-white dark:hover:text-gray-300 transition-colors">Pricing</router-link></li>
              <li><router-link to="/security" class="hover:text-white dark:hover:text-gray-300 transition-colors">Security</router-link></li>
              <li><router-link to="/updates" class="hover:text-white dark:hover:text-gray-300 transition-colors">Updates</router-link></li>
            </ul>
          </div>
          
          <div>
            <h4 class="text-lg font-semibold mb-4">Support</h4>
            <ul class="space-y-2 text-gray-400 dark:text-gray-500 transition-colors duration-300">
              <li><router-link to="/help" class="hover:text-white dark:hover:text-gray-300 transition-colors">Help Center</router-link></li>
              <li><router-link to="/contact" class="hover:text-white dark:hover:text-gray-300 transition-colors">Contact Us</router-link></li>
              <li><router-link to="/privacy" class="hover:text-white dark:hover:text-gray-300 transition-colors">Privacy Policy</router-link></li>
              <li><router-link to="/terms" class="hover:text-white dark:hover:text-gray-300 transition-colors">Terms of Service</router-link></li>
            </ul>
          </div>
        </div>
        
        <div class="border-t border-gray-800 dark:border-gray-700 mt-8 pt-8 text-center text-gray-400 dark:text-gray-500 transition-colors duration-300">
          <p>&copy; 2025 Aesclea Medical Management System. All rights reserved.</p>
        </div>
      </div>
    </footer>
  </div>
</template>

<script setup lang="ts">
import AppNavbar from '../components/AppNavbar.vue'
// Welcome page component for Aesclea Medical Management System
</script>
