using System;
using Dorbit.Framework.Database;
using Dorbit.Identity.Entities;
using Microsoft.EntityFrameworkCore;

namespace Dorbit.Identity.Databases;

public class IdentityInMemoryDbContext : EfDbContext
{
    public IdentityInMemoryDbContext(DbContextOptions<IdentityInMemoryDbContext> options, IServiceProvider serviceProvider) : base(options, serviceProvider)
    {
    }
    
    public DbSet<Access> Accesses { get; set; }
}