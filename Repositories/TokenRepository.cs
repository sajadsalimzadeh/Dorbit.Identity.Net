using Dorbit.Framework.Attributes;
using Dorbit.Framework.Repositories;
using Dorbit.Identity.Databases;
using Dorbit.Identity.Databases.Entities;

namespace Dorbit.Identity.Repositories;

[ServiceRegister]
public class TokenRepository : BaseRepository<Token>
{
    public TokenRepository(IdentityDbContext dbContext) : base(dbContext)
    {
    }
}