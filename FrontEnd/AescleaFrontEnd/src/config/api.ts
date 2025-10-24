// Copyright (c) 2025 Alexander Sinapov | Simeon Petkov

// All rights reserved.
// This code is proprietary and confidential.  
// Unauthorized copying, modification, distribution, or use is strictly prohibited.

// API Configuration
export const API_CONFIG = {
  // Base URL for the backend API
  // Backend runs on https://localhost:7000 by default
  baseUrl: import.meta.env.VITE_API_URL || 'https://localhost:7000/api',
  
  // API endpoints
  endpoints: {
    // AI endpoints
    aiChat: '/ai/chat',
    aiAnalyze: '/ai/analyze',
    
    // Auth endpoints
    login: '/auth/login',
    register: '/auth/register',
    
    // Patient endpoints
    patients: '/patients',
    
    // Other endpoints can be added here
  },
  
  // Request timeout in milliseconds
  timeout: 30000,
  
  // Default headers
  headers: {
    'Content-Type': 'application/json',
  }
}

// Helper function to get full endpoint URL
export function getEndpointUrl(endpoint: keyof typeof API_CONFIG.endpoints): string {
  return `${API_CONFIG.baseUrl}${API_CONFIG.endpoints[endpoint]}`
}

// Helper function for API calls with error handling
export async function apiCall<T>(
  endpoint: keyof typeof API_CONFIG.endpoints,
  options: RequestInit = {}
): Promise<T> {
  const url = getEndpointUrl(endpoint)
  
  const response = await fetch(url, {
    ...options,
    headers: {
      ...API_CONFIG.headers,
      ...options.headers,
    },
  })
  
  if (!response.ok) {
    const error = await response.json().catch(() => ({ message: response.statusText }))
    throw new Error(error.message || 'API request failed')
  }
  
  return response.json()
}
