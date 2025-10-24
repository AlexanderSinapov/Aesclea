<!-- Copyright (c) 2025 Alexander Sinapov | Simeon Petkov -->

<!-- All rights reserved. -->
<!-- This code is proprietary and confidential.   -->
<!-- Unauthorized copying, modification, distribution, or use is strictly prohibited. -->

<template>
  <div class="flex h-full overflow-hidden bg-white dark:bg-gray-900">
    <!-- Conversations Sidebar -->
    <div 
      class="flex-shrink-0 w-64 overflow-y-auto border-r border-gray-200 dark:border-gray-700"
      :class="{'hidden md:block': !showConversations}"
    >
      <div class="sticky top-0 z-10 p-4 border-b border-gray-200 bg-white/80 dark:bg-gray-900/80 backdrop-blur-sm dark:border-gray-700">
        <button
          @click="createNewConversation"
          class="flex items-center justify-center w-full px-4 py-2.5 text-sm font-medium text-white transition-all duration-200 rounded-lg bg-gradient-to-r from-purple-600 to-indigo-600 hover:from-purple-700 hover:to-indigo-700 shadow-lg shadow-purple-500/30 hover:shadow-purple-500/50"
        >
          <svg class="w-5 h-5 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4v16m8-8H4" />
          </svg>
          New Conversation
        </button>
      </div>

      <div class="p-2 space-y-1">
        <div
          v-for="conv in conversations"
          :key="conv.id"
          @click="selectConversation(conv.id)"
          class="relative p-3 transition-all duration-200 rounded-lg cursor-pointer group"
          :class="currentConversationId === conv.id 
            ? 'bg-gradient-to-r from-purple-50 to-indigo-50 dark:from-purple-900/20 dark:to-indigo-900/20 border border-purple-200 dark:border-purple-700' 
            : 'hover:bg-gray-50 dark:hover:bg-gray-800'"
        >
          <div class="flex items-start justify-between">
            <div class="flex-1 min-w-0">
              <p class="text-sm font-medium text-gray-900 truncate dark:text-white">
                {{ conv.title }}
              </p>
              <p class="text-xs text-gray-500 dark:text-gray-400">
                {{ formatDate(conv.updatedAt) }}
              </p>
              <p v-if="conv.lastMessage" class="mt-1 text-xs text-gray-600 truncate dark:text-gray-300">
                {{ conv.lastMessage }}
              </p>
            </div>
            <button
              @click.stop="deleteConversation(conv.id)"
              class="p-1 ml-2 text-gray-400 transition-opacity opacity-0 hover:text-red-500 group-hover:opacity-100"
            >
              <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
              </svg>
            </button>
          </div>
        </div>

        <div v-if="conversations.length === 0" class="p-8 text-center">
          <svg class="w-12 h-12 mx-auto text-gray-300 dark:text-gray-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 12h.01M12 12h.01M16 12h.01M21 12c0 4.418-4.03 8-9 8a9.863 9.863 0 01-4.255-.949L3 20l1.395-3.72C3.512 15.042 3 13.574 3 12c0-4.418 4.03-8 9-8s9 3.582 9 8z" />
          </svg>
          <p class="mt-3 text-sm text-gray-500 dark:text-gray-400">No conversations yet</p>
        </div>
      </div>
    </div>

    <!-- Main Chat Area -->
    <div class="flex flex-col flex-1 min-w-0">
      <!-- Chat Header -->
      <div class="sticky top-0 z-10 flex items-center justify-between px-6 py-4 border-b border-gray-200 bg-white/80 dark:bg-gray-900/80 backdrop-blur-sm dark:border-gray-700">
        <div class="flex items-center space-x-3">
          <button
            @click="showConversations = !showConversations"
            class="p-2 text-gray-500 rounded-lg md:hidden hover:bg-gray-100 dark:hover:bg-gray-800"
          >
            <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 6h16M4 12h16M4 18h16" />
            </svg>
          </button>
          <div class="flex items-center justify-center w-10 h-10 rounded-full bg-gradient-to-r from-purple-600 to-indigo-600">
            <svg class="w-6 h-6 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9.663 17h4.673M12 3v1m6.364 1.636l-.707.707M21 12h-1M4 12H3m3.343-5.657l-.707-.707m2.828 9.9a5 5 0 117.072 0l-.548.547A3.374 3.374 0 0014 18.469V19a2 2 0 11-4 0v-.531c0-.895-.356-1.754-.988-2.386l-.548-.547z" />
            </svg>
          </div>
          <div>
            <h2 class="text-lg font-semibold text-gray-900 dark:text-white">Medical AI Assistant</h2>
            <p class="text-xs text-gray-500 dark:text-gray-400">Powered by advanced medical AI</p>
          </div>
        </div>
        
        <div class="flex items-center space-x-2">
          <button
            @click="showContext = !showContext"
            class="p-2 text-gray-500 transition-colors rounded-lg hover:bg-gray-100 dark:hover:bg-gray-800"
            :class="{'bg-purple-100 text-purple-600 dark:bg-purple-900/30 dark:text-purple-400': showContext}"
            title="Toggle Context Panel"
          >
            <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
            </svg>
          </button>
          <button
            @click="exportConversation"
            class="p-2 text-gray-500 transition-colors rounded-lg hover:bg-gray-100 dark:hover:bg-gray-800"
            title="Export Conversation"
          >
            <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 10v6m0 0l-3-3m3 3l3-3m2 8H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
            </svg>
          </button>
          <button
            @click="clearConversation"
            class="p-2 text-gray-500 transition-colors rounded-lg hover:bg-gray-100 dark:hover:bg-gray-800"
            title="Clear Conversation"
          >
            <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
            </svg>
          </button>
        </div>
      </div>

      <div class="flex flex-1 min-h-0">
        <!-- Messages Area -->
        <div class="flex flex-col flex-1 min-w-0">
          <!-- Messages -->
          <div ref="messagesContainer" class="flex-1 px-6 py-4 overflow-y-auto">
            <!-- Welcome Screen -->
            <div v-if="currentMessages.length === 0" class="flex flex-col items-center justify-center h-full space-y-6">
              <div class="p-6 rounded-full bg-gradient-to-br from-purple-100 to-indigo-100 dark:from-purple-900/20 dark:to-indigo-900/20">
                <svg class="w-16 h-16 text-purple-600 dark:text-purple-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9.663 17h4.673M12 3v1m6.364 1.636l-.707.707M21 12h-1M4 12H3m3.343-5.657l-.707-.707m2.828 9.9a5 5 0 117.072 0l-.548.547A3.374 3.374 0 0014 18.469V19a2 2 0 11-4 0v-.531c0-.895-.356-1.754-.988-2.386l-.548-.547z" />
                </svg>
              </div>
              
              <div class="text-center">
                <h3 class="text-2xl font-bold text-gray-900 dark:text-white">Welcome to Aesclea Medical AI</h3>
                <p class="mt-2 text-gray-600 dark:text-gray-300">Your intelligent medical assistant, ready to help with diagnoses, treatment plans, and medical research.</p>
              </div>

              <!-- Suggested Prompts -->
              <div class="w-full max-w-3xl">
                <p class="mb-3 text-sm font-medium text-gray-700 dark:text-gray-300">Suggested prompts:</p>
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
            </div>

            <!-- Chat Messages -->
            <div v-else class="space-y-6">
              <div
                v-for="(message, index) in currentMessages"
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
                      <span class="text-sm font-medium">{{ userInitials }}</span>
                    </div>
                  </div>

                  <!-- Message Content -->
                  <div class="flex-1 min-w-0">
                    <div
                      class="px-4 py-3 rounded-2xl"
                      :class="message.role === 'user' 
                        ? 'bg-gradient-to-r from-purple-600 to-indigo-600 text-white' 
                        : 'bg-gray-100 dark:bg-gray-800 text-gray-900 dark:text-white'"
                    >
                      <div v-if="message.image" class="mb-3">
                        <img :src="message.image" class="max-w-full rounded-lg" alt="Uploaded image" />
                      </div>
                      <div v-html="formatMessage(message.content)"></div>
                    </div>
                    
                    <!-- Message Actions -->
                    <div v-if="message.role === 'assistant'" class="flex items-center mt-2 space-x-2">
                      <button
                        @click="copyMessage(message.content)"
                        class="p-1 text-gray-400 transition-colors hover:text-gray-600 dark:hover:text-gray-300"
                        title="Copy"
                      >
                        <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 16H6a2 2 0 01-2-2V6a2 2 0 012-2h8a2 2 0 012 2v2m-6 12h8a2 2 0 002-2v-8a2 2 0 00-2-2h-8a2 2 0 00-2 2v8a2 2 0 002 2z" />
                        </svg>
                      </button>
                      <button
                        @click="saveToPatientFile(message.content)"
                        class="p-1 text-gray-400 transition-colors hover:text-gray-600 dark:hover:text-gray-300"
                        title="Save to Patient File"
                      >
                        <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 7H5a2 2 0 00-2 2v9a2 2 0 002 2h14a2 2 0 002-2V9a2 2 0 00-2-2h-3m-1 4l-3 3m0 0l-3-3m3 3V4" />
                        </svg>
                      </button>
                      <button
                        @click="regenerateResponse(index)"
                        class="p-1 text-gray-400 transition-colors hover:text-gray-600 dark:hover:text-gray-300"
                        title="Regenerate"
                      >
                        <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15" />
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
                    <svg class="w-6 h-6 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9.663 17h4.673M12 3v1m6.364 1.636l-.707.707M21 12h-1M4 12H3m3.343-5.657l-.707-.707m2.828 9.9a5 5 0 117.072 0l-.548.547A3.374 3.374 0 0014 18.469V19a2 2 0 11-4 0v-.531c0-.895-.356-1.754-.988-2.386l-.548-.547z" />
                    </svg>
                  </div>
                  <div class="px-4 py-3 bg-gray-100 dark:bg-gray-800 rounded-2xl">
                    <div class="flex space-x-2">
                      <div class="w-2 h-2 bg-gray-400 rounded-full animate-bounce" style="animation-delay: 0s"></div>
                      <div class="w-2 h-2 bg-gray-400 rounded-full animate-bounce" style="animation-delay: 0.2s"></div>
                      <div class="w-2 h-2 bg-gray-400 rounded-full animate-bounce" style="animation-delay: 0.4s"></div>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <!-- Input Area -->
          <div class="sticky bottom-0 p-4 border-t border-gray-200 bg-white/80 dark:bg-gray-900/80 backdrop-blur-sm dark:border-gray-700">
            <div class="max-w-4xl mx-auto">
              <!-- Image Preview -->
              <div v-if="imagePreview" class="relative inline-block mb-3">
                <img :src="imagePreview" class="h-20 rounded-lg" alt="Preview" />
                <button
                  @click="removeImage"
                  class="absolute top-0 right-0 p-1 text-white bg-red-500 rounded-full -translate-y-1/2 translate-x-1/2 hover:bg-red-600"
                >
                  <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
                  </svg>
                </button>
              </div>

              <div class="flex items-end space-x-2">
                <!-- Image Upload -->
                <input
                  ref="fileInput"
                  type="file"
                  accept="image/*"
                  @change="handleImageUpload"
                  class="hidden"
                />
                <button
                  @click="() => fileInput?.click()"
                  class="p-3 text-gray-500 transition-colors rounded-lg hover:bg-gray-100 dark:hover:bg-gray-800"
                  title="Attach Image"
                >
                  <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 16l4.586-4.586a2 2 0 012.828 0L16 16m-2-2l1.586-1.586a2 2 0 012.828 0L20 14m-6-6h.01M6 20h12a2 2 0 002-2V6a2 2 0 00-2-2H6a2 2 0 00-2 2v12a2 2 0 002 2z" />
                  </svg>
                </button>

                <!-- Voice Input -->
                <button
                  @click="toggleVoiceInput"
                  :class="isRecording ? 'text-red-500' : 'text-gray-500'"
                  class="p-3 transition-colors rounded-lg hover:bg-gray-100 dark:hover:bg-gray-800"
                  title="Voice Input"
                >
                  <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 11a7 7 0 01-7 7m0 0a7 7 0 01-7-7m7 7v4m0 0H8m4 0h4m-4-8a3 3 0 01-3-3V5a3 3 0 116 0v6a3 3 0 01-3 3z" />
                  </svg>
                </button>

                <!-- Text Input -->
                <div class="flex-1">
                  <textarea
                    v-model="userInput"
                    @keydown.enter.exact.prevent="sendMessage"
                    @keydown.shift.enter.exact="userInput += '\n'"
                    rows="1"
                    placeholder="Ask anything about medical diagnosis, treatment, or research..."
                    class="w-full px-4 py-3 text-sm border border-gray-300 rounded-lg resize-none dark:border-gray-600 dark:bg-gray-800 dark:text-white focus:outline-none focus:ring-2 focus:ring-purple-500 focus:border-transparent"
                    :disabled="isTyping"
                  ></textarea>
                </div>

                <!-- Send Button -->
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
        </div>

        <!-- Context Panel -->
        <div
          v-if="showContext"
          class="w-80 border-l border-gray-200 dark:border-gray-700 overflow-y-auto p-4 space-y-4 bg-gray-50 dark:bg-gray-800/50"
        >
          <div>
            <h3 class="text-sm font-semibold text-gray-900 dark:text-white">Context Information</h3>
            <p class="mt-1 text-xs text-gray-500 dark:text-gray-400">Additional details for better AI responses</p>
          </div>

          <!-- Patient Context -->
          <div class="p-3 bg-white border border-gray-200 rounded-lg dark:bg-gray-800 dark:border-gray-700">
            <h4 class="text-xs font-semibold text-gray-700 dark:text-gray-300">Patient Context</h4>
            <div class="mt-2 space-y-2">
              <label class="block">
                <span class="text-xs text-gray-600 dark:text-gray-400">Patient ID</span>
                <input
                  v-model="context.patientId"
                  type="text"
                  placeholder="e.g., P-12345"
                  class="w-full px-2 py-1 mt-1 text-xs border border-gray-300 rounded dark:border-gray-600 dark:bg-gray-700 dark:text-white"
                />
              </label>
              <label class="block">
                <span class="text-xs text-gray-600 dark:text-gray-400">Age</span>
                <input
                  v-model="context.age"
                  type="number"
                  placeholder="e.g., 45"
                  class="w-full px-2 py-1 mt-1 text-xs border border-gray-300 rounded dark:border-gray-600 dark:bg-gray-700 dark:text-white"
                />
              </label>
              <label class="block">
                <span class="text-xs text-gray-600 dark:text-gray-400">Gender</span>
                <select
                  v-model="context.gender"
                  class="w-full px-2 py-1 mt-1 text-xs border border-gray-300 rounded dark:border-gray-600 dark:bg-gray-700 dark:text-white"
                >
                  <option value="">Select...</option>
                  <option value="male">Male</option>
                  <option value="female">Female</option>
                  <option value="other">Other</option>
                </select>
              </label>
            </div>
          </div>

          <!-- Medical History -->
          <div class="p-3 bg-white border border-gray-200 rounded-lg dark:bg-gray-800 dark:border-gray-700">
            <h4 class="text-xs font-semibold text-gray-700 dark:text-gray-300">Medical History</h4>
            <textarea
              v-model="context.medicalHistory"
              rows="3"
              placeholder="Enter relevant medical history..."
              class="w-full px-2 py-1 mt-2 text-xs border border-gray-300 rounded dark:border-gray-600 dark:bg-gray-700 dark:text-white"
            ></textarea>
          </div>

          <!-- Current Medications -->
          <div class="p-3 bg-white border border-gray-200 rounded-lg dark:bg-gray-800 dark:border-gray-700">
            <h4 class="text-xs font-semibold text-gray-700 dark:text-gray-300">Current Medications</h4>
            <textarea
              v-model="context.medications"
              rows="3"
              placeholder="List current medications..."
              class="w-full px-2 py-1 mt-2 text-xs border border-gray-300 rounded dark:border-gray-600 dark:bg-gray-700 dark:text-white"
            ></textarea>
          </div>

          <!-- Allergies -->
          <div class="p-3 bg-white border border-gray-200 rounded-lg dark:bg-gray-800 dark:border-gray-700">
            <h4 class="text-xs font-semibold text-gray-700 dark:text-gray-300">Allergies</h4>
            <textarea
              v-model="context.allergies"
              rows="2"
              placeholder="List known allergies..."
              class="w-full px-2 py-1 mt-2 text-xs border border-gray-300 rounded dark:border-gray-600 dark:bg-gray-700 dark:text-white"
            ></textarea>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, nextTick, onMounted, h } from 'vue'
