using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Aesclea_Back_End_.Services.VitalSigns;
using Aesclea_Back_End_.Models.VitalSigns;
using System.Security.Claims;

namespace Aesclea_Back_End_.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class VitalSignsController : ControllerBase
    {
        private readonly VitalSignsService _vitalSignsService;
        private readonly ILogger<VitalSignsController> _logger;

        public VitalSignsController(VitalSignsService vitalSignsService, ILogger<VitalSignsController> logger)
        {
            _vitalSignsService = vitalSignsService;
            _logger = logger;
        }

        /// <summary>
        /// Record new vital signs reading for a patient
        /// </summary>
        [HttpPost("record")]
        public async Task<ActionResult<VitalSignsReading>> RecordVitalSigns([FromBody] VitalSignsReading reading)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Get user ID from claims
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                {
                    return Unauthorized("Invalid user ID");
                }

                reading.PatientId = int.Parse(userIdClaim); // Assuming PatientId can be converted from string userId
                reading.RecordedAt = DateTime.UtcNow;

                var savedReading = await _vitalSignsService.SaveVitalSignsAsync(reading);
                
                _logger.LogInformation("Vital signs recorded for user {UserId} at {Timestamp}", userIdClaim, reading.RecordedAt);
                
                return CreatedAtAction(nameof(GetVitalSigns), new { id = savedReading.Id }, savedReading);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error recording vital signs for user");
                return StatusCode(500, "An error occurred while recording vital signs");
            }
        }

        /// <summary>
        /// Get vital signs reading by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<VitalSignsReading>> GetVitalSigns(int id)
        {
            try
            {
                var reading = await _vitalSignsService.GetVitalSignsAsync(id);
                if (reading == null)
                {
                    return NotFound($"Vital signs reading with ID {id} not found");
                }

                // Check if user has access to this reading
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                {
                    return Unauthorized("Invalid user ID");
                }

                // For now, we'll assume the user can access their own readings
                // In a real implementation, you'd check if reading.PatientId matches the user's ID
                if (reading.RecordedByUserId?.ToString() != userIdClaim && !User.IsInRole("Admin"))
                {
                    return Forbid("Access denied to this vital signs reading");
                }

                return Ok(reading);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving vital signs reading {Id}", id);
                return StatusCode(500, "An error occurred while retrieving vital signs");
            }
        }

        /// <summary>
        /// Get all vital signs readings for the current user
        /// </summary>
        [HttpGet("user")]
        public async Task<ActionResult<IEnumerable<VitalSignsReading>>> GetUserVitalSigns(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 50)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized("Invalid user ID");
                }

                var readings = await _vitalSignsService.GetUserVitalSignsAsync(userId, startDate, endDate, pageNumber, pageSize);
                
                return Ok(readings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user vital signs");
                return StatusCode(500, "An error occurred while retrieving vital signs");
            }
        }

        /// <summary>
        /// Get patient vital signs readings (for healthcare providers)
        /// </summary>
        [HttpGet("patient/{patientId}")]
        [Authorize(Roles = "Doctor,Nurse,Admin")]
        public async Task<ActionResult<IEnumerable<VitalSignsReading>>> GetPatientVitalSigns(
            int patientId,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 50)
        {
            try
            {
                var readings = await _vitalSignsService.GetUserVitalSignsAsync(patientId, startDate, endDate, pageNumber, pageSize);
                
                return Ok(readings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving patient {PatientId} vital signs", patientId);
                return StatusCode(500, "An error occurred while retrieving patient vital signs");
            }
        }

        /// <summary>
        /// Analyze vital signs and get health insights
        /// </summary>
        [HttpPost("analyze")]
        public async Task<ActionResult<VitalSignsAnalysis>> AnalyzeVitalSigns([FromBody] VitalSignsReading reading)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var analysis = await _vitalSignsService.AnalyzeVitalSignsAsync(reading);
                
                _logger.LogInformation("Vital signs analysis performed for reading type");
                
                return Ok(analysis);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error analyzing vital signs");
                return StatusCode(500, "An error occurred while analyzing vital signs");
            }
        }

        /// <summary>
        /// Get vital signs trends for a user
        /// </summary>
        [HttpGet("trends")]
        public async Task<ActionResult<object>> GetVitalSignsTrends(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] string trendType = "all")
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized("Invalid user ID");
                }

                var trends = await _vitalSignsService.GetVitalSignsTrendsAsync(userId, startDate, endDate, trendType);
                
                return Ok(trends);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving vital signs trends");
                return StatusCode(500, "An error occurred while retrieving trends");
            }
        }

        /// <summary>
        /// Get active alerts for user's vital signs
        /// </summary>
        [HttpGet("alerts")]
        public async Task<ActionResult<IEnumerable<VitalSignsAlert>>> GetActiveAlerts()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized("Invalid user ID");
                }

                var alerts = await _vitalSignsService.GetActiveAlertsAsync(userId);
                
                return Ok(alerts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving vital signs alerts");
                return StatusCode(500, "An error occurred while retrieving alerts");
            }
        }

        /// <summary>
        /// Update vital signs reading
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<VitalSignsReading>> UpdateVitalSigns(int id, [FromBody] VitalSignsReading reading)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (id != reading.Id)
                {
                    return BadRequest("ID mismatch");
                }

                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                {
                    return Unauthorized("Invalid user ID");
                }

                // Check if user owns this reading
                var existingReading = await _vitalSignsService.GetVitalSignsAsync(id);
                if (existingReading == null)
                {
                    return NotFound();
                }

                if (existingReading.RecordedByUserId?.ToString() != userIdClaim && !User.IsInRole("Admin"))
                {
                    return Forbid("Access denied to this vital signs reading");
                }

                var updatedReading = await _vitalSignsService.UpdateVitalSignsAsync(reading);
                
                _logger.LogInformation("Vital signs reading {Id} updated", id);
                
                return Ok(updatedReading);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating vital signs reading {Id}", id);
                return StatusCode(500, "An error occurred while updating vital signs");
            }
        }

        /// <summary>
        /// Delete vital signs reading
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteVitalSigns(int id)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                {
                    return Unauthorized("Invalid user ID");
                }

                // Check if user owns this reading
                var existingReading = await _vitalSignsService.GetVitalSignsAsync(id);
                if (existingReading == null)
                {
                    return NotFound();
                }

                if (existingReading.RecordedByUserId?.ToString() != userIdClaim && !User.IsInRole("Admin"))
                {
                    return Forbid("Access denied to this vital signs reading");
                }

                await _vitalSignsService.DeleteVitalSignsAsync(id);
                
                _logger.LogInformation("Vital signs reading {Id} deleted", id);
                
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting vital signs reading {Id}", id);
                return StatusCode(500, "An error occurred while deleting vital signs");
            }
        }

        /// <summary>
        /// Get vital signs statistics for dashboard
        /// </summary>
        [HttpGet("statistics")]
        public async Task<ActionResult<object>> GetVitalSignsStatistics(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized("Invalid user ID");
                }

                var statistics = await _vitalSignsService.GetVitalSignsStatisticsAsync(userId, startDate, endDate);
                
                return Ok(statistics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving vital signs statistics");
                return StatusCode(500, "An error occurred while retrieving statistics");
            }
        }
    }
}
