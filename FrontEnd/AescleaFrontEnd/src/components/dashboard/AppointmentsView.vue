<!-- Copyright (c) 2025 Alexander Sinapov | Simeon Petkov -->

<!-- All rights reserved. -->
<!-- This code is proprietary and confidential.   -->
<!-- Unauthorized copying, modification, distribution, or use is strictly prohibited. -->

<template>
  <div class="space-y-6">
    <!-- Page Header -->
    <div class="sm:flex sm:items-center sm:justify-between">
      <div>
        <h1 class="text-2xl font-bold text-gray-900 dark:text-white">Appointments</h1>
        <p class="mt-1 text-sm text-gray-500 dark:text-gray-400">
          Manage patient appointments and scheduling
          <span v-if="selectedDepartment" class="capitalize">
            - {{ selectedDepartment }} Department
          </span>
        </p>
      </div>
      <div class="flex mt-4 space-x-3 sm:mt-0">
        <button
          @click="showScheduleModal = true"
          class="inline-flex items-center px-4 py-2 text-sm font-medium text-white bg-purple-600 border border-transparent rounded-md shadow-sm hover:bg-purple-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-purple-500"
        >
          <PlusIcon class="w-4 h-4 mr-2" />
          Schedule Appointment
        </button>
      </div>
    </div>

    <!-- Calendar View Toggle and Filters -->
    <div class="bg-white border border-gray-200 rounded-lg shadow-sm dark:bg-gray-800 dark:border-gray-700">
      <div class="p-6">
        <div class="flex flex-wrap items-center justify-between gap-4">
          <!-- View Toggle -->
          <div class="flex items-center space-x-2">
            <button
              @click="viewMode = 'list'"
              :class="viewMode === 'list' 
                ? 'bg-purple-100 text-purple-700 dark:bg-purple-900 dark:text-purple-200' 
                : 'text-gray-500 hover:text-gray-700 dark:text-gray-400 dark:hover:text-gray-200'"
              class="px-3 py-2 text-sm font-medium rounded-md"
            >
              <ListBulletIcon class="inline w-4 h-4 mr-2" />
              List View
            </button>
            <button
              @click="viewMode = 'calendar'"
              :class="viewMode === 'calendar' 
                ? 'bg-purple-100 text-purple-700 dark:bg-purple-900 dark:text-purple-200' 
                : 'text-gray-500 hover:text-gray-700 dark:text-gray-400 dark:hover:text-gray-200'"
              class="px-3 py-2 text-sm font-medium rounded-md"
            >
              <CalendarIcon class="inline w-4 h-4 mr-2" />
              Calendar View
            </button>
          </div>

          <!-- Filters -->
          <div class="flex items-center space-x-4">
            <select
              v-model="statusFilter"
              class="block px-3 py-2 text-gray-900 bg-white border border-gray-300 rounded-md dark:border-gray-600 dark:bg-gray-700 dark:text-white focus:outline-none focus:ring-1 focus:ring-purple-500 focus:border-purple-500 sm:text-sm"
            >
              <option value="">All Statuses</option>
              <option value="scheduled">Scheduled</option>
              <option value="completed">Completed</option>
              <option value="cancelled">Cancelled</option>
              <option value="no-show">No Show</option>
            </select>

            <select
              v-model="departmentFilter"
              class="block px-3 py-2 text-gray-900 bg-white border border-gray-300 rounded-md dark:border-gray-600 dark:bg-gray-700 dark:text-white focus:outline-none focus:ring-1 focus:ring-purple-500 focus:border-purple-500 sm:text-sm"
            >
              <option value="">All Departments</option>
              <option value="cardiology">Cardiology</option>
              <option value="neurology">Neurology</option>
              <option value="oncology">Oncology</option>
              <option value="radiology">Radiology</option>
              <option value="emergency">Emergency</option>
              <option value="pediatrics">Pediatrics</option>
            </select>

            <input
              v-model="dateFilter"
              type="date"
              class="block px-3 py-2 text-gray-900 bg-white border border-gray-300 rounded-md dark:border-gray-600 dark:bg-gray-700 dark:text-white focus:outline-none focus:ring-1 focus:ring-purple-500 focus:border-purple-500 sm:text-sm"
            />
          </div>
        </div>
      </div>
    </div>

    <!-- Appointments Statistics -->
    <div class="grid grid-cols-1 gap-6 md:grid-cols-4">
      <div class="overflow-hidden bg-white border border-gray-200 rounded-lg shadow-sm dark:bg-gray-800 dark:border-gray-700">
        <div class="p-5">
          <div class="flex items-center">
            <div class="flex-shrink-0">
              <CalendarIcon class="w-6 h-6 text-blue-400" />
            </div>
            <div class="flex-1 w-0 ml-5">
              <dl>
                <dt class="text-sm font-medium text-gray-500 truncate dark:text-gray-400">Today's Appointments</dt>
                <dd class="text-lg font-medium text-gray-900 dark:text-white">{{ todayAppointments.length }}</dd>
              </dl>
            </div>
          </div>
        </div>
      </div>

      <div class="overflow-hidden bg-white border border-gray-200 rounded-lg shadow-sm dark:bg-gray-800 dark:border-gray-700">
        <div class="p-5">
          <div class="flex items-center">
            <div class="flex-shrink-0">
              <ClockIcon class="w-6 h-6 text-yellow-400" />
            </div>
            <div class="flex-1 w-0 ml-5">
              <dl>
                <dt class="text-sm font-medium text-gray-500 truncate dark:text-gray-400">Upcoming</dt>
                <dd class="text-lg font-medium text-gray-900 dark:text-white">{{ upcomingAppointments.length }}</dd>
              </dl>
            </div>
          </div>
        </div>
      </div>

      <div class="overflow-hidden bg-white border border-gray-200 rounded-lg shadow-sm dark:bg-gray-800 dark:border-gray-700">
        <div class="p-5">
          <div class="flex items-center">
            <div class="flex-shrink-0">
              <CheckCircleIcon class="w-6 h-6 text-green-400" />
            </div>
            <div class="flex-1 w-0 ml-5">
              <dl>
                <dt class="text-sm font-medium text-gray-500 truncate dark:text-gray-400">Completed</dt>
                <dd class="text-lg font-medium text-gray-900 dark:text-white">{{ completedAppointments.length }}</dd>
              </dl>
            </div>
          </div>
        </div>
      </div>

      <div class="overflow-hidden bg-white border border-gray-200 rounded-lg shadow-sm dark:bg-gray-800 dark:border-gray-700">
        <div class="p-5">
          <div class="flex items-center">
            <div class="flex-shrink-0">
              <XCircleIcon class="w-6 h-6 text-red-400" />
            </div>
            <div class="flex-1 w-0 ml-5">
              <dl>
                <dt class="text-sm font-medium text-gray-500 truncate dark:text-gray-400">Cancelled/No Show</dt>
                <dd class="text-lg font-medium text-gray-900 dark:text-white">{{ cancelledAppointments.length }}</dd>
              </dl>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- List View -->
    <div v-if="viewMode === 'list'" class="bg-white border border-gray-200 rounded-lg shadow-sm dark:bg-gray-800 dark:border-gray-700">
      <div class="px-6 py-4 border-b border-gray-200 dark:border-gray-700">
        <h3 class="text-lg font-medium text-gray-900 dark:text-white">
          Appointments ({{ filteredAppointments.length }})
        </h3>
      </div>
      
      <div class="overflow-x-auto">
        <table class="min-w-full divide-y divide-gray-200 dark:divide-gray-700">
          <thead class="bg-gray-50 dark:bg-gray-900">
            <tr>
              <th class="px-6 py-3 text-xs font-medium tracking-wider text-left text-gray-500 uppercase dark:text-gray-400">
                Patient & Time
              </th>
              <th class="px-6 py-3 text-xs font-medium tracking-wider text-left text-gray-500 uppercase dark:text-gray-400">
                Appointment Type
              </th>
              <th class="px-6 py-3 text-xs font-medium tracking-wider text-left text-gray-500 uppercase dark:text-gray-400">
                Department
              </th>
              <th class="px-6 py-3 text-xs font-medium tracking-wider text-left text-gray-500 uppercase dark:text-gray-400">
                Doctor
              </th>
              <th class="px-6 py-3 text-xs font-medium tracking-wider text-left text-gray-500 uppercase dark:text-gray-400">
                Status
              </th>
              <th class="relative px-6 py-3">
                <span class="sr-only">Actions</span>
              </th>
            </tr>
          </thead>
          <tbody class="bg-white divide-y divide-gray-200 dark:bg-gray-800 dark:divide-gray-700">
            <tr v-for="appointment in paginatedAppointments" :key="appointment.id" class="hover:bg-gray-50 dark:hover:bg-gray-700">
              <td class="px-6 py-4 whitespace-nowrap">
                <div class="flex items-center">
                  <div class="flex-shrink-0 w-10 h-10">
                    <div class="flex items-center justify-center w-10 h-10 bg-purple-100 rounded-full dark:bg-purple-900">
                      <span class="text-sm font-medium text-purple-600 dark:text-purple-300">
                        {{ appointment.patientName.split(' ').map((n: string) => n[0]).join('') }}
                      </span>
                    </div>
                  </div>
                  <div class="ml-4">
                    <div class="text-sm font-medium text-gray-900 dark:text-white">
                      {{ appointment.patientName }}
                    </div>
                    <div class="text-sm text-gray-500 dark:text-gray-400">
                      {{ formatDateTime(appointment.dateTime) }}
                    </div>
                  </div>
                </div>
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <div class="text-sm text-gray-900 dark:text-white">{{ appointment.appointmentType }}</div>
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <span 
                  class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium capitalize"
                  :class="getDepartmentBadgeClass(appointment.department)"
                >
                  {{ appointment.department }}
                </span>
              </td>
              <td class="px-6 py-4 text-sm text-gray-900 whitespace-nowrap dark:text-white">
                {{ appointment.doctor }}
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <span 
                  class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium"
                  :class="getStatusBadgeClass(appointment.status)"
                >
                  {{ appointment.status }}
                </span>
              </td>
              <td class="px-6 py-4 text-sm font-medium text-right whitespace-nowrap">
                <div class="flex items-center space-x-2">
                  <button
                    @click="viewAppointment(appointment)"
                    class="text-purple-600 hover:text-purple-900 dark:text-purple-400 dark:hover:text-purple-300"
                  >
                    View
                  </button>
                  <button
                    v-if="appointment.status === 'scheduled'"
                    @click="editAppointment(appointment)"
                    class="text-blue-600 hover:text-blue-900 dark:text-blue-400 dark:hover:text-blue-300"
                  >
                    Edit
                  </button>
                  <button
                    v-if="appointment.status === 'scheduled'"
                    @click="cancelAppointment(appointment)"
                    class="text-red-600 hover:text-red-900 dark:text-red-400 dark:hover:text-red-300"
                  >
                    Cancel
                  </button>
                </div>
              </td>
            </tr>
          </tbody>
        </table>
        
        <!-- Empty State -->
        <div v-if="filteredAppointments.length === 0" class="py-12 text-center">
          <CalendarIcon class="w-12 h-12 mx-auto text-gray-400" />
          <h3 class="mt-2 text-sm font-medium text-gray-900 dark:text-white">No appointments found</h3>
          <p class="mt-1 text-sm text-gray-500 dark:text-gray-400">
            Get started by scheduling a new appointment.
          </p>
          <div class="mt-6">
            <button
              @click="showScheduleModal = true"
              class="inline-flex items-center px-4 py-2 text-sm font-medium text-white bg-purple-600 border border-transparent rounded-md shadow-sm hover:bg-purple-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-purple-500"
            >
              <PlusIcon class="w-4 h-4 mr-2" />
              Schedule Appointment
            </button>
          </div>
        </div>
      </div>

      <!-- Pagination -->
      <div v-if="filteredAppointments.length > appointmentsPerPage" class="px-6 py-4 border-t border-gray-200 dark:border-gray-700">
        <div class="flex items-center justify-between">
          <div class="text-sm text-gray-700 dark:text-gray-300">
            Showing {{ (currentPage - 1) * appointmentsPerPage + 1 }} to {{ Math.min(currentPage * appointmentsPerPage, filteredAppointments.length) }} of {{ filteredAppointments.length }} results
          </div>
          <div class="flex space-x-2">
            <button
              @click="currentPage--"
              :disabled="currentPage === 1"
              class="px-3 py-1 text-sm font-medium text-gray-500 bg-white border border-gray-300 rounded-md hover:bg-gray-50 disabled:opacity-50 disabled:cursor-not-allowed dark:bg-gray-800 dark:border-gray-600 dark:text-gray-400 dark:hover:bg-gray-700"
            >
              Previous
            </button>
            <button
              @click="currentPage++"
              :disabled="currentPage >= totalPages"
              class="px-3 py-1 text-sm font-medium text-gray-500 bg-white border border-gray-300 rounded-md hover:bg-gray-50 disabled:opacity-50 disabled:cursor-not-allowed dark:bg-gray-800 dark:border-gray-600 dark:text-gray-400 dark:hover:bg-gray-700"
            >
              Next
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Calendar View -->
    <div v-else class="bg-white border border-gray-200 rounded-lg shadow-sm dark:bg-gray-800 dark:border-gray-700">
      <div class="p-6">
        <!-- Calendar Header -->
        <div class="flex items-center justify-between mb-6">
          <div class="flex items-center space-x-4">
            <button
              @click="previousMonth"
              class="p-2 text-gray-400 hover:text-gray-600 dark:text-gray-500 dark:hover:text-gray-300"
            >
              <ChevronLeftIcon class="w-5 h-5" />
            </button>
            
            <!-- Month/Year Selection -->
            <div class="flex items-center space-x-2">
              <select 
                v-model="selectedMonth"
                @change="updateCalendarDate"
                class="px-3 py-2 text-sm font-medium text-gray-700 bg-white border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent dark:bg-gray-700 dark:border-gray-600 dark:text-gray-200"
              >
                <option v-for="(month, index) in monthNames" :key="index" :value="index">
                  {{ month }}
                </option>
              </select>
              
              <select 
                v-model="selectedYear"
                @change="updateCalendarDate"
                class="px-3 py-2 text-sm font-medium text-gray-700 bg-white border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent dark:bg-gray-700 dark:border-gray-600 dark:text-gray-200"
              >
                <option v-for="year in availableYears" :key="year" :value="year">
                  {{ year }}
                </option>
              </select>
            </div>
            
            <button
              @click="nextMonth"
              class="p-2 text-gray-400 hover:text-gray-600 dark:text-gray-500 dark:hover:text-gray-300"
            >
              <ChevronRightIcon class="w-5 h-5" />
            </button>
          </div>
          <div class="flex items-center space-x-2">
            <button
              @click="goToToday"
              class="px-3 py-2 text-sm font-medium text-gray-700 bg-gray-100 border border-gray-300 rounded-md hover:bg-gray-200 dark:bg-gray-700 dark:border-gray-600 dark:text-gray-200 dark:hover:bg-gray-600"
            >
              Today
            </button>
          </div>
        </div>

        <!-- Calendar Grid -->
        <div class="grid grid-cols-7 gap-px overflow-hidden bg-gray-200 border border-gray-200 rounded-lg dark:bg-gray-700 dark:border-gray-600">
          <!-- Day Headers -->
          <div 
            v-for="day in dayHeaders" 
            :key="day"
            class="p-2 text-xs font-medium text-center text-gray-500 bg-gray-50 dark:bg-gray-800 dark:text-gray-400"
          >
            {{ day }}
          </div>

          <!-- Calendar Days -->
          <div
            v-for="day in calendarDays"
            :key="`${day.date.getTime()}`"
            @click="selectDay(day.date)"
            :class="[
              'bg-white dark:bg-gray-900 p-2 min-h-[120px] border-b border-r border-gray-100 dark:border-gray-700 cursor-pointer hover:bg-gray-50 dark:hover:bg-gray-800 transition-colors',
              {
                'bg-gray-50 dark:bg-gray-800': !day.isCurrentMonth,
                'bg-blue-50 dark:bg-blue-900/20': day.isToday && day.isCurrentMonth,
                'ring-2 ring-blue-500 ring-inset bg-blue-100 dark:bg-blue-900/30': selectedDay && selectedDay.toDateString() === day.date.toDateString()
              }
            ]"
          >
            <!-- Date Number -->
            <div class="flex items-center justify-between mb-1">
              <span 
                :class="[
                  'text-sm font-medium',
                  {
                    'text-gray-900 dark:text-white': day.isCurrentMonth,
                    'text-gray-400 dark:text-gray-500': !day.isCurrentMonth,
                    'text-blue-600 dark:text-blue-400 font-bold': day.isToday && day.isCurrentMonth
                  }
                ]"
              >
                {{ day.date.getDate() }}
              </span>
              <span v-if="getAppointmentsForDate(day.date).length > 0" 
                    class="px-2 py-1 text-xs font-medium text-purple-600 bg-purple-100 rounded-full dark:bg-purple-900 dark:text-purple-300">
                {{ getAppointmentsForDate(day.date).length }}
              </span>
            </div>

            <!-- Appointments for this day -->
            <div class="space-y-1">
              <div
                v-for="appointment in getAppointmentsForDate(day.date).slice(0, 3)"
                :key="appointment.id"
                @click="viewAppointmentDetails(appointment)"
                :class="[
                  'text-xs p-1 rounded cursor-pointer text-white font-medium truncate',
                  getAppointmentColorClass(appointment.status)
                ]"
                :title="`${formatAppointmentTime(appointment.dateTime)} - ${appointment.patientName} (${appointment.appointmentType})`"
              >
                {{ formatAppointmentTime(appointment.dateTime) }} {{ appointment.patientName }}
              </div>
              
              <!-- More indicator -->
              <div v-if="getAppointmentsForDate(day.date).length > 3"
                   class="text-xs text-gray-500 dark:text-gray-400 font-medium">
                +{{ getAppointmentsForDate(day.date).length - 3 }} more
              </div>
            </div>
          </div>
        </div>

        <!-- Calendar Legend -->
        <div class="flex items-center justify-center mt-6 space-x-6">
          <div class="flex items-center space-x-2">
            <div class="w-3 h-3 bg-green-500 rounded"></div>
            <span class="text-xs text-gray-600 dark:text-gray-400">Completed</span>
          </div>
          <div class="flex items-center space-x-2">
            <div class="w-3 h-3 bg-blue-500 rounded"></div>
            <span class="text-xs text-gray-600 dark:text-gray-400">Scheduled</span>
          </div>
          <div class="flex items-center space-x-2">
            <div class="w-3 h-3 bg-red-500 rounded"></div>
            <span class="text-xs text-gray-600 dark:text-gray-400">Cancelled</span>
          </div>
          <div class="flex items-center space-x-2">
            <div class="w-3 h-3 bg-yellow-500 rounded"></div>
            <span class="text-xs text-gray-600 dark:text-gray-400">Rescheduled</span>
          </div>
        </div>
      </div>
    </div>

    <!-- Selected Day Appointments Section -->
    <div v-if="selectedDay" class="mt-6 bg-white border border-gray-200 rounded-lg shadow-sm dark:bg-gray-800 dark:border-gray-700">
      <div class="p-6">
        <div class="flex items-center justify-between mb-4">
          <h3 class="text-lg font-semibold text-gray-900 dark:text-white">
            Appointments for {{ formatSelectedDate(selectedDay) }}
          </h3>
          <button
            @click="scheduleForSelectedDay"
            class="inline-flex items-center px-4 py-2 text-sm font-medium text-white bg-blue-600 border border-transparent rounded-md shadow-sm hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500 dark:bg-blue-500 dark:hover:bg-blue-600"
          >
            <PlusIcon class="w-4 h-4 mr-2" />
            Schedule Appointment
          </button>
        </div>

        <div v-if="selectedDayAppointments.length === 0" class="py-8 text-center">
          <CalendarIcon class="w-12 h-12 mx-auto text-gray-400" />
          <h4 class="mt-2 text-sm font-medium text-gray-900 dark:text-white">No appointments scheduled</h4>
          <p class="mt-1 text-sm text-gray-500 dark:text-gray-400">
            No appointments are scheduled for this day.
          </p>
        </div>

        <div v-else class="space-y-4">
          <div
            v-for="appointment in selectedDayAppointments"
            :key="appointment.id"
            class="p-4 border border-gray-200 rounded-lg dark:border-gray-700 hover:bg-gray-50 dark:hover:bg-gray-700 transition-colors"
          >
            <div class="flex items-center justify-between">
              <div class="flex-1">
                <div class="flex items-center space-x-3">
                  <div class="flex-shrink-0">
                    <ClockIcon class="w-5 h-5 text-gray-400" />
                  </div>
                  <div>
                    <p class="text-sm font-medium text-gray-900 dark:text-white">
                      {{ formatAppointmentTime(appointment.dateTime) }} - {{ appointment.patientName }}
                    </p>
                    <p class="text-sm text-gray-500 dark:text-gray-400">
                      {{ appointment.appointmentType }} • {{ appointment.department }}
                      <span v-if="appointment.duration"> • {{ appointment.duration }} min</span>
                    </p>
                  </div>
                </div>
                
                <div class="mt-2 flex items-center space-x-2">
                  <span
                    :class="[
                      'inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium',
                      getStatusBadgeClass(appointment.status)
                    ]"
                  >
                    {{ appointment.status }}
                  </span>
                  <span
                    v-if="appointment.priority"
                    :class="[
                      'inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium',
                      getPriorityBadgeClass(appointment.priority)
                    ]"
                  >
                    {{ appointment.priority }} priority
                  </span>
                </div>
              </div>

              <!-- Action Buttons -->
              <div class="flex items-center space-x-2 ml-4">
                <button
                  @click="viewAppointment(appointment)"
                  class="inline-flex items-center px-3 py-1.5 text-xs font-medium text-gray-700 bg-white border border-gray-300 rounded-md hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500 dark:bg-gray-700 dark:border-gray-600 dark:text-gray-200 dark:hover:bg-gray-600"
                >
                  View
                </button>
                <button
                  @click="editAppointment(appointment)"
                  class="inline-flex items-center px-3 py-1.5 text-xs font-medium text-blue-700 bg-blue-100 border border-transparent rounded-md hover:bg-blue-200 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500 dark:bg-blue-900 dark:text-blue-200 dark:hover:bg-blue-800"
                >
                  Edit
                </button>
                <button
                  v-if="appointment.status === 'scheduled'"
                  @click="cancelAppointment(appointment)"
                  class="inline-flex items-center px-3 py-1.5 text-xs font-medium text-red-700 bg-red-100 border border-transparent rounded-md hover:bg-red-200 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-red-500 dark:bg-red-900 dark:text-red-200 dark:hover:bg-red-800"
                >
                  Cancel
                </button>
              </div>
            </div>

            <!-- Additional appointment details if needed -->
            <div v-if="appointment.notes" class="mt-3 p-3 bg-gray-50 dark:bg-gray-700 rounded-md">
              <p class="text-sm text-gray-600 dark:text-gray-300">
                <span class="font-medium">Notes:</span> {{ appointment.notes }}
              </p>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Schedule Appointment Modal -->
    <AppointmentModal
      v-if="showScheduleModal || showEditModal"
      :appointment="selectedAppointment"
      :is-edit="showEditModal"
      @close="closeModals"
      @save="handleSaveAppointment"
    />

    <!-- Appointment Details Modal -->
    <AppointmentDetailsModal
      v-if="showDetailsModal && selectedAppointment"
      :appointment="selectedAppointment"
      @close="showDetailsModal = false"
      @edit="editAppointment"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch, onMounted, onUnmounted } from 'vue'
