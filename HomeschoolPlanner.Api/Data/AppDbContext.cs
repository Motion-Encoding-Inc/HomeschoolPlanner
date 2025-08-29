using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace HomeschoolPlanner.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> opts) : base(opts) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Learner> Learners => Set<Learner>();
    public DbSet<Subject> Subjects => Set<Subject>();
    public DbSet<Resource> Resources => Set<Resource>();
    public DbSet<ResourceUnit> ResourceUnits => Set<ResourceUnit>();
    public DbSet<Plan> Plans => Set<Plan>();
    public DbSet<TaskOccurrence> TaskOccurrences => Set<TaskOccurrence>();
    public DbSet<CompletionLog> CompletionLogs => Set<CompletionLog>();
    public DbSet<Attachment> Attachments => Set<Attachment>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<User>().HasIndex(x => x.Email).IsUnique();
        b.Entity<Learner>().HasIndex(x => new { x.UserId, x.Name });
        b.Entity<ResourceUnit>().HasIndex(x => new { x.ResourceId, x.Index }).IsUnique();

        b.Entity<Plan>()
            .Property(p => p.Strategy) // "push"|"catchup"|"smart"
            .HasMaxLength(16);

        b.Entity<TaskOccurrence>()
            .HasIndex(t => new { t.PlanId, t.Date });

        // Small enums/flags as ints
    }
}

// === Entities (trimmed to essentials for MVP) ===
public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; } = "";
    public byte[] PasswordHash { get; set; } = Array.Empty<byte>(); // placeholder
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
}

public class Subject
{
    public Guid Id { get; set; }
    public Guid LearnerId { get; set; }
    public string Title { get; set; } = "";
    public string? ColorHex { get; set; }
}

public enum ResourceType { Book = 1, Time = 2, Custom = 3 }

public class Resource
{
    public Guid Id { get; set; }
    public Guid SubjectId { get; set; }
    public ResourceType Type { get; set; }
    public string Title { get; set; } = "";
    public int? MinutesPerOccurrence { get; set; }   // for Time resources
    public int? MaxUnitsPerDay { get; set; }         // pacing guardrail
}

public class ResourceUnit
{
    public Guid Id { get; set; }
    public Guid ResourceId { get; set; }
    public int Index { get; set; }                   // 1..N (chapter/lesson/etc)
    public string Label { get; set; } = "";          // "Lesson 12"
}

public class Plan
{
    public Guid Id { get; set; }
    public Guid ResourceId { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public int AllowedDaysMask { get; set; }         // bitmask Mon..Sun
    public string Strategy { get; set; } = "push";   // push | catchup | smart(later)
    public int LookaheadDays { get; set; } = 7;
}

public class TaskOccurrence
{
    public Guid Id { get; set; }
    public Guid PlanId { get; set; }
    public DateOnly Date { get; set; }
    public int? UnitIndex { get; set; }              // for Book/Custom
    public int? MinutesPlanned { get; set; }         // for Time
    public bool Locked { get; set; }                 // if generated but fixed
}

public class CompletionLog
{
    public Guid Id { get; set; }
    public Guid TaskOccurrenceId { get; set; }
    public DateTime CompletedUtc { get; set; } = DateTime.UtcNow;
    public int? MinutesActual { get; set; }
    public string? Notes { get; set; }
}

public class Attachment
{
    public Guid Id { get; set; }
    public Guid SubjectId { get; set; }
    public string FileName { get; set; } = "";
    public string MimeType { get; set; } = "";
    public long SizeBytes { get; set; }
    public string BlobPath { get; set; } = "";       // container/key
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
}
