using MediatR;
using RGamaFelix.ServiceResponse;

namespace RGamaFelix.TimeTracker.Rest.Model;

public record CreateAdminUserRequest(string Name, string Email, string Password)
    : IRequest<ServiceResultOf<CreateUserResponse>>;