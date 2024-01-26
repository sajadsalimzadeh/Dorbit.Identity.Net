using System.Collections.Generic;

namespace Dorbit.Identity.Contracts.Accesses;

public class AccessWithChildrenDto
{
    public string Name { get; set; }
    public List<string> Children { get; set; } = new();
}