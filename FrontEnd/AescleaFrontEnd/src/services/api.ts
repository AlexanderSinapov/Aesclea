import axios from 'axios'

console.log('API_BASE_URL from env:', import.meta.env.VITE_API_URL)
// Check if running in development or production
const API_BASE_URL = import.meta.env.VITE_API_URL || 'http://localhost:7000/api'

// Create axios instance
const api = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
  timeout: 10000, // 10 second timeout
})

// Request interceptor to add auth token
api.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('accessToken')
    if (token) {
      config.headers.Authorization = `Bearer ${token}`
    }
    console.log('API Request:', config.method?.toUpperCase(), config.url, config.data)
    return config
  },
  (error) => {
    console.error('API Request Error:', error)
    return Promise.reject(error)
  }
)

// Response interceptor to handle auth errors
api.interceptors.response.use(
  (response) => {
    console.log('API Response:', response.status, response.config.url, response.data)
    return response
  },
  (error) => {
    console.error('API Response Error:', error.response?.status, error.response?.data || error.message)
    
    if (error.response?.status === 401) {
      // Clear tokens and redirect to login
      localStorage.removeItem('accessToken')
      localStorage.removeItem('refreshToken')
      localStorage.removeItem('user')
      window.location.href = '/login'
    }
    return Promise.reject(error)
  }
)

// Add a test function to verify API connectivity
export const testApiConnection = async () => {
  try {
    console.log('Testing API connection...')
    const response = await api.get('/test')
    console.log('API Test successful:', response.data)
    return true
  } catch (error) {
    console.error('API Test failed:', error)
    return false
  }
}

export default api