using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace RGamaFelix.TimeTracker.Infrastructure;

public class ValidatedRequestPreprocessor<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<ValidatedRequestPreprocessor<TRequest, TResponse>> _logger;
    private readonly IValidator<TRequest> _validator;

    public ValidatedRequestPreprocessor(IValidator<TRequest> validator,
        ILogger<ValidatedRequestPreprocessor<TRequest, TResponse>> logger)
    {
        _validator = validator;
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (validationResult.IsValid)
        {
            return await next.Invoke();
        }

        _logger.LogWarning("Request validation failed for {RequestType}: {Errors}", nameof(TRequest),
            string.Join(',', validationResult.Errors.Select(x => x.ErrorMessage)));
        throw new ValidationException(validationResult.Errors);
        //return validationResult.ToErrorServiceResultOf<TResponse>();
    }
}