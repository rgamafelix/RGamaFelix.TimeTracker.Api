﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace RGamaFelix.TimeTracker.Repository.Adapter.InMemory;

public static class Setup
{
    public static IServiceCollection UseInMemoryDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<TimeTrackerDbContext>(options =>
        {
            options.UseInMemoryDatabase("TimeTrackerInMemoryDb");
        });
        return services;
    }
}