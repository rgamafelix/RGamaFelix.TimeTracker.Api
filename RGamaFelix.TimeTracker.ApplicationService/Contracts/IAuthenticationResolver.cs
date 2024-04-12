using System.Security.Claims;

namespace RGamaFelix.TimeTracker.Domain.Service.Handler;

public interface IAuthenticationResolver
{
    bool Resolve(bool requireAuthentication, IEnumerable<string>? permittedRoles = null,
        IEnumerable<Claim>? requiredClaims = null);
}
