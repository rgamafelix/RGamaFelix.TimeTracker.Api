using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RGamaFelix.ServiceResponse;
using RGamaFelix.TimeTracker.Application.Infrastructure;
using RGamaFelix.TimeTracker.Application.Service.Contracts;
using RGamaFelix.TimeTracker.Domain.Model;
using RGamaFelix.TimeTracker.Repository;
using RGamaFelix.TimeTracker.Request.Model;

namespace RGamaFelix.TimeTracker.Domain.Service.Handler;

[AuthorizeRequest(Roles = ["Admin"])]
public class SignInHandler : IRequestHandler<SignInRequest, IServiceResultOf<AuthResponse>>
{
    private readonly TimeTrackerDbContext _dbContext;
    private readonly HttpContext _httpContext;
    private readonly ILogger<SignInHandler> _logger;
    private readonly ITokenService _tokenService;
    private readonly UserManager<User> _userManager;

    public SignInHandler(ILogger<SignInHandler> logger, IHttpContextAccessor httpContextAccessor,
        UserManager<User> userManager, ITokenService tokenService, TimeTrackerDbContext dbContext)
    {
        _logger = logger;
        _userManager = userManager;
        _tokenService = tokenService;
        _dbContext = dbContext;
        _httpContext = httpContextAccessor.HttpContext;
    }

    public async Task<IServiceResultOf<AuthResponse>> Handle(SignInRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var userNameForQuery = request.UserName.ToUpperInvariant();
            var user = await _dbContext.Users.Include(u => u.Sessions).SingleOrDefaultAsync(
                u => u.NormalizedUserName.Equals(userNameForQuery), cancellationToken);
            if (user == null)
            {
                _logger.LogWarning("User {User} not found", request.UserName);
                return ServiceResultOf<AuthResponse>.Fail("AuthenticationError", ResultTypeCode.AuthenticationError);
            }

            if (!await _userManager.CheckPasswordAsync(user, request.Password))
            {
                _logger.LogWarning("Invalid password for user {User}", request.UserName);
                return ServiceResultOf<AuthResponse>.Fail("AuthenticationError", ResultTypeCode.AuthenticationError);
            }

            Session? newSession = null;
            var currentSession = user.Sessions.SingleOrDefault(s =>
                s.IsRevoked == false && Equals(s.RequestIp, _httpContext.Connection.RemoteIpAddress));
            var (accessToken, accessTokenExpireDate) = _tokenService.CreateAccessToken(request.UserName);
            var (refreshToken, refreshTokenExpireDate) = _tokenService.CreateRefreshToken(request.UserName);
            if (currentSession != null)
            {
                newSession = user.ReplaceSession(currentSession, accessToken, accessTokenExpireDate, refreshToken,
                    refreshTokenExpireDate, _httpContext.Connection.RemoteIpAddress);
            }
            else
            {
                newSession = user.AddSession(accessToken, accessTokenExpireDate, refreshToken, refreshTokenExpireDate,
                    _httpContext.Connection.RemoteIpAddress);
            }

            _dbContext.Add(newSession);
            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return ServiceResultOf<AuthResponse>.Success(new AuthResponse(accessToken, refreshToken, request.UserName),
                ResultTypeCode.Ok);
        }
        catch (Exception e)
        {
            return ServiceResultOf<AuthResponse>.Fail(e);
        }
    }
}
