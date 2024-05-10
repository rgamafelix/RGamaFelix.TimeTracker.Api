using MediatR;
using RGamaFelix.ServiceResponse;

namespace RGamaFelix.TimeTracker.Request.Model;

public record RefreshTokenRequest(string RefreshToken, string UserName) : IRequest<ServiceResultOf<AuthResponse>>;