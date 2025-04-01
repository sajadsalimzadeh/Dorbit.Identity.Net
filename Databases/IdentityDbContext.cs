using System;
using Dorbit.Framework.Attributes;
using Dorbit.Framework.Database;
using Dorbit.Identity.Entities;
using Microsoft.EntityFrameworkCore;

namespace Dorbit.Identity.Databases;

[ServiceRegister]
public class IdentityDbContext(DbContextOptions<IdentityDbContext> options, IServiceProvider serviceProvider)
    : EfDbContext(options, serviceProvider)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<UserPrivilege> Privileges { get; set; }
    public DbSet<Otp> Otp { get; set; }
    public DbSet<Token> Tokens { get; set; }
}