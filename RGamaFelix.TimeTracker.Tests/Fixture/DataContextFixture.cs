using Microsoft.EntityFrameworkCore;
using RGamaFelix.TimeTracker.DataContext;
using RGamaFelix.TimeTracker.Domain.Model;

namespace RGamaFelix.TimeTracker.Tests.Fixture;

public class DataContextFixture : IDisposable
{
    public DataContextFixture()
    {
        var options = new DbContextOptionsBuilder<TimeTrackerDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        Context = new TimeTrackerDbContext(options);
        Context.Users.Add(User.Create("ExistingUser", "existing@email.com"));
        Context.SaveChanges();
    }

    public TimeTrackerDbContext Context { get; }

    public void Dispose()
    {
        Context.Dispose();
    }
}