import { useAppointmentsStore, type Appointment } from '../../stores/appointments'
import AppointmentModal from './AppointmentModal.vue'
import AppointmentDetailsModal from './AppointmentDetailsModal.vue'

// Define props
interface Props {
  selectedDepartment?: string | null
}

const props = defineProps<Props>()

// Store
const appointmentsStore = useAppointmentsStore()

// Local state
const viewMode = ref<'list' | 'calendar'>('list')
const statusFilter = ref('')
const departmentFilter = ref('')
const dateFilter = ref('')
const currentPage = ref(1)
const appointmentsPerPage = 20

const showScheduleModal = ref(false)
const showEditModal = ref(false)
const showDetailsModal = ref(false)
const selectedAppointment = ref<Appointment | null>(null)

// Calendar state
const currentCalendarDate = ref(new Date())
const selectedDay = ref<Date | null>(null)

// Date selection variables
const selectedMonth = ref(new Date().getMonth())
const selectedYear = ref(new Date().getFullYear())

// Calendar computed properties
const monthNames = ['January', 'February', 'March', 'April', 'May', 'June',
  'July', 'August', 'September', 'October', 'November', 'December']

const availableYears = computed(() => {
  const currentYear = new Date().getFullYear()
  const years = []
  for (let i = currentYear - 5; i <= currentYear + 5; i++) {
    years.push(i)
  }
  return years
})


