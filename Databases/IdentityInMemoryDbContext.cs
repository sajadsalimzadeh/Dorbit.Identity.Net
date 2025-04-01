using System;
using Dorbit.Framework.Database;
using Dorbit.Identity.Entities;
using Microsoft.EntityFrameworkCore;

namespace Dorbit.Identity.Databases;

public class IdentityInMemoryDbContext(DbContextOptions<IdentityInMemoryDbContext> options, IServiceProvider serviceProvider)
    : EfDbContext(options, serviceProvider)
{
    public DbSet<Access> Accesses { get; set; }
}