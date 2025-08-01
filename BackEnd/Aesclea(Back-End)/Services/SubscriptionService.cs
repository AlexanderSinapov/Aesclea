using Aesclea_Back_End_.Data;
using Aesclea_Back_End_.Models;
using Microsoft.EntityFrameworkCore;

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
            return await _context.SubscriptionPlans
                .Where(p => p.IsActive)
                .OrderBy(p => p.Priority)
                .ToListAsync();
        }

        public async Task<UserSubscription?> GetUserSubscriptionAsync(string userId)
        {
            return await _context.UserSubscriptions
                .Include(s => s.Plan)
                .Where(s => s.UserId == userId && (s.Status == "active" || s.Status == "trialing"))
                .OrderByDescending(s => s.CreatedAt)
                .FirstOrDefaultAsync();
        }

        public async Task<UserSubscription> SubscribeToPlanAsync(string userId, string planId, string paymentMethodId)
        {
            // Validate plan exists
            var plan = await _context.SubscriptionPlans.FindAsync(planId);
            if (plan == null || !plan.IsActive)
            {
                throw new ArgumentException("Invalid or inactive subscription plan");
            }

            // Validate user exists
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                throw new ArgumentException("User not found");
            }

            // Cancel any existing active subscription
            var existingSubscription = await GetUserSubscriptionAsync(userId);
            if (existingSubscription != null)
            {
                existingSubscription.Status = "canceled";
                existingSubscription.CancelAtPeriodEnd = true;
                existingSubscription.UpdatedAt = DateTime.UtcNow;
            }

            // Create new subscription
            var subscription = new UserSubscription
            {
                Id = Guid.NewGuid().ToString(),
                UserId = userId,
                PlanId = planId,
                Status = "trialing", // Start with trial
                CurrentPeriodStart = DateTime.UtcNow,
                CurrentPeriodEnd = DateTime.UtcNow.AddDays(14), // 14-day trial
                CancelAtPeriodEnd = false,
                CreatedAt = DateTime.UtcNow
            };

            _context.UserSubscriptions.Add(subscription);
            await _context.SaveChangesAsync();

            // Load the subscription with the plan
            return await _context.UserSubscriptions
                .Include(s => s.Plan)
                .FirstAsync(s => s.Id == subscription.Id);
        }

        public async Task<UserSubscription> CancelSubscriptionAsync(string subscriptionId, string userId)
        {
            var subscription = await _context.UserSubscriptions
                .Include(s => s.Plan)
                .FirstOrDefaultAsync(s => s.Id == subscriptionId && s.UserId == userId);

            if (subscription == null)
            {
                throw new ArgumentException("Subscription not found");
            }

            subscription.CancelAtPeriodEnd = true;
            subscription.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return subscription;
        }

        public async Task<UserSubscription> ReactivateSubscriptionAsync(string subscriptionId, string userId)
        {
            var subscription = await _context.UserSubscriptions
                .Include(s => s.Plan)
                .FirstOrDefaultAsync(s => s.Id == subscriptionId && s.UserId == userId);

            if (subscription == null)
            {
                throw new ArgumentException("Subscription not found");
            }

            subscription.CancelAtPeriodEnd = false;
            subscription.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return subscription;
        }

        public async Task<UserSubscription> ChangePlanAsync(string userId, string newPlanId, string paymentMethodId)
        {
            var currentSubscription = await GetUserSubscriptionAsync(userId);
            if (currentSubscription == null)
            {
                throw new ArgumentException("No active subscription found");
            }

            var newPlan = await _context.SubscriptionPlans.FindAsync(newPlanId);
            if (newPlan == null || !newPlan.IsActive)
            {
                throw new ArgumentException("Invalid or inactive subscription plan");
            }

            // Update current subscription
            currentSubscription.PlanId = newPlanId;
            currentSubscription.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // Return updated subscription with plan
            return await _context.UserSubscriptions
                .Include(s => s.Plan)
                .FirstAsync(s => s.Id == currentSubscription.Id);
        }

        public async Task SeedSubscriptionPlansAsync()
        {
            // Check if plans already exist
            var existingPlans = await _context.SubscriptionPlans.AnyAsync();
            if (existingPlans)
            {
                return; // Plans already seeded
            }

            var plans = new List<SubscriptionPlan>
            {
                new SubscriptionPlan
                {
                    Id = "starter",
                    Name = "Starter",
                    Price = 49,
                    Interval = "month",
                    MaxPatients = 50,
                    AiAnalysisLimit = 10,
                    Priority = 1,
                    Features = new List<string>
                    {
                        "Up to 50 patients",
                        "10 AI analyses per month",
                        "Basic reporting",
                        "Email support",
                        "Secure data storage"
                    }
                },
                new SubscriptionPlan
                {
                    Id = "professional",
                    Name = "Professional",
                    Price = 149,
                    Interval = "month",
                    MaxPatients = 200,
                    AiAnalysisLimit = 50,
                    Priority = 2,
                    Recommended = true,
                    Features = new List<string>
                    {
                        "Up to 200 patients",
                        "50 AI analyses per month",
                        "Advanced reporting",
                        "Priority email support",
                        "Secure data storage",
                        "Custom templates",
                        "Team collaboration"
                    }
                },
                new SubscriptionPlan
                {
                    Id = "enterprise",
                    Name = "Enterprise",
                    Price = 399,
                    Interval = "month",
                    MaxPatients = -1,
                    AiAnalysisLimit = -1,
                    Priority = 3,
                    Features = new List<string>
                    {
                        "Unlimited patients",
                        "Unlimited AI analyses",
                        "Advanced reporting & analytics",
                        "24/7 phone & email support",
                        "Secure data storage",
                        "Custom templates",
                        "Team collaboration",
                        "API access"
                    }
                }
            };

            _context.SubscriptionPlans.AddRange(plans);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Subscription plans seeded successfully");
        }
    }
}
