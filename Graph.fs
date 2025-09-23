namespace ScholarsForge

[<RequireQualifiedAccess>]
module Graph =

    open System
    open System.Net.Http
    open System.Net.Http.Headers
    open System.Text
    open System.Text.Json
    open System.Text.Json.Serialization
    open Microsoft.Identity.Client

    // minimal DTOs matching Graph's sendMail payload
    type EmailAddress = { address: string }
    type Recipient = { emailAddress: EmailAddress }
    type ItemBody = { contentType: string; content: string }
    type Message = { subject: string; body: ItemBody; toRecipients: Recipient[]; replyTo: Recipient[] }
    type SendMailRequest = { message: Message; saveToSentItems: bool }

    let private tenantId = System.Environment.GetEnvironmentVariable("GRAPH_TENANTID")
    let private clientId = System.Environment.GetEnvironmentVariable("GRAPH_CLIENTID")
    let private clientSecret = System.Environment.GetEnvironmentVariable("GRAPH_CLIENTSECRET")


    /// Minimal app-credentials Graph sender.
    /// Requirements:
    ///  - App Registration has Application permission: Mail.Send
    ///  - Admin consent granted
    ///  - 'sender' is a user/shared mailbox in the tenant (UPN or GUID or id)
    /// replyTo cat be a empty list or a list of recipients that should be in the To field of the reply email, this is how you can
    /// fake out the from address to be different than the sender address
    let sendEmail
        (sender: string)        // e.g. "noreply@yourtenant.com"
        (toRecipients: string list)
        (subject: string)
        (body: string)
        (isHtml: bool)
        (replyTo: string list)
        : Async<unit> =
        async {
            // 1) acquire app token
            let authority = $"https://login.microsoftonline.com/{tenantId}"
            let app =
                ConfidentialClientApplicationBuilder
                    .Create(clientId)
                    .WithClientSecret(clientSecret)
                    .WithAuthority(authority)
                    .Build()

            let scopes = [| "https://graph.microsoft.com/.default" |]
            let! tokenResult = app.AcquireTokenForClient(scopes).ExecuteAsync() |> Async.AwaitTask

            // 2) build payload
            let recipients =
                toRecipients
                |> List.map (fun addr -> { emailAddress = { address = addr } })
                |> List.toArray

            let replyToRecipients =
                replyTo
                |> List.map (fun addr -> { emailAddress = { address = addr } })
                |> List.toArray

            let message : Message =
                {
                    subject = subject
                    body = { contentType = (if isHtml then "HTML" else "Text"); content = body }
                    toRecipients = recipients
                    replyTo = replyToRecipients
                }

            let payload : SendMailRequest = { message = message; saveToSentItems = true }

            let jsonOptions =
                JsonSerializerOptions(PropertyNamingPolicy = null, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)

            let json = JsonSerializer.Serialize(payload, jsonOptions)
            use content = new StringContent(json, Encoding.UTF8, "application/json")

            // 3) call Graph sendMail as the specific user/mailbox
            let url = $"https://graph.microsoft.com/v1.0/users/{Uri.EscapeDataString(sender)}/sendMail"
            use http = new HttpClient()
            http.DefaultRequestHeaders.Authorization <- AuthenticationHeaderValue("Bearer", tokenResult.AccessToken)
            http.DefaultRequestHeaders.Accept.Add(MediaTypeWithQualityHeaderValue("application/json"))

            let! resp = http.PostAsync(url, content) |> Async.AwaitTask
            if not resp.IsSuccessStatusCode then
                let! err = resp.Content.ReadAsStringAsync() |> Async.AwaitTask
                failwithf "Graph sendMail failed (%O): %s" resp.StatusCode err
            // on success Graph returns 202 Accepted with empty body
        }

    // example usage - replace with your own addresses
    //sendEmailAppAsync "support@scholarsforge.com" ["patrick@scholarsforge.com";"cheryl@scholarsforge.com";"allen@scholarsforge.com"] "Test from F# script 2" "<b>hello</b> from F# script 2 reply to psimpson@cannellamedia.com" true (ValueSome ["psimpson@cannellamedia.com"])
    //|> Async.RunSynchronously

