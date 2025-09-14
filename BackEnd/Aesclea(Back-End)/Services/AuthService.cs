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
    }
}