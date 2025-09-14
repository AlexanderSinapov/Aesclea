<!-- Copyright (c) 2025 Alexander Sinapov | Simeon Petkov -->

<!-- All rights reserved. -->
<!-- This code is proprietary and confidential.   -->
<!-- Unauthorized copying, modification, distribution, or use is strictly prohibited. -->

<template>
  <div class="space-y-6">
    <!-- Page Header -->
    <div class="sm:flex sm:items-center sm:justify-between">
      <div>
        <h1 class="text-2xl font-bold text-gray-900 dark:text-white">AI Analysis</h1>
        <p class="mt-1 text-sm text-gray-500 dark:text-gray-400">
          Manage and review AI-powered medical analyses
          <span v-if="selectedDepartment" class="capitalize">
            - {{ selectedDepartment }} Department
          </span>
        </p>
      </div>
      <div class="flex mt-4 space-x-3 sm:mt-0">
        <button
          @click="showNewAnalysisModal = true"
          class="inline-flex items-center px-4 py-2 text-sm font-medium text-white bg-purple-600 border border-transparent rounded-md shadow-sm hover:bg-purple-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-purple-500"
        >
          <PlusIcon class="w-4 h-4 mr-2" />
          New Analysis
        </button>
      </div>
    </div>

    <!-- Analysis Types Quick Access -->
    <div class="grid grid-cols-1 gap-4 md:grid-cols-2 lg:grid-cols-4">
      <div 
        v-for="analysisType in analysisTypes" 
        :key="analysisType.id"
        @click="showAnalysisModal(analysisType)"
        class="p-6 transition-shadow bg-white border border-gray-200 rounded-lg cursor-pointer dark:bg-gray-800 dark:border-gray-700 hover:shadow-md"
      >
        <div class="flex items-center">
          <div class="flex-shrink-0">
            <component :is="analysisType.icon" :class="analysisType.iconColor" class="w-8 h-8" />
          </div>
          <div class="ml-4">
            <h3 class="text-lg font-medium text-gray-900 dark:text-white">{{ analysisType.name }}</h3>
            <p class="text-sm text-gray-500 dark:text-gray-400">{{ analysisType.description }}</p>
          </div>
        </div>
        <div class="mt-4">
          <div class="flex items-center justify-between text-sm">
            <span class="text-gray-500 dark:text-gray-400">Available</span>
            <span 
              class="inline-flex items-center px-2 py-1 text-xs font-medium rounded-full"
              :class="analysisType.available ? 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-200' : 'bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-200'"
            >
              {{ analysisType.available ? 'Active' : 'Requires Subscription' }}
            </span>
          </div>
        </div>
      </div>
    </div>

    <!-- Filters and Stats -->
    <div class="bg-white border border-gray-200 rounded-lg shadow-sm dark:bg-gray-800 dark:border-gray-700">
      <div class="p-6">
        <div class="grid grid-cols-1 gap-4 mb-6 md:grid-cols-4">
          <!-- Status Filter -->
          <div>
            <select
              v-model="statusFilter"
              class="block w-full px-3 py-2 text-gray-900 bg-white border border-gray-300 rounded-md dark:border-gray-600 dark:bg-gray-700 dark:text-white focus:outline-none focus:ring-1 focus:ring-purple-500 focus:border-purple-500 sm:text-sm"
            >
              <option value="">All Statuses</option>
              <option value="pending">Pending</option>
              <option value="processing">Processing</option>
              <option value="completed">Completed</option>
              <option value="failed">Failed</option>
            </select>
          </div>

          <!-- Type Filter -->
          <div>
            <select
              v-model="typeFilter"
              class="block w-full px-3 py-2 text-gray-900 bg-white border border-gray-300 rounded-md dark:border-gray-600 dark:bg-gray-700 dark:text-white focus:outline-none focus:ring-1 focus:ring-purple-500 focus:border-purple-500 sm:text-sm"
            >
              <option value="">All Types</option>
              <option value="tumor">Tumor Analysis</option>
              <option value="vital-signs">Vital Signs</option>
              <option value="diagnosis">Diagnosis</option>
              <option value="enhanced-text">Enhanced Text</option>
            </select>
          </div>

          <!-- Department Filter -->
          <div>
            <select
              v-model="departmentFilter"
              class="block w-full px-3 py-2 text-gray-900 bg-white border border-gray-300 rounded-md dark:border-gray-600 dark:bg-gray-700 dark:text-white focus:outline-none focus:ring-1 focus:ring-purple-500 focus:border-purple-500 sm:text-sm"
            >
              <option value="">All Departments</option>
              <option value="cardiology">Cardiology</option>
              <option value="neurology">Neurology</option>
              <option value="oncology">Oncology</option>
              <option value="radiology">Radiology</option>
              <option value="emergency">Emergency</option>
              <option value="pediatrics">Pediatrics</option>
            </select>
          </div>

          <!-- Date Range -->
          <div>
            <input
              v-model="dateFilter"
              type="date"
              class="block w-full px-3 py-2 text-gray-900 bg-white border border-gray-300 rounded-md dark:border-gray-600 dark:bg-gray-700 dark:text-white focus:outline-none focus:ring-1 focus:ring-purple-500 focus:border-purple-500 sm:text-sm"
            />
          </div>
        </div>

        <!-- Stats Cards -->
        <div class="grid grid-cols-1 gap-4 md:grid-cols-4">
          <div class="text-center">
            <div class="text-2xl font-bold text-purple-600 dark:text-purple-400">{{ analysisStore.analyses.length }}</div>
            <div class="text-sm text-gray-500 dark:text-gray-400">Total Analyses</div>
          </div>
          <div class="text-center">
            <div class="text-2xl font-bold text-yellow-600 dark:text-yellow-400">{{ analysisStore.pendingAnalyses.length }}</div>
            <div class="text-sm text-gray-500 dark:text-gray-400">Pending</div>
          </div>
          <div class="text-center">
            <div class="text-2xl font-bold text-green-600 dark:text-green-400">{{ analysisStore.completedAnalyses.length }}</div>
            <div class="text-sm text-gray-500 dark:text-gray-400">Completed</div>
          </div>
          <div class="text-center">
            <div class="text-2xl font-bold text-blue-600 dark:text-blue-400">{{ Math.round(averageConfidence) }}%</div>
            <div class="text-sm text-gray-500 dark:text-gray-400">Avg. Confidence</div>
          </div>
        </div>
      </div>
    </div>

    <!-- Analysis Results -->
    <div class="bg-white border border-gray-200 rounded-lg shadow-sm dark:bg-gray-800 dark:border-gray-700">
      <div class="px-6 py-4 border-b border-gray-200 dark:border-gray-700">
        <h3 class="text-lg font-medium text-gray-900 dark:text-white">
          Analysis Results ({{ filteredAnalyses.length }})
        </h3>
      </div>
      
      <div class="overflow-x-auto">
        <table class="min-w-full divide-y divide-gray-200 dark:divide-gray-700">
          <thead class="bg-gray-50 dark:bg-gray-900">
            <tr>
              <th class="px-6 py-3 text-xs font-medium tracking-wider text-left text-gray-500 uppercase dark:text-gray-400">
                Patient & Analysis
              </th>
              <th class="px-6 py-3 text-xs font-medium tracking-wider text-left text-gray-500 uppercase dark:text-gray-400">
                Type & Department
              </th>
              <th class="px-6 py-3 text-xs font-medium tracking-wider text-left text-gray-500 uppercase dark:text-gray-400">
                Status
              </th>
              <th class="px-6 py-3 text-xs font-medium tracking-wider text-left text-gray-500 uppercase dark:text-gray-400">
                Confidence
              </th>
              <th class="px-6 py-3 text-xs font-medium tracking-wider text-left text-gray-500 uppercase dark:text-gray-400">
                Date
              </th>
              <th class="relative px-6 py-3">
                <span class="sr-only">Actions</span>
              </th>
            </tr>
          </thead>
          <tbody class="bg-white divide-y divide-gray-200 dark:bg-gray-800 dark:divide-gray-700">
            <tr v-for="analysis in paginatedAnalyses" :key="analysis.id" class="hover:bg-gray-50 dark:hover:bg-gray-700">
              <td class="px-6 py-4 whitespace-nowrap">
                <div class="flex items-center">
                  <div class="flex-shrink-0 w-10 h-10">
                    <div class="flex items-center justify-center w-10 h-10 bg-purple-100 rounded-full dark:bg-purple-900">
                      <component :is="getAnalysisIcon(analysis.analysisType)" class="w-5 h-5 text-purple-600 dark:text-purple-300" />
                    </div>
                  </div>
                  <div class="ml-4">
                    <div class="text-sm font-medium text-gray-900 dark:text-white">
                      {{ analysis.patientName }}
                    </div>
                    <div class="text-sm text-gray-500 dark:text-gray-400">
                      ID: {{ analysis.id.substring(0, 8) }}...
                    </div>
                  </div>
                </div>
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <div class="text-sm text-gray-900 capitalize dark:text-white">{{ analysis.analysisType.replace('-', ' ') }}</div>
                <div class="text-sm text-gray-500 capitalize dark:text-gray-400">{{ analysis.department }}</div>
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <span 
                  class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium"
                  :class="getStatusBadgeClass(analysis.status)"
                >
                  {{ analysis.status }}
                </span>
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <div v-if="analysis.confidence" class="flex items-center">
                  <div class="text-sm text-gray-900 dark:text-white">{{ Math.round(analysis.confidence * 100) }}%</div>
                  <div class="w-16 h-2 ml-2 bg-gray-200 rounded-full dark:bg-gray-700">
                    <div 
                      class="h-2 rounded-full" 
                      :class="getConfidenceColor(analysis.confidence)"
                      :style="{ width: `${analysis.confidence * 100}%` }"
                    ></div>
                  </div>
                </div>
                <div v-else class="text-sm text-gray-500 dark:text-gray-400">N/A</div>
              </td>
              <td class="px-6 py-4 text-sm text-gray-500 whitespace-nowrap dark:text-gray-400">
                {{ formatDateTime(analysis.createdAt) }}
              </td>
              <td class="px-6 py-4 text-sm font-medium text-right whitespace-nowrap">
                <div class="flex items-center space-x-2">
                  <button
                    @click="viewAnalysis(analysis)"
                    class="text-purple-600 hover:text-purple-900 dark:text-purple-400 dark:hover:text-purple-300"
                  >
                    View
                  </button>
                  <button
                    v-if="analysis.status === 'completed'"
                    @click="downloadReport(analysis)"
                    class="text-blue-600 hover:text-blue-900 dark:text-blue-400 dark:hover:text-blue-300"
                  >
                    Download
                  </button>
                  <button
                    @click="deleteAnalysis(analysis)"
                    class="text-red-600 hover:text-red-900 dark:text-red-400 dark:hover:text-red-300"
                  >
                    Delete
                  </button>
                </div>
              </td>
            </tr>
          </tbody>
        </table>
        
        <!-- Empty State -->
        <div v-if="filteredAnalyses.length === 0" class="py-12 text-center">
          <ChartBarIcon class="w-12 h-12 mx-auto text-gray-400" />
          <h3 class="mt-2 text-sm font-medium text-gray-900 dark:text-white">No analyses found</h3>
          <p class="mt-1 text-sm text-gray-500 dark:text-gray-400">
            Get started by running your first AI analysis.
          </p>
          <div class="mt-6">
            <button
              @click="showNewAnalysisModal = true"
              class="inline-flex items-center px-4 py-2 text-sm font-medium text-white bg-purple-600 border border-transparent rounded-md shadow-sm hover:bg-purple-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-purple-500"
            >
              <PlusIcon class="w-4 h-4 mr-2" />
              Run Analysis
            </button>
          </div>
        </div>
      </div>

      <!-- Pagination -->
      <div v-if="filteredAnalyses.length > analysesPerPage" class="px-6 py-4 border-t border-gray-200 dark:border-gray-700">
        <div class="flex items-center justify-between">
          <div class="text-sm text-gray-700 dark:text-gray-300">
            Showing {{ (currentPage - 1) * analysesPerPage + 1 }} to {{ Math.min(currentPage * analysesPerPage, filteredAnalyses.length) }} of {{ filteredAnalyses.length }} results
          </div>
          <div class="flex space-x-2">
            <button
              @click="currentPage--"
              :disabled="currentPage === 1"
              class="px-3 py-1 text-sm font-medium text-gray-500 bg-white border border-gray-300 rounded-md hover:bg-gray-50 disabled:opacity-50 disabled:cursor-not-allowed dark:bg-gray-800 dark:border-gray-600 dark:text-gray-400 dark:hover:bg-gray-700"
            >
              Previous
            </button>
            <button
              @click="currentPage++"
              :disabled="currentPage >= totalPages"
              class="px-3 py-1 text-sm font-medium text-gray-500 bg-white border border-gray-300 rounded-md hover:bg-gray-50 disabled:opacity-50 disabled:cursor-not-allowed dark:bg-gray-800 dark:border-gray-600 dark:text-gray-400 dark:hover:bg-gray-700"
            >
              Next
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Analysis Modal -->
    <AnalysisModal
      v-if="showNewAnalysisModal || showAnalysisDetailsModal"
      :analysis="selectedAnalysis"
      :analysis-type="selectedAnalysisType"
      :is-view-mode="showAnalysisDetailsModal"
      @close="closeModals"
      @save="handleRunAnalysis"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import { useAnalysisStore, type AnalysisResult } from '../../stores/analysis'
