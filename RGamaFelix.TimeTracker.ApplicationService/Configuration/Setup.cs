using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RGamaFelix.TimeTracker.ApplicationService.Contracts;

namespace RGamaFelix.TimeTracker.ApplicationService.Configuration;

public static class Setup
{
    public static IServiceCollection AddApplicationService(this IServiceCollection services,
        IConfigurationSection jwtConfigurationSection)
    {
        services.Configure<JwtConfiguration>(jwtConfigurationSection);
        services.AddScoped<ITokenService, JwtTokenService>();
        services.AddScoped<IAuthenticationResolver, AuthenticationResolver>();
        return services;
    }
}