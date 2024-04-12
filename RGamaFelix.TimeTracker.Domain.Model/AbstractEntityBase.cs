namespace RGamaFelix.TimeTracker.Domain.Model;

public abstract class AbstractEntityBase : IEntityBase
{
    public Guid Id { get; set; }
}