const dayHeaders = ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat']

interface CalendarDay {
  date: Date
  isCurrentMonth: boolean
  isToday: boolean
}

const calendarDays = computed((): CalendarDay[] => {
  const year = currentCalendarDate.value.getFullYear()
  const month = currentCalendarDate.value.getMonth()
  
  // First day of current month
  const firstDay = new Date(year, month, 1)
  // Last day of current month
  const lastDay = new Date(year, month + 1, 0)
  
  // Start from the first Sunday before or on the first day of month
  const startDate = new Date(firstDay)
  startDate.setDate(startDate.getDate() - startDate.getDay())
  
  // End at the last Saturday after or on the last day of month
  const endDate = new Date(lastDay)
  endDate.setDate(endDate.getDate() + (6 - endDate.getDay()))
  
  const days: CalendarDay[] = []
  const today = new Date()
  const todayStr = today.toDateString()
  
  for (let date = new Date(startDate); date <= endDate; date.setDate(date.getDate() + 1)) {
    const currentDate = new Date(date)
    days.push({
      date: currentDate,
      isCurrentMonth: currentDate.getMonth() === month,
      isToday: currentDate.toDateString() === todayStr
    })
  }
  
  return days
})

// Watch for department changes from parent
watch(() => props.selectedDepartment, (newDepartment) => {
  if (newDepartment) {
    departmentFilter.value = newDepartment
  }
}, { immediate: true })

