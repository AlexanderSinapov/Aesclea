<!-- Copyright (c) 2025 Alexander Sinapov | Simeon Petkov -->

<!-- All rights reserved. -->
<!-- This code is proprietary and confidential.   -->
<!-- Unauthorized copying, modification, distribution, or use is strictly prohibited. -->

<template>
  <div class="flex flex-col min-h-screen bg-gradient-to-br from-purple-50 via-white to-indigo-50 dark:from-gray-900 dark:via-gray-900 dark:to-gray-800">
    <!-- Header -->
    <SiteNavbar />

    <!-- Main Chat Container -->
    <div class="flex flex-col flex-1 px-4 py-8 mx-auto max-w-7xl sm:px-6 lg:px-8">
      <!-- Header Section -->
      <div class="mb-8 text-center">
        <div class="inline-flex items-center justify-center w-16 h-16 mb-4 rounded-full bg-gradient-to-r from-purple-600 to-indigo-600">
          <svg class="w-10 h-10 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9.663 17h4.673M12 3v1m6.364 1.636l-.707.707M21 12h-1M4 12H3m3.343-5.657l-.707-.707m2.828 9.9a5 5 0 117.072 0l-.548.547A3.374 3.374 0 0014 18.469V19a2 2 0 11-4 0v-.531c0-.895-.356-1.754-.988-2.386l-.548-.547z" />
          </svg>
        </div>
        <h1 class="text-4xl font-bold text-gray-900 dark:text-white">
          Aesclea Health Assistant
        </h1>
        <p class="mt-2 text-lg text-gray-600 dark:text-gray-300">
          Get instant health information and guidance powered by advanced AI
        </p>
        <p class="mt-4 text-sm text-gray-500 dark:text-gray-400">
          Free to use • No account required (for now)
        </p>
      </div>

      <!-- Chat Container -->
      <div class="flex flex-col flex-1 overflow-hidden bg-white shadow-2xl dark:bg-gray-800 rounded-2xl">
        <!-- Messages Area -->
        <div ref="messagesContainer" class="flex-1 px-6 py-4 overflow-y-auto">
          <!-- Welcome Screen -->
          <div v-if="messages.length === 0" class="flex flex-col items-center justify-center h-full space-y-6">
            <div class="p-6 rounded-full bg-gradient-to-br from-purple-100 to-indigo-100 dark:from-purple-900/20 dark:to-indigo-900/20">
              <svg class="w-16 h-16 text-purple-600 dark:text-purple-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 10h.01M12 10h.01M16 10h.01M9 16H5a2 2 0 01-2-2V6a2 2 0 012-2h14a2 2 0 012 2v8a2 2 0 01-2 2h-5l-5 5v-5z" />
              </svg>
            </div>
            
            <div class="max-w-2xl text-center">
              <h3 class="text-2xl font-bold text-gray-900 dark:text-white">Welcome to Your Health Assistant</h3>
              <p class="mt-2 text-gray-600 dark:text-gray-300">
                I can help you understand symptoms, get health information, and provide guidance on when to seek medical care.
              </p>
            </div>

            <!-- Suggested Questions -->
            <div class="w-full max-w-3xl">
              <p class="mb-3 text-sm font-medium text-gray-700 dark:text-gray-300">Try asking about:</p>
              <div class="grid grid-cols-1 gap-3 md:grid-cols-2">
                <button
                  v-for="prompt in suggestedPrompts"
                  :key="prompt.text"
                  @click="useSuggestedPrompt(prompt.text)"
                  class="p-4 text-left transition-all duration-200 border border-gray-200 rounded-xl hover:border-purple-300 hover:shadow-md hover:shadow-purple-100 dark:border-gray-700 dark:hover:border-purple-600 group"
                >
                  <div class="flex items-start space-x-3">
                    <div class="flex-shrink-0 p-2 text-purple-600 transition-colors bg-purple-100 rounded-lg dark:bg-purple-900/30 dark:text-purple-400 group-hover:bg-purple-200 dark:group-hover:bg-purple-900/50">
                      <component :is="prompt.icon" class="w-5 h-5" />
                    </div>
                    <div>
                      <p class="text-sm font-medium text-gray-900 dark:text-white">{{ prompt.title }}</p>
                      <p class="mt-1 text-xs text-gray-600 dark:text-gray-400">{{ prompt.text }}</p>
                    </div>
                  </div>
                </button>
              </div>
            </div>

            <!-- Disclaimer -->
            <div class="max-w-2xl p-4 border border-yellow-200 rounded-lg bg-yellow-50 dark:bg-yellow-900/20 dark:border-yellow-800">
              <div class="flex items-start">
                <svg class="w-5 h-5 mt-0.5 mr-2 text-yellow-600 dark:text-yellow-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-2.5L13.732 4c-.77-.833-1.964-.833-2.732 0L3.732 16.5c-.77.833.192 2.5 1.732 2.5z" />
                </svg>
                <div class="flex-1 text-sm text-yellow-800 dark:text-yellow-200">
                  <strong>Important:</strong> This is for informational purposes only. Always consult with healthcare professionals for medical advice, diagnosis, or treatment.
                </div>
              </div>
            </div>
          </div>

          <!-- Chat Messages -->
          <div v-else class="space-y-6">
            <div
              v-for="(message, index) in messages"
              :key="index"
              class="flex"
              :class="message.role === 'user' ? 'justify-end' : 'justify-start'"
            >
              <div
                class="flex max-w-[85%] space-x-3"
                :class="message.role === 'user' ? 'flex-row-reverse space-x-reverse' : ''"
              >
                <!-- Avatar -->
                <div class="flex-shrink-0">
                  <div
                    v-if="message.role === 'assistant'"
                    class="flex items-center justify-center w-10 h-10 rounded-full bg-gradient-to-r from-purple-600 to-indigo-600"
                  >
                    <svg class="w-6 h-6 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9.663 17h4.673M12 3v1m6.364 1.636l-.707.707M21 12h-1M4 12H3m3.343-5.657l-.707-.707m2.828 9.9a5 5 0 117.072 0l-.548.547A3.374 3.374 0 0014 18.469V19a2 2 0 11-4 0v-.531c0-.895-.356-1.754-.988-2.386l-.548-.547z" />
                    </svg>
                  </div>
                  <div
                    v-else
                    class="flex items-center justify-center w-10 h-10 text-white bg-gray-500 rounded-full"
                  >
                    <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z" />
                    </svg>
                  </div>
                </div>

                <!-- Message Content -->
                <div class="flex-1 min-w-0">
                  <div
                    class="px-4 py-3 rounded-2xl"
                    :class="message.role === 'user' 
                      ? 'bg-gradient-to-r from-purple-600 to-indigo-600 text-white' 
                      : 'bg-gray-100 dark:bg-gray-700 text-gray-900 dark:text-white'"
                  >
                    <!-- User messages: simple text -->
                    <div v-if="message.role === 'user'" v-html="formatMessage(message.content)"></div>
                    
                    <!-- Assistant messages: structured analysis ONLY (no duplicate old text) -->
                    <div v-else class="space-y-4">
                      <!-- Only show content if there's no analysis (fallback/error messages) -->
                      <div v-if="!message.analysis && message.content" v-html="formatMessage(message.content)"></div>

                      <!-- Structured medical analysis from trained model -->
                      <div v-if="message.analysis" class="space-y-4 text-sm leading-relaxed">
                        <div class="p-4 border-l-4 border-purple-500 rounded-lg bg-purple-50 dark:bg-purple-900/40 dark:border-purple-400">
                          <p class="font-semibold text-purple-900 dark:text-purple-100">Primary Category: {{ message.analysis.primaryCategory }}</p>
                          <p class="mt-1 text-gray-700 dark:text-gray-200">
                            Severity Level {{ message.analysis.severityLevel }} · {{ message.analysis.severityDescription }}
                            <span v-if="message.analysis.severityConfidence !== undefined" class="text-xs text-gray-500">({{ formatPercentage(message.analysis.severityConfidence) }} confidence)</span>
                          </p>
                          <p class="mt-1 text-gray-700 dark:text-gray-200">
                            Urgency: {{ message.analysis.urgencyDescription }}
                            <span v-if="message.analysis.urgencyConfidence !== undefined" class="text-xs text-gray-500">({{ formatPercentage(message.analysis.urgencyConfidence) }})</span>
                          </p>
                        </div>

                        <div v-if="message.analysis.diagnosticConfidences && getTopConfidences(message.analysis.diagnosticConfidences).length" class="p-4 border border-gray-200 rounded-lg dark:border-gray-600">
                          <h4 class="font-semibold text-gray-800 dark:text-gray-100">Top Diagnostic Probabilities</h4>
                          <ul class="mt-2 space-y-1 text-gray-700 dark:text-gray-200">
                            <li v-for="([label, score], idx) in getTopConfidences(message.analysis.diagnosticConfidences)" :key="label" class="flex items-center justify-between">
                              <span>{{ idx + 1 }}. {{ label }}</span>
                              <span class="font-medium">{{ formatPercentage(score) }}</span>
                            </li>
                          </ul>
                        </div>

                        <div v-if="hasItems(message.analysis.clinicalAlerts)" class="p-4 border border-red-200 rounded-lg bg-red-50 dark:bg-red-900/30 dark:border-red-700">
                          <h4 class="font-semibold text-red-700 dark:text-red-200">Clinical Alerts</h4>
                          <ul class="mt-2 space-y-1 text-red-700 dark:text-red-100">
                            <li v-for="alert in message.analysis.clinicalAlerts" :key="alert">⚠️ {{ alert }}</li>
                          </ul>
                        </div>

                        <div v-if="hasItems(message.analysis.symptoms)" class="p-4 border border-gray-200 rounded-lg dark:border-gray-600">
                          <h4 class="font-semibold text-gray-800 dark:text-gray-100">Identified Symptoms</h4>
                          <div class="mt-2 flex flex-wrap gap-2">
                            <span v-for="symptom in message.analysis.symptoms" :key="symptom" class="px-2 py-1 text-xs font-medium text-purple-700 bg-purple-100 rounded-full dark:bg-purple-900/40 dark:text-purple-200">{{ symptom }}</span>
                          </div>
                        </div>

                        <div v-if="hasObjectEntries(message.analysis.symptomClusters)" class="p-4 border border-gray-200 rounded-lg dark:border-gray-600">
                          <h4 class="font-semibold text-gray-800 dark:text-gray-100">Symptom Clusters</h4>
                          <div class="grid gap-3 mt-2 md:grid-cols-2">
                            <div v-for="(symptoms, cluster) in message.analysis.symptomClusters" :key="cluster" class="p-3 rounded-lg bg-gray-50 dark:bg-gray-800/60">
                              <p class="text-sm font-semibold text-gray-700 dark:text-gray-200">{{ cluster }}</p>
                              <ul class="mt-1 space-y-1 text-xs text-gray-600 dark:text-gray-300">
                                <li v-for="symptom in symptoms" :key="symptom">• {{ symptom }}</li>
                              </ul>
                            </div>
                          </div>
                        </div>

                        <div v-if="hasObjectEntries(message.analysis.vitalSignsAnalysis)" class="p-4 border border-gray-200 rounded-lg dark:border-gray-600">
                          <h4 class="font-semibold text-gray-800 dark:text-gray-100">Vital Signs</h4>
                          <ul class="mt-2 space-y-1 text-gray-700 dark:text-gray-200">
                            <li v-for="(value, key) in message.analysis.vitalSignsAnalysis" :key="key">
                              <strong>{{ key.replace('_', ' ') }}:</strong> {{ value }}
                            </li>
                          </ul>
                        </div>

                        <div v-if="hasObjectEntries(message.analysis.temporalPatterns)" class="p-4 border border-gray-200 rounded-lg dark:border-gray-600">
                          <h4 class="font-semibold text-gray-800 dark:text-gray-100">Temporal Patterns</h4>
                          <ul class="mt-2 space-y-1 text-gray-700 dark:text-gray-200">
                            <li v-for="(value, key) in message.analysis.temporalPatterns" :key="key">
                              <strong>{{ key }}:</strong> {{ value }}
                            </li>
                          </ul>
                        </div>

                        <div v-if="hasObjectEntries(message.analysis.functionalImpact)" class="p-4 border border-gray-200 rounded-lg dark:border-gray-600">
                          <h4 class="font-semibold text-gray-800 dark:text-gray-100">Functional Impact</h4>
                          <ul class="mt-2 space-y-1 text-gray-700 dark:text-gray-200">
                            <li v-for="(value, key) in message.analysis.functionalImpact" :key="key">
                              <strong>{{ key }}:</strong> {{ value }}
                            </li>
                          </ul>
                        </div>

                        <div v-if="hasItems(message.analysis.riskFactors)" class="p-4 border border-gray-200 rounded-lg dark:border-gray-600">
                          <h4 class="font-semibold text-gray-800 dark:text-gray-100">Risk Factors</h4>
                          <ul class="mt-2 space-y-1 text-gray-700 dark:text-gray-200">
                            <li v-for="factor in message.analysis.riskFactors" :key="factor">• {{ factor }}</li>
                          </ul>
                        </div>

                        <div v-if="hasItems(message.analysis.medications)" class="p-4 border border-gray-200 rounded-lg dark:border-gray-600">
                          <h4 class="font-semibold text-gray-800 dark:text-gray-100">Medication Mentions</h4>
                          <div class="mt-2 flex flex-wrap gap-2">
                            <span v-for="med in message.analysis.medications" :key="med" class="px-2 py-1 text-xs font-medium text-indigo-700 bg-indigo-100 rounded-full dark:bg-indigo-900/40 dark:text-indigo-200">{{ med }}</span>
                          </div>
                        </div>

                        <div v-if="hasItems(message.analysis.differentialDiagnosis)" class="p-4 border border-gray-200 rounded-lg dark:border-gray-600">
                          <h4 class="font-semibold text-gray-800 dark:text-gray-100">Differential Diagnosis Considerations</h4>
                          <ul class="mt-2 space-y-1 text-gray-700 dark:text-gray-200">
                            <li v-for="item in message.analysis.differentialDiagnosis" :key="item">• {{ item }}</li>
                          </ul>
                        </div>

                        <div v-if="hasItems(message.analysis.suggestedTests)" class="p-4 border border-gray-200 rounded-lg dark:border-gray-600">
                          <h4 class="font-semibold text-gray-800 dark:text-gray-100">Recommended Diagnostic Tests</h4>
                          <ul class="mt-2 space-y-1 text-gray-700 dark:text-gray-200">
                            <li v-for="test in message.analysis.suggestedTests" :key="test">• {{ test }}</li>
                          </ul>
                        </div>

                        <div v-if="hasItems(message.analysis.recommendations)" class="p-4 border border-gray-200 rounded-lg dark:border-gray-600">
                          <h4 class="font-semibold text-gray-800 dark:text-gray-100">Clinical Recommendations</h4>
                          <ul class="mt-2 space-y-1 text-gray-700 dark:text-gray-200">
                            <li v-for="rec in message.analysis.recommendations" :key="rec">• {{ rec }}</li>
                          </ul>
                        </div>

                        <div v-if="message.analysis.summary" class="p-4 border border-gray-200 rounded-lg bg-gray-50 dark:bg-gray-800/60 dark:border-gray-600">
                          <h4 class="font-semibold text-gray-800 dark:text-gray-100">Summary</h4>
                          <p class="mt-2 text-gray-700 dark:text-gray-200">{{ message.analysis.summary }}</p>
                        </div>

                        <p class="text-xs text-gray-500 dark:text-gray-400">
                          This analysis is for informational purposes only. Always consult a licensed medical professional.
                        </p>
                      </div>
                    </div>
                  </div>
                  
                  <!-- Message Actions for AI responses -->
                  <div v-if="message.role === 'assistant'" class="flex items-center mt-2 space-x-2">
                    <button
                      @click="copyMessage(message)"
                      class="p-1 text-gray-400 transition-colors hover:text-gray-600 dark:hover:text-gray-300"
                      title="Copy"
                    >
                      <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 16H6a2 2 0 01-2-2V6a2 2 0 012-2h8a2 2 0 012 2v2m-6 12h8a2 2 0 002-2v-8a2 2 0 00-2-2h-8a2 2 0 00-2 2v8a2 2 0 002 2z" />
                      </svg>
                    </button>
                    <button
                      @click="shareMessage(message)"
                      class="p-1 text-gray-400 transition-colors hover:text-gray-600 dark:hover:text-gray-300"
                      title="Share"
                    >
                      <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8.684 13.342C8.886 12.938 9 12.482 9 12c0-.482-.114-.938-.316-1.342m0 2.684a3 3 0 110-2.684m0 2.684l6.632 3.316m-6.632-6l6.632-3.316m0 0a3 3 0 105.367-2.684 3 3 0 00-5.367 2.684zm0 9.316a3 3 0 105.368 2.684 3 3 0 00-5.368-2.684z" />
                      </svg>
                    </button>
                  </div>
                  
                  <p class="mt-1 text-xs text-gray-500 dark:text-gray-400">
                    {{ formatTime(message.timestamp) }}
                  </p>
                </div>
              </div>
            </div>

            <!-- Typing Indicator -->
            <div v-if="isTyping" class="flex justify-start">
              <div class="flex max-w-[85%] space-x-3">
                <div class="flex items-center justify-center w-10 h-10 rounded-full bg-gradient-to-r from-purple-600 to-indigo-600">
                  <svg class="w-6 h-6 text-white animate-pulse" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9.663 17h4.673M12 3v1m6.364 1.636l-.707.707M21 12h-1M4 12H3m3.343-5.657l-.707-.707m2.828 9.9a5 5 0 117.072 0l-.548.547A3.374 3.374 0 0014 18.469V19a2 2 0 11-4 0v-.531c0-.895-.356-1.754-.988-2.386l-.548-.547z" />
                  </svg>
                </div>
                <div class="flex-1 space-y-3">
                  <div class="px-4 py-3 bg-gray-100 dark:bg-gray-700 rounded-2xl">
                    <div class="flex items-center space-x-2">
                      <div class="flex space-x-1.5">
                        <div class="w-2.5 h-2.5 bg-purple-500 rounded-full animate-bounce" style="animation-delay: 0s; animation-duration: 1.4s;"></div>
                        <div class="w-2.5 h-2.5 bg-purple-500 rounded-full animate-bounce" style="animation-delay: 0.2s; animation-duration: 1.4s;"></div>
                        <div class="w-2.5 h-2.5 bg-purple-500 rounded-full animate-bounce" style="animation-delay: 0.4s; animation-duration: 1.4s;"></div>
                      </div>
                      <span class="text-sm text-gray-600 dark:text-gray-300 animate-pulse">Analyzing medical information...</span>
                    </div>
                  </div>
                  
                  <!-- Skeleton loading for analysis sections -->
                  <div class="space-y-2 animate-pulse">
                    <div class="h-20 bg-gray-200 dark:bg-gray-600 rounded-lg"></div>
                    <div class="h-16 bg-gray-200 dark:bg-gray-600 rounded-lg"></div>
                    <div class="h-12 bg-gray-200 dark:bg-gray-600 rounded-lg"></div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Input Area -->
        <div class="p-4 border-t border-gray-200 dark:border-gray-700">
          <div class="flex items-end space-x-2">
            <div class="flex-1">
              <textarea
                v-model="userInput"
                @keydown.enter.exact.prevent="sendMessage"
                @keydown.shift.enter.exact="userInput += '\n'"
                rows="1"
                placeholder="Ask about symptoms, conditions, or health topics..."
                class="w-full px-4 py-3 text-sm border border-gray-300 rounded-lg resize-none dark:border-gray-600 dark:bg-gray-700 dark:text-white focus:outline-none focus:ring-2 focus:ring-purple-500 focus:border-transparent"
                :disabled="isTyping"
              ></textarea>
            </div>

            <button
              @click="sendMessage"
              :disabled="!userInput.trim() || isTyping"
              class="p-3 text-white transition-all duration-200 rounded-lg bg-gradient-to-r from-purple-600 to-indigo-600 hover:from-purple-700 hover:to-indigo-700 disabled:opacity-50 disabled:cursor-not-allowed shadow-lg shadow-purple-500/30 hover:shadow-purple-500/50"
            >
              <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 19l9 2-9-18-9 18 9-2zm0 0v-8" />
              </svg>
            </button>
          </div>
          <p class="mt-2 text-xs text-center text-gray-500 dark:text-gray-400">
            Press Enter to send, Shift+Enter for new line
          </p>
        </div>
      </div>

      <!-- Login CTA (for future requirement) -->
      <div v-if="messages.length > 3" class="p-4 mt-6 border border-purple-200 rounded-lg bg-purple-50 dark:bg-purple-900/20 dark:border-purple-700">
        <div class="flex items-center justify-between">
          <div class="flex-1">
            <h3 class="text-sm font-semibold text-purple-900 dark:text-purple-200">Want to save your conversation history?</h3>
            <p class="mt-1 text-xs text-purple-700 dark:text-purple-300">Create a free account to access advanced features and save your chats.</p>
          </div>
          <router-link
            to="/register"
            class="px-4 py-2 ml-4 text-sm font-medium text-white transition-all rounded-lg bg-gradient-to-r from-purple-600 to-indigo-600 hover:from-purple-700 hover:to-indigo-700"
          >
            Sign Up Free
          </router-link>
        </div>
      </div>
    </div>

    <!-- Footer -->
    <SiteFooter />
  </div>
