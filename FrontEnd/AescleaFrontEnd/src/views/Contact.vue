<template>
  <div class="min-h-screen transition-colors duration-300">
    <div class="min-h-screen bg-gradient-to-br from-purple-50 via-blue-50 to-indigo-100 dark:from-gray-900 dark:via-gray-800 dark:to-gray-900">
      <!-- Header -->
      <AppNavbar />

      <!-- Hero Section -->
      <section class="relative overflow-hidden">
        <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-16">
          <div class="text-center">
            <h1 class="text-4xl sm:text-5xl font-extrabold text-gray-900 dark:text-white mb-6">
              <span class="block">Get in Touch</span>
              <span class="block text-transparent bg-clip-text bg-gradient-to-r from-purple-600 to-indigo-600">
                We're Here to Help
              </span>
            </h1>
            <p class="max-w-3xl mx-auto text-xl text-gray-600 dark:text-gray-300 mb-10">
              Have questions about Aesclea? Need support with your account? Want to discuss enterprise solutions?
              Our team is ready to assist you.
            </p>
          </div>
        </div>
      </section>

      <!-- Contact Form & Info -->
      <section class="py-20 bg-white dark:bg-gray-800">
        <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div class="grid grid-cols-1 lg:grid-cols-2 gap-16">
            
            <!-- Contact Form -->
            <div>
              <h2 class="text-3xl font-bold text-gray-900 dark:text-white mb-8">Send us a Message</h2>
              <form class="space-y-6">
                <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
                  <div>
                    <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                      First Name
                    </label>
                    <input
                      type="text"
                      class="w-full px-4 py-3 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:border-transparent"
                      placeholder="John"
                    />
                  </div>
                  <div>
                    <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                      Last Name
                    </label>
                    <input
                      type="text"
                      class="w-full px-4 py-3 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:border-transparent"
                      placeholder="Doe"
                    />
                  </div>
                </div>
                
                <div>
                  <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                    Email Address
                  </label>
                  <input
                    type="email"
                    class="w-full px-4 py-3 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:border-transparent"
                    placeholder="john.doe@hospital.com"
                  />
                </div>
                
                <div>
                  <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                    Organization
                  </label>
                  <input
                    type="text"
                    class="w-full px-4 py-3 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:border-transparent"
                    placeholder="Your Hospital or Clinic"
                  />
                </div>
                
                <div>
                  <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                    Subject
                  </label>
                  <select class="w-full px-4 py-3 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:border-transparent">
                    <option>General Inquiry</option>
                    <option>Technical Support</option>
                    <option>Sales Question</option>
                    <option>Enterprise Solutions</option>
                    <option>Partnership Opportunities</option>
                    <option>Security Inquiry</option>
                  </select>
                </div>
                
                <div>
                  <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                    Message
                  </label>
                  <textarea
                    rows="6"
                    class="w-full px-4 py-3 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:border-transparent"
                    placeholder="Tell us how we can help you..."
                  ></textarea>
                </div>
                
                <div class="flex items-center">
                  <input
                    type="checkbox"
                    class="h-4 w-4 text-indigo-600 focus:ring-indigo-500 border-gray-300 rounded"
                  />
                  <label class="ml-2 block text-sm text-gray-700 dark:text-gray-300">
                    I agree to receive communications from Aesclea about products and services.
                  </label>
                </div>
                
                <button
                  type="submit"
                  class="w-full bg-gradient-to-r from-purple-600 to-indigo-600 text-white py-3 px-6 rounded-lg font-medium hover:from-purple-700 hover:to-indigo-700 transition-all duration-200 shadow-lg hover:shadow-xl"
                >
                  Send Message
                </button>
              </form>
            </div>
            
            <!-- Contact Information -->
            <div>
              <h2 class="text-3xl font-bold text-gray-900 dark:text-white mb-8">Contact Information</h2>
              
              <div class="space-y-8">
                <div class="flex items-start">
                  <div class="flex-shrink-0 w-12 h-12 bg-gradient-to-r from-blue-500 to-indigo-600 rounded-lg flex items-center justify-center">
                    <svg class="w-6 h-6 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 8l7.89 4.26a2 2 0 002.22 0L21 8M5 19h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v10a2 2 0 002 2z" />
                    </svg>
                  </div>
                  <div class="ml-4">
                    <h3 class="text-lg font-semibold text-gray-900 dark:text-white">Email</h3>
                    <p class="text-gray-600 dark:text-gray-300 mt-1">Send us an email and we'll respond within 24 hours</p>
                    <a href="mailto:contact@aesclea.com" class="text-indigo-600 dark:text-indigo-400 font-medium hover:text-indigo-500 transition-colors">
                      contact@aesclea.com
                    </a>
                  </div>
                </div>
                
                <div class="flex items-start">
                  <div class="flex-shrink-0 w-12 h-12 bg-gradient-to-r from-green-500 to-emerald-600 rounded-lg flex items-center justify-center">
                    <svg class="w-6 h-6 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 5a2 2 0 012-2h3.28a1 1 0 01.948.684l1.498 4.493a1 1 0 01-.502 1.21l-2.257 1.13a11.042 11.042 0 005.516 5.516l1.13-2.257a1 1 0 011.21-.502l4.493 1.498a1 1 0 01.684.949V19a2 2 0 01-2 2h-1C9.716 21 3 14.284 3 6V5z" />
                    </svg>
                  </div>
                  <div class="ml-4">
                    <h3 class="text-lg font-semibold text-gray-900 dark:text-white">Phone</h3>
                    <p class="text-gray-600 dark:text-gray-300 mt-1">Call us during business hours for immediate assistance</p>
                    <a href="tel:+1-800-AESCLEA" class="text-indigo-600 dark:text-indigo-400 font-medium hover:text-indigo-500 transition-colors">
                      +1 (800) AESCLEA
                    </a>
                    <p class="text-sm text-gray-500 dark:text-gray-400 mt-1">Monday - Friday, 9am - 6pm EST</p>
                  </div>
                </div>
                
                <div class="flex items-start">
                  <div class="flex-shrink-0 w-12 h-12 bg-gradient-to-r from-purple-500 to-pink-600 rounded-lg flex items-center justify-center">
                    <svg class="w-6 h-6 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17.657 16.657L13.414 20.9a1.998 1.998 0 01-2.827 0l-4.244-4.243a8 8 0 1111.314 0z" />
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 11a3 3 0 11-6 0 3 3 0 016 0z" />
                    </svg>
                  </div>
                  <div class="ml-4">
                    <h3 class="text-lg font-semibold text-gray-900 dark:text-white">Office</h3>
                    <p class="text-gray-600 dark:text-gray-300 mt-1">Visit our headquarters for in-person meetings</p>
                    <address class="text-indigo-600 dark:text-indigo-400 not-italic">
                      123 Healthcare Drive<br />
                      Medical District<br />
                      San Francisco, CA 94105
                    </address>
                  </div>
                </div>
                
                <div class="flex items-start">
                  <div class="flex-shrink-0 w-12 h-12 bg-gradient-to-r from-orange-500 to-red-600 rounded-lg flex items-center justify-center">
                    <svg class="w-6 h-6 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 12h.01M12 12h.01M16 12h.01M21 12c0 4.418-4.03 8-9 8a9.863 9.863 0 01-4.255-.949L3 20l1.395-3.72C3.512 15.042 3 13.574 3 12c0-4.418 4.03-8 9-8s9 3.582 9 8z" />
                    </svg>
                  </div>
                  <div class="ml-4">
                    <h3 class="text-lg font-semibold text-gray-900 dark:text-white">Live Chat</h3>
                    <p class="text-gray-600 dark:text-gray-300 mt-1">Chat with our support team in real-time</p>
                    <button class="text-indigo-600 dark:text-indigo-400 font-medium hover:text-indigo-500 transition-colors">
                      Start Live Chat
                    </button>
                    <p class="text-sm text-gray-500 dark:text-gray-400 mt-1">Available 24/7</p>
                  </div>
                </div>
              </div>
              
              <!-- Emergency Contact -->
              <div class="mt-12 p-6 bg-gradient-to-r from-red-50 to-orange-50 dark:from-gray-700 dark:to-gray-600 rounded-xl">
                <h3 class="text-lg font-semibold text-gray-900 dark:text-white mb-2">Emergency Support</h3>
                <p class="text-gray-600 dark:text-gray-300 mb-4">
                  For critical system issues affecting patient care, contact our emergency support line 24/7.
                </p>
                <a 
                  href="tel:+1-800-911-HELP" 
                  class="inline-flex items-center px-4 py-2 bg-red-600 text-white rounded-lg font-medium hover:bg-red-700 transition-colors"
                >
                  <svg class="w-4 h-4 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-2.5L13.732 4c-.77-.833-1.732-.833-2.464 0L4.35 16.5c-.77.833.192 2.5 1.732 2.5z" />
                  </svg>
                  Emergency: +1 (800) 911-HELP
                </a>
              </div>
            </div>
          </div>
        </div>
      </section>

      <!-- Sales Team -->
      <section class="py-20 bg-gradient-to-r from-purple-600 to-indigo-600">
        <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 text-center">
          <h2 class="text-3xl font-extrabold text-white mb-4">
            Ready to Transform Your Practice?
          </h2>
          <p class="text-xl text-purple-100 mb-8 max-w-2xl mx-auto">
            Speak with our sales team to learn how Aesclea can be customized for your specific needs.
          </p>
          <div class="flex flex-col sm:flex-row gap-4 justify-center">
            <a
              href="mailto:sales@aesclea.com"
              class="inline-flex items-center px-8 py-4 border border-transparent text-lg font-medium rounded-lg text-purple-600 bg-white hover:bg-gray-50 transition-colors shadow-lg"
            >
              Contact Sales
              <svg class="ml-2 -mr-1 w-5 h-5" fill="currentColor" viewBox="0 0 20 20">
                <path fill-rule="evenodd" d="M10.293 3.293a1 1 0 011.414 0l6 6a1 1 0 010 1.414l-6 6a1 1 0 01-1.414-1.414L14.586 11H3a1 1 0 110-2h11.586l-4.293-4.293a1 1 0 010-1.414z" clip-rule="evenodd" />
              </svg>
            </a>
            <button class="inline-flex items-center px-8 py-4 border-2 border-white text-lg font-medium rounded-lg text-white hover:bg-white hover:text-purple-600 transition-all duration-200">
              Schedule Demo
            </button>
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
import { onMounted } from 'vue'
import { useTheme } from '../composables/useTheme'
import AppNavbar from '../components/AppNavbar.vue'

const { isDark, toggleTheme, initializeTheme } = useTheme()

onMounted(() => {
  initializeTheme()
})
</script>