// Watch for calendar date changes to sync dropdowns
watch(currentCalendarDate, (newDate) => {
  selectedMonth.value = newDate.getMonth()
  selectedYear.value = newDate.getFullYear()
}, { immediate: true })

// Computed properties
const filteredAppointments = computed(() => {
  let appointments = appointmentsStore.appointments

  // Apply status filter
  if (statusFilter.value) {
    appointments = appointments.filter((apt: Appointment) => apt.status === statusFilter.value)
  }

  // Apply department filter
  if (departmentFilter.value) {
    appointments = appointments.filter((apt: Appointment) => apt.department === departmentFilter.value)
  }

  // Apply date filter
  if (dateFilter.value) {
    const filterDate = new Date(dateFilter.value).toDateString()
    appointments = appointments.filter((apt: Appointment) => 
      new Date(apt.dateTime).toDateString() === filterDate
    )
  }

  return appointments.sort((a: Appointment, b: Appointment) => new Date(a.dateTime).getTime() - new Date(b.dateTime).getTime())
})

const totalPages = computed(() => Math.ceil(filteredAppointments.value.length / appointmentsPerPage))

const paginatedAppointments = computed(() => {
  const start = (currentPage.value - 1) * appointmentsPerPage
  const end = start + appointmentsPerPage
  return filteredAppointments.value.slice(start, end)
})