import { useAuthStore } from '../stores/auth'
import { getEndpointUrl } from '../config/api'

const authStore = useAuthStore()

interface Message {
  role: 'user' | 'assistant'
  content: string
  timestamp: Date
  image?: string
}

interface Conversation {
  id: string
  title: string
  messages: Message[]
  lastMessage: string
  updatedAt: Date
}

// State
const conversations = ref<Conversation[]>([])
const currentConversationId = ref<string>('')
const userInput = ref('')
const isTyping = ref(false)
const isRecording = ref(false)
const messagesContainer = ref<HTMLElement | null>(null)
const showConversations = ref(true)
const showContext = ref(false)
const imagePreview = ref('')
const fileInput = ref<HTMLInputElement | null>(null)

// Context data
const context = ref({
  patientId: '',
  age: '',
  gender: '',
  medicalHistory: '',
  medications: '',
  allergies: ''
})

// Suggested prompts
const suggestedPrompts = [
  {
    icon: h('svg', { fill: 'none', stroke: 'currentColor', viewBox: '0 0 24 24' }, [
      h('path', { 'stroke-linecap': 'round', 'stroke-linejoin': 'round', 'stroke-width': '2', d: 'M9 5H7a2 2 0 00-2 2v12a2 2 0 002 2h10a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2' })
    ]),
    title: 'Differential Diagnosis',
    text: 'Help me create a differential diagnosis for a patient with chest pain'
  },
  {
    icon: h('svg', { fill: 'none', stroke: 'currentColor', viewBox: '0 0 24 24' }, [
      h('path', { 'stroke-linecap': 'round', 'stroke-linejoin': 'round', 'stroke-width': '2', d: 'M19.428 15.428a2 2 0 00-1.022-.547l-2.387-.477a6 6 0 00-3.86.517l-.318.158a6 6 0 01-3.86.517L6.05 15.21a2 2 0 00-1.806.547M8 4h8l-1 1v5.172a2 2 0 00.586 1.414l5 5c1.26 1.26.367 3.414-1.415 3.414H4.828c-1.782 0-2.674-2.154-1.414-3.414l5-5A2 2 0 009 10.172V5L8 4z' })
    ]),
    title: 'Lab Interpretation',
    text: 'Analyze these lab results and suggest possible conditions'
  },
  {
    icon: h('svg', { fill: 'none', stroke: 'currentColor', viewBox: '0 0 24 24' }, [
      h('path', { 'stroke-linecap': 'round', 'stroke-linejoin': 'round', 'stroke-width': '2', d: 'M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z' })
    ]),
    title: 'Treatment Plan',
    text: 'Create a treatment plan for type 2 diabetes management'
  },
  {
    icon: h('svg', { fill: 'none', stroke: 'currentColor', viewBox: '0 0 24 24' }, [
      h('path', { 'stroke-linecap': 'round', 'stroke-linejoin': 'round', 'stroke-width': '2', d: 'M12 6.253v13m0-13C10.832 5.477 9.246 5 7.5 5S4.168 5.477 3 6.253v13C4.168 18.477 5.754 18 7.5 18s3.332.477 4.5 1.253m0-13C13.168 5.477 14.754 5 16.5 5c1.747 0 3.332.477 4.5 1.253v13C19.832 18.477 18.247 18 16.5 18c-1.746 0-3.332.477-4.5 1.253' })
    ]),
    title: 'Research Summary',
    text: 'Summarize recent research on immunotherapy for melanoma'
  }
]

