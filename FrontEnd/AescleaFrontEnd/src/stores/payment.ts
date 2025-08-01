import { defineStore } from 'pinia'
import { ref, computed } from 'vue'

export interface PaymentMethod {
  id: string
  type: 'card' | 'bank_account'
  last4: string
  brand?: string
  expiryMonth?: number
  expiryYear?: number
  isDefault: boolean
}

export interface Transaction {
  id: string
  transactionId: string
  amount: number
  currency: string
  status: 'pending' | 'succeeded' | 'failed' | 'canceled' | 'completed' | 'processing' | 'refunded'
  description: string
  type: 'subscription' | 'one_time' | 'refund'
  date: string
  createdAt: string
  patientId?: string
  patientName?: string
  paymentMethod: string
  invoiceId?: string
}

export interface Invoice {
  id: string
  subscriptionId?: string
  patientId: string
  patientName: string
  amount: number
  totalAmount: number
  currency: string
  status: 'draft' | 'open' | 'paid' | 'void' | 'uncollectible' | 'pending' | 'overdue' | 'cancelled'
  invoiceNumber: string
  dueDate: string
  paidAt?: string
  createdAt: string
  downloadUrl?: string
  services: Array<{
    id: string
    name: string
    department: string
    amount: number
    quantity: number
    description?: string
  }>
}

export interface PaymentIntent {
  id: string
  clientSecret: string
  amount: number
  currency: string
  status: string
}

