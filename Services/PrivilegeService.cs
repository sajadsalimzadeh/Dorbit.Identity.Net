using System.Linq;
using System.Threading.Tasks;
using Dorbit.Framework.Attributes;
using Dorbit.Framework.Extensions;
using Dorbit.Identity.Contracts.Privileges;
using Dorbit.Identity.Entities;
using Dorbit.Identity.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Dorbit.Identity.Services;

[ServiceRegister]
public class PrivilegeService(UserPrivilegeRepository userPrivilegeRepository)
{
    public async Task<UserPrivilege> SaveAsync(PrivilegeSaveRequest request)
    {
        request.Accessibility = request.Accessibility?.Select(x => x.ToLower()).ToList();
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