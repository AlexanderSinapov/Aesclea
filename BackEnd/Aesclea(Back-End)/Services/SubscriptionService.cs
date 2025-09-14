// Copyright (c) 2025 Alexander Sinapov | Simeon Petkov

// All rights reserved.
// This code is proprietary and confidential.  
// Unauthorized copying, modification, distribution, or use is strictly prohibited.

using Aesclea_Back_End_.Data;
using Aesclea_Back_End_.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Aesclea_Back_End_.Services
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly AescleaDbContext _context;
        private readonly ILogger<SubscriptionService> _logger;

        public SubscriptionService(AescleaDbContext context, ILogger<SubscriptionService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<SubscriptionPlan>> GetAvailablePlansAsync()
        {
            try
            {
                return await _context.SubscriptionPlans
                    .Where(p => p.IsActive)
                    .OrderBy(p => p.Priority)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching subscription plans");
                return new List<SubscriptionPlan>();
            }
        }

        public async Task<UserSubscription?> GetUserSubscriptionAsync(string userId)
        {
            try
            {
                return await _context.UserSubscriptions
                    .Include(s => s.Plan)
                    .Include(s => s.User)
                    .FirstOrDefaultAsync(s => s.UserId == userId && 
                                           (s.Status == "active" || s.Status == "trialing"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching user subscription for user {UserId}", userId);
                return null;
            }
        }

        public async Task<bool> CreateSubscriptionAsync(string userId, string planId)
        {
            try
            {
                var plan = await _context.SubscriptionPlans.FindAsync(planId);
                if (plan == null)
                {
                    _logger.LogWarning("Plan {PlanId} not found", planId);
                    return false;
                }

                // Cancel any existing subscription
                var existingSubscription = await _context.UserSubscriptions
                    .FirstOrDefaultAsync(s => s.UserId == userId && 
                                            (s.Status == "active" || s.Status == "trialing"));

                if (existingSubscription != null)
                {
                    existingSubscription.Status = "canceled";
                    existingSubscription.CanceledAt = DateTime.UtcNow;
                    existingSubscription.UpdatedAt = DateTime.UtcNow;
                }

                // Create new subscription
                var subscription = new UserSubscription
                {
                    UserId = userId,
                    PlanId = planId,
                    Status = "active",
                    CurrentPeriodStart = DateTime.UtcNow,
                    CurrentPeriodEnd = plan.Interval == "year" 
                        ? DateTime.UtcNow.AddYears(1) 
                        : DateTime.UtcNow.AddMonths(1)
                };

                _context.UserSubscriptions.Add(subscription);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Created subscription for user {UserId} with plan {PlanId}", userId, planId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating subscription for user {UserId}", userId);
                return false;
            }
        }

        public async Task<bool> CancelSubscriptionAsync(string userId)
        {
            try
            {
                var subscription = await _context.UserSubscriptions
                    .FirstOrDefaultAsync(s => s.UserId == userId && 
                                            (s.Status == "active" || s.Status == "trialing"));

                if (subscription == null)
                {
                    _logger.LogWarning("No active subscription found for user {UserId}", userId);
                    return false;
                }

                subscription.Status = "canceled";
                subscription.CanceledAt = DateTime.UtcNow;
                subscription.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Canceled subscription for user {UserId}", userId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error canceling subscription for user {UserId}", userId);
                return false;
            }
        }

        public async Task<bool> UpdateSubscriptionAsync(string userId, string newPlanId)
        {
            try
            {
                // Cancel existing and create new subscription
                await CancelSubscriptionAsync(userId);
                return await CreateSubscriptionAsync(userId, newPlanId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating subscription for user {UserId}", userId);
                return false;
            }
        }

        public async Task SeedSubscriptionPlansAsync()
        {
            try
            {
                // Check if plans already exist
                if (await _context.SubscriptionPlans.AnyAsync())
                {
                    _logger.LogInformation("Subscription plans already exist, skipping seed");
                    return;
                }

                var plans = new List<SubscriptionPlan>
                {
                    new SubscriptionPlan
                    {
                        Id = "free",
                        Name = "Free",
                        Price = 0,
                        Interval = "month",
                        Features = new List<string> { "Basic patient management", "5 AI analyses per month", "Standard support" },
                        MaxPatients = 10,
                        AiAnalysisLimit = 5,
                        Priority = 1
                    },
                    new SubscriptionPlan
                    {
                        Id = "professional",
                        Name = "Professional",
                        Price = 29.99m,
                        Interval = "month",
                        Features = new List<string> { "Advanced patient management", "50 AI analyses per month", "Priority support", "Advanced reporting" },
                        MaxPatients = 100,
                        AiAnalysisLimit = 50,
                        Priority = 2,
                        Recommended = true
                    },
                    new SubscriptionPlan
                    {
                        Id = "enterprise",
                        Name = "Enterprise",
                        Price = 99.99m,
                        Interval = "month",
                        Features = new List<string> { "Unlimited patient management", "Unlimited AI analyses", "24/7 support", "Custom integrations", "Advanced analytics" },
                        MaxPatients = -1,
                        AiAnalysisLimit = -1,
                        Priority = 3
                    }
                };

                _context.SubscriptionPlans.AddRange(plans);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Successfully seeded {Count} subscription plans", plans.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error seeding subscription plans");
            }
        }

        public async Task<bool> CheckSubscriptionLimitsAsync(string userId, string feature)
        {
            try
            {
                var subscription = await GetUserSubscriptionAsync(userId);
                if (subscription == null)
                {
                    _logger.LogWarning("No active subscription found for user {UserId}", userId);
                    return false;
                }

                // Check feature-specific limits
                switch (feature.ToLower())
                {
                    case "ai_analysis":
                        if (subscription.Plan.AiAnalysisLimit == -1) return true; // Unlimited
                        
                        var usage = await GetOrCreateUsageAsync(userId, "ai_analysis");
                        return usage.UsageCount < subscription.Plan.AiAnalysisLimit;

                    case "patient_storage":
                        if (subscription.Plan.MaxPatients == -1) return true; // Unlimited
                        
                        var patientCount = await _context.Patients.CountAsync(p => p.UserId == userId);
                        return patientCount < subscription.Plan.MaxPatients;

                    default:
                        return true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking subscription limits for user {UserId}", userId);
                return false;
            }
        }

        private async Task<SubscriptionUsage> GetOrCreateUsageAsync(string userId, string feature)
        {
            var usage = await _context.SubscriptionUsages
                .FirstOrDefaultAsync(u => u.UserId == userId && u.Feature == feature);

            if (usage == null)
            {
                usage = new SubscriptionUsage
                {
                    UserId = userId,
                    Feature = feature,
                    UsageCount = 0,
                    ResetDate = DateTime.UtcNow.AddMonths(1)
                };
                _context.SubscriptionUsages.Add(usage);
                await _context.SaveChangesAsync();
            }
            else if (DateTime.UtcNow > usage.ResetDate)
            {
                // Reset usage count if reset date has passed
                usage.UsageCount = 0;
                usage.ResetDate = DateTime.UtcNow.AddMonths(1);
                usage.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }

            return usage;
        }
    }
}
