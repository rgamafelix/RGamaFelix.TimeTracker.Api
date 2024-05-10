using MediatR;
using RGamaFelix.ServiceResponse;

namespace RGamaFelix.TimeTracker.Request.Model;

public record SignInRequest(string UserName, string Password) : IRequest<ServiceResultOf<AuthResponse>>;