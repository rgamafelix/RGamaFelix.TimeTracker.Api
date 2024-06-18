using FluentValidation.TestHelper;
using RGamaFelix.TimeTracker.Domain.Service.Validator;
using RGamaFelix.TimeTracker.Request.Model;

namespace RGamaFelix.TimeTracker.Tests;

[Trait("Category", "Validator")]
[Trait("Category", "CreateUser")]
public class CreateRegularUserValidatorTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task When_EmailIsEmpty_ShouldReturn_EmptyEmailError(string? email)
    {
        // Arrange
        var validator = new CreateRegularUserRequestValidator();
        var request = new CreateRegularUserRequest("Valid Name", email!, "V@l1dPwd");

        // Act
        var result = await validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage(CreateRegularUserRequestValidator.EmptyEmail);
    }

    [Theory]
    [InlineData("invalidemail")]
    [InlineData("invalidemail@")]
    [InlineData("invalidemail@domain")]
    [InlineData("@domain")]
    [InlineData("@domain.com")]
    public async Task When_EmailIsInvalid_ShouldReturn_InvalidEmailError(string email)
    {
        // Arrange
        var validator = new CreateRegularUserRequestValidator();
        var request = new CreateRegularUserRequest("Valid Name", email, "V@l1dPwd");

        // Act
        var result = await validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage(CreateRegularUserRequestValidator.InvalidEmail);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task When_NameIsEmpty_ShouldReturn_EmptyNameError(string? name)
    {
        // Arrange
        var validator = new CreateRegularUserRequestValidator();
        var request = new CreateRegularUserRequest(name!, "valid@email.com", "V@l2dPwd");

        // Act
        var result = await validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name).WithErrorMessage(CreateRegularUserRequestValidator.EmptyName);
    }

    [Fact]
    public async Task When_PasswordDoesNotContainLowerCaseLetter_ShouldReturn_PasswordMustContainLowerCaseLetterError()
    {
        // Arrange
        var validator = new CreateRegularUserRequestValidator();
        var request = new CreateRegularUserRequest("Valid Name", "valid@email.com", "P@SSW0RD");

        // Act
        var result = await validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage(CreateRegularUserRequestValidator.PasswordMustContainLowerCaseLetter);
    }

    [Fact]
    public async Task When_PasswordDoesNotContainNumber_ShouldReturn_PasswordMustContainNumberError()
    {
        // Arrange
        var validator = new CreateRegularUserRequestValidator();
        var request = new CreateRegularUserRequest("Valid Name", "valid@email.com", "P@ssword");

        // Act
        var result = await validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage(CreateRegularUserRequestValidator.PasswordMustContainNumber);
    }

    [Fact]
    public async Task
        When_PasswordDoesNotContainSpecialCharacter_ShouldReturn_PasswordMustContainSpecialCharacterError()
    {
        // Arrange
        var validator = new CreateRegularUserRequestValidator();
        var request = new CreateRegularUserRequest("Valid Name", "valid@email.com", "Passw0rd");

        // Act
        var result = await validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage(CreateRegularUserRequestValidator.PasswordMustContainSpecialCharacter);
    }

    [Fact]
    public async Task When_PasswordDoesNotContainUpperCaseLetter_ShouldReturn_PasswordMustContainUpperCaseLetterError()
    {
        // Arrange
        var validator = new CreateRegularUserRequestValidator();
        var request = new CreateRegularUserRequest("Valid Name", "valid@email.com", "p@ssw0rd");

        // Act
        var result = await validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage(CreateRegularUserRequestValidator.PasswordMustContainUpperCaseLetter);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task When_PasswordIsEmpty_ShouldReturn_EmptyPasswordError(string? password)
    {
        // Arrange
        var validator = new CreateRegularUserRequestValidator();
        var request = new CreateRegularUserRequest("Valid Name", "valid@email.com", password!);

        // Act
        var result = await validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage(CreateRegularUserRequestValidator.EmptyPassword);
    }

    [Fact]
    public async Task When_PasswordIsTooShort_ShouldReturn_PasswordTooShortError()
    {
        // Arrange
        var validator = new CreateRegularUserRequestValidator();
        var request = new CreateRegularUserRequest("Valid Name", "valid@email.com", "inV@l1d");

        // Act
        var result = await validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage(CreateRegularUserRequestValidator.PasswordTooShort);
    }

    [Fact]
    public async Task When_RequestIsValid_ShouldNotReturnErrors()
    {
        // Arrange
        var validator = new CreateRegularUserRequestValidator();
        var request = new CreateRegularUserRequest("Valid Name", "valid@email.com", "V@l1dPwd");

        // Act
        var result = await validator.TestValidateAsync(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
