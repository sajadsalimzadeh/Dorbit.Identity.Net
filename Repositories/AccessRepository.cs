using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dorbit.Framework.Attributes;
using Dorbit.Framework.Extensions;
using Dorbit.Framework.Repositories;
using Dorbit.Identity.Contracts.Accesses;
using Dorbit.Identity.Databases;
using Dorbit.Identity.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Dorbit.Identity.Repositories;

[ServiceRegister]
public class AccessRepository(IdentityInMemoryDbContext dbContext, IMemoryCache memoryCache) : BaseRepository<Access>(dbContext)
{
    public async Task<HashSet<string>> GetTotalAccessibilityAsync(List<string> accessibility)
    {
        var key = $"{nameof(AccessRepository)}-{nameof(GetTotalAccessibilityAsync)}";
        var accessDictionary = await memoryCache.GetValueWithLockAsync(key, async () =>
        {
            var allAccesses = await Set().ToListAsync();
            allAccesses.ForEach(x =>
            {
                x.Name = x.Name.ToLower();
                x.Parent = allAccesses.FirstOrDefault(y => y.Id == x.ParentId);
            });

            void FindAllChildren(Access access, AccessWithChildrenDto accessWithChildren)
            {
                if (!accessWithChildren.Children.Add(access.Name.ToLower())) return;
                foreach (var childAccess in allAccesses.Where(x => x.ParentId == access.Id || x.Parent?.Name == access.Name))
                {
                    FindAllChildren(childAccess, accessWithChildren);
                }
            }

            var result = new Dictionary<string, HashSet<string>>();
            foreach (var access in allAccesses.DistinctBy(x => x.Name))
            {
                var accessWithChildren = new AccessWithChildrenDto();
                FindAllChildren(access, accessWithChildren);
                result.Add(access.Name.ToLower(), accessWithChildren.Children);
            }

            return result;
        }, TimeSpan.FromMinutes(10));

        var allAccessibility = new List<string>();
        foreach (var access in accessibility)
        {
            if (accessDictionary.TryGetValue(access, out var accesses))
            {
                allAccessibility.AddRange(accesses);
            }
        }

        return allAccessibility.Distinct().ToHashSet();
    }
}