</template>

<script setup lang="ts">
import { ref, nextTick, h } from 'vue'
import SiteNavbar from '../components/SiteNavbar.vue'
import SiteFooter from '../components/SiteFooter.vue'
import { getEndpointUrl } from '../config/api'

interface MedicalAnalysisResponse {
  primaryCategory: string
  severityLevel: number
  severityDescription: string
  severityConfidence?: number
  urgencyLevel: number
  urgencyDescription: string
  urgencyConfidence?: number
  diagnosticConfidences?: Record<string, number>
  clinicalAlerts?: string[]
  symptoms?: string[]
  symptomClusters?: Record<string, string[]>
  vitalSignsAnalysis?: Record<string, string>
  temporalPatterns?: Record<string, string>
  functionalImpact?: Record<string, string>
  riskFactors?: string[]
  medications?: string[]
  differentialDiagnosis?: string[]
  suggestedTests?: string[]
  recommendations?: string[]
  summary?: string
  processedOn?: string
}

interface Message {
  role: 'user' | 'assistant'
  content?: string
  timestamp: Date
  analysis?: MedicalAnalysisResponse
}

// State
const messages = ref<Message[]>([])
const userInput = ref('')
const isTyping = ref(false)
const messagesContainer = ref<HTMLElement | null>(null)

