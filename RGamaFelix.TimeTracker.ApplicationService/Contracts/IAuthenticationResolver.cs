using System.Security.Claims;

namespace RGamaFelix.TimeTracker.ApplicationService.Contracts;

public interface IAuthenticationResolver
{
    bool Resolve(bool requireAuthentication, IEnumerable<string>? permittedRoles = null,
        IEnumerable<Claim>? requiredClaims = null);
}