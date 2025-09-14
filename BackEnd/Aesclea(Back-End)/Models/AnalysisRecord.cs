// Copyright (c) 2025 Alexander Sinapov | Simeon Petkov

// All rights reserved.
// This code is proprietary and confidential.  
// Unauthorized copying, modification, distribution, or use is strictly prohibited.

using System;
using System.ComponentModel.DataAnnotations;

namespace Aesclea_Back_End_.Models
{
    public class AnalysisRecord
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        
        [Required]
        public string PatientId { get; set; } = string.Empty;
        
        public string PatientName { get; set; } = string.Empty;
        
        [Required]
        public string AnalysisType { get; set; } = string.Empty; // "tumor", "vital-signs", "diagnosis", "enhanced-text"
        
        [Required]
        public string Status { get; set; } = "pending"; // "pending", "processing", "completed", "failed"
        
        public string Department { get; set; } = string.Empty;
        
        public string? Notes { get; set; }
        
        public string? Data { get; set; } // JSON string containing analysis input data
        
        public string? Results { get; set; } // JSON string containing analysis results
        
        public double? Confidence { get; set; }
        
        public string? Recommendations { get; set; } // JSON string containing recommendations
        
        [Required]
        public string CreatedBy { get; set; } = string.Empty; // User ID who created the analysis
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? CompletedAt { get; set; }
        
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation property
        public virtual User CreatedByUser { get; set; } = null!;
        public virtual Patient? Patient { get; set; }
    }
}
