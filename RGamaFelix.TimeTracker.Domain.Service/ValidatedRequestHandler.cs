using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using RGamaFelix.ServiceResponse;
using RGamaFelix.TimeTracker.Domain.Service.Handler;
using RGamaFelix.TimeTracker.ServiceResponse.FluentValidation;

namespace RGamaFelix.TimeTracker.Domain.Service;

public abstract class
    ValidatedRequestHandler<TRequest, TResponse> : IRequestHandler<TRequest, IServiceResultOf<TResponse>>
    where TRequest : IRequest<IServiceResultOf<TResponse>>
{
    private readonly IValidator<TRequest> _validator;
    protected readonly ILogger Logger;

    protected ValidatedRequestHandler(IValidator<TRequest> validator, ILogger logger)
    {
        _validator = validator;
        Logger = logger;
    }

    public async Task<IServiceResultOf<TResponse>> Handle(TRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (validationResult.IsValid)
        {
            return await HandleValidatedRequest(request, cancellationToken);
        }

        Logger.LogWarning("Request validation failed for {RequestType}: {Errors}", nameof(TRequest),
            string.Join(',', validationResult.Errors.Select(x => x.ErrorMessage)));
        return validationResult.ToErrorServiceResultOf<TResponse>();
    }

    protected abstract Task<IServiceResultOf<TResponse>> HandleValidatedRequest(TRequest request,
        CancellationToken cancellationToken);
}
