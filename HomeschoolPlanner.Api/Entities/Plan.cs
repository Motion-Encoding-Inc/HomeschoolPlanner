namespace HomeschoolPlanner.Data;

/// Planning entity linking a resource to a date range and strategy.
public class Plan
{
    public Guid Id { get; set; }
    public Guid ResourceId { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    /// Allowed days of week encoded as bitmask Monday=1<<0 .. Sunday=1<<6.
    public int AllowedDaysMask { get; set; }
    /// "push" | "catchup" | "smart" (later) determines scheduling behaviour.
    public string Strategy { get; set; } = "push";
    /// Lookahead window for catchup/smart strategies.
    public int LookaheadDays { get; set; } = 7;
}
