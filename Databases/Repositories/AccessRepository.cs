using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dorbit.Framework.Attributes;
using Dorbit.Framework.Extensions;
using Dorbit.Framework.Repositories;
using Dorbit.Identity.Contracts.Accesses;
using Dorbit.Identity.Databases.Entities;
using Microsoft.Extensions.Caching.Memory;

namespace Dorbit.Identity.Databases.Repositories;

[ServiceRegister]
public class AccessRepository : BaseRepository<Access>
{
    private readonly IMemoryCache _memoryCache;

    public AccessRepository(IdentityDbContext dbContext, IMemoryCache memoryCache) : base(dbContext)
    {
        _memoryCache = memoryCache;
    }

    public async Task<List<AccessWithChildrenDto>> GetAllAccessHierarchyWithCache()
    {
        var key = $"{nameof(AccessRepository)}-{nameof(GetAllAccessHierarchyWithCache)}";
        if (!_memoryCache.TryGetValue(key, out List<AccessWithChildrenDto> result))
        {
            var allAccesses = await Set().ToListAsyncWithCache(key, TimeSpan.FromMinutes(1));
            allAccesses.ForEach(x =>
            {
                x.Name = x.Name.ToLower();
                x.Parent = allAccesses.FirstOrDefault(y => y.Id == x.ParentId);
            });

            void FindAllChildren(Access access, AccessWithChildrenDto accessWithChildren)
            {
                if (accessWithChildren.Children.Contains(access.Name)) return;
                accessWithChildren.Children.Add(access.Name);
                foreach (var childAccess in allAccesses.Where(x => x.ParentId == access.Id || x.Parent?.Name == access.Name))
                {
                    FindAllChildren(childAccess, accessWithChildren);
                }
            }

            result = new List<AccessWithChildrenDto>();
            foreach (var access in allAccesses)
            {
                var accessWithChildren = new AccessWithChildrenDto()
                {
                    Name = access.Name.ToLower()
                };
                FindAllChildren(access, accessWithChildren);
                result.Add(accessWithChildren);
            }
        }

        return result;
    }
}