using System;
using System.ComponentModel.DataAnnotations.Schema;
using Dorbit.Framework.Entities;
using Dorbit.Identity.Contracts;
using Dorbit.Identity.Contracts.Tokens;
using Innofactor.EfCoreJsonValueConverter;

namespace Dorbit.Identity.Entities;

[Table(nameof(Token), Schema = "identity")]
public class Token : CreateEntity
{
    public Guid UserId { get; set; }
    public DateTime ExpireTime { get; set; }
    public TokenState State { get; set; } = TokenState.Valid;
    
    [JsonField]
    public TokenInfo TokenInfo { get; set; }

    [ForeignKey(nameof(UserId))] public User User { get; set; }
}