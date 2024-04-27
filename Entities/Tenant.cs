using System;
using Dorbit.Framework.Entities;
using Dorbit.Framework.Entities.Abstractions;

namespace Dorbit.Identity.Entities;

public class Tenant : Entity, IFullAudit
{
    public string Name { get; set; }
    
    public DateTime CreationTime { get; set; }
    public string CreatorId { get; set; }
    public string CreatorName { get; set; }
    public DateTime ModificationTime { get; set; }
    public string ModifierId { get; set; }
    public string ModifierName { get; set; }
    public DateTime? DeletionTime { get; set; }
    public bool IsDeleted { get; set; }
    public string DeleterId { get; set; }
    public string DeleterName { get; set; }
}