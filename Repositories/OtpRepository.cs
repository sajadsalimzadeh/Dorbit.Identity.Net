using Dorbit.Attributes;
using Dorbit.Identity.Databases;
using Dorbit.Identity.Entities;
using Dorbit.Repositories;

namespace Dorbit.Identity.Repositories;

[ServiceRegister]
public class OtpRepository : BaseRepository<Otp>
{
    public OtpRepository(IdentityDbContext dbContext) : base(dbContext)
    {
    }
}