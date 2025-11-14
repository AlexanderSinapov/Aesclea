// Copyright (c) 2025 Alexander Sinapov | Simeon Petkov

// All rights reserved.
// This code is proprietary and confidential.  
// Unauthorized copying, modification, distribution, or use is strictly prohibited.

using System.ComponentModel.DataAnnotations;

namespace Aesclea_Back_End_.Models 
{
    public class RegisterRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;

        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [Phone]
        public string Phone { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = string.Empty;

        [Required]
        public string MedicalNumber { get; set; } = string.Empty;

        [Required]
        public string Hospital { get; set; } = string.Empty;
    }

    public class LoginRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        public string Password { get; set; } = string.Empty;
    }

    public class AuthResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public User? User { get; set; }
    }

    public class User
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string MedicalNumber { get; set; } = string.Empty;
        public string Hospital { get; set; } = string.Empty;
        
        // Settings-related fields
        public string? Department { get; set; }
        public string? Specialization { get; set; } // Medical specialization for doctors
        public string? Avatar { get; set; } // URL or base64 string for profile photo
        public string Theme { get; set; } = "system"; // light, dark, system
        public string Language { get; set; } = "en"; // ISO language code
        public string Timezone { get; set; } = "UTC"; // Timezone identifier
        public bool TwoFactorEnabled { get; set; } = false;
        
        // Notification preferences
        public bool NotifyAppointments { get; set; } = true;
        public bool NotifyPatientUpdates { get; set; } = true;
        public bool NotifyAnalysisResults { get; set; } = true;
        public bool NotifyBilling { get; set; } = false;
        public bool NotifySystem { get; set; } = true;
        public bool NotifyAppointmentsPush { get; set; } = true;
        public bool NotifyPatientUpdatesPush { get; set; } = false;
        public bool NotifyAnalysisResultsPush { get; set; } = true;
        public bool NotifyBillingPush { get; set; } = false;
        public bool NotifySystemPush { get; set; } = false;
        
        // Existing fields
        public bool IsEmailVerified { get; set; } = false;
        public string? EmailVerificationToken { get; set; }
        public DateTime? EmailVerificationTokenExpires { get; set; }
        public string? PasswordResetToken { get; set; }
        public DateTime? PasswordResetTokenExpiry { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

    // Settings-related DTOs
    public class UpdateProfileRequest
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Phone { get; set; }
        public string? Department { get; set; }
        public string? Specialization { get; set; }
        public string? Role { get; set; }
    }

    public class ChangePasswordRequest
    {
        [Required]
        public string CurrentPassword { get; set; } = string.Empty;
        
        [Required]
        [MinLength(8)]
        public string NewPassword { get; set; } = string.Empty;
    }

    public class UpdatePreferencesRequest
    {
        public string? Theme { get; set; }
        public string? Language { get; set; }
        public string? Timezone { get; set; }
    }

    public class UpdateNotificationSettingsRequest
    {
        public bool? NotifyAppointments { get; set; }
        public bool? NotifyPatientUpdates { get; set; }
        public bool? NotifyAnalysisResults { get; set; }
        public bool? NotifyBilling { get; set; }
        public bool? NotifySystem { get; set; }
        public bool? NotifyAppointmentsPush { get; set; }
        public bool? NotifyPatientUpdatesPush { get; set; }
        public bool? NotifyAnalysisResultsPush { get; set; }
        public bool? NotifyBillingPush { get; set; }
        public bool? NotifySystemPush { get; set; }
    }

    public class UpdateAvatarRequest
    {
        [Required]
        public string Avatar { get; set; } = string.Empty; // Base64 encoded image
    }

    public class SettingsResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public User? User { get; set; }
    }
}