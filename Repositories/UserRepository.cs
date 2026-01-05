using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dorbit.Framework.Attributes;
using Dorbit.Framework.Contracts;
using Dorbit.Framework.Extensions;
using Dorbit.Framework.Repositories;
using Dorbit.Identity.Configs;
using Dorbit.Identity.Databases.Abstractions;
using Dorbit.Identity.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Dorbit.Identity.Repositories;

[ServiceRegister]
public class UserRepository(IIdentityDbContext dbContext, IOptions<ConfigAdmin> configAdminOptions) : BaseRepository<User>(dbContext)
{
    public Task<User> GetByUsernameAsync(string value)
    {
        if (dbContext.ProviderType == DatabaseProviderType.InMemory)
        {
            return Set(false).FirstOrDefaultAsync(x => string.Equals(x.Username, value, StringComparison.OrdinalIgnoreCase));
        }

        return Set(false).FirstOrDefaultAsync(x => EF.Functions.ILike(x.Username, value));
    }

    public Task<User> GetByCellphoneAsync(string value)
    {
        return Set().FirstOrDefaultAsync(x => x.Cellphone == value);
    }

    public Task<User> GetByEmailAsync(string value)
    {
        return Set().FirstOrDefaultAsync(x => x.Email == value);
    }

    public Task<User> GetAdminAsync()
    {
        var adminUsername = configAdminOptions.Value.Username;
        return Set().FirstOrDefaultAsync(x => x.Username.ToLower() == adminUsername);
    }

    public Task<List<UserPrivilege>> GetAllUserPrivileges(Guid id)
    {
        var now = DateTime.UtcNow;
        return dbContext.DbSet<UserPrivilege>()
            .Where(x => x.UserId == id && x.From < now && x.To > now)
            .ToListAsyncWithCache($"{nameof(GetAllUserPrivileges)}-{id}", TimeSpan.FromMinutes(5));
    }
}