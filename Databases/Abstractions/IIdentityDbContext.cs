using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Dorbit.Framework.Database.Abstractions;
using Dorbit.Framework.Entities.Abstractions;
using Dorbit.Identity.Entities;
using Microsoft.EntityFrameworkCore;

namespace Dorbit.Identity.Databases.Abstractions;

public interface IIdentityDbContext : IDbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<UserPrivilege> Privileges { get; set; }
    public DbSet<Otp> Otp { get; set; }
    public DbSet<Token> Tokens { get; set; }
}