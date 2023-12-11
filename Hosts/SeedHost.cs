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
        if (string.IsNullOrEmpty(App.AppSetting.Admin.Username)) return;

        if (!await userRepository.Set().AnyAsync(x => x.Username.ToLower() == "admin"))
        {
            await userService.AddAsync(new UserAddRequest()
            {
                Name = "admin",
                Username = App.AppSetting.Admin.Username,
                Password = App.AppSetting.Admin.Password,
                Cellphone = App.AppSetting.Admin.Cellphone,
                Email = App.AppSetting.Admin.Email,
                NeedResetPassword = true
            });
        }
    }
}