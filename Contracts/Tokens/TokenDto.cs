namespace Dorbit.Identity.Contracts.Tokens;

public class TokenDto
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public TokenState State { get; set; }
}