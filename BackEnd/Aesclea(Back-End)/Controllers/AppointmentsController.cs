// Copyright (c) 2025 Alexander Sinapov | Simeon Petkov

// All rights reserved.
// This code is proprietary and confidential.  
// Unauthorized copying, modification, distribution, or use is strictly prohibited.

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Aesclea_Back_End_.Data;
using Aesclea_Back_End_.Models;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;

namespace Aesclea_Back_End_.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class AppointmentsController : ControllerBase
    {
        private readonly AescleaDbContext _context;
        private readonly ILogger<AppointmentsController> _logger;

        public AppointmentsController(AescleaDbContext context, ILogger<AppointmentsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/appointments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Appointment>>> GetAppointments(
            [FromQuery] string? patientId = null,
            [FromQuery] string? department = null,
            [FromQuery] string? status = null,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null)
        {
            try
            {
                var query = _context.Appointments.AsQueryable();

                // Apply filters
                if (!string.IsNullOrEmpty(patientId))
                    query = query.Where(a => a.PatientId == patientId);

                if (!string.IsNullOrEmpty(department))
                    query = query.Where(a => a.Department == department);

                if (!string.IsNullOrEmpty(status))
                    query = query.Where(a => a.Status == status);

                if (fromDate.HasValue)
                    query = query.Where(a => a.DateTime >= fromDate.Value);

                if (toDate.HasValue)
                    query = query.Where(a => a.DateTime <= toDate.Value);

                var appointments = await query
                    .OrderBy(a => a.DateTime)
                    .ToListAsync();

                return Ok(appointments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving appointments");
                return StatusCode(500, "An error occurred while retrieving appointments");
            }
        }

        // GET: api/appointments/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Appointment>> GetAppointment(string id)
        {
            try
            {
                var appointment = await _context.Appointments
                    .Include(a => a.Patient)
                    .FirstOrDefaultAsync(a => a.Id == id);

                if (appointment == null)
                {
                    return NotFound($"Appointment with ID {id} not found");
                }

                return Ok(appointment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving appointment {AppointmentId}", id);
                return StatusCode(500, "An error occurred while retrieving the appointment");
            }
        }

        // GET: api/appointments/today
        [HttpGet("today")]
        public async Task<ActionResult<IEnumerable<Appointment>>> GetTodayAppointments()
        {
            try
            {
                var today = DateTime.Today;
                var tomorrow = today.AddDays(1);

                var appointments = await _context.Appointments
                    .Where(a => a.DateTime >= today && a.DateTime < tomorrow)
                    .OrderBy(a => a.DateTime)
                    .ToListAsync();

                return Ok(appointments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving today's appointments");
                return StatusCode(500, "An error occurred while retrieving today's appointments");
            }
        }

        // GET: api/appointments/patient/5
        [HttpGet("patient/{patientId}")]
        public async Task<ActionResult<IEnumerable<Appointment>>> GetAppointmentsByPatient(string patientId)
        {
            try
            {
                var appointments = await _context.Appointments
                    .Where(a => a.PatientId == patientId)
                    .OrderBy(a => a.DateTime)
                    .ToListAsync();

                return Ok(appointments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving appointments for patient {PatientId}", patientId);
                return StatusCode(500, "An error occurred while retrieving patient appointments");
            }
        }

        // POST: api/appointments
        [HttpPost]
        public async Task<ActionResult<Appointment>> CreateAppointment(CreateAppointmentDto appointmentDto)
        {
            try
            {
                // Validate the patient exists
                var patientExists = await _context.Patients.AnyAsync(p => p.Id == appointmentDto.PatientId);
                if (!patientExists)
                {
                    return BadRequest($"Patient with ID {appointmentDto.PatientId} not found");
                }

                // Check for conflicting appointments
                var hasConflict = await _context.Appointments
                    .Where(a => a.Doctor == appointmentDto.Doctor && a.Status == "scheduled")
                    .AnyAsync(a => 
                        (appointmentDto.DateTime >= a.DateTime && appointmentDto.DateTime < a.DateTime.AddMinutes(a.Duration)) ||
                        (appointmentDto.DateTime.AddMinutes(appointmentDto.Duration) > a.DateTime && appointmentDto.DateTime < a.DateTime));

                if (hasConflict)
                {
                    return Conflict("The selected time slot conflicts with another appointment for this doctor");
                }

                var appointment = new Appointment
                {
                    Id = Guid.NewGuid().ToString(),
                    PatientId = appointmentDto.PatientId,
                    PatientName = appointmentDto.PatientName,
                    Department = appointmentDto.Department,
                    Doctor = appointmentDto.Doctor,
                    AppointmentType = appointmentDto.AppointmentType,
                    DateTime = appointmentDto.DateTime,
                    Duration = appointmentDto.Duration,
                    Status = appointmentDto.Status ?? "scheduled",
                    Priority = appointmentDto.Priority ?? "normal",
                    Reason = appointmentDto.Reason,
                    Notes = appointmentDto.Notes,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Appointments.Add(appointment);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Created appointment {AppointmentId} for patient {PatientId}", appointment.Id, appointment.PatientId);

                return CreatedAtAction(nameof(GetAppointment), new { id = appointment.Id }, appointment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating appointment");
                return StatusCode(500, "An error occurred while creating the appointment");
            }
        }

        // PUT: api/appointments/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAppointment(string id, UpdateAppointmentDto appointmentDto)
        {
            try
            {
                var appointment = await _context.Appointments.FindAsync(id);
                if (appointment == null)
                {
                    return NotFound($"Appointment with ID {id} not found");
                }

                // Update only provided fields
                if (!string.IsNullOrEmpty(appointmentDto.PatientName))
                    appointment.PatientName = appointmentDto.PatientName;
                
                if (!string.IsNullOrEmpty(appointmentDto.Department))
                    appointment.Department = appointmentDto.Department;
                
                if (!string.IsNullOrEmpty(appointmentDto.Doctor))
                    appointment.Doctor = appointmentDto.Doctor;
                
                if (!string.IsNullOrEmpty(appointmentDto.AppointmentType))
                    appointment.AppointmentType = appointmentDto.AppointmentType;
                
                if (appointmentDto.DateTime.HasValue)
                    appointment.DateTime = appointmentDto.DateTime.Value;
                
                if (appointmentDto.Duration.HasValue)
                    appointment.Duration = appointmentDto.Duration.Value;
                
                if (!string.IsNullOrEmpty(appointmentDto.Status))
                    appointment.Status = appointmentDto.Status;
                
                if (!string.IsNullOrEmpty(appointmentDto.Priority))
                    appointment.Priority = appointmentDto.Priority;
                
                if (appointmentDto.Reason != null)
                    appointment.Reason = appointmentDto.Reason;
                
                if (appointmentDto.Notes != null)
                    appointment.Notes = appointmentDto.Notes;

                appointment.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Updated appointment {AppointmentId}", id);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating appointment {AppointmentId}", id);
                return StatusCode(500, "An error occurred while updating the appointment");
            }
        }

        // DELETE: api/appointments/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAppointment(string id)
        {
            try
            {
                var appointment = await _context.Appointments.FindAsync(id);
                if (appointment == null)
                {
                    return NotFound($"Appointment with ID {id} not found");
                }

                _context.Appointments.Remove(appointment);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Deleted appointment {AppointmentId}", id);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting appointment {AppointmentId}", id);
                return StatusCode(500, "An error occurred while deleting the appointment");
            }
        }
    }

    // DTOs for request/response
    public class CreateAppointmentDto
    {
        [Required]
        public string PatientId { get; set; } = string.Empty;
        
        [Required]
        public string PatientName { get; set; } = string.Empty;
        
        [Required]
        public string Department { get; set; } = string.Empty;
        
        [Required]
        public string Doctor { get; set; } = string.Empty;
        
        [Required]
        public string AppointmentType { get; set; } = string.Empty;
        
        [Required]
        public DateTime DateTime { get; set; }
        
        [Required]
        public int Duration { get; set; }
        
        public string? Status { get; set; }
        public string? Priority { get; set; }
        public string? Reason { get; set; }
        public string? Notes { get; set; }
    }

    public class UpdateAppointmentDto
    {
        public string? PatientName { get; set; }
        public string? Department { get; set; }
        public string? Doctor { get; set; }
        public string? AppointmentType { get; set; }
        public DateTime? DateTime { get; set; }
        public int? Duration { get; set; }
        public string? Status { get; set; }
        public string? Priority { get; set; }
        public string? Reason { get; set; }
        public string? Notes { get; set; }
    }
}