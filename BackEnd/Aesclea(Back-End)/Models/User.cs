using System.ComponentModel.DataAnnotations;
using Postgrest.Models;
using Postgrest.Attributes;

namespace Aesclea_Back_End_.Models 
{
    public class RegisterRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;

        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [Phone]
        public string Phone { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = string.Empty;

        [Required]
        public string Hospital { get; set; } = string.Empty;
    }

    public class LoginRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        public string Password { get; set; } = string.Empty;
    }

    public class AuthResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public User? User { get; set; }
    }

    [Table("users")]  // Add this line to specify the table name
    public class User : BaseModel
    {
        [PrimaryKey("id")]
        public string Id { get; set; } = string.Empty;
        
        [Column("email")]
        public string Email { get; set; } = string.Empty;
        
        [Column("first_name")]
        public string FirstName { get; set; } = string.Empty;
        
        [Column("last_name")]
        public string LastName { get; set; } = string.Empty;
        
        [Column("phone")]
        public string Phone { get; set; } = string.Empty;
        
        [Column("role")]
        public string Role { get; set; } = string.Empty;
        
        [Column("hospital")]
        public string Hospital { get; set; } = string.Empty;
        
        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
        
        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }
}