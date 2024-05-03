using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace RGamaFelix.TimeTracker.Infrastructure;

public class AuthorizedRequestPreprocessor<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<AuthorizedRequestPreprocessor<TRequest, TResponse>> _logger;

    public AuthorizedRequestPreprocessor(IHttpContextAccessor httpContextAccessor,
        ILogger<AuthorizedRequestPreprocessor<TRequest, TResponse>> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var authorizeRequestAttribute = request.GetType().GetCustomAttributes(typeof(AutorizeRequestAttribute), true)
            .FirstOrDefault();
        if (authorizeRequestAttribute is null)
        {
            _logger.LogDebug("No authorization required for {RequestType}", nameof(TRequest));
            var result = await next.Invoke();
            return result;
        }

        var token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];
        if (string.IsNullOrEmpty(token))
        {
            _logger.LogWarning("Unauthorized request for {RequestType}", nameof(TRequest));
            throw new Exception();
            //return ServiceResultOf<TResponse>.Fail("Unauthorized", ResultTypeCode.AuthorizationError);
        }

        var claims = _httpContextAccessor.HttpContext.User.Claims;
        if (authorizeRequestAttribute is AutorizeRequestAttribute authorizeRequest)
        {
            if (authorizeRequest.Roles is not null)
            {
                if (!authorizeRequest.Roles.Any(r => claims.Any(c => c.Type == "role" && c.Value == r)))
                {
                    _logger.LogWarning("Unauthorized request for {RequestType}", nameof(TRequest));
                    throw new Exception();
                    //return ServiceResultOf<TResponse>.Fail("Unauthorized", ResultTypeCode.AuthorizationError);
                }
            }

            if (authorizeRequest.Claims is not null)
            {
                if (!authorizeRequest.Claims.Any(r => claims.Any(c => c.Type == r)))
                {
                    _logger.LogWarning("Unauthorized request for {RequestType}", nameof(TRequest));
                    throw new Exception();
                    //return ServiceResultOf<TResponse>.Fail("Unauthorized", ResultTypeCode.AuthorizationError);
                }
            }
        }

        return await next.Invoke();
    }
}