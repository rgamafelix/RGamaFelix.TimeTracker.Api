using FluentValidation;
using RGamaFelix.TimeTracker.Rest.Model;

namespace RGamaFelix.TimeTracker.Domain.Service.Validator;

public class RefreshTokenRequestValidator : AbstractValidator<RefreshTokenRequest>
{
    public const string EmptyRefreshToken = "EmptyRefreshToken";
    public const string EmptyUserName = "EmptyUserName";

    public RefreshTokenRequestValidator()
    {
        RuleFor(x => x.RefreshToken).NotEmpty().WithMessage(EmptyRefreshToken);
        RuleFor(x => x.UserName).NotEmpty().WithMessage(EmptyUserName);
    }
}