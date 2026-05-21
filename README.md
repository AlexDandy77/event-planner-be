# Event Planner Backend

ASP.NET Core Web API backend for the Event Planner application.

## Architecture

```txt
src/
  EventPlanner.Api/
  EventPlanner.Application/
  EventPlanner.Domain/
  EventPlanner.Infrastructure/
tests/
  EventPlanner.UnitTests/
  EventPlanner.IntegrationTests/
```

## Project Roles

`EventPlanner.Api`

- Starts the web server.
- Owns controllers, HTTP routing, CORS, OpenAPI, middleware, and authentication setup.
- Should stay thin: it receives HTTP requests and delegates real work to Application services.

`EventPlanner.Application`

- Owns use cases, DTOs, service interfaces, validators, and app-level business workflow.
- Depends on Domain.
- Should not know about EF Core, databases, controllers, or JWT implementation details.

`EventPlanner.Domain`

- Owns core business entities, enums, value objects, and rules.
- Has no dependency on the other backend projects.
- This is the most stable layer.

`EventPlanner.Infrastructure`

- Owns technical implementations such as EF Core DbContext, repositories, password hashing, JWT token creation, and external services.
- Depends on Application and Domain.

## Useful Commands

```bash
dotnet restore
dotnet build
dotnet run --project src/EventPlanner.Api/EventPlanner.Api.csproj
dotnet test
```

The API runs locally on:

```txt
http://localhost:5000
https://localhost:5001
```

Initial health endpoint:

```txt
GET /api/health
```
