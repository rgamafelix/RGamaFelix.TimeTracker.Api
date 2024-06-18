using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RGamaFelix.ServiceResponse;
using RGamaFelix.TimeTracker.DataContext;
using RGamaFelix.TimeTracker.Domain.Model;
using RGamaFelix.TimeTracker.Request.Preprocessor;
using Void = RGamaFelix.ServiceResponse.Void;

namespace RGamaFelix.TimeTracker.Tests.Fixture;

public class TestFixture : IDisposable
{
    private readonly ServiceProvider _servicesProvider;

    public TestFixture()
    {
        var services = new ServiceCollection();
        services.AddDbContext<TimeTrackerDbContext>(o => o.UseInMemoryDatabase(Guid.NewGuid().ToString()));
        services.AddDataProtection();
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
        _servicesProvider = services.BuildServiceProvider();
        ContextMock.Roles.Add(new IdentityRole<Guid> { Id = Guid.NewGuid(), Name = "Admin", NormalizedName = "ADMIN" });
        ContextMock.Roles.Add(new IdentityRole<Guid>
        {
            Id = Guid.NewGuid(), Name = "Regular", NormalizedName = "REGULAR"
        });
        ContextMock.SaveChanges();
    }

    public TimeTrackerDbContext ContextMock => _servicesProvider.GetRequiredService<TimeTrackerDbContext>();
    public UserManager<User> UserManagerMock => _servicesProvider.GetRequiredService<UserManager<User>>();

    public void Dispose()
    {
        _servicesProvider.Dispose();
    }
}

public class UnauthorizedRequest : IRequest<IServiceResultOf<Void>>
{ }

[AuthorizeRequest(Roles = new[] { "Admin" })]
public class AuthorizedRolesRequest : IRequest<IServiceResultOf<Void>>
{ }

[AuthorizeRequest(Claims = new[] { "Claim1" })]
public class AuthorizedClaimsRequest : IRequest<IServiceResultOf<Void>>
{ }