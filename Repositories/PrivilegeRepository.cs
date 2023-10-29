using Dorbit.Attributes;
using Dorbit.Database.Abstractions;
using Dorbit.Identity.Databases;
using Dorbit.Identity.Entities;
using Dorbit.Repositories;

namespace Dorbit.Identity.Repositories;

[ServiceRegister]
public class PrivilegeRepository : BaseRepository<Privilege>
{
    public PrivilegeRepository(IdentityDbContext dbContext) : base(dbContext)
    {
    }
}