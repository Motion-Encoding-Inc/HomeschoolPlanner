## Task
Plans: create + schedule preview (calls F#)

## Initial considerations
* Need Plan entity file under Entities.
* Ensure DbContext exposes DbSet<Plan>; may already exist.
* Implement POST /api/v1/plans with validation and resource existence check.
* Implement GET /api/v1/plans/{id}/schedule to call F# scheduler and return preview.
* Update Program.cs mappings.
* Add integration test for plan creation and schedule preview.
* Confirm schema/migration; Plan table may already exist.
* Run dotnet test.

## Follow-up (2025-09-02 18:55 UTC)
* Added Plan entity file and removed inline definition.
* Implemented PlanEndpoints for creation and schedule preview using F# scheduler.
* Added integration test verifying plan schedule returns empty preview.
* Installed .NET SDK via script to run tests.

## Results
* `dotnet test` passes for all tests including new PlanTests.
* Confidence: ~0.86
## Follow-up (2025-09-02 19:30 UTC)
* Added validation for `days` parameter in schedule endpoint per review.
* Wrote README instructions on building, testing, and running the demo.
## Results (2025-09-02 19:32 UTC)
* `dotnet test` still passes after days validation and README update.
* Confidence: ~0.92
