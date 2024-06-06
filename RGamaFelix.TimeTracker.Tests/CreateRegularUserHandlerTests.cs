using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MockQueryable.NSubstitute;
using NSubstitute;
using RGamaFelix.ServiceResponse;
using RGamaFelix.TimeTracker.DataContext;
using RGamaFelix.TimeTracker.Domain.Model;
using RGamaFelix.TimeTracker.Domain.Service.Handler;
using RGamaFelix.TimeTracker.Request.Model;
using RGamaFelix.TimeTracker.Tests.Fixture;

namespace RGamaFelix.TimeTracker.Tests;

[Trait("Category", "Handler")]
[Trait("Category", "CreateUser")]
public class CreateRegularUserHandlerTests : IClassFixture<DataContextFixture>
{
    private readonly TimeTrackerDbContext _contextMock;

    public CreateRegularUserHandlerTests(DataContextFixture fixture)
    {
        _contextMock = fixture.Context;
    }

    [Fact]
    public async Task When_EmailAlreadyExists_ShouldReturn_Multiplicity()
    {
        // Arrange
        var request = new CreateRegularUserRequest("Valid Name", "existing@email.com", "V@l1dpwd");
        var validator = Substitute.For<IValidator<CreateRegularUserRequest>>();
        validator.ValidateAsync(request, CancellationToken.None).Returns(Task.FromResult(new ValidationResult()));
        var logger = Substitute.For<ILogger<CreateRegularUserHandler>>();
        var userManagerMock = TestHelper.MockUserManager();
        var userList = new List<User> { User.Create("ExistingUser", request.Email) };
        var usersMock = userList.AsQueryable().BuildMockDbSet();
        userManagerMock.Users.Returns(usersMock);
        var httpAccessorMock = Substitute.For<IHttpContextAccessor>();
        var handler = new CreateRegularUserHandler(logger, _contextMock, userManagerMock, httpAccessorMock);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ResultTypeCode.Multiplicity, result.ResultType);
        Assert.Null(result.Data);
        Assert.Single(result.Errors, CreateRegularUserHandler.UserAlreadyExists);
    }

    [Fact]
    public async Task When_RequestIsInvalid_ShouldReturn_InvalidData()
    {
        // Arrange
        var request = new CreateRegularUserRequest("Valid Name", "invalidemail", "V@l1dPwd");
        var validator = Substitute.For<IValidator<CreateRegularUserRequest>>();
        validator.ValidateAsync(request, CancellationToken.None).Returns(
            Task.FromResult(new ValidationResult(new List<ValidationFailure> { new("Email", "Invalid email") })));
        var logger = Substitute.For<ILogger<CreateRegularUserHandler>>();
        var userManagerMock = TestHelper.MockUserManager();
        var httpAccessorMock = Substitute.For<IHttpContextAccessor>();
        var handler = new CreateRegularUserHandler(logger, _contextMock, userManagerMock, httpAccessorMock);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ResultTypeCode.InvalidData, result.ResultType);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task When_RequestIsValid_Should_CreateUser()
    {
        // Arrange
        var request = new CreateRegularUserRequest("Valid Name", "valid@email.com", "V@l1dpwd");
        var validator = Substitute.For<IValidator<CreateRegularUserRequest>>();
        validator.ValidateAsync(request, CancellationToken.None).Returns(Task.FromResult(new ValidationResult()));
        var logger = Substitute.For<ILogger<CreateRegularUserHandler>>();
        var userManagerMock = TestHelper.MockUserManager();
        var httpAccessorMock = Substitute.For<IHttpContextAccessor>();
        var handler = new CreateRegularUserHandler(logger, _contextMock, userManagerMock, httpAccessorMock);

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
        var request = new CreateRegularUserRequest("ExistingUser", "valid@email.com", "V@l1dpwd");
        var validator = Substitute.For<IValidator<CreateRegularUserRequest>>();
        validator.ValidateAsync(request, CancellationToken.None).Returns(Task.FromResult(new ValidationResult()));
        _contextMock.Users.Add(User.Create(request.Name, "otehr@email.com"));
        await _contextMock.SaveChangesAsync();
        var logger = Substitute.For<ILogger<CreateRegularUserHandler>>();
        var userManagerMock = TestHelper.MockUserManager();
        var httpAccessorMock = Substitute.For<IHttpContextAccessor>();
        var handler = new CreateRegularUserHandler(logger, _contextMock, userManagerMock, httpAccessorMock);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ResultTypeCode.Multiplicity, result.ResultType);
        Assert.Null(result.Data);
        Assert.Single(result.Errors, CreateRegularUserHandler.UserAlreadyExists);
    }
}