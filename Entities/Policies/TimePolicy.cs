namespace Dorbit.Identity.Entities.Policies;

public class TimePolicy
{
    public List<TimeRange> TimesRange { get; set; }
    public List<DateRange> DatesRange { get; set; }
    public List<byte> DaysOfWeek { get; set; }
    public List<byte> DaysOfMonth { get; set; }
}

public class TimeRange
{
    public TimeSpan Start { get; set; }
    public TimeSpan End { get; set; }
}

public class DateRange
{
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
}