using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Dorbit.Entities;

namespace Dorbit.Identity.Entities;

public class Access : Entity
{
    [StringLength(32), Required] public string Name { get; set; }
    [StringLength(128)] public string Description { get; set; }
    public Guid? ParentId { get; set; }

    [ForeignKey(nameof(Id))] public Access Parent { get; set; }

    public ICollection<Access> Children { get; set; }
}