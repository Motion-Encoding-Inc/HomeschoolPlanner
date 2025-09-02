namespace HomeschoolPlanner.Data;

public class ResourceUnit
{
    public Guid Id { get; set; }
    public Guid ResourceId { get; set; }
    public int Index { get; set; }
    public string Label { get; set; } = "";
}
