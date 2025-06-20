using Aesclea_Back_End_.Configuration;
using Aesclea_Back_End_.Services;
using Aesclea_Back_End_.AIModel;
using Aesclea_Back_End_.AIModel.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Aesclea_Back_End_.Server
{
    public class WebServer
    {
        private WebApplication? _app;
        private readonly ILogger<WebServer> _logger;

        public WebServer()
        {
            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            _logger = loggerFactory.CreateLogger<WebServer>();
        }

        public async Task StartAsync(string[] args)
        {
            try
            {
                var builder = WebApplication.CreateBuilder(args);

                // Configure services
                ConfigureServices(builder);

                _app = builder.Build();

                // Configure middleware pipeline
                ConfigureMiddleware(_app);

                _logger.LogInformation("Starting web server...");
                
                // Start the server
                await _app.RunAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error starting web server");
                throw;
            }
        }

        public async Task StopAsync()
        {
            if (_app != null)
            {
                _logger.LogInformation("Stopping web server...");
                await _app.StopAsync();
            }
        }

        private void ConfigureServices(WebApplicationBuilder builder)
        {
            // Add services to the container
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "Aesclea Medical API",
                    Version = "v1",
                    Description = "Medical management system with AI tumor detection"
                });
            });

            // Configure Supabase
            builder.Services.Configure<SupabaseConfig>(
                builder.Configuration.GetSection("Supabase"));

            // Add Supabase client
            builder.Services.AddScoped<Supabase.Client>(provider =>
            {
                var config = builder.Configuration.GetSection("Supabase").Get<SupabaseConfig>();
                if (config == null || string.IsNullOrEmpty(config.Url) || string.IsNullOrEmpty(config.Key))
                {
                    throw new InvalidOperationException("Supabase configuration is missing or invalid");
                }

                var options = new Supabase.SupabaseOptions
                {
                    AutoConnectRealtime = true,
                    // Add this to handle SSL issues in development
                    AutoRefreshToken = true
                };
                
                var client = new Supabase.Client(config.Url, config.Key, options);
                
                // Initialize the client
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await client.InitializeAsync();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Failed to initialize Supabase client: {ex.Message}");
                    }
                });
                
                return client;
            });

            // Add custom services
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<TumorAnalysisService>();
            builder.Services.AddScoped<ImageProcessingService>();
            builder.Services.AddScoped<FileService>();            // Configure your existing TumorClassifier with required dependencies
            builder.Services.AddScoped<TumorClassifier>(provider =>
            {
                // You'll need to initialize with your existing NeuronNetwork
                // This depends on how you currently create your base network
                var baseNetwork = new NeuronNetwork(new int[] { 16384, 1024, 512, 256, 1 });
                var classifier = new TumorClassifier(baseNetwork);
                
                // Auto-load existing weights if available
                try
                {
                    var fileHelper = new FileHelper();
                    fileHelper.OpenFolder();
                    classifier.LoadWeights(fileHelper, "tgl");
                    Console.WriteLine("✓ API: Successfully loaded pre-trained tumor classifier weights");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️  API: Could not load existing weights: {ex.Message}");
                    Console.WriteLine("   You may need to train the classifier first via console application.");
                }
                
                return classifier;
            });

            // Configure CORS - IMPORTANT: This must be configured properly
            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(corsBuilder =>
                {
                    corsBuilder.WithOrigins(
                        "http://localhost:5173", 
                        "http://localhost:3000",
                        "http://localhost:8080",
                        "http://127.0.0.1:5173"
                    )
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
                });

                // Add CORS for frontend integration
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            // Configure JWT Authentication
            var supabaseConfig = builder.Configuration.GetSection("Supabase").Get<SupabaseConfig>();
            if (supabaseConfig != null && !string.IsNullOrEmpty(supabaseConfig.Url))
            {
                builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        options.Authority = $"{supabaseConfig.Url}/auth/v1";
                        options.Audience = "authenticated";
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,
                            ValidIssuer = $"{supabaseConfig.Url}/auth/v1",
                            ValidAudience = "authenticated"
                        };
                    });
            }

            // Add logging
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
            builder.Logging.AddDebug();
        }

        private void ConfigureMiddleware(WebApplication app)
        {
            // Configure the HTTP request pipeline
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Aesclea Medical API v1");
                    c.RoutePrefix = "swagger";
                });
            }

            // IMPORTANT: CORS must come before Authentication and Authorization
            app.UseCors("AllowAll");
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            // Add health check endpoint
            app.MapGet("/health", () => new { status = "healthy", timestamp = DateTime.UtcNow });

            // Add API info endpoint
            app.MapGet("/api/info", () => new 
            { 
                name = "Aesclea Medical API",
                version = "1.0.0",
                description = "Medical management system with AI tumor detection",
                timestamp = DateTime.UtcNow
            });

            // Add a test endpoint for debugging
            app.MapGet("/api/test", () => new 
            { 
                message = "API is working",
                timestamp = DateTime.UtcNow
            });
        }
    }
}