using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using RGamaFelix.TimeTracker.Infrastructure;

namespace RGamaFelix.TimeTracker.Domain.Service.Configuration;

public static class Setup
{
    public static IServiceCollection AddDomainService(this IServiceCollection services)
    {
        services.AddMediatR(configuration =>
        {
            configuration.RegisterServicesFromAssembly(typeof(Setup).Assembly);
            configuration.AddBehavior(typeof(IPipelineBehavior<,>), typeof(AuthorizedRequestPreprocessor<,>));
            configuration.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidatedRequestPreprocessor<,>));
        });
        // services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidatedRequestPreprocessor<,>));
        // services.AddTransient(typeof(IPipelineBehavior<,>), typeof(AuthorizedRequestPreprocessor<,>));
        services.AddHttpContextAccessor();
        services.AddValidatorsFromAssembly(typeof(Setup).Assembly);
        services.Configure<ApiBehaviorOptions>(options => { options.SuppressModelStateInvalidFilter = true; });
        return services;
    }
}
