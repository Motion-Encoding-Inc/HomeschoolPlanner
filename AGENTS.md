# Agent Instructions — Homeschool Planner

This repository supports **conversational memory** *and* a concrete .NET backend build/test loop. These instructions merge your standard AgentNotes flow with this project’s environment and task conventions.

---

## ⚡ Quick Task Checklist (TL;DR)

1. **New user task or reply ➜**

   1. Check `/AgentNotes` for a related note.
   2. If found → skim and **append** a *Follow‑up* section.
   3. If not found → **create** a new note `YYYY‑MM‑DDThh‑mm‑ssZ‑slug.md` with *Task* and *Initial considerations*.
2. **While working ➜**

   * Log each meaningful step (what & why) in the note.
   * Comment code thoroughly (what + why).
3. **Delivering ➜**

   * Summarize what changed and **why**.
   * State confidence (e.g., “\~90%”).
   * Ask concise clarifying questions.
4. **House‑keeping ➜**

   * If `> 50` notes **or** `/AgentNotes` > 300 kB → run an **aggregate memory** task (see §4). *Do not create an extra note for the aggregation task itself.*

---

## 1. Directory layout

```
/AgentNotes/
  Archived/
  AGGREGATE_YYYYMMDDThhmmssZ.md
  YYYY‑MM‑DDThh‑mm‑ssZ‑<short‑slug>.md

/HomeschoolPlanner.Api/        # C#: Minimal API, EF Core, middleware, endpoints, services
/HomeschoolPlanner.Domain/     # F#: shared domain types + scheduling logic
```

* Use **UTC ISO‑8601** timestamps in `/AgentNotes` so `ls` sorts chronologically.
* Keep raw notes ≤ \~5 kB (split if larger).

---

## 2. Task handling — step‑by‑step

### 2.1 Check for related notes

* Search `/AgentNotes` for keywords.
* If a relevant note exists, read it before coding.

### 2.2 Create or update a note

* **No match?**  Create:

  ```markdown
  ## Task
  <verbatim user request>

  ## Initial considerations
  * bullets: constraints, caveats, risks
  ```
* **Match found?**  Append:

  ```markdown
  ## Follow‑up (YYYY‑MM‑DD HH:mm UTC)
  * …what changed
  * …how you’re approaching it
  ```

### 2.3 During the task

* Log significant actions/decisions (concise).
* Keep code comments aligned with the note.

### 2.4 After code changes

* Append **Results** (what & why). Include confidence.

---

## 3. Responding to the user

* Short summary of work.
* Confidence emoji/percent.
* Clarifying questions if any.

---

## 4. House‑keeping / aggregation logic

| Parameter                           | Default | Rationale                                |
| ----------------------------------- | :-----: | ---------------------------------------- |
| `NOTE_COUNT_THRESHOLD`              |    50   | Too many files hurts retrieval & context |
| `NOTE_SIZE_THRESHOLD` (total bytes) |  300 kB | Safety valve for many tiny notes         |

When a threshold is exceeded:

1. **Do not** ask the user; create an internal **aggregate memory** task.
2. Phases:

   * **Stats**: count files & bytes; log.
   * **Archive**: create `/AgentNotes/Archived` and move raw notes + old `AGGREGATE_*.md` there.
   * **Synthesize**: read archived notes (+ previous aggregates), extract key lessons grouped by theme (subsystem, technology, domain). Write new `AGGREGATE_<UTC>.md` at `/AgentNotes` root.
   * **Evaluate**: self‑rate confidence (0–100%). If < 90%, footer: *“Please re‑analyse with o3‑pro.”*
3. After completion the `/AgentNotes` root must contain only the newest `AGGREGATE_*.md` and the `Archived/` folder.

**Bash sketch:**

```sh
cd AgentNotes
TS=$(date -u +%Y%m%dT%H%M%SZ)
mkdir -p Archived
for f in *.md; do [ "$f" != "$(ls -1t *.md | head -n1)" ] && mv "$f" Archived/ ; done
{
  printf '# AGGREGATED LESSONS\nGenerated: %s UTC\n\n' "$TS"
  printf '_(Synthesised from %s files)_\n\n' "$(ls Archived | wc -l)"
  printf '## Categories & Insights\n\n- Sample insight…\n'
} > "AGGREGATE_${TS}.md"
```

---

## 5. Environment — this project’s specifics

### 5.1 Prerequisites

* **.NET 9 SDK** → `dotnet --info`
* **EF Core CLI** → `dotnet tool install --global dotnet-ef`
* **Docker Desktop** (optional; for SQL + Azurite)

**Required NuGets (Api):**

