## Task
Goal: Serve files under wwwroot/demo and add a friendly /demo redirect.
Steps
1. Open Program.cs and, near the top-level middleware (after UseStatusCodePages() and before your endpoint maps), add:
```
// Serve static files from wwwroot/
app.UseDefaultFiles();   // enables index.html discovery
app.UseStaticFiles();

// Convenience redirect: /demo -> /demo/index.html
app.MapGet("/demo", () => Results.Redirect("/demo/index.html"));
```
2. Build & run:
```
dotnet run --project .\HomeschoolPlanner.Api
```
3. In your browser, open:
- `http://localhost:<printed-port>/demo`
- `http://localhost:<printed-port>/demo/week.html`

DoD
- Hitting `/demo` loads your demo landing page.
- `/demo/week.html` renders the week preview.

week.html should reflect the Week Preview wireframe (day cards, chips, CTA), and day.html matches the Day Detail layout (timeline + sticky actions).

## Initial considerations
* Add static file middleware and redirect in Program.cs with comments.
* Create `wwwroot/demo` with `index.html`, `week.html`, and `day.html` matching wireframes.
* Ensure code comments explain what and why.
* Run `dotnet test` and verify static file endpoints with `dotnet run` and curl.

## Follow-up (2025-09-06 19:15 UTC)
* Created `wwwroot/demo` with landing, week preview, and day detail HTML files.
* Added static file middleware and /demo redirect to Program.cs.

## Follow-up (2025-09-06 19:20 UTC)
* Executed `dotnet test` – all tests passed.
* Launched API and confirmed `/demo` redirect and `/demo/week.html` load expected pages via curl.

## Results
* Static demo files are served via new middleware and /demo redirect.
* Verified pages load and all tests pass.
* Confidence: 0.9
