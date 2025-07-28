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

                // Generate email verification token
                var verificationToken = Guid.NewGuid().ToString();

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
                    MedicalNumber = request.MedicalNumber,
                    Hospital = request.Hospital,
                    EmailVerified = false,
                    EmailVerificationToken = verificationToken,
                    EmailVerificationTokenExpiry = DateTime.UtcNow.AddHours(24), // 24 hour expiry
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                // Save to database
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                _logger.LogInformation("User registered successfully: {Email}", request.Email);

                // Send verification email
                var emailSent = await _emailService.SendVerificationEmailAsync(user, verificationToken);
                if (!emailSent)
                {
                    _logger.LogWarning("Failed to send verification email to: {Email}", user.Email);
                }

                // Generate tokens (user is registered but not verified)
                var accessToken = _jwtService.GenerateAccessToken(user);
                var refreshToken = _jwtService.GenerateRefreshToken();

                // Create response without password hash and verification token
                var userResponse = new User
                {
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Phone = user.Phone,
                    Role = user.Role,
                    MedicalNumber = user.MedicalNumber,
                    Hospital = user.Hospital,
                    EmailVerified = user.EmailVerified,
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
                    .AsNoTracking() // Ensure fresh data from database
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

                // Debug: Log the email verification status from database
                _logger.LogInformation("User found - Email: {Email}, EmailVerified: {EmailVerified}", user.Email, user.EmailVerified);

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
                    EmailVerified = user.EmailVerified, // Include email verification status
                    CreatedAt = user.CreatedAt,
                    UpdatedAt = user.UpdatedAt
                };

                // Debug: Log what we're sending back
                _logger.LogInformation("Sending user response - Email: {Email}, EmailVerified: {EmailVerified}", 
                    userResponse.Email, userResponse.EmailVerified);

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
                    MedicalNumber = user.MedicalNumber,
                    Hospital = user.Hospital,
                    EmailVerified = user.EmailVerified,
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
                    .FirstOrDefaultAsync(u => u.EmailVerificationToken == token 
                        && u.EmailVerificationTokenExpiry > DateTime.UtcNow);

                if (user == null)
                {
                    return new AuthResponse
                    {
                        Success = false,
                        Message = "Invalid or expired verification token."
                    };
                }

                // Update user's email verification status
                user.EmailVerified = true;
                user.EmailVerificationToken = null;
                user.EmailVerificationTokenExpiry = null;
                user.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Email verified successfully for user: {Email}", user.Email);

                // Send welcome email
                var emailSent = await _emailService.SendWelcomeEmailAsync(user);
                if (!emailSent)
                {
                    _logger.LogWarning("Failed to send welcome email to: {Email}", user.Email);
                }

                return new AuthResponse
                {
                    Success = true,
                    Message = "Email verified successfully!"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying email with token: {Token}", token);
                return new AuthResponse
                {
                    Success = false,
                    Message = "An error occurred while verifying your email."
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
                    // Don't reveal if email exists for security
                    return new AuthResponse
                    {
                        Success = true,
                        Message = "If the email address is registered, a verification email has been sent."
                    };
                }

                if (user.EmailVerified)
                {
                    return new AuthResponse
                    {
                        Success = false,
                        Message = "Email address is already verified."
                    };
                }

                // Generate new verification token
                var verificationToken = Guid.NewGuid().ToString();
                user.EmailVerificationToken = verificationToken;
                user.EmailVerificationTokenExpiry = DateTime.UtcNow.AddHours(24);
                user.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                // Send verification email
                var emailSent = await _emailService.SendVerificationEmailAsync(user, verificationToken);
                if (!emailSent)
                {
                    _logger.LogWarning("Failed to resend verification email to: {Email}", user.Email);
                    return new AuthResponse
                    {
                        Success = false,
                        Message = "Failed to send verification email. Please try again later."
                    };
                }

                _logger.LogInformation("Verification email resent to: {Email}", email);

                return new AuthResponse
                {
                    Success = true,
                    Message = "If the email address is registered, a verification email has been sent."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resending verification email to: {Email}", email);
                return new AuthResponse
                {
                    Success = false,
                    Message = "An error occurred while sending the verification email."
                };
            }
        }
    }
}
