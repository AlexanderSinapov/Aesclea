using Aesclea_Back_End_.Models;

namespace Aesclea_Back_End_.Services
{
    public interface IEmailService
    {
        Task<bool> SendVerificationEmailAsync(User user, string verificationToken);
        Task<bool> SendPasswordResetEmailAsync(User user, string resetToken);
        Task<bool> SendWelcomeEmailAsync(User user);
        Task<bool> SendSubscriptionConfirmationEmailAsync(User user, string planName);
        Task<bool> SendGenericEmailAsync(string to, string subject, string htmlContent, string? plainTextContent = null);
    }

    public class EmailConfiguration
    {
        public string SmtpServer { get; set; } = string.Empty;
        public int SmtpPort { get; set; }
        public string SmtpUsername { get; set; } = string.Empty;
        public string SmtpPassword { get; set; } = string.Empty;
        public bool UseSsl { get; set; } = true;
        public string FromEmail { get; set; } = string.Empty;
        public string FromName { get; set; } = string.Empty;
        public string BaseUrl { get; set; } = string.Empty; // For verification links
    }
}
