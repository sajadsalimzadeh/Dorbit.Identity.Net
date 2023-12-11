using System.ComponentModel.DataAnnotations.Schema;
using Dorbit.Framework.Entities;
using Dorbit.Identity.Enums;

namespace Dorbit.Identity.Entities;

public class Token : Entity
{
    public Guid UserId { get; set; }
    public string Os { get; set; }
    public string Platform { get; set; }
    public string Application { get; set; }
    public string Accesses { get; set; }
    public TokenState State { get; set; } = TokenState.Valid;

    [ForeignKey(nameof(UserId))] public User User { get; set; }
}