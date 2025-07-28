using Aesclea_Back_End_.Models;
using Aesclea_Back_End_.Services;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Options;

namespace Aesclea_Back_End_.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailConfiguration _emailConfig;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IOptions<EmailConfiguration> emailConfig, ILogger<EmailService> logger)
        {
            _emailConfig = emailConfig.Value;
            _logger = logger;
        }

        public async Task<bool> SendVerificationEmailAsync(User user, string verificationToken)
        {
            var verificationUrl = $"{_emailConfig.BaseUrl}/verify-email?token={verificationToken}";
            
            var subject = "Verify Your Email Address - Aesclea Medical System";
            
            var htmlContent = $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <meta charset=""utf-8"">
                    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                    <title>Email Verification</title>
                    <style>
                        body {{ font-family: Arial, sans-serif; margin: 0; padding: 0; background-color: #f4f4f4; }}
                        .container {{ max-width: 600px; margin: 0 auto; background-color: #ffffff; padding: 20px; border-radius: 8px; box-shadow: 0 2px 10px rgba(0,0,0,0.1); }}
                        .header {{ text-align: center; margin-bottom: 30px; }}
                        .logo {{ width: 60px; height: 60px; background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); border-radius: 50%; margin: 0 auto 20px; display: flex; align-items: center; justify-content: center; }}
                        .logo svg {{ width: 30px; height: 30px; fill: white; }}
                        h1 {{ color: #333; margin: 0; font-size: 24px; }}
                        .content {{ margin: 30px 0; line-height: 1.6; color: #555; }}
                        .verification-button {{ text-align: center; margin: 30px 0; }}
                        .btn {{ display: inline-block; padding: 15px 30px; background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; text-decoration: none; border-radius: 6px; font-weight: bold; }}
                        .btn:hover {{ background: linear-gradient(135deg, #5a6fd8 0%, #6a4190 100%); }}
                        .footer {{ text-align: center; margin-top: 30px; padding-top: 20px; border-top: 1px solid #eee; color: #888; font-size: 12px; }}
                        .warning {{ background-color: #fff3cd; border: 1px solid #ffeaa7; color: #856404; padding: 15px; border-radius: 4px; margin: 20px 0; }}
                    </style>
                </head>
                <body>
                    <div class=""container"">
                        <div class=""header"">
                            <div class=""logo"">
                                <svg viewBox=""0 0 24 24"">
                                    <path d=""M4.318 6.318a4.5 4.5 0 000 6.364L12 20.364l7.682-7.682a4.5 4.5 0 00-6.364-6.364L12 7.636l-1.318-1.318a4.5 4.5 0 00-6.364 0z""/>
                                </svg>
                            </div>
                            <h1>Welcome to Aesclea!</h1>
                        </div>
                        
                        <div class=""content"">
                            <p>Hi {user.FirstName},</p>
                            
                            <p>Thank you for registering with Aesclea Medical Management System. To complete your registration and secure your account, please verify your email address by clicking the button below:</p>
                            
                            <div class=""verification-button"">
                                <a href=""{verificationUrl}"" class=""btn"">Verify Email Address</a>
                            </div>
                            
                            <p>If the button doesn't work, you can copy and paste this link into your browser:</p>
                            <p style=""word-break: break-all; color: #667eea;"">{verificationUrl}</p>
                            
                            <div class=""warning"">
                                <strong>Important:</strong> This verification link will expire in 24 hours. If you didn't create an account with Aesclea, please ignore this email.
                            </div>
                            
                            <p>Once verified, you'll be able to access all features of our medical management system, including:</p>
                            <ul>
                                <li>Patient management and records</li>
                                <li>AI-powered medical analysis (with subscription)</li>
                                <li>Appointment scheduling</li>
                                <li>Secure data storage and reporting</li>
                            </ul>
                            
                            <p>If you have any questions or need assistance, please don't hesitate to contact our support team.</p>
                            
                            <p>Best regards,<br>The Aesclea Team</p>
                        </div>
                        
                        <div class=""footer"">
                            <p>&copy; 2025 Aesclea Medical Management System. All rights reserved.</p>
                            <p>This email was sent to {user.Email}. If you did not sign up for an Aesclea account, please ignore this email.</p>
                        </div>
                    </div>
                </body>
                </html>";

            var plainTextContent = $@"
                Welcome to Aesclea Medical Management System!

                Hi {user.FirstName},

                Thank you for registering with Aesclea. To complete your registration, please verify your email address by visiting this link:

                {verificationUrl}

                This verification link will expire in 24 hours.

                If you didn't create an account with Aesclea, please ignore this email.

                Best regards,
                The Aesclea Team

                ---
                This email was sent to {user.Email}
                © 2025 Aesclea Medical Management System. All rights reserved.
            ";

            return await SendGenericEmailAsync(user.Email, subject, htmlContent, plainTextContent);
        }

        public async Task<bool> SendPasswordResetEmailAsync(User user, string resetToken)
        {
            var resetUrl = $"{_emailConfig.BaseUrl}/reset-password?token={resetToken}";
            
            var subject = "Reset Your Password - Aesclea Medical System";
            
            var htmlContent = $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <meta charset=""utf-8"">
                    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                    <title>Password Reset</title>
                    <style>
                        body {{ font-family: Arial, sans-serif; margin: 0; padding: 0; background-color: #f4f4f4; }}
                        .container {{ max-width: 600px; margin: 0 auto; background-color: #ffffff; padding: 20px; border-radius: 8px; box-shadow: 0 2px 10px rgba(0,0,0,0.1); }}
                        .header {{ text-align: center; margin-bottom: 30px; }}
                        .logo {{ width: 60px; height: 60px; background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); border-radius: 50%; margin: 0 auto 20px; display: flex; align-items: center; justify-content: center; }}
                        h1 {{ color: #333; margin: 0; font-size: 24px; }}
                        .content {{ margin: 30px 0; line-height: 1.6; color: #555; }}
                        .reset-button {{ text-align: center; margin: 30px 0; }}
                        .btn {{ display: inline-block; padding: 15px 30px; background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; text-decoration: none; border-radius: 6px; font-weight: bold; }}
                        .footer {{ text-align: center; margin-top: 30px; padding-top: 20px; border-top: 1px solid #eee; color: #888; font-size: 12px; }}
                        .warning {{ background-color: #fff3cd; border: 1px solid #ffeaa7; color: #856404; padding: 15px; border-radius: 4px; margin: 20px 0; }}
                    </style>
                </head>
                <body>
                    <div class=""container"">
                        <div class=""header"">
                            <div class=""logo"">🔒</div>
                            <h1>Reset Your Password</h1>
                        </div>
                        
                        <div class=""content"">
                            <p>Hi {user.FirstName},</p>
                            
                            <p>We received a request to reset your password for your Aesclea account. Click the button below to reset your password:</p>
                            
                            <div class=""reset-button"">
                                <a href=""{resetUrl}"" class=""btn"">Reset Password</a>
                            </div>
                            
                            <div class=""warning"">
                                <strong>Important:</strong> This password reset link will expire in 1 hour. If you didn't request this password reset, please ignore this email.
                            </div>
                            
                            <p>For security reasons, this link can only be used once.</p>
                        </div>
                        
                        <div class=""footer"">
                            <p>&copy; 2025 Aesclea Medical Management System. All rights reserved.</p>
                        </div>
                    </div>
                </body>
                </html>";

            return await SendGenericEmailAsync(user.Email, subject, htmlContent);
        }

        public async Task<bool> SendWelcomeEmailAsync(User user)
        {
            var subject = "Welcome to Aesclea Medical Management System!";
            
            var htmlContent = $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <meta charset=""utf-8"">
                    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                    <title>Welcome to Aesclea</title>
                    <style>
                        body {{ font-family: Arial, sans-serif; margin: 0; padding: 0; background-color: #f4f4f4; }}
                        .container {{ max-width: 600px; margin: 0 auto; background-color: #ffffff; padding: 20px; border-radius: 8px; box-shadow: 0 2px 10px rgba(0,0,0,0.1); }}
                        .header {{ text-align: center; margin-bottom: 30px; }}
                        .logo {{ width: 60px; height: 60px; background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); border-radius: 50%; margin: 0 auto 20px; display: flex; align-items: center; justify-content: center; }}
                        h1 {{ color: #333; margin: 0; font-size: 24px; }}
                        .content {{ margin: 30px 0; line-height: 1.6; color: #555; }}
                        .cta-button {{ text-align: center; margin: 30px 0; }}
                        .btn {{ display: inline-block; padding: 15px 30px; background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; text-decoration: none; border-radius: 6px; font-weight: bold; }}
                        .footer {{ text-align: center; margin-top: 30px; padding-top: 20px; border-top: 1px solid #eee; color: #888; font-size: 12px; }}
                        .feature-list {{ background-color: #f8f9fa; padding: 20px; border-radius: 6px; margin: 20px 0; }}
                    </style>
                </head>
                <body>
                    <div class=""container"">
                        <div class=""header"">
                            <div class=""logo"">🎉</div>
                            <h1>Welcome to Aesclea!</h1>
                        </div>
                        
                        <div class=""content"">
                            <p>Hi {user.FirstName},</p>
                            
                            <p>Congratulations! Your email has been verified and your Aesclea account is now active. You're ready to revolutionize your medical practice management!</p>
                            
                            <div class=""feature-list"">
                                <h3>What you can do now:</h3>
                                <ul>
                                    <li>🏥 Manage patient records securely</li>
                                    <li>📅 Schedule and track appointments</li>
                                    <li>📊 Generate comprehensive reports</li>
                                    <li>🔒 Store data with enterprise-grade security</li>
                                    <li>🤖 Access AI-powered medical analysis (with subscription)</li>
                                </ul>
                            </div>
                            
                            <div class=""cta-button"">
                                <a href=""{_emailConfig.BaseUrl}/dashboard"" class=""btn"">Get Started</a>
                            </div>
                            
                            <p>If you have any questions or need assistance getting started, our support team is here to help.</p>
                            
                            <p>Best regards,<br>The Aesclea Team</p>
                        </div>
                        
                        <div class=""footer"">
                            <p>&copy; 2025 Aesclea Medical Management System. All rights reserved.</p>
                        </div>
                    </div>
                </body>
                </html>";

            return await SendGenericEmailAsync(user.Email, subject, htmlContent);
        }

        public async Task<bool> SendSubscriptionConfirmationEmailAsync(User user, string planName)
        {
            var subject = $"Subscription Confirmed - {planName} Plan";
            
            var htmlContent = $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <meta charset=""utf-8"">
                    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                    <title>Subscription Confirmed</title>
                    <style>
                        body {{ font-family: Arial, sans-serif; margin: 0; padding: 0; background-color: #f4f4f4; }}
                        .container {{ max-width: 600px; margin: 0 auto; background-color: #ffffff; padding: 20px; border-radius: 8px; box-shadow: 0 2px 10px rgba(0,0,0,0.1); }}
                        .header {{ text-align: center; margin-bottom: 30px; }}
                        .logo {{ width: 60px; height: 60px; background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); border-radius: 50%; margin: 0 auto 20px; display: flex; align-items: center; justify-content: center; }}
                        h1 {{ color: #333; margin: 0; font-size: 24px; }}
                        .content {{ margin: 30px 0; line-height: 1.6; color: #555; }}
                        .plan-box {{ background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 20px; border-radius: 8px; text-align: center; margin: 20px 0; }}
                        .footer {{ text-align: center; margin-top: 30px; padding-top: 20px; border-top: 1px solid #eee; color: #888; font-size: 12px; }}
                    </style>
                </head>
                <body>
                    <div class=""container"">
                        <div class=""header"">
                            <div class=""logo"">✅</div>
                            <h1>Subscription Confirmed!</h1>
                        </div>
                        
                        <div class=""content"">
                            <p>Hi {user.FirstName},</p>
                            
                            <p>Great news! Your subscription to the {planName} plan has been confirmed and is now active.</p>
                            
                            <div class=""plan-box"">
                                <h2>{planName} Plan</h2>
                                <p>Your enhanced features are now available!</p>
                            </div>
                            
                            <p>You now have access to all the advanced features included in your plan. Start exploring the power of AI-driven medical analysis and enhanced practice management tools.</p>
                            
                            <p>If you have any questions about your subscription or need help getting started with the new features, please don't hesitate to contact our support team.</p>
                            
                            <p>Thank you for choosing Aesclea!</p>
                            
                            <p>Best regards,<br>The Aesclea Team</p>
                        </div>
                        
                        <div class=""footer"">
                            <p>&copy; 2025 Aesclea Medical Management System. All rights reserved.</p>
                        </div>
                    </div>
                </body>
                </html>";

            return await SendGenericEmailAsync(user.Email, subject, htmlContent);
        }

        public async Task<bool> SendGenericEmailAsync(string to, string subject, string htmlContent, string? plainTextContent = null)
        {
            try
            {
                _logger.LogInformation("Starting email send process to {Email} with subject: {Subject}", to, subject);
                _logger.LogInformation("SMTP Configuration - Server: {Server}, Port: {Port}, Username: {Username}, UseSsl: {UseSsl}", 
                    _emailConfig.SmtpServer, _emailConfig.SmtpPort, _emailConfig.SmtpUsername, _emailConfig.UseSsl);

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(_emailConfig.FromName, _emailConfig.FromEmail));
                message.To.Add(new MailboxAddress("", to));
                message.Subject = subject;

                var bodyBuilder = new BodyBuilder();
                
                if (!string.IsNullOrEmpty(htmlContent))
                {
                    bodyBuilder.HtmlBody = htmlContent;
                }
                
                if (!string.IsNullOrEmpty(plainTextContent))
                {
                    bodyBuilder.TextBody = plainTextContent;
                }
                else if (!string.IsNullOrEmpty(htmlContent))
                {
                    // Generate a simple plain text version from HTML if not provided
                    bodyBuilder.TextBody = StripHtml(htmlContent);
                }

                message.Body = bodyBuilder.ToMessageBody();

                using var client = new SmtpClient();
                
                // Gmail SMTP requires StartTls on port 587
                var secureSocketOptions = _emailConfig.SmtpPort == 587 ? SecureSocketOptions.StartTls : 
                                         _emailConfig.UseSsl ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.None;
                
                _logger.LogInformation("Connecting to SMTP server: {Server}:{Port} with {Security}", 
                    _emailConfig.SmtpServer, _emailConfig.SmtpPort, secureSocketOptions);
                
                await client.ConnectAsync(_emailConfig.SmtpServer, _emailConfig.SmtpPort, secureSocketOptions);
                
                if (!string.IsNullOrEmpty(_emailConfig.SmtpUsername) && !string.IsNullOrEmpty(_emailConfig.SmtpPassword))
                {
                    _logger.LogInformation("Authenticating with username: {Username}", _emailConfig.SmtpUsername);
                    await client.AuthenticateAsync(_emailConfig.SmtpUsername, _emailConfig.SmtpPassword);
                    _logger.LogInformation("Authentication successful");
                }
                
                _logger.LogInformation("Sending email...");
                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                _logger.LogInformation("Email sent successfully to {Email}", to);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {Email} with subject: {Subject}. Error details: {Error}", to, subject, ex.Message);
                return false;
            }
        }

        private static string StripHtml(string html)
        {
            if (string.IsNullOrEmpty(html))
                return string.Empty;

            // Simple HTML tag removal for plain text fallback
            var result = System.Text.RegularExpressions.Regex.Replace(html, "<.*?>", string.Empty);
            result = System.Text.RegularExpressions.Regex.Replace(result, @"\s+", " ");
            return result.Trim();
        }
    }
}
