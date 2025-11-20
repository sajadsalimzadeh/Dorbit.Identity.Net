using Dorbit.Framework.Contracts.Identities;
using Dorbit.Identity.Contracts.Users;

namespace Dorbit.Identity.Contracts.Auth;

public class AuthIdentityDto : IdentityDto
{
    public new UserDto User { get; set; }
    public bool IsCellphoneVerified { get; set; }
    public bool IsEmailVerified { get; set; }
}