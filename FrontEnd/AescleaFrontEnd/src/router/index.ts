import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '../stores/auth'
import { useSubscriptionStore } from '../stores/subscription'
import Welcome from '../views/Welcome.vue'
import Login from '../views/Login.vue'
import Register from '../views/Register.vue'
import EmailVerification from '../views/EmailVerification.vue'
import EmailVerificationSuccess from '../views/EmailVerificationSuccess.vue'
import SubscriptionSelection from '../views/SubscriptionSelection.vue'
import Home from '../views/Home.vue'
import Features from '../views/Features.vue'
import Pricing from '../views/Pricing.vue'
import Security from '../views/Security.vue'
import Updates from '../views/Updates.vue'
import Help from '../views/Help.vue'
import Contact from '../views/Contact.vue'
import Privacy from '../views/Privacy.vue'
import Terms from '../views/Terms.vue'

const routes = [
  {
    path: '/',
    name: 'Welcome',
    component: Welcome,
    meta: { requiresGuest: true }
  },
  {
    path: '/login',
    name: 'Login',
    component: Login,
    meta: { requiresGuest: true }
  },
  {
    path: '/register',
    name: 'Register',
    component: Register,
    meta: { requiresGuest: true }
  },
  {
    path: '/email-verification',
    name: 'EmailVerification',
    component: EmailVerification,
    meta: { requiresAuth: true }
  },
  {
    path: '/verify-email',
    name: 'EmailVerificationSuccess',
    component: EmailVerificationSuccess
  },
  {
    path: '/subscription-selection',
    name: 'SubscriptionSelection',
    component: SubscriptionSelection,
    meta: { requiresAuth: true, requiresEmailVerified: true }
  },
  {
    path: '/dashboard',
    name: 'Dashboard',
    component: Home,
    meta: { requiresAuth: true }
  },
  {
    path: '/home',
    redirect: '/dashboard'
  },
  {
    path: '/features',
    name: 'Features',
    component: Features
  },
  {
    path: '/pricing',
    name: 'Pricing',
    component: Pricing
  },
  {
    path: '/security',
    name: 'Security',
    component: Security
  },
  {
    path: '/updates',
    name: 'Updates',
    component: Updates
  },
  {
    path: '/help',
    name: 'Help',
    component: Help
  },
  {
    path: '/contact',
    name: 'Contact',
    component: Contact
  },
  {
    path: '/privacy',
    name: 'Privacy',
    component: Privacy
  },
  {
    path: '/terms',
    name: 'Terms',
    component: Terms
  }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

// Navigation guards
router.beforeEach(async (to, _from, next) => {
  // Initialize auth store - this ensures Pinia is ready
  const authStore = useAuthStore()
  const subscriptionStore = useSubscriptionStore()
  
  // Initialize auth state from localStorage
  authStore.initializeAuth()

  const requiresAuth = to.matched.some(record => record.meta.requiresAuth)
  const requiresGuest = to.matched.some(record => record.meta.requiresGuest)
  const requiresEmailVerified = to.matched.some(record => record.meta.requiresEmailVerified)

  // Check authentication
  if (requiresAuth && !authStore.isAuthenticated) {
    next('/login')
    return
  }
  
  if (requiresGuest && authStore.isAuthenticated) {
    next('/dashboard')
    return
  }

  // Check email verification for authenticated users
  if (authStore.isAuthenticated && authStore.user && !authStore.user.emailVerified && to.path !== '/email-verification') {
    next(`/email-verification?email=${encodeURIComponent(authStore.user.email)}`)
    return
  }

  // Check subscription for email verified users accessing dashboard
  if (authStore.isAuthenticated && authStore.user?.emailVerified && to.path === '/dashboard') {
    await subscriptionStore.loadUserSubscription()
    if (!subscriptionStore.hasActiveSubscription) {
      next('/subscription-selection')
      return
    }
  }

  // Check email verification requirement
  if (requiresEmailVerified && authStore.user && !authStore.user.emailVerified) {
    next(`/email-verification?email=${encodeURIComponent(authStore.user.email)}`)
    return
  }

  next()
})

export default router