import { useSubscriptionStore } from '../../stores/subscription'
import AnalysisModal from './AnalysisModal.vue'

// Define props
interface Props {
  selectedDepartment?: string | null
}

const props = defineProps<Props>()

// Stores
const analysisStore = useAnalysisStore()
const subscriptionStore = useSubscriptionStore()

// Local state
const statusFilter = ref('')
const typeFilter = ref('')
const departmentFilter = ref('')
const dateFilter = ref('')
const currentPage = ref(1)
const analysesPerPage = 20

const showNewAnalysisModal = ref(false)
const showAnalysisDetailsModal = ref(false)
const selectedAnalysis = ref<AnalysisResult | null>(null)
const selectedAnalysisType = ref<any>(null)

// Watch for department changes from parent
watch(() => props.selectedDepartment, (newDepartment) => {
  if (newDepartment) {
    departmentFilter.value = newDepartment
  }
}, { immediate: true })

// Analysis types configuration
const analysisTypes = ref([
  {
    id: 'tumor',
    name: 'Tumor Analysis',
    description: 'AI-powered tumor detection and classification',
    icon: 'BeakerIcon',
    iconColor: 'text-purple-600',
    available: subscriptionStore.hasActiveSubscription
  },
  {
    id: 'vital-signs',
    name: 'Vital Signs',
    description: 'Analyze patient vital signs patterns',
    icon: 'HeartIcon',
    iconColor: 'text-red-600',
    available: subscriptionStore.hasActiveSubscription
  },
  {
    id: 'diagnosis',
    name: 'Medical Diagnosis',
    description: 'AI-assisted medical diagnosis',
    icon: 'PuzzlePieceIcon',
    iconColor: 'text-blue-600',
    available: subscriptionStore.hasActiveSubscription
  },
  {
    id: 'enhanced-text',
    name: 'Enhanced Text Analysis',
    description: 'Advanced text analysis for medical reports',
    icon: 'DocumentTextIcon',
    iconColor: 'text-green-600',
    available: subscriptionStore.hasActiveSubscription
  }
])

