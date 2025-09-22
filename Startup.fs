#nowarn "0020"

open System
open System.Threading.Tasks
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.DependencyInjection
open Microsoft.FluentUI.AspNetCore.Components


let builder = WebApplication.CreateBuilder(Environment.GetCommandLineArgs())

builder.Services.AddControllersWithViews()
builder.Services.AddServerSideBlazor().Services.AddFunBlazorServer()

builder.Services.AddHttpClient();
builder.Services.AddFluentUIComponents();
//builder.Services.AddMudServices()


let app = builder.Build()

app.UseStaticFiles()

app.MapFunBlazor(ScholarsForge.LandingPage.page,"/")
app.MapFunBlazor(ScholarsForge.AboutPage.page,"/about")
app.MapFunBlazor(ScholarsForge.ContactPage.page,"/contact")
app.MapFunBlazor(ScholarsForge.Index.page,"/app")
app.MapBlazorHub()
app.MapFallback(fun ctx ->
    ctx.Response.Redirect("/")
    Task.CompletedTask)

app.Run()
