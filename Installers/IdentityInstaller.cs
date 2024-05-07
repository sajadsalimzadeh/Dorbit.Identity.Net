using System;
using Dorbit.Framework.Extensions;
using Dorbit.Identity.Databases;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Dorbit.Identity.Installers;

public static class IdentityInstaller
{
    public static IServiceCollection AddDorbitIdentity(this IServiceCollection services, Configuration configuration)
    {
        AppIdentity.Setting = services.BindConfiguration<IdentityAppSetting>();

        services.AddDbContext<IdentityDbContext>(options =>
        {
            configuration.DbContextConfiguration(options);
            options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        });

        services.AddAutoMapper(typeof(IdentityInstaller).Assembly);

        services.AddControllers(typeof(IdentityInstaller).Assembly).AddODataDefault();

        return services;
    }

    public class Configuration
    {
        public required Action<DbContextOptionsBuilder> DbContextConfiguration { get; set; }
    }
}