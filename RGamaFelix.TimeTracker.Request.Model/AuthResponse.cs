namespace RGamaFelix.TimeTracker.Request.Model;

public record AuthResponse(string AccessToken, string RefreshToken, string UserName);