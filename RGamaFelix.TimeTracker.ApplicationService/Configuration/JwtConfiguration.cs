namespace RGamaFelix.TimeTracker.ApplicationService.Configuration;

public class JwtConfiguration
{
    public int AccessTokenExpirationMinutes { get; set; }
    public string Audience { get; set; }
    public string Issuer { get; set; }
    public int RefreshTokenExpirationDays { get; set; }
    public string SecretKey { get; set; }
}