// Suggested prompts for public users
const suggestedPrompts = [
  {
    icon: h('svg', { fill: 'none', stroke: 'currentColor', viewBox: '0 0 24 24' }, [
      h('path', { 'stroke-linecap': 'round', 'stroke-linejoin': 'round', 'stroke-width': '2', d: 'M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z' })
    ]),
    title: 'Common Cold vs Flu',
    text: 'What are the differences between a cold and the flu?'
  },
  {
    icon: h('svg', { fill: 'none', stroke: 'currentColor', viewBox: '0 0 24 24' }, [
      h('path', { 'stroke-linecap': 'round', 'stroke-linejoin': 'round', 'stroke-width': '2', d: 'M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z' })
    ]),
    title: 'When to See a Doctor',
    text: 'When should I seek medical attention for a headache?'
  },
  {
    icon: h('svg', { fill: 'none', stroke: 'currentColor', viewBox: '0 0 24 24' }, [
      h('path', { 'stroke-linecap': 'round', 'stroke-linejoin': 'round', 'stroke-width': '2', d: 'M4.318 6.318a4.5 4.5 0 000 6.364L12 20.364l7.682-7.682a4.5 4.5 0 00-6.364-6.364L12 7.636l-1.318-1.318a4.5 4.5 0 00-6.364 0z' })
    ]),
    title: 'Healthy Lifestyle',
    text: 'What are some tips for maintaining a healthy lifestyle?'
  },
  {
    icon: h('svg', { fill: 'none', stroke: 'currentColor', viewBox: '0 0 24 24' }, [
      h('path', { 'stroke-linecap': 'round', 'stroke-linejoin': 'round', 'stroke-width': '2', d: 'M13 10V3L4 14h7v7l9-11h-7z' })
    ]),
    title: 'First Aid',
    text: 'What should I do for a minor burn or cut?'
  }
]

