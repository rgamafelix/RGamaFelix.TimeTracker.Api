using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RGamaFelix.TimeTracker.ApplicationService.Contracts;
using RGamaFelix.TimeTracker.Domain.Service.Handler;
using RGamaFelix.TimeTracker.Domain.Service.Handler.Middleware;

namespace RGamaFelix.TimeTracker.ApplicationService.Configuration;

public static class Setup
{
    public static IServiceCollection AddApplicationService(this IServiceCollection services, IConfigurationSection jwtConfigurationSection)
    {
        services.Configure<JwtConfiguration>(jwtConfigurationSection);
        services.AddScoped<ITokenService, JwtTokenService>();
        services.AddScoped<IAuthenticationResolver, AuthenticationResolver>();

        return services;
    }
    
    
}
