using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using RGamaFelix.TimeTracker.Domain.Model;
using RGamaFelix.TimeTracker.Repository;
using RGamaFelix.TimeTracker.Rest.Api.Middleware;

namespace RGamaFelix.TimeTracker.Rest.Api.Configuration;

public static class Setup
{
    public static IServiceCollection AddIdentityServices(this IServiceCollection services,
        IConfigurationSection section)
    {
        var key = Encoding.ASCII.GetBytes(section["SecretKey"]);
        services.AddAuthentication(x =>
        {
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(x =>
        {
            x.TokenValidationParameters = new TokenValidationParameters
            {
                IssuerSigningKey = new SymmetricSecurityKey(key), ValidateIssuer = true, ValidateAudience = true
            };
        });
        services.AddIdentityCore<User>().AddRoles<IdentityRole<Guid>>().AddEntityFrameworkStores<TimeTrackerDbContext>()
            .AddDefaultTokenProviders();
        services.Configure<IdentityOptions>(options =>
        {
            options.SignIn.RequireConfirmedEmail = false;
            options.User.RequireUniqueEmail = true;
            options.Password.RequireDigit = true;
            options.Password.RequiredLength = 8;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireLowercase = true;
        });
        return services;
    }

    public static IApplicationBuilder AddMiddlewares(this IApplicationBuilder app)
    {
        return app.UseMiddleware<TokenValidationMiddleware>();
    }

    public static IMvcBuilder AddTimeTrackerControllers(this IMvcBuilder mvcBuilder)
    {
        mvcBuilder.AddApplicationPart(typeof(Setup).Assembly);
        return mvcBuilder;
    }
}