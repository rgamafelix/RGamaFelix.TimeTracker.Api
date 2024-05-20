using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.Extensions.Logging;
using NSubstitute;
using RGamaFelix.ServiceResponse;
using RGamaFelix.TimeTracker.Request.Model;
using RGamaFelix.TimeTracker.Request.Preprocessor;

namespace RGamaFelix.TimeTracker.Tests;

[Trait("Category", "Preprocessor")]
public class ValidatedRequestPreprocessorTests
{
    [Fact]
    public async Task When_RequestIsInvalid_ShouldReturn_InvalidDataResponse()
    {
        // Arrange
        var validatorMock = Substitute.For<IValidator<CreateRegularUserRequest>>();
        var request = new CreateRegularUserRequest("", "", "");
        validatorMock.ValidateAsync(request, CancellationToken.None).Returns(
            Task.FromResult(new ValidationResult(new List<ValidationFailure> { new("Name", "Name is required") })));
        var loggerMock = Substitute
            .For<ILogger<ValidatedRequestPreprocessor<CreateRegularUserRequest,
                ServiceResultOf<CreateUserResponse>>>>();
        var preprocessor =
            new ValidatedRequestPreprocessor<CreateRegularUserRequest, ServiceResultOf<CreateUserResponse>>(
                validatorMock, loggerMock);
        var next = Substitute.For<RequestHandlerDelegate<ServiceResultOf<CreateUserResponse>>>();

        // Act
        var result = await preprocessor.Handle(request, next, default);

        // Assert
        await next.DidNotReceive().Invoke();
        Assert.False(result.IsSuccess);
        Assert.Equal(ResultTypeCode.InvalidData, result.ResultType);
        Assert.Single(result.Errors);
    }

    [Fact]
    public async Task When_RequestIsValid_ShouldReturn_SuccessResponse()
    {
        // Arrange
        var validatorMock = Substitute.For<IValidator<CreateRegularUserRequest>>();
        var request = new CreateRegularUserRequest("", "", "");
        validatorMock.ValidateAsync(request, CancellationToken.None).Returns(Task.FromResult(new ValidationResult()));
        var loggerMock = Substitute
            .For<ILogger<ValidatedRequestPreprocessor<CreateRegularUserRequest,
                ServiceResultOf<CreateUserResponse>>>>();
        var preprocessor =
            new ValidatedRequestPreprocessor<CreateRegularUserRequest, ServiceResultOf<CreateUserResponse>>(
                validatorMock, loggerMock);
        var next = Substitute.For<RequestHandlerDelegate<ServiceResultOf<CreateUserResponse>>>();

        // Act
        var result = await preprocessor.Handle(request, next, default);

        // Assert
        await next.Received(1).Invoke();
    }
}