using Aesclea_Back_End_.Models;

namespace Aesclea_Back_End_.Services
{
    public interface ISubscriptionService
    {
        Task<List<SubscriptionPlan>> GetAvailablePlansAsync();
        Task<UserSubscription?> GetUserSubscriptionAsync(string userId);
        Task<UserSubscription> SubscribeToPlanAsync(string userId, string planId, string paymentMethodId);
        Task<UserSubscription> CancelSubscriptionAsync(string subscriptionId, string userId);
        Task<UserSubscription> ReactivateSubscriptionAsync(string subscriptionId, string userId);
        Task<UserSubscription> ChangePlanAsync(string userId, string newPlanId, string paymentMethodId);
        Task SeedSubscriptionPlansAsync();
    }
}
