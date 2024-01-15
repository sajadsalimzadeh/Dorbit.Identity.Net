using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dorbit.Framework.Attributes;
using Dorbit.Framework.Extensions;
using Dorbit.Framework.Repositories;
using Dorbit.Identity.Databases;
using Dorbit.Identity.Entities;
using Microsoft.EntityFrameworkCore;

namespace Dorbit.Identity.Repositories;

[ServiceRegister]
public class UserRepository : BaseRepository<User>
{
    private readonly IdentityDbContext _dbContext;

    public UserRepository(IdentityDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<User> GetByUsernameAsync(string username)
    {
        return Set().FirstOrDefaultAsync(x => x.Username == username);
    }

    public Task<List<Privilege>> GetAllUserPrivileges(Guid id)
    {
        var now = DateTime.UtcNow;
        return _dbContext.Set<Privilege>()
            .Where(x => x.UserId == id && x.StartTime < now && x.EndTime > now)
            .ToListAsyncWithCache($"{nameof(GetAllUserPrivileges)}-{id}", TimeSpan.FromMinutes(5));
    }
}