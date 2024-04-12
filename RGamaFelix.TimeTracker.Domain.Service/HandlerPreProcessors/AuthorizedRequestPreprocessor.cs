using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using RGamaFelix.ServiceResponse;

namespace RGamaFelix.TimeTracker.Domain.Service.HandlerPreProcessors;

public class
    AuthorizedRequestPreprocessor<TRequest, TResponse> : IPipelineBehavior<TRequest, IServiceResultOf<TResponse>>
    where TRequest : IRequest<IServiceResultOf<TResponse>>
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    protected readonly ILogger _logger;

    protected AuthorizedRequestPreprocessor(IHttpContextAccessor httpContextAccessor, ILogger logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public async Task<IServiceResultOf<TResponse>> Handle(TRequest request,
        RequestHandlerDelegate<IServiceResultOf<TResponse>> next, CancellationToken cancellationToken)
    {
        var authorizeRequestAttribute = request.GetType().GetCustomAttributes(typeof(AutorizeRequestAttribute), true)
            .FirstOrDefault();
        if (authorizeRequestAttribute is null)
        {
            _logger.LogDebug("No authorization required for {RequestType}", nameof(TRequest));
            return await next.Invoke();
        }

        var token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];
        if (string.IsNullOrEmpty(token))
        {
            _logger.LogWarning("Unauthorized request for {RequestType}", nameof(TRequest));
            return ServiceResultOf<TResponse>.Fail("Unauthorized", ResultTypeCode.AuthorizationError);
        }

        var claims = _httpContextAccessor.HttpContext.User.Claims;
        if (authorizeRequestAttribute is AutorizeRequestAttribute authorizeRequest)
        {
            if (authorizeRequest.Roles is not null)
            {
                if (!authorizeRequest.Roles.Any(r => claims.Any(c => c.Type == "role" && c.Value == r)))
                {
                    _logger.LogWarning("Unauthorized request for {RequestType}", nameof(TRequest));
                    return ServiceResultOf<TResponse>.Fail("Unauthorized", ResultTypeCode.AuthorizationError);
                }
            }

            if (authorizeRequest.Claims is not null)
            {
                if (!authorizeRequest.Claims.Any(r => claims.Any(c => c.Type == r)))
                {
                    _logger.LogWarning("Unauthorized request for {RequestType}", nameof(TRequest));
                    return ServiceResultOf<TResponse>.Fail("Unauthorized", ResultTypeCode.AuthorizationError);
                }
            }
        }

        return await next.Invoke();
    }
}
