using System;

namespace Dorbit.Identity.Contracts.Tokens;

public class TokenDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public DateTime ExpireTime { get; set; }
    public TokenState State { get; set; }
    public TokenInfo TokenInfo { get; set; }
    public DateTime CreationTime { get; set; }
}