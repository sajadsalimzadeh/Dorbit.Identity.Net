using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dorbit.Framework.Attributes;
using Dorbit.Framework.Extensions;
using Dorbit.Identity.Contracts.Privileges;
using Dorbit.Identity.Databases;
using Dorbit.Identity.Entities;
using Dorbit.Identity.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Dorbit.Identity.Services;

[ServiceRegister]
public class PrivilegeService(
    RoleRepository roleRepository,
    UserPrivilegeRepository userPrivilegeRepository,
    AccessRepository accessRepository)
{
    public async Task<IEnumerable<string>> GetAllByUserIdAsync(object id)
    {
        var privileges = await userPrivilegeRepository.Set().Where(x => x.UserId.Equals(id))
            .ToListAsyncWithCache($"User-{id}-Privileges", TimeSpan.FromSeconds(10));
        var allAccesses = await accessRepository.GetAllAccessHierarchyWithCache();
        var roles = await roleRepository.Set().ToListAsyncWithCache("Roles", TimeSpan.FromSeconds(10));
        var totalAccesses = new List<string>();
        foreach (var privilege in privileges)
        {
            if (privilege is not null)
            {
                var accesses = new List<string>();
                if (privilege.Roles is not null)
                {
                    var findRoles = roles.Where(role => privilege.Roles.Contains(role.Id.ToString()));
                    accesses.AddRange(findRoles.SelectMany(x => x.Accesses));
                }

                accesses.AddRange(privilege.Accesses);
                foreach (var access in accesses)
                {
                    var accessWithChildren = allAccesses.FirstOrDefault(x => x.Name == access);
                    if (accessWithChildren is not null)
                    {
                        totalAccesses.Add(accessWithChildren.Name);
                        totalAccesses.AddRange(accessWithChildren.Children);
                    }
                }
            }
        }

        return totalAccesses.Distinct();
    }

    public async Task<UserPrivilege> SaveAsync(PrivilegeSaveRequest request)
    {
        request.Accesses = request.Accesses.Select(x => x.ToLower()).ToList();
        var privilege = await userPrivilegeRepository.Set().FirstOrDefaultAsync(x => x.UserId == request.UserId);
        if (privilege is null)
        {
            privilege = await userPrivilegeRepository.InsertAsync(request.MapTo<UserPrivilege>());
        }
        else
        {
            privilege = await userPrivilegeRepository.UpdateAsync(request.MapTo(privilege));
        }

        return privilege;
    }
}