namespace RGamaFelix.TimeTracker.Domain.Service.HandlerPreProcessors;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class AutorizeRequestAttribute : Attribute
{
    public AutorizeRequestAttribute(IEnumerable<string>? roles = null, IEnumerable<string>? claims = null)
    {
        Roles = roles;
        Claims = claims;
    }

    public IEnumerable<string>? Claims { get; set; }
    public IEnumerable<string>? Roles { get; set; }
}