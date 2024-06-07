using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NSubstitute;
using RGamaFelix.ServiceResponse;
using RGamaFelix.TimeTracker.Domain.Model;
using RGamaFelix.TimeTracker.Domain.Service.Handler;
using RGamaFelix.TimeTracker.Request.Model;
using RGamaFelix.TimeTracker.Tests.Fixture;

namespace RGamaFelix.TimeTracker.Tests;

[Trait("Category", "Handler")]
[Trait("Category", "CreateUser")]
public class CreateRegularUserHandlerTests : IClassFixture<TestFixture>
{
    private readonly TestFixture _fixture;

    public CreateRegularUserHandlerTests(TestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task When_EmailAlreadyExists_ShouldReturn_Multiplicity()
    {
        // Arrange
        var contextMock = _fixture.ContextMock;
        var request = new CreateRegularUserRequest("ValidName", "existing@email.com", "V@l1dpwd");
        var validator = Substitute.For<IValidator<CreateRegularUserRequest>>();
        validator.ValidateAsync(request, CancellationToken.None).Returns(Task.FromResult(new ValidationResult()));
        var logger = Substitute.For<ILogger<CreateRegularUserHandler>>();
        var userManagerMock = _fixture.UserManagerMock;
        await userManagerMock.CreateAsync(User.Create("ExistingUser", request.Email), request.Password);
        var httpAccessorMock = Substitute.For<IHttpContextAccessor>();
        var handler = new CreateRegularUserHandler(logger, contextMock, userManagerMock, httpAccessorMock);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ResultTypeCode.Multiplicity, result.ResultType);
        Assert.Null(result.Data);
        Assert.Single(result.Errors, CreateRegularUserHandler.UserAlreadyExists);
    }

    [Fact]
    public async Task When_RequestIsValid_Should_CreateUser()
    {
        // Arrange
        var contextMock = _fixture.ContextMock;
        var request = new CreateRegularUserRequest("ValidName", "valid@email.com", "V@l1dpwd");
        var validator = Substitute.For<IValidator<CreateRegularUserRequest>>();
        validator.ValidateAsync(request, CancellationToken.None).Returns(Task.FromResult(new ValidationResult()));
        var logger = Substitute.For<ILogger<CreateRegularUserHandler>>();
        var userManagerMock = _fixture.UserManagerMock;
        var httpAccessorMock = Substitute.For<IHttpContextAccessor>();
        var handler = new CreateRegularUserHandler(logger, contextMock, userManagerMock, httpAccessorMock);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(ResultTypeCode.Ok, result.ResultType);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task When_UserAlreadyExists_ShouldReturn_Multiplicity()
    {
        // Arrange
        var contextMock = _fixture.ContextMock;
        var request = new CreateRegularUserRequest("ExistingUser", "valid@email.com", "V@l1dpwd");
        var validator = Substitute.For<IValidator<CreateRegularUserRequest>>();
        validator.ValidateAsync(request, CancellationToken.None).Returns(Task.FromResult(new ValidationResult()));
        contextMock.Users.Add(User.Create(request.Name, "otehr@email.com"));
        await contextMock.SaveChangesAsync();
        var logger = Substitute.For<ILogger<CreateRegularUserHandler>>();
        var userManagerMock = _fixture.UserManagerMock;
        var httpAccessorMock = Substitute.For<IHttpContextAccessor>();
        var handler = new CreateRegularUserHandler(logger, contextMock, userManagerMock, httpAccessorMock);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ResultTypeCode.Multiplicity, result.ResultType);
        Assert.Null(result.Data);
        Assert.Single(result.Errors, CreateRegularUserHandler.UserAlreadyExists);
    }
}
