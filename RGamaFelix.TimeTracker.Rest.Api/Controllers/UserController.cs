using System.Net.Mime;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using RGamaFelix.TimeTracker.Rest.Model;

namespace RGamaFelix.TimeTracker.Rest.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Consumes(MediaTypeNames.Application.Json)]
[Produces("application/json")]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;
    private readonly IMediator _mediator;

    public UserController(IMediator mediator, ILogger<UserController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpPost]
    [Route("[action]")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CreateUserResponse))]
    public async Task<IActionResult> CreateAdminUser(CreateAdminUserRequest request,
        CancellationToken cancellationToken = default)
    {
        return await ControllerHelper.ProcessRequest<CreateAdminUserRequest, CreateUserResponse>(_mediator, _logger,
            request, cancellationToken);
    }

    [HttpPost]
    [Route("[action]")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CreateUserResponse))]
    public async Task<IActionResult> CreateRegularUser(CreateRegularUserRequest request,
        CancellationToken cancellationToken = default)
    {
        return await ControllerHelper.ProcessRequest<CreateRegularUserRequest, CreateUserResponse>(_mediator, _logger,
            request, cancellationToken);
    }
}