const todayAppointments = computed(() => {
  const today = new Date().toDateString()
  return appointmentsStore.appointments.filter((apt: Appointment) => 
    new Date(apt.dateTime).toDateString() === today
  )
})

const upcomingAppointments = computed(() => {
  const now = new Date()
  return appointmentsStore.appointments.filter((apt: Appointment) => 
    new Date(apt.dateTime) > now && apt.status === 'scheduled'
  )
})

const completedAppointments = computed(() => 
  appointmentsStore.appointments.filter((apt: Appointment) => apt.status === 'completed')
)

const cancelledAppointments = computed(() => 
  appointmentsStore.appointments.filter((apt: Appointment) => 
    apt.status === 'cancelled' || apt.status === 'rescheduled'
  )
)

const selectedDayAppointments = computed(() => {
  if (!selectedDay.value) return []
  const selectedDateStr = selectedDay.value.toDateString()
  return filteredAppointments.value.filter((appointment: Appointment) => 
    new Date(appointment.dateTime).toDateString() === selectedDateStr
  ).sort((a: Appointment, b: Appointment) => 
    new Date(a.dateTime).getTime() - new Date(b.dateTime).getTime()
  )
})

// Icon components
const PlusIcon = {
  template: `<svg fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" d="M12 4.5v15m7.5-7.5h-15" /></svg>`
}

