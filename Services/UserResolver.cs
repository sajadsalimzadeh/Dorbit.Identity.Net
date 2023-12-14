using Dorbit.Framework.Attributes;
using Dorbit.Framework.Models.Abstractions;
using Dorbit.Framework.Services.Abstractions;

namespace Dorbit.Identity.Services;

[ServiceRegister]
public class UserResolver : IUserResolver
{
    public IUserDto User { get; set; }
}