<template>
  <div class="fixed inset-0 z-[9999] overflow-y-auto">
    <div class="flex items-end justify-center min-h-screen px-4 pt-4 pb-20 text-center sm:block sm:p-0">
      <!-- Background overlay with blur effect -->
      <div 
        class="fixed inset-0 backdrop-blur-sm bg-black/20 transition-opacity" 
        @click="$emit('close')"
      ></div>

      <!-- Modal panel -->
      <div class="relative inline-block overflow-hidden text-left align-bottom transition-all transform bg-white rounded-lg shadow-xl dark:bg-gray-800 sm:my-8 sm:align-middle sm:max-w-2xl sm:w-full z-10">
        <!-- Header -->
        <div class="px-4 pt-5 pb-4 bg-white dark:bg-gray-800 sm:p-6 sm:pb-4">
          <div class="flex items-center justify-between">
            <div class="flex items-center">
              <div class="flex-shrink-0">
                <component :is="getAnalysisIcon()" class="w-8 h-8 text-purple-600" />
              </div>
              <div class="ml-3">
                <h3 class="text-lg font-medium text-gray-900 dark:text-white">
                  {{ isViewMode ? 'Analysis Results' : `Run ${analysisType?.name || 'Analysis'}` }}
                </h3>
                <p v-if="!isViewMode" class="text-sm text-gray-500 dark:text-gray-400">
                  {{ analysisType?.description }}
                </p>
              </div>
            </div>
            <button
              @click="$emit('close')"
              class="text-gray-400 hover:text-gray-500 dark:hover:text-gray-300"
            >
              <XMarkIcon class="w-6 h-6" />
            </button>
          </div>
        </div>

        <!-- Content -->
        <div class="px-4 pb-4 sm:px-6">
          <!-- View Mode - Analysis Results -->
          <div v-if="isViewMode && analysis" class="space-y-6">
            <!-- Analysis Info -->
            <div class="p-4 rounded-lg bg-gray-50 dark:bg-gray-700">
              <div class="grid grid-cols-2 gap-4">
                <div>
                  <dt class="text-sm font-medium text-gray-500 dark:text-gray-400">Patient</dt>
                  <dd class="mt-1 text-sm text-gray-900 dark:text-white">{{ analysis.patientName }}</dd>
                </div>
                <div>
                  <dt class="text-sm font-medium text-gray-500 dark:text-gray-400">Analysis Type</dt>
                  <dd class="mt-1 text-sm text-gray-900 capitalize dark:text-white">{{ analysis.analysisType.replace('-', ' ') }}</dd>
                </div>
                <div>
                  <dt class="text-sm font-medium text-gray-500 dark:text-gray-400">Status</dt>
                  <dd class="mt-1">
                    <span 
                      class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium"
                      :class="getStatusBadgeClass(analysis.status)"
                    >
                      {{ analysis.status }}
                    </span>
                  </dd>
                </div>
                <div v-if="analysis.confidence">
                  <dt class="text-sm font-medium text-gray-500 dark:text-gray-400">Confidence</dt>
                  <dd class="mt-1 text-sm text-gray-900 dark:text-white">{{ Math.round(analysis.confidence * 100) }}%</dd>
                </div>
                <div>
                  <dt class="text-sm font-medium text-gray-500 dark:text-gray-400">Created</dt>
                  <dd class="mt-1 text-sm text-gray-900 dark:text-white">{{ formatDateTime(analysis.createdAt) }}</dd>
                </div>
                <div v-if="analysis.completedAt">
                  <dt class="text-sm font-medium text-gray-500 dark:text-gray-400">Completed</dt>
                  <dd class="mt-1 text-sm text-gray-900 dark:text-white">{{ formatDateTime(analysis.completedAt) }}</dd>
                </div>
              </div>
            </div>

            <!-- Results -->
            <div v-if="analysis.results" class="p-4 rounded-lg bg-gray-50 dark:bg-gray-700">
              <h4 class="mb-3 text-sm font-medium text-gray-900 dark:text-white">Analysis Results</h4>
              <pre class="text-sm text-gray-700 whitespace-pre-wrap dark:text-gray-300">{{ JSON.stringify(analysis.results, null, 2) }}</pre>
            </div>

            <!-- Recommendations -->
            <div v-if="analysis.recommendations && analysis.recommendations.length > 0" class="p-4 rounded-lg bg-gray-50 dark:bg-gray-700">
              <h4 class="mb-3 text-sm font-medium text-gray-900 dark:text-white">Recommendations</h4>
              <ul class="space-y-2">
                <li v-for="(recommendation, index) in analysis.recommendations" :key="index" class="flex items-start">
                  <CheckCircleIcon class="h-5 w-5 text-green-500 mt-0.5 mr-2 flex-shrink-0" />
                  <span class="text-sm text-gray-700 dark:text-gray-300">{{ recommendation }}</span>
                </li>
              </ul>
            </div>
          </div>

          <!-- Create Mode - Analysis Form -->
          <form v-else @submit.prevent="handleSubmit">
            <div class="space-y-4">
              <!-- Patient Selection -->
              <div>
                <label for="patientId" class="block text-sm font-medium text-gray-700 dark:text-gray-300">
                  Select Patient *
                </label>
                <select
                  id="patientId"
                  v-model="formData.patientId"
                  required
                  class="block w-full px-3 py-2 mt-1 text-gray-900 bg-white border border-gray-300 rounded-md shadow-sm dark:border-gray-600 dark:bg-gray-700 dark:text-white focus:outline-none focus:ring-purple-500 focus:border-purple-500 sm:text-sm"
                >
                  <option value="">Choose a patient</option>
                  <option v-for="patient in availablePatients" :key="patient.id" :value="patient.id">
                    {{ patient.firstName }} {{ patient.lastName }} - {{ patient.department }}
                  </option>
                </select>
              </div>

              <!-- Department Selection -->
              <div>
                <label for="department" class="block text-sm font-medium text-gray-700 dark:text-gray-300">
                  Department *
                </label>
                <select
                  id="department"
                  v-model="formData.department"
                  required
                  class="block w-full px-3 py-2 mt-1 text-gray-900 bg-white border border-gray-300 rounded-md shadow-sm dark:border-gray-600 dark:bg-gray-700 dark:text-white focus:outline-none focus:ring-purple-500 focus:border-purple-500 sm:text-sm"
                >
                  <option value="">Select department</option>
                  <option value="cardiology">Cardiology</option>
                  <option value="neurology">Neurology</option>
                  <option value="oncology">Oncology</option>
                  <option value="radiology">Radiology</option>
                  <option value="emergency">Emergency</option>
                  <option value="pediatrics">Pediatrics</option>
                </select>
              </div>

              <!-- Analysis Type Specific Fields -->
              <div v-if="analysisType?.id === 'tumor'" class="space-y-4">
                <div>
                  <label for="tumorType" class="block text-sm font-medium text-gray-700 dark:text-gray-300">
                    Tumor Type
                  </label>
                  <select
                    id="tumorType"
                    v-model="formData.data.tumorType"
                    class="block w-full px-3 py-2 mt-1 text-gray-900 bg-white border border-gray-300 rounded-md shadow-sm dark:border-gray-600 dark:bg-gray-700 dark:text-white focus:outline-none focus:ring-purple-500 focus:border-purple-500 sm:text-sm"
                  >
                    <option value="brain">Brain Tumor</option>
                    <option value="breast">Breast Cancer</option>
                    <option value="lung">Lung Cancer</option>
                    <option value="skin">Skin Cancer</option>
                  </select>
                </div>
                
                <!-- Image Upload Section -->
                <div>
                  <label class="block mb-2 text-sm font-medium text-gray-700 dark:text-gray-300">
                    Medical Image *
                  </label>
                  
                  <!-- File Upload -->
                  <div class="space-y-4">
                    <div class="flex items-center justify-center w-full">
                      <label for="imageFile" class="flex flex-col items-center justify-center w-full h-32 border-2 border-gray-300 border-dashed rounded-lg cursor-pointer bg-gray-50 dark:hover:bg-bray-800 dark:bg-gray-700 hover:bg-gray-100 dark:border-gray-600 dark:hover:border-gray-500 dark:hover:bg-gray-600">
                        <div class="flex flex-col items-center justify-center pt-5 pb-6">
                          <svg class="w-8 h-8 mb-4 text-gray-500 dark:text-gray-400" fill="none" viewBox="0 0 20 20">
                            <path stroke="currentColor" stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13.13 5.13l4.24 4.24M15 9.75l-6.75 6.75H3v-5.25l6.75-6.75m.75.75l1.5 1.5m3-3l1.5 1.5M9 12.75h6.75"/>
                          </svg>
                          <p class="mb-2 text-sm text-gray-500 dark:text-gray-400">
                            <span class="font-semibold">Click to upload</span> medical image
                          </p>
                          <p class="text-xs text-gray-500 dark:text-gray-400">DICOM, PNG, JPG, JPEG (MAX. 10MB)</p>
                        </div>
                        <input 
                          id="imageFile" 
                          type="file" 
                          class="hidden" 
                          accept=".dcm,.png,.jpg,.jpeg,.tiff,.bmp"
                          @change="handleImageUpload"
                        />
                      </label>
                    </div>
                    
                    <!-- Image Preview -->
                    <div v-if="selectedImage" class="relative">
                      <img 
                        :src="selectedImage.preview" 
                        alt="Medical image preview" 
                        class="mx-auto border border-gray-300 rounded-lg max-h-48 dark:border-gray-600"
                      />
                      <button
                        @click="removeImage"
                        class="absolute p-1 text-white bg-red-500 rounded-full top-2 right-2 hover:bg-red-600"
                      >
                        <svg class="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
                        </svg>
                      </button>
                      <div class="mt-2 text-sm text-gray-600 dark:text-gray-400">
                        <p>File: {{ selectedImage.name }}</p>
                        <p>Size: {{ formatFileSize(selectedImage.size) }}</p>
                      </div>
                    </div>
                    
                    <!-- Alternative: URL Input -->
                    <div class="relative">
                      <div class="absolute inset-0 flex items-center">
                        <div class="w-full border-t border-gray-300 dark:border-gray-600" />
                      </div>
                      <div class="relative flex justify-center text-sm">
                        <span class="px-2 text-gray-500 bg-white dark:bg-gray-800">Or provide image URL</span>
                      </div>
                    </div>
                    
                    <div>
                      <input
                        v-model="formData.data.imageUrl"
                        type="url"
                        placeholder="https://example.com/medical-image.jpg"
                        class="block w-full px-3 py-2 mt-1 text-gray-900 bg-white border border-gray-300 rounded-md shadow-sm dark:border-gray-600 dark:bg-gray-700 dark:text-white focus:outline-none focus:ring-purple-500 focus:border-purple-500 sm:text-sm"
                      />
                    </div>
                  </div>
                </div>
              </div>

              <div v-else-if="analysisType?.id === 'vital-signs'" class="space-y-4">
                <div class="grid grid-cols-2 gap-4">
                  <div>
                    <label for="heartRate" class="block text-sm font-medium text-gray-700 dark:text-gray-300">
                      Heart Rate (BPM)
                    </label>
                    <input
                      id="heartRate"
                      v-model.number="formData.data.heartRate"
                      type="number"
                      min="30"
                      max="200"
                      class="block w-full px-3 py-2 mt-1 text-gray-900 bg-white border border-gray-300 rounded-md shadow-sm dark:border-gray-600 dark:bg-gray-700 dark:text-white focus:outline-none focus:ring-purple-500 focus:border-purple-500 sm:text-sm"
                    />
                  </div>
                  <div>
                    <label for="bloodPressure" class="block text-sm font-medium text-gray-700 dark:text-gray-300">
                      Blood Pressure
                    </label>
                    <input
                      id="bloodPressure"
                      v-model="formData.data.bloodPressure"
                      type="text"
                      placeholder="120/80"
                      class="block w-full px-3 py-2 mt-1 text-gray-900 bg-white border border-gray-300 rounded-md shadow-sm dark:border-gray-600 dark:bg-gray-700 dark:text-white focus:outline-none focus:ring-purple-500 focus:border-purple-500 sm:text-sm"
                    />
                  </div>
                  <div>
                    <label for="temperature" class="block text-sm font-medium text-gray-700 dark:text-gray-300">
                      Temperature (°F)
                    </label>
                    <input
                      id="temperature"
                      v-model.number="formData.data.temperature"
                      type="number"
                      step="0.1"
                      min="95"
                      max="110"
                      class="block w-full px-3 py-2 mt-1 text-gray-900 bg-white border border-gray-300 rounded-md shadow-sm dark:border-gray-600 dark:bg-gray-700 dark:text-white focus:outline-none focus:ring-purple-500 focus:border-purple-500 sm:text-sm"
                    />
                  </div>
                  <div>
                    <label for="oxygenSaturation" class="block text-sm font-medium text-gray-700 dark:text-gray-300">
                      Oxygen Saturation (%)
                    </label>
                    <input
                      id="oxygenSaturation"
                      v-model.number="formData.data.oxygenSaturation"
                      type="number"
                      min="80"
                      max="100"
                      class="block w-full px-3 py-2 mt-1 text-gray-900 bg-white border border-gray-300 rounded-md shadow-sm dark:border-gray-600 dark:bg-gray-700 dark:text-white focus:outline-none focus:ring-purple-500 focus:border-purple-500 sm:text-sm"
                    />
                  </div>
                </div>
              </div>

              <div v-else-if="analysisType?.id === 'diagnosis'" class="space-y-4">
                <div>
                  <label for="symptoms" class="block text-sm font-medium text-gray-700 dark:text-gray-300">
                    Symptoms *
                  </label>
                  <textarea
                    id="symptoms"
                    v-model="formData.data.symptoms"
                    rows="4"
                    required
                    class="block w-full px-3 py-2 mt-1 text-gray-900 bg-white border border-gray-300 rounded-md shadow-sm dark:border-gray-600 dark:bg-gray-700 dark:text-white focus:outline-none focus:ring-purple-500 focus:border-purple-500 sm:text-sm"
                    placeholder="Describe the patient's symptoms..."
                  ></textarea>
                </div>
                <div>
                  <label for="medicalHistory" class="block text-sm font-medium text-gray-700 dark:text-gray-300">
                    Relevant Medical History
                  </label>
                  <textarea
                    id="medicalHistory"
                    v-model="formData.data.medicalHistory"
                    rows="3"
                    class="block w-full px-3 py-2 mt-1 text-gray-900 bg-white border border-gray-300 rounded-md shadow-sm dark:border-gray-600 dark:bg-gray-700 dark:text-white focus:outline-none focus:ring-purple-500 focus:border-purple-500 sm:text-sm"
                    placeholder="Any relevant medical history..."
                  ></textarea>
                </div>
              </div>

              <div v-else-if="analysisType?.id === 'enhanced-text'" class="space-y-4">
                <div>
                  <label for="textData" class="block text-sm font-medium text-gray-700 dark:text-gray-300">
                    Medical Report Text *
                  </label>
                  <textarea
                    id="textData"
                    v-model="formData.data.textData"
                    rows="6"
                    required
                    class="block w-full px-3 py-2 mt-1 text-gray-900 bg-white border border-gray-300 rounded-md shadow-sm dark:border-gray-600 dark:bg-gray-700 dark:text-white focus:outline-none focus:ring-purple-500 focus:border-purple-500 sm:text-sm"
                    placeholder="Paste the medical report text here for analysis..."
                  ></textarea>
                </div>
                <div>
                  <label for="analysisType" class="block text-sm font-medium text-gray-700 dark:text-gray-300">
                    Analysis Focus
                  </label>
                  <select
                    id="analysisType"
                    v-model="formData.data.analysisType"
                    class="block w-full px-3 py-2 mt-1 text-gray-900 bg-white border border-gray-300 rounded-md shadow-sm dark:border-gray-600 dark:bg-gray-700 dark:text-white focus:outline-none focus:ring-purple-500 focus:border-purple-500 sm:text-sm"
                  >
                    <option value="general">General Analysis</option>
                    <option value="pathology">Pathology Report</option>
                    <option value="radiology">Radiology Report</option>
                    <option value="lab">Lab Results</option>
                  </select>
                </div>
              </div>

              <!-- Notes -->
              <div>
                <label for="notes" class="block text-sm font-medium text-gray-700 dark:text-gray-300">
                  Additional Notes
                </label>
                <textarea
                  id="notes"
                  v-model="formData.notes"
                  rows="3"
                  class="block w-full px-3 py-2 mt-1 text-gray-900 bg-white border border-gray-300 rounded-md shadow-sm dark:border-gray-600 dark:bg-gray-700 dark:text-white focus:outline-none focus:ring-purple-500 focus:border-purple-500 sm:text-sm"
                  placeholder="Any additional notes or instructions for the analysis..."
                ></textarea>
              </div>
            </div>

            <!-- Footer -->
            <div class="flex justify-end mt-6 space-x-3">
              <button
                type="button"
                @click="$emit('close')"
                class="px-4 py-2 text-sm font-medium text-gray-700 bg-white border border-gray-300 rounded-md shadow-sm dark:border-gray-600 dark:text-gray-300 dark:bg-gray-800 hover:bg-gray-50 dark:hover:bg-gray-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-purple-500"
              >
                Cancel
              </button>
              <button
                type="submit"
                :disabled="isSubmitting || !isFormValid"
                class="px-4 py-2 text-sm font-medium text-white bg-purple-600 border border-transparent rounded-md shadow-sm hover:bg-purple-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-purple-500 disabled:opacity-50 disabled:cursor-not-allowed"
              >
                <svg 
                  v-if="isSubmitting" 
                  class="w-5 h-5 mr-3 -ml-1 text-white animate-spin" 
                  fill="none" 
                  viewBox="0 0 24 24"
                >
                  <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
                  <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                </svg>
                {{ isSubmitting ? 'Running Analysis...' : 'Run Analysis' }}
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import { usePatientsStore } from '../../stores/patients'
import type { AnalysisResult } from '../../stores/analysis'

