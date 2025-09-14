// Copyright (c) 2025 Alexander Sinapov | Simeon Petkov

// All rights reserved.
// This code is proprietary and confidential.  
// Unauthorized copying, modification, distribution, or use is strictly prohibited.

using Aesclea_Back_End_.Models;
using Aesclea_Back_End_.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;

namespace Aesclea_Back_End_.Controllers
{
    [Authorize] // Temporarily disabled to debug 500 error
    [Route("api/[controller]")]
    [ApiController]
    public class PatientsController : ControllerBase
    {
        private readonly AescleaDbContext _context;
        private readonly ILogger<PatientsController> _logger;

        public PatientsController(AescleaDbContext context, ILogger<PatientsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/patients
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Patient>>> GetPatients(
            [FromQuery] string? department = null,
            [FromQuery] string? status = null,
            [FromQuery] string? search = null)
        {
            try
            {
                // Get current authenticated user ID
                var userId = GetCurrentUserId();
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User not authenticated");
                }

                // Filter patients by current user
                var query = _context.Patients.Where(p => p.UserId == userId);

                // Apply additional filters
                if (!string.IsNullOrEmpty(department))
                    query = query.Where(p => p.Department == department);

                if (!string.IsNullOrEmpty(status))
                    query = query.Where(p => p.Status == status);

                if (!string.IsNullOrEmpty(search))
                {
                    query = query.Where(p => 
                        p.FirstName.Contains(search) || 
                        p.LastName.Contains(search) ||
                        p.Email.Contains(search) ||
                        p.Phone.Contains(search));
                }

                var patients = await query
                    .OrderBy(p => p.LastName)
                    .ThenBy(p => p.FirstName)
                    .ToListAsync();

                return Ok(patients);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving patients");
                return StatusCode(500, "An error occurred while retrieving patients");
            }
        }

        // GET: api/patients/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Patient>> GetPatient(string id)
        {
            try
            {
                // Get current authenticated user ID
                var userId = GetCurrentUserId();
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User not authenticated");
                }

                var patient = await _context.Patients
                    .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);

                if (patient == null)
                {
                    return NotFound($"Patient with ID {id} not found");
                }

