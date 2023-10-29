using Dorbit.Database;
using Microsoft.EntityFrameworkCore;

namespace Dorbit.Identity.Databases;

public class IdentityDbContext : EfDbContext
{
    public IdentityDbContext(DbContextOptions<IdentityDbContext> options, IServiceProvider serviceProvider) : base(options, serviceProvider)
    {
    }
}