// Computed properties
const filteredAnalyses = computed(() => {
  let analyses = analysisStore.analyses

  // Apply status filter
  if (statusFilter.value) {
    analyses = analyses.filter(analysis => analysis.status === statusFilter.value)
  }

  // Apply type filter
  if (typeFilter.value) {
    analyses = analyses.filter(analysis => analysis.analysisType === typeFilter.value)
  }

  // Apply department filter
  if (departmentFilter.value) {
    analyses = analyses.filter(analysis => analysis.department === departmentFilter.value)
  }

  // Apply date filter
  if (dateFilter.value) {
    const filterDate = new Date(dateFilter.value).toDateString()
    analyses = analyses.filter(analysis => 
      new Date(analysis.createdAt).toDateString() === filterDate
    )
  }

  return analyses.sort((a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime())
})

const totalPages = computed(() => Math.ceil(filteredAnalyses.value.length / analysesPerPage))

const paginatedAnalyses = computed(() => {
  const start = (currentPage.value - 1) * analysesPerPage
  const end = start + analysesPerPage
  return filteredAnalyses.value.slice(start, end)
})

const averageConfidence = computed(() => {
  const completedAnalyses = analysisStore.completedAnalyses.filter(a => a.confidence)
  if (completedAnalyses.length === 0) return 0
  
  const sum = completedAnalyses.reduce((acc, analysis) => acc + (analysis.confidence || 0), 0)
  return (sum / completedAnalyses.length) * 100
})

// Icon components
const PlusIcon = {
  template: `<svg fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" d="M12 4.5v15m7.5-7.5h-15" /></svg>`
}

const ChartBarIcon = {
  template: `<svg fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" d="M3 13.125C3 12.504 3.504 12 4.125 12h2.25c.621 0 1.125.504 1.125 1.125v6.75C7.5 20.496 6.996 21 6.375 21h-2.25A1.125 1.125 0 013 19.875v-6.75zM9.75 8.625c0-.621.504-1.125 1.125-1.125h2.25c.621 0 1.125.504 1.125 1.125v11.25c0 .621-.504 1.125-1.125 1.125h-2.25a1.125 1.125 0 01-1.125-1.125V8.625zM16.5 4.125c0-.621.504-1.125 1.125-1.125h2.25C20.496 3 21 3.504 21 4.125v15.75c0 .621-.504 1.125-1.125 1.125h-2.25a1.125 1.125 0 01-1.125-1.125V4.125z" /></svg>`
}

const BeakerIcon = {
  template: `<svg fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" d="M9.75 3.104v5.714a2.25 2.25 0 01-.659 1.591L5 14.5a2.25 2.25 0 00-.659 1.591v.035c0 .623.505 1.125 1.125 1.125h12.999a1.125 1.125 0 001.125-1.125v-.035c0-.592-.237-1.16-.659-1.591L14.25 10.409a2.25 2.25 0 01-.659-1.591V3.104z" /></svg>`
}

const HeartIcon = {
  template: `<svg fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" d="M21 8.25c0-2.485-2.099-4.5-4.688-4.5-1.935 0-3.597 1.126-4.312 2.733-.715-1.607-2.377-2.733-4.313-2.733C5.1 3.75 3 5.765 3 8.25c0 7.22 9 12 9 12s9-4.78 9-12z" /></svg>`
}

const PuzzlePieceIcon = {
  template: `<svg fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" d="M14.25 6.087c0-.355.186-.676.401-.959.221-.29.349-.634.349-1.003 0-1.036-1.007-1.875-2.25-1.875s-2.25.84-2.25 1.875c0 .369.128.713.349 1.003.215.283.401.604.401.959v0a.64.64 0 01-.657.643 48.39 48.39 0 01-4.163-.3c.186 1.613.293 3.25.315 4.907a.656.656 0 01-.658.663v0c-.355 0-.676-.186-.959-.401a1.647 1.647 0 00-1.003-.349c-1.036 0-1.875 1.007-1.875 2.25s.84 2.25 1.875 2.25c.369 0 .713-.128 1.003-.349.283-.215.604-.401.959-.401v0c.31 0 .555.26.532.57a48.039 48.039 0 01-.642 5.056c1.518.19 3.058.309 4.616.354a.64.64 0 00.657-.643v0c0-.355-.186-.676-.401-.959a1.647 1.647 0 01-.349-1.003c0-1.035 1.008-1.875 2.25-1.875 1.243 0 2.25.84 2.25 1.875 0 .369-.128.713-.349 1.003-.215.283-.4.604-.4.959v0c0 .333.277.599.61.58a48.1 48.1 0 005.427-.63 48.05 48.05 0 00.582-4.717.532.532 0 00-.533-.57v0c-.355 0-.676.186-.959.401-.29.221-.634.349-1.003.349-1.035 0-1.875-1.007-1.875-2.25s.84-2.25 1.875-2.25c.37 0 .713.128 1.003.349.283.215.604.401.96.401v0a.656.656 0 00.658-.663 48.422 48.422 0 00-.37-5.36c-1.886.342-3.81.574-5.766.689a.578.578 0 01-.61-.58v0z" /></svg>`
}

const DocumentTextIcon = {
  template: `<svg fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" d="M19.5 14.25v-2.625a3.375 3.375 0 0 0-3.375-3.375h-1.5A1.125 1.125 0 0 1 13.5 7.125v-1.5a3.375 3.375 0 0 0-3.375-3.375H8.25m2.25 0H5.625c-.621 0-1.125.504-1.125 1.125v17.25c0 .621.504 1.125 1.125 1.125h12.75c.621 0 1.125-.504 1.125-1.125V11.25a9 9 0 0 0-9-9Z" /></svg>`
}

// Methods
const showAnalysisModal = (analysisType: any) => {
  if (!analysisType.available) {
    alert('This analysis type requires an active subscription.')
    return
  }
  selectedAnalysisType.value = analysisType
  showNewAnalysisModal.value = true
}

const getAnalysisIcon = (type: string) => {
  const icons: Record<string, any> = {
    'tumor': BeakerIcon,
    'vital-signs': HeartIcon,
    'diagnosis': PuzzlePieceIcon,
    'enhanced-text': DocumentTextIcon
  }
  return icons[type] || BeakerIcon
}

const getStatusBadgeClass = (status: string) => {
  const classes: Record<string, string> = {
    pending: 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900 dark:text-yellow-200',
    processing: 'bg-blue-100 text-blue-800 dark:bg-blue-900 dark:text-blue-200',
    completed: 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-200',
    failed: 'bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-200'
  }
  return classes[status] || classes.pending
}

const getConfidenceColor = (confidence: number) => {
  if (confidence >= 0.8) return 'bg-green-500'
  if (confidence >= 0.6) return 'bg-yellow-500'
  return 'bg-red-500'
}

const formatDateTime = (dateTime: string) => {
  return new Date(dateTime).toLocaleString()
}

const viewAnalysis = (analysis: AnalysisResult) => {
  selectedAnalysis.value = analysis
  showAnalysisDetailsModal.value = true
}

const downloadReport = (analysis: AnalysisResult) => {
  console.log('Download report for analysis:', analysis.id)
  // Implement report download functionality
}

const deleteAnalysis = async (analysis: AnalysisResult) => {
  if (confirm('Are you sure you want to delete this analysis?')) {
    try {
      await analysisStore.deleteAnalysis(analysis.id)
    } catch (error) {
      console.error('Error deleting analysis:', error)
      alert('Failed to delete analysis. Please try again.')
    }
  }
}

const closeModals = () => {
  showNewAnalysisModal.value = false
  showAnalysisDetailsModal.value = false
  selectedAnalysis.value = null
  selectedAnalysisType.value = null
}

const handleRunAnalysis = async (analysisData: any) => {
  try {
    const analysisType = selectedAnalysisType.value?.id
    
    switch (analysisType) {
      case 'tumor':
        await analysisStore.requestTumorAnalysis(analysisData)
        break
      case 'vital-signs':
        await analysisStore.requestVitalSignsAnalysis(analysisData)
        break
      case 'diagnosis':
        await analysisStore.requestDiagnosisAnalysis(analysisData)
        break
      case 'enhanced-text':
        await analysisStore.requestEnhancedTextAnalysis(analysisData)
        break
      default:
        throw new Error('Unknown analysis type')
    }
    
    closeModals()
  } catch (error) {
    console.error('Error running analysis:', error)
    alert('Failed to run analysis. Please try again.')
  }
}
</script>
