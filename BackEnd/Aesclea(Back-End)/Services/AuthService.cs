// Copyright (c) 2025 Alexander Sinapov | Simeon Petkov

// All rights reserved.
// This code is proprietary and confidential.  
// Unauthorized copying, modification, distribution, or use is strictly prohibited.

using Aesclea_Back_End_.Models;
using Aesclea_Back_End_.Data;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

namespace Aesclea_Back_End_.Services
{
    public class AuthService : IAuthService
    {
        private readonly AescleaDbContext _context;
        private readonly IJwtService _jwtService;
        private readonly IEmailService _emailService;
        private readonly ILogger<AuthService> _logger;

        public AuthService(AescleaDbContext context, IJwtService jwtService, IEmailService emailService, ILogger<AuthService> logger)
        {
            _context = context;
            _jwtService = jwtService;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
        {
            try
            {
                _logger.LogInformation("Starting registration for email: {Email}", request.Email);

                // Check if user already exists
                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email.ToLower() == request.Email.ToLower());

                if (existingUser != null)
                {
                    _logger.LogWarning("Registration failed: User already exists with email: {Email}", request.Email);
                    return new AuthResponse
                    {
                        Success = false,
                        Message = "A user with this email already exists."
                    };
                }

                // Hash the password
                var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

                // Create new user
                var user = new User
                {
                    Id = Guid.NewGuid().ToString(),
                    Email = request.Email.ToLower(),
                    PasswordHash = passwordHash,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Phone = request.Phone,
                    Role = request.Role,
                    Hospital = request.Hospital,
                    IsEmailVerified = false,
                    EmailVerificationToken = Guid.NewGuid().ToString(),
                    EmailVerificationTokenExpires = DateTime.UtcNow.AddHours(24),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                // Save to database
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                _logger.LogInformation("User registered successfully: {Email}", request.Email);

                // Send verification email
                try
                {
                    var emailSent = await _emailService.SendVerificationEmailAsync(
                        user.Email, 
                        user.EmailVerificationToken!, 
                        $"{user.FirstName} {user.LastName}"
                    );
                    
                    if (!emailSent)
                    {
                        _logger.LogWarning("Failed to send verification email to {Email}", user.Email);
                    }
                    else
                    {
                        _logger.LogInformation("Verification email sent successfully to {Email}", user.Email);
                    }
                }
                catch (Exception emailEx)
                {
                    _logger.LogError(emailEx, "Error sending verification email to {Email}", user.Email);
                }

                // Generate tokens (user can login but should verify email)
                var accessToken = _jwtService.GenerateAccessToken(user);
                var refreshToken = _jwtService.GenerateRefreshToken();

                // Create response without password hash
                var userResponse = new User
                {
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Phone = user.Phone,
                    Role = user.Role,
                    Hospital = user.Hospital,
                    IsEmailVerified = user.IsEmailVerified,
                    CreatedAt = user.CreatedAt,
                    UpdatedAt = user.UpdatedAt
                };

                return new AuthResponse
                {
                    Success = true,
                    Message = "Registration successful.",
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    User = userResponse
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during user registration for email: {Email}", request.Email);
                return new AuthResponse
                {
                    Success = false,
                    Message = "Registration failed. Please try again."
                };
            }
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            try
            {
                _logger.LogInformation("Login attempt for email: {Email}", request.Email);

                // Find user by email
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email.ToLower() == request.Email.ToLower());

                if (user == null)
                {
                    _logger.LogWarning("Login failed for email: {Email} - User not found", request.Email);
                    return new AuthResponse
                    {
                        Success = false,
                        Message = "Invalid email or password."
                    };
                }

                // Verify password
                if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                {
                    _logger.LogWarning("Login failed for email: {Email} - Invalid password", request.Email);
                    return new AuthResponse
                    {
                        Success = false,
                        Message = "Invalid email or password."
                    };
                }

                // Update last login time
                user.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                _logger.LogInformation("Login successful for email: {Email}", request.Email);

                // Generate tokens
                var accessToken = _jwtService.GenerateAccessToken(user);
                var refreshToken = _jwtService.GenerateRefreshToken();

                // Create response without password hash
                var userResponse = new User
                {
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Phone = user.Phone,
                    Role = user.Role,
                    Hospital = user.Hospital,
                    IsEmailVerified = user.IsEmailVerified,
                    CreatedAt = user.CreatedAt,
                    UpdatedAt = user.UpdatedAt
                };

                return new AuthResponse
                {
                    Success = true,
                    Message = "Login successful.",
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    User = userResponse
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during user login for email: {Email}", request.Email);
                return new AuthResponse
                {
                    Success = false,
                    Message = "Login failed. Please check your credentials."
                };
            }
        }        public Task<AuthResponse> LogoutAsync(string accessToken)
        {
            try
            {
                // In a more complex implementation, you might want to blacklist the token
                // For now, we'll just return success as the client will remove the token
                
                return Task.FromResult(new AuthResponse
                {
                    Success = true,
                    Message = "Logout successful."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout");
                return Task.FromResult(new AuthResponse
                {
                    Success = false,
                    Message = "Logout failed."
                });
            }
        }

        public async Task<User?> GetUserAsync(string userId)
        {
            try
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Id == userId);

                if (user == null)
                {
                    return null;
                }

                // Return user without password hash
                return new User
                {
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Phone = user.Phone,
                    Role = user.Role,
                    Hospital = user.Hospital,
                    IsEmailVerified = user.IsEmailVerified,
                    CreatedAt = user.CreatedAt,
                    UpdatedAt = user.UpdatedAt
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user with ID: {UserId}", userId);
                return null;
            }
        }

        public async Task<AuthResponse> VerifyEmailAsync(string token)
        {
            try
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.EmailVerificationToken == token && 
                                            u.EmailVerificationTokenExpires > DateTime.UtcNow);

                if (user == null)
                {
                    return new AuthResponse
                    {
                        Success = false,
                        Message = "Invalid or expired verification token."
                    };
                }

                user.IsEmailVerified = true;
                user.EmailVerificationToken = null;
                user.EmailVerificationTokenExpires = null;
                user.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                // Send welcome email
                try
                {
                    await _emailService.SendWelcomeEmailAsync(user.Email, $"{user.FirstName} {user.LastName}");
                }
                catch (Exception emailEx)
                {
                    _logger.LogError(emailEx, "Error sending welcome email to {Email}", user.Email);
                }

                return new AuthResponse
                {
                    Success = true,
                    Message = "Email verified successfully."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying email for token: {Token}", token);
                return new AuthResponse
                {
                    Success = false,
                    Message = "Email verification failed."
                };
            }
        }

        public async Task<AuthResponse> ResendVerificationEmailAsync(string email)
        {
            try
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());

                if (user == null)
                {
                    return new AuthResponse
                    {
                        Success = false,
                        Message = "User not found."
                    };
                }

                if (user.IsEmailVerified)
                {
                    return new AuthResponse
                    {
                        Success = false,
                        Message = "Email is already verified."
                    };
                }

                // Generate new verification token
                user.EmailVerificationToken = Guid.NewGuid().ToString();
                user.EmailVerificationTokenExpires = DateTime.UtcNow.AddHours(24);
                user.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                // Send verification email
                var emailSent = await _emailService.SendVerificationEmailAsync(
                    user.Email, 
                    user.EmailVerificationToken!, 
                    $"{user.FirstName} {user.LastName}"
                );

                if (!emailSent)
                {
                    return new AuthResponse
                    {
                        Success = false,
                        Message = "Failed to send verification email."
                    };
                }

                return new AuthResponse
                {
                    Success = true,
                    Message = "Verification email sent successfully."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resending verification email for {Email}", email);
                return new AuthResponse
                {
                    Success = false,
                    Message = "Failed to resend verification email."
                };
            }
        }

        public async Task<SettingsResponse> UpdateProfileAsync(string userId, UpdateProfileRequest request)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                
                if (user == null)
                {
                    return new SettingsResponse
                    {
                        Success = false,
                        Message = "User not found."
                    };
                }

                // Update fields if provided
                if (!string.IsNullOrEmpty(request.FirstName))
                    user.FirstName = request.FirstName;
                
                if (!string.IsNullOrEmpty(request.LastName))
                    user.LastName = request.LastName;
                
                if (!string.IsNullOrEmpty(request.Phone))
                    user.Phone = request.Phone;
                
                if (!string.IsNullOrEmpty(request.Department))
                    user.Department = request.Department;
                
                if (!string.IsNullOrEmpty(request.Specialization))
                    user.Specialization = request.Specialization;
                
                if (!string.IsNullOrEmpty(request.Role))
                    user.Role = request.Role;

                user.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                return new SettingsResponse
                {
                    Success = true,
                    Message = "Profile updated successfully.",
                    User = user
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating profile for user: {UserId}", userId);
                return new SettingsResponse
                {
                    Success = false,
                    Message = "Failed to update profile."
                };
            }
        }

