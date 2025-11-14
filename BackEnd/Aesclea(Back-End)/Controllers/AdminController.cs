// Copyright (c) 2025 Alexander Sinapov | Simeon Petkov

// All rights reserved.
// This code is proprietary and confidential.  
// Unauthorized copying, modification, distribution, or use is strictly prohibited.

using Aesclea_Back_End_.Models;
using Aesclea_Back_End_.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Aesclea_Back_End_.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AdminController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AdminController> _logger;

        public AdminController(IAuthService authService, ILogger<AdminController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetAdminUsers()
        {
            try
            {
                var users = await _authService.GetAllUsersAsync();
                
                // Filter only admin users
                var adminUsers = users.Where(u => 
                    u.Role.ToLower() == "admin" || 
                    u.Role.ToLower() == "super_admin" || 
                    u.Role.ToLower() == "manager"
                ).Select(u => new
                {
                    id = u.Id,
                    firstName = u.FirstName,
                    lastName = u.LastName,
                    email = u.Email,
                    role = u.Role,
                    department = u.Department,
                    isActive = true, // Assuming all users are active
                    lastLogin = (DateTime?)null,
                    createdAt = u.CreatedAt
                });

                return Ok(adminUsers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching admin users");
                return StatusCode(500, new { message = "Failed to fetch admin users" });
            }
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetSystemStats()
        {
            try
            {
                var users = await _authService.GetAllUsersAsync();
                
                var stats = new
                {
                    totalUsers = users.Count,
                    activeSubscriptions = 0, // TODO: Implement subscription counting
                    totalRevenue = 0.0,
                    monthlyRevenue = 0.0,
                    systemHealth = "healthy",
                    serverLoad = 45.2,
                    systemLoad = 32.5,
                    activeSessions = users.Count,
                    databaseSize = 256.8,
                    databaseStatus = "online",
                    apiRequests24h = 15234,
                    errorRate = 0.5
                };

                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching system stats");
                return StatusCode(500, new { message = "Failed to fetch system stats" });
            }
        }

        [HttpGet("audit-logs")]
        public async Task<IActionResult> GetAuditLogs([FromQuery] int limit = 100)
        {
            try
            {
                // TODO: Implement actual audit logging system
                // For now, return empty array or mock data
                var logs = new List<object>();

                return Ok(logs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching audit logs");
                return StatusCode(500, new { message = "Failed to fetch audit logs" });
            }
        }

        [HttpGet("all-users")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _authService.GetAllUsersAsync();
                
                var userList = users.Select(u => new
                {
                    id = u.Id,
                    firstName = u.FirstName,
                    lastName = u.LastName,
                    email = u.Email,
                    role = u.Role,
                    department = u.Department,
                    specialization = u.Specialization,
                    hospital = u.Hospital,
                    medicalNumber = u.MedicalNumber,
                    isEmailVerified = u.IsEmailVerified,
                    createdAt = u.CreatedAt,
                    updatedAt = u.UpdatedAt
                });

                return Ok(userList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all users");
                return StatusCode(500, new { message = "Failed to fetch all users" });
            }
        }

        [HttpGet("doctors")]
        public async Task<IActionResult> GetDoctors()
        {
            try
            {
                var users = await _authService.GetAllUsersAsync();
                
                var doctors = users
                    .Where(u => u.Role.ToLower() == "doctor")
                    .Select(u => new
                    {
                        id = u.Id,
                        firstName = u.FirstName,
                        lastName = u.LastName,
                        email = u.Email,
                        department = u.Department,
                        specialization = u.Specialization,
                        hospital = u.Hospital
                    });

                return Ok(doctors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching doctors");
                return StatusCode(500, new { message = "Failed to fetch doctors" });
            }
        }

        [HttpPost("users")]
        public async Task<IActionResult> CreateAdminUser([FromBody] RegisterRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { message = "Invalid request data" });
                }

                var result = await _authService.RegisterAsync(request);
                
                if (!result.Success)
                {
                    return BadRequest(new { message = result.Message });
                }

                return Ok(new { 
                    success = true, 
                    message = "Admin user created successfully",
                    user = result.User 
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating admin user");
                return StatusCode(500, new { message = "Failed to create admin user" });
            }
        }

        [HttpPut("users/{id}")]
        public async Task<IActionResult> UpdateAdminUser(string id, [FromBody] UpdateProfileRequest request)
        {
            try
            {
                var user = await _authService.GetUserByIdAsync(id);
                if (user == null)
                {
                    return NotFound(new { message = "User not found" });
                }

                var updatedUser = await _authService.UpdateUserProfileAsync(id, request);
                
                return Ok(new { 
                    success = true, 
                    message = "User updated successfully",
                    user = updatedUser 
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating admin user");
                return StatusCode(500, new { message = "Failed to update admin user" });
            }
        }

        [HttpDelete("users/{id}")]
        public async Task<IActionResult> DeleteAdminUser(string id)
        {
            try
            {
                var result = await _authService.DeleteUserAsync(id);
                
                if (!result)
                {
                    return NotFound(new { message = "User not found" });
                }

                return Ok(new { 
                    success = true, 
                    message = "User deleted successfully" 
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting admin user");
                return StatusCode(500, new { message = "Failed to delete admin user" });
            }
        }

        [HttpGet("subscriptions")]
        public async Task<IActionResult> GetAllSubscriptions()
        {
            try
            {
                // TODO: Implement actual subscription fetching
                // For now, return empty array
                var subscriptions = new List<object>();

                return Ok(subscriptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching subscriptions");
                return StatusCode(500, new { message = "Failed to fetch subscriptions" });
            }
        }

        [HttpPut("settings")]
        public async Task<IActionResult> UpdateSystemSettings([FromBody] object settings)
        {
            try
            {
                // TODO: Implement system settings update
                return Ok(new { 
                    success = true, 
                    message = "System settings updated successfully" 
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating system settings");
                return StatusCode(500, new { message = "Failed to update system settings" });
            }
        }
    }
}
