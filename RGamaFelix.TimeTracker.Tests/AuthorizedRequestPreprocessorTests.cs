using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using NSubstitute;
using RGamaFelix.ServiceResponse;
using RGamaFelix.TimeTracker.Request.Preprocessor;
using RGamaFelix.TimeTracker.Tests.Fixture;
using Void = RGamaFelix.ServiceResponse.Void;

namespace RGamaFelix.TimeTracker.Tests;

[Trait("Category", "Authorization")]
[Trait("Category", "Preprocessor")]
public class AuthorizedRequestPreprocessorTests : IClassFixture<TestFixture>, IDisposable
{
    private readonly TestFixture _fixture;

    public AuthorizedRequestPreprocessorTests(TestFixture fixture)
    {
        _fixture = fixture;
    }

    public void Dispose()
    {
        _fixture.Dispose();
    }

    [Fact]
    public async Task When_ClaimIsInvalid_Should_NotAuthorize()
    {
        // Arrange
        var logger = Substitute
            .For<ILogger<AuthorizedRequestPreprocessor<AuthorizedClaimsRequest, IServiceResultOf<Void>>>>();
        var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
        httpContextAccessor.HttpContext.User.Claims.Returns(new[] { new Claim("Claim2", "Value1") });
        httpContextAccessor.HttpContext.Request.Headers["Authorization"].Returns(new StringValues(["bearer token"]));
        var next = Substitute.For<RequestHandlerDelegate<IServiceResultOf<Void>>>();
        var preprocessor =
            new AuthorizedRequestPreprocessor<AuthorizedClaimsRequest, IServiceResultOf<Void>>(httpContextAccessor,
                logger);
        var request = new AuthorizedClaimsRequest();

        // Act
        await preprocessor.Handle(request, next, CancellationToken.None);

        // Assert
        await next.DidNotReceive().Invoke();
        httpContextAccessor.HttpContext.Request.Headers.Received(1);
    }

    [Fact]
    public async Task When_ClaimIsValid_Should_Authorize()
    {
        // Arrange
        var logger = Substitute
            .For<ILogger<AuthorizedRequestPreprocessor<AuthorizedClaimsRequest, IServiceResultOf<Void>>>>();
        var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
        httpContextAccessor.HttpContext.User.Claims.Returns(new[] { new Claim("Claim1", "Value1") });
        httpContextAccessor.HttpContext.Request.Headers["Authorization"].Returns(new StringValues(["bearer token"]));
        var next = Substitute.For<RequestHandlerDelegate<IServiceResultOf<Void>>>();
        var preprocessor =
            new AuthorizedRequestPreprocessor<AuthorizedClaimsRequest, IServiceResultOf<Void>>(httpContextAccessor,
                logger);
        var request = new AuthorizedClaimsRequest();

        // Act
        await preprocessor.Handle(request, next, CancellationToken.None);

        // Assert
        await next.Received(1).Invoke();
        httpContextAccessor.HttpContext.Request.Headers.Received(1);
    }

    [Fact]
    public async Task When_RequestHasNoAuthorizationHeader_Should_Authorize()
    {
        // Arrange
        var logger =
            Substitute.For<ILogger<AuthorizedRequestPreprocessor<UnauthorizedRequest, IServiceResultOf<Void>>>>();
        var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
        var next = Substitute.For<RequestHandlerDelegate<IServiceResultOf<Void>>>();
        var preprocessor =
            new AuthorizedRequestPreprocessor<UnauthorizedRequest, IServiceResultOf<Void>>(httpContextAccessor, logger);
        var request = new UnauthorizedRequest();

        // Act
        await preprocessor.Handle(request, next, CancellationToken.None);

        // Assert
        await next.Received(1).Invoke();
    }

    [Fact]
    public async Task When_RequestIsNotAuthenticated_Should_NotAuthorize()
    {
        // Arrange
        var logger = Substitute
            .For<ILogger<AuthorizedRequestPreprocessor<AuthorizedRolesRequest, IServiceResultOf<Void>>>>();
        var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
        var next = Substitute.For<RequestHandlerDelegate<IServiceResultOf<Void>>>();
        var preprocessor =
            new AuthorizedRequestPreprocessor<AuthorizedRolesRequest, IServiceResultOf<Void>>(httpContextAccessor,
                logger);
        var request = new AuthorizedRolesRequest();

        // Act
        await preprocessor.Handle(request, next, CancellationToken.None);

        // Assert
        await next.DidNotReceive().Invoke();
        httpContextAccessor.HttpContext.Request.Headers.Received(1);
    }

    [Fact]
    public async Task When_RoleIsInvalid_Should_NotAuthorize()
    {
        // Arrange
        var logger = Substitute
            .For<ILogger<AuthorizedRequestPreprocessor<AuthorizedRolesRequest, IServiceResultOf<Void>>>>();
        var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
        httpContextAccessor.HttpContext.User.Claims.Returns(new[] { new Claim("role", "User") });
        httpContextAccessor.HttpContext.Request.Headers["Authorization"].Returns(new StringValues(["bearer token"]));
        var next = Substitute.For<RequestHandlerDelegate<IServiceResultOf<Void>>>();
        var preprocessor =
            new AuthorizedRequestPreprocessor<AuthorizedRolesRequest, IServiceResultOf<Void>>(httpContextAccessor,
                logger);
        var request = new AuthorizedRolesRequest();

        // Act
        await preprocessor.Handle(request, next, CancellationToken.None);

        // Assert
        await next.DidNotReceive().Invoke();
        httpContextAccessor.HttpContext.Request.Headers.Received(1);
    }

    [Fact]
    public async Task When_RoleIsValid_Should_Authorize()
    {
        // Arrange
        var logger = Substitute
            .For<ILogger<AuthorizedRequestPreprocessor<AuthorizedRolesRequest, IServiceResultOf<Void>>>>();
        var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
        httpContextAccessor.HttpContext.User.Claims.Returns(new[] { new Claim("role", "Admin") });
        httpContextAccessor.HttpContext.Request.Headers["Authorization"].Returns(new StringValues(["bearer token"]));
        var next = Substitute.For<RequestHandlerDelegate<IServiceResultOf<Void>>>();
        var preprocessor =
            new AuthorizedRequestPreprocessor<AuthorizedRolesRequest, IServiceResultOf<Void>>(httpContextAccessor,
                logger);
        var request = new AuthorizedRolesRequest();

        // Act
        await preprocessor.Handle(request, next, CancellationToken.None);

        // Assert
        await next.Received(1).Invoke();
        httpContextAccessor.HttpContext.Request.Headers.Received(1);
    }
}
