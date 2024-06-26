﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RGamaFelix.TimeTracker.DataContext;

namespace RGamaFelix.TimeTracker.Repository.Adapter.PostgresSql;

public static class Setup
{
    public static IServiceCollection UsePostgresSql(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<TimeTrackerDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("TimeTrackerPostgresDbConnectionString"),
                    options =>
                    {
                        options.MigrationsAssembly("RGamaFelix.TimeTracker.DataContext.Adapter.PostgresSql");
                    })
#if DEBUG
                .EnableSensitiveDataLogging().EnableDetailedErrors()
#endif
                ;
        });
        return services;
    }
}