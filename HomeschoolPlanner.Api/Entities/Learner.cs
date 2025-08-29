namespace HomeschoolPlanner.Data;

public class Learner
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Name { get; set; } = "";
    public string Grade { get; set; } = "";
}
