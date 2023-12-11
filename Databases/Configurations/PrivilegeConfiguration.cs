using Dorbit.Identity.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

namespace Dorbit.Identity.Databases.Configurations;

public class PrivilegeConfiguration : IEntityTypeConfiguration<Privilege>
{
    public void Configure(EntityTypeBuilder<Privilege> builder)
    {
        builder.Property(x => x.Accesses).HasConversion(
            x => JsonConvert.SerializeObject(x, new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.All,
                NullValueHandling = NullValueHandling.Ignore
            }),
            x => JsonConvert.DeserializeObject<List<string>>(x, new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.All
            })
        );
    }
}