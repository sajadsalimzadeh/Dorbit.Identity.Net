using Dorbit.Framework.Attributes;
using Dorbit.Framework.Repositories;
using Dorbit.Identity.Databases.Abstractions;
using Dorbit.Identity.Entities;

namespace Dorbit.Identity.Repositories;

[ServiceRegister]
public class UserPrivilegeRepository(IIdentityDbContext dbContext) : BaseRepository<UserPrivilege>(dbContext);