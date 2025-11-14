<template>
  <div class="min-h-screen px-4 py-6 bg-gray-50 dark:bg-gray-900 sm:px-6 lg:px-8">
    <div class="mx-auto space-y-6 max-w-7xl">
    <!-- Breadcrumb Navigation -->
    <nav class="flex mb-6" aria-label="Breadcrumb">
      <ol class="inline-flex items-center space-x-1 md:space-x-2">
        <li class="inline-flex items-center">
          <button @click="$router.push('/dashboard')" 
            class="inline-flex items-center px-3 py-2 text-sm font-medium text-gray-600 transition-colors duration-200 rounded-lg hover:text-purple-600 hover:bg-purple-50 dark:text-gray-400 dark:hover:text-white dark:hover:bg-purple-900"
          >
            <svg class="flex-shrink-0 w-4 h-4 mr-2" fill="currentColor" viewBox="0 0 20 20">
              <path d="M10.707 2.293a1 1 0 00-1.414 0l-7 7a1 1 0 001.414 1.414L9 5.414V17a1 1 0 102 0V5.414l5.293 5.293a1 1 0 001.414-1.414l-7-7z"></path>
            </svg>
            Dashboard
          </button>
        </li>
        <li>
          <div class="flex items-center">
            <svg class="flex-shrink-0 w-3 h-3 mx-2 text-gray-400" fill="currentColor" viewBox="0 0 20 20">
              <path fill-rule="evenodd" d="M7.293 14.707a1 1 0 010-1.414L10.586 10 7.293 6.707a1 1 0 011.414-1.414l4 4a1 1 0 010 1.414l-4 4a1 1 0 01-1.414 0z" clip-rule="evenodd"></path>
            </svg>
            <button @click="$router.push('/dashboard?tab=appointments')" 
              class="inline-flex items-center px-3 py-2 text-sm font-medium text-gray-600 transition-colors duration-200 rounded-lg hover:text-purple-600 hover:bg-purple-50 dark:text-gray-400 dark:hover:text-white dark:hover:bg-purple-900"
            >
              <svg class="flex-shrink-0 w-4 h-4 mr-2" fill="currentColor" viewBox="0 0 20 20">
                <path d="M8 7V3a1 1 0 012 0v4h4a1 1 0 010 2h-4v4a1 1 0 01-2 0v-4H4a1 1 0 010-2h4z"></path>
              </svg>
              Appointments
            </button>
          </div>
        </li>
        <li aria-current="page">
          <div class="flex items-center">
            <svg class="flex-shrink-0 w-3 h-3 mx-2 text-gray-400" fill="currentColor" viewBox="0 0 20 20">
              <path fill-rule="evenodd" d="M7.293 14.707a1 1 0 010-1.414L10.586 10 7.293 6.707a1 1 0 011.414-1.414l4 4a1 1 0 010 1.414l-4 4a1 1 0 01-1.414 0z" clip-rule="evenodd"></path>
            </svg>
            <span class="inline-flex items-center px-3 py-2 text-sm font-medium text-purple-600 rounded-lg bg-purple-50 dark:text-purple-400 dark:bg-purple-900">
              <svg class="flex-shrink-0 w-4 h-4 mr-2" fill="currentColor" viewBox="0 0 20 20">
                <path d="M9 2a1 1 0 000 2h2a1 1 0 100-2H9z"></path>
                <path fill-rule="evenodd" d="M4 5a2 2 0 012-2v1a2 2 0 002 2h8a2 2 0 002-2V3a2 2 0 012 2v6h-3a3 3 0 00-3 3v3H6a2 2 0 01-2-2V5zm8 8a1 1 0 100-2 1 1 0 000 2z" clip-rule="evenodd"></path>
              </svg>
              Medical Records
            </span>
          </div>
        </li>
      </ol>
    </nav>

    <!-- Page Header with Patient Info -->
    <div class="bg-white border border-gray-200 rounded-lg shadow-sm dark:bg-gray-800 dark:border-gray-700">
      <div class="px-6 py-5">
        <div class="sm:flex sm:items-start sm:justify-between">
          <div class="flex items-start space-x-5">
            <div class="flex-shrink-0" v-if="patient">
              <div class="relative">
                <div class="flex items-center justify-center w-16 h-16 bg-purple-100 rounded-full shadow-lg dark:bg-purple-900">
                  <svg class="w-8 h-8 text-purple-600 dark:text-purple-300" fill="currentColor" viewBox="0 0 20 20">
                    <path fill-rule="evenodd" d="M10 9a3 3 0 100-6 3 3 0 000 6zm-7 9a7 7 0 1114 0H3z" clip-rule="evenodd"></path>
                  </svg>
                </div>
                <div class="absolute w-5 h-5 bg-green-400 border-white rounded-full shadow-sm -bottom-1 -right-1 border-3 dark:border-gray-800"></div>
              </div>
            </div>
            <div class="flex-1 min-w-0">
              <div class="flex items-center mb-2">
                <h1 class="text-2xl font-bold text-gray-900 dark:text-white">
                  Medical Records
                </h1>
              </div>
              <div class="flex flex-col gap-4 sm:flex-row sm:flex-wrap">
                <div class="flex items-center text-sm text-gray-600 dark:text-gray-400" v-if="patient">
                  <svg class="flex-shrink-0 w-4 h-4 mr-2 text-purple-600" fill="currentColor" viewBox="0 0 20 20">
                    <path fill-rule="evenodd" d="M10 9a3 3 0 100-6 3 3 0 000 6zm-7 9a7 7 0 1114 0H3z" clip-rule="evenodd"></path>
                  </svg>
                  <span class="mr-2 font-semibold text-gray-900 dark:text-white">Patient:</span>
                  {{ patient.firstName }} {{ patient.lastName }}
                </div>
                <div class="flex items-center text-sm text-gray-600 dark:text-gray-400" v-if="patient && patient.egn">
                  <svg class="flex-shrink-0 w-4 h-4 mr-2 text-purple-600" fill="currentColor" viewBox="0 0 20 20">
                    <path d="M10 12a2 2 0 100-4 2 2 0 000 4z"></path>
                    <path fill-rule="evenodd" d="M.458 10C1.732 5.943 5.522 3 10 3s8.268 2.943 9.542 7c-1.274 4.057-5.064 7-9.542 7S1.732 14.057.458 10zM14 10a4 4 0 11-8 0 4 4 0 018 0z" clip-rule="evenodd"></path>
                  </svg>
                  <span class="mr-2 font-semibold text-gray-900 dark:text-white">ЕГН:</span>
                  {{ patient.egn }}
                </div>
                <div class="flex items-center text-sm text-gray-600 dark:text-gray-400" v-if="patient">
                  <svg class="flex-shrink-0 w-4 h-4 mr-2 text-purple-600" fill="currentColor" viewBox="0 0 20 20">
                    <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm1-12a1 1 0 10-2 0v4a1 1 0 00.293.707l2.828 2.829a1 1 0 101.415-1.415L11 9.586V6z" clip-rule="evenodd"></path>
                  </svg>
                  <span class="mr-2 font-semibold text-gray-900 dark:text-white">Age:</span>
                  {{ calculateAge(patient.dateOfBirth) }} години
                </div>
                <div class="flex items-center text-sm text-gray-600 dark:text-gray-400">
                  <svg class="flex-shrink-0 w-4 h-4 mr-2 text-purple-600" fill="currentColor" viewBox="0 0 20 20">
                    <path fill-rule="evenodd" d="M6 2a1 1 0 00-1 1v1H4a2 2 0 00-2 2v10a2 2 0 002 2h12a2 2 0 002-2V6a2 2 0 00-2-2h-1V3a1 1 0 10-2 0v1H7V3a1 1 0 00-1-1zm0 5a1 1 0 000 2h8a1 1 0 100-2H6z" clip-rule="evenodd"></path>
                  </svg>
                  <span class="mr-2 font-semibold text-gray-900 dark:text-white">Date:</span>
                  {{ formatDate(new Date()) }}
                </div>
              </div>
            </div>
          </div>
          <div class="flex flex-col mt-5 space-y-2 sm:flex-row sm:mt-0 sm:space-y-0 sm:space-x-3">
            <button @click="saveAll" 
              :disabled="isSaving"
              class="inline-flex items-center justify-center px-4 py-2.5 text-sm font-semibold text-white bg-green-600 border border-transparent rounded-lg shadow-sm hover:bg-green-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-green-500 disabled:opacity-50 disabled:cursor-not-allowed transition-colors duration-200"
            >
              <svg class="flex-shrink-0 w-4 h-4 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 7H5a2 2 0 00-2 2v9a2 2 0 002 2h14a2 2 0 002-2V9a2 2 0 00-2-2h-3m-1 4l-3 3m0 0l-3-3m3 3V4"></path>
              </svg>
              {{ isSaving ? 'Запазване...' : 'Запази' }}
            </button>
            <button @click="printOutpatientRecord" 
              class="inline-flex items-center justify-center px-4 py-2.5 text-sm font-semibold text-gray-700 bg-white border border-gray-300 rounded-lg shadow-sm hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-purple-500 dark:bg-gray-800 dark:text-gray-300 dark:border-gray-600 dark:hover:bg-gray-700 transition-colors duration-200"
            >
              <svg class="flex-shrink-0 w-4 h-4 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17 17h2a2 2 0 002-2v-4a2 2 0 00-2-2H5a2 2 0 00-2 2v4a2 2 0 002 2h2m2 4h6a2 2 0 002-2v-4a2 2 0 00-2-2H9a2 2 0 00-2 2v4a2 2 0 002 2zm8-12V5a2 2 0 00-2-2H9a2 2 0 00-2 2v4h10z"></path>
              </svg>
              Печат
            </button>
            <button @click="$router.back()" 
              class="inline-flex items-center justify-center px-4 py-2.5 text-sm font-semibold text-gray-700 bg-white border border-gray-300 rounded-lg shadow-sm hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-purple-500 dark:bg-gray-800 dark:text-gray-300 dark:border-gray-600 dark:hover:bg-gray-700 transition-colors duration-200"
            >
              <svg class="flex-shrink-0 w-4 h-4 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10 19l-7-7m0 0l7-7m-7 7h18"></path>
              </svg>
              Назад
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Main Content Grid -->
    <div class="grid grid-cols-1 gap-6 lg:grid-cols-2">
      <!-- Left Column - Medical Information -->
      <div class="space-y-6">
        <!-- Chief Complaint -->
        <div class="bg-white border border-gray-200 rounded-lg shadow-sm dark:bg-gray-800 dark:border-gray-700">
          <div class="px-6 py-4 border-b border-gray-200 dark:border-gray-700">
            <div class="flex items-center">
              <svg class="flex-shrink-0 w-5 h-5 mr-3 text-purple-600 dark:text-purple-400" fill="currentColor" viewBox="0 0 20 20">
                <path fill-rule="evenodd" d="M18 10c0 3.866-3.582 7-8 7a8.841 8.841 0 01-4.083-.98L2 17l1.338-3.123C2.493 12.767 2 11.434 2 10c0-3.866 3.582-7 8-7s8 3.134 8 7zM7 9H5v2h2V9zm8 0h-2v2h2V9zM9 9h2v2H9V9z" clip-rule="evenodd"></path>
              </svg>
              <h3 class="text-lg font-semibold text-gray-900 dark:text-white">Оплаквания</h3>
            </div>
          </div>
          <div class="p-6">
            <div class="relative">
              <textarea 
                v-model="outpatientRecord.chiefComplaint" 
                placeholder="Опишете основните оплаквания на пациента..."
                rows="3"
                class="w-full px-3 py-2 placeholder-gray-400 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-purple-500 focus:border-purple-500 sm:text-sm dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-purple-500 dark:focus:border-purple-500"
              ></textarea>
              <button 
                class="absolute flex items-center justify-center w-8 h-8 text-xs font-medium text-purple-700 transition-all duration-200 bg-purple-100 rounded top-3 right-3 hover:bg-purple-200 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-purple-500 hover:scale-110 dark:bg-purple-900 dark:text-purple-200 dark:hover:bg-purple-800" 
                @click="aiAssist('chiefComplaint')" 
                title="AI помощ"
              >
                <svg class="w-4 h-4" fill="currentColor" viewBox="0 0 20 20">
                  <path d="M9.049 2.927c.3-.921 1.603-.921 1.902 0l1.07 3.292a1 1 0 00.95.69h3.462c.969 0 1.371 1.24.588 1.81l-2.8 2.034a1 1 0 00-.364 1.118l1.07 3.292c.3.921-.755 1.688-1.54 1.118l-2.8-2.034a1 1 0 00-1.175 0l-2.8 2.034c-.784.57-1.838-.197-1.539-1.118l1.07-3.292a1 1 0 00-.364-1.118L2.98 8.72c-.783-.57-.38-1.81.588-1.81h3.461a1 1 0 00.951-.69l1.07-3.292z"></path>
                </svg>
              </button>
            </div>
          </div>
        </div>

        <!-- Medical History -->
        <div class="bg-white border border-gray-200 rounded-lg shadow-sm dark:bg-gray-800 dark:border-gray-700">
          <div class="px-6 py-4 border-b border-gray-200 dark:border-gray-700">
            <div class="flex items-center">
              <svg class="flex-shrink-0 w-5 h-5 mr-3 text-purple-600 dark:text-purple-400" fill="currentColor" viewBox="0 0 20 20">
                <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm1-12a1 1 0 10-2 0v4a1 1 0 00.293.707l2.828 2.829a1 1 0 101.415-1.415L11 9.586V6z" clip-rule="evenodd"></path>
              </svg>
              <h3 class="text-lg font-semibold text-gray-900 dark:text-white">Анамнеза</h3>
            </div>
          </div>
          <div class="p-6">
            <div class="relative">
              <textarea 
                v-model="outpatientRecord.medicalHistory" 
                placeholder="Предишни заболявания, операции, алергии..."
                rows="4"
                class="w-full px-3 py-2 placeholder-gray-400 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-purple-500 focus:border-purple-500 sm:text-sm dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-purple-500 dark:focus:border-purple-500"
              ></textarea>
              <button 
                class="absolute flex items-center justify-center w-8 h-8 text-xs font-medium text-purple-700 transition-all duration-200 bg-purple-100 rounded top-3 right-3 hover:bg-purple-200 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-purple-500 hover:scale-110 dark:bg-purple-900 dark:text-purple-200 dark:hover:bg-purple-800" 
                @click="aiAssist('medicalHistory')" 
                title="AI помощ"
              >
                <svg class="w-4 h-4" fill="currentColor" viewBox="0 0 20 20">
                  <path d="M9.049 2.927c.3-.921 1.603-.921 1.902 0l1.07 3.292a1 1 0 00.95.69h3.462c.969 0 1.371 1.24.588 1.81l-2.8 2.034a1 1 0 00-.364 1.118l1.07 3.292c.3.921-.755 1.688-1.54 1.118l-2.8-2.034a1 1 0 00-1.175 0l-2.8 2.034c-.784.57-1.838-.197-1.539-1.118l1.07-3.292a1 1 0 00-.364-1.118L2.98 8.72c-.783-.57-.38-1.81.588-1.81h3.461a1 1 0 00.951-.69l1.07-3.292z"></path>
                </svg>
              </button>
            </div>
          </div>
        </div>

        <!-- Physical Examination -->
        <div class="bg-white border border-gray-200 rounded-lg shadow-sm dark:bg-gray-800 dark:border-gray-700">
          <div class="px-6 py-4 border-b border-gray-200 dark:border-gray-700">
            <div class="flex items-center">
              <svg class="w-5 h-5 mr-2 text-purple-600 dark:text-purple-400" fill="currentColor" viewBox="0 0 20 20">
                <path d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z"></path>
              </svg>
              <h3 class="text-lg font-medium text-gray-900 dark:text-white">Обективно Изследване</h3>
            </div>
          </div>
          <div class="p-6">
            <div class="relative">
              <textarea 
                v-model="outpatientRecord.physicalExamination" 
                placeholder="Физикален преглед, находки..."
                rows="4"
                class="w-full px-3 py-2 placeholder-gray-400 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-purple-500 focus:border-purple-500 sm:text-sm dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-purple-500 dark:focus:border-purple-500"
              ></textarea>
              <button 
                class="absolute top-2 right-2 inline-flex items-center px-2.5 py-1.5 border border-transparent text-xs font-medium rounded text-purple-700 bg-purple-100 hover:bg-purple-200 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-purple-500 dark:bg-purple-900 dark:text-purple-200 dark:hover:bg-purple-800" 
                @click="aiAssist('physicalExamination')" 
                title="AI помощ"
              >
                <svg class="w-4 h-4" fill="currentColor" viewBox="0 0 20 20">
                  <path d="M9.049 2.927c.3-.921 1.603-.921 1.902 0l1.07 3.292a1 1 0 00.95.69h3.462c.969 0 1.371 1.24.588 1.81l-2.8 2.034a1 1 0 00-.364 1.118l1.07 3.292c.3.921-.755 1.688-1.54 1.118l-2.8-2.034a1 1 0 00-1.175 0l-2.8 2.034c-.784.57-1.838-.197-1.539-1.118l1.07-3.292a1 1 0 00-.364-1.118L2.98 8.72c-.783-.57-.38-1.81.588-1.81h3.461a1 1 0 00.951-.69l1.07-3.292z"></path>
                </svg>
              </button>
            </div>
          </div>
        </div>

        <!-- Vital Signs -->
        <div class="bg-white border border-gray-200 rounded-lg shadow-sm dark:bg-gray-800 dark:border-gray-700">
          <div class="px-6 py-4 border-b border-gray-200 dark:border-gray-700">
            <div class="flex items-center">
              <svg class="w-5 h-5 mr-2 text-purple-600 dark:text-purple-400" fill="currentColor" viewBox="0 0 20 20">
                <path d="M4.828 5a3 3 0 01.474-1.65A3 3 0 017.5 2h5a3 3 0 012.198 1.35A3 3 0 0115.172 5H17a1 1 0 110 2h-1.172a3 3 0 01-.474 1.65A3 3 0 0113.5 10h-5a3 3 0 01-2.198-1.35A3 3 0 014.828 7H3a1 1 0 110-2h1.828zM8.5 4a1 1 0 000 2h3a1 1 0 000-2h-3z"></path>
              </svg>
              <h3 class="text-lg font-medium text-gray-900 dark:text-white">Витални Показатели</h3>
            </div>
          </div>
          <div class="p-6">
            <div class="grid grid-cols-2 gap-4">
              <div class="space-y-2">
                <label class="flex items-center text-sm font-medium text-gray-700 dark:text-gray-200">
                  <svg class="w-4 h-4 mr-2 text-purple-600" fill="currentColor" viewBox="0 0 20 20">
                    <path d="M5 3a2 2 0 00-2 2v2a2 2 0 002 2h2a2 2 0 002-2V5a2 2 0 00-2-2H5zM5 11a2 2 0 00-2 2v2a2 2 0 002 2h2a2 2 0 002-2v-2a2 2 0 00-2-2H5zM11 5a2 2 0 012-2h2a2 2 0 012 2v2a2 2 0 01-2 2h-2a2 2 0 01-2-2V5zM11 13a2 2 0 012-2h2a2 2 0 012 2v2a2 2 0 01-2 2h-2a2 2 0 01-2-2v-2z"></path>
                  </svg>
                  Кръвно Налягане
                </label>
                <input type="text" v-model="vitalSigns.bloodPressure" placeholder="120/80 mmHg" class="w-full px-3 py-2 placeholder-gray-400 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-purple-500 focus:border-purple-500 sm:text-sm dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-purple-500 dark:focus:border-purple-500">
              </div>
              <div class="space-y-2">
                <label class="flex items-center text-sm font-medium text-gray-700 dark:text-gray-200">
                  <svg class="w-4 h-4 mr-2 text-purple-600" fill="currentColor" viewBox="0 0 20 20">
                    <path d="M3.172 5.172a4 4 0 015.656 0L10 6.343l1.172-1.171a4 4 0 115.656 5.656L10 17.657l-6.828-6.829a4 4 0 010-5.656z"></path>
                  </svg>
                  Пулс
                </label>
                <input type="text" v-model="vitalSigns.pulse" placeholder="72 уд/мин" class="w-full px-3 py-2 placeholder-gray-400 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-purple-500 focus:border-purple-500 sm:text-sm dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-purple-500 dark:focus:border-purple-500">
              </div>
              <div class="space-y-2">
                <label class="flex items-center text-sm font-medium text-gray-700 dark:text-gray-200">
                  <svg class="w-4 h-4 mr-2 text-purple-600" fill="currentColor" viewBox="0 0 20 20">
                    <path d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z"></path>
                  </svg>
                  Температура
                </label>
                <input type="text" v-model="vitalSigns.temperature" placeholder="36.6°C" class="w-full px-3 py-2 placeholder-gray-400 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-purple-500 focus:border-purple-500 sm:text-sm dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-purple-500 dark:focus:border-purple-500">
              </div>
              <div class="space-y-2">
                <label class="flex items-center text-sm font-medium text-gray-700 dark:text-gray-200">
                  <svg class="w-4 h-4 mr-2 text-purple-600" fill="currentColor" viewBox="0 0 20 20">
                    <path d="M10 2L3 7v11a2 2 0 002 2h10a2 2 0 002-2V7l-7-5z"></path>
                  </svg>
                  Дишане
                </label>
                <input type="text" v-model="vitalSigns.respiration" placeholder="16/мин" class="w-full px-3 py-2 placeholder-gray-400 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-purple-500 focus:border-purple-500 sm:text-sm dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-purple-500 dark:focus:border-purple-500">
              </div>
            </div>
          </div>
        </div>

        <!-- Treatment Plan -->
        <div class="bg-white border border-gray-200 rounded-lg shadow-sm dark:bg-gray-800 dark:border-gray-700">
          <div class="px-6 py-4 border-b border-gray-200 dark:border-gray-700">
            <div class="flex items-center">
              <svg class="w-5 h-5 mr-2 text-purple-600 dark:text-purple-400" fill="currentColor" viewBox="0 0 20 20">
                <path d="M9 5H7a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2V7a2 2 0 00-2-2zm5 0h-2a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2V7a2 2 0 00-2-2z"></path>
              </svg>
              <h3 class="text-lg font-medium text-gray-900 dark:text-white">План за Лечение</h3>
            </div>
          </div>
          <div class="p-6">
            <div class="relative">
              <textarea 
                v-model="outpatientRecord.treatmentPlan" 
                placeholder="Препоръки, план за лечение..."
                rows="3"
                class="w-full px-3 py-2 placeholder-gray-400 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-purple-500 focus:border-purple-500 sm:text-sm dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-purple-500 dark:focus:border-purple-500"
              ></textarea>
              <button 
                class="absolute top-2 right-2 inline-flex items-center px-2.5 py-1.5 border border-transparent text-xs font-medium rounded text-purple-700 bg-purple-100 hover:bg-purple-200 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-purple-500 dark:bg-purple-900 dark:text-purple-200 dark:hover:bg-purple-800" 
                @click="aiAssist('treatmentPlan')" 
                title="AI помощ"
              >
                <svg class="w-4 h-4" fill="currentColor" viewBox="0 0 20 20">
                  <path d="M9.049 2.927c.3-.921 1.603-.921 1.902 0l1.07 3.292a1 1 0 00.95.69h3.462c.969 0 1.371 1.24.588 1.81l-2.8 2.034a1 1 0 00-.364 1.118l1.07 3.292c.3.921-.755 1.688-1.54 1.118l-2.8-2.034a1 1 0 00-1.175 0l-2.8 2.034c-.784.57-1.838-.197-1.539-1.118l1.07-3.292a1 1 0 00-.364-1.118L2.98 8.72c-.783-.57-.38-1.81.588-1.81h3.461a1 1 0 00.951-.69l1.07-3.292z"></path>
                </svg>
              </button>
            </div>
          </div>
        </div>

        <!-- Notes -->
        <div class="bg-white border border-gray-200 rounded-lg shadow-sm dark:bg-gray-800 dark:border-gray-700">
          <div class="px-6 py-4 border-b border-gray-200 dark:border-gray-700">
            <div class="flex items-center">
              <svg class="w-5 h-5 mr-2 text-purple-600 dark:text-purple-400" fill="currentColor" viewBox="0 0 20 20">
                <path d="M4 4a2 2 0 012-2h8a2 2 0 012 2v12a2 2 0 01-2 2H6a2 2 0 01-2-2V4z"></path>
              </svg>
              <h3 class="text-lg font-medium text-gray-900 dark:text-white">Забележки</h3>
            </div>
          </div>
          <div class="p-6">
            <div class="relative">
              <textarea 
                v-model="outpatientRecord.notes" 
                placeholder="Допълнителни бележки..."
                rows="2"
                class="w-full px-3 py-2 placeholder-gray-400 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-purple-500 focus:border-purple-500 sm:text-sm dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-purple-500 dark:focus:border-purple-500"
              ></textarea>
              <button 
                class="absolute top-2 right-2 inline-flex items-center px-2.5 py-1.5 border border-transparent text-xs font-medium rounded text-purple-700 bg-purple-100 hover:bg-purple-200 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-purple-500 dark:bg-purple-900 dark:text-purple-200 dark:hover:bg-purple-800" 
                @click="aiAssist('notes')" 
                title="AI помощ"
              >
                <svg class="w-4 h-4" fill="currentColor" viewBox="0 0 20 20">
                  <path d="M9.049 2.927c.3-.921 1.603-.921 1.902 0l1.07 3.292a1 1 0 00.95.69h3.462c.969 0 1.371 1.24.588 1.81l-2.8 2.034a1 1 0 00-.364 1.118l1.07 3.292c.3.921-.755 1.688-1.54 1.118l-2.8-2.034a1 1 0 00-1.175 0l-2.8 2.034c-.784.57-1.838-.197-1.539-1.118l1.07-3.292a1 1 0 00-.364-1.118L2.98 8.72c-.783-.57-.38-1.81.588-1.81h3.461a1 1 0 00.951-.69l1.07-3.292z"></path>
                </svg>
              </button>
            </div>
          </div>
        </div>
      </div>

      <!-- Right Column - Actions -->
      <div class="space-y-6">
        <!-- Diagnoses -->
        <div class="bg-white border border-gray-200 rounded-lg shadow-sm dark:bg-gray-800 dark:border-gray-700">
          <div class="px-6 py-4 border-b border-gray-200 dark:border-gray-700">
            <div class="flex items-center justify-between">
              <div class="flex items-center">
                <svg class="w-5 h-5 mr-2 text-purple-600 dark:text-purple-400" fill="currentColor" viewBox="0 0 20 20">
                  <path d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z"></path>
                </svg>
                <h3 class="text-lg font-medium text-gray-900 dark:text-white">Диагнози</h3>
              </div>
              <button @click="showDiagnosisModal = true" class="inline-flex items-center p-2 text-sm font-medium text-white bg-green-600 rounded-full hover:bg-green-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-green-500">
                <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 6v6m0 0v6m0-6h6m-6 0H6"></path>
                </svg>
              </button>
            </div>
          </div>
          <div class="p-6">
            <div class="space-y-4">
              <div v-for="diagnosis in diagnoses" :key="diagnosis.id" class="relative p-4 border border-gray-200 rounded-lg bg-gray-50 dark:bg-gray-700 dark:border-gray-600">
                <button @click="removeDiagnosis(diagnosis.id)" class="absolute p-1 text-red-600 rounded-full top-2 right-2 hover:text-red-800 hover:bg-red-100 dark:text-red-400 dark:hover:text-red-300 dark:hover:bg-red-900">
                  <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"></path>
                  </svg>
                </button>
                <div class="flex flex-wrap gap-2 mb-2">
                  <span v-if="diagnosis.icd10Code" class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-purple-100 text-purple-800 dark:bg-purple-900 dark:text-purple-200">{{ diagnosis.icd10Code }}</span>
                  <span class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-gray-100 text-gray-800 dark:bg-gray-600 dark:text-gray-200">{{ formatDiagnosisType(diagnosis.diagnosisType) }}</span>
                </div>
                <h4 class="mb-1 font-medium text-gray-900 dark:text-white">{{ diagnosis.diagnosisName }}</h4>
                <p v-if="diagnosis.clinicalFindings" class="text-sm text-gray-600 dark:text-gray-400">{{ diagnosis.clinicalFindings }}</p>
              </div>
              <div v-if="diagnoses.length === 0" class="py-6 text-center">
                <svg class="w-12 h-12 mx-auto text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z"></path>
                </svg>
                <p class="mt-2 text-sm text-gray-500 dark:text-gray-400">Няма добавени диагнози</p>
              </div>
            </div>
          </div>
        </div>

        <!-- Prescriptions -->
        <div class="bg-white border border-gray-200 rounded-lg shadow-sm dark:bg-gray-800 dark:border-gray-700">
          <div class="px-6 py-4 border-b border-gray-200 dark:border-gray-700">
            <div class="flex items-center justify-between">
              <div class="flex items-center">
                <svg class="w-5 h-5 mr-2 text-purple-600 dark:text-purple-400" fill="currentColor" viewBox="0 0 20 20">
                  <path d="M10 2L3 7v11a2 2 0 002 2h10a2 2 0 002-2V7l-7-5z"></path>
                </svg>
                <h3 class="text-lg font-medium text-gray-900 dark:text-white">Лекарства</h3>
              </div>
              <button @click="showPrescriptionModal = true" class="inline-flex items-center p-2 text-sm font-medium text-white bg-green-600 rounded-full hover:bg-green-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-green-500">
                <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 6v6m0 0v6m0-6h6m-6 0H6"></path>
                </svg>
              </button>
            </div>
          </div>
          <div class="p-6">
            <div class="space-y-4">
              <div v-for="prescription in prescriptions" :key="prescription.id" class="relative p-4 border border-gray-200 rounded-lg bg-gray-50 dark:bg-gray-700 dark:border-gray-600">
                <button @click="removePrescription(prescription.id)" class="absolute p-1 text-red-600 rounded-full top-2 right-2 hover:text-red-800 hover:bg-red-100 dark:text-red-400 dark:hover:text-red-300 dark:hover:bg-red-900">
                  <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"></path>
                  </svg>
                </button>
                <h4 class="flex items-center mb-3 font-medium text-gray-900 dark:text-white">
                  <svg class="w-4 h-4 mr-2 text-purple-600" fill="currentColor" viewBox="0 0 20 20">
                    <path d="M10 2L3 7v11a2 2 0 002 2h10a2 2 0 002-2V7l-7-5z"></path>
                  </svg>
                  {{ prescription.medicationName }}
                </h4>
                <div class="space-y-2">
                  <div class="flex justify-between text-sm">
                    <span class="font-medium text-gray-700 dark:text-gray-300">Дозировка:</span>
                    <span class="text-gray-600 dark:text-gray-400">{{ prescription.dosage }}</span>
                  </div>
                  <div class="flex justify-between text-sm">
                    <span class="font-medium text-gray-700 dark:text-gray-300">Честота:</span>
                    <span class="text-gray-600 dark:text-gray-400">{{ prescription.frequency }}</span>
                  </div>
                  <div class="flex justify-between text-sm">
                    <span class="font-medium text-gray-700 dark:text-gray-300">Начин:</span>
                    <span class="text-gray-600 dark:text-gray-400">{{ formatRoute(prescription.route) }}</span>
                  </div>
                  <div class="flex justify-between text-sm">
                    <span class="font-medium text-gray-700 dark:text-gray-300">Продължителност:</span>
                    <span class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-blue-100 text-blue-800 dark:bg-blue-900 dark:text-blue-200">{{ prescription.duration }} дни</span>
                  </div>
                </div>
                <p v-if="prescription.instructions" class="p-2 mt-3 text-sm text-yellow-700 border-l-4 border-yellow-400 bg-yellow-50 dark:bg-yellow-900/20 dark:text-yellow-300">
                  <svg class="inline w-4 h-4 mr-1" fill="currentColor" viewBox="0 0 20 20">
                    <path fill-rule="evenodd" d="M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-7-4a1 1 0 11-2 0 1 1 0 012 0zM9 9a1 1 0 000 2v3a1 1 0 001 1h1a1 1 0 100-2v-3a1 1 0 00-1-1H9z" clip-rule="evenodd"></path>
                  </svg>
                  {{ prescription.instructions }}
                </p>
              </div>
              <div v-if="prescriptions.length === 0" class="py-6 text-center">
                <svg class="w-12 h-12 mx-auto text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 11H5m14 0a2 2 0 012 2v6a2 2 0 01-2 2H5a2 2 0 01-2-2v-6a2 2 0 012-2m14 0V9a2 2 0 00-2-2M5 11V9a2 2 0 012-2m0 0V5a2 2 0 012-2h6a2 2 0 012 2v2M7 7h10"></path>
                </svg>
                <p class="mt-2 text-sm text-gray-500 dark:text-gray-400">Няма предписани лекарства</p>
              </div>
            </div>
          </div>
        </div>

        <!-- Referrals -->
        <div class="bg-white border border-gray-200 rounded-lg shadow-sm dark:bg-gray-800 dark:border-gray-700">
          <div class="px-6 py-4 border-b border-gray-200 dark:border-gray-700">
            <div class="flex items-center justify-between">
              <div class="flex items-center">
                <svg class="w-5 h-5 mr-2 text-purple-600 dark:text-purple-400" fill="currentColor" viewBox="0 0 20 20">
                  <path fill-rule="evenodd" d="M10 9a3 3 0 100-6 3 3 0 000 6zm-7 9a7 7 0 1114 0H3z" clip-rule="evenodd"></path>
                </svg>
                <h3 class="text-lg font-medium text-gray-900 dark:text-white">Направления</h3>
              </div>
              <button @click="showReferralModal = true" class="inline-flex items-center p-2 text-sm font-medium text-white bg-green-600 rounded-full hover:bg-green-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-green-500">
                <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 6v6m0 0v6m0-6h6m-6 0H6"></path>
                </svg>
              </button>
            </div>
          </div>
          <div class="p-6">
            <div class="space-y-4">
              <div v-for="referral in referrals" :key="referral.id" class="relative p-4 border border-gray-200 rounded-lg bg-gray-50 dark:bg-gray-700 dark:border-gray-600">
                <button @click="removeReferral(referral.id)" class="absolute p-1 text-red-600 rounded-full top-2 right-2 hover:text-red-800 hover:bg-red-100 dark:text-red-400 dark:hover:text-red-300 dark:hover:bg-red-900">
                  <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"></path>
                  </svg>
                </button>
                <h4 class="flex items-center mb-2 font-medium text-gray-900 dark:text-white">
                  <svg class="w-4 h-4 mr-2 text-purple-600" fill="currentColor" viewBox="0 0 20 20">
                    <path d="M3 4a1 1 0 011-1h12a1 1 0 011 1v2a1 1 0 01-1 1H4a1 1 0 01-1-1V4zM3 10a1 1 0 011-1h6a1 1 0 011 1v6a1 1 0 01-1 1H4a1 1 0 01-1-1v-6zM14 9a1 1 0 00-1 1v6a1 1 0 001 1h2a1 1 0 001-1v-6a1 1 0 00-1-1h-2z"></path>
                  </svg>
                  {{ referral.specialty }}
                </h4>
                <p class="mb-2 text-sm text-gray-600 dark:text-gray-400"><strong>Причина:</strong> {{ referral.reason }}</p>
                <p v-if="referral.expiryDate" class="inline-flex items-center px-2 py-1 text-xs text-blue-600 bg-blue-100 rounded-lg dark:bg-blue-900 dark:text-blue-200">
                  <svg class="w-3 h-3 mr-1" fill="currentColor" viewBox="0 0 20 20">
                    <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm1-12a1 1 0 10-2 0v4a1 1 0 00.293.707l2.828 2.829a1 1 0 101.415-1.415L11 9.586V6z" clip-rule="evenodd"></path>
                  </svg>
                  Валидно до: {{ formatDate(referral.expiryDate) }}
                </p>
              </div>
              <div v-if="referrals.length === 0" class="py-6 text-center">
                <svg class="w-12 h-12 mx-auto text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z"></path>
                </svg>
                <p class="mt-2 text-sm text-gray-500 dark:text-gray-400">Няма издадени направления</p>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Enhanced Diagnosis Modal -->
    <div v-if="showDiagnosisModal" class="fixed inset-0 z-50 overflow-y-auto" @click.self="showDiagnosisModal = false">
      <div class="flex items-center justify-center min-h-screen px-4 pt-4 pb-20 text-center sm:block sm:p-0">
        <div class="fixed inset-0 transition-opacity bg-gray-500 bg-opacity-75 z-40" @click="showDiagnosisModal = false"></div>
        <div class="relative inline-block overflow-hidden text-left align-bottom transition-all transform bg-white rounded-lg shadow-xl dark:bg-gray-800 sm:my-8 sm:align-middle sm:max-w-2xl sm:w-full z-50">
          <div class="px-4 pt-5 pb-4 bg-white dark:bg-gray-800 sm:p-6 sm:pb-4">
            <div class="flex items-center justify-between mb-4">
              <h3 class="text-lg font-medium leading-6 text-gray-900 dark:text-white">Добави Диагноза (МКБ-10)</h3>
              <button @click="showDiagnosisModal = false" class="p-2 text-gray-400 rounded-lg hover:text-gray-600 hover:bg-gray-100 dark:hover:text-gray-300 dark:hover:bg-gray-700">
                <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"></path>
                </svg>
              </button>
            </div>
            
            <!-- Search Bar -->
            <div class="mb-4">
              <label class="block mb-2 text-sm font-medium text-gray-700 dark:text-gray-200">Търси МКБ-10 диагноза</label>
              <input 
                type="text" 
                v-model="diagnosisSearchTerm" 
                @input="searchDiagnoses"
                placeholder="Търси по код или име на диагноза..."
                class="w-full px-3 py-2 placeholder-gray-400 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-purple-500 focus:border-purple-500 sm:text-sm dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white"
              >
            </div>
            
            <!-- Search Results -->
            <div v-if="filteredDiagnoses.length > 0" class="mb-4 overflow-y-auto border border-gray-200 rounded-md max-h-40 dark:border-gray-600">
              <div 
                v-for="diagnosis in filteredDiagnoses.slice(0, 10)" 
                :key="diagnosis.code"
                @click="selectDiagnosis(diagnosis)"
                class="p-3 border-b border-gray-100 cursor-pointer hover:bg-purple-50 dark:hover:bg-purple-900 dark:border-gray-700 last:border-b-0"
              >
                <div class="text-sm font-medium text-purple-600 dark:text-purple-300">{{ diagnosis.code }}</div>
                <div class="text-sm text-gray-700 dark:text-gray-300">{{ diagnosis.name }}</div>
              </div>
            </div>
            
            <div class="space-y-4">
              <div class="grid grid-cols-2 gap-4">
                <div>
                  <label class="block text-sm font-medium text-gray-700 dark:text-gray-200">МКБ-10 Код</label>
                  <input type="text" v-model="newDiagnosis.icd10Code" placeholder="J06.9" class="block w-full px-3 py-2 mt-1 placeholder-gray-400 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-purple-500 focus:border-purple-500 sm:text-sm dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white">
                </div>
                <div>
                  <label class="block text-sm font-medium text-gray-700 dark:text-gray-200">Тип</label>
                  <select v-model="newDiagnosis.diagnosisType" class="block w-full px-3 py-2 mt-1 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-purple-500 focus:border-purple-500 sm:text-sm dark:bg-gray-700 dark:border-gray-600 dark:text-white">
                    <option value="primary">Основна</option>
                    <option value="secondary">Съпътстваща</option>
                    <option value="differential">Диференциална</option>
                  </select>
                </div>
              </div>
              
              <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-200">Име на Диагнозата *</label>
                <input type="text" v-model="newDiagnosis.diagnosisName" placeholder="Остра горнодихателна инфекция" class="block w-full px-3 py-2 mt-1 placeholder-gray-400 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-purple-500 focus:border-purple-500 sm:text-sm dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white">
              </div>
              
              <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-200">Клинични Находки</label>
                <textarea v-model="newDiagnosis.clinicalFindings" rows="3" placeholder="Описание на клиничните находки..." class="block w-full px-3 py-2 mt-1 placeholder-gray-400 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-purple-500 focus:border-purple-500 sm:text-sm dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white"></textarea>
              </div>
              
              <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-200">Бележки</label>
                <textarea v-model="newDiagnosis.notes" rows="2" placeholder="Допълнителни бележки..." class="block w-full px-3 py-2 mt-1 placeholder-gray-400 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-purple-500 focus:border-purple-500 sm:text-sm dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white"></textarea>
              </div>
            </div>
          </div>
          <div class="px-4 py-3 bg-gray-50 dark:bg-gray-700 sm:px-6 sm:flex sm:flex-row-reverse">
            <button @click="addDiagnosis" :disabled="!newDiagnosis.diagnosisName" class="inline-flex justify-center w-full px-4 py-2 text-base font-medium text-white bg-purple-600 border border-transparent rounded-md shadow-sm hover:bg-purple-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-purple-500 sm:ml-3 sm:w-auto sm:text-sm disabled:opacity-50 disabled:cursor-not-allowed">Добави</button>
            <button @click="showDiagnosisModal = false" class="inline-flex justify-center w-full px-4 py-2 mt-3 text-base font-medium text-gray-700 bg-white border border-gray-300 rounded-md shadow-sm hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-purple-500 sm:mt-0 sm:ml-3 sm:w-auto sm:text-sm dark:bg-gray-800 dark:text-gray-300 dark:border-gray-600 dark:hover:bg-gray-700">Отказ</button>
          </div>
        </div>
      </div>
    </div>

    <!-- Enhanced Prescription Modal -->
    <div v-if="showPrescriptionModal" class="fixed inset-0 z-50 overflow-y-auto" @click.self="showPrescriptionModal = false">
      <div class="flex items-center justify-center min-h-screen px-4 pt-4 pb-20 text-center sm:block sm:p-0">
        <div class="fixed inset-0 transition-opacity bg-gray-500 bg-opacity-75 z-40" @click="showPrescriptionModal = false"></div>
        <div class="relative inline-block overflow-hidden text-left align-bottom transition-all transform bg-white rounded-lg shadow-xl dark:bg-gray-800 sm:my-8 sm:align-middle sm:max-w-2xl sm:w-full z-50">
          <div class="px-4 pt-5 pb-4 bg-white dark:bg-gray-800 sm:p-6 sm:pb-4">
            <div class="flex items-center justify-between mb-4">
              <h3 class="text-lg font-medium leading-6 text-gray-900 dark:text-white">Добави Лекарство</h3>
              <button @click="showPrescriptionModal = false" class="p-2 text-gray-400 rounded-lg hover:text-gray-600 hover:bg-gray-100 dark:hover:text-gray-300 dark:hover:bg-gray-700">
                <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"></path>
                </svg>
              </button>
            </div>
            
            <!-- Medication Search -->
            <div class="mb-4">
              <label class="block mb-2 text-sm font-medium text-gray-700 dark:text-gray-200">Търси лекарство</label>
              <input 
                type="text" 
                v-model="medicationSearchTerm" 
                @input="searchMedications"
                placeholder="Търси по име на лекарството..."
                class="w-full px-3 py-2 placeholder-gray-400 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-purple-500 focus:border-purple-500 sm:text-sm dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white"
              >
            </div>
            
            <!-- Medication Results -->
            <div v-if="filteredMedications.length > 0" class="mb-4 overflow-y-auto border border-gray-200 rounded-md max-h-40 dark:border-gray-600">
              <div 
                v-for="medication in filteredMedications.slice(0, 10)" 
                :key="medication.name"
                @click="selectMedication(medication)"
                class="p-3 border-b border-gray-100 cursor-pointer hover:bg-purple-50 dark:hover:bg-purple-900 dark:border-gray-700 last:border-b-0"
              >
                <div class="text-sm font-medium text-purple-600 dark:text-purple-300">{{ medication.name }}</div>
                <div class="text-xs text-gray-500 dark:text-gray-400">{{ medication.activeIngredient }} | {{ medication.form }}</div>
              </div>
            </div>
            
            <div class="space-y-4">
              <div class="grid grid-cols-2 gap-4">
                <div>
                  <label class="block text-sm font-medium text-gray-700 dark:text-gray-200">Лекарство *</label>
                  <input type="text" v-model="newPrescription.medicationName" placeholder="Парацетамол" class="block w-full px-3 py-2 mt-1 placeholder-gray-400 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-purple-500 focus:border-purple-500 sm:text-sm dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white">
                </div>
                <div>
                  <label class="block text-sm font-medium text-gray-700 dark:text-gray-200">Дозировка *</label>
                  <input type="text" v-model="newPrescription.dosage" placeholder="500mg" class="block w-full px-3 py-2 mt-1 placeholder-gray-400 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-purple-500 focus:border-purple-500 sm:text-sm dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white">
                </div>
              </div>
              
              <div class="grid grid-cols-2 gap-4">
                <div>
                  <label class="block text-sm font-medium text-gray-700 dark:text-gray-200">Честота</label>
                  <select v-model="newPrescription.frequency" class="block w-full px-3 py-2 mt-1 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-purple-500 focus:border-purple-500 sm:text-sm dark:bg-gray-700 dark:border-gray-600 dark:text-white">
                    <option value="1xдневно">1x дневно</option>
                    <option value="2xдневно">2x дневно</option>
                    <option value="3xдневно">3x дневно</option>
                    <option value="4xдневно">4x дневно</option>
                    <option value="по нужда">По нужда</option>
                  </select>
                </div>
                <div>
                  <label class="block text-sm font-medium text-gray-700 dark:text-gray-200">Път на въвеждане</label>
                  <select v-model="newPrescription.route" class="block w-full px-3 py-2 mt-1 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-purple-500 focus:border-purple-500 sm:text-sm dark:bg-gray-700 dark:border-gray-600 dark:text-white">
                    <option value="Орално">Орално</option>
                    <option value="Интравенозно">Интравенозно</option>
                    <option value="Интрамускулно">Интрамускулно</option>
                    <option value="Субкутанно">Субкутанно</option>
                    <option value="Местно">Местно</option>
                    <option value="Инхалаторно">Инхалаторно</option>
                  </select>
                </div>
              </div>
              
              <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-200">Продължителност (дни)</label>
                <input type="number" v-model="newPrescription.duration" placeholder="7" min="1" class="block w-full px-3 py-2 mt-1 placeholder-gray-400 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-purple-500 focus:border-purple-500 sm:text-sm dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white">
              </div>
              
              <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-200">Инструкции</label>
                <textarea v-model="newPrescription.instructions" rows="3" placeholder="Специални инструкции за прием..." class="block w-full px-3 py-2 mt-1 placeholder-gray-400 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-purple-500 focus:border-purple-500 sm:text-sm dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white"></textarea>
              </div>
            </div>
          </div>
          <div class="px-4 py-3 bg-gray-50 dark:bg-gray-700 sm:px-6 sm:flex sm:flex-row-reverse">
            <button @click="addPrescription" :disabled="!newPrescription.medicationName || !newPrescription.dosage" class="inline-flex justify-center w-full px-4 py-2 text-base font-medium text-white bg-purple-600 border border-transparent rounded-md shadow-sm hover:bg-purple-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-purple-500 sm:ml-3 sm:w-auto sm:text-sm disabled:opacity-50 disabled:cursor-not-allowed">Добави</button>
            <button @click="showPrescriptionModal = false" class="inline-flex justify-center w-full px-4 py-2 mt-3 text-base font-medium text-gray-700 bg-white border border-gray-300 rounded-md shadow-sm hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-purple-500 sm:mt-0 sm:ml-3 sm:w-auto sm:text-sm dark:bg-gray-800 dark:text-gray-300 dark:border-gray-600 dark:hover:bg-gray-700">Отказ</button>
          </div>
        </div>
      </div>
    </div>

    <!-- Enhanced Referral Modal -->
    <div v-if="showReferralModal" class="fixed inset-0 z-50 overflow-y-auto" @click.self="showReferralModal = false">
      <div class="flex items-center justify-center min-h-screen px-4 pt-4 pb-20 text-center sm:block sm:p-0">
        <div class="fixed inset-0 transition-opacity bg-gray-500 bg-opacity-75 z-40" @click="showReferralModal = false"></div>
        <div class="relative inline-block overflow-hidden text-left align-bottom transition-all transform bg-white rounded-lg shadow-xl dark:bg-gray-800 sm:my-8 sm:align-middle sm:max-w-2xl sm:w-full z-50">
          <div class="px-4 pt-5 pb-4 bg-white dark:bg-gray-800 sm:p-6 sm:pb-4">
            <div class="flex items-center justify-between mb-4">
              <h3 class="text-lg font-medium leading-6 text-gray-900 dark:text-white">Добави Направление</h3>
              <button @click="showReferralModal = false" class="p-2 text-gray-400 rounded-lg hover:text-gray-600 hover:bg-gray-100 dark:hover:text-gray-300 dark:hover:bg-gray-700">
                <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"></path>
                </svg>
              </button>
            </div>
            
            <!-- Specialty Search -->
            <div class="mb-4">
              <label class="block mb-2 text-sm font-medium text-gray-700 dark:text-gray-200">Търси специалност</label>
              <input 
                type="text" 
                v-model="specialtySearchTerm" 
                @input="searchSpecialties"
                placeholder="Търси медицинска специалност..."
                class="w-full px-3 py-2 placeholder-gray-400 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-purple-500 focus:border-purple-500 sm:text-sm dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white"
              >
            </div>
            
            <!-- Specialty Results -->
            <div v-if="filteredSpecialties.length > 0" class="mb-4 overflow-y-auto border border-gray-200 rounded-md max-h-40 dark:border-gray-600">
              <div 
                v-for="specialty in filteredSpecialties.slice(0, 10)" 
                :key="specialty.name"
                @click="selectSpecialty(specialty)"
                class="p-3 border-b border-gray-100 cursor-pointer hover:bg-purple-50 dark:hover:bg-purple-900 dark:border-gray-700 last:border-b-0"
              >
                <div class="text-sm font-medium text-purple-600 dark:text-purple-300">{{ specialty.name }}</div>
                <div class="text-xs text-gray-500 dark:text-gray-400">{{ specialty.description }}</div>
              </div>
            </div>
            
            <div class="space-y-4">
              <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-200">Специалност *</label>
                <input type="text" v-model="newReferral.specialty" placeholder="Кардиология" class="block w-full px-3 py-2 mt-1 placeholder-gray-400 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-purple-500 focus:border-purple-500 sm:text-sm dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white">
              </div>
              
              <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-200">Приоритет</label>
                <select v-model="newReferral.priority" class="block w-full px-3 py-2 mt-1 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-purple-500 focus:border-purple-500 sm:text-sm dark:bg-gray-700 dark:border-gray-600 dark:text-white">
                  <option value="обичаен">Обичаен</option>
                  <option value="спешен">Спешен</option>
                  <option value="спешно">Спешно</option>
                </select>
              </div>
              
              <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-200">Причина за Направление *</label>
                <textarea v-model="newReferral.reason" rows="4" placeholder="Описание на причината за направлението..." class="block w-full px-3 py-2 mt-1 placeholder-gray-400 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-purple-500 focus:border-purple-500 sm:text-sm dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white"></textarea>
              </div>
              
              <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-200">Предложена Дата</label>
                <input type="date" v-model="newReferral.suggestedDate" class="block w-full px-3 py-2 mt-1 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-purple-500 focus:border-purple-500 sm:text-sm dark:bg-gray-700 dark:border-gray-600 dark:text-white">
              </div>
            </div>
          </div>
          <div class="px-4 py-3 bg-gray-50 dark:bg-gray-700 sm:px-6 sm:flex sm:flex-row-reverse">
            <button @click="addReferral" :disabled="!newReferral.specialty || !newReferral.reason" class="inline-flex justify-center w-full px-4 py-2 text-base font-medium text-white bg-purple-600 border border-transparent rounded-md shadow-sm hover:bg-purple-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-purple-500 sm:ml-3 sm:w-auto sm:text-sm disabled:opacity-50 disabled:cursor-not-allowed">Добави</button>
            <button @click="showReferralModal = false" class="inline-flex justify-center w-full px-4 py-2 mt-3 text-base font-medium text-gray-700 bg-white border border-gray-300 rounded-md shadow-sm hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-purple-500 sm:mt-0 sm:ml-3 sm:w-auto sm:text-sm dark:bg-gray-800 dark:text-gray-300 dark:border-gray-600 dark:hover:bg-gray-700">Отказ</button>
          </div>
        </div>
      </div>
    </div>

    <!-- Print Template (Hidden) -->
    <div ref="printTemplate" class="print-template" style="display: none;">
      <div class="print-header">
        <h1>АМБУЛАТОРЕН ЛИСТ</h1>
        <p>Дата: {{ formatDate(new Date()) }}</p>
      </div>
      
      <div class="print-section">
        <h2>Данни за Пациента</h2>
        <p><strong>Име:</strong> {{ patient?.firstName }} {{ patient?.lastName }}</p>
        <p><strong>ЕГН:</strong> {{ outpatientRecord.patientEGN || 'N/A' }}</p>
        <p><strong>Възраст:</strong> {{ calculateAge(patient?.dateOfBirth) }} години</p>
      </div>

      <div class="print-section">
        <h2>Данни за Лекаря</h2>
        <p><strong>Лекар:</strong> {{ currentUser?.firstName }} {{ currentUser?.lastName }}</p>
        <p><strong>Специалност:</strong> {{ currentUser?.department || 'N/A' }}</p>
      </div>

      <div class="print-section">
        <h2>Оплаквания</h2>
        <p>{{ outpatientRecord.chiefComplaint || 'Няма данни' }}</p>
      </div>

      <div class="print-section">
        <h2>Анамнеза</h2>
        <p>{{ outpatientRecord.medicalHistory || 'Няма данни' }}</p>
      </div>

      <div class="print-section">
        <h2>Обективно Изследване</h2>
        <p>{{ outpatientRecord.physicalExamination || 'Няма данни' }}</p>
      </div>

      <div class="print-section">
        <h2>Витални Показатели</h2>
        <p><strong>Кръвно Налягане:</strong> {{ vitalSigns.bloodPressure || 'N/A' }}</p>
        <p><strong>Пулс:</strong> {{ vitalSigns.pulse || 'N/A' }}</p>
        <p><strong>Температура:</strong> {{ vitalSigns.temperature || 'N/A' }}</p>
        <p><strong>Дишане:</strong> {{ vitalSigns.respiration || 'N/A' }}</p>
      </div>

      <div class="print-section">
        <h2>Диагнози</h2>
        <div v-for="diagnosis in diagnoses" :key="diagnosis.id" class="print-item">
          <p><strong>{{ diagnosis.icd10Code }}:</strong> {{ diagnosis.diagnosisName }}</p>
          <p v-if="diagnosis.clinicalFindings"><em>{{ diagnosis.clinicalFindings }}</em></p>
        </div>
      </div>

      <div class="print-section">
        <h2>Предписани Лекарства</h2>
        <div v-for="prescription in prescriptions" :key="prescription.id" class="print-item">
          <p><strong>{{ prescription.medicationName }}</strong></p>
          <p>Дозировка: {{ prescription.dosage }} | Честота: {{ prescription.frequency }}</p>
          <p>Начин: {{ prescription.route }} | Продължителност: {{ prescription.duration }} дни</p>
          <p v-if="prescription.instructions">Инструкции: {{ prescription.instructions }}</p>
        </div>
      </div>

      <div class="print-section">
        <h2>Направления</h2>
        <div v-for="referral in referrals" :key="referral.id" class="print-item">
          <p><strong>{{ referral.specialty }}</strong></p>
          <p>Причина: {{ referral.reason }}</p>
        </div>
      </div>

      <div class="print-section">
        <h2>План за Лечение</h2>
        <p>{{ outpatientRecord.treatmentPlan || 'Няма данни' }}</p>
      </div>

      <div class="print-footer">
        <p>Лекар: _______________________</p>
        <p>Подпис и Печат</p>
      </div>
    </div>
  </div>
