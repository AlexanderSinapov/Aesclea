import { ref, watch } from 'vue'

// Global reactive state
const isDark = ref(false)
let isInitialized = false

// Simple theme functions
const applyTheme = (dark: boolean) => {
  console.log('🎨 Applying theme:', dark ? 'DARK' : 'LIGHT')
  
  // Ensure we have a DOM
  if (typeof document === 'undefined') {
    console.warn('⚠️ Document not available, skipping theme application')
    return
  }
  
  // Apply to html element
  const html = document.documentElement
  
  // Force remove first, then add if needed - this prevents stuck classes
  html.classList.remove('dark')
  console.log('🧹 Forced removal of dark class')
  
  if (dark) {
    html.classList.add('dark')
    console.log('✅ Added dark class to html')
  } else {
    console.log('✅ Ensured dark class removed from html')
  }
  
  // Log current classes
  console.log('📝 HTML classes:', html.className)
  console.log('🔍 Has dark class:', html.classList.contains('dark'))
  
  // Check computed styles on body to verify dark mode is working
  const bodyStyles = window.getComputedStyle(document.body)
  console.log('🎨 Body background color:', bodyStyles.backgroundColor)
  console.log('🎨 Body text color:', bodyStyles.color)
  
  // Save to localStorage
  if (typeof localStorage !== 'undefined') {
    localStorage.setItem('theme', dark ? 'dark' : 'light')
    console.log('💾 Saved to localStorage:', dark ? 'dark' : 'light')
  }
}

const initializeTheme = () => {
  if (isInitialized) {
    console.log('⚠️ Theme already initialized, skipping')
    return
  }
  
  console.log('🚀 Initializing theme system...')
  
  // First, ensure we start clean
  if (typeof document !== 'undefined') {
    document.documentElement.classList.remove('dark')
    console.log('🧹 Cleaned up any existing dark class')
  }
  
  // Check localStorage
  let saved = null
  if (typeof localStorage !== 'undefined') {
    saved = localStorage.getItem('theme')
  }
  console.log('📦 Saved theme:', saved)
  
  let shouldBeDark = false
  
  if (saved) {
    shouldBeDark = saved === 'dark'
    console.log('💾 Using saved preference:', shouldBeDark ? 'DARK' : 'LIGHT')
  } else {
    // Check system preference
    if (typeof window !== 'undefined' && window.matchMedia) {
      shouldBeDark = window.matchMedia('(prefers-color-scheme: dark)').matches
      console.log('🖥️ System prefers dark:', shouldBeDark)
    } else {
      // Default to light mode if no preferences available
      shouldBeDark = false
      console.log('🌟 Defaulting to light mode')
    }
  }
  
  console.log('🎯 Setting initial theme to:', shouldBeDark ? 'DARK' : 'LIGHT')
  
  // Set the reactive state without triggering watchers yet
  isDark.value = shouldBeDark
  
  // Apply theme immediately during initialization
  applyTheme(shouldBeDark)
  
  isInitialized = true
  console.log('✅ Theme initialization complete')
}

const toggleTheme = () => {
  console.log('🔄 Toggle clicked! Current:', isDark.value ? 'DARK' : 'LIGHT')
  const newValue = !isDark.value
  isDark.value = newValue
  console.log('🔄 New state:', newValue ? 'DARK' : 'LIGHT')
  
  // Apply immediately to avoid delay
  applyTheme(newValue)
}

// Watch for changes (but not during initialization)
watch(isDark, (newValue) => {
  if (!isInitialized) {
    console.log('👀 Theme watcher triggered during init, skipping')
    return
  }
  console.log('👀 Theme watcher triggered after init:', newValue ? 'DARK' : 'LIGHT')
  applyTheme(newValue)
}, { immediate: false })

// Initialize when DOM is ready
if (typeof window !== 'undefined') {
  console.log('🌐 Browser detected, setting up initialization...')
  
  const initialize = () => {
    if (document.readyState === 'loading') {
      document.addEventListener('DOMContentLoaded', () => {
        console.log('📄 DOMContentLoaded event fired')
        initializeTheme()
      })
    } else {
      // DOM is already ready
      console.log('📄 DOM already ready')
      // Small delay to ensure everything is set up
      setTimeout(initializeTheme, 10)
    }
  }
  
  // Try to initialize immediately, but also set up fallback
  initialize()
  
  // Make debug function available globally for testing
  ;(window as any).debugTheme = () => {
    const { debugTheme } = useTheme()
    debugTheme()
  }
}

export function useTheme() {
  console.log('🔧 useTheme called, current state:', isDark.value ? 'DARK' : 'LIGHT')
  
  // Add a force reset function for debugging
  const forceReset = () => {
    console.log('🔄 Force resetting theme...')
    isInitialized = false
    isDark.value = false
    if (typeof document !== 'undefined') {
      document.documentElement.classList.remove('dark')
    }
    if (typeof localStorage !== 'undefined') {
      localStorage.setItem('theme', 'light')
    }
    isInitialized = true
    console.log('✅ Theme force reset complete')
  }
  
  const setTheme = (dark: boolean) => {
    console.log('🎯 Manually setting theme to:', dark ? 'DARK' : 'LIGHT')
    isDark.value = dark
    applyTheme(dark)
  }
  
  const debugTheme = () => {
    console.log('🔍 THEME DEBUG INFO:')
    console.log('isDark.value:', isDark.value)
    console.log('isInitialized:', isInitialized)
    
    if (typeof document !== 'undefined') {
      const html = document.documentElement
      console.log('HTML classList:', html.classList.toString())
      console.log('Has dark class:', html.classList.contains('dark'))
      
      const bodyStyles = window.getComputedStyle(document.body)
      console.log('Body background:', bodyStyles.backgroundColor)
      console.log('Body color:', bodyStyles.color)
    }
    
    if (typeof localStorage !== 'undefined') {
      console.log('localStorage theme:', localStorage.getItem('theme'))
    }
    
    if (typeof window !== 'undefined' && window.matchMedia) {
      console.log('System prefers dark:', window.matchMedia('(prefers-color-scheme: dark)').matches)
    }
  }
  
  return {
    isDark,
    toggleTheme,
    initializeTheme,
    forceReset,
    setTheme,
    debugTheme
  }
}