// Computed
const currentMessages = computed(() => {
  const conv = conversations.value.find(c => c.id === currentConversationId.value)
  return conv?.messages || []
})

const userInitials = computed(() => {
  if (authStore.user?.firstName && authStore.user?.lastName) {
    return `${authStore.user.firstName[0]}${authStore.user.lastName[0]}`.toUpperCase()
  }
  return authStore.user?.email?.[0].toUpperCase() || 'U'
})

// Methods
const createNewConversation = () => {
  const newConv: Conversation = {
    id: Date.now().toString(),
    title: 'New Conversation',
    messages: [],
    lastMessage: '',
    updatedAt: new Date()
  }
  conversations.value.unshift(newConv)
  currentConversationId.value = newConv.id
}

const selectConversation = (id: string) => {
  currentConversationId.value = id
  nextTick(() => {
    scrollToBottom()
  })
}

const deleteConversation = (id: string) => {
  conversations.value = conversations.value.filter(c => c.id !== id)
  if (currentConversationId.value === id && conversations.value.length > 0) {
    currentConversationId.value = conversations.value[0].id
  } else if (conversations.value.length === 0) {
    currentConversationId.value = ''
  }
}

const sendMessage = async () => {
  if (!userInput.value.trim() || isTyping.value) return

  // Create conversation if none exists
  if (!currentConversationId.value) {
    createNewConversation()
  }

  const currentConv = conversations.value.find(c => c.id === currentConversationId.value)
  if (!currentConv) return

  // Add user message
  const userMessage: Message = {
    role: 'user',
    content: userInput.value.trim(),
    timestamp: new Date(),
    image: imagePreview.value
  }

  currentConv.messages.push(userMessage)
  currentConv.lastMessage = userInput.value.trim()
  currentConv.updatedAt = new Date()

  // Update title if first message
  if (currentConv.messages.length === 1) {
    currentConv.title = userInput.value.trim().substring(0, 50) + (userInput.value.length > 50 ? '...' : '')
  }

  const messageText = userInput.value
  userInput.value = ''
  imagePreview.value = ''
  
  nextTick(() => {
    scrollToBottom()
  })

  // Call actual AI API
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
        context: {
          patientId: context.value.patientId || null,
          age: context.value.age || null,
          gender: context.value.gender || null,
          medicalHistory: context.value.medicalHistory || null,
          medications: context.value.medications || null,
          allergies: context.value.allergies || null
        }
      })
    })

    if (!response.ok) {
      throw new Error('Failed to get AI response')
    }

    const data = await response.json()
    
    const aiMessage: Message = {
      role: 'assistant',
      content: data.message || 'I apologize, but I encountered an issue processing your request.',
      timestamp: new Date()
    }
    
    currentConv.messages.push(aiMessage)
    currentConv.lastMessage = aiMessage.content.substring(0, 50) + '...'
    currentConv.updatedAt = new Date()
    isTyping.value = false
    
    nextTick(() => {
      scrollToBottom()
    })
  } catch (error) {
    console.error('Error calling AI API:', error)
    
    // Fallback to mock response if API fails
    const aiMessage: Message = {
      role: 'assistant',
      content: generateMockResponse(messageText),
      timestamp: new Date()
    }
    currentConv.messages.push(aiMessage)
    currentConv.lastMessage = aiMessage.content.substring(0, 50) + '...'
    currentConv.updatedAt = new Date()
    isTyping.value = false
    
    nextTick(() => {
      scrollToBottom()
    })
  }
}

