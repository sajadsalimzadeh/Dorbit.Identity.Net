using Dorbit.Framework.Attributes;
using Dorbit.Identity.Models.Users;
using Dorbit.Identity.Repositories;
using Dorbit.Identity.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Dorbit.Identity.Hosts;

[ServiceRegister(Lifetime = ServiceLifetime.Singleton)]
public class SeedHost : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    public SeedHost(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var sp = _serviceProvider.CreateScope().ServiceProvider;
        var userRepository = sp.GetService<UserRepository>();
        var userService = sp.GetService<UserService>();
        if (string.IsNullOrEmpty(App.Setting.Admin.Username)) return;

        if (!await userRepository.Set().AnyAsync(x => x.Username.ToLower() == "admin"))
        {
            await userService.AddAsync(new UserAddRequest()
            {
                Id = Guid.Parse("733cc50c-a40e-4b79-96f5-3f5654dd33f0"),
                Name = "admin",
                Username = App.Setting.Admin.Username,
                Password = App.Setting.Admin.Password,
                Cellphone = App.Setting.Admin.Cellphone,
                Email = App.Setting.Admin.Email,
                NeedResetPassword = true
            });
        }
    }
}