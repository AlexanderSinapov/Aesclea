// Copyright (c) 2025 Alexander Sinapov | Simeon Petkov

// All rights reserved.
// This code is proprietary and confidential.  
// Unauthorized copying, modification, distribution, or use is strictly prohibited.

using Aesclea_Back_End_.Models;

namespace Aesclea_Back_End_.Services
{
    public interface IAuthService
    {
        Task<AuthResponse> RegisterAsync(RegisterRequest request);
        Task<AuthResponse> LoginAsync(LoginRequest request);
        Task<AuthResponse> LogoutAsync(string accessToken);
        Task<User?> GetUserAsync(string userId);
        Task<AuthResponse> VerifyEmailAsync(string token);
        Task<AuthResponse> ResendVerificationEmailAsync(string email);
        
        // Settings-related methods
        Task<SettingsResponse> UpdateProfileAsync(string userId, UpdateProfileRequest request);
        Task<SettingsResponse> ChangePasswordAsync(string userId, ChangePasswordRequest request);
        Task<SettingsResponse> UpdatePreferencesAsync(string userId, UpdatePreferencesRequest request);
        Task<SettingsResponse> UpdateNotificationSettingsAsync(string userId, UpdateNotificationSettingsRequest request);
        Task<SettingsResponse> UpdateAvatarAsync(string userId, UpdateAvatarRequest request);
        Task<SettingsResponse> ToggleTwoFactorAsync(string userId);
    }
}