using Dorbit.Framework.Attributes;
using Dorbit.Framework.Repositories;
using Dorbit.Identity.Databases;
using Dorbit.Identity.Entities;

namespace Dorbit.Identity.Repositories;

[ServiceRegister]
public class UserPrivilegeRepository(IdentityDbContext dbContext) : BaseRepository<UserPrivilege>(dbContext);