export const usePaymentStore = defineStore('payment', () => {
  const paymentMethods = ref<PaymentMethod[]>([])
  const transactions = ref<Transaction[]>([])
  const invoices = ref<Invoice[]>([])
  const isLoading = ref(false)
  const error = ref<string | null>(null)

  const defaultPaymentMethod = computed(() => 
    paymentMethods.value.find(pm => pm.isDefault)
  )

  const recentTransactions = computed(() => 
    transactions.value
      .sort((a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime())
      .slice(0, 10)
  )

  const paidInvoices = computed(() => 
    invoices.value.filter(inv => inv.status === 'paid')
  )

  const fetchPaymentMethods = async () => {
    isLoading.value = true
    error.value = null
    
    try {
      const response = await fetch('/api/payment/methods', {
        headers: {
          'Authorization': `Bearer ${localStorage.getItem('auth_token')}`,
          'Content-Type': 'application/json'
        }
      })
      
      if (!response.ok) {
        throw new Error('Failed to fetch payment methods')
      }
      
      const data = await response.json()
      paymentMethods.value = data
    } catch (err: any) {
      error.value = err.message
      console.error('Error fetching payment methods:', err)
    } finally {
      isLoading.value = false
    }
  }

  const addPaymentMethod = async (paymentMethodData: {
    number: string
    expiryMonth: number
    expiryYear: number
    cvc: string
    name: string
    address?: any
  }) => {
    isLoading.value = true
    error.value = null
    
    try {
      // Simulate payment method creation with a test ID
      const paymentMethodId = `pm_${Math.random().toString(36).substr(2, 9)}`
      
      const newMethod: PaymentMethod = {
        id: paymentMethodId,
        type: 'card',
        last4: paymentMethodData.number.slice(-4),
        brand: detectCardBrand(paymentMethodData.number),
        expiryMonth: paymentMethodData.expiryMonth,
        expiryYear: paymentMethodData.expiryYear,
        isDefault: paymentMethods.value.length === 0
      }
      
      paymentMethods.value.push(newMethod)
      return paymentMethodId
    } catch (err: any) {
      error.value = err.message || 'Failed to add payment method'
      throw err
    } finally {
      isLoading.value = false
    }
  }

  const detectCardBrand = (number: string): string => {
    const cleanNumber = number.replace(/\s/g, '')
    
    if (/^4/.test(cleanNumber)) return 'visa'
    if (/^5[1-5]/.test(cleanNumber)) return 'mastercard'
    if (/^3[47]/.test(cleanNumber)) return 'amex'
    if (/^6/.test(cleanNumber)) return 'discover'
    
    return 'card'
  }

  const deletePaymentMethod = async (id: string) => {
    isLoading.value = true
    error.value = null
    
    try {
      const response = await fetch(`/api/payment/methods/${id}`, {
        method: 'DELETE',
        headers: {
          'Authorization': `Bearer ${localStorage.getItem('auth_token')}`
        }
      })
      
      if (!response.ok) {
        throw new Error('Failed to delete payment method')
      }
      
      paymentMethods.value = paymentMethods.value.filter(pm => pm.id !== id)
    } catch (err: any) {
      error.value = err.message
      throw err
    } finally {
      isLoading.value = false
    }
  }

  const setDefaultPaymentMethod = async (id: string) => {
    isLoading.value = true
    error.value = null
    
    try {
      const response = await fetch(`/api/payment/methods/${id}/default`, {
        method: 'PUT',
        headers: {
          'Authorization': `Bearer ${localStorage.getItem('auth_token')}`,
          'Content-Type': 'application/json'
        }
      })
      
      if (!response.ok) {
        throw new Error('Failed to set default payment method')
      }
      
      // Update local state
      paymentMethods.value.forEach(pm => {
        pm.isDefault = pm.id === id
      })
    } catch (err: any) {
      error.value = err.message
      throw err
    } finally {
      isLoading.value = false
    }
  }

  const fetchTransactions = async () => {
    isLoading.value = true
    error.value = null
    
    try {
      const response = await fetch('/api/payment/transactions', {
        headers: {
          'Authorization': `Bearer ${localStorage.getItem('auth_token')}`,
          'Content-Type': 'application/json'
        }
      })
      
      if (!response.ok) {
        throw new Error('Failed to fetch transactions')
      }
      
      const data = await response.json()
      transactions.value = data
    } catch (err: any) {
      error.value = err.message
      console.error('Error fetching transactions:', err)
    } finally {
      isLoading.value = false
    }
  }

  const fetchInvoices = async () => {
    isLoading.value = true
    error.value = null
    
    try {
      const response = await fetch('/api/payment/invoices', {
        headers: {
          'Authorization': `Bearer ${localStorage.getItem('auth_token')}`,
          'Content-Type': 'application/json'
        }
      })
      
      if (!response.ok) {
        throw new Error('Failed to fetch invoices')
      }
      
      const data = await response.json()
      invoices.value = data
    } catch (err: any) {
      error.value = err.message
      console.error('Error fetching invoices:', err)
    } finally {
      isLoading.value = false
    }
  }

  const createPaymentIntent = async (amount: number, currency = 'usd') => {
    isLoading.value = true
    error.value = null
    
    try {
      const response = await fetch('/api/payment/create-payment-intent', {
        method: 'POST',
        headers: {
          'Authorization': `Bearer ${localStorage.getItem('auth_token')}`,
          'Content-Type': 'application/json'
        },
        body: JSON.stringify({ amount, currency })
      })
      
      if (!response.ok) {
        throw new Error('Failed to create payment intent')
      }
      
      return await response.json()
    } catch (err: any) {
      error.value = err.message
      throw err
    } finally {
      isLoading.value = false
    }
  }

  const confirmPayment = async (paymentIntentId: string, paymentMethodId: string) => {
    isLoading.value = true
    error.value = null
    
    try {
      const response = await fetch('/api/payment/confirm', {
        method: 'POST',
        headers: {
          'Authorization': `Bearer ${localStorage.getItem('auth_token')}`,
          'Content-Type': 'application/json'
        },
        body: JSON.stringify({ 
          paymentIntentId, 
          paymentMethodId 
        })
      })
      
      if (!response.ok) {
        throw new Error('Failed to confirm payment')
      }
      
      return await response.json()
    } catch (err: any) {
      error.value = err.message
      throw err
    } finally {
      isLoading.value = false
    }
  }

  const processRefund = async (transactionId: string, amount?: number) => {
    isLoading.value = true
    error.value = null
    
    try {
      const response = await fetch('/api/payment/refund', {
        method: 'POST',
        headers: {
          'Authorization': `Bearer ${localStorage.getItem('auth_token')}`,
          'Content-Type': 'application/json'
        },
        body: JSON.stringify({ transactionId, amount })
      })
      
      if (!response.ok) {
        throw new Error('Failed to process refund')
      }
      
      const refund = await response.json()
      
      // Add refund transaction to local state
      const refundTransaction: Transaction = {
        id: refund.id,
        transactionId: refund.id,
        amount: refund.amount,
        currency: refund.currency,
        status: 'succeeded',
        description: `Refund for transaction ${transactionId}`,
        type: 'refund',
        date: new Date().toISOString(),
        createdAt: new Date().toISOString(),
        paymentMethod: 'Refund'
      }
      
      transactions.value.unshift(refundTransaction)
      return refund
    } catch (err: any) {
      error.value = err.message
      throw err
    } finally {
      isLoading.value = false
    }
  }

  const downloadInvoice = async (invoiceId: string) => {
    try {
      const response = await fetch(`/api/payment/invoices/${invoiceId}/download`, {
        headers: {
          'Authorization': `Bearer ${localStorage.getItem('auth_token')}`
        }
      })
      
      if (!response.ok) {
        throw new Error('Failed to download invoice')
      }
      
      const blob = await response.blob()
      const url = window.URL.createObjectURL(blob)
      const a = document.createElement('a')
      a.href = url
      a.download = `invoice-${invoiceId}.pdf`
      document.body.appendChild(a)
      a.click()
      window.URL.revokeObjectURL(url)
      document.body.removeChild(a)
    } catch (err) {
      console.error('Error downloading invoice:', err)
      throw err
    }
  }

  const createInvoice = async (invoiceData: Omit<Invoice, 'id'>) => {
    try {
      isLoading.value = true
      error.value = null

      const response = await fetch('/api/invoices', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${localStorage.getItem('token')}`
        },
        body: JSON.stringify(invoiceData)
      })

      if (!response.ok) {
        throw new Error('Failed to create invoice')
      }

      const newInvoice = await response.json()
      invoices.value.unshift(newInvoice)
      return newInvoice
    } catch (err: any) {
      error.value = err.message
      throw err
    } finally {
      isLoading.value = false
    }
  }

  const initializeSampleData = () => {
    // Add sample payment methods
    if (paymentMethods.value.length === 0) {
      paymentMethods.value = [
        {
          id: 'pm_sample_1',
          type: 'card',
          last4: '4242',
          brand: 'visa',
          expiryMonth: 12,
          expiryYear: 2027,
          isDefault: true
        },
        {
          id: 'pm_sample_2',
          type: 'card',
          last4: '0005',
          brand: 'mastercard',
          expiryMonth: 8,
          expiryYear: 2026,
          isDefault: false
        }
      ]
    }

    // Add sample transactions
    if (transactions.value.length === 0) {
      transactions.value = [
        {
          id: 'txn_1',
          transactionId: 'TXN-2025-001',
          amount: 149.00,
          currency: 'USD',
          status: 'succeeded',
          description: 'Monthly subscription - Professional Plan',
          type: 'subscription',
          date: '2025-01-15',
          createdAt: '2025-01-15T10:30:00Z',
          paymentMethod: 'Visa ****4242'
        },
        {
          id: 'txn_2',
          transactionId: 'TXN-2025-002',
          amount: 75.00,
          currency: 'USD',
          status: 'succeeded',
          description: 'Medical consultation - Dr. Smith',
          type: 'one_time',
          date: '2025-01-20',
          createdAt: '2025-01-20T14:15:00Z',
          patientId: 'patient_1',
          patientName: 'John Doe',
          paymentMethod: 'Mastercard ****0005'
        },
        {
          id: 'txn_3',
          transactionId: 'TXN-2025-003',
          amount: 125.00,
          currency: 'USD',
          status: 'succeeded',
          description: 'Laboratory tests',
          type: 'one_time',
          date: '2025-01-25',
          createdAt: '2025-01-25T09:45:00Z',
          patientId: 'patient_2',
          patientName: 'Jane Smith',
          paymentMethod: 'Visa ****4242'
        }
      ]
    }

    // Add sample invoices
    if (invoices.value.length === 0) {
      invoices.value = [
        {
          id: 'inv_1',
          patientId: 'patient_1',
          patientName: 'John Doe',
          amount: 150.00,
          totalAmount: 150.00,
          currency: 'USD',
          status: 'paid',
          invoiceNumber: 'INV-2025-001',
          dueDate: '2025-02-01',
          paidAt: '2025-01-20T14:15:00Z',
          createdAt: '2025-01-15T10:00:00Z',
          services: [
            {
              id: 'svc_1',
              name: 'General Consultation',
              department: 'General Medicine',
              amount: 75.00,
              quantity: 1,
              description: 'Standard consultation with Dr. Smith'
            },
            {
              id: 'svc_2',
              name: 'Blood Test',
              department: 'Laboratory',
              amount: 75.00,
              quantity: 1,
              description: 'Complete blood count analysis'
            }
          ]
        },
        {
          id: 'inv_2',
          patientId: 'patient_2',
          patientName: 'Jane Smith',
          amount: 275.00,
          totalAmount: 275.00,
          currency: 'USD',
          status: 'open',
          invoiceNumber: 'INV-2025-002',
          dueDate: '2025-02-15',
          createdAt: '2025-01-28T11:30:00Z',
          services: [
            {
              id: 'svc_3',
              name: 'Specialist Consultation',
              department: 'Cardiology',
              amount: 150.00,
              quantity: 1,
              description: 'Cardiology consultation with Dr. Johnson'
            },
            {
              id: 'svc_4',
              name: 'ECG Test',
              department: 'Cardiology',
              amount: 125.00,
              quantity: 1,
              description: 'Electrocardiogram analysis'
            }
          ]
        },
        {
          id: 'inv_3',
          patientId: 'patient_3',
          patientName: 'Robert Wilson',
          amount: 95.00,
          totalAmount: 95.00,
          currency: 'USD',
          status: 'overdue',
          invoiceNumber: 'INV-2025-003',
          dueDate: '2025-01-30',
          createdAt: '2025-01-10T09:15:00Z',
          services: [
            {
              id: 'svc_5',
              name: 'Follow-up Consultation',
              department: 'General Medicine',
              amount: 60.00,
              quantity: 1
            },
            {
              id: 'svc_6',
              name: 'X-Ray',
              department: 'Radiology',
              amount: 35.00,
              quantity: 1
            }
          ]
        }
      ]
    }
  }

  return {
    paymentMethods,
    transactions,
    invoices,
    isLoading,
    error,
    defaultPaymentMethod,
    recentTransactions,
    paidInvoices,
    fetchPaymentMethods,
    addPaymentMethod,
    deletePaymentMethod,
    setDefaultPaymentMethod,
    fetchTransactions,
    fetchInvoices,
    createPaymentIntent,
    confirmPayment,
    processRefund,
    downloadInvoice,
    createInvoice,
    initializeSampleData
  }
})
