using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using RGamaFelix.ServiceResponse;
using RGamaFelix.TimeTracker.ServiceResponse.FluentValidation;

namespace RGamaFelix.TimeTracker.Domain.Service.HandlerPreProcessors;

public class
    ValidatedRequestPreprocessor<TRequest, TResponse> : IPipelineBehavior<TRequest, IServiceResultOf<TResponse>>
    where TRequest : IRequest<IServiceResultOf<TResponse>>
{
    private readonly IValidator<TRequest> _validator;
    private readonly ILogger _logger;

    protected ValidatedRequestPreprocessor(IValidator<TRequest> validator, ILogger logger)
    {
        _validator = validator;
        _logger = logger;
    }

    public async Task<IServiceResultOf<TResponse>> Handle(TRequest request,
        RequestHandlerDelegate<IServiceResultOf<TResponse>> next, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (validationResult.IsValid)
        {
            return await next.Invoke();
        }

        _logger.LogWarning("Request validation failed for {RequestType}: {Errors}", nameof(TRequest),
            string.Join(',', validationResult.Errors.Select(x => x.ErrorMessage)));
        return validationResult.ToErrorServiceResultOf<TResponse>();
    }
}
