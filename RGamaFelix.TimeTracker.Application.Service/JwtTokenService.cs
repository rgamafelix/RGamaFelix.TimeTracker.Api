using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RGamaFelix.TimeTracker.Application.Service.Configuration;
using RGamaFelix.TimeTracker.Application.Service.Contracts;

namespace RGamaFelix.TimeTracker.Application.Service;

public class JwtTokenService : ITokenService
{
    private readonly JwtConfiguration _jwtConfiguration;

    public JwtTokenService(IOptions<JwtConfiguration> jwtConfiguration)
    {
        _jwtConfiguration = jwtConfiguration.Value;
    }

    public (string accessTokenString, DateTime expirationDate) CreateAccessToken(string userName)
    {
        // Create claims for the token
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userName)

            // Add more claims as needed
        };

        // Create the signing credentials using the secret key
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfiguration.SecretKey));
        var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        // Create the token descriptor
        var expires = DateTime.UtcNow.AddMinutes(_jwtConfiguration.AccessTokenExpirationMinutes);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = _jwtConfiguration.Issuer,
            Audience = _jwtConfiguration.Audience,
            Subject = new ClaimsIdentity(claims),
            Expires = expires,
            SigningCredentials = signingCredentials
        };

        // Create the token handler
        var tokenHandler = new JwtSecurityTokenHandler();

        // Generate the token
        var token = tokenHandler.CreateToken(tokenDescriptor);

        // Serialize the token to a string
        var tokenString = tokenHandler.WriteToken(token);
        return (tokenString, expires);
    }

    public (string refreshTokenString, DateTime expires) CreateRefreshToken(string userName)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_jwtConfiguration.SecretKey);
        var expires = DateTime.UtcNow.AddDays(_jwtConfiguration.RefreshTokenExpirationDays);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userName)

                // Add any additional claims as needed
            }),
            Expires = expires, // Set the expiration time for the refresh token
            SigningCredentials =
                new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return (tokenHandler.WriteToken(token), expires);
    }
}