using Dorbit.Framework.Attributes;
using Dorbit.Framework.Repositories;
using Dorbit.Identity.Databases.Entities;

namespace Dorbit.Identity.Databases.Repositories;

[ServiceRegister]
public class OtpRepository : BaseRepository<Otp>
{
    public OtpRepository(IdentityDbContext dbContext) : base(dbContext)
    {
    }
}