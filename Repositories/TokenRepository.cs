using Dorbit.Attributes;
using Dorbit.Database.Abstractions;
using Dorbit.Identity.Databases;
using Dorbit.Identity.Entities;
using Dorbit.Repositories;

namespace Dorbit.Identity.Repositories;

[ServiceRegister]
public class TokenRepository : BaseRepository<Token>
{
    public TokenRepository(IdentityDbContext dbContext) : base(dbContext)
    {
    }
}