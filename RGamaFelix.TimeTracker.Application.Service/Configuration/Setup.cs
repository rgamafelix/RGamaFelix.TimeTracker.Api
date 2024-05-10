using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RGamaFelix.TimeTracker.Application.Service.Contracts;

namespace RGamaFelix.TimeTracker.Application.Service.Configuration;

public static class Setup
{
    public static IServiceCollection AddApplicationService(this IServiceCollection services,
        IConfigurationSection jwtConfigurationSection)
    {
        services.Configure<JwtConfiguration>(jwtConfigurationSection);
        services.AddScoped<ITokenService, JwtTokenService>();
        return services;
    }
}