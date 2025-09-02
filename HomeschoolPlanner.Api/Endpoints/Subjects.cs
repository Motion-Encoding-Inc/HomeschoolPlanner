using HomeschoolPlanner.Data;
using Microsoft.EntityFrameworkCore;

namespace HomeschoolPlanner.Api.Endpoints;

public static class SubjectEndpoints
{
    public static async Task<IResult> Create(AppDbContext db, Subject dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Title))
            return Results.BadRequest(new { error = "Title is required" });

        var learnerExists = await db.Learners.AnyAsync(l => l.Id == dto.LearnerId);
        if (!learnerExists)
            return Results.BadRequest(new { error = "Learner not found" });

        dto.Id = Guid.NewGuid();
        dto.CreatedUtc = DateTime.UtcNow;
        db.Subjects.Add(dto);
        await db.SaveChangesAsync();

        return Results.Created($"/api/v1/subjects/{dto.Id}", dto);
    }

    public static async Task<IResult> List(Guid learnerId, AppDbContext db)
    {
        var subjects = await db.Subjects
            .Where(s => s.LearnerId == learnerId)
            .ToListAsync();
        return Results.Ok(subjects);
    }
}
