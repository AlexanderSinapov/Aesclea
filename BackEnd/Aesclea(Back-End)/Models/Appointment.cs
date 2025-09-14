// Copyright (c) 2025 Alexander Sinapov | Simeon Petkov

// All rights reserved.
// This code is proprietary and confidential.  
// Unauthorized copying, modification, distribution, or use is strictly prohibited.

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aesclea_Back_End_.Models
{
    public class Appointment
    {
        [Key]
        public string Id { get; set; } = string.Empty;

        [Required]
        public string PatientId { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string PatientName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Department { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string Doctor { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string AppointmentType { get; set; } = string.Empty;

        [Required]
        public DateTime DateTime { get; set; }

        [Required]
        public int Duration { get; set; } // Duration in minutes

        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = "scheduled"; // scheduled, completed, cancelled, rescheduled

        [Required]
        [MaxLength(50)]
        public string Priority { get; set; } = "normal"; // normal, urgent, emergency

        [MaxLength(1000)]
        public string? Reason { get; set; }

        [MaxLength(2000)]
        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        [ForeignKey("PatientId")]
        public virtual Patient? Patient { get; set; }
    }
}