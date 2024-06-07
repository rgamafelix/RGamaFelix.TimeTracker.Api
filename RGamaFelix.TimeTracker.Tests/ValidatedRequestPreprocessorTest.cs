using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.Extensions.Logging;
using NSubstitute;
using RGamaFelix.ServiceResponse;
using RGamaFelix.TimeTracker.Request.Model;
using RGamaFelix.TimeTracker.Request.Preprocessor;

namespace RGamaFelix.TimeTracker.Tests;

[Trait("Category", "Validator")]
[Trait("Category", "Preprocessor")]
public class ValidatedRequestPreprocessorTest
{
    [Fact]
    public async Task When_RequestIsInvalid_Should_ReturnFailResponse()
    {
        // Arrange
        var validator = Substitute.For<IValidator<CreateRegularUserRequest>>();
        validator.ValidateAsync(Arg.Any<CreateRegularUserRequest>(), CancellationToken.None).Returns(
            Task.FromResult(new ValidationResult(new List<ValidationFailure> { new("Email", "Invalid email") })));
        var logger = Substitute
            .For<ILogger<ValidatedRequestPreprocessor<CreateRegularUserRequest,
                IServiceResultOf<CreateUserResponse>>>>();
        var next = Substitute.For<RequestHandlerDelegate<IServiceResultOf<CreateUserResponse>>>();
        var request = new CreateRegularUserRequest("Valid Name", "invalidemail", "V@l1dpwd");
        var preprocessor =
            new ValidatedRequestPreprocessor<CreateRegularUserRequest, IServiceResultOf<CreateUserResponse>>(validator,
                logger);

        // Act
        var result = await preprocessor.Handle(request, next, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ResultTypeCode.InvalidData, result.ResultType);
        Assert.Null(result.Data);
        Assert.Single(result.Errors, "Invalid email");
    }

    [Fact]
    public async Task When_RequestIsValid_Should_ReturnNext()
    {
        // Arrange
        var validator = Substitute.For<IValidator<CreateRegularUserRequest>>();
        validator.ValidateAsync(Arg.Any<CreateRegularUserRequest>(), CancellationToken.None)
            .Returns(Task.FromResult(new ValidationResult()));
        var logger = Substitute
            .For<ILogger<ValidatedRequestPreprocessor<CreateRegularUserRequest,
                IServiceResultOf<CreateUserResponse>>>>();
        var next = Substitute.For<RequestHandlerDelegate<IServiceResultOf<CreateUserResponse>>>();
        var preprocessor =
            new ValidatedRequestPreprocessor<CreateRegularUserRequest, IServiceResultOf<CreateUserResponse>>(validator,
                logger);

        // Act
        await preprocessor.Handle(default!, next, CancellationToken.None);

        // Assert
        await next.Received(1).Invoke();
    }
}