using Dorbit.Framework.Attributes;
using Dorbit.Framework.Repositories;
using Dorbit.Identity.Databases.Entities;

namespace Dorbit.Identity.Databases.Repositories;

[ServiceRegister]
public class PrivilegeRepository : BaseRepository<Privilege>
{
    public PrivilegeRepository(IdentityDbContext dbContext) : base(dbContext)
    {
    }
}