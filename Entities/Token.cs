using System.ComponentModel.DataAnnotations.Schema;
using Dorbit.Entities;
using Dorbit.Identity.Enums;

namespace Dorbit.Identity.Entities;

public class Token : Entity
{
    public Guid UserId { get; set; }
    public string Os { get; set; }
    public string Platform { get; set; }
    public string AppName { get; set; }
    public string AppVersion { get; set; }
    public string Accesses { get; set; }
    public TokenState State { get; set; } = TokenState.Valid;

    [ForeignKey(nameof(UserId))] public User User { get; set; }
}