using MediatR;
using RGamaFelix.ServiceResponse;
using RGamaFElix.TimeTracker.Infrastructure;

namespace RGamaFelix.TimeTracker.Rest.Model;

[AutorizeRequest]
public record SignInRequest(string UserName, string Password) : IRequest<ServiceResultOf<AuthResponse>>;

public record AuthResponse(string AccessToken, string RefreshToken, string UserName);