// Define props
interface Props {
  analysis?: AnalysisResult | null
  analysisType?: any
  isViewMode?: boolean
}

const props = defineProps<Props>()

// Define emits
const emit = defineEmits(['close', 'save'])

// Store
const patientsStore = usePatientsStore()

// Local state
const isSubmitting = ref(false)
const selectedImage = ref<{
  file: File
  preview: string
  name: string
  size: number
} | null>(null)

const formData = ref({
  patientId: '',
  department: '',
  data: {} as any,
  notes: ''
})

// Computed properties
const availablePatients = computed(() => patientsStore.patients)

const isFormValid = computed(() => {
  if (!formData.value.patientId) return false
  
  if (formData.value.department === 'tumor') {
    return (selectedImage.value || formData.value.data.imageUrl) && formData.value.data.tumorType
  }
  
  if (formData.value.department === 'diagnosis') {
    return formData.value.data.symptoms
  }
  
  if (formData.value.department === 'enhanced-text') {
    return formData.value.data.text
  }
  
  return true
})

// Icon components
const XMarkIcon = {
  template: `<svg fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" d="M6 18L18 6M6 6l12 12" /></svg>`
}

const CheckCircleIcon = {
  template: `<svg fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" d="M9 12.75L11.25 15 15 9.75M21 12a9 9 0 11-18 0 9 9 0 0118 0z" /></svg>`
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

// Watch for analysis type changes
watch(() => props.analysisType, (newType) => {
  if (newType) {
    // Initialize form data based on analysis type
    formData.value.data = {}
    
    switch (newType.id) {
      case 'tumor':
        formData.value.data = {
          tumorType: 'brain',
          imageUrl: ''
        }
        break
      case 'vital-signs':
        formData.value.data = {
          heartRate: null,
          bloodPressure: '',
          temperature: null,
          oxygenSaturation: null
        }
        break
      case 'diagnosis':
        formData.value.data = {
          symptoms: '',
          medicalHistory: ''
        }
        break
      case 'enhanced-text':
        formData.value.data = {
          textData: '',
          analysisType: 'general'
        }
        break
    }
  }
}, { immediate: true })

// Methods
const getAnalysisIcon = () => {
  if (props.isViewMode && props.analysis) {
    const icons: Record<string, any> = {
      'tumor': BeakerIcon,
      'vital-signs': HeartIcon,
      'diagnosis': PuzzlePieceIcon,
      'enhanced-text': DocumentTextIcon
    }
    return icons[props.analysis.analysisType] || BeakerIcon
  }
  
  if (props.analysisType) {
    const icons: Record<string, any> = {
      'tumor': BeakerIcon,
      'vital-signs': HeartIcon,
      'diagnosis': PuzzlePieceIcon,
      'enhanced-text': DocumentTextIcon
    }
    return icons[props.analysisType.id] || BeakerIcon
  }
  
  return BeakerIcon
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

const formatDateTime = (dateTime: string) => {
  return new Date(dateTime).toLocaleString()
}

// Image handling functions
const handleImageUpload = (event: Event) => {
  const target = event.target as HTMLInputElement
  const file = target.files?.[0]
  
  if (!file) return
  
  // Validate file type
  const allowedTypes = ['image/jpeg', 'image/jpg', 'image/png', 'image/tiff', 'image/bmp', 'application/dicom']
  if (!allowedTypes.includes(file.type) && !file.name.toLowerCase().endsWith('.dcm')) {
    alert('Please select a valid medical image file (DICOM, PNG, JPG, JPEG, TIFF, BMP)')
    return
  }
  
  // Validate file size (10MB max)
  if (file.size > 10 * 1024 * 1024) {
    alert('File size must be less than 10MB')
    return
  }
  
  // Create preview
  const reader = new FileReader()
  reader.onload = (e) => {
    selectedImage.value = {
      file,
      preview: e.target?.result as string,
      name: file.name,
      size: file.size
    }
  }
  reader.readAsDataURL(file)
}

const removeImage = () => {
  selectedImage.value = null
  // Clear the file input
  const fileInput = document.getElementById('imageFile') as HTMLInputElement
  if (fileInput) {
    fileInput.value = ''
  }
}

const formatFileSize = (bytes: number) => {
  if (bytes === 0) return '0 Bytes'
  const k = 1024
  const sizes = ['Bytes', 'KB', 'MB', 'GB']
  const i = Math.floor(Math.log(bytes) / Math.log(k))
  return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i]
}

const handleSubmit = async () => {
  isSubmitting.value = true
  
  try {
    // Get patient name for the request
    const patient = availablePatients.value.find(p => p.id === formData.value.patientId)
    
    const analysisRequest = {
      patientId: formData.value.patientId,
      patientName: patient ? `${patient.firstName} ${patient.lastName}` : 'Unknown',
      analysisType: props.analysisType?.id || 'unknown',
      department: formData.value.department,
      data: { ...formData.value.data },
      notes: formData.value.notes
    }
    
    // Handle image for tumor analysis
    if (props.analysisType?.id === 'tumor' && selectedImage.value) {
      // Convert file to base64 for API request
      const reader = new FileReader()
      reader.onload = async () => {
        analysisRequest.data.imageData = reader.result as string
        analysisRequest.data.imageName = selectedImage.value!.name
        analysisRequest.data.imageSize = selectedImage.value!.size
        
        emit('save', analysisRequest)
      }
      reader.readAsDataURL(selectedImage.value.file)
      return
    }
    
    emit('save', analysisRequest)
  } finally {
    isSubmitting.value = false
  }
}
</script>
