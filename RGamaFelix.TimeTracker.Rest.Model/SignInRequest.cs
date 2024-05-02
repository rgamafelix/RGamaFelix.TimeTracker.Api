using MediatR;
using RGamaFelix.ServiceResponse;
using RGamaFelix.TimeTracker.Rest.Model.Infraestructure;

namespace RGamaFelix.TimeTracker.Rest.Model;

[AutorizeRequest]
public record SignInRequest(string UserName, string Password) : IRequest<ServiceResultOf<AuthResponse>>;

public record AuthResponse(string AccessToken, string RefreshToken, string UserName);
