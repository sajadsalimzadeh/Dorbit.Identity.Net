using Dorbit.Framework.Attributes;
using Dorbit.Framework.Repositories;
using Dorbit.Identity.Databases;
using Dorbit.Identity.Entities;

namespace Dorbit.Identity.Repositories;

[ServiceRegister]
public class OtpRepository : BaseRepository<Otp>
{
    public OtpRepository(IdentityDbContext dbContext) : base(dbContext)
    {
    }
}