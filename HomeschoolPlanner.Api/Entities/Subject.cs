namespace HomeschoolPlanner.Data;

public class Subject
{
    public Guid Id { get; set; }
    public Guid LearnerId { get; set; }
    public string Title { get; set; } = "";
    public string? ColorHex { get; set; }
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
}
