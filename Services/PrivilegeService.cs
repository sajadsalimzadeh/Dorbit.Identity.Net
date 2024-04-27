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
public class PrivilegeService
{
    private readonly IdentityDbContext _dbContext;
    private readonly PrivilegeRepository _privilegeRepository;
    private readonly AccessRepository _accessRepository;

    public PrivilegeService(
        IdentityDbContext dbContext,
        PrivilegeRepository privilegeRepository,
        AccessRepository accessRepository)
    {
        _dbContext = dbContext;
        _privilegeRepository = privilegeRepository;
        _accessRepository = accessRepository;
    }

    public async Task<IEnumerable<string>> GetAllByUserIdAsync(object id)
    {
        var privilege = await _privilegeRepository.Set().FirstOrDefaultAsync(x => x.UserId.Equals(id));
        var allAccesses = await _accessRepository.GetAllAccessHierarchyWithCache();
        var result = new List<string>();
        if (privilege is not null)
        {
            foreach (var access in privilege.Accesses)
            {
                var accessWithChildren = allAccesses.FirstOrDefault(x => x.Name == access);
                if (accessWithChildren is not null)
                {
                    result.Add(accessWithChildren.Name);
                    result.AddRange(accessWithChildren.Children);
                }
            }
        }

        return result.Distinct();
    }

    public async Task<Privilege> SaveAsync(PrivilegeSaveRequest request)
    {
        request.Accesses = request.Accesses.Select(x => x.ToLower()).ToList();
        var privilege = await _privilegeRepository.Set().FirstOrDefaultAsync(x => x.UserId == request.UserId);
        if (privilege is null)
        {
            privilege = await _privilegeRepository.InsertAsync(request.MapTo<Privilege>());
        }
        else
        {
            privilege = await _privilegeRepository.UpdateAsync(request.MapTo(privilege));
        }

        return privilege;
    }
}