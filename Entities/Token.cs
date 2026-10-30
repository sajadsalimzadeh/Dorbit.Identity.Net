using System;
using System.ComponentModel.DataAnnotations.Schema;
using Dorbit.Framework.Entities;
using Dorbit.Framework.Utils.Json;
using Dorbit.Identity.Contracts.Tokens;

namespace Dorbit.Identity.Entities;

[Table(nameof(Token), Schema = "identity")]
public class Token : CreateEntity
{
    public Guid UserId { get; set; }
    public DateTime ExpireTime { get; set; }
    public DateTime TerminateTime { get; set; }
    public TokenState State { get; set; } = TokenState.Valid;

    public bool IsApiToken { get; set; }
    
    [JsonField]
    public TokenInfo TokenInfo { get; set; }

    [ForeignKey(nameof(UserId))] 
    public UserBase User { get; set; }
}