</div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useRoute } from 'vue-router'
import api from '../../services/api'
import { useAuthStore } from '../../stores/auth'

const route = useRoute()
const authStore = useAuthStore()
const currentUser = computed(() => authStore.user)

// Data
const appointmentId = ref(route.params.id as string)
const patient = ref<any>(null)
const isSaving = ref(false)

// Outpatient Record
const outpatientRecord = ref({
  appointmentId: appointmentId.value,
  patientId: '',
  patientEGN: '',
  chiefComplaint: '',
  medicalHistory: '',
  physicalExamination: '',
  vitalSigns: '',
  treatmentPlan: '',
  notes: ''
})

// Vital Signs (separate for better UX)
const vitalSigns = ref({
  bloodPressure: '',
  pulse: '',
  temperature: '',
  respiration: ''
})

// Medical Data Lists
const diagnoses = ref<any[]>([])
const prescriptions = ref<any[]>([])
const referrals = ref<any[]>([])

// Modals
const showDiagnosisModal = ref(false)
const showPrescriptionModal = ref(false)
const showReferralModal = ref(false)

// Enhanced search functionality for modals
const diagnosisSearchTerm = ref('');
const medicationSearchTerm = ref('');
const specialtySearchTerm = ref('');

// Comprehensive MKB-10 Diagnoses Database
const mkb10Diagnoses = ref([
  { code: 'I10', name: 'Есенциална хипертония' },
  { code: 'E11', name: 'Диабет мелитус тип 2' },
  { code: 'J44', name: 'Хронична обструктивна белодробна болест' },
  { code: 'M79', name: 'Други нарушения на меките тъкани' },
  { code: 'K59', name: 'Други функционални чревни нарушения' },
  { code: 'R06', name: 'Нарушения на дишането' },
  { code: 'R50', name: 'Треска, некласифицирана другаде' },
  { code: 'Z51', name: 'Други медицински грижи' },
  { code: 'N39', name: 'Други нарушения на пикочно-отделителната система' },
  { code: 'F32', name: 'Депресивен епизод' },
  { code: 'G43', name: 'Мигрена' },
  { code: 'I25', name: 'Хронична исхемична болест на сърцето' },
  { code: 'J06', name: 'Остри инфекции на горните дихателни пътища' },
  { code: 'K21', name: 'Гастроезофагеален рефлукс' },
  { code: 'M54', name: 'Болка в гърба' },
  { code: 'N18', name: 'Хронична бъбречна недостатъчност' },
  { code: 'Z00', name: 'Общ медицински преглед' },
  { code: 'A09', name: 'Диария и гастроентерит' },
  { code: 'B34', name: 'Вирусна инфекция' },
  { code: 'C78', name: 'Вторичен злокачествен тумор' },
]);

