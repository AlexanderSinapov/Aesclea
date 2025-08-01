using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace Aesclea_Back_End_.Models
{
    public class SubscriptionPlan
    {
        [Key]
        public string Id { get; set; } = string.Empty;
        
        [Required]
        public string Name { get; set; } = string.Empty;
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        
        [Required]
        public string Interval { get; set; } = string.Empty; // month, year
        
        public string FeaturesJson { get; set; } = "[]";
        
        [NotMapped]
        public List<string> Features
        {
            get => string.IsNullOrEmpty(FeaturesJson) ? new List<string>() : JsonSerializer.Deserialize<List<string>>(FeaturesJson) ?? new List<string>();
            set => FeaturesJson = JsonSerializer.Serialize(value);
        }
        
        public int MaxPatients { get; set; }
        public int AiAnalysisLimit { get; set; }
        public int Priority { get; set; }
        public bool Recommended { get; set; } = false;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navigation property
        public virtual ICollection<UserSubscription> UserSubscriptions { get; set; } = new List<UserSubscription>();
    }

    public class UserSubscription
    {
        [Key]
        public string Id { get; set; } = string.Empty;
        
        [Required]
        public string UserId { get; set; } = string.Empty;
        
        [Required]
        public string PlanId { get; set; } = string.Empty;
        
        [Required]
        public string Status { get; set; } = string.Empty; // active, canceled, past_due, trialing, incomplete
        
        public DateTime CurrentPeriodStart { get; set; }
        public DateTime CurrentPeriodEnd { get; set; }
        public bool CancelAtPeriodEnd { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;
        
        [ForeignKey("PlanId")]
        public virtual SubscriptionPlan Plan { get; set; } = null!;
    }

    public class SubscribeRequest
    {
        [Required]
        public string PlanId { get; set; } = string.Empty;
        
        public string PaymentMethodId { get; set; } = string.Empty;
    }
}