const CalendarIcon = {
  template: `<svg fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" d="M6.75 3v2.25M17.25 3v2.25M3 18.75V7.5a2.25 2.25 0 012.25-2.25h13.5A2.25 2.25 0 0121 7.5v11.25m-18 0A2.25 2.25 0 005.25 21h13.5A2.25 2.25 0 0021 18.75m-18 0v-7.5A2.25 2.25 0 015.25 9h13.5A2.25 2.25 0 0121 11.25v7.5" /></svg>`
}

const ListBulletIcon = {
  template: `<svg fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" d="M8.25 6.75h12M8.25 12h12m-12 5.25h12M3.75 6.75h.007v.008H3.75V6.75zm.375 0a.375.375 0 11-.75 0 .375.375 0 01.75 0zM3.75 12h.007v.008H3.75V12zm.375 0a.375.375 0 11-.75 0 .375.375 0 01.75 0zM3.75 17.25h.007v.008H3.75v-.008zm.375 0a.375.375 0 11-.75 0 .375.375 0 01.75 0z" /></svg>`
}

const ClockIcon = {
  template: `<svg fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" d="M12 6v6h4.5m4.5 0a9 9 0 11-18 0 9 9 0 0118 0z" /></svg>`
}

const CheckCircleIcon = {
  template: `<svg fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" d="M9 12.75L11.25 15 15 9.75M21 12a9 9 0 11-18 0 9 9 0 0118 0z" /></svg>`
}

