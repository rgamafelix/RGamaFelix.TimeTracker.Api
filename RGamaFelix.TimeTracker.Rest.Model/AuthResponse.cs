namespace RGamaFelix.TimeTracker.Rest.Model;

public record AuthResponse(string AccessToken, string RefreshToken, string UserName);