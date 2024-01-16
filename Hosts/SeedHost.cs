using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Dorbit.Framework.Attributes;
using Dorbit.Framework.Entities.Abstractions;
using Dorbit.Framework.Hosts;
using Dorbit.Framework.Utils.Json;
using Dorbit.Identity.Entities;
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
        var accessRepository = sp.GetService<AccessRepository>();

        var allAccesses = await accessRepository.Set().Include(x => x.Parent).ToListAsync(cancellationToken: cancellationToken);

        var accessFilename = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets/accesses.json");
        if (File.Exists(accessFilename))
        {
            var json = await File.ReadAllTextAsync(accessFilename, cancellationToken);
            var accesses = JsonSerializer.Deserialize<List<Access>>(json);
            await ImportAsync(accesses, null);
        }


        var userService = sp.GetService<UserService>();
        if (string.IsNullOrEmpty(AppIdentity.Setting.Admin.Username)) return;

        if (!await userRepository.Set().AnyAsync(x => x.Username.ToLower() == "admin", cancellationToken: cancellationToken))
        {
            await userService.AddAsync(new UserAddRequest()
            {
                Id = Guid.Parse("733cc50c-a40e-4b79-96f5-3f5654dd33f0"),
                Name = "admin",
                Username = AppIdentity.Setting.Admin.Username,
                Password = AppIdentity.Setting.Admin.Password,
                Cellphone = AppIdentity.Setting.Admin.Cellphone,
                Email = AppIdentity.Setting.Admin.Email,
                NeedResetPassword = true
            });
        }

        return;

        async Task ImportAsync(IEnumerable<Access> accesses, IEntity parent)
        {
            foreach (var access in accesses)
            {
                if (allAccesses.Any(x => x.Name == access.Name && x.Parent?.Name == access.Parent.Name)) continue;
                var children = access.Children;
                access.ParentId = parent?.Id;
                access.Children = null;
                var entity = await accessRepository.InsertAsync(access);
                if (children is not null) await ImportAsync(children, entity);
            }
        }
    }
}