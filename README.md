# HomeschoolPlanner

Quick instructions to build, test, and run the demo API.

## Prerequisites

* [.NET 9 SDK](https://dotnet.microsoft.com/download)

## Build & Test

```bash
dotnet restore
dotnet build --no-restore
dotnet test --no-build
```

## Run the API (demo)

Uses the in-memory database for easy setup.

```bash
DB_PROVIDER=InMemory dotnet run --project HomeschoolPlanner.Api
```

The API listens on `http://localhost:5000`. Try the health check:

```bash
curl http://localhost:5000/healthz
```

Sample workflow:

1. POST a learner → `/api/v1/learners`
2. POST a subject → `/api/v1/subjects`
3. POST a resource and units → `/api/v1/resources` + `/units/bulk`
4. POST a plan → `/api/v1/plans`
5. GET the schedule preview → `/api/v1/plans/{id}/schedule?from=YYYY-MM-DD&days=7`

See `requests.http` for ready-made examples.
