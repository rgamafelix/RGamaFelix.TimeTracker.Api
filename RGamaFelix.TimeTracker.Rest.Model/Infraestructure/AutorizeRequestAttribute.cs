namespace RGamaFelix.TimeTracker.Rest.Model.Infraestructure;

[AttributeUsage(AttributeTargets.All)]
public class AutorizeRequestAttribute : Attribute
{
    public string[] Claims { get; set; }
    public string[] Roles { get; set; }
}