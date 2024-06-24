using System;
using Dorbit.Framework.Extensions;
using Dorbit.Identity.Configs;
using Dorbit.Identity.Databases;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Dorbit.Identity.Installers;

public static class IdentityInstaller
{
    public static IServiceCollection AddDorbitIdentity(this IServiceCollection services, Configs configs)
    {
        AppIdentity.Setting = services.BindConfiguration<IdentityAppSetting>();

        services.AddDbContext<IdentityDbContext>(configs.DbContextConfiguration);
        services.AddDbContext<IdentityInMemoryDbContext>(o => o.UseInMemoryDatabase("IdentityInMemoryDb"));
        
        services.AddAutoMapper(typeof(IdentityInstaller).Assembly);

        services.AddControllers(typeof(IdentityInstaller).Assembly).AddODataDefault();

        services.Configure<ConfigAdmin>(configs.ConfigAdmin);

        return services;
    }

    public class Configs
    {
        public required IConfiguration ConfigAdmin { get; set; }
        public required Action<DbContextOptionsBuilder> DbContextConfiguration { get; set; }
    }
}