// Comprehensive Medications Database
const medications = ref([
  { name: 'Парацетамол', activeIngredient: 'Paracetamol', form: 'таблетки 500mg' },
  { name: 'Ибупрофен', activeIngredient: 'Ibuprofen', form: 'таблетки 400mg' },
  { name: 'Лозартан', activeIngredient: 'Losartan', form: 'таблетки 50mg' },
  { name: 'Метформин', activeIngredient: 'Metformin', form: 'таблетки 850mg' },
  { name: 'Омепразол', activeIngredient: 'Omeprazole', form: 'капсули 20mg' },
  { name: 'Симвастатин', activeIngredient: 'Simvastatin', form: 'таблетки 20mg' },
  { name: 'Амоксицилин', activeIngredient: 'Amoxicillin', form: 'капсули 500mg' },
  { name: 'Салбутамол', activeIngredient: 'Salbutamol', form: 'инхалатор 100mcg' },
  { name: 'Диазепам', activeIngredient: 'Diazepam', form: 'таблетки 5mg' },
  { name: 'Фуросемид', activeIngredient: 'Furosemide', form: 'таблетки 40mg' },
  { name: 'Преднизолон', activeIngredient: 'Prednisolone', form: 'таблетки 5mg' },
  { name: 'Варфарин', activeIngredient: 'Warfarin', form: 'таблетки 5mg' },
  { name: 'Инсулин', activeIngredient: 'Insulin', form: 'инжекция' },
  { name: 'Дигоксин', activeIngredient: 'Digoxin', form: 'таблетки 0.25mg' },
  { name: 'Лизиноприл', activeIngredient: 'Lisinopril', form: 'таблетки 10mg' },
]);

