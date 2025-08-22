<template>
  <div class="min-h-screen transition-colors duration-300">
    <div class="min-h-screen bg-gradient-to-br from-purple-50 via-blue-50 to-indigo-100 dark:from-gray-900 dark:via-gray-800 dark:to-gray-900">
      <!-- Header -->
      <AppNavbar />

      <!-- Hero Section -->
      <section class="relative overflow-hidden">
        <div class="px-4 py-16 mx-auto max-w-7xl sm:px-6 lg:px-8">
          <div class="text-center">
            <div class="flex items-center justify-center w-20 h-20 mx-auto mb-8 rounded-full bg-gradient-to-r from-green-500 to-emerald-600">
              <svg class="w-10 h-10 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m5.618-4.016A11.955 11.955 0 0112 2.944a11.955 11.955 0 01-8.618 3.04A12.02 12.02 0 003 9c0 5.591 3.824 10.29 9 11.622 5.176-1.332 9-6.03 9-11.622 0-1.042-.133-2.052-.382-3.016z" />
              </svg>
            </div>
            <h1 class="mb-6 text-4xl font-extrabold text-gray-900 sm:text-5xl dark:text-white">
              <span class="block">Enterprise-Grade</span>
              <span class="block text-transparent bg-clip-text bg-gradient-to-r from-green-600 to-emerald-600">
                Security & Compliance
              </span>
            </h1>
            <p class="max-w-3xl mx-auto mb-10 text-xl text-gray-600 dark:text-gray-300">
              Your patients' data is protected with military-grade security, HIPAA compliance, 
              and industry-leading privacy standards.
            </p>
          </div>
        </div>
      </section>

      <!-- Security Features Grid -->
      <section class="py-20 bg-white dark:bg-gray-800">
        <div class="px-4 mx-auto max-w-7xl sm:px-6 lg:px-8">
          <div class="grid grid-cols-1 gap-8 md:grid-cols-2 lg:grid-cols-3">
            
            <!-- HIPAA Compliance -->
            <div class="p-8 transition-all duration-300 bg-gradient-to-br from-blue-50 to-indigo-50 dark:from-gray-700 dark:to-gray-600 rounded-2xl hover:shadow-xl">
              <div class="flex items-center justify-center w-16 h-16 mb-6 bg-blue-100 dark:bg-blue-900 rounded-xl">
                <svg class="w-8 h-8 text-blue-600 dark:text-blue-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4M7.835 4.697a3.42 3.42 0 001.946-.806 3.42 3.42 0 014.438 0 3.42 3.42 0 001.946.806 3.42 3.42 0 013.138 3.138 3.42 3.42 0 00.806 1.946 3.42 3.42 0 010 4.438 3.42 3.42 0 00-.806 1.946 3.42 3.42 0 01-3.138 3.138 3.42 3.42 0 00-1.946.806 3.42 3.42 0 01-4.438 0 3.42 3.42 0 00-1.946-.806 3.42 3.42 0 01-3.138-3.138 3.42 3.42 0 00-.806-1.946 3.42 3.42 0 010-4.438 3.42 3.42 0 00.806-1.946 3.42 3.42 0 013.138-3.138z" />
                </svg>
              </div>
              <h3 class="mb-4 text-2xl font-bold text-gray-900 dark:text-white">HIPAA Compliant</h3>
              <p class="mb-6 text-gray-600 dark:text-gray-300">Fully compliant with HIPAA regulations, ensuring patient data privacy and security standards are met.</p>
              <ul class="space-y-2 text-sm text-gray-600 dark:text-gray-300">
                <li class="flex items-center">
                  <svg class="w-4 h-4 mr-2 text-green-500" fill="currentColor" viewBox="0 0 20 20">
                    <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd" />
                  </svg>
                  Business Associate Agreements
                </li>
                <li class="flex items-center">
                  <svg class="w-4 h-4 mr-2 text-green-500" fill="currentColor" viewBox="0 0 20 20">
                    <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd" />
                  </svg>
                  PHI Protection Protocols
                </li>
                <li class="flex items-center">
                  <svg class="w-4 h-4 mr-2 text-green-500" fill="currentColor" viewBox="0 0 20 20">
                    <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd" />
                  </svg>
                  Audit Trail Management
                </li>
              </ul>
            </div>

            <!-- Data Encryption -->
            <div class="p-8 transition-all duration-300 bg-gradient-to-br from-green-50 to-emerald-50 dark:from-gray-700 dark:to-gray-600 rounded-2xl hover:shadow-xl">
              <div class="flex items-center justify-center w-16 h-16 mb-6 bg-green-100 dark:bg-green-900 rounded-xl">
                <svg class="w-8 h-8 text-green-600 dark:text-green-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 15v2m-6 4h12a2 2 0 002-2v-6a2 2 0 00-2-2H6a2 2 0 00-2 2v6a2 2 0 002 2zm10-10V7a4 4 0 00-8 0v4h8z" />
                </svg>
              </div>
              <h3 class="mb-4 text-2xl font-bold text-gray-900 dark:text-white">256-bit AES Encryption</h3>
              <p class="mb-6 text-gray-600 dark:text-gray-300">Military-grade encryption protects all data in transit and at rest with advanced cryptographic protocols.</p>
              <ul class="space-y-2 text-sm text-gray-600 dark:text-gray-300">
                <li class="flex items-center">
                  <svg class="w-4 h-4 mr-2 text-green-500" fill="currentColor" viewBox="0 0 20 20">
                    <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd" />
                  </svg>
                  End-to-end encryption
                </li>
                <li class="flex items-center">
                  <svg class="w-4 h-4 mr-2 text-green-500" fill="currentColor" viewBox="0 0 20 20">
                    <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd" />
                  </svg>
                  TLS 1.3 secure transmission
                </li>
                <li class="flex items-center">
                  <svg class="w-4 h-4 mr-2 text-green-500" fill="currentColor" viewBox="0 0 20 20">
                    <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd" />
                  </svg>
                  Advanced key management
                </li>
              </ul>
            </div>

            <!-- Access Control -->
            <div class="p-8 transition-all duration-300 bg-gradient-to-br from-purple-50 to-pink-50 dark:from-gray-700 dark:to-gray-600 rounded-2xl hover:shadow-xl">
              <div class="flex items-center justify-center w-16 h-16 mb-6 bg-purple-100 dark:bg-purple-900 rounded-xl">
                <svg class="w-8 h-8 text-purple-600 dark:text-purple-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 7a2 2 0 012 2m4 0a6 6 0 01-7.743 5.743L11 17H9v2H7v2H4a1 1 0 01-1-1v-2.586a1 1 0 01.293-.707l5.964-5.964A6 6 0 1121 9z" />
                </svg>
              </div>
              <h3 class="mb-4 text-2xl font-bold text-gray-900 dark:text-white">Multi-Factor Authentication</h3>
              <p class="mb-6 text-gray-600 dark:text-gray-300">Advanced access controls with role-based permissions and multi-factor authentication for enhanced security.</p>
              <ul class="space-y-2 text-sm text-gray-600 dark:text-gray-300">
                <li class="flex items-center">
                  <svg class="w-4 h-4 mr-2 text-green-500" fill="currentColor" viewBox="0 0 20 20">
                    <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd" />
                  </svg>
                  TOTP & SMS authentication
                </li>
                <li class="flex items-center">
                  <svg class="w-4 h-4 mr-2 text-green-500" fill="currentColor" viewBox="0 0 20 20">
                    <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd" />
                  </svg>
                  Role-based access control
                </li>
                <li class="flex items-center">
                  <svg class="w-4 h-4 mr-2 text-green-500" fill="currentColor" viewBox="0 0 20 20">
                    <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd" />
                  </svg>
                  Session management
                </li>
              </ul>
            </div>

            <!-- Infrastructure Security -->
            <div class="p-8 transition-all duration-300 bg-gradient-to-br from-red-50 to-orange-50 dark:from-gray-700 dark:to-gray-600 rounded-2xl hover:shadow-xl">
              <div class="flex items-center justify-center w-16 h-16 mb-6 bg-red-100 dark:bg-red-900 rounded-xl">
                <svg class="w-8 h-8 text-red-600 dark:text-red-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 15a4 4 0 004 4h9a5 5 0 10-.1-9.999 5.002 5.002 0 10-9.78 2.096A4.001 4.001 0 003 15z" />
                </svg>
              </div>
              <h3 class="mb-4 text-2xl font-bold text-gray-900 dark:text-white">Secure Cloud Infrastructure</h3>
              <p class="mb-6 text-gray-600 dark:text-gray-300">Built on enterprise-grade cloud infrastructure with 99.9% uptime and distributed security controls.</p>
              <ul class="space-y-2 text-sm text-gray-600 dark:text-gray-300">
                <li class="flex items-center">
                  <svg class="w-4 h-4 mr-2 text-green-500" fill="currentColor" viewBox="0 0 20 20">
                    <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd" />
                  </svg>
                  Multi-region redundancy
                </li>
                <li class="flex items-center">
                  <svg class="w-4 h-4 mr-2 text-green-500" fill="currentColor" viewBox="0 0 20 20">
                    <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd" />
                  </svg>
                  DDoS protection
                </li>
                <li class="flex items-center">
                  <svg class="w-4 h-4 mr-2 text-green-500" fill="currentColor" viewBox="0 0 20 20">
                    <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd" />
                  </svg>
                  24/7 monitoring
                </li>
              </ul>
            </div>

            <!-- Data Backup -->
            <div class="p-8 transition-all duration-300 bg-gradient-to-br from-yellow-50 to-amber-50 dark:from-gray-700 dark:to-gray-600 rounded-2xl hover:shadow-xl">
              <div class="flex items-center justify-center w-16 h-16 mb-6 bg-yellow-100 dark:bg-yellow-900 rounded-xl">
                <svg class="w-8 h-8 text-yellow-600 dark:text-yellow-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 7H5a2 2 0 00-2 2v9a2 2 0 002 2h14a2 2 0 002-2V9a2 2 0 00-2-2h-3m-1 4l-3 3m0 0l-3-3m3 3V4" />
                </svg>
              </div>
              <h3 class="mb-4 text-2xl font-bold text-gray-900 dark:text-white">Automated Backups</h3>
              <p class="mb-6 text-gray-600 dark:text-gray-300">Regular automated backups with point-in-time recovery and geographically distributed storage.</p>
              <ul class="space-y-2 text-sm text-gray-600 dark:text-gray-300">
                <li class="flex items-center">
                  <svg class="w-4 h-4 mr-2 text-green-500" fill="currentColor" viewBox="0 0 20 20">
                    <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd" />
                  </svg>
                  Daily automated backups
                </li>
                <li class="flex items-center">
                  <svg class="w-4 h-4 mr-2 text-green-500" fill="currentColor" viewBox="0 0 20 20">
                    <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd" />
                  </svg>
                  Point-in-time recovery
                </li>
                <li class="flex items-center">
                  <svg class="w-4 h-4 mr-2 text-green-500" fill="currentColor" viewBox="0 0 20 20">
                    <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd" />
                  </svg>
                  Encrypted backup storage
                </li>
              </ul>
            </div>

            <!-- Compliance Monitoring -->
            <div class="p-8 transition-all duration-300 bg-gradient-to-br from-indigo-50 to-blue-50 dark:from-gray-700 dark:to-gray-600 rounded-2xl hover:shadow-xl">
              <div class="flex items-center justify-center w-16 h-16 mb-6 bg-indigo-100 dark:bg-indigo-900 rounded-xl">
                <svg class="w-8 h-8 text-indigo-600 dark:text-indigo-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z" />
                </svg>
              </div>
              <h3 class="mb-4 text-2xl font-bold text-gray-900 dark:text-white">Compliance Monitoring</h3>
              <p class="mb-6 text-gray-600 dark:text-gray-300">Continuous monitoring and reporting to ensure ongoing compliance with healthcare regulations.</p>
              <ul class="space-y-2 text-sm text-gray-600 dark:text-gray-300">
                <li class="flex items-center">
                  <svg class="w-4 h-4 mr-2 text-green-500" fill="currentColor" viewBox="0 0 20 20">
                    <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd" />
                  </svg>
                  Real-time security monitoring
                </li>
                <li class="flex items-center">
                  <svg class="w-4 h-4 mr-2 text-green-500" fill="currentColor" viewBox="0 0 20 20">
                    <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd" />
                  </svg>
                  Compliance reporting
                </li>
                <li class="flex items-center">
                  <svg class="w-4 h-4 mr-2 text-green-500" fill="currentColor" viewBox="0 0 20 20">
                    <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd" />
                  </svg>
                  Incident response protocols
                </li>
              </ul>
            </div>

          </div>
        </div>
      </section>

      <!-- Certifications Section -->
      <section class="py-20 bg-gradient-to-r from-green-600 to-emerald-600">
        <div class="px-4 mx-auto max-w-7xl sm:px-6 lg:px-8">
          <div class="mb-16 text-center">
            <h2 class="mb-4 text-3xl font-extrabold text-white">
              Industry Certifications & Standards
            </h2>
            <p class="max-w-3xl mx-auto text-xl text-green-100">
              Our security framework meets or exceeds industry standards and regulatory requirements
            </p>
          </div>
          
          <div class="grid grid-cols-1 gap-8 md:grid-cols-2 lg:grid-cols-4">
            <div class="p-6 text-center bg-white bg-opacity-10 backdrop-blur-sm rounded-xl">
              <div class="flex items-center justify-center w-16 h-16 mx-auto mb-4 bg-white rounded-lg bg-opacity-20">
                <svg class="w-8 h-8 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m5.618-4.016A11.955 11.955 0 0112 2.944a11.955 11.955 0 01-8.618 3.04A12.02 12.02 0 003 9c0 5.591 3.824 10.29 9 11.622 5.176-1.332 9-6.03 9-11.622 0-1.042-.133-2.052-.382-3.016z" />
                </svg>
              </div>
              <h3 class="mb-2 text-xl font-bold text-white">SOC 2 Type II</h3>
              <p class="text-sm text-green-100">Independently audited security controls and processes</p>
            </div>
            
            <div class="p-6 text-center bg-white bg-opacity-10 backdrop-blur-sm rounded-xl">
              <div class="flex items-center justify-center w-16 h-16 mx-auto mb-4 bg-white rounded-lg bg-opacity-20">
                <svg class="w-8 h-8 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 15v2m-6 4h12a2 2 0 002-2v-6a2 2 0 00-2-2H6a2 2 0 00-2 2v6a2 2 0 002 2zm10-10V7a4 4 0 00-8 0v4h8z" />
                </svg>
              </div>
              <h3 class="mb-2 text-xl font-bold text-white">HIPAA Compliant</h3>
              <p class="text-sm text-green-100">Full compliance with healthcare privacy regulations</p>
            </div>
            
            <div class="p-6 text-center bg-white bg-opacity-10 backdrop-blur-sm rounded-xl">
              <div class="flex items-center justify-center w-16 h-16 mx-auto mb-4 bg-white rounded-lg bg-opacity-20">
                <svg class="w-8 h-8 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3.055 11H5a2 2 0 012 2v1a2 2 0 002 2 2 2 0 012 2v2.945M8 3.935V5.5A2.5 2.5 0 0010.5 8h.5a2 2 0 012 2 2 2 0 104 0 2 2 0 012-2h1.064M15 20.488V18a2 2 0 012-2h3.064M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
                </svg>
              </div>
              <h3 class="mb-2 text-xl font-bold text-white">ISO 27001</h3>
              <p class="text-sm text-green-100">International standard for information security management</p>
            </div>
            
            <div class="p-6 text-center bg-white bg-opacity-10 backdrop-blur-sm rounded-xl">
              <div class="flex items-center justify-center w-16 h-16 mx-auto mb-4 bg-white rounded-lg bg-opacity-20">
                <svg class="w-8 h-8 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z" />
                </svg>
              </div>
              <h3 class="mb-2 text-xl font-bold text-white">GDPR Ready</h3>
              <p class="text-sm text-green-100">European data protection regulation compliance</p>
            </div>
          </div>
        </div>
      </section>

      <!-- Security Process Section -->
      <section class="py-20 bg-white dark:bg-gray-800">
        <div class="px-4 mx-auto max-w-7xl sm:px-6 lg:px-8">
          <div class="mb-16 text-center">
            <h2 class="mb-4 text-3xl font-extrabold text-gray-900 dark:text-white">
              Our Security Process
            </h2>
            <p class="max-w-3xl mx-auto text-xl text-gray-600 dark:text-gray-300">
              A comprehensive approach to protecting your data at every level
            </p>
          </div>
          
          <div class="grid grid-cols-1 gap-8 md:grid-cols-2 lg:grid-cols-4">
            <div class="text-center">
              <div class="flex items-center justify-center w-16 h-16 mx-auto mb-6 rounded-full bg-gradient-to-r from-blue-500 to-indigo-600">
                <span class="text-2xl font-bold text-white">1</span>
              </div>
              <h3 class="mb-4 text-xl font-semibold text-gray-900 dark:text-white">Risk Assessment</h3>
              <p class="text-gray-600 dark:text-gray-300">Comprehensive analysis of potential security vulnerabilities and threats to your data.</p>
            </div>
            
            <div class="text-center">
              <div class="flex items-center justify-center w-16 h-16 mx-auto mb-6 rounded-full bg-gradient-to-r from-green-500 to-emerald-600">
                <span class="text-2xl font-bold text-white">2</span>
              </div>
              <h3 class="mb-4 text-xl font-semibold text-gray-900 dark:text-white">Implementation</h3>
              <p class="text-gray-600 dark:text-gray-300">Deployment of multi-layered security controls and encryption protocols.</p>
            </div>
            
            <div class="text-center">
              <div class="flex items-center justify-center w-16 h-16 mx-auto mb-6 rounded-full bg-gradient-to-r from-purple-500 to-pink-600">
                <span class="text-2xl font-bold text-white">3</span>
              </div>
              <h3 class="mb-4 text-xl font-semibold text-gray-900 dark:text-white">Monitoring</h3>
              <p class="text-gray-600 dark:text-gray-300">24/7 continuous monitoring and real-time threat detection and response.</p>
            </div>
            
            <div class="text-center">
              <div class="flex items-center justify-center w-16 h-16 mx-auto mb-6 rounded-full bg-gradient-to-r from-orange-500 to-red-600">
                <span class="text-2xl font-bold text-white">4</span>
              </div>
              <h3 class="mb-4 text-xl font-semibold text-gray-900 dark:text-white">Improvement</h3>
              <p class="text-gray-600 dark:text-gray-300">Regular security audits and updates to maintain the highest protection standards.</p>
            </div>
          </div>
        </div>
      </section>

      <!-- Trust Indicators -->
      <section class="py-20 bg-gray-50 dark:bg-gray-900">
        <div class="max-w-4xl px-4 mx-auto text-center sm:px-6 lg:px-8">
          <h2 class="mb-8 text-3xl font-extrabold text-gray-900 dark:text-white">
            Security You Can Trust
          </h2>
          
          <div class="grid grid-cols-1 gap-8 mb-12 md:grid-cols-3">
            <div>
              <div class="mb-2 text-4xl font-bold text-green-600">99.9%</div>
              <p class="text-gray-600 dark:text-gray-300">Uptime SLA with redundant infrastructure</p>
            </div>
            <div>
              <div class="mb-2 text-4xl font-bold text-blue-600">256-bit</div>
              <p class="text-gray-600 dark:text-gray-300">AES encryption for all data</p>
            </div>
            <div>
              <div class="mb-2 text-4xl font-bold text-purple-600">24/7</div>
              <p class="text-gray-600 dark:text-gray-300">Security monitoring and support</p>
            </div>
          </div>
          
          <div class="p-8 bg-gradient-to-r from-green-50 to-blue-50 dark:from-gray-700 dark:to-gray-600 rounded-xl">
            <h3 class="mb-4 text-2xl font-bold text-gray-900 dark:text-white">Security Transparency</h3>
            <p class="mb-6 text-gray-600 dark:text-gray-300">
              We believe in transparency about our security practices. Our security team provides regular 
              updates on our measures and can work with your compliance team for audits.
            </p>
            <router-link
              to="/contact"
              class="inline-flex items-center px-6 py-3 text-base font-medium text-white transition-all duration-200 border border-transparent rounded-lg bg-gradient-to-r from-green-600 to-blue-600 hover:from-green-700 hover:to-blue-700"
            >
              Contact Security Team
              <svg class="w-5 h-5 ml-2 -mr-1" fill="currentColor" viewBox="0 0 20 20">
                <path fill-rule="evenodd" d="M10.293 3.293a1 1 0 011.414 0l6 6a1 1 0 010 1.414l-6 6a1 1 0 01-1.414-1.414L14.586 11H3a1 1 0 110-2h11.586l-4.293-4.293a1 1 0 010-1.414z" clip-rule="evenodd" />
              </svg>
            </router-link>
          </div>
        </div>
      </section>

      <!-- CTA Section -->
      <section class="py-16 bg-gradient-to-r from-purple-600 to-indigo-600">
        <div class="px-4 mx-auto text-center max-w-7xl sm:px-6 lg:px-8">
          <h2 class="mb-4 text-3xl font-extrabold text-white">
            Your Data Security is Our Priority
          </h2>
          <p class="max-w-2xl mx-auto mb-8 text-xl text-purple-100">
            Experience the peace of mind that comes with enterprise-grade security. Start your secure journey today.
          </p>
          <div class="flex flex-col justify-center gap-4 sm:flex-row">
            <router-link
              to="/register"
              class="inline-flex items-center px-8 py-4 text-lg font-medium text-purple-600 transition-colors bg-white border border-transparent rounded-lg shadow-lg hover:bg-gray-50"
            >
              Start Secure Trial
              <svg class="w-5 h-5 ml-2 -mr-1" fill="currentColor" viewBox="0 0 20 20">
                <path fill-rule="evenodd" d="M10.293 3.293a1 1 0 011.414 0l6 6a1 1 0 010 1.414l-6 6a1 1 0 01-1.414-1.414L14.586 11H3a1 1 0 110-2h11.586l-4.293-4.293a1 1 0 010-1.414z" clip-rule="evenodd" />
              </svg>
            </router-link>
            <router-link
              to="/contact"
              class="inline-flex items-center px-8 py-4 text-lg font-medium text-white transition-all duration-200 border-2 border-white rounded-lg hover:bg-white hover:text-purple-600"
            >
              Security Questions?
            </router-link>
          </div>
        </div>
      </section>

      <!-- Footer -->
      <footer class="py-12 text-white bg-gray-900">
        <div class="px-4 mx-auto max-w-7xl sm:px-6 lg:px-8">
          <div class="grid grid-cols-1 gap-8 md:grid-cols-4">
            <div class="col-span-1 md:col-span-2">
              <div class="flex items-center mb-4">
                <div class="flex items-center justify-center w-8 h-8 rounded-full bg-gradient-to-r from-purple-600 to-indigo-600">
                  <svg class="w-5 h-5 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4.318 6.318a4.5 4.5 0 000 6.364L12 20.364l7.682-7.682a4.5 4.5 0 00-6.364-6.364L12 7.636l-1.318-1.318a4.5 4.5 0 00-6.364 0z" />
                  </svg>
                </div>
                <h3 class="ml-2 text-xl font-bold">Aesclea</h3>
              </div>
              <p class="max-w-md mb-4 text-gray-400">
                Empowering healthcare professionals with modern, efficient, and secure practice management solutions.
              </p>
            </div>
            
            <div>
              <h4 class="mb-4 text-lg font-semibold">Product</h4>
              <ul class="space-y-2 text-gray-400">
                <li><router-link to="/features" class="transition-colors hover:text-white">Features</router-link></li>
                <li><router-link to="/pricing" class="transition-colors hover:text-white">Pricing</router-link></li>
                <li><router-link to="/security" class="transition-colors hover:text-white">Security</router-link></li>
                <li><router-link to="/updates" class="transition-colors hover:text-white">Updates</router-link></li>
              </ul>
            </div>
            
            <div>
              <h4 class="mb-4 text-lg font-semibold">Support</h4>
              <ul class="space-y-2 text-gray-400">
                <li><router-link to="/help" class="transition-colors hover:text-white">Help Center</router-link></li>
                <li><router-link to="/contact" class="transition-colors hover:text-white">Contact Us</router-link></li>
                <li><router-link to="/privacy" class="transition-colors hover:text-white">Privacy Policy</router-link></li>
                <li><router-link to="/terms" class="transition-colors hover:text-white">Terms of Service</router-link></li>
              </ul>
            </div>
          </div>
          
          <div class="pt-8 mt-8 text-center text-gray-400 border-t border-gray-800">
            <p>&copy; 2025 Aesclea Medical Management System. All rights reserved.</p>
          </div>
        </div>
      </footer>
    </div>
  </div>
</template>

<script setup lang="ts">
import { onMounted } from 'vue'
import { useTheme } from '../composables/useTheme'
import AppNavbar from '../components/AppNavbar.vue'

const { isDark, toggleTheme, initializeTheme } = useTheme()

onMounted(() => {
  initializeTheme()
})
</script>
