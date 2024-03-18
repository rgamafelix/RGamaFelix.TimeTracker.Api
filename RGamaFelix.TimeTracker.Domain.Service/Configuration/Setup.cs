using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace RGamaFelix.TimeTracker.Domain.Service.Configuration;

public static class Setup
{
    public static IServiceCollection AddDomainService(this IServiceCollection services)
    {
        services.AddMediatR(configuration => configuration.RegisterServicesFromAssembly(typeof(Setup).Assembly));
        services.AddHttpContextAccessor();
        services.AddValidatorsFromAssembly(typeof(Setup).Assembly);
        

        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.SuppressModelStateInvalidFilter = true;
        });


        return services;

    }
}
