using System.ComponentModel.DataAnnotations;

namespace Aesclea_Back_End_.Models
{
    public class ResendVerificationRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}
