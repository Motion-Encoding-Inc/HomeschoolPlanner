using HomeschoolPlanner.Api.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails();

var app = builder.Build();

app.UseMiddleware<ProblemDetailsMiddleware>();

app.MapGet("/healthz", () => Results.Json(new { status = "ok" }));

app.MapGet("/error", () =>
{
    throw new InvalidOperationException("Test exception");
});

app.Run();
