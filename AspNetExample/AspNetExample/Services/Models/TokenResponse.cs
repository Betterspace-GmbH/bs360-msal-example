namespace AspNetExample.Clients.Models
{
    public class TokenResponse
    {
        public string AccessToken { get; set; } = null!;
        public string TokenType { get; set; } = null!;
        public long NotBefore { get; set; }
        public long ExpiresIn { get; set; }
        public long ExpiresOn { get; set; }
        public string Resource { get; set; } = null!;
    }
}
