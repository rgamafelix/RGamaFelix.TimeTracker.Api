using FluentValidation.TestHelper;
using RGamaFelix.TimeTracker.Domain.Service.Validator;
using RGamaFelix.TimeTracker.Rest.Model;

namespace RGamaFelix.TimeTracker.Tests;

[Trait("Category", "Validator")]
[Trait("Category", "CreateUser")]
public class CreateUserValidatorTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task When_NameIsEmpty_ShouldReturn_EmptyNameError(string? name)
    {
        // Arrange
        var validator = new CreateUserRequestValidator();
        var request = new CreateRegularUserRequest(name!, "valid@email.com", "V@l1dPwd");

        // Act
        var result = await validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name).WithErrorMessage(CreateUserRequestValidator.EmptyName);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task When_EmailIsEmpty_ShouldReturn_EmptyEmailError(string email)
    {
        // Arrange
        var validator = new CreateUserRequestValidator();
        var request = new CreateRegularUserRequest("Valid Name", email, "V@l1dPwd");

        // Act
        var result = await validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email).WithErrorMessage(CreateUserRequestValidator.EmptyEmail);
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
        var validator = new CreateUserRequestValidator();
        var request = new CreateRegularUserRequest("Valid Name", email, "V@l1dPwd");

        // Act
        var result = await validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email).WithErrorMessage(CreateUserRequestValidator.InvalidEmail);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task When_PasswordIsEmpty_ShouldReturn_EmptyPasswordError(string password)
    {
        // Arrange
        var validator = new CreateUserRequestValidator();
        var request = new CreateRegularUserRequest("Valid Name", "valid@email.com", password);

        // Act
        var result = await validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Password).WithErrorMessage(CreateUserRequestValidator.EmptyPassword);
    }

    [Theory]
    [InlineData("inVal1dpwd")]
    [InlineData("inV@lidpwd")]
    [InlineData("inv@l1dpwd")]
    [InlineData("INV@L1DPWD")]
    [InlineData("V@l1dpd")]
    public async Task When_PasswordIsInvalid_ShouldReturn_InvalidPasswordError(string password)
    {
        // Arrange
        var validator = new CreateUserRequestValidator();
        var request = new CreateRegularUserRequest("Valid Name", "valid@email.com", password);

        // Act
        var result = await validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Password).WithErrorMessage(CreateUserRequestValidator.InvalidPassword);
    }

    [Fact]
    public async Task When_RequestIsValid_ShouldNotReturnErrors()
    {
        // Arrange
        var validator = new CreateUserRequestValidator();
        var request = new CreateRegularUserRequest("Valid Name", "valid@email.com", "V@l1dPwd");

        // Act
        var result = await validator.TestValidateAsync(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
