using HomeschoolPlanner.Data;
using HomeschoolPlanner.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.FSharp.Core;

namespace HomeschoolPlanner.Api.Endpoints;

/// Endpoints for creating plans and previewing schedules.
public static class PlanEndpoints
{
    /// Create a new plan for a resource.
    public static async Task<IResult> Create(AppDbContext db, Plan dto)
    {
        if (dto.StartDate > dto.EndDate)
            return Results.BadRequest(new { error = "StartDate must be before EndDate" });

        var resourceExists = await db.Resources.AnyAsync(r => r.Id == dto.ResourceId);
        if (!resourceExists)
            return Results.BadRequest(new { error = "Resource not found" });

        dto.Id = Guid.NewGuid();
        db.Plans.Add(dto);
        await db.SaveChangesAsync();

        return Results.Created($"/api/v1/plans/{dto.Id}", dto);
    }

    /// Generate a schedule preview for a plan.
    public static async Task<IResult> GetSchedule(Guid id, DateOnly from, int days, AppDbContext db)
    {
        if (days <= 0 || days > 365)
            return Results.BadRequest(new { error = "days must be between 1 and 365" });

        var plan = await db.Plans.FindAsync(id);
        if (plan == null)
            return Results.NotFound();

        var resource = await db.Resources.FindAsync(plan.ResourceId);
        if (resource == null)
            return Results.BadRequest(new { error = "Resource not found" });

        int? units = null;
        int? minutes = null;
        if (resource.Type == Data.ResourceType.Book)
        {
            units = await db.ResourceUnits.CountAsync(u => u.ResourceId == plan.ResourceId);
        }
        else if (resource.Type == Data.ResourceType.Time)
        {
            minutes = resource.MinutesPerOccurrence;
        }

        var to = from.AddDays(days - 1);
        // Clamp preview within plan range
        var start = from < plan.StartDate ? plan.StartDate : from;
        var end = to > plan.EndDate ? plan.EndDate : to;

        var strategy = plan.Strategy?.ToLower() switch
        {
            "catchup" => PlanStrategy.CatchUp,
            "smart" => PlanStrategy.Smart,
            _ => PlanStrategy.Push
        };

        var unitOpt = units.HasValue ? FSharpOption<int>.Some(units.Value) : FSharpOption<int>.None;
        var minOpt = minutes.HasValue ? FSharpOption<int>.Some(minutes.Value) : FSharpOption<int>.None;

        var preview = Scheduler.materialize(strategy, plan.AllowedDaysMask, start, end, unitOpt, minOpt, plan.LookaheadDays);
        return Results.Ok(preview);
    }
}
