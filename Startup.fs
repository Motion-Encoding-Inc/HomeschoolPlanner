#nowarn "0020"

open System
open Microsoft.AspNetCore.Builder
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

app.UseDefaultFiles()
app.UseStaticFiles()
app.MapFallbackToFile("index.html");

app.MapBlazorHub()
app.MapFunBlazor(ScholarsForge.Index.page,"/app")


app.Run()
