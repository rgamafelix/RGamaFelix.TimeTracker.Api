using FluentValidation;
using RGamaFelix.TimeTracker.Rest.Model;

namespace RGamaFelix.TimeTracker.Domain.Service.Validator;

public class CreateUserRequestValidator : AbstractValidator<CreateRegularUserRequest>
{
    public const string EmptyEmail = "EmptyEmail";
    public const string EmptyName = "EmptyName";
    public const string EmptyPassword = "EmptyPassword";
    public const string InvalidEmail = "InvalidEmail";
    public const string InvalidPassword = "InvalidPassword";
    public const string PasswordMustContainLowerCaseLetter = "PasswordMustContainLowercaseLetter";
    public const string PasswordMustContainNumber = "PasswordMustContainNumber";
    public const string PasswordMustContainSpecialCharacter = "PasswordMustContainSpecialCharacter";
    public const string PasswordMustContainUpperCaseLetter = "PasswordMustContainUppercaseLetter";
    public const string PasswordTooShort = "PasswordTooShort";

    public CreateUserRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage(EmptyName);
        RuleFor(x => x.Email).NotEmpty().WithMessage(EmptyEmail);
        RuleFor(x => x.Email).Matches(@"^(?!\.)[a-zA-Z0-9._%+-]+@(?:[a-zA-Z0-9-]+\.)+[a-zA-Z]{2,}$")
            .WithMessage(InvalidEmail);
        RuleFor(x => x.Password).NotEmpty().WithMessage(EmptyPassword);
        RuleFor(x => x.Password).Matches(@"\d").WithMessage(PasswordMustContainNumber);
        RuleFor(x => x.Password).Matches(@"[a-z]").WithMessage(PasswordMustContainLowerCaseLetter);
        RuleFor(x => x.Password).Matches(@"[A-Z]").WithMessage(PasswordMustContainUpperCaseLetter);
        RuleFor(x => x.Password).Matches(@"[^\da-zA-Z]").WithMessage(PasswordMustContainSpecialCharacter);
        RuleFor(x => x.Password).MinimumLength(8).WithMessage(PasswordTooShort);
    }
}