// Medical Specialties Database
const medicalSpecialties = ref([
  { name: 'Кардиология', description: 'Заболявания на сърцето и съдовете' },
  { name: 'Неврология', description: 'Заболявания на нервната система' },
  { name: 'Ортопедия', description: 'Заболявания на костно-мускулната система' },
  { name: 'Гастроентерология', description: 'Заболявания на храносмилателната система' },
  { name: 'Дерматология', description: 'Заболявания на кожата' },
  { name: 'Офталмология', description: 'Заболявания на очите' },
  { name: 'Ендокринология', description: 'Заболявания на ендокринната система' },
  { name: 'Пулмология', description: 'Заболявания на белите дробове' },
  { name: 'Психиатрия', description: 'Психични заболявания' },
  { name: 'Урология', description: 'Заболявания на пикочно-половата система' },
  { name: 'Ревматология', description: 'Ревматични заболявания' },
  { name: 'Онкология', description: 'Онкологични заболявания' },
  { name: 'Инфекциозни болести', description: 'Инфекциозни заболявания' },
  { name: 'Хематология', description: 'Заболявания на кръвта' },
]);

// Computed filtered arrays for search functionality
const filteredDiagnoses = computed(() => {
  if (!diagnosisSearchTerm.value) return mkb10Diagnoses.value;
  const term = diagnosisSearchTerm.value.toLowerCase();
  return mkb10Diagnoses.value.filter(diagnosis => 
    diagnosis.code.toLowerCase().includes(term) || 
    diagnosis.name.toLowerCase().includes(term)
  );
});

