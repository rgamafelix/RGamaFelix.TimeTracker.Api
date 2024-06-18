using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RGamaFelix.TimeTracker.DataContext;

namespace RGamaFelix.TimeTracker.DataContext.Adapter.InMemory;

public static class Setup
{
    public static IServiceCollection UseInMemoryDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<TimeTrackerDbContext>(options =>
        {
            options.UseInMemoryDatabase("TimeTrackerInMemoryDb", config => { config.EnableNullChecks(); })
                .EnableSensitiveDataLogging().EnableDetailedErrors();
        });
        return services;
    }
}