using MediatR;
using RGamaFelix.ServiceResponse;

namespace RGamaFelix.TimeTracker.Rest.Model;

public record RefreshTokenRequest(string RefreshToken, string UserName) : IRequest<ServiceResultOf<AuthResponse>>;
