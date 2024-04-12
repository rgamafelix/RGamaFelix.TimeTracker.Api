using Microsoft.Extensions.Configuration;

public class JwtConfiguration// : ConfigurationSection
{
    public string Audience { get; set; }
    public int AccessTokenExpirationMinutes { get; set; }
    public string Issuer { get; set; }
    public int RefreshTokenExpirationDays { get; set; }
    public string SecretKey { get; set; }

    // public JwtConfiguration(IConfigurationRoot root, string path) : base(root, path)
    // { }
}
