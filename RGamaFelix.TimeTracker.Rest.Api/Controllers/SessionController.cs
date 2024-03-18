using MediatR;
using Microsoft.AspNetCore.Mvc;
using RGamaFelix.TimeTracker.Rest.Model;

namespace RGamaFelix.TimeTracker.Rest.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class SessionController
{
    private readonly IMediator _mediator;
    private readonly ILogger<SessionController> _logger;

    public SessionController(IMediator mediator, ILogger<SessionController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }
    [HttpPost]
    [Route("[action]")]
    public async Task<IActionResult> SignIn(SignInRequest request, CancellationToken cancellationToken = default)
    {
       return await ControllerHelper.ProcessRequest<SignInRequest, AuthResponse>(_mediator, _logger, request,
            cancellationToken);
    }
    
    [HttpPost]
    [Route("[action]")]
    public async Task<IActionResult> RefreshToken(RefreshTokenRequest request, CancellationToken cancellationToken = default)
    {
        return await ControllerHelper.ProcessRequest<RefreshTokenRequest, AuthResponse>(_mediator, _logger, request,
            cancellationToken);
    }
    
}