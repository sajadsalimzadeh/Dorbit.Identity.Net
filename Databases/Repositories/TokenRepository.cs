using Dorbit.Framework.Attributes;
using Dorbit.Framework.Repositories;
using Dorbit.Identity.Databases.Entities;

namespace Dorbit.Identity.Databases.Repositories;

[ServiceRegister]
public class TokenRepository : BaseRepository<Token>
{
    public TokenRepository(IdentityDbContext dbContext) : base(dbContext)
    {
    }
}