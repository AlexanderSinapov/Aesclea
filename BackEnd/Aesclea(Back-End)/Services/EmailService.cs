// Copyright (c) 2025 Alexander Sinapov | Simeon Petkov

// All rights reserved.
// This code is proprietary and confidential.  
// Unauthorized copying, modification, distribution, or use is strictly prohibited.

using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Aesclea_Back_End_.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<bool> SendEmailAsync(string to, string subject, string body, bool isHtml = false)
        {
            try
            {
                var emailConfig = _configuration.GetSection("EmailConfiguration");
                var smtpHost = emailConfig["SmtpHost"];
                var smtpPort = int.Parse(emailConfig["SmtpPort"] ?? "587");
                var smtpUsername = emailConfig["SmtpUsername"];
                var smtpPassword = emailConfig["SmtpPassword"];
                var fromEmail = emailConfig["FromEmail"];
                var fromName = emailConfig["FromName"];
                var enableSsl = bool.Parse(emailConfig["EnableSsl"] ?? "true");

                // If email configuration is not properly set up, log and skip email sending
                if (string.IsNullOrEmpty(smtpUsername) || string.IsNullOrEmpty(smtpPassword))
                {
                    _logger.LogWarning("Email configuration is incomplete. SmtpUsername: {Username}, SmtpPassword: {HasPassword}", 
                        smtpUsername, !string.IsNullOrEmpty(smtpPassword) ? "***" : "empty");
                    return false;
                }

                _logger.LogInformation("Attempting to send email to {Email} using SMTP {Host}:{Port}", to, smtpHost, smtpPort);

                using var client = new SmtpClient(smtpHost, smtpPort)
                {
                    Credentials = new NetworkCredential(smtpUsername, smtpPassword),
                    EnableSsl = enableSsl
                };

                using var mailMessage = new MailMessage
                {
                    From = new MailAddress(fromEmail!, fromName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = isHtml
                };

                mailMessage.To.Add(to);

                await client.SendMailAsync(mailMessage);
                _logger.LogInformation("Email sent successfully to {Email}", to);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {Email}: {Message}", to, ex.Message);
                return false;
            }
        }

        public async Task<bool> SendVerificationEmailAsync(string to, string verificationToken, string userName)
        {
            var subject = "Verify Your Aesclea Account";
            var body = $@"
                <html>
                <body>
                    <h2>Welcome to Aesclea Medical Platform!</h2>
                    <p>Hello {userName},</p>
                    <p>Thank you for registering with Aesclea. Please click the link below to verify your email address:</p>
                    <p><a href='http://localhost:5173/verify-email?token={verificationToken}' style='background-color: #007bff; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px;'>Verify Email</a></p>
                    <p>If the button doesn't work, copy and paste this link into your browser:</p>
                    <p>http://localhost:5173/verify-email?token={verificationToken}</p>
                    <p>This link will expire in 24 hours.</p>
                    <br>
                    <p>Best regards,<br>The Aesclea Team</p>
                </body>
                </html>";

            return await SendEmailAsync(to, subject, body, true);
        }

        public async Task<bool> SendPasswordResetEmailAsync(string to, string resetToken, string userName)
        {
            var subject = "Reset Your Aesclea Password";
            var body = $@"
                <html>
                <body>
                    <h2>Password Reset Request</h2>
                    <p>Hello {userName},</p>
                    <p>We received a request to reset your password. Click the link below to reset it:</p>
                    <p><a href='http://localhost:5173/reset-password?token={resetToken}' style='background-color: #dc3545; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px;'>Reset Password</a></p>
                    <p>If the button doesn't work, copy and paste this link into your browser:</p>
                    <p>http://localhost:5173/reset-password?token={resetToken}</p>
                    <p>This link will expire in 1 hour.</p>
                    <p>If you didn't request this, please ignore this email.</p>
                    <br>
                    <p>Best regards,<br>The Aesclea Team</p>
                </body>
                </html>";

            return await SendEmailAsync(to, subject, body, true);
        }

        public async Task<bool> SendWelcomeEmailAsync(string to, string userName)
        {
            var subject = "Welcome to Aesclea Medical Platform!";
            var body = $@"
                <html>
                <body>
                    <h2>Welcome to Aesclea!</h2>
                    <p>Hello {userName},</p>
                    <p>Your account has been successfully verified and you're now ready to use Aesclea Medical Platform.</p>
                    <p>You can now:</p>
                    <ul>
                        <li>Manage patient records</li>
                        <li>Perform AI-powered medical analysis</li>
                        <li>Access advanced diagnostic tools</li>
                        <li>Generate comprehensive reports</li>
                    </ul>
                    <p><a href='http://localhost:5173/dashboard' style='background-color: #28a745; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px;'>Access Dashboard</a></p>
                    <br>
                    <p>Best regards,<br>The Aesclea Team</p>
                </body>
                </html>";

            return await SendEmailAsync(to, subject, body, true);
        }
    }
}
