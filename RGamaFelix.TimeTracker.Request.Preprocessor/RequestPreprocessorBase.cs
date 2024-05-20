using System.Reflection;
using MediatR;
using Microsoft.Extensions.Logging;
using RGamaFelix.ServiceResponse;

namespace RGamaFelix.TimeTracker.Request.Preprocessor;

public abstract class RequestPreprocessorBase<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    protected readonly ILogger<RequestPreprocessorBase<TRequest, TResponse>> Logger;

    protected RequestPreprocessorBase(ILogger<RequestPreprocessorBase<TRequest, TResponse>> logger)
    {
        Logger = logger;
    }

    public abstract Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken);

    protected static TResponse CreateFailResponse(IEnumerable<string> errorMessages, ResultTypeCode resultTypeCode)
    {
        MethodInfo failMethod;
        if (typeof(TResponse).GenericTypeArguments.Length != 0)
        {
            var type = typeof(TResponse).GenericTypeArguments[0];
            var resultType = typeof(ServiceResultOf<>).MakeGenericType(type);
            failMethod = resultType.GetMethod("Fail",
                BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy, null, CallingConventions.Any,
                new[] { typeof(string[]), typeof(ResultTypeCode) }, null)!;
        }
        else
        {
            var resultType = typeof(ServiceResult);
            failMethod = resultType.GetMethod("Fail",
                BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy, null, CallingConventions.Any,
                new[] { typeof(string[]), typeof(ResultTypeCode) }, null)!;
        }

        return (TResponse)failMethod.Invoke(null, [errorMessages, resultTypeCode])!;
    }
}