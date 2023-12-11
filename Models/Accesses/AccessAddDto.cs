using Dorbit.Framework.Entities.Abstractions;

namespace Dorbit.Identity.Models.Accesses;

public class AccessAddDto
{
    public string Name { get; set; }
    public Guid? ParentId { get; set; }
}