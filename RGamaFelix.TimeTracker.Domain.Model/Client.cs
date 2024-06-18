namespace RGamaFelix.TimeTracker.Domain.Model;

public class Client : IEntityBase
{
    private Client()
    { }

    public string Name { get; private set; }
    public string NormalizedName { get; private set; }
    public Guid Id { get; }

    public static Client Create(string name)
    {
        return new Client { Name = name, NormalizedName = name.ToUpperInvariant() };
    }

    public Client Rename(string name)
    {
        Name = name;
        NormalizedName = name.ToUpperInvariant();
        return this;
    }
}