const XCircleIcon = {
  template: `<svg fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" d="M9.75 9.75l4.5 4.5m0-4.5l-4.5 4.5M21 12a9 9 0 11-18 0 9 9 0 0118 0z" /></svg>`
}

const ChevronLeftIcon = {
  template: `<svg fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" d="M15.75 19.5L8.25 12l7.5-7.5" /></svg>`
}

const ChevronRightIcon = {
  template: `<svg fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" d="M8.25 4.5l7.5 7.5-7.5 7.5" /></svg>`
}

// Methods
const formatDateTime = (dateTime: string) => {
  return new Date(dateTime).toLocaleString()
}

// Calendar methods
const previousMonth = () => {
  currentCalendarDate.value = new Date(
    currentCalendarDate.value.getFullYear(),
    currentCalendarDate.value.getMonth() - 1,
    1
  )
}

const nextMonth = () => {
  currentCalendarDate.value = new Date(
    currentCalendarDate.value.getFullYear(),
    currentCalendarDate.value.getMonth() + 1,
    1
  )
}

const goToToday = () => {
  const today = new Date()
  currentCalendarDate.value = today
  selectedMonth.value = today.getMonth()
  selectedYear.value = today.getFullYear()
  selectedDay.value = today
}

const updateCalendarDate = () => {
  currentCalendarDate.value = new Date(selectedYear.value, selectedMonth.value, 1)
}

const selectDay = (date: Date) => {
  selectedDay.value = date
}

