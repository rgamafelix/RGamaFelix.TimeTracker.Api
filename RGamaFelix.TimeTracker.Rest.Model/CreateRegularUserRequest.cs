using MediatR;
using RGamaFelix.ServiceResponse;
using RGamaFelix.TimeTracker.Infrastructure;

namespace RGamaFelix.TimeTracker.Rest.Model;

[AuthorizeRequest]
public record CreateRegularUserRequest(string Name, string Email, string Password)
    : IRequest<ServiceResultOf<CreateUserResponse>>;
