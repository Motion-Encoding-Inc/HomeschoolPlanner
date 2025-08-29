using System.Net.Http.Json;
using HomeschoolPlanner.Data;
using Microsoft.AspNetCore.Mvc.Testing;

namespace HomeschoolPlanner.Api.Tests;

public class LearnerTests
{
    [Fact]
    public async Task Learner_Create_and_List_roundtrip()
    {
        Environment.SetEnvironmentVariable("DB_PROVIDER", "InMemory");
        await using var app = new WebApplicationFactory<Program>();
        var client = app.CreateClient();

        var create = await client.PostAsJsonAsync("/api/v1/learners",
            new { name = "Alice", grade = "4" });
        create.EnsureSuccessStatusCode();

        var list = await client.GetFromJsonAsync<List<Learner>>("/api/v1/learners");
        Assert.Contains(list!, l => l.Name == "Alice");
    }
}
