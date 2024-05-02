using FluentValidation;
using RGamaFelix.TimeTracker.Rest.Model;

namespace RGamaFelix.TimeTracker.Domain.Service.Validator;

public class ListClientRequestValidator : AbstractValidator<ListClientRequest>
{
    public const string pagemustbegreaterthanzero = "PageMustBeGreaterThanZero";
    public const string pagesizemustbegreaterthanzero = "PageSizeMustBeGreaterThanZero";

    public ListClientRequestValidator()
    {
        RuleFor(x => x.Page).GreaterThan(0).When(x => x.PageSize > 0).WithMessage(pagemustbegreaterthanzero);
        RuleFor(x => x.PageSize).GreaterThan(0).When(x => x.Page > 0).WithMessage(pagesizemustbegreaterthanzero);
    }
}