using MediatR;
using RGamaFelix.ServiceResponse;

namespace RGamaFelix.TimeTracker.Request.Model;

public record CreateAdminUserRequest(string Name, string Email, string Password)
    : IRequest<ServiceResultOf<CreateUserResponse>>;