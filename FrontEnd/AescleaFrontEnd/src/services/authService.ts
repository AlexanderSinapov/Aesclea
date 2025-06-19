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

      return data
    } catch (error: any) {
      console.error('AuthService: Registration error:', error)
      const errorMessage = error.response?.data?.message || error.message || 'Registration failed'
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