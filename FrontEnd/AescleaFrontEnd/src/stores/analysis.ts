import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import api from '../services/api'

export interface AnalysisResult {
  id: string
  patientId: string
  patientName: string
  analysisType: 'tumor' | 'vital-signs' | 'diagnosis' | 'enhanced-text'
  status: 'pending' | 'processing' | 'completed' | 'failed'
  results?: any
  confidence?: number
  recommendations?: string[]
  createdAt: string
  completedAt?: string
  department: string
}

export interface AIAnalysisRequest {
  patientId: string
  analysisType: string
  data: any
  department: string
}

export const useAnalysisStore = defineStore('analysis', () => {
  const analyses = ref<AnalysisResult[]>([])
  const isLoading = ref(false)
  const error = ref<string | null>(null)

  const pendingAnalyses = computed(() => 
    analyses.value.filter(a => a.status === 'pending' || a.status === 'processing')
  )
  
  const completedAnalyses = computed(() => 
    analyses.value.filter(a => a.status === 'completed')
  )

  const analysesByDepartment = computed(() => {
    const departments: Record<string, AnalysisResult[]> = {}
    analyses.value.forEach(analysis => {
      if (!departments[analysis.department]) {
        departments[analysis.department] = []
      }
      departments[analysis.department].push(analysis)
    })
    return departments
  })

  const fetchAnalyses = async () => {
    isLoading.value = true
    error.value = null
    
    try {
      const response = await api.get('/analysis')
      analyses.value = response.data
    } catch (err: any) {
      error.value = err.response?.data?.message || err.message || 'Failed to fetch analyses'
      console.error('Error fetching analyses:', err)
    } finally {
      isLoading.value = false
    }
  }

  const requestTumorAnalysis = async (request: AIAnalysisRequest) => {
    isLoading.value = true
    error.value = null
    
    try {
      const response = await fetch('/api/TumorAnalysis/analyze', {
        method: 'POST',
        headers: {
          'Authorization': `Bearer ${localStorage.getItem('auth_token')}`,
          'Content-Type': 'application/json'
        },
        body: JSON.stringify(request)
      })
      
      if (!response.ok) {
        throw new Error('Failed to request tumor analysis')
      }
      
      const result = await response.json()
      const analysis: AnalysisResult = {
        id: result.id || crypto.randomUUID(),
        patientId: request.patientId,
        patientName: result.patientName || 'Unknown',
        analysisType: 'tumor',
        status: 'completed',
        results: result,
        confidence: result.confidence,
        recommendations: result.recommendations,
        createdAt: new Date().toISOString(),
        completedAt: new Date().toISOString(),
        department: request.department
      }
      
      analyses.value.push(analysis)
      return analysis
    } catch (err: any) {
      error.value = err.message
      throw err
    } finally {
      isLoading.value = false
    }
  }

  const requestDiagnosisAnalysis = async (request: AIAnalysisRequest) => {
    isLoading.value = true
    error.value = null
    
    try {
      const response = await fetch('/api/Diagnosis/analyze', {
        method: 'POST',
        headers: {
          'Authorization': `Bearer ${localStorage.getItem('auth_token')}`,
          'Content-Type': 'application/json'
        },
        body: JSON.stringify(request)
      })
      
      if (!response.ok) {
        throw new Error('Failed to request diagnosis analysis')
      }
      
      const result = await response.json()
      const analysis: AnalysisResult = {
        id: result.id || crypto.randomUUID(),
        patientId: request.patientId,
        patientName: result.patientName || 'Unknown',
        analysisType: 'diagnosis',
        status: 'completed',
        results: result,
        confidence: result.confidence,
        recommendations: result.recommendations,
        createdAt: new Date().toISOString(),
        completedAt: new Date().toISOString(),
        department: request.department
      }
      
      analyses.value.push(analysis)
      return analysis
    } catch (err: any) {
      error.value = err.message
      throw err
    } finally {
      isLoading.value = false
    }
  }

  const requestVitalSignsAnalysis = async (request: AIAnalysisRequest) => {
    isLoading.value = true
    error.value = null
    
    try {
      const response = await fetch('/api/VitalSigns/analyze', {
        method: 'POST',
        headers: {
          'Authorization': `Bearer ${localStorage.getItem('auth_token')}`,
          'Content-Type': 'application/json'
        },
        body: JSON.stringify(request)
      })
      
      if (!response.ok) {
        throw new Error('Failed to request vital signs analysis')
      }
      
      const result = await response.json()
      const analysis: AnalysisResult = {
        id: result.id || crypto.randomUUID(),
        patientId: request.patientId,
        patientName: result.patientName || 'Unknown',
        analysisType: 'vital-signs',
        status: 'completed',
        results: result,
        confidence: result.confidence,
        recommendations: result.recommendations,
        createdAt: new Date().toISOString(),
        completedAt: new Date().toISOString(),
        department: request.department
      }
      
      analyses.value.push(analysis)
      return analysis
    } catch (err: any) {
      error.value = err.message
      throw err
    } finally {
      isLoading.value = false
    }
  }

  const requestEnhancedTextAnalysis = async (request: AIAnalysisRequest) => {
    isLoading.value = true
    error.value = null
    
    try {
      const response = await fetch('/api/EnhancedTumorAnalysis/analyze', {
        method: 'POST',
        headers: {
          'Authorization': `Bearer ${localStorage.getItem('auth_token')}`,
          'Content-Type': 'application/json'
        },
        body: JSON.stringify(request)
      })
      
      if (!response.ok) {
        throw new Error('Failed to request enhanced text analysis')
      }
      
      const result = await response.json()
      const analysis: AnalysisResult = {
        id: result.id || crypto.randomUUID(),
        patientId: request.patientId,
        patientName: result.patientName || 'Unknown',
        analysisType: 'enhanced-text',
        status: 'completed',
        results: result,
        confidence: result.confidence,
        recommendations: result.recommendations,
        createdAt: new Date().toISOString(),
        completedAt: new Date().toISOString(),
        department: request.department
      }
      
      analyses.value.push(analysis)
      return analysis
    } catch (err: any) {
      error.value = err.message
      throw err
    } finally {
      isLoading.value = false
    }
  }

  const requestAnalysis = async (request: AIAnalysisRequest) => {
    isLoading.value = true
    error.value = null
    
    try {
      const response = await api.post('/analysis', request)
      const newAnalysis = response.data.analysis
      analyses.value.push(newAnalysis)
      return newAnalysis
    } catch (err: any) {
      error.value = err.response?.data?.message || err.message || 'Failed to request analysis'
      throw err
    } finally {
      isLoading.value = false
    }
  }

  const deleteAnalysis = async (id: string) => {
    try {
      await api.delete(`/analysis/${id}`)
      analyses.value = analyses.value.filter(a => a.id !== id)
    } catch (err: any) {
      console.error('Error deleting analysis:', err)
      throw err
    }
  }

  return {
    analyses,
    isLoading,
    error,
    pendingAnalyses,
    completedAnalyses,
    analysesByDepartment,
    fetchAnalyses,
    requestAnalysis,
    requestTumorAnalysis,
    requestDiagnosisAnalysis,
    requestVitalSignsAnalysis,
    requestEnhancedTextAnalysis,
    deleteAnalysis
  }
})
