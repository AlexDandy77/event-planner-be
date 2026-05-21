# Event Planner Backend

ASP.NET Core Web API backend for the Event Planner internship project.

The backend provides event CRUD, filtering, sorting, JWT authentication, validation, centralized error handling, PostgreSQL persistence, Swagger UI, unit tests, integration tests, and Docker-based local deployment support.

## Tech Stack

- .NET 10
- ASP.NET Core Web API
- Entity Framework Core
- PostgreSQL with Npgsql
- JWT bearer authentication
- BCrypt password hashing
- Swashbuckle Swagger UI
- xUnit integration and unit tests
- Docker Compose for local PostgreSQL/API runtime

## Project Structure

```txt
event-planner-be/
  EventPlanner.slnx
  api-contract.md
  be-exec-plan.md
  deployment.md
  docker-compose.yml
  Dockerfile
  src/
    EventPlanner.Api/
    EventPlanner.Application/
    EventPlanner.Domain/
    EventPlanner.Infrastructure/
  tests/
    EventPlanner.UnitTests/
    EventPlanner.IntegrationTests/
```

## Architecture

The backend uses a Clean Architecture inspired structure.

`EventPlanner.Api`

- Starts the web server.
- Owns controllers, routing, CORS, authentication setup, Swagger, middleware, and HTTP response behavior.
- Should stay thin: it receives HTTP requests and delegates work to Application services.

`EventPlanner.Application`

- Owns use cases, DTOs, service interfaces, validators, query models, and application exceptions.
- Depends on Domain.
- Does not know about controllers, EF Core, PostgreSQL, or JWT implementation details.

`EventPlanner.Domain`

- Owns core entities, enums, and domain rules.
- Contains `CalendarEvent`, `User`, `EventCategory`, and `UserRole`.
- Has no dependency on ASP.NET Core, EF Core, or Infrastructure.

`EventPlanner.Infrastructure`

- Owns technical implementations.
- Contains EF Core `DbContext`, entity configuration, migrations, repositories, seed data, JWT token creation, password hashing, and clock implementation.
- Implements interfaces defined by Application.

## API Overview

Base path:

```txt
/api
```

Main endpoints:

```txt
GET    /api/health

POST   /api/auth/register
POST   /api/auth/login
GET    /api/auth/me

GET    /api/events
GET    /api/events/{id}
POST   /api/events
PUT    /api/events/{id}
DELETE /api/events/{id}
```

Event reads are public. Event create, update, and delete require a JWT bearer token.

Swagger UI is available in development:

```txt
http://localhost:5000/swagger
```

The full HTTP contract lives in [api-contract.md](api-contract.md).

## Prerequisites

- .NET 10 SDK
- Docker Desktop or another Docker runtime
- PostgreSQL can be run through Docker Compose

Restore local .NET tools:

```bash
dotnet tool restore
```

## Local Development Setup

1. Restore dependencies:

```bash
dotnet restore EventPlanner.slnx
```

2. Set the local JWT secret in .NET user secrets:

```bash
dotnet user-secrets set "Jwt:Secret" "development-only-jwt-secret-change-before-production-1234567890" --project src/EventPlanner.Api/EventPlanner.Api.csproj
```

3. Start PostgreSQL:

```bash
docker compose up -d postgres
```

4. Apply EF Core migrations:

```bash
dotnet tool run dotnet-ef database update --project src/EventPlanner.Infrastructure --startup-project src/EventPlanner.Api
```

5. Run the API:

```bash
dotnet run --project src/EventPlanner.Api/EventPlanner.Api.csproj
```

Default local URLs:

```txt
http://localhost:5000
https://localhost:5001
```

Health check:

```txt
GET http://localhost:5000/api/health
```

## Configuration

Configuration is split by responsibility:

- `appsettings.json` contains shared non-secret defaults.
- `appsettings.Development.json` contains local development defaults.
- `appsettings.Production.json` contains production logging defaults.
- .NET user secrets contain local development secrets.
- Environment variables contain Docker, CI, and cloud secrets.

Important configuration keys:

```txt
ConnectionStrings:DefaultConnection
Jwt:Issuer
Jwt:Audience
Jwt:AccessTokenMinutes
Jwt:Secret
Cors:AllowedOrigins
```

Environment variable form uses double underscores:

```txt
ConnectionStrings__DefaultConnection
Jwt__Secret
Cors__AllowedOrigins__0
```

Never commit real secrets or a real `.env` file.

## Database

The application uses PostgreSQL and EF Core migrations.

Main persistence files:

```txt
src/EventPlanner.Infrastructure/Persistence/EventPlannerDbContext.cs
src/EventPlanner.Infrastructure/Persistence/Configurations/
src/EventPlanner.Infrastructure/Persistence/Migrations/
src/EventPlanner.Infrastructure/Persistence/Repositories/
src/EventPlanner.Infrastructure/Persistence/Seed/DevelopmentSeedData.cs
```

Development seed users:

```txt
organizer@example.com
admin@example.com
mara.community@example.com
victor.design@example.com
nina.operations@example.com
```

All seeded users use this development password:

```txt
Password123!
```

## Docker Compose

Create a local `.env` from `.env.example` and replace `JWT_SECRET` with a long random value:

```bash
cp .env.example .env
```

Start PostgreSQL and the API:

```bash
docker compose up --build
```

Container URLs:

```txt
API:        http://localhost:5080
Swagger:    http://localhost:5080/swagger
Health:     http://localhost:5080/api/health
PostgreSQL: localhost:5432
```

For a fresh database volume, apply migrations before using event endpoints:

```bash
dotnet tool run dotnet-ef database update --project src/EventPlanner.Infrastructure --startup-project src/EventPlanner.Api
```

More deployment details are in [deployment.md](deployment.md).

## Testing

Run all tests:

```bash
dotnet test EventPlanner.slnx
```

Run with the same stricter command used during implementation checks:

```bash
dotnet test EventPlanner.slnx --disable-build-servers
```

Current test coverage includes:

- Domain rules for calendar events.
- Application service behavior.
- Request validators.
- Query filtering and sorting.
- EF repository behavior.
- Auth endpoint integration tests.
- Event endpoint integration tests.

## Build And Format

Build:

```bash
dotnet build EventPlanner.slnx
```

Format:

```bash
dotnet format EventPlanner.slnx
```

## Helpful Files

- [api-contract.md](api-contract.md): concrete frontend/backend HTTP contract.
- [be-exec-plan.md](be-exec-plan.md): backend execution roadmap and completed task list.
- [deployment.md](deployment.md): user secrets, Docker, and Azure deployment notes.
- [docker-compose.yml](docker-compose.yml): local PostgreSQL and API services.
- [Dockerfile](Dockerfile): API container image build.

## Troubleshooting

`JWT secret is not configured.`

Set the local user secret:

```bash
dotnet user-secrets set "Jwt:Secret" "development-only-jwt-secret-change-before-production-1234567890" --project src/EventPlanner.Api/EventPlanner.Api.csproj
```

Database connection fails.

Start PostgreSQL and confirm it is healthy:

```bash
docker compose up -d postgres
docker compose ps
```

Database tables are missing.

Run migrations:

```bash
dotnet tool run dotnet-ef database update --project src/EventPlanner.Infrastructure --startup-project src/EventPlanner.Api
```
