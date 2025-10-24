// Copyright (c) 2025 Alexander Sinapov | Simeon Petkov

// All rights reserved.
// This code is proprietary and confidential.  
// Unauthorized copying, modification, distribution, or use is strictly prohibited.

import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '../stores/auth'
import { useSubscriptionStore } from '../stores/subscription'
import Welcome from '../views/Welcome.vue'
import Login from '../views/Login.vue'
import Register from '../views/Register.vue'
import EmailVerification from '../views/EmailVerification.vue'
import EmailVerificationSuccess from '../views/EmailVerificationSuccess.vue'
import SubscriptionSelection from '../views/SubscriptionSelection.vue'
import Dashboard from '../views/Dashboard.vue'
import Features from '../views/Features.vue'
import Pricing from '../views/Pricing.vue'
import Security from '../views/Security.vue'
import Updates from '../views/Updates.vue'
import Help from '../views/Help.vue'
import Contact from '../views/Contact.vue'
import Privacy from '../views/Privacy.vue'
import Terms from '../views/Terms.vue'
import Support from '../views/Support.vue'
import SupportAgent from '../views/dashboard/SupportAgent.vue'
import Products from '../views/Products.vue'

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
    meta: { requiresAuth: true, requiresEmailVerified: true, requiresNoSubscription: true }
  },
  {
    path: '/dashboard',
    name: 'Dashboard',
    component: Dashboard,
    meta: { requiresAuth: true, requiresEmailVerified: true }
  },
  {
    path: '/support',
    name: 'Support',
    component: Support,
    meta: { requiresAuth: true, requiresEmailVerified: true }
  },
  {
    path: '/dashboard/support-agent',
    name: 'SupportAgent',
    component: SupportAgent,
    meta: { requiresAuth: true, requiresEmailVerified: true, requiresSupportRole: true }
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
    path: '/products',
    name: 'Products',
    component: Products
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
  const requiresSubscription = to.matched.some(record => record.meta.requiresSubscription)
  const requiresNoSubscription = to.matched.some(record => record.meta.requiresNoSubscription)
  const requiresSupportRole = to.matched.some(record => record.meta.requiresSupportRole)

  // Check authentication
  if (requiresAuth && !authStore.isAuthenticated) {
    next('/login')
    return
  }
  
  if (requiresGuest && authStore.isAuthenticated) {
    // If user is authenticated, check if they need email verification
    if (authStore.user && !authStore.user.isEmailVerified) {
      next(`/email-verification?email=${encodeURIComponent(authStore.user.email)}`)
      return
    }
    
    // If email is verified, check subscription and route accordingly
    try {
      await subscriptionStore.loadUserSubscription()
      if (!subscriptionStore.hasActiveSubscription) {
        next('/subscription-selection')
        return
      }
    } catch (error) {
      // If subscription loading fails, still allow access to dashboard
      console.warn('Failed to load subscription, proceeding to dashboard:', error)
    }
    
    next('/dashboard')
    return
  }

  // Check email verification for authenticated users
  if (authStore.isAuthenticated && authStore.user && !authStore.user.isEmailVerified && to.path !== '/email-verification' && to.path !== '/verify-email') {
    next(`/email-verification?email=${encodeURIComponent(authStore.user.email)}`)
    return
  }

  // Check email verification requirement
  if (requiresEmailVerified && authStore.user && !authStore.user.isEmailVerified) {
    next(`/email-verification?email=${encodeURIComponent(authStore.user.email)}`)
    return
  }

  // Check support role requirement
  if (requiresSupportRole && authStore.user) {
    const userRole = authStore.user.role?.toLowerCase()
    if (userRole !== 'supportagent' && userRole !== 'admin') {
      next('/dashboard') // Redirect to regular dashboard if not support agent
      return
    }
  }

  // Check subscription requirement
  if (requiresSubscription && authStore.isAuthenticated && authStore.user?.isEmailVerified) {
    try {
      await subscriptionStore.loadUserSubscription()
      if (!subscriptionStore.hasActiveSubscription) {
        next('/subscription-selection')
        return
      }
    } catch (error) {
      // If subscription loading fails, log but don't block access
      console.warn('Failed to load subscription for required check:', error)
    }
  }

  // Check no subscription requirement (for subscription selection page)
  if (requiresNoSubscription && authStore.isAuthenticated && authStore.user?.isEmailVerified) {
    try {
      await subscriptionStore.loadUserSubscription()
      if (subscriptionStore.hasActiveSubscription) {
        next('/dashboard')
        return
      }
    } catch (error) {
      // If subscription loading fails, allow access to subscription selection
      console.warn('Failed to load subscription for no-subscription check:', error)
    }
  }

  next()
})

export default router