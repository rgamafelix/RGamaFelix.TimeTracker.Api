namespace RGamaFelix.TimeTracker.Infrastructure;

[AttributeUsage(AttributeTargets.All)]
public class AutorizeRequestAttribute : Attribute
{
    public string[] Claims { get; set; }
    public string[] Roles { get; set; }
}