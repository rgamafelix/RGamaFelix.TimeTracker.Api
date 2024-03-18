using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RGamaFelix.TimeTracker.Domain.Model;

namespace RGamaFelix.TimeTracker.Repository;

public partial class TimeTrackerDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
{
    public TimeTrackerDbContext()
    {
        
    }
    public TimeTrackerDbContext(DbContextOptions<TimeTrackerDbContext> options) : base(options)
    { }

    public DbSet<Client> Clients { get; set; }
    public DbSet<Audit> Audits { get; set; }

    protected override void OnModelCreating(ModelBuilder model)
    {
        base.OnModelCreating(model);
        model.Entity<Client>(entity => { entity.ToTable("Client"); });
    }
}