const generateMockResponse = (input: string): string => {
  // Mock AI responses for demonstration
  const lowerInput = input.toLowerCase()
  
  if (lowerInput.includes('diagnosis') || lowerInput.includes('chest pain')) {
    return `Based on the patient's symptoms of chest pain, here are the key differential diagnoses to consider:

**Primary Considerations:**
1. **Acute Coronary Syndrome (ACS)**
   - ST-elevation MI (STEMI)
   - Non-ST-elevation MI (NSTEMI)
   - Unstable angina

2. **Pulmonary Embolism (PE)**
   - Risk factors: recent surgery, immobilization, DVT history

3. **Aortic Dissection**
   - Sharp, tearing pain radiating to back
   - Blood pressure differential between arms

**Recommended Workup:**
- ECG (immediate)
- Troponin levels (serial)
- Chest X-ray
- D-dimer if PE suspected
- Consider CT angiography

Would you like me to elaborate on any specific diagnosis or provide treatment protocols?`
  }
  
  if (lowerInput.includes('treatment') || lowerInput.includes('diabetes')) {
    return `Here's a comprehensive treatment plan for Type 2 Diabetes management:

**Lifestyle Modifications:**
- Diet: Mediterranean or DASH diet, carb counting
- Exercise: 150 min/week moderate aerobic activity
- Weight loss: 5-10% reduction if overweight

**Pharmacotherapy:**
1. **First-line:** Metformin 500mg BID, titrate to 1000mg BID
2. **Add-on therapy** (if HbA1c > 7% after 3 months):
   - GLP-1 agonist (if cardiovascular disease)
   - SGLT2 inhibitor (if heart failure/CKD)
   - DPP-4 inhibitor (alternative)

**Monitoring:**
- HbA1c every 3 months
- Annual comprehensive foot exam
- Annual eye exam
- Lipid panel quarterly
- Microalbuminuria screening

**Target Goals:**
- HbA1c < 7% (individualize based on patient)
- BP < 130/80 mmHg
- LDL < 100 mg/dL

Would you like specific medication dosing or patient education materials?`
  }
  
  return `Thank you for your question. I'm here to help with medical inquiries. Based on the context you've provided${context.value.patientId ? ` for patient ${context.value.patientId}` : ''}, I can assist with:

- Differential diagnoses
- Treatment recommendations
- Lab interpretation
- Medication interactions
- Clinical guidelines

Please provide more specific details about your medical query, and I'll provide detailed, evidence-based information.`
}

