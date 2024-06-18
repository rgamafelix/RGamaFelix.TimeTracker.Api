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
        var result = await mediator.Send(request, cancellationToken);
        if (!result.IsSuccess)
        {
            logger.LogError("{REQUEST} failed: {ERROR}", nameof(TRequest), result.ToErrorString());
        }

        return result.ReturnServiceResult();
    }
}