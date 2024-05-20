using MediatR;
using RGamaFelix.ServiceResponse;
using RGamaFelix.TimeTracker.Request.Preprocessor;

namespace RGamaFelix.TimeTracker.Request.Model;

[AuthorizeRequest]
public record CreateRegularUserRequest(string Name, string Email, string Password)
    : IRequest<ServiceResultOf<CreateUserResponse>>;