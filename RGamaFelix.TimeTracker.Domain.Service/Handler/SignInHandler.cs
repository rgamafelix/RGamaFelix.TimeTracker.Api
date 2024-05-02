using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RGamaFelix.ServiceResponse;
using RGamaFelix.TimeTracker.ApplicationService.Contracts;
using RGamaFelix.TimeTracker.Domain.Model;
using RGamaFelix.TimeTracker.Repository;
using RGamaFelix.TimeTracker.Rest.Model;
using RGamaFelix.TimeTracker.Rest.Model.Infraestructure;

namespace RGamaFelix.TimeTracker.Domain.Service.Handler;

[AutorizeRequest(Roles = ["Admin"])]
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
            var user = await _dbContext.Users.Include(u => u.Sessions).SingleOrDefaultAsync(
                u => u.NormalizedUserName.Equals(request.UserName, StringComparison.InvariantCultureIgnoreCase),
                cancellationToken);
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

            var currentSession = user.Sessions.SingleOrDefault(s =>
                s.IsRevoked == false && Equals(s.RequestIp, _httpContext.Connection.RemoteIpAddress));
            var (accessToken, accessTokenExpireDate) = _tokenService.CreateAccessToken(request.UserName);
            var (refreshToken, refreshTokenExpireDate) = _tokenService.CreateRefreshToken(request.UserName);
            if (currentSession != null)
            {
                user.ReplaceSession(currentSession, accessToken, accessTokenExpireDate, refreshToken,
                    refreshTokenExpireDate, _httpContext.Connection.RemoteIpAddress);
            }
            else
            {
                user.AddSession(accessToken, accessTokenExpireDate, refreshToken, refreshTokenExpireDate,
                    _httpContext.Connection.RemoteIpAddress);
            }

            _dbContext.Users.Update(user);
            return ServiceResultOf<AuthResponse>.Success(new AuthResponse(accessToken, refreshToken, request.UserName),
                ResultTypeCode.Ok);
        }
        catch (Exception e)
        {
            return ServiceResultOf<AuthResponse>.Fail(e);
        }
    }
}