                return Ok(patient);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving patient {PatientId}", id);
                return StatusCode(500, "An error occurred while retrieving the patient");
            }
        }

        // GET: api/patients/test-auth - Test authentication
        [HttpGet("test-auth")]
        public async Task<IActionResult> TestAuth()
        {
            try
            {
                var userId = GetCurrentUserId();
                var isAuthenticated = User.Identity?.IsAuthenticated ?? false;
                
                bool userExistsInDb = false;
                if (!string.IsNullOrEmpty(userId))
                {
                    userExistsInDb = await _context.Users.AnyAsync(u => u.Id == userId);
                }
                
                return Ok(new
                {
                    isAuthenticated = isAuthenticated,
                    userId = userId,
                    userExistsInDb = userExistsInDb,
                    userName = User.Identity?.Name,
                    claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in test auth endpoint");
                return StatusCode(500, ex.Message);
            }
        }

        // POST: api/patients
        [HttpPost]
        public async Task<ActionResult<Patient>> CreatePatient(CreatePatientDto patientDto)
        {
            try
            {
                // Check if patient with same email already exists
                var existingPatient = await _context.Patients
                    .FirstOrDefaultAsync(p => p.Email == patientDto.Email);
                
                if (existingPatient != null)
                {
                    return Conflict($"Patient with email {patientDto.Email} already exists");
                }

                // Get current authenticated user ID
                _logger.LogInformation("CreatePatient: Attempting to get current user ID");
                var userId = GetCurrentUserId();
                
                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogWarning("CreatePatient: No user ID found, user not authenticated");
                    return Unauthorized("User not authenticated");
                }
                
                _logger.LogInformation("CreatePatient: Using user ID: {UserId}", userId);

                // Verify that the user exists in the database
                var userExists = await _context.Users.AnyAsync(u => u.Id == userId);
                if (!userExists)
                {
                    _logger.LogError("CreatePatient: User with ID {UserId} does not exist in database", userId);
                    return BadRequest($"Invalid user ID: {userId}. User not found in database.");
                }

                var patient = new Patient
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = userId,
                    FirstName = patientDto.FirstName,
                    LastName = patientDto.LastName,
                    Email = patientDto.Email,
                    Phone = patientDto.Phone,
                    DateOfBirth = patientDto.DateOfBirth.HasValue 
                        ? DateTime.SpecifyKind(patientDto.DateOfBirth.Value, DateTimeKind.Utc) 
                        : null,
                    Gender = patientDto.Gender,
                    MedicalHistory = patientDto.MedicalHistory ?? string.Empty,
                    Department = patientDto.Department,
                    Status = patientDto.Status ?? "active",
                    CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc),
                    UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc)
                };

                _context.Patients.Add(patient);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Created patient {PatientId} ({PatientName})", patient.Id, $"{patient.FirstName} {patient.LastName}");

                return CreatedAtAction(nameof(GetPatient), new { id = patient.Id }, patient);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating patient");
                return StatusCode(500, "An error occurred while creating the patient");
            }
        }

        // PUT: api/patients/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePatient(string id, UpdatePatientDto patientDto)
        {
            try
            {
                // Get current authenticated user ID
                var userId = GetCurrentUserId();
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User not authenticated");
                }

                var patient = await _context.Patients
                    .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);
                if (patient == null)
                {
                    return NotFound($"Patient with ID {id} not found");
                }

                // Update only provided fields
                if (!string.IsNullOrEmpty(patientDto.FirstName))
                    patient.FirstName = patientDto.FirstName;
                
                if (!string.IsNullOrEmpty(patientDto.LastName))
                    patient.LastName = patientDto.LastName;
                
                if (!string.IsNullOrEmpty(patientDto.Email))
                {
                    // Check if email is being changed to an existing email
                    var existingPatient = await _context.Patients
                        .FirstOrDefaultAsync(p => p.Email == patientDto.Email && p.Id != id);
                    
                    if (existingPatient != null)
                    {
                        return Conflict($"Patient with email {patientDto.Email} already exists");
                    }
                    patient.Email = patientDto.Email;
                }
                
                if (!string.IsNullOrEmpty(patientDto.Phone))
                    patient.Phone = patientDto.Phone;
                
                if (patientDto.DateOfBirth.HasValue)
                    patient.DateOfBirth = DateTime.SpecifyKind(patientDto.DateOfBirth.Value, DateTimeKind.Utc);
                
                if (!string.IsNullOrEmpty(patientDto.Gender))
                    patient.Gender = patientDto.Gender;
                
                if (patientDto.MedicalHistory != null)
                    patient.MedicalHistory = patientDto.MedicalHistory;
                
                if (!string.IsNullOrEmpty(patientDto.Department))
                    patient.Department = patientDto.Department;
                
                if (!string.IsNullOrEmpty(patientDto.Status))
                    patient.Status = patientDto.Status;

                patient.UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);

                await _context.SaveChangesAsync();

                _logger.LogInformation("Updated patient {PatientId}", id);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating patient {PatientId}", id);
                return StatusCode(500, "An error occurred while updating the patient");
            }
        }

        // DELETE: api/patients/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePatient(string id)
        {
            try
            {
                // Get current authenticated user ID
                var userId = GetCurrentUserId();
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User not authenticated");
                }

                var patient = await _context.Patients
                    .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);
                if (patient == null)
                {
                    return NotFound($"Patient with ID {id} not found");
                }

                _context.Patients.Remove(patient);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Deleted patient {PatientId}", id);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting patient {PatientId}", id);
                return StatusCode(500, "An error occurred while deleting the patient");
            }
        }

        // GET: api/patients/stats
        [HttpGet("stats")]
        public async Task<ActionResult<PatientStatsDto>> GetPatientStats()
        {
            try
            {
                // Get current authenticated user ID
                var userId = GetCurrentUserId();
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User not authenticated");
                }

                // Filter statistics by current user's patients
                var totalPatients = await _context.Patients.CountAsync(p => p.UserId == userId);
                var activePatients = await _context.Patients.CountAsync(p => p.UserId == userId && p.Status == "active");
                var criticalPatients = await _context.Patients.CountAsync(p => p.UserId == userId && p.Status == "critical");
                
                var departmentStats = await _context.Patients
                    .Where(p => p.UserId == userId && p.Department != null)
                    .GroupBy(p => p.Department)
                    .Select(g => new { Department = g.Key, Count = g.Count() })
                    .ToListAsync();

                var stats = new PatientStatsDto
                {
                    TotalPatients = totalPatients,
                    ActivePatients = activePatients,
                    CriticalPatients = criticalPatients,
                    DepartmentStats = departmentStats.ToDictionary(x => x.Department!, x => x.Count)
                };

                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving patient statistics");
                return StatusCode(500, "An error occurred while retrieving patient statistics");
            }
        }

        private string? GetCurrentUserId()
        {
            try
            {
                _logger.LogInformation("GetCurrentUserId: User.Identity.IsAuthenticated = {IsAuthenticated}", User.Identity?.IsAuthenticated);
                _logger.LogInformation("GetCurrentUserId: User.Identity.Name = {Name}", User.Identity?.Name);
                
                // Log all claims
                if (User.Claims.Any())
                {
                    _logger.LogInformation("GetCurrentUserId: Available claims:");
                    foreach (var claim in User.Claims)
                    {
                        _logger.LogInformation("  - {Type}: {Value}", claim.Type, claim.Value);
                    }
                }
                else
                {
                    _logger.LogWarning("GetCurrentUserId: No claims found in User.Claims");
                }

                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                _logger.LogInformation("GetCurrentUserId: Retrieved userId = {UserId}", userId ?? "null");
                
                return userId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetCurrentUserId");
                return null;
            }
        }
    }

    // DTOs for request/response
    public class CreatePatientDto
    {
        [Required]
        public string FirstName { get; set; } = string.Empty;
        
        [Required]
        public string LastName { get; set; } = string.Empty;
        
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        public string Phone { get; set; } = string.Empty;
        
        public DateTime? DateOfBirth { get; set; }
        
        [Required]
        public string Gender { get; set; } = string.Empty;
        
        public string? MedicalHistory { get; set; }
        public string? Department { get; set; }
        public string? Status { get; set; }
    }

    public class UpdatePatientDto
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        
        [EmailAddress]
        public string? Email { get; set; }
        
        public string? Phone { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public string? MedicalHistory { get; set; }
        public string? Department { get; set; }
        public string? Status { get; set; }
    }

    public class PatientStatsDto
    {
        public int TotalPatients { get; set; }
        public int ActivePatients { get; set; }
        public int CriticalPatients { get; set; }
        public Dictionary<string, int> DepartmentStats { get; set; } = new();
    }
}
