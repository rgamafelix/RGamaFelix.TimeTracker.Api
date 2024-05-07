namespace RGamaFelix.TimeTracker.ApplicationService.Contracts;

public interface ITokenService
{
    (string accessTokenString, DateTime expirationDate) CreateAccessToken(string userName);
    (string refreshTokenString, DateTime expires) CreateRefreshToken(string userName);
}