```
Microsoft.EntityFrameworkCore.SqlServer
Microsoft.EntityFrameworkCore.Design
Microsoft.AspNetCore.Authentication.JwtBearer
Swashbuckle.AspNetCore
Microsoft.AspNetCore.Mvc.Versioning
Microsoft.AspNetCore.Mvc.Versioning.ApiExplorer
Azure.Storage.Blobs
```

### 5.2 Local configuration

`HomeschoolPlanner.Api/appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "Sql": "Server=(localdb)\\MSSQLLocalDB;Database=HomeschoolPlanner;Trusted_Connection=True;MultipleActiveResultSets=true"
  },
  "Storage": { "ConnectionString": "UseDevelopmentStorage=true", "Container": "attachments" },
  "Jwt": { "Issuer": "hsplanner", "Audience": "hsplanner.app", "Key": "local-dev-very-long-secret-change-in-prod" }
}
```

**Optional** `docker-compose.yml` (SQL + Azurite): see previous file or add as needed; then set

```
"Sql": "Server=localhost,14333;Initial Catalog=HomeschoolPlanner;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True;"
```

### 5.3 Build & run

```bash
dotnet restore
dotnet build HomeschoolPlanner.Api
dotnet run --project HomeschoolPlanner.Api
```

Open Swagger at `/swagger`.

### 5.4 Migrations

```bash
dotnet ef migrations add <Name> --project HomeschoolPlanner.Api --startup-project HomeschoolPlanner.Api
dotnet ef database update --project HomeschoolPlanner.Api --startup-project HomeschoolPlanner.Api
```

Rules: don’t edit existing migrations; add a new one per schema change.

### 5.5 Dev seed

Add `Data/DevSeed.cs` and call it in Development:

```csharp
if (app.Environment.IsDevelopment())
{
  using var scope = app.Services.CreateScope();
  var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
  await db.Database.MigrateAsync();
  await DevSeed.RunAsync(db); // idempotent
}
```

Seed: one user, one learner, 1–2 subjects, a book resource (10 units), a time resource.

---

## 6. Testing strategy (automatable)

### 6.1 F# unit tests (Domain)

```
dotnet new xunit -lang F# -n HomeschoolPlanner.Domain.Tests
dotnet add HomeschoolPlanner.Domain.Tests reference HomeschoolPlanner.Domain
```

Create `SchedulingTests.fs` (examples welcome); run `dotnet test`.

### 6.2 API integration tests (C#)

```
dotnet new xunit -n HomeschoolPlanner.Api.Tests
dotnet add HomeschoolPlanner.Api.Tests reference HomeschoolPlanner.Api
dotnet add HomeschoolPlanner.Api.Tests package Microsoft.AspNetCore.Mvc.Testing
# Prefer real SQL via Testcontainers; InMemory acceptable for smoke
```

Minimal test should: POST learner → GET learners → assert success.

### 6.3 Smoke tests (HTTP)

Check in `requests.http` (or a Postman collection) covering:

* `/healthz`
* Learner CRUD
* Subject create/list
* Resource create + `/units/bulk`
* Plan create + `/schedule`
* Task actions (complete/skip/extra)
* Attachments start/confirm
* Reports weekly CSV; ICS feed

Run via VS Code REST Client or `curl`.

---

## 7. CI (GitHub Actions)

`.github/workflows/ci.yml`:

```yaml
name: ci
on: [push, pull_request]
jobs:
  build:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-dotnet@v4
        with: { dotnet-version: '9.0.x' }
      - run: dotnet restore
      - run: dotnet build --no-restore -c Release
      - run: dotnet test --no-build -c Release
```

---

## 8. Task backlog conventions (for this repo)

Each ticket should list:

* **Goal** (one sentence)
* **Files to touch**
* **Routes** & sample payloads
* **DB impact** (migration? yes/no)
* **Tests** (unit/integration/smoke)
* **DoD** (precise success criteria)

Use the numbered tasks (T00…T12) from the planning note; uncomment routes in `Program.cs` as you implement them.

---

## 9. FAQ & pitfalls

* EF CLI not found → install `dotnet-ef` globally and restart terminal.
* Program.cs namespace errors → add missing NuGets listed in §5.1.
* “No DbContext found” → ensure API builds and `AddDbContext<AppDbContext>` is configured.
* InMemory EF hides SQL behaviours; prefer Testcontainers once endpoints stabilize.

---

## 10. Hand‑off to **GPT 5**

After creating an aggregate with confidence < 75%, ping the user to run GPT‑5 on `/AgentNotes/Archived` + the new aggregate; otherwise proceed.

---

*The context you write today saves you re‑debugging tomorrow.*
