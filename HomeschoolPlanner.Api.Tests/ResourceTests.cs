using System.Linq;
using System.Net.Http.Json;
using HomeschoolPlanner.Data;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace HomeschoolPlanner.Api.Tests;

public class ResourceTests
{
    [Fact]
    public async Task Resource_Create_and_BulkUnits_roundtrip()
    {
        Environment.SetEnvironmentVariable("DB_PROVIDER", "InMemory");
        await using var app = new WebApplicationFactory<Program>();
        var client = app.CreateClient();

        var learnerResp = await client.PostAsJsonAsync("/api/v1/learners", new { name = "Eve", grade = "6" });
        learnerResp.EnsureSuccessStatusCode();
        var learner = await learnerResp.Content.ReadFromJsonAsync<Learner>();

        var subjectResp = await client.PostAsJsonAsync("/api/v1/subjects", new { learnerId = learner!.Id, title = "Science" });
        subjectResp.EnsureSuccessStatusCode();
        var subject = await subjectResp.Content.ReadFromJsonAsync<Subject>();

        var resourceResp = await client.PostAsJsonAsync("/api/v1/resources", new { subjectId = subject!.Id, type = ResourceType.Book, title = "Biology" });
        resourceResp.EnsureSuccessStatusCode();
        var resource = await resourceResp.Content.ReadFromJsonAsync<Resource>();

        var labels = Enumerable.Range(1, 10).Select(i => $"Lesson {i}").ToArray();
        var bulkResp = await client.PostAsJsonAsync($"/api/v1/resources/{resource!.Id}/units/bulk", labels);
        bulkResp.EnsureSuccessStatusCode();

        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var count = await db.ResourceUnits.CountAsync(u => u.ResourceId == resource!.Id);
        Assert.Equal(10, count);
    }
}
