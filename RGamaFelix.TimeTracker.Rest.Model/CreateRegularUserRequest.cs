using MediatR;
using RGamaFelix.ServiceResponse;

namespace RGamaFelix.TimeTracker.Rest.Model;

public record CreateRegularUserRequest(string Name, string Email, string Password)
    : IRequest<ServiceResultOf<CreateUserResponse>>;