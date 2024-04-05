using Dorbit.Framework.Attributes;
using Dorbit.Framework.Repositories;
using Dorbit.Identity.Databases;
using Dorbit.Identity.Entities;

namespace Dorbit.Identity.Repositories;

[ServiceRegister]
public class PrivilegeRepository : BaseRepository<Privilege>
{
    public PrivilegeRepository(IdentityDbContext dbContext) : base(dbContext)
    {
    }
}