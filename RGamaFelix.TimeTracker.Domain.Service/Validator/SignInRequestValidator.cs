using FluentValidation;
using RGamaFelix.TimeTracker.Rest.Model;

namespace RGamaFelix.TimeTracker.Domain.Service.Validator;

public class SignInRequestValidator : AbstractValidator<SignInRequest>
{
    public const string Emptypassword = "EmptyPassword";
    public const string Emptyusername = "EmptyUserName";

    public SignInRequestValidator()
    {
        RuleFor(x => x.UserName).NotEmpty().WithMessage(Emptyusername);
        RuleFor(x => x.Password).NotEmpty().WithMessage(Emptypassword);
    }
}