﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RGamaFelix.TimeTracker.Domain.Model;

namespace RGamaFelix.TimeTracker.Repository;

public class TimeTrackerDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
{
    public TimeTrackerDbContext()
    {
    }

    public TimeTrackerDbContext(DbContextOptions<TimeTrackerDbContext> options) : base(options)
    {
    }

    public DbSet<Client> Clients { get; set; }
    public DbSet<User> User { get; set; }

    protected override void OnModelCreating(ModelBuilder model)
    {
        base.OnModelCreating(model);
        model.Entity<Client>(entity =>
        {
            entity.ToTable("Client");
            entity.HasKey(e => e.Id);
        });
        model.Entity<User>(entity =>
        {
            entity.ToTable("User");
            entity.HasKey(e => e.Id);
            entity.HasMany(e => e.Sessions).WithOne().HasForeignKey("UserId");
        });
        model.Entity<Session>(entity =>
        {
            entity.ToTable("Session");
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.User).WithMany(e => e.Sessions).HasForeignKey("UserId");
            entity.HasOne(e => e.ReplacedBy).WithMany().HasForeignKey("ReplacedById");
        });
    }
}
