using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace RGamaFelix.TimeTracker.Domain.Service.Handler;

public interface IAuthenticationResolver
{
    bool Resolve(bool requireAuthentication,
        IEnumerable<string>? permittedRoles = null, IEnumerable<Claim>? requiredClaims = null);
}
