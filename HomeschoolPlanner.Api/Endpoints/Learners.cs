using HomeschoolPlanner.Data;
using Microsoft.EntityFrameworkCore;

namespace HomeschoolPlanner.Api.Endpoints
{
    public static class LearnerEndpoints
    {
        public static async Task<IResult> List(AppDbContext db, HttpContext ctx)
        {
            var userId = GetUserId(ctx); // TODO: from JWT
            var learners = await db.Learners.Where(l => l.UserId == userId).ToListAsync();
            return Results.Ok(learners);
        }

        public static async Task<IResult> Create(AppDbContext db, HttpContext ctx, Learner dto)
        {
            var userId = GetUserId(ctx);
            dto.Id = Guid.NewGuid();
            dto.UserId = userId;
            db.Learners.Add(dto);
            await db.SaveChangesAsync();
            return Results.Created($"/api/v1/learners/{dto.Id}", dto);
        }

        public static async Task<IResult> Get(AppDbContext db, Guid id) =>
            await db.Learners.FindAsync(id) is { } l ? Results.Ok(l) : Results.NotFound();

        public static async Task<IResult> Update(AppDbContext db, Guid id, Learner input)
        {
            var e = await db.Learners.FindAsync(id);
            if (e is null) return Results.NotFound();
            e.Name = input.Name; e.Grade = input.Grade;
            await db.SaveChangesAsync();
            return Results.NoContent();
        }

        public static async Task<IResult> Delete(AppDbContext db, Guid id)
        {
            var e = await db.Learners.FindAsync(id);
            if (e is null) return Results.NotFound();
            db.Remove(e); await db.SaveChangesAsync();
            return Results.NoContent();
        }

        private static Guid GetUserId(HttpContext ctx)
            => Guid.Parse("11111111-1111-1111-1111-111111111111"); // TODO: extract from JWT for MVP
    }

}
