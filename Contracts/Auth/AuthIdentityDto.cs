using Dorbit.Framework.Contracts.Identities;
using Dorbit.Identity.Contracts.Users;

namespace Dorbit.Identity.Contracts.Auth;

public class AuthIdentityDto : IdentityDto
{
    public new UserDto User { get; set; }
}