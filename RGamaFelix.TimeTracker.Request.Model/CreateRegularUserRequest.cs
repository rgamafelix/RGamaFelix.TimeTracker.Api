using MediatR;
using RGamaFelix.ServiceResponse;
using RGamaFelix.TimeTracker.Application.Infrastructure;

namespace RGamaFelix.TimeTracker.Request.Model;

[AuthorizeRequest]
public record CreateRegularUserRequest(string Name, string Email, string Password)
    : IRequest<ServiceResultOf<CreateUserResponse>>;