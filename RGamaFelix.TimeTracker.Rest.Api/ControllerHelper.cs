using MediatR;
using Microsoft.AspNetCore.Mvc;
using RGamaFelix.ServiceResponse;
using RGamaFelix.ServiceResponse.RestResponse;

namespace RGamaFelix.TimeTracker.Rest.Api;

public static class ControllerHelper
{
    public static async Task<IActionResult> ProcessRequest<TRequest, TResponse>(IMediator mediator, ILogger logger,
        TRequest request, CancellationToken cancellationToken) where TRequest : IRequest<IServiceResultOf<TResponse>>
    {
        try
        {
            var result = await mediator.Send(request, cancellationToken);
            if (!result.IsSuccess)
            {
                logger.LogError("{REQUEST} failed: {ERROR}", nameof(TRequest), result.ToErrorString());
            }

            return result.ReturnServiceResult();
        }
        catch (Exception e)
        {
            logger.LogCritical(e, "Error processing {REQUEST}", nameof(TRequest));
            throw;
        }
    }
}
