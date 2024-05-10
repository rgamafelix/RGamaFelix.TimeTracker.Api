namespace RGamaFelix.TimeTracker.Application.Infrastructure;

[AttributeUsage(AttributeTargets.All)]
public class AuthorizeRequestAttribute : Attribute
{
    public string[]? Claims { get; set; }
    public string[]? Roles { get; set; }
}