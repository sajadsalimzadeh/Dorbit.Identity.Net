using System;
using System.Threading;
using System.Threading.Tasks;
using Dorbit.Framework.Attributes;
using Dorbit.Framework.Hosts;
using Dorbit.Identity.Contracts.Privileges;
using Dorbit.Identity.Contracts.Users;
using Dorbit.Identity.Extensions;
using Dorbit.Identity.Repositories;
using Dorbit.Identity.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Dorbit.Identity.Hosts;

[ServiceRegister(Lifetime = ServiceLifetime.Singleton)]
public class SeedHost : BaseHost
{
    public SeedHost(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    protected override async Task InvokeAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        var sp = ServiceProvider.CreateScope().ServiceProvider;

        var userRepository = sp.GetService<UserRepository>();
        var userService = sp.GetService<UserService>();
        if (string.IsNullOrEmpty(AppIdentity.Setting.Admin.Password)) return;

        var admin = await userRepository.GetAdminAsync();
        if (admin is null)
        {
            admin = await userService.AddAsync(new UserAddRequest()
            {
                Name = AppIdentity.Setting.Admin.Name ?? "admin",
                Username = "admin",
                Password = AppIdentity.Setting.Admin.Password,
                Cellphone = AppIdentity.Setting.Admin.Cellphone,
                Email = AppIdentity.Setting.Admin.Email,
                NeedResetPassword = true
            });
        }
        var accessRepository = sp.GetService<AccessRepository>();
        await accessRepository.SeedAccessAsync("Assets/accesses-identity.json");

        var privilegeService = serviceProvider.GetService<PrivilegeService>();

        if (admin is not null)
        {
            await privilegeService.SaveAsync(new PrivilegeSaveRequest()
            {
                UserId = admin.Id,
                Accesses = ["Admin"]
            });
        }
    }
}