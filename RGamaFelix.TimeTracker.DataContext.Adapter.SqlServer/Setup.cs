using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace RGamaFelix.TimeTracker.DataContext.Adapter.SqlServer;

public static class Setup
{
    public static IServiceCollection UseSqlServer(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<TimeTrackerDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("TimeTrackerSqlServerDbConnectionString"),
                    opt => { opt.MigrationsAssembly("RGamaFelix.TimeTracker.DataContext.Adapter.SqlServer"); })
#if DEBUG
                .EnableSensitiveDataLogging().EnableDetailedErrors()
#endif
                ;
        });
        return services;
    }
}