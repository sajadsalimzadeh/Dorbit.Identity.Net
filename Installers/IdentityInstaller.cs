using Dorbit.Framework.Extensions;
using Dorbit.Identity.Databases;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Dorbit.Identity.Installers;

public static class IdentityInstaller
{
    public static IServiceCollection AddDorbitIdentity(this IServiceCollection services, Configuration configuration)
    {
        App.AppSetting = services.BindConfiguration<AppSetting>();

        services.AddDbContext<IdentityDbContext>(configuration.DbContextConfiguration);

        services.AddAutoMapper(typeof(IdentityInstaller).Assembly);

        services.AddControllers(typeof(IdentityInstaller).Assembly).AddODataDefault();

        return services;
    }

    public class Configuration
    {
        public required Action<DbContextOptionsBuilder> DbContextConfiguration { get; set; }
    }
}