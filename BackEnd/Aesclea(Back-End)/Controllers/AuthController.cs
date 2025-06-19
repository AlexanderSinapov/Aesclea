using Aesclea_Back_End_.Models;
using Aesclea_Back_End_.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Aesclea_Back_End_.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            try
            {
                _logger.LogInformation("Registration attempt for email: {Email}", request?.Email ?? "null");
                _logger.LogInformation("Request data: {@Request}", request);
                
                if (request == null)
                {
                    _logger.LogWarning("Registration request is null");
                    return BadRequest(new { success = false, message = "Invalid request data" });
                }
                
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state for registration: {@ModelState}", ModelState);
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                    return BadRequest(new { 
                        success = false, 
                        message = "Validation failed",
                        errors = errors
                    });
                }

                var result = await _authService.RegisterAsync(request);
                
                if (result.Success)
                {
                    _logger.LogInformation("Registration successful for email: {Email}", request.Email);
                    return Ok(result);
                }

                _logger.LogWarning("Registration failed for email: {Email}. Reason: {Message}", request.Email, result.Message);
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception during registration for email: {Email}", request?.Email ?? "unknown");
                return StatusCode(500, new { 
                    success = false, 
                    message = "An internal server error occurred",
                    details = ex.Message // Remove this in production
                });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.LoginAsync(request);
            
            if (result.Success)
            {
                return Ok(result);
            }

            return Unauthorized(result);
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            
            if (string.IsNullOrEmpty(token))
            {
                return BadRequest("Token is required");
            }

            var result = await _authService.LogoutAsync(token);
            return Ok(result);
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userId = HttpContext.User.FindFirst("sub")?.Value;
            
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var user = await _authService.GetUserAsync(userId);
            
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }
    }
}