using System.Net.Http.Json;
using HomeschoolPlanner.Data;
using Microsoft.AspNetCore.Mvc.Testing;

namespace HomeschoolPlanner.Api.Tests;

public class SubjectTests
{
    [Fact]
    public async Task Subject_Create_and_List_roundtrip()
    {
        Environment.SetEnvironmentVariable("DB_PROVIDER", "InMemory");
        await using var app = new WebApplicationFactory<Program>();
        var client = app.CreateClient();

        var learnerResp = await client.PostAsJsonAsync("/api/v1/learners",
            new { name = "Bob", grade = "5" });
        learnerResp.EnsureSuccessStatusCode();
        var learner = await learnerResp.Content.ReadFromJsonAsync<Learner>();

        var create = await client.PostAsJsonAsync("/api/v1/subjects",
            new { learnerId = learner!.Id, title = "Math" });
        create.EnsureSuccessStatusCode();

        var list = await client.GetFromJsonAsync<List<Subject>>($"/api/v1/subjects?learnerId={learner!.Id}");
        Assert.Contains(list!, s => s.Title == "Math");
    }
}
