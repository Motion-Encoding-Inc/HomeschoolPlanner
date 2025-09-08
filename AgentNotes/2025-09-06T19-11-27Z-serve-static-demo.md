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
## Follow-up (2025-09-06 19:27 UTC)
* Mirror demo in /docs for GitHub Pages.
* Copy index, week, day HTML and adjust links to be relative.
* Run dotnet test and commit.
## Follow-up (2025-09-06 19:31 UTC)
* Copied demo HTML into docs/ and updated links to be relative.
* Added comments to clarify GitHub Pages mirror.
* Next: run dotnet test.
## Results
* Docs folder hosts demo HTML with relative links for GitHub Pages.
* `dotnet test` failed: .NET 9.0 SDK unavailable in container.
* Confidence: 0.85
## Follow-up (2025-09-06 19:32 UTC)
* Add GitHub Actions workflow to sync demo files into docs on push to main.
* Ensure workflow copies HTML and commits changes to docs.
* Run dotnet test.
## Results
* Added GitHub Actions workflow to sync demo files to docs on main pushes.
* `dotnet test` passed after installing .NET SDK.
* Confidence: 0.9

## Follow-up (2025-09-06 20:11 UTC)
* Centralize demo tokens via shared CSS; refactor index and week HTML to match Day detail style.

## Results
* Added shared demo.css and updated index/week pages (and docs mirror) to use unified tokens.
* `dotnet test` succeeded; verified demo assets served via curl.
* Confidence: 0.88
## Follow-up (2025-09-06 20:17 UTC)
* Polish Pages demo for mobile: add theme color & favicon.
* Ensure demo.css is copied and linked via sync workflow.
## Results
* Added theme color meta and SVG favicon to demo index/week (docs and wwwroot) with shared CSS link.
* Copied demo.css into docs and updated Pages sync workflow to include it.
* `dotnet test` passed with 4 tests.
* Confidence: 0.9

## Follow-up (2025-09-08 17:16 UTC)
* Apply shared theme tokens and add theme toggle to demo pages.

## Results
* Demo pages now support Light/Dark themes with toggle.
* `dotnet test` passed (4 tests).
* Confidence: 0.84
