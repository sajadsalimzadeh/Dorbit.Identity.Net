namespace Dorbit.Identity.Entities.Policies;

public class EntityPolicy
{
    public string EntityName { get; set; }
    public Dictionary<string, string> Claims { get; set; }
}