const sendMessage = async () => {
  if (!userInput.value.trim() || isTyping.value) return

  // Add user message
  const userMessage: Message = {
    role: 'user',
    content: userInput.value.trim(),
    timestamp: new Date()
  }

  messages.value.push(userMessage)
  const messageText = userInput.value
  userInput.value = ''
  
  nextTick(() => {
    scrollToBottom()
  })

  // Call AI API
  isTyping.value = true
  try {
    const apiUrl = getEndpointUrl('aiChat')
    const response = await fetch(apiUrl, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({
        message: messageText,
        context: null // No patient context for public users
      })
    })

    if (!response.ok) {
      throw new Error('Failed to get AI response')
    }

    const data: { message?: string; analysis?: MedicalAnalysisResponse } = await response.json()

    const aiMessage: Message = {
      role: 'assistant',
      content: data.message?.trim(),
      analysis: data.analysis,
      timestamp: new Date()
    }
    
    messages.value.push(aiMessage)
    isTyping.value = false
    
    nextTick(() => {
      scrollToBottom()
    })
  } catch (error) {
    console.error('Error calling AI API:', error)
    
    // Fallback response
    const fallbackMessage: Message = {
      role: 'assistant',
      content: 'I apologize, but I\'m having trouble connecting right now. Please try again in a moment, or consider creating an account for a better experience with our full AI health assistant.',
      timestamp: new Date()
    }
    messages.value.push(fallbackMessage)
    isTyping.value = false
    
    nextTick(() => {
      scrollToBottom()
    })
  }
}

