using Dorbit.Attributes;
using Dorbit.Identity.Databases;
using Dorbit.Identity.Entities;
using Dorbit.Repositories;

namespace Dorbit.Identity.Repositories;

[ServiceRegister]
public class AccessRepository : BaseRepository<Access>
{
    public AccessRepository(IdentityDbContext dbContext) : base(dbContext)
    {
    }
}