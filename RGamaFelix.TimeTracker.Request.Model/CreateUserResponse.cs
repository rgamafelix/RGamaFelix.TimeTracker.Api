namespace RGamaFelix.TimeTracker.Request.Model;

public record CreateUserResponse(Guid Id, string Name, string Email, IEnumerable<string> Roles);