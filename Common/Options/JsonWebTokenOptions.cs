namespace Common.Options
{
    public class JsonWebTokenOptions
    {
        public string Issuer { get; set; }
        public bool ValidateIssuer { get; set; }
        public string Audience { get; set; }
        public bool ValidateAudience { get; set; }
        public string IssuerSigningKey { get; set; }
        public bool ValidateIssuerSigningKey { get; set; }
        public bool ValidateLifetime { get; set; }
        public int ExpirySeconds { get; set; }
    }
}