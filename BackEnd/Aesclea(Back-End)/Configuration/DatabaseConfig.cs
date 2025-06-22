namespace Aesclea_Back_End_.Configuration
{
    public class DatabaseConfig
    {
        public string ConnectionString { get; set; } = string.Empty;
    }
    
    public class JwtConfig
    {
        public string Secret { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public int ExpiryInMinutes { get; set; } = 60;
    }
}