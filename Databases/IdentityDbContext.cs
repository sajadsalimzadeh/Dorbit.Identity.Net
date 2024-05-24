using System;
using Dorbit.Framework.Attributes;
using Dorbit.Framework.Database;
using Dorbit.Identity.Entities;
using Microsoft.EntityFrameworkCore;

namespace Dorbit.Identity.Databases;

[ServiceRegister]
public class IdentityDbContext : EfDbContext
{
    public IdentityDbContext(DbContextOptions<IdentityDbContext> options, IServiceProvider serviceProvider) : base(options, serviceProvider)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(IdentityDbContext).Assembly);

        modelBuilder.HasDefaultSchema("identity");
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Privilege> Privileges { get; set; }
    public DbSet<Otp> Otp { get; set; }
    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<Token> Tokens { get; set; }
}