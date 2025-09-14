// Copyright (c) 2025 Alexander Sinapov | Simeon Petkov

// All rights reserved.
// This code is proprietary and confidential.  
// Unauthorized copying, modification, distribution, or use is strictly prohibited.

using Aesclea_Back_End_.Models;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Aesclea_Back_End_.Services
{
    public interface ISubscriptionService
    {
        Task<List<SubscriptionPlan>> GetAvailablePlansAsync();
        Task<UserSubscription?> GetUserSubscriptionAsync(string userId);
        Task<bool> CreateSubscriptionAsync(string userId, string planId);
        Task<bool> CancelSubscriptionAsync(string userId);
        Task<bool> UpdateSubscriptionAsync(string userId, string newPlanId);
        Task SeedSubscriptionPlansAsync();
        Task<bool> CheckSubscriptionLimitsAsync(string userId, string feature);
    }
}
