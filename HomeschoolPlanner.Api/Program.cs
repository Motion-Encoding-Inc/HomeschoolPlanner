using HomeschoolPlanner.Api.Middleware;
using HomeschoolPlanner.Data;
//using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
//using Microsoft.OpenApi.Models;
using System;

var builder = WebApplication.CreateBuilder(args);

// 1) ProblemDetails
builder.Services.AddProblemDetails();

// 2) EF Core (Azure SQL)
builder.Services.AddDbContext<AppDbContext>(opts =>
    opts.UseSqlServer(builder.Configuration.GetConnectionString("Sql")));

//// 3) Auth (JWT placeholder — wire real keys later)
//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//    .AddJwtBearer();

// 4) Authorization
builder.Services.AddAuthorization();

//// 5) API versioning (future-proof)
//builder.Services.AddApiVersioning(opt =>
//{
//    opt.AssumeDefaultVersionWhenUnspecified = true;
//    opt.DefaultApiVersion = new ApiVersion(1, 0);
//}).AddApiExplorer();

//// 6) Swagger
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen(c =>
//{
//    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Homeschool Planner API", Version = "v1" });
//});

// 7) CORS (mobile + future web)
builder.Services.AddCors(opt =>
{
    opt.AddPolicy("default", p => p
        .AllowAnyHeader()
        .AllowAnyMethod()
        .WithOrigins(
            "http://localhost:5173",   // dev web
            "http://10.0.2.2",         // Android emulator to host
            "https://your-web-origin") // prod web
        .AllowCredentials());
});

var app = builder.Build();

// Pipeline
app.UseMiddleware<ProblemDetailsMiddleware>();
app.UseStatusCodePages(); // pairs well with ProblemDetails for non-exception 4xx/5xx

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("default");
app.UseAuthentication();
app.UseAuthorization();

// Health
app.MapGet("/healthz", () => Results.Json(new { status = "ok" }));

// === Endpoint groups (stubs) ===
var v1 = app.MapGroup("/api/v1");

//// Auth
//v1.MapPost("/auth/register", AuthEndpoints.Register);
//v1.MapPost("/auth/login", AuthEndpoints.Login);

//// Learners
//v1.MapGet("/learners", LearnerEndpoints.List);
//v1.MapPost("/learners", LearnerEndpoints.Create);
//v1.MapGet("/learners/{id:guid}", LearnerEndpoints.Get);
//v1.MapPut("/learners/{id:guid}", LearnerEndpoints.Update);
//v1.MapDelete("/learners/{id:guid}", LearnerEndpoints.Delete);

//// Subjects
//v1.MapGet("/subjects", SubjectEndpoints.List);
//v1.MapPost("/subjects", SubjectEndpoints.Create);

//// Resources (+units)
//v1.MapPost("/resources", ResourceEndpoints.Create);
//v1.MapPost("/resources/{id:guid}/units/bulk", ResourceEndpoints.BulkUnits);

//// Plans (+ schedule materialization)
//v1.MapPost("/plans", PlanEndpoints.Create);
//v1.MapGet("/plans/{id:guid}/schedule", PlanEndpoints.GetSchedule);

//// Task actions
//v1.MapPost("/tasks/{id:guid}/complete", TaskEndpoints.Complete);
//v1.MapPost("/tasks/{id:guid}/skip", TaskEndpoints.Skip);
//v1.MapPost("/tasks/{id:guid}/extra", TaskEndpoints.DoExtra);

//// Portfolio / attachments (SAS)
//v1.MapPost("/attachments/start", AttachmentEndpoints.RequestUpload);
//v1.MapPost("/attachments/confirm", AttachmentEndpoints.ConfirmUpload);
//v1.MapGet("/portfolio/{subjectId:guid}", PortfolioEndpoints.List);

//// Reports / ICS
//v1.MapGet("/reports/weekly", ReportEndpoints.WeeklyCsvOrPdf);
//v1.MapGet("/ics/{learnerPublicId}.ics", IcsEndpoints.GetCalendar);

app.Run();