const useSuggestedPrompt = (text: string) => {
  userInput.value = text
  sendMessage()
}

const getTopConfidences = (confidences?: Record<string, number>, limit = 5) => {
  if (!confidences) return []
  return Object.entries(confidences)
    .sort((a, b) => (b[1] ?? 0) - (a[1] ?? 0))
    .slice(0, limit)
}

const hasItems = (value?: unknown[] | null): value is unknown[] => Array.isArray(value) && value.length > 0

const hasObjectEntries = (value?: Record<string, unknown> | null): value is Record<string, unknown> => !!value && Object.keys(value).length > 0

const formatPercentage = (value?: number): string => {
  if (value === undefined || value === null || Number.isNaN(value)) return 'N/A'
  return `${(value * 100).toFixed(1)}%`
}

const buildAnalysisText = (analysis?: MedicalAnalysisResponse): string => {
  if (!analysis) return ''
  const lines: string[] = []

  lines.push(`Primary Category: ${analysis.primaryCategory}`)
  lines.push(`Severity: Level ${analysis.severityLevel} - ${analysis.severityDescription} (${formatPercentage(analysis.severityConfidence)})`)
  lines.push(`Urgency: ${analysis.urgencyDescription} (${formatPercentage(analysis.urgencyConfidence)})`)

  if (hasItems(analysis.clinicalAlerts)) {
    lines.push('Clinical Alerts:')
    analysis.clinicalAlerts!.forEach(alert => lines.push(` - ${alert}`))
  }

  if (hasItems(analysis.symptoms)) {
    lines.push('Symptoms:')
    analysis.symptoms!.forEach(symptom => lines.push(` - ${symptom}`))
  }

  if (analysis.diagnosticConfidences) {
    lines.push('Top Diagnostic Probabilities:')
    getTopConfidences(analysis.diagnosticConfidences).forEach(([label, score]) => {
      lines.push(` - ${label}: ${formatPercentage(score)}`)
    })
  }

  if (hasItems(analysis.differentialDiagnosis)) {
    lines.push('Differential Diagnosis:')
    analysis.differentialDiagnosis!.forEach(item => lines.push(` - ${item}`))
  }

  if (hasItems(analysis.suggestedTests)) {
    lines.push('Suggested Tests:')
    analysis.suggestedTests!.forEach(test => lines.push(` - ${test}`))
  }

  if (hasItems(analysis.recommendations)) {
    lines.push('Recommendations:')
    analysis.recommendations!.forEach(rec => lines.push(` - ${rec}`))
  }

  if (hasItems(analysis.riskFactors)) {
    lines.push('Risk Factors:')
    analysis.riskFactors!.forEach(factor => lines.push(` - ${factor}`))
  }

  if (hasItems(analysis.medications)) {
    lines.push('Medication Mentions:')
    analysis.medications!.forEach(med => lines.push(` - ${med}`))
  }

  if (hasObjectEntries(analysis.vitalSignsAnalysis)) {
    lines.push('Vital Signs:')
    Object.entries(analysis.vitalSignsAnalysis!).forEach(([key, value]) => {
      lines.push(` - ${key}: ${value}`)
    })
  }

  if (analysis.summary) {
    lines.push('Summary:')
    lines.push(analysis.summary)
  }

  return lines.join('\n')
}

