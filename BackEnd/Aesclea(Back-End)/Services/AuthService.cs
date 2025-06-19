using Aesclea_Back_End_.Models;
using Supabase.Gotrue;
using AppUser = Aesclea_Back_End_.Models.User;

namespace Aesclea_Back_End_.Services
{
    public class AuthService : IAuthService
    {
        private readonly Supabase.Client _supabase;
        private readonly ILogger<AuthService> _logger;

        public AuthService(Supabase.Client supabase, ILogger<AuthService> logger)
        {
            _supabase = supabase;
            _logger = logger;
        }

        public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
        {
            try
            {
                _logger.LogInformation("Starting registration for email: {Email}", request.Email);
                
                // Create user metadata for Supabase Auth
                var options = new SignUpOptions
                {
                    Data = new Dictionary<string, object>
                    {
                        ["first_name"] = request.FirstName,
                        ["last_name"] = request.LastName,
                        ["phone"] = request.Phone,
                        ["role"] = request.Role,
                        ["hospital"] = request.Hospital
                    }
                };

                _logger.LogInformation("Calling Supabase Auth SignUp for email: {Email}", request.Email);
                var response = await _supabase.Auth.SignUp(request.Email, request.Password, options);

                if (response?.User == null)
                {
                    _logger.LogWarning("Supabase Auth SignUp returned null user for email: {Email}", request.Email);
                    return new AuthResponse
                    {
                        Success = false,
                        Message = "Registration failed. Please try again."
                    };
                }

                _logger.LogInformation("Supabase Auth successful, creating user profile for: {Email}", request.Email);

                // Create user profile in your users table
                var user = new AppUser
                {
                    Id = response.User.Id,
                    Email = request.Email,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Phone = request.Phone,
                    Role = request.Role,
                    Hospital = request.Hospital,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                try
                {
                    // Insert user profile into database
                    _logger.LogInformation("Inserting user profile into database for: {Email}", request.Email);
                    await _supabase
                        .From<AppUser>()
                        .Insert(user);

                    _logger.LogInformation("User profile created successfully for: {Email}", request.Email);
                }
                catch (Exception dbEx)
                {
                    _logger.LogError(dbEx, "Failed to create user profile in database for: {Email}", request.Email);
                    // Auth user was created but profile insertion failed
                    // You might want to handle this scenario differently
                }

                return new AuthResponse
                {
                    Success = true,
                    Message = "Registration successful. Please check your email for verification.",
                    AccessToken = response.AccessToken,
                    RefreshToken = response.RefreshToken,
                    User = user
                };
            }
            catch (Supabase.Gotrue.Exceptions.GotrueException gex)
            {
                _logger.LogError(gex, "Supabase Auth error during registration for email: {Email}", request.Email);
                return new AuthResponse
                {
                    Success = false,
                    Message = $"Registration failed: {gex.Message}"
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
                
                var response = await _supabase.Auth.SignInWithPassword(request.Email, request.Password);

                if (response?.User == null || response == null)
                {
                    _logger.LogWarning("Login failed for email: {Email} - Invalid credentials", request.Email);
                    return new AuthResponse
                    {
                        Success = false,
                        Message = "Invalid email or password."
                    };
                }

                // Get user profile from database
                var userResponse = await _supabase
                    .From<AppUser>()
                    .Where(u => u.Id == response.User.Id)
                    .Single();

                _logger.LogInformation("Login successful for email: {Email}", request.Email);

                return new AuthResponse
                {
                    Success = true,
                    Message = "Login successful.",
                    AccessToken = response.AccessToken,
                    RefreshToken = response.RefreshToken,
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
        }

        public async Task<AuthResponse> LogoutAsync(string accessToken)
        {
            try
            {
                await _supabase.Auth.SignOut();
                
                return new AuthResponse
                {
                    Success = true,
                    Message = "Logout successful."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout");
                return new AuthResponse
                {
                    Success = false,
                    Message = "Logout failed."
                };
            }
        }

        public async Task<AppUser?> GetUserAsync(string userId)
        {
            try
            {
                var user = await _supabase
                    .From<AppUser>()
                    .Where(u => u.Id == userId)
                    .Single();

                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user");
                return null;
            }
        }
    }
}