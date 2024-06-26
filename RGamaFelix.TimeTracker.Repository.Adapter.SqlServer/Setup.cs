﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RGamaFelix.TimeTracker.DataContext;

namespace RGamaFelix.TimeTracker.Repository.Adapter.SqlServer;

public static class Setup
{
    public static IServiceCollection UseSqlServer(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<TimeTrackerDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("TimeTrackerSqlServerDbConnectionString"),
                    options => { options.MigrationsAssembly("RGamaFelix.TimeTracker.DataContext.Adapter.SqlServer"); })
#if DEBUG
                .EnableSensitiveDataLogging().EnableDetailedErrors()
#endif
                ;
        });
        return services;
    }
}