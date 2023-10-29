using Dorbit.Entities;
using Dorbit.Identity.Entities.Policies;

namespace Dorbit.Identity.Entities;

public class Privilege : FullEntity
{
    public Guid UserId { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public List<Policy> Policies { get; set; }
}