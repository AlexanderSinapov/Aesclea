using Aesclea_Back_End_.Configuration;
using Aesclea_Back_End_.Services;
using Aesclea_Back_End_.AIModel;
using Aesclea_Back_End_.AIModel.Helpers;
using Aesclea_Back_End_.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
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
            });            // Configure Database
            builder.Services.Configure<DatabaseConfig>(
                builder.Configuration.GetSection("Database"));
            
            // Configure JWT
            builder.Services.Configure<JwtConfig>(
                builder.Configuration.GetSection("Jwt"));

            // Add PostgreSQL Database Context
            builder.Services.AddDbContext<AescleaDbContext>(options =>
            {
                var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
                if (string.IsNullOrEmpty(connectionString))
                {
                    throw new InvalidOperationException("Database connection string is missing");
                }
                options.UseNpgsql(connectionString);
            });

            // Add custom services in correct dependency order with explicit registrations
            builder.Services.AddScoped<IJwtService, JwtService>();
            builder.Services.AddScoped<IAuthService, AuthService>();
              // Register base services with no custom dependencies
            builder.Services.AddScoped<Aesclea_Back_End_.Services.FileService>(provider => 
            {
                var configuration = provider.GetRequiredService<IConfiguration>();
                var logger = provider.GetRequiredService<ILogger<Aesclea_Back_End_.Services.FileService>>();
                return new Aesclea_Back_End_.Services.FileService(configuration, logger);
            });
            
            builder.Services.AddScoped<ImageProcessingService>(provider =>
            {
                var logger = provider.GetRequiredService<ILogger<ImageProcessingService>>();
                return new ImageProcessingService(logger);
            });
            
            // Configure TumorClassifier (no dependencies on other custom services)
            builder.Services.AddScoped<TumorClassifier>(provider =>
            {
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
              // Register TumorAnalysisService with explicit dependencies
            builder.Services.AddScoped<TumorAnalysisService>(provider =>
            {
                var tumorClassifier = provider.GetRequiredService<TumorClassifier>();
                var imageProcessingService = provider.GetRequiredService<ImageProcessingService>();
                var fileService = provider.GetRequiredService<Aesclea_Back_End_.Services.FileService>();
                var logger = provider.GetRequiredService<ILogger<TumorAnalysisService>>();
                
                return new TumorAnalysisService(tumorClassifier, imageProcessingService, fileService, logger);
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
            });            // Configure JWT Authentication
            var jwtConfig = builder.Configuration.GetSection("Jwt").Get<JwtConfig>();
            if (jwtConfig != null && !string.IsNullOrEmpty(jwtConfig.Secret))
            {
                builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,
                            ValidIssuer = jwtConfig.Issuer is string issuerProp ? issuerProp : jwtConfig.Issuer.ToString(),
                            ValidAudience = jwtConfig.Audience,
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.Secret))
                        };
                    });
            }

            // Add logging
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
            builder.Logging.AddDebug();
        }        private void ConfigureMiddleware(WebApplication app)
        {
            // Initialize database
            InitializeDatabase(app);

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

        private void InitializeDatabase(WebApplication app)
        {
            try
            {
                using var scope = app.Services.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<AescleaDbContext>();
                
                _logger.LogInformation("Ensuring database is created...");
                context.Database.EnsureCreated();
                _logger.LogInformation("Database initialization completed successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Database initialization failed");
                throw;
            }
        }
    }
}