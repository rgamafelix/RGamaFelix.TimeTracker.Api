using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RGamaFelix.ServiceResponse;
using RGamaFelix.TimeTracker.ApplicationService.Contracts;
using RGamaFelix.TimeTracker.Domain.Model;
using RGamaFelix.TimeTracker.Repository;
using RGamaFelix.TimeTracker.Rest.Model;

namespace RGamaFelix.TimeTracker.Domain.Service.Handler;

public class RefreshTokenHandler : ValidatedRequestHandler<RefreshTokenRequest, AuthResponse>
{
    private readonly TimeTrackerDbContext _dbContext;
    private readonly ITokenService _tokenService;
    private readonly HttpContext _httpContext;
    public RefreshTokenHandler(IValidator<RefreshTokenRequest> validator, ILogger<RefreshTokenHandler> logger,
        TimeTrackerDbContext dbContext, ITokenService tokenService, IHttpContextAccessor httpContext) : base(validator,
        logger)
    {
        _dbContext = dbContext;
        _tokenService = tokenService;
        _httpContext = httpContext.HttpContext;
    }
    protected override async Task<IServiceResultOf<AuthResponse>> HandleValidatedRequest(RefreshTokenRequest request,
        CancellationToken cancellationToken)
    {
        var user = await _dbContext.Users.Include(u => u.Sessions)
            .SingleOrDefaultAsync(
                u => u.NormalizedUserName.Equals(request.UserName, StringComparison.InvariantCultureIgnoreCase),
                cancellationToken);

        if (user is null)
        {
            Logger.LogWarning("User {User} not found", request.UserName);
            return ServiceResultOf<AuthResponse>.Fail("AuthenticationError", ResultTypeCode.AuthenticationError);
        }

        var session = user?.Sessions.SingleOrDefault(s => s.RefreshToken == request.RefreshToken);
        if (session is null)
        {
            Logger.LogWarning("Session not found for user {User}", request.UserName);
            return ServiceResultOf<AuthResponse>.Fail("AuthenticationError", ResultTypeCode.AuthenticationError);
        }

        if (session.IsRevoked)
        {
            Logger.LogWarning("Session revoked for user {User}", request.UserName);
            return ServiceResultOf<AuthResponse>.Fail("AuthenticationError", ResultTypeCode.AuthenticationError);
        }

        if (session.RefreshTokenExpiresAt < DateTime.UtcNow)
        {
            Logger.LogWarning("Refresh token expired for user {User}", request.UserName);
            session.Revoke(SessionRevocationReason.TokenExpired, null);
            return ServiceResultOf<AuthResponse>.Fail("AuthenticationError", ResultTypeCode.AuthenticationError);
        }

        var (accessToken, accessTokenExpireDate) = _tokenService.CreateAccessToken(request.UserName);
        var (refreshToken, refreshTokenExpireDate) = _tokenService.CreateRefreshToken(request.UserName);
        user.ReplaceSession(session, accessToken, accessTokenExpireDate, refreshToken, refreshTokenExpireDate,
            _httpContext.Connection.RemoteIpAddress);
        _dbContext.Update(user);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return ServiceResultOf<AuthResponse>.Success(new AuthResponse(accessToken, refreshToken, user.UserName),
            ResultTypeCode.Ok);
    }
}
