using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace RGamaFelix.TimeTracker.Domain.Service.Handler;

public class AuthenticationResolver : IAuthenticationResolver
{
    private readonly HttpContext _httpContext;

    public AuthenticationResolver(IHttpContextAccessor httpContextAccessor)
    {
        _httpContext = httpContextAccessor.HttpContext;
    }

    public bool Resolve(bool requireAuthentication, IEnumerable<string>? permittedRoles = null,
        IEnumerable<Claim>? requiredClaims = null)
    {
        if (!requireAuthentication)
        {
            return true;
        }

        var user = _httpContext.User;
        if (user?.Identity is null)
        {
            return false;
        }

        if (!user.Identity.IsAuthenticated)
        {
            return false;
        }

        var roleList = permittedRoles as string[] ?? permittedRoles?.ToArray();
        if (roleList is null || roleList.Length == 0)
        {
            return true;
        }

        if (!roleList.Any(user.IsInRole))
        {
            return false;
        }

        return requiredClaims != null && requiredClaims.All(claim => user.HasClaim(claim.Type, claim.Value));
    }
}
