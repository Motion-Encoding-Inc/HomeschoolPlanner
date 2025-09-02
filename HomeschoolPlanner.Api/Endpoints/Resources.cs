using HomeschoolPlanner.Data;
using Microsoft.EntityFrameworkCore;

namespace HomeschoolPlanner.Api.Endpoints;

public static class ResourceEndpoints
{
    public static async Task<IResult> Create(AppDbContext db, Resource dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Title))
            return Results.BadRequest(new { error = "Title is required" });

        var subjectExists = await db.Subjects.AnyAsync(s => s.Id == dto.SubjectId);
        if (!subjectExists)
            return Results.BadRequest(new { error = "Subject not found" });

        dto.Id = Guid.NewGuid();
        db.Resources.Add(dto);
        await db.SaveChangesAsync();

        return Results.Created($"/api/v1/resources/{dto.Id}", dto);
    }

    public static async Task<IResult> BulkUnits(Guid id, string[] labels, AppDbContext db)
    {
        if (labels == null || labels.Length == 0)
            return Results.BadRequest(new { error = "Labels are required" });

        var resource = await db.Resources.FindAsync(id);
        if (resource == null)
            return Results.NotFound();

        var existing = await db.ResourceUnits
            .Where(u => u.ResourceId == id)
            .CountAsync();

        var units = labels.Select((label, i) => new ResourceUnit
        {
            Id = Guid.NewGuid(),
            ResourceId = id,
            Index = existing + i + 1,
            Label = label
        }).ToList();

        db.ResourceUnits.AddRange(units);
        await db.SaveChangesAsync();

        return Results.Ok(units);
    }
}