const filteredMedications = computed(() => {
  if (!medicationSearchTerm.value) return medications.value;
  const term = medicationSearchTerm.value.toLowerCase();
  return medications.value.filter(medication => 
    medication.name.toLowerCase().includes(term) || 
    medication.activeIngredient.toLowerCase().includes(term)
  );
});

const filteredSpecialties = computed(() => {
  if (!specialtySearchTerm.value) return medicalSpecialties.value;
  const term = specialtySearchTerm.value.toLowerCase();
  return medicalSpecialties.value.filter(specialty => 
    specialty.name.toLowerCase().includes(term) || 
    specialty.description.toLowerCase().includes(term)
  );
});

// New Items
const newDiagnosis = ref({
  icd10Code: '',
  diagnosisName: '',
  diagnosisType: 'primary',
  clinicalFindings: '',
  notes: ''
})

const newPrescription = ref({
  medicationName: '',
  dosage: '',
  frequency: '2xдневно',
  instructions: '',
  duration: 7,
  route: 'Орално',
  quantity: 0
})

const newReferral = ref({
  specialty: '',
  reason: '',
  expiryDate: '',
  notes: '',
  priority: 'обичаен',
  suggestedDate: ''
})

// Computed
const isPrescriptionValid = computed(() => {
  return newPrescription.value.medicationName && 
         newPrescription.value.dosage && 
         newPrescription.value.frequency && 
         newPrescription.value.route && 
         newPrescription.value.duration > 0 &&
         newPrescription.value.quantity > 0
})