        public async Task<SettingsResponse> ChangePasswordAsync(string userId, ChangePasswordRequest request)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                
                if (user == null)
                {
                    return new SettingsResponse
                    {
                        Success = false,
                        Message = "User not found."
                    };
                }

                // Verify current password
                if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash))
                {
                    return new SettingsResponse
                    {
                        Success = false,
                        Message = "Current password is incorrect."
                    };
                }

                // Hash and set new password
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
                user.UpdatedAt = DateTime.UtcNow;
                
                await _context.SaveChangesAsync();

                return new SettingsResponse
                {
                    Success = true,
                    Message = "Password changed successfully.",
                    User = user
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing password for user: {UserId}", userId);
                return new SettingsResponse
                {
                    Success = false,
                    Message = "Failed to change password."
                };
            }
        }

        public async Task<SettingsResponse> UpdatePreferencesAsync(string userId, UpdatePreferencesRequest request)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                
                if (user == null)
                {
                    return new SettingsResponse
                    {
                        Success = false,
                        Message = "User not found."
                    };
                }

                // Update preferences if provided
                if (!string.IsNullOrEmpty(request.Theme))
                    user.Theme = request.Theme;
                
                if (!string.IsNullOrEmpty(request.Language))
                    user.Language = request.Language;
                
                if (!string.IsNullOrEmpty(request.Timezone))
                    user.Timezone = request.Timezone;

                user.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                return new SettingsResponse
                {
                    Success = true,
                    Message = "Preferences updated successfully.",
                    User = user
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating preferences for user: {UserId}", userId);
                return new SettingsResponse
                {
                    Success = false,
                    Message = "Failed to update preferences."
                };
            }
        }

        public async Task<SettingsResponse> UpdateNotificationSettingsAsync(string userId, UpdateNotificationSettingsRequest request)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                
                if (user == null)
                {
                    return new SettingsResponse
                    {
                        Success = false,
                        Message = "User not found."
                    };
                }

                // Update notification settings if provided
                if (request.NotifyAppointments.HasValue)
                    user.NotifyAppointments = request.NotifyAppointments.Value;
                
                if (request.NotifyPatientUpdates.HasValue)
                    user.NotifyPatientUpdates = request.NotifyPatientUpdates.Value;
                
                if (request.NotifyAnalysisResults.HasValue)
                    user.NotifyAnalysisResults = request.NotifyAnalysisResults.Value;
                
                if (request.NotifyBilling.HasValue)
                    user.NotifyBilling = request.NotifyBilling.Value;
                
                if (request.NotifySystem.HasValue)
                    user.NotifySystem = request.NotifySystem.Value;
                
                if (request.NotifyAppointmentsPush.HasValue)
                    user.NotifyAppointmentsPush = request.NotifyAppointmentsPush.Value;
                
                if (request.NotifyPatientUpdatesPush.HasValue)
                    user.NotifyPatientUpdatesPush = request.NotifyPatientUpdatesPush.Value;
                
                if (request.NotifyAnalysisResultsPush.HasValue)
                    user.NotifyAnalysisResultsPush = request.NotifyAnalysisResultsPush.Value;
                
                if (request.NotifyBillingPush.HasValue)
                    user.NotifyBillingPush = request.NotifyBillingPush.Value;
                
                if (request.NotifySystemPush.HasValue)
                    user.NotifySystemPush = request.NotifySystemPush.Value;

                user.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                return new SettingsResponse
                {
                    Success = true,
                    Message = "Notification settings updated successfully.",
                    User = user
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating notification settings for user: {UserId}", userId);
                return new SettingsResponse
                {
                    Success = false,
                    Message = "Failed to update notification settings."
                };
            }
        }

        public async Task<SettingsResponse> UpdateAvatarAsync(string userId, UpdateAvatarRequest request)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                
                if (user == null)
                {
                    return new SettingsResponse
                    {
                        Success = false,
                        Message = "User not found."
                    };
                }

                user.Avatar = request.Avatar;
                user.UpdatedAt = DateTime.UtcNow;
                
                await _context.SaveChangesAsync();

                return new SettingsResponse
                {
                    Success = true,
                    Message = "Avatar updated successfully.",
                    User = user
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating avatar for user: {UserId}", userId);
                return new SettingsResponse
                {
                    Success = false,
                    Message = "Failed to update avatar."
                };
            }
        }

        public async Task<SettingsResponse> ToggleTwoFactorAsync(string userId)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                
                if (user == null)
                {
                    return new SettingsResponse
                    {
                        Success = false,
                        Message = "User not found."
                    };
                }

                user.TwoFactorEnabled = !user.TwoFactorEnabled;
                user.UpdatedAt = DateTime.UtcNow;
                
                await _context.SaveChangesAsync();

                return new SettingsResponse
                {
                    Success = true,
                    Message = $"Two-factor authentication {(user.TwoFactorEnabled ? "enabled" : "disabled")} successfully.",
                    User = user
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling two-factor authentication for user: {UserId}", userId);
                return new SettingsResponse
                {
                    Success = false,
                    Message = "Failed to toggle two-factor authentication."
                };
            }
        }
    
        // Admin methods
        public async Task<List<User>> GetAllUsersAsync()
        {
            try
            {
                return await _context.Users.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all users");
                throw;
            }
        }

        public async Task<User?> GetUserByIdAsync(string userId)
        {
            try
            {
                return await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching user by ID: {UserId}", userId);
                throw;
            }
        }

        public async Task<User> UpdateUserProfileAsync(string userId, UpdateProfileRequest request)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                
                if (user == null)
                {
                    throw new InvalidOperationException("User not found");
                }

                if (!string.IsNullOrWhiteSpace(request.FirstName))
                    user.FirstName = request.FirstName;
                
                if (!string.IsNullOrWhiteSpace(request.LastName))
                    user.LastName = request.LastName;
                
                if (!string.IsNullOrWhiteSpace(request.Phone))
                    user.Phone = request.Phone;
                
                if (!string.IsNullOrWhiteSpace(request.Department))
                    user.Department = request.Department;
                
                if (!string.IsNullOrWhiteSpace(request.Specialization))
                    user.Specialization = request.Specialization;
                
                if (!string.IsNullOrWhiteSpace(request.Role))
                    user.Role = request.Role;

                user.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user profile for user: {UserId}", userId);
                throw;
            }
        }

        public async Task<bool> DeleteUserAsync(string userId)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                
                if (user == null)
                {
                    return false;
                }

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();

                _logger.LogInformation("User deleted successfully: {UserId}", userId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user: {UserId}", userId);
                throw;
            }
        }
    }
}
