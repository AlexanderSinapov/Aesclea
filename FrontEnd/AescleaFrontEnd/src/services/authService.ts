import api from './api'

export interface LoginRequest {
  email: string
  password: string
}

export interface RegisterRequest {
  email: string
  password: string
  firstName: string
  lastName: string
  phone: string
  role: string
  medicalNumber: string
  hospital: string
}

export interface AuthResponse {
  success: boolean
  message: string
  accessToken?: string
  refreshToken?: string
  user?: User
}

export interface User {
  id: string
  email: string
  firstName: string
  lastName: string
  phone: string
  role: string
  hospital: string
  createdAt: string
  updatedAt: string
}

class AuthService {
  async login(credentials: LoginRequest): Promise<AuthResponse> {
    try {
      console.log('AuthService: Attempting login with:', { email: credentials.email })
      const response = await api.post('/auth/login', credentials)
      const data = response.data

      console.log('AuthService: Login response:', data)

      if (data.success && data.accessToken) {
        localStorage.setItem('accessToken', data.accessToken)
        if (data.refreshToken) {
          localStorage.setItem('refreshToken', data.refreshToken)
        }
        if (data.user) {
          localStorage.setItem('user', JSON.stringify(data.user))
        }
        console.log('AuthService: Login successful, tokens stored')
      }

      return data
    } catch (error: any) {
      console.error('AuthService: Login error:', error)
      const errorMessage = error.response?.data?.message || error.message || 'Login failed'
      throw new Error(errorMessage)
    }
  }

  async register(userData: RegisterRequest): Promise<AuthResponse> {
    try {
      console.log('AuthService: Attempting registration with:', userData)
      const response = await api.post('/auth/register', userData)
      const data = response.data

      console.log('AuthService: Registration response:', data)

      if (data.success && data.accessToken) {
        localStorage.setItem('accessToken', data.accessToken)
        if (data.refreshToken) {
          localStorage.setItem('refreshToken', data.refreshToken)
        }
        if (data.user) {
          localStorage.setItem('user', JSON.stringify(data.user))
        }
        console.log('AuthService: Registration successful, tokens stored')
      }

      return data    } catch (error: any) {
      console.error('AuthService: Registration error:', error)
      console.error('AuthService: Error response:', error.response?.data)
      
      let errorMessage = 'Registration failed. Please try again.'
      
      if (error.response?.data) {
        if (error.response.data.message) {
          errorMessage = error.response.data.message
        } else if (error.response.data.errors) {
          // Handle validation errors
          const validationErrors = error.response.data.errors
          const errorMessages = []
          for (const field in validationErrors) {
            errorMessages.push(`${field}: ${validationErrors[field].join(', ')}`)
          }
          errorMessage = errorMessages.join('; ')
        } else if (error.response.data.title) {
          errorMessage = error.response.data.title
        }
      } else if (error.message) {
        errorMessage = error.message
      }
      
      throw new Error(errorMessage)
    }
  }

  async logout(): Promise<void> {
    try {
      await api.post('/auth/logout')
    } catch (error) {
      console.error('Logout error:', error)
    } finally {
      localStorage.removeItem('accessToken')
      localStorage.removeItem('refreshToken')
      localStorage.removeItem('user')
    }
  }

  async getCurrentUser(): Promise<User | null> {
    try {
      const response = await api.get('/auth/me')
      return response.data
    } catch (error) {
      return null
    }
  }

  isAuthenticated(): boolean {
    return !!localStorage.getItem('accessToken')
  }

  getUser(): User | null {
    const userStr = localStorage.getItem('user')
    return userStr ? JSON.parse(userStr) : null
  }
}

export default new AuthService()