const isReferralValid = computed(() => {
  return newReferral.value.specialty && newReferral.value.reason
})

// Methods
const fetchAppointmentData = async () => {
  try {
    const response = await api.get(`/appointments/${appointmentId.value}`)
    const appointment = response.data
    
    // Fetch patient data
    const patientResponse = await api.get(`/patients/${appointment.patientId}`)
    patient.value = patientResponse.data
    outpatientRecord.value.patientId = patient.value.id
    
    // Load existing medical records
    await loadMedicalRecords()
  } catch (error) {
    console.error('Error fetching appointment data:', error)
  }
}

const loadMedicalRecords = async () => {
  try {
    // Load diagnoses with error handling
    try {
      const diagnosesRes = await api.get(`/medical-records/diagnoses/appointment/${appointmentId.value}`)
      diagnoses.value = diagnosesRes.data
    } catch (error) {
      console.warn('Diagnoses endpoint not available, using empty array')
      diagnoses.value = []
    }
    
    // Load prescriptions with error handling
    try {
      const prescriptionsRes = await api.get(`/medical-records/prescriptions/appointment/${appointmentId.value}`)
      prescriptions.value = prescriptionsRes.data
    } catch (error) {
      console.warn('Prescriptions endpoint not available, using empty array')
      prescriptions.value = []
    }
    
    // Load referrals with error handling
    try {
      const referralsRes = await api.get(`/medical-records/referrals/appointment/${appointmentId.value}`)
      referrals.value = referralsRes.data
    } catch (error) {
      console.warn('Referrals endpoint not available, using empty array')
      referrals.value = []
    }
    
    // Load outpatient record with error handling
    try {
      const recordRes = await api.get(`/medical-records/outpatient-records/appointment/${appointmentId.value}`)
      if (recordRes.data.record) {
        outpatientRecord.value = recordRes.data.record
        // Parse vital signs
        if (outpatientRecord.value.vitalSigns) {
          try {
            const parsed = JSON.parse(outpatientRecord.value.vitalSigns)
            vitalSigns.value = parsed
          } catch (e) {
            console.error('Error parsing vital signs:', e)
          }
        }
      }
    } catch (error) {
      console.warn('Outpatient record endpoint not available')
      // Initialize with default structure
      outpatientRecord.value = {
        appointmentId: appointmentId.value,
        patientId: '',
        patientEGN: '',
        chiefComplaint: '',
        medicalHistory: '',
        physicalExamination: '',
        vitalSigns: '',
        treatmentPlan: '',
        notes: ''
      }
    }
  } catch (error: any) {
    if (error.response?.status !== 404) {
      console.error('Error loading medical records:', error)
    }
  }
}

// Enhanced search and selection functions
const searchDiagnoses = () => {
  // Search is handled by computed property filteredDiagnoses
}

const selectDiagnosis = (diagnosis: any) => {
  newDiagnosis.value.icd10Code = diagnosis.code;
  newDiagnosis.value.diagnosisName = diagnosis.name;
  diagnosisSearchTerm.value = '';
}

const searchMedications = () => {
  // Search is handled by computed property filteredMedications
}

const selectMedication = (medication: any) => {
  newPrescription.value.medicationName = medication.name;
  medicationSearchTerm.value = '';
}

const searchSpecialties = () => {
  // Search is handled by computed property filteredSpecialties
}

const selectSpecialty = (specialty: any) => {
  newReferral.value.specialty = specialty.name;
  specialtySearchTerm.value = '';
}

const addDiagnosis = async () => {
  try {
    const response = await api.post('/medical-records/diagnoses', {
      appointmentId: appointmentId.value,
      patientId: outpatientRecord.value.patientId,
      ...newDiagnosis.value
    })
    diagnoses.value.push(response.data.diagnosis)
  } catch (error) {
    console.warn('API not available, adding diagnosis locally:', error)
    // Add diagnosis locally for demo purposes
    const localDiagnosis = {
      id: Date.now().toString(),
      appointmentId: appointmentId.value,
      patientId: outpatientRecord.value.patientId,
      ...newDiagnosis.value,
      createdAt: new Date().toISOString()
    }
    diagnoses.value.push(localDiagnosis)
  }
  showDiagnosisModal.value = false
  resetNewDiagnosis()
}

const addPrescription = async () => {
  try {
    const response = await api.post('/medical-records/prescriptions', {
      appointmentId: appointmentId.value,
      patientId: outpatientRecord.value.patientId,
      ...newPrescription.value
    })
    prescriptions.value.push(response.data.prescription)
  } catch (error) {
    console.warn('API not available, adding prescription locally:', error)
    // Add prescription locally for demo purposes
    const localPrescription = {
      id: Date.now().toString(),
      appointmentId: appointmentId.value,
      patientId: outpatientRecord.value.patientId,
      ...newPrescription.value,
      createdAt: new Date().toISOString()
    }
    prescriptions.value.push(localPrescription)
  }
  showPrescriptionModal.value = false
  resetNewPrescription()
}

const addReferral = async () => {
  try {
    const response = await api.post('/medical-records/referrals', {
      appointmentId: appointmentId.value,
      patientId: outpatientRecord.value.patientId,
      ...newReferral.value
    })
    referrals.value.push(response.data.referral)
  } catch (error) {
    console.warn('API not available, adding referral locally:', error)
    // Add referral locally for demo purposes
    const localReferral = {
      id: Date.now().toString(),
      appointmentId: appointmentId.value,
      patientId: outpatientRecord.value.patientId,
      ...newReferral.value,
      createdAt: new Date().toISOString()
    }
    referrals.value.push(localReferral)
  }
  showReferralModal.value = false
  resetNewReferral()
}

