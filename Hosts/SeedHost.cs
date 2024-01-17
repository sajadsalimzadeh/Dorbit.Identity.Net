﻿using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Dorbit.Framework.Attributes;
using Dorbit.Framework.Hosts;
using Dorbit.Framework.Utils.Json;
using Dorbit.Identity.Entities;
using Dorbit.Identity.Extensions;
using Dorbit.Identity.Models.Privileges;
using Dorbit.Identity.Models.Users;
using Dorbit.Identity.Repositories;
using Dorbit.Identity.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
        if (string.IsNullOrEmpty(AppIdentity.Setting.Admin.Username)) return;

        if (!await userRepository.Set().AnyAsync(x => x.Username.ToLower() == "admin"))
        {
            await userService.AddAsync(new UserAddRequest()
            {
                Id = Guid.Parse("733cc50c-a40e-4b79-96f5-3f5654dd33f0"),
                Name = AppIdentity.Setting.Admin.Name ?? AppIdentity.Setting.Admin.Username,
                Username = AppIdentity.Setting.Admin.Username,
                Password = AppIdentity.Setting.Admin.Password,
                Cellphone = AppIdentity.Setting.Admin.Cellphone,
                Email = AppIdentity.Setting.Admin.Email,
                NeedResetPassword = true
            });
        }
        
        var accessRepository = sp.GetService<AccessRepository>();
        await accessRepository.SeedAccessAsync("accesses-identity");
        
        var privilegeService = serviceProvider.GetService<PrivilegeService>();

        var admin = await userRepository.GetByUsernameAsync("admin");
        if (admin is not null)
        {
            await privilegeService.SaveAsync(new PrivilegeSaveRequest()
            {
                UserId =admin.Id,
                Accesses = ["AllAccess"]
            });
        }
    }
}