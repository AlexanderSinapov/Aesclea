// Copyright (c) 2025 Alexander Sinapov | Simeon Petkov

// All rights reserved.
// This code is proprietary and confidential.  
// Unauthorized copying, modification, distribution, or use is strictly prohibited.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Aesclea_Back_End_.Models
{
    public class SubscriptionPlan
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        
        [Required]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        public decimal Price { get; set; }
        
        [Required]
        public string Interval { get; set; } = "month"; // "month" or "year"
        
        public List<string> Features { get; set; } = new();
        
        public int MaxPatients { get; set; } = -1; // -1 for unlimited
        
        public int AiAnalysisLimit { get; set; } = -1; // -1 for unlimited
        
        public int Priority { get; set; } = 0;
        
        public bool Recommended { get; set; } = false;
        
        public bool IsActive { get; set; } = true;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

    public class UserSubscription
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        
        [Required]
        public string UserId { get; set; } = string.Empty;
        
        [Required]
        public string PlanId { get; set; } = string.Empty;
        
        [Required]
        public string Status { get; set; } = "active"; // "active", "canceled", "past_due", "trialing", "incomplete"
        
        public DateTime CurrentPeriodStart { get; set; } = DateTime.UtcNow;
        
        public DateTime CurrentPeriodEnd { get; set; } = DateTime.UtcNow.AddMonths(1);
        
        public bool CancelAtPeriodEnd { get; set; } = false;
        
        public DateTime? CanceledAt { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public virtual User User { get; set; } = null!;
        public virtual SubscriptionPlan Plan { get; set; } = null!;
    }

    public class SubscriptionUsage
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        
        [Required]
        public string UserId { get; set; } = string.Empty;
        
        [Required]
        public string Feature { get; set; } = string.Empty; // "ai_analysis", "patient_storage", etc.
        
        public int UsageCount { get; set; } = 0;
        
        public DateTime ResetDate { get; set; } = DateTime.UtcNow.AddMonths(1);
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation property
        public virtual User User { get; set; } = null!;
    }
}
