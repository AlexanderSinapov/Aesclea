using Aesclea_Back_End_.Models;
using Aesclea_Back_End_.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Aesclea_Back_End_.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SubscriptionsController : ControllerBase
    {
        private readonly ISubscriptionService _subscriptionService;
        private readonly IEmailService _emailService;
        private readonly ILogger<SubscriptionsController> _logger;

        public SubscriptionsController(
            ISubscriptionService subscriptionService,
            IEmailService emailService,
            ILogger<SubscriptionsController> logger)
        {
            _subscriptionService = subscriptionService;
            _emailService = emailService;
            _logger = logger;
        }

        private string GetCurrentUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                ?? throw new UnauthorizedAccessException("User ID not found in token");
        }

        [HttpGet("plans")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAvailablePlans()
        {
            try
            {
                var plans = await _subscriptionService.GetAvailablePlansAsync();
                return Ok(plans);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving subscription plans");
                return StatusCode(500, new { message = "Failed to retrieve subscription plans" });
            }
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserSubscription(string userId)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                
                // Users can only access their own subscription
                if (userId != currentUserId)
                {
                    return Forbid("You can only access your own subscription");
                }

                var subscription = await _subscriptionService.GetUserSubscriptionAsync(userId);
                return Ok(subscription);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user subscription for user {UserId}", userId);
                return StatusCode(500, new { message = "Failed to retrieve subscription" });
            }
        }

        [HttpGet("user/current-user")]
        public async Task<IActionResult> GetCurrentUserSubscription()
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                var subscription = await _subscriptionService.GetUserSubscriptionAsync(currentUserId);
                return Ok(subscription);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving current user subscription");
                return StatusCode(500, new { message = "Failed to retrieve subscription" });
            }
        }

        [HttpPost("subscribe")]
        public async Task<IActionResult> Subscribe([FromBody] SubscribeRequest request)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                
                var subscription = await _subscriptionService.SubscribeToPlanAsync(
                    currentUserId, 
                    request.PlanId, 
                    request.PaymentMethodId);

                // Send confirmation email (fire and forget)
                _ = Task.Run(async () =>
                {
                    try
                    {
                        // Get user info for email
                        var user = await GetUserAsync(currentUserId);
                        if (user != null)
                        {
                            await _emailService.SendSubscriptionConfirmationEmailAsync(user, subscription.Plan.Name);
                        }
                    }
                    catch (Exception emailEx)
                    {
                        _logger.LogWarning(emailEx, "Failed to send subscription confirmation email");
                    }
                });

                _logger.LogInformation("User {UserId} successfully subscribed to plan {PlanId}", currentUserId, request.PlanId);
                return Ok(subscription);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating subscription for user");
                return StatusCode(500, new { message = "Subscription failed. Please try again." });
            }
        }

        [HttpPost("cancel/{subscriptionId}")]
        public async Task<IActionResult> CancelSubscription(string subscriptionId)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                
                var subscription = await _subscriptionService.CancelSubscriptionAsync(subscriptionId, currentUserId);
                
                _logger.LogInformation("User {UserId} cancelled subscription {SubscriptionId}", currentUserId, subscriptionId);
                return Ok(new { success = true, message = "Subscription will be cancelled at the end of the current period", subscription });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling subscription {SubscriptionId}", subscriptionId);
                return StatusCode(500, new { message = "Failed to cancel subscription" });
            }
        }

        [HttpPost("reactivate/{subscriptionId}")]
        public async Task<IActionResult> ReactivateSubscription(string subscriptionId)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                
                var subscription = await _subscriptionService.ReactivateSubscriptionAsync(subscriptionId, currentUserId);
                
                _logger.LogInformation("User {UserId} reactivated subscription {SubscriptionId}", currentUserId, subscriptionId);
                return Ok(subscription);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reactivating subscription {SubscriptionId}", subscriptionId);
                return StatusCode(500, new { message = "Failed to reactivate subscription" });
            }
        }

        [HttpPut("change-plan")]
        public async Task<IActionResult> ChangePlan([FromBody] SubscribeRequest request)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                
                var subscription = await _subscriptionService.ChangePlanAsync(
                    currentUserId, 
                    request.PlanId, 
                    request.PaymentMethodId);

                _logger.LogInformation("User {UserId} changed plan to {PlanId}", currentUserId, request.PlanId);
                return Ok(subscription);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing subscription plan for user");
                return StatusCode(500, new { message = "Failed to change plan" });
            }
        }

        private Task<User?> GetUserAsync(string userId)
        {
            // This is a simple way to get user info. In a real app, you might inject UserManager or a UserService
            // For now, we'll return null and handle the email sending gracefully
            return Task.FromResult<User?>(null);
        }
    }
}
