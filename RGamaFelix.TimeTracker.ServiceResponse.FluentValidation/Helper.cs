using FluentValidation.Results;
using RGamaFelix.ServiceResponse;

namespace RGamaFelix.TimeTracker.ServiceResponse.FluentValidation;

public static class Helper
{
    public static IServiceResult ToErrorServiceResult(this ValidationResult validationResult)
    {
        if (validationResult.IsValid)
        {
            throw new ArgumentException("Success validation result cannot be converted to error service result");
        }

        return ServiceResult.Fail(validationResult.Errors.Select(e => e.ErrorMessage).ToList(),
            ResultTypeCode.InvalidData);
    }

    public static IServiceResultOf<T> ToErrorServiceResultOf<T>(this ValidationResult validationResult)
    {
        if (validationResult.IsValid)
        {
            throw new ArgumentException("Success validation result cannot be converted to error service result");
        }

        return ServiceResultOf<T>.Fail(validationResult.Errors.Select(e => e.ErrorMessage).ToList(),
            ResultTypeCode.InvalidData);
    }
}