const useSuggestedPrompt = (text: string) => {
  userInput.value = text
  sendMessage()
}

const formatMessage = (content: string): string => {
  // Basic markdown-like formatting
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

const formatDate = (date: Date): string => {
  const now = new Date()
  const diff = now.getTime() - date.getTime()
  const days = Math.floor(diff / 86400000)
  
  if (days === 0) return 'Today'
  if (days === 1) return 'Yesterday'
  if (days < 7) return `${days} days ago`
  return date.toLocaleDateString()
}

const scrollToBottom = () => {
  if (messagesContainer.value) {
    messagesContainer.value.scrollTop = messagesContainer.value.scrollHeight
  }
}

const handleImageUpload = (event: Event) => {
  const target = event.target as HTMLInputElement
  const file = target.files?.[0]
  if (file) {
    const reader = new FileReader()
    reader.onload = (e) => {
      imagePreview.value = e.target?.result as string
    }
    reader.readAsDataURL(file)
  }
}

const removeImage = () => {
  imagePreview.value = ''
  if (fileInput.value) {
    fileInput.value.value = ''
  }
}

const toggleVoiceInput = () => {
  isRecording.value = !isRecording.value
  // Voice input would be implemented here
  if (isRecording.value) {
    // Start recording
    console.log('Voice recording started...')
  } else {
    // Stop recording
    console.log('Voice recording stopped...')
  }
}

const copyMessage = (content: string) => {
  navigator.clipboard.writeText(content.replace(/<[^>]*>/g, ''))
  // You could add a toast notification here
}

const saveToPatientFile = (content: string) => {
  // Implement save to patient file logic
  console.log('Saving to patient file:', content)
  // You could add a modal to select patient and confirmation
}

const regenerateResponse = async (index: number) => {
  const currentConv = conversations.value.find(c => c.id === currentConversationId.value)
  if (!currentConv) return
  
  // Remove the AI response
  if (index < currentConv.messages.length && currentConv.messages[index].role === 'assistant') {
    currentConv.messages.splice(index, 1)
    
    // Get the previous user message
    const previousMessage = currentConv.messages[index - 1]?.content || ''
    
    // Call actual AI API
    isTyping.value = true
    try {
      const apiUrl = getEndpointUrl('aiChat')
      const response = await fetch(apiUrl, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({
          message: previousMessage,
          context: {
            patientId: context.value.patientId || null,
            age: context.value.age || null,
            gender: context.value.gender || null,
            medicalHistory: context.value.medicalHistory || null,
            medications: context.value.medications || null,
            allergies: context.value.allergies || null
          }
        })
      })

      if (!response.ok) {
        throw new Error('Failed to regenerate response')
      }

      const data = await response.json()
      
      const aiMessage: Message = {
        role: 'assistant',
        content: data.message + '\n\n*[Regenerated response]*',
        timestamp: new Date()
      }
      
      currentConv.messages.splice(index, 0, aiMessage)
      isTyping.value = false
      
      nextTick(() => {
        scrollToBottom()
      })
    } catch (error) {
      console.error('Error regenerating response:', error)
      
      // Fallback to mock response
      const aiMessage: Message = {
        role: 'assistant',
        content: generateMockResponse(previousMessage) + '\n\n*[Regenerated response]*',
        timestamp: new Date()
      }
      currentConv.messages.splice(index, 0, aiMessage)
      isTyping.value = false
      
      nextTick(() => {
        scrollToBottom()
      })
    }
  }
}

const exportConversation = () => {
  const currentConv = conversations.value.find(c => c.id === currentConversationId.value)
  if (!currentConv) return
  
  let exportText = `Aesclea Medical AI Assistant - Conversation Export\n`
  exportText += `Title: ${currentConv.title}\n`
  exportText += `Date: ${currentConv.updatedAt.toLocaleString()}\n`
  exportText += `\n${'='.repeat(60)}\n\n`
  
  currentConv.messages.forEach(msg => {
    exportText += `[${msg.role.toUpperCase()}] - ${msg.timestamp.toLocaleTimeString()}\n`
    exportText += `${msg.content.replace(/<[^>]*>/g, '')}\n\n`
  })
  
  const blob = new Blob([exportText], { type: 'text/plain' })
  const url = URL.createObjectURL(blob)
  const a = document.createElement('a')
  a.href = url
  a.download = `conversation-${currentConv.id}.txt`
  a.click()
  URL.revokeObjectURL(url)
}

const clearConversation = () => {
  const currentConv = conversations.value.find(c => c.id === currentConversationId.value)
  if (currentConv && confirm('Are you sure you want to clear this conversation?')) {
    currentConv.messages = []
    currentConv.title = 'New Conversation'
    currentConv.lastMessage = ''
  }
}

// Initialize with a default conversation
onMounted(() => {
  createNewConversation()
})
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

/* Custom scrollbar */
::-webkit-scrollbar {
  width: 8px;
}

::-webkit-scrollbar-track {
  background: transparent;
}

::-webkit-scrollbar-thumb {
  background: rgba(156, 163, 175, 0.5);
  border-radius: 4px;
}

::-webkit-scrollbar-thumb:hover {
  background: rgba(156, 163, 175, 0.7);
}

/* Dark mode scrollbar */
.dark ::-webkit-scrollbar-thumb {
  background: rgba(75, 85, 99, 0.5);
}

.dark ::-webkit-scrollbar-thumb:hover {
  background: rgba(75, 85, 99, 0.7);
}
</style>
