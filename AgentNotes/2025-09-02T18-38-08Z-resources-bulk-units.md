## Task
Resources: entity + Create + Bulk Units
- Why: Subjects need assignables (book/time/custom).
- Add:
   - Entities/Resource.cs (Id, SubjectId, Type enum Book|Time|Custom, Title, MinutesPerOccurrence?, MaxUnitsPerDay?)
   - Entities/ResourceUnit.cs (Id, ResourceId, Index:int, Label) with unique index (ResourceId, Index).
- DbContext: DbSet<Resource>, DbSet<ResourceUnit>.
- Endpoints:
   - POST /api/v1/resources → create.
   - POST /api/v1/resources/{id}/units/bulk → accept labels: string[], create sequential units starting at 1.
- DoD: Create a resource and bulk in e.g. 10 units; list via a simple GET on resource (optional) or check DB via subsequent schedule call.

## Initial considerations
* Need to define Resource and ResourceUnit entities with relationships and type enum.
* Need DbContext entries and configuration, including unique index on (ResourceId, Index).
* Implement endpoints for creating resources and bulk adding units.
* Need to handle validation: subject must exist? maybe using foreign keys.
* Ensure sequential index starts at 1 for units; when bulk insertion, index increments from existing count + 1? The spec says starting at 1 when creating sequential units - but should always start at 1; maybe just create new units with indexes starting at 1 ignoring existing units? But to support multiple bulk calls we should continue from existing count + 1, but spec may not require. Possibly next call should continue sequential; we will implement to start after existing units (so from existing count + 1).
* Need to add migrations for new entities.
* Implement tests: integration tests for creating resource and bulk units? DoD indicates manual check; but repo instructions emphasise `dotnet test`. We'll add tests verifying resource creation and bulk units maybe.
* Must run dotnet restore/build/test, create EF migration.

## Work log
* Created Resource and ResourceUnit entity files and removed inline definitions from AppDbContext.
* Added ResourceEndpoints for create and bulk unit operations; uncommented routes in Program.cs.
* Updated requests.http with resource examples.
* Added integration test covering resource creation and bulk units.
* Installed .NET SDK and dotnet-ef; ran restore/build/test and database update.

## Results
* Resource creation and bulk unit endpoints work; tests pass.
* Confidence: ~0.86.
