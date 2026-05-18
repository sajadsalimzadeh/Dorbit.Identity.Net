using System;
using System.Linq;
using Dorbit.Framework.Database;
using Dorbit.Framework.Extensions;
using Dorbit.Identity.Databases.Abstractions;
using Dorbit.Identity.Entities;
using Microsoft.EntityFrameworkCore;

namespace Dorbit.Identity.Databases;

public class IdentityDbContext<TUserEntity>(DbContextOptions options, IServiceProvider serviceProvider) : EfDbContext(options, serviceProvider), IIdentityDbContext<TUserEntity> where TUserEntity : UserBase
{
    
    public DbSet<Role> Roles { get; set; }
    public DbSet<Otp> Otp { get; set; }
    public DbSet<Token> Tokens { get; set; }
    public DbSet<UserPrivilege> UserPrivileges { get; set; }

    public DbSet<TUserEntity> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<TUserEntity>()
            .ToTable("User", "identity");
    }

    public override IQueryable<TEntity> DbSet<TEntity, TKey>(bool excludeDeleted = true)
    {
        if (typeof(TEntity) == typeof(UserBase))
        {
            return Users.AsNoTracking().ExcludeSoftDelete(excludeDeleted).Cast<TEntity>();
        }

        return base.DbSet<TEntity, TKey>(excludeDeleted);
    }
    
    public UserBase CreateNewUser()
    {
        return Activator.CreateInstance<TUserEntity>();
    }
}