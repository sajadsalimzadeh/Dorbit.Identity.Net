using Dorbit.Identity.Entities.Policies;

namespace Dorbit.Identity.Models.UserPrivileges;

public class PrivilegeDto
{
    public long Id { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public List<Policy> Policies { get; set; }
}