using System;
using System.Threading;
using System.Threading.Tasks;
using Dorbit.Framework.Attributes;
using Dorbit.Framework.Hosts;
using Dorbit.Identity.Configs;
using Dorbit.Identity.Contracts.Privileges;
using Dorbit.Identity.Contracts.Users;
using Dorbit.Identity.Extensions;
using Dorbit.Identity.Repositories;
using Dorbit.Identity.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Dorbit.Identity.Hosts;

[ServiceRegister(Lifetime = ServiceLifetime.Singleton, Order = -10)]
public class SeedHost(IServiceProvider serviceProvider) : BaseHost(serviceProvider)
{
    protected override async Task InvokeAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        var userRepository = serviceProvider.GetRequiredService<UserRepository>();
        var userService = serviceProvider.GetRequiredService<UserService>();
        var configAdmin = serviceProvider.GetRequiredService<IOptions<ConfigAdmin>>()?.Value;
        if (string.IsNullOrEmpty(configAdmin?.Password)) return;

        var admin = await userRepository.GetAdminAsync();
        if (admin is null)
        {
            admin = await userService.AddAsync(new UserAddRequest()
            {
                Name = configAdmin.Name ?? "admin",
                Username = "admin",
                Password = configAdmin.Password,
                Cellphone = configAdmin.Cellphone,
                Email = configAdmin.Email,
                NeedResetPassword = true,
                MaxTokenCount = 3
            });
        }
        var accessRepository = serviceProvider.GetRequiredService<AccessRepository>();
        await accessRepository.SeedAccessAsync("Assets/accesses-identity.json");
        await accessRepository.SeedAccessAsync("Assets/accesses-framework.json");

        if (admin is not null)
        {
            await userService.SavePrivilegeAsync(new PrivilegeSaveRequest()
            {
                UserId = admin.Id,
                IsFullAccess = true,
            });
        }
    }
}