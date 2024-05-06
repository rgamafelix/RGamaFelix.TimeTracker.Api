using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using RGamaFelix.ServiceResponse;

namespace RGamaFelix.TimeTracker.Infrastructure;

public class ValidatedRequestPreprocessor<TRequest, TResponse> : RequestPreprocessorBase<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IValidator<TRequest> _validator;

    public ValidatedRequestPreprocessor(IValidator<TRequest> validator,
        ILogger<ValidatedRequestPreprocessor<TRequest, TResponse>> logger) : base(logger)
    {
        _validator = validator;
    }

    public override async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (validationResult.IsValid)
        {
            return await next.Invoke();
        }

        Logger.LogWarning("Request validation failed for {RequestType}: {Errors}", nameof(TRequest),
            string.Join(',', validationResult.Errors.Select(x => x.ErrorMessage)));
        return CreateFailResponse(validationResult.Errors.Select(x => x.ErrorMessage), ResultTypeCode.InvalidData);
    }
}
