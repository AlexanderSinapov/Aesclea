// Copyright (c) 2025 Alexander Sinapov | Simeon Petkov

// All rights reserved.
// This code is proprietary and confidential.  
// Unauthorized copying, modification, distribution, or use is strictly prohibited.

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Aesclea_Back_End_.Services;
using Aesclea_Back_End_.Models;
using System.Security.Claims;

namespace Aesclea_Back_End_.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SubscriptionsController : ControllerBase
    {
        private readonly ISubscriptionService _subscriptionService;
        private readonly ILogger<SubscriptionsController> _logger;

        public SubscriptionsController(ISubscriptionService subscriptionService, ILogger<SubscriptionsController> logger)
        {
            _subscriptionService = subscriptionService;
            _logger = logger;
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
                _logger.LogError(ex, "Error fetching subscription plans");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpGet("current")]
        public async Task<IActionResult> GetCurrentSubscription()
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return Unauthorized(new { message = "User not authenticated" });
                }

                var subscription = await _subscriptionService.GetUserSubscriptionAsync(userId);
                
                if (subscription == null)
                {
                    return Ok(new { subscription = (object?)null, hasSubscription = false });
                }

                return Ok(new 
                { 
                    subscription = new
                    {
                        id = subscription.Id,
                        planId = subscription.PlanId,
                        status = subscription.Status,
                        currentPeriodStart = subscription.CurrentPeriodStart,
                        currentPeriodEnd = subscription.CurrentPeriodEnd,
                        cancelAtPeriodEnd = subscription.CancelAtPeriodEnd,
                        plan = new
                        {
                            id = subscription.Plan.Id,
                            name = subscription.Plan.Name,
                            price = subscription.Plan.Price,
                            interval = subscription.Plan.Interval,
                            features = subscription.Plan.Features,
                            maxPatients = subscription.Plan.MaxPatients,
                            aiAnalysisLimit = subscription.Plan.AiAnalysisLimit
                        }
                    },
                    hasSubscription = true 
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching current subscription");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpPost("subscribe")]
        public async Task<IActionResult> CreateSubscription([FromBody] CreateSubscriptionRequest request)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return Unauthorized(new { message = "User not authenticated" });
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var success = await _subscriptionService.CreateSubscriptionAsync(userId, request.PlanId);
                
                if (!success)
                {
                    return BadRequest(new { message = "Failed to create subscription. Plan may not exist." });
                }

                // Get the created subscription and return it
                var subscription = await _subscriptionService.GetUserSubscriptionAsync(userId);
                
                if (subscription == null)
                {
                    return StatusCode(500, new { message = "Subscription created but could not be retrieved" });
                }

                return Ok(new 
                { 
                    id = subscription.Id,
                    planId = subscription.PlanId,
                    status = subscription.Status,
                    currentPeriodStart = subscription.CurrentPeriodStart,
                    currentPeriodEnd = subscription.CurrentPeriodEnd,
                    cancelAtPeriodEnd = subscription.CancelAtPeriodEnd,
                    plan = new
                    {
                        id = subscription.Plan.Id,
                        name = subscription.Plan.Name,
                        price = subscription.Plan.Price,
                        interval = subscription.Plan.Interval,
                        features = subscription.Plan.Features,
                        maxPatients = subscription.Plan.MaxPatients,
                        aiAnalysisLimit = subscription.Plan.AiAnalysisLimit,
                        priority = subscription.Plan.Priority,
                        recommended = subscription.Plan.Recommended
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating subscription");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpPost("cancel")]
        public async Task<IActionResult> CancelSubscription()
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return Unauthorized(new { message = "User not authenticated" });
                }

                var success = await _subscriptionService.CancelSubscriptionAsync(userId);
                
                if (!success)
                {
                    return BadRequest(new { message = "Failed to cancel subscription. No active subscription found." });
                }

                return Ok(new { message = "Subscription canceled successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error canceling subscription");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpPut("change-plan")]
        public async Task<IActionResult> ChangePlan([FromBody] ChangeSubscriptionPlanRequest request)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return Unauthorized(new { message = "User not authenticated" });
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var success = await _subscriptionService.UpdateSubscriptionAsync(userId, request.NewPlanId);
                
                if (!success)
                {
                    return BadRequest(new { message = "Failed to update subscription. Plan may not exist." });
                }

                return Ok(new { message = "Subscription plan updated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating subscription plan");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpGet("check-limits/{feature}")]
        public async Task<IActionResult> CheckSubscriptionLimits(string feature)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return Unauthorized(new { message = "User not authenticated" });
                }

                var canAccess = await _subscriptionService.CheckSubscriptionLimitsAsync(userId, feature);
                
                return Ok(new { canAccess, feature });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking subscription limits");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        private string? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return string.IsNullOrEmpty(userIdClaim) ? null : userIdClaim;
        }
    }

    // Request Models
    public class CreateSubscriptionRequest
    {
        public string PlanId { get; set; } = string.Empty;
        public string? PaymentMethodId { get; set; }
    }

    public class ChangeSubscriptionPlanRequest
    {
        public string NewPlanId { get; set; } = string.Empty;
    }
}