const getAppointmentsForDate = (date: Date): Appointment[] => {
  const dateStr = date.toDateString()
  return filteredAppointments.value.filter((appointment: Appointment) => 
    new Date(appointment.dateTime).toDateString() === dateStr
  )
}

const formatAppointmentTime = (dateTime: string): string => {
  return new Date(dateTime).toLocaleTimeString([], { 
    hour: '2-digit', 
    minute: '2-digit' 
  })
}

const getAppointmentColorClass = (status: string): string => {
  const classes: Record<string, string> = {
    scheduled: 'bg-blue-500',
    completed: 'bg-green-500',
    cancelled: 'bg-red-500',
    rescheduled: 'bg-yellow-500',
    'no-show': 'bg-gray-500'
  }
  return classes[status] || classes.scheduled
}

const viewAppointmentDetails = (appointment: Appointment) => {
  viewAppointment(appointment)
}

const formatSelectedDate = (date: Date): string => {
  return date.toLocaleDateString('en-US', { 
    weekday: 'long', 
    year: 'numeric', 
    month: 'long', 
    day: 'numeric' 
  })
}

const scheduleForSelectedDay = () => {
  if (selectedDay.value) {
    // Pre-fill the appointment modal with the selected date
    showScheduleModal.value = true
  }
}

const getPriorityBadgeClass = (priority: string): string => {
  const classes: Record<string, string> = {
    high: 'bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-200',
    medium: 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900 dark:text-yellow-200',
    low: 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-200',
    urgent: 'bg-red-200 text-red-900 dark:bg-red-800 dark:text-red-100'
  }
  return classes[priority] || classes.medium
}

const getDepartmentBadgeClass = (department: string) => {
  const classes: Record<string, string> = {
    cardiology: 'bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-200',
    neurology: 'bg-blue-100 text-blue-800 dark:bg-blue-900 dark:text-blue-200',
    oncology: 'bg-purple-100 text-purple-800 dark:bg-purple-900 dark:text-purple-200',
    radiology: 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-200',
    emergency: 'bg-orange-100 text-orange-800 dark:bg-orange-900 dark:text-orange-200',
    pediatrics: 'bg-pink-100 text-pink-800 dark:bg-pink-900 dark:text-pink-200'
  }
  return classes[department] || 'bg-gray-100 text-gray-800 dark:bg-gray-900 dark:text-gray-200'
}

const getStatusBadgeClass = (status: string) => {
  const classes: Record<string, string> = {
    scheduled: 'bg-blue-100 text-blue-800 dark:bg-blue-900 dark:text-blue-200',
    completed: 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-200',
    cancelled: 'bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-200',
    'no-show': 'bg-gray-100 text-gray-800 dark:bg-gray-900 dark:text-gray-200'
  }
  return classes[status] || classes.scheduled
}

const viewAppointment = (appointment: Appointment) => {
  selectedAppointment.value = appointment
  showDetailsModal.value = true
}

const editAppointment = (appointment: Appointment) => {
  selectedAppointment.value = appointment
  showEditModal.value = true
  showDetailsModal.value = false
}

const cancelAppointment = async (appointment: Appointment) => {
  if (confirm(`Are you sure you want to cancel the appointment for ${appointment.patientName}?`)) {
    try {
      // Update appointment status
      // This would typically call an API endpoint
      appointment.status = 'cancelled'
      console.log('Appointment cancelled:', appointment.id)
    } catch (error) {
      console.error('Error cancelling appointment:', error)
      alert('Failed to cancel appointment. Please try again.')
    }
  }
}

const closeModals = () => {
  showScheduleModal.value = false
  showEditModal.value = false
  selectedAppointment.value = null
}

const handleSaveAppointment = async (appointmentData: Omit<Appointment, 'id' | 'createdAt' | 'updatedAt'>) => {
  try {
    if (showEditModal.value && selectedAppointment.value) {
      // Update existing appointment
      await appointmentsStore.updateAppointment(selectedAppointment.value.id, appointmentData)
    } else {
      // Create new appointment
      await appointmentsStore.addAppointment(appointmentData)
    }
    closeModals()
  } catch (error) {
    console.error('Error saving appointment:', error)
    alert('Failed to save appointment. Please try again.')
  }
}

// Lifecycle
onMounted(async () => {
  await appointmentsStore.fetchAppointments()
  // Add event listener for dashboard header button
  document.addEventListener('show-new-appointment-modal', handleShowScheduleModal)
})

onUnmounted(() => {
  document.removeEventListener('show-new-appointment-modal', handleShowScheduleModal)
})

// Event listener for dashboard header button
const handleShowScheduleModal = () => {
  showScheduleModal.value = true
}
</script>