const removeDiagnosis = (id: string) => {
  diagnoses.value = diagnoses.value.filter(d => d.id !== id)
}

const removePrescription = (id: string) => {
  prescriptions.value = prescriptions.value.filter(p => p.id !== id)
}

const removeReferral = (id: string) => {
  referrals.value = referrals.value.filter(r => r.id !== id)
}

const saveAll = async () => {
  try {
    isSaving.value = true
    
    // Combine vital signs into JSON string
    outpatientRecord.value.vitalSigns = JSON.stringify(vitalSigns.value)
    
    // Save/update outpatient record
    await api.post('/medical-records/outpatient-records', outpatientRecord.value)
    
    alert('Всичко е запазено успешно!')
  } catch (error) {
    console.error('Error saving records:', error)
    alert('Грешка при запазване на данните')
  } finally {
    isSaving.value = false
  }
}

const printOutpatientRecord = () => {
  const printContent = (document.querySelector('.print-template') as HTMLElement)?.innerHTML
  const originalContent = document.body.innerHTML
  
  document.body.innerHTML = printContent
  window.print()
  document.body.innerHTML = originalContent
  window.location.reload() // Reload to restore Vue app
}

const calculateAge = (dateOfBirth: string | undefined) => {
  if (!dateOfBirth) return 'N/A'
  const today = new Date()
  const birthDate = new Date(dateOfBirth)
  let age = today.getFullYear() - birthDate.getFullYear()
  const monthDiff = today.getMonth() - birthDate.getMonth()
  if (monthDiff < 0 || (monthDiff === 0 && today.getDate() < birthDate.getDate())) {
    age--
  }
  return age
}

const formatDate = (date: string | Date) => {
  if (!date) return 'N/A'
  return new Date(date).toLocaleDateString('bg-BG')
}

const resetNewDiagnosis = () => {
  newDiagnosis.value = {
    icd10Code: '',
    diagnosisName: '',
    diagnosisType: 'primary',
    clinicalFindings: '',
    notes: ''
  }
}

const resetNewPrescription = () => {
  newPrescription.value = {
    medicationName: '',
    dosage: '',
    frequency: '',
    route: 'oral',
    duration: 7,
    quantity: 0,
    instructions: ''
  }
}

const resetNewReferral = () => {
  newReferral.value = {
    specialty: '',
    reason: '',
    expiryDate: '',
    notes: '',
    priority: 'обичаен',
    suggestedDate: ''
  }
}

const formatDiagnosisType = (type: string) => {
  const types: Record<string, string> = {
    'primary': 'Основна',
    'secondary': 'Съпътстваща',
    'differential': 'Диференциална'
  }
  return types[type] || type
}

const formatRoute = (route: string) => {
  const routes: Record<string, string> = {
    'oral': 'Перорално',
    'injection': 'Инжекция',
    'topical': 'Местно',
    'inhalation': 'Инхалация',
    'other': 'Друго'
  }
  return routes[route] || route
}

const aiAssist = async (field: string) => {
  // TODO: Implement AI assistance
  alert(`AI помощ за поле: ${field} - Функционалността ще бъде добавена скоро!`)
}

onMounted(() => {
  fetchAppointmentData()
})
</script>

<style scoped>
/* Enhanced Header Styles */
.page-header {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  border-radius: 20px;
  padding: 32px;
  margin-bottom: 24px;
  box-shadow: 0 8px 32px rgba(102, 126, 234, 0.3);
  display: flex;
  flex-direction: column;
  gap: 20px;
  color: white;
}

/* Breadcrumb Navigation */
.breadcrumb {
  display: flex;
  align-items: center;
  gap: 8px;
  margin-bottom: 16px;
}

.breadcrumb-link {
  background: rgba(255, 255, 255, 0.1);
  color: white;
  border: none;
  padding: 8px 12px;
  border-radius: 8px;
  display: flex;
  align-items: center;
  gap: 6px;
  font-size: 14px;
  cursor: pointer;
  transition: all 0.3s;
  backdrop-filter: blur(10px);
}

.breadcrumb-link:hover {
  background: rgba(255, 255, 255, 0.2);
  transform: translateY(-2px);
}

.breadcrumb-separator {
  color: rgba(255, 255, 255, 0.6);
  font-size: 12px;
}

.breadcrumb-current {
  display: flex;
  align-items: center;
  gap: 6px;
  font-size: 14px;
  font-weight: 600;
  color: rgba(255, 255, 255, 0.9);
}

/* Enhanced Header Layout */
.header-left {
  display: flex;
  flex-direction: column;
  flex: 1;
}

/* Patient Info Redesign */
.patient-info {
  display: flex;
  align-items: center;
  gap: 20px;
}

.patient-avatar {
  position: relative;
  flex-shrink: 0;
}

