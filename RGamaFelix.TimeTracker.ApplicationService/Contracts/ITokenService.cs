namespace RGamaFelix.TimeTracker.ApplicationService.Contracts;

public interface ITokenService
{
    (string tokenString, DateTime expirationDate) CreateAccessToken(string userName);
    (string, DateTime expires) CreateRefreshToken(string userName);
}
