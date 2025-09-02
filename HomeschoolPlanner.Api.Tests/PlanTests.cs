using System.Net.Http.Json;
using HomeschoolPlanner.Data;
using HomeschoolPlanner.Domain;
using Microsoft.AspNetCore.Mvc.Testing;

namespace HomeschoolPlanner.Api.Tests;

public class PlanTests
{
    [Fact]
    public async Task Plan_Create_and_GetSchedule_returns_preview()
    {
        Environment.SetEnvironmentVariable("DB_PROVIDER", "InMemory");
        await using var app = new WebApplicationFactory<Program>();
        var client = app.CreateClient();

        // Arrange learner -> subject -> resource with units
        var learnerResp = await client.PostAsJsonAsync("/api/v1/learners", new { name = "Amy", grade = "4" });
        learnerResp.EnsureSuccessStatusCode();
        var learner = await learnerResp.Content.ReadFromJsonAsync<Learner>();

        var subjectResp = await client.PostAsJsonAsync("/api/v1/subjects", new { learnerId = learner!.Id, title = "Math" });
        subjectResp.EnsureSuccessStatusCode();
        var subject = await subjectResp.Content.ReadFromJsonAsync<Subject>();

        var resourceResp = await client.PostAsJsonAsync("/api/v1/resources", new { subjectId = subject!.Id, type = HomeschoolPlanner.Data.ResourceType.Book, title = "Algebra" });
        resourceResp.EnsureSuccessStatusCode();
        var resource = await resourceResp.Content.ReadFromJsonAsync<Resource>();

        var labels = new[] { "Ch1", "Ch2" };
        var unitsResp = await client.PostAsJsonAsync($"/api/v1/resources/{resource!.Id}/units/bulk", labels);
        unitsResp.EnsureSuccessStatusCode();

        // Create plan
        var start = new DateOnly(2025, 1, 1);
        var end = new DateOnly(2025, 1, 31);
        var planResp = await client.PostAsJsonAsync("/api/v1/plans", new {
            resourceId = resource.Id,
            startDate = start,
            endDate = end,
            allowedDaysMask = 0b0111110,
            strategy = "push"
        });
        planResp.EnsureSuccessStatusCode();
        var plan = await planResp.Content.ReadFromJsonAsync<Plan>();

        // Act schedule
        var schedResp = await client.GetAsync($"/api/v1/plans/{plan!.Id}/schedule?from=2025-01-01&days=7");
        schedResp.EnsureSuccessStatusCode();
        var preview = await schedResp.Content.ReadFromJsonAsync<SchedulePreview>();

        Assert.NotNull(preview);
        Assert.Empty(preview!.Items);
    }
}