const buildPlainText = (message: Message): string => {
  const parts: string[] = []
  if (message.content) parts.push(message.content)
  const analysisText = buildAnalysisText(message.analysis)
  if (analysisText) parts.push(analysisText)
  return parts.join('\n\n')
}

const formatMessage = (content?: string): string => {
  if (!content) return ''
  let formatted = content
    .replace(/\*\*(.*?)\*\*/g, '<strong>$1</strong>')
    .replace(/\*(.*?)\*/g, '<em>$1</em>')
    .replace(/\n/g, '<br>')

  return formatted
}

const formatTime = (date: Date): string => {
  const now = new Date()
  const diff = now.getTime() - date.getTime()
  const minutes = Math.floor(diff / 60000)
  const hours = Math.floor(diff / 3600000)
  
  if (minutes < 1) return 'Just now'
  if (minutes < 60) return `${minutes}m ago`
  if (hours < 24) return `${hours}h ago`
  return date.toLocaleDateString()
}

const scrollToBottom = () => {
  if (messagesContainer.value) {
    messagesContainer.value.scrollTop = messagesContainer.value.scrollHeight
  }
}

const copyMessage = (message: Message) => {
  const plain = buildPlainText(message)
  navigator.clipboard.writeText(plain)
  // Could add a toast notification here
}

const shareMessage = (message: Message) => {
  const plain = buildPlainText(message)
  if (navigator.share) {
    navigator.share({
      title: 'Aesclea Health Assistant',
      text: plain
    })
  } else {
    copyMessage(message)
  }
}
</script>

<style scoped>
@keyframes bounce {
  0%, 100% {
    transform: translateY(0);
  }
  50% {
    transform: translateY(-0.5rem);
  }
}

.animate-bounce {
  animation: bounce 1s infinite;
}
</style>