.avatar-circle {
  width: 64px;
  height: 64px;
  background: linear-gradient(135deg, #ffd89b 0%, #19547b 100%);
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
  font-size: 24px;
  box-shadow: 0 4px 16px rgba(0, 0, 0, 0.2);
}

.patient-status {
  position: absolute;
  bottom: 4px;
  right: 4px;
  width: 16px;
  height: 16px;
  border-radius: 50%;
  border: 3px solid white;
}

.patient-status.online {
  background: #10b981;
}

.patient-details h1 {
  margin: 0 0 8px 0;
  font-size: 28px;
  font-weight: 700;
  text-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

.patient-meta {
  display: flex;
  flex-wrap: wrap;
  gap: 16px;
  margin: 0;
}

.meta-item {
  display: flex;
  align-items: center;
  gap: 6px;
  font-size: 14px;
  color: rgba(255, 255, 255, 0.9);
  background: rgba(255, 255, 255, 0.1);
  padding: 6px 12px;
  border-radius: 20px;
  backdrop-filter: blur(10px);
}

.meta-item i {
  font-size: 13px;
}

/* Header Actions */
.header-actions {
  display: flex;
  align-items: flex-end;
}

.action-group {
  display: flex;
  gap: 12px;
}

.btn-save-header,
.btn-print-header,
.btn-back-header {
  padding: 12px 20px;
  border: none;
  border-radius: 12px;
  cursor: pointer;
  font-weight: 600;
  display: flex;
  align-items: center;
  gap: 8px;
  transition: all 0.3s;
  backdrop-filter: blur(10px);
  font-size: 14px;
  min-width: 120px;
  justify-content: center;
}

.btn-save-header {
  background: rgba(16, 185, 129, 0.9);
  color: white;
  box-shadow: 0 4px 16px rgba(16, 185, 129, 0.3);
}

.btn-save-header:hover:not(:disabled) {
  background: rgba(16, 185, 129, 1);
  transform: translateY(-2px);
  box-shadow: 0 6px 20px rgba(16, 185, 129, 0.4);
}

.btn-save-header:disabled {
  opacity: 0.6;
  cursor: not-allowed;
  transform: none;
}

.btn-print-header {
  background: rgba(59, 130, 246, 0.9);
  color: white;
  box-shadow: 0 4px 16px rgba(59, 130, 246, 0.3);
}

.btn-print-header:hover {
  background: rgba(59, 130, 246, 1);
  transform: translateY(-2px);
  box-shadow: 0 6px 20px rgba(59, 130, 246, 0.4);
}

.btn-back-header {
  background: rgba(255, 255, 255, 0.1);
  color: white;
  border: 2px solid rgba(255, 255, 255, 0.2);
}

.btn-back-header:hover {
  background: rgba(255, 255, 255, 0.2);
  transform: translateY(-2px);
}

/* Content Grid */
.content-grid {
  display: grid;
  grid-template-columns: 1fr 500px;
  gap: 24px;
  max-width: 1600px;
  margin: 0 auto;
}

/* Enhanced Cards */
.card {
  background: white;
  border-radius: 20px;
  padding: 0;
  margin-bottom: 24px;
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.08);
  transition: all 0.3s ease;
  border: 1px solid rgba(102, 126, 234, 0.1);
  overflow: hidden;
}

.card:hover {
  box-shadow: 0 8px 30px rgba(102, 126, 234, 0.15);
  transform: translateY(-2px);
  border-color: rgba(102, 126, 234, 0.2);
}

.card-header {
  display: flex;
  align-items: center;
  gap: 12px;
  padding: 24px 24px 0 24px;
  margin-bottom: 20px;
  position: relative;
}

.card-header::after {
  content: '';
  position: absolute;
  bottom: -10px;
  left: 24px;
  right: 24px;
  height: 2px;
  background: linear-gradient(90deg, #667eea 0%, #764ba2 50%, transparent 100%);
}

.card-header i {
  color: #667eea;
  font-size: 20px;
  width: 24px;
  text-align: center;
}

.card-header h2 {
  margin: 0;
  color: #2c3e50;
  font-size: 18px;
  font-weight: 600;
}

.card-body {
  padding: 0 24px 24px 24px;
}

.header-title {
  display: flex;
  align-items: center;
  gap: 12px;
  flex: 1;
}

/* Enhanced Textarea with AI */
.textarea-with-ai {
  position: relative;
  margin-bottom: 16px;
}

.textarea-with-ai textarea {
  width: 100%;
  padding: 16px;
  padding-right: 60px;
  border: 2px solid #f1f5f9;
  border-radius: 16px;
  font-family: inherit;
  font-size: 14px;
  line-height: 1.6;
  resize: vertical;
  transition: all 0.3s;
  background: #fafbfc;
  min-height: 120px;
}

.textarea-with-ai textarea:focus {
  outline: none;
  border-color: #667eea;
  box-shadow: 0 0 0 4px rgba(102, 126, 234, 0.1);
  background: white;
}

.textarea-with-ai textarea::placeholder {
  color: #94a3b8;
  font-style: italic;
}

.ai-assist-btn {
  position: absolute;
  top: 12px;
  right: 12px;
  background: linear-gradient(135deg, #fbbf24 0%, #f59e0b 100%);
  border: none;
  width: 40px;
  height: 40px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  color: white;
  transition: all 0.3s;
  box-shadow: 0 4px 12px rgba(251, 191, 36, 0.4);
}

.ai-assist-btn:hover {
  transform: scale(1.1) rotate(15deg);
  box-shadow: 0 6px 20px rgba(251, 191, 36, 0.6);
}

.ai-assist-btn i {
  font-size: 18px;
  animation: sparkle 3s infinite ease-in-out;
}

@keyframes sparkle {
  0%, 100% { 
    opacity: 1; 
    transform: scale(1);
  }
  50% { 
    opacity: 0.7; 
    transform: scale(1.1);
  }
}

/* Enhanced Vital Signs */
.vital-signs-grid {
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  gap: 16px;
  margin-bottom: 20px;
}

.vital-input {
  background: linear-gradient(135deg, #f8fafc 0%, #f1f5f9 100%);
  padding: 16px;
  border-radius: 16px;
  border: 2px solid transparent;
  transition: all 0.3s;
  position: relative;
  overflow: hidden;
}

.vital-input::before {
  content: '';
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  height: 3px;
  background: linear-gradient(90deg, #667eea 0%, #764ba2 100%);
  transform: scaleX(0);
  transition: transform 0.3s;
}

.vital-input:hover {
  border-color: #667eea;
  background: white;
  transform: translateY(-2px);
  box-shadow: 0 8px 25px rgba(102, 126, 234, 0.15);
}

.vital-input:hover::before {
  transform: scaleX(1);
}

.vital-input label {
  display: flex;
  align-items: center;
  gap: 8px;
  font-size: 13px;
  font-weight: 600;
  color: #495057;
  margin-bottom: 8px;
}

.vital-input label i {
  color: #667eea;
  font-size: 14px;
}

.vital-input input {
  width: 100%;
  padding: 12px;
  border: 1px solid #e2e8f0;
  border-radius: 10px;
  font-size: 14px;
  transition: all 0.3s;
  background: white;
}

.vital-input input:focus {
  outline: none;
  border-color: #667eea;
  box-shadow: 0 0 0 3px rgba(102, 126, 234, 0.1);
}

/* Enhanced Action Cards */
.action-card .card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.btn-add {
  background: linear-gradient(135deg, #10b981 0%, #059669 100%);
  color: white;
  border: none;
  width: 40px;
  height: 40px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  transition: all 0.3s;
  box-shadow: 0 4px 12px rgba(16, 185, 129, 0.3);
  position: relative;
  overflow: hidden;
}

.btn-add::before {
  content: '';
  position: absolute;
  top: 50%;
  left: 50%;
  width: 0;
  height: 0;
  background: rgba(255, 255, 255, 0.2);
  border-radius: 50%;
  transform: translate(-50%, -50%);
  transition: all 0.3s;
}

.btn-add:hover {
  transform: scale(1.1);
  box-shadow: 0 6px 20px rgba(16, 185, 129, 0.5);
}

.btn-add:hover::before {
  width: 100%;
  height: 100%;
}

.btn-add:active {
  transform: scale(0.95);
}

/* Enhanced Items List */
.items-list {
  display: flex;
  flex-direction: column;
  gap: 16px;
  max-height: 500px;
  overflow-y: auto;
  padding-right: 8px;
}

.items-list::-webkit-scrollbar {
  width: 6px;
}

.items-list::-webkit-scrollbar-track {
  background: #f1f3f5;
  border-radius: 10px;
}

.items-list::-webkit-scrollbar-thumb {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  border-radius: 10px;
}

.item-card {
  position: relative;
  background: linear-gradient(135deg, #f8fafc 0%, #f1f5f9 100%);
  padding: 20px;
  border-radius: 16px;
  border: 2px solid transparent;
  transition: all 0.3s;
  overflow: hidden;
}

.item-card::before {
  content: '';
  position: absolute;
  top: 0;
  left: 0;
  width: 4px;
  height: 100%;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  transform: scaleY(0);
  transition: transform 0.3s;
}

.item-card:hover {
  border-color: #667eea;
  transform: translateX(8px);
  box-shadow: 0 8px 25px rgba(102, 126, 234, 0.15);
  background: white;
}

.item-card:hover::before {
  transform: scaleY(1);
}

.btn-remove {
  position: absolute;
  top: 12px;
  right: 12px;
  background: #ff6b6b;
  color: white;
  border: none;
  width: 28px;
  height: 28px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  opacity: 0;
  transition: all 0.3s;
}

.item-card:hover .btn-remove {
  opacity: 1;
}

.btn-remove:hover {
  background: #ff5252;
  transform: scale(1.1);
}

/* Diagnosis Card */
.diagnosis-card h4 {
  margin: 8px 0;
  color: #2c3e50;
  font-size: 16px;
}

.item-badges {
  display: flex;
  gap: 8px;
  margin-bottom: 8px;
}

.badge {
  padding: 4px 12px;
  border-radius: 20px;
  font-size: 12px;
  font-weight: 600;
}

.badge-primary {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
}

.badge-secondary {
  background: #e9ecef;
  color: #495057;
}

.badge-info {
  background: linear-gradient(135deg, #4facfe 0%, #00f2fe 100%);
  color: white;
}

.item-detail {
  margin: 8px 0 0 0;
  font-size: 13px;
  color: #6c757d;
  line-height: 1.5;
}

/* Prescription Card */
.prescription-card h4 {
  display: flex;
  align-items: center;
  gap: 8px;
  margin: 0 0 12px 0;
  color: #2c3e50;
  font-size: 16px;
}

.prescription-card h4 i {
  color: #667eea;
}

.prescription-details {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.detail-row {
  display: flex;
  justify-content: space-between;
  align-items: center;
  font-size: 13px;
}

.detail-label {
  color: #6c757d;
  font-weight: 600;
}

.prescription-instructions {
  margin-top: 12px;
  padding: 10px;
  background: #fff3cd;
  border-left: 3px solid #ffc107;
  border-radius: 6px;
  font-size: 13px;
  color: #856404;
  display: flex;
  align-items: start;
  gap: 8px;
}

.prescription-instructions i {
  margin-top: 2px;
}

/* Referral Card */
.referral-card h4 {
  display: flex;
  align-items: center;
  gap: 8px;
  margin: 0 0 12px 0;
  color: #2c3e50;
  font-size: 16px;
}

.referral-card h4 i {
  color: #667eea;
}

.referral-expiry {
  margin-top: 8px;
  padding: 8px;
  background: #d1ecf1;
  border-radius: 6px;
  font-size: 12px;
  color: #0c5460;
  display: flex;
  align-items: center;
  gap: 6px;
}

/* Empty State */
.empty-state {
  text-align: center;
  padding: 40px 20px;
  color: #adb5bd;
}

.empty-state i {
  font-size: 48px;
  margin-bottom: 12px;
  opacity: 0.5;
}

.empty-state p {
  margin: 0;
  font-size: 14px;
}

/* Modal Styles */
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(0, 0, 0, 0.6);
  backdrop-filter: blur(4px);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
  animation: fadeIn 0.3s;
}

@keyframes fadeIn {
  from { opacity: 0; }
  to { opacity: 1; }
}

.modal-content {
  background: white;
  border-radius: 20px;
  max-width: 600px;
  width: 90%;
  max-height: 90vh;
  overflow-y: auto;
  animation: slideUp 0.3s;
  box-shadow: 0 20px 60px rgba(0, 0, 0, 0.3);
}

@keyframes slideUp {
  from { 
    transform: translateY(50px);
    opacity: 0;
  }
  to { 
    transform: translateY(0);
    opacity: 1;
  }
}

.modal-header {
  padding: 24px;
  border-bottom: 2px solid #f8f9fa;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.modal-header h3 {
  margin: 0;
  color: #2c3e50;
  font-size: 20px;
}

.btn-close {
  background: #f8f9fa;
  border: none;
  width: 36px;
  height: 36px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 20px;
  color: #6c757d;
  cursor: pointer;
  transition: all 0.3s;
}

.btn-close:hover {
  background: #ff6b6b;
  color: white;
  transform: rotate(90deg);
}

.modal-body {
  padding: 24px;
}

.form-group {
  margin-bottom: 20px;
}

.form-group label {
  display: block;
  margin-bottom: 8px;
  color: #2c3e50;
  font-weight: 600;
  font-size: 14px;
}

.form-group input,
.form-group select,
.form-group textarea {
  width: 100%;
  padding: 12px;
  border: 2px solid #e9ecef;
  border-radius: 10px;
  font-family: inherit;
  font-size: 14px;
  transition: all 0.3s;
}

.form-group input:focus,
.form-group select:focus,
.form-group textarea:focus {
  outline: none;
  border-color: #667eea;
  box-shadow: 0 0 0 3px rgba(102, 126, 234, 0.1);
}

.modal-footer {
  padding: 24px;
  border-top: 2px solid #f8f9fa;
  display: flex;
  justify-content: flex-end;
  gap: 12px;
}

.btn-primary,
.btn-secondary {
  padding: 12px 24px;
  border: none;
  border-radius: 10px;
  cursor: pointer;
  font-size: 14px;
  font-weight: 600;
  transition: all 0.3s;
}

.btn-primary {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
}

.btn-primary:hover:not(:disabled) {
  transform: translateY(-2px);
  box-shadow: 0 6px 12px rgba(102, 126, 234, 0.4);
}

.btn-primary:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.btn-secondary {
  background: #e9ecef;
  color: #495057;
}

.btn-secondary:hover {
  background: #dee2e6;
}

/* Enhanced Responsive Design */
@media (max-width: 1200px) {
  .content-grid {
    grid-template-columns: 1fr;
    gap: 20px;
  }
}

@media (max-width: 768px) {
  .page-header {
    padding: 20px;
    border-radius: 16px;
  }
  
  .breadcrumb {
    flex-wrap: wrap;
    gap: 6px;
  }
  
  .breadcrumb-link {
    padding: 6px 10px;
    font-size: 13px;
  }
  
  .patient-info {
    flex-direction: column;
    align-items: flex-start;
    gap: 12px;
  }
  
  .patient-details h1 {
    font-size: 22px;
  }
  
  .patient-meta {
    flex-direction: column;
    gap: 8px;
  }
  
  .action-group {
    flex-direction: column;
    width: 100%;
  }
  
  .btn-save-header,
  .btn-print-header,
  .btn-back-header {
    width: 100%;
    min-width: auto;
  }
  
  .vital-signs-grid {
    grid-template-columns: 1fr;
  }
  
  .card {
    border-radius: 16px;
    margin-bottom: 16px;
  }
  
  .card-header {
    padding: 20px 20px 0 20px;
  }
  
  .card-body {
    padding: 0 20px 20px 20px;
  }
  
  .textarea-with-ai textarea {
    min-height: 100px;
    font-size: 16px; /* Prevents zoom on iOS */
  }
}

@media (max-width: 480px) {
  .page-header {
    padding: 16px;
  }
  
  .patient-avatar .avatar-circle {
    width: 48px;
    height: 48px;
    font-size: 20px;
  }
  
  .patient-details h1 {
    font-size: 20px;
  }
  
  .meta-item {
    font-size: 13px;
    padding: 4px 8px;
  }
  
  .card-header h2 {
    font-size: 16px;
  }
  
  .vital-input {
    padding: 12px;
  }
  
  .item-card {
    padding: 16px;
  }
}

/* Enhanced animations and micro-interactions */
@keyframes slideInUp {
  from {
    transform: translateY(30px);
    opacity: 0;
  }
  to {
    transform: translateY(0);
    opacity: 1;
  }
}

@keyframes fadeInScale {
  from {
    transform: scale(0.95);
    opacity: 0;
  }
  to {
    transform: scale(1);
    opacity: 1;
  }
}

.card {
  animation: slideInUp 0.5s ease-out;
}

.modal-content {
  animation: fadeInScale 0.3s ease-out;
}

/* Loading states */
.loading-placeholder {
  background: linear-gradient(90deg, #f0f0f0 25%, #e0e0e0 50%, #f0f0f0 75%);
  background-size: 200% 100%;
  animation: loading 1.5s infinite;
}

@keyframes loading {
  0% {
    background-position: 200% 0;
  }
  100% {
    background-position: -200% 0;
  }
}

/* Print Styles */
@media print {
  body * {
    visibility: hidden;
  }
  
  .print-template,
  .print-template * {
    visibility: visible;
  }
  
  .print-template {
    position: absolute;
    left: 0;
    top: 0;
    width: 100%;
    padding: 20px;
  }
}

.print-template {
  font-family: Arial, sans-serif;
  color: #000;
}

.print-header {
  text-align: center;
  margin-bottom: 30px;
  border-bottom: 2px solid #000;
  padding-bottom: 10px;
}

.print-section {
  margin-bottom: 20px;
}

.print-section h2 {
  font-size: 16px;
  margin-bottom: 10px;
  color: #000;
  border-bottom: 1px solid #666;
}

.print-item {
  margin-bottom: 10px;
  padding-left: 15px;
}

.print-footer {
  margin-top: 50px;
  text-align: right;
}
</style>
