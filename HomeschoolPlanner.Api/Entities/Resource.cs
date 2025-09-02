namespace HomeschoolPlanner.Data;

public enum ResourceType { Book = 1, Time = 2, Custom = 3 }

public class Resource
{
    public Guid Id { get; set; }
    public Guid SubjectId { get; set; }
    public ResourceType Type { get; set; }
    public string Title { get; set; } = "";
    public int? MinutesPerOccurrence { get; set; }
    public int? MaxUnitsPerDay { get; set; }
}
