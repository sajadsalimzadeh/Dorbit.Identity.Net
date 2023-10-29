using Dorbit.Attributes;
using Dorbit.Identity.Databases;
using Dorbit.Identity.Entities;
using Dorbit.Repositories;

namespace Dorbit.Identity.Repositories;

[ServiceRegister]
public class UserRepository : BaseRepository<User>
{
    public UserRepository(IdentityDbContext dbContext) : base(dbContext)
    {
    }
}