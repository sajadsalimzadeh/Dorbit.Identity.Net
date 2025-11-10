using Dorbit.Framework.Configs.Abstractions;
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
        services.AddDbContext<IdentityInMemoryDbContext>(o => o.UseInMemoryDatabase("IdentityInMemoryDb"));
        
        services.AddAutoMapper(typeof(IdentityInstaller).Assembly);

        services.AddControllers(typeof(IdentityInstaller).Assembly).AddODataDefault();

        configs.ConfigAdmin?.Configure(services);
        configs.ConfigSecurity?.Configure(services);
        configs.ConfigAppleOAuth?.Configure(services);
        configs.ConfigGoogleOAuth?.Configure(services);

        return services;
    }

    public class Configs(IConfiguration configuration)
    {
        public IConfig<ConfigAdmin> ConfigAdmin { get; init; } = configuration.GetConfig<ConfigAdmin>("Admin");
        public IConfig<ConfigIdentitySecurity> ConfigSecurity { get; init; } = configuration.GetConfig<ConfigIdentitySecurity>("Security");
        public IConfig<ConfigGoogleOAuth> ConfigGoogleOAuth { get; init; } = configuration.GetConfig<ConfigGoogleOAuth>("GoogleOAuth");
        public IConfig<ConfigAppleOAuth> ConfigAppleOAuth { get; init; } = configuration.GetConfig<ConfigAppleOAuth>("AppleOAuth");
    }
}