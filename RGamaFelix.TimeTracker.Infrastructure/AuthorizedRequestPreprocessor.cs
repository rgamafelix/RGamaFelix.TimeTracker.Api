using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using RGamaFelix.ServiceResponse;

namespace RGamaFelix.TimeTracker.Infrastructure;

public class AuthorizedRequestPreprocessor<TRequest, TResponse> : RequestPreprocessorBase<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthorizedRequestPreprocessor(IHttpContextAccessor httpContextAccessor,
        ILogger<AuthorizedRequestPreprocessor<TRequest, TResponse>> logger) : base(logger)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public override async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var authorizeRequestAttribute = GetAuthorizeRequestAttribute(request);
        if (authorizeRequestAttribute is null)
        {
            return await ProcessUnAuthorizedRequest(next);
        }

        var token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];
        if (string.IsNullOrEmpty((string)token))
        {
            Logger.LogWarning("Unauthorized request for {RequestType}", nameof(TRequest));
            return CreateFailResponse(["Unauthorized request"], ResultTypeCode.AuthorizationError);
        }

        var claims = _httpContextAccessor.HttpContext.User.Claims;
        if (!authorizeRequestAttribute.Roles!.Any(r => claims.Any(c => c.Type == "role" && c.Value == r)))
        {
            Logger.LogWarning("Unauthorized request for {RequestType}", nameof(TRequest));
            return CreateFailResponse(["Unauthorized request"], ResultTypeCode.AuthorizationError);
        }

        if (!authorizeRequestAttribute.Claims!.Any(r => claims.Any(c => c.Type == r)))
        {
            Logger.LogWarning("Unauthorized request for {RequestType}", nameof(TRequest));
            return CreateFailResponse(["Unauthorized request"], ResultTypeCode.AuthorizationError);
        }

        return await next.Invoke();
    }

    private static AuthorizeRequestAttribute? GetAuthorizeRequestAttribute(TRequest request)
    {
        return request.GetType().GetCustomAttributes(typeof(AuthorizeRequestAttribute), true).FirstOrDefault() as
            AuthorizeRequestAttribute;
    }

    private async Task<TResponse> ProcessUnAuthorizedRequest(RequestHandlerDelegate<TResponse> next)
    {
        Logger.LogDebug("No authorization required for {RequestType}", nameof(TRequest));
        return await next.Invoke();
    }
}