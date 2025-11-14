// Copyright (c) 2025 Alexander Sinapov | Simeon Petkov

// All rights reserved.
// This code is proprietary and confidential.  
// Unauthorized copying, modification, distribution, or use is strictly prohibited.

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aesclea_Back_End_.Models
{
    public class Referral
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        
        [Required]
        public string AppointmentId { get; set; } = string.Empty;
        
        [Required]
        public string PatientId { get; set; } = string.Empty;
        
        [Required]
        public string PatientName { get; set; } = string.Empty;
        
        [Required]
        public string DoctorId { get; set; } = string.Empty;
        
        [Required]
        public string DoctorName { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(200)]
        public string Specialty { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(1000)]
        public string Reason { get; set; } = string.Empty;
        
        [MaxLength(2000)]
        public string? Notes { get; set; }
        
        public DateTime IssuedDate { get; set; } = DateTime.UtcNow;
        
        public DateTime? ExpiryDate { get; set; }
        
        [MaxLength(50)]
        public string Status { get; set; } = "active"; // active, used, expired
        
        [ForeignKey("AppointmentId")]
        public virtual Appointment? Appointment { get; set; }
    }

    // Диагноза (Diagnosis)
    public class Diagnosis
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        
        [Required]
        public string AppointmentId { get; set; } = string.Empty;
        
        [Required]
        public string PatientId { get; set; } = string.Empty;
        
        [Required]
        public string DoctorId { get; set; } = string.Empty;
        
        [MaxLength(20)]
        public string? ICD10Code { get; set; }
        
        [Required]
        [MaxLength(500)]
        public string DiagnosisName { get; set; } = string.Empty;
        
        [MaxLength(50)]
        public string DiagnosisType { get; set; } = "primary";
        
        [MaxLength(2000)]
        public string? ClinicalFindings { get; set; }
        
        [MaxLength(2000)]
        public string? Notes { get; set; }
        
        public DateTime DiagnosedDate { get; set; } = DateTime.UtcNow;
        
        [ForeignKey("AppointmentId")]
        public virtual Appointment? Appointment { get; set; }
    }

    public class Prescription
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        
        [Required]
        public string AppointmentId { get; set; } = string.Empty;
        
        [Required]
        public string PatientId { get; set; } = string.Empty;
        
        [Required]
        public string DoctorId { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(300)]
        public string MedicationName { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(200)]
        public string Dosage { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(200)]
        public string Frequency { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(100)]
        public string Route { get; set; } = string.Empty;
        
        [Required]
        public int Duration { get; set; }
        
        [MaxLength(1000)]
        public string? Instructions { get; set; }
        
        public int Quantity { get; set; }
        
        [MaxLength(2000)]
        public string? Notes { get; set; }
        
        public DateTime PrescribedDate { get; set; } = DateTime.UtcNow;
        
        public DateTime? StartDate { get; set; }
        
        public DateTime? EndDate { get; set; }
        
        [MaxLength(50)]
        public string Status { get; set; } = "active";
        
        [ForeignKey("AppointmentId")]
        public virtual Appointment? Appointment { get; set; }
    }

    public class OutpatientRecord
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        
        [Required]
        public string AppointmentId { get; set; } = string.Empty;
        
        [Required]
        public string PatientId { get; set; } = string.Empty;
        
        [Required]
        public string PatientName { get; set; } = string.Empty;
        
        public string? PatientEGN { get; set; }
        
        [Required]
        public string DoctorId { get; set; } = string.Empty;
        
        [Required]
        public string DoctorName { get; set; } = string.Empty;
        
        public string? DoctorSpecialty { get; set; }
        
        [Required]
        public DateTime VisitDate { get; set; }
        
        [MaxLength(2000)]
        public string? ChiefComplaint { get; set; }
        
        [MaxLength(3000)]
        public string? MedicalHistory { get; set; }
        
        [MaxLength(3000)]
        public string? PhysicalExamination { get; set; }
        
        [MaxLength(2000)]
        public string? VitalSigns { get; set; }
        
        public List<string> DiagnosisIds { get; set; } = new();
        
        public List<string> PrescriptionIds { get; set; } = new();
        
        public List<string> ReferralIds { get; set; } = new();
        
        [MaxLength(3000)]
        public string? TreatmentPlan { get; set; }
        
        [MaxLength(2000)]
        public string? Notes { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? UpdatedAt { get; set; }
        
        [ForeignKey("AppointmentId")]
        public virtual Appointment? Appointment { get; set; }
    }

    // DTOs for requests
    public class CreateReferralRequest
    {
        public string AppointmentId { get; set; } = string.Empty;
        public string PatientId { get; set; } = string.Empty;
        public string Specialty { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public DateTime? ExpiryDate { get; set; }
    }

    public class CreateDiagnosisRequest
    {
        public string AppointmentId { get; set; } = string.Empty;
        public string PatientId { get; set; } = string.Empty;
        public string? ICD10Code { get; set; }
        public string DiagnosisName { get; set; } = string.Empty;
        public string DiagnosisType { get; set; } = "primary";
        public string? ClinicalFindings { get; set; }
        public string? Notes { get; set; }
    }

    public class CreatePrescriptionRequest
    {
        public string AppointmentId { get; set; } = string.Empty;
        public string PatientId { get; set; } = string.Empty;
        public string MedicationName { get; set; } = string.Empty;
        public string Dosage { get; set; } = string.Empty;
        public string Frequency { get; set; } = string.Empty;
        public string Route { get; set; } = string.Empty;
        public int Duration { get; set; }
        public string? Instructions { get; set; }
        public int Quantity { get; set; }
        public DateTime? StartDate { get; set; }
    }

    public class CreateOutpatientRecordRequest
    {
        public string AppointmentId { get; set; } = string.Empty;
        public string PatientId { get; set; } = string.Empty;
        public string? PatientEGN { get; set; }
        public string? ChiefComplaint { get; set; }
        public string? MedicalHistory { get; set; }
        public string? PhysicalExamination { get; set; }
        public string? VitalSigns { get; set; }
        public string? TreatmentPlan { get; set; }
        public string? Notes { get; set; }
    }
}
