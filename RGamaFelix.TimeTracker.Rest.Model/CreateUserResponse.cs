namespace RGamaFelix.TimeTracker.Rest.Model;

public record CreateUserResponse(Guid Id, string Name, string Email, IEnumerable<string> Roles);
