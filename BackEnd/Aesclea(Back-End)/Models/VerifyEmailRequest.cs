using System.ComponentModel.DataAnnotations;

namespace Aesclea_Back_End_.Models
{
    public class VerifyEmailRequest
    {
        [Required]
        public string Token { get; set; } = string.Empty;
    }
}
