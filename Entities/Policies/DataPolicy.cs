namespace Dorbit.Identity.Entities.Policies;

public class DataPolicy
{
    public string EntityNamePattern { get; set; }
    public List<TimePolicy> TimePolicies { get; set; }
}