using Microsoft.AspNetCore.Mvc;

namespace Aesclea_Back_End_.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        [HttpGet("stats")]
        public IActionResult GetSystemStats()
        {
            // Mock system statistics
            var stats = new
            {
                TotalUsers = 1247,
                ActiveSubscriptions = 892,
                TotalRevenue = 125000,
                MonthlyRevenue = 45000,
                SystemHealth = "healthy",
                ServerLoad = 78,
                SystemLoad = 65,
                ActiveSessions = 234,
                DatabaseSize = 12.5,
                DatabaseStatus = "online",
                ApiRequests24h = 15420,
                ErrorRate = 0.02
            };

            return Ok(stats);
        }

        [HttpGet("users")]
        public IActionResult GetUsers()
        {
            // Mock user data
            var users = new[]
            {
                new
                {
                    Id = "1",
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "john.doe@hospital.com",
                    Role = "admin",
                    Department = "Cardiology",
                    IsActive = true,
                    LastLogin = "2025-07-29T10:30:00Z",
                    CreatedAt = "2025-01-15T09:00:00Z"
                },
                new
                {
                    Id = "2",
                    FirstName = "Jane",
                    LastName = "Smith",
                    Email = "jane.smith@hospital.com",
                    Role = "manager",
                    Department = "Neurology",
                    IsActive = true,
                    LastLogin = "2025-07-29T08:45:00Z",
                    CreatedAt = "2025-02-10T14:30:00Z"
                },
                new
                {
                    Id = "3",
                    FirstName = "Dr. Robert",
                    LastName = "Johnson",
                    Email = "robert.johnson@hospital.com",
                    Role = "super_admin",
                    Department = "Administration",
                    IsActive = true,
                    LastLogin = "2025-07-29T07:15:00Z",
                    CreatedAt = "2024-12-01T08:00:00Z"
                }
            };

            return Ok(users);
        }

        [HttpPost("users")]
        public IActionResult CreateUser([FromBody] CreateUserRequest request)
        {
            // Mock user creation
            var newUser = new
            {
                Id = Guid.NewGuid().ToString(),
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Role = request.Role,
                Department = request.Department,
                IsActive = true,
                LastLogin = (string?)null,
                CreatedAt = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ")
            };

            return Ok(new { success = true, user = newUser });
        }

        [HttpPut("users/{id}")]
        public IActionResult UpdateUser(string id, [FromBody] UpdateUserRequest request)
        {
            // Mock user update
            var updatedUser = new
            {
                Id = id,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Role = request.Role,
                Department = request.Department,
                IsActive = request.IsActive,
                LastLogin = "2025-07-29T10:30:00Z",
                CreatedAt = "2025-01-15T09:00:00Z"
            };

            return Ok(new { success = true, user = updatedUser });
        }

        [HttpDelete("users/{id}")]
        public IActionResult DeleteUser(string id)
        {
            // Mock user deletion
            return Ok(new { success = true, message = "User deleted successfully" });
        }

        [HttpGet("audit-logs")]
        public IActionResult GetAuditLogs()
        {
            // Mock audit log data
            var logs = new[]
            {
                new
                {
                    Id = "1",
                    UserId = "1",
                    UserName = "John Doe",
                    Action = "User Login",
                    Resource = "Authentication",
                    Timestamp = "2025-07-29T10:30:00Z",
                    IpAddress = "192.168.1.100",
                    UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36",
                    Level = "info",
                    Message = "User successfully logged in"
                },
                new
                {
                    Id = "2",
                    UserId = "2",
                    UserName = "Jane Smith",
                    Action = "Patient Created",
                    Resource = "Patients",
                    Timestamp = "2025-07-29T09:15:00Z",
                    IpAddress = "192.168.1.101",
                    UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36",
                    Level = "info",
                    Message = "New patient record created: ID 12345"
                },
                new
                {
                    Id = "3",
                    UserId = "3",
                    UserName = "Dr. Robert Johnson",
                    Action = "Failed Login Attempt",
                    Resource = "Authentication",
                    Timestamp = "2025-07-29T07:45:00Z",
                    IpAddress = "192.168.1.102",
                    UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36",
                    Level = "warning",
                    Message = "Failed login attempt with incorrect password"
                }
            };

            return Ok(logs);
        }
    }

    public class CreateUserRequest
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string? Department { get; set; }
    }

    public class UpdateUserRequest
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string? Department { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
