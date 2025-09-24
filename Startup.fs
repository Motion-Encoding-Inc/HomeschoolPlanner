#nowarn "0020"

open System
open System.Threading.Tasks
open System.Net
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.DependencyInjection
open Microsoft.FluentUI.AspNetCore.Components
open Microsoft.Extensions.Primitives
open ScholarsForge


let builder = WebApplication.CreateBuilder(Environment.GetCommandLineArgs())

builder.Services.AddControllersWithViews()
builder.Services.AddServerSideBlazor().Services.AddFunBlazorServer()

builder.Services.AddHttpClient();
builder.Services.AddFluentUIComponents();
//builder.Services.AddMudServices()


let app = builder.Build()

app.UseStaticFiles()

app.MapPost(
    "/contact/send",
    Func<HttpContext, Task>(fun ctx ->
        task {
            let! form = ctx.Request.ReadFormAsync()

            let getValue (key: string) =
                match form.TryGetValue key with
                | true, value when not (StringValues.IsNullOrEmpty value) -> value.ToString().Trim()
                | _ -> String.Empty

            let firstName = getValue "firstName"
            let lastName = getValue "lastName"
            let telephone = getValue "telephone"
            let title = getValue "title"
            let company = getValue "company"
            let email = getValue "email"
            let message = getValue "message"

            let requiredFields =
                [ ("firstName", "First name", firstName)
                  ("lastName", "Last name", lastName)
                  ("telephone", "Telephone", telephone)
                  ("email", "Email", email)
                  ("message", "Message", message) ]

            match requiredFields |> List.tryFind (fun (_, _, value) -> String.IsNullOrWhiteSpace value) with
            | Some (_, label, _) ->
                ctx.Response.StatusCode <- StatusCodes.Status400BadRequest
                do! ctx.Response.WriteAsync($"Missing required field: {label}.")
            | None ->
                let encodeSingle (value: string) = WebUtility.HtmlEncode value
                let encodeMultiline (value: string) =
                    value.Replace("\r", String.Empty).Split('\n')
                    |> Array.map WebUtility.HtmlEncode
                    |> String.concat "<br/>"

                let optionalRows =
                    [ ("Title", title); ("Company", company) ]
                    |> List.choose (fun (label, value) ->
                        if String.IsNullOrWhiteSpace value then
                            None
                        else
                            Some $"<p><strong>{label}:</strong> {WebUtility.HtmlEncode value}</p>")
                    |> String.concat String.Empty

                let subject = $"Contact form submission from {firstName} {lastName}"
                let body =
                    $"""<p>A new contact form submission was received.</p>
<p><strong>First name:</strong> {encodeSingle firstName}</p>
<p><strong>Last name:</strong> {encodeSingle lastName}</p>
<p><strong>Email:</strong> {encodeSingle email}</p>
<p><strong>Telephone:</strong> {encodeSingle telephone}</p>
{optionalRows}
<p><strong>Message:</strong><br/>{encodeMultiline message}</p>
"""

                try
                    do!
                        Graph.sendEmail
                            "support@scholarsforge.com"
                            [ "support@scholarsforge.com" ]
                            subject
                            body
                            true
                            [ email ]
                        |> Async.StartAsTask

                    ctx.Response.Redirect("/")
                with _ ->
                    ctx.Response.StatusCode <- StatusCodes.Status500InternalServerError
                    do! ctx.Response.WriteAsync("We were unable to send your message. Please try again later.")

            return ()
        }
        :> Task)
    )

app.MapFunBlazor(ScholarsForge.LandingPage.page,"/")
app.MapFunBlazor(ScholarsForge.AboutPage.page,"/about")
app.MapFunBlazor(ScholarsForge.ContactPage.page,"/contact")
app.MapFunBlazor(ScholarsForge.Index.page,"/app")
app.MapBlazorHub()
app.MapFallback(fun ctx ->
    ctx.Response.Redirect("/")
    Task.CompletedTask)

app.Run()
