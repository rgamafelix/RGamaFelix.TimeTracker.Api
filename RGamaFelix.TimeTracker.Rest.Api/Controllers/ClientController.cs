using System.Net.Mime;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using RGamaFelix.TimeTracker.Rest.Model;

namespace RGamaFelix.TimeTracker.Rest.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ClientController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ClientController> _logger;

    public ClientController(IMediator mediator, ILogger<ClientController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> ListClient(ListClientRequest request,
        CancellationToken cancellationToken = default)
    {
        return await ControllerHelper.ProcessRequest<ListClientRequest, PagedResponse<ListClientResponse>>(_mediator, _logger, request,
            cancellationToken);
    }
}
