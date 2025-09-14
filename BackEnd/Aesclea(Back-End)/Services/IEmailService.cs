// Copyright (c) 2025 Alexander Sinapov | Simeon Petkov

// All rights reserved.
// This code is proprietary and confidential.  
// Unauthorized copying, modification, distribution, or use is strictly prohibited.

using System.Threading.Tasks;

namespace Aesclea_Back_End_.Services
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(string to, string subject, string body, bool isHtml = false);
        Task<bool> SendVerificationEmailAsync(string to, string verificationToken, string userName);
        Task<bool> SendPasswordResetEmailAsync(string to, string resetToken, string userName);
        Task<bool> SendWelcomeEmailAsync(string to, string userName);
    }
}
