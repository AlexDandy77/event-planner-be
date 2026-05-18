# Event Planner Backend Execution Plan

## Goal

Design and implement a maintainable .NET backend for the Event Planner app. The backend should support event CRUD, filtering, sorting, authentication, validation, error handling, tests, and optional Azure deployment.

The implementation should follow OOP and SOLID principles without adding unnecessary complexity. The project is small enough to stay pragmatic, but the boundaries should be clear from the beginning.

## Current State

- This file is the single backend planning source of truth.
- `../event-planner-fe/fe-exec-plan.md` expects backend integration in this order:
  - [Completed] Week 1: API contract alignment.
  - Week 2: `GET /events` with static or seeded data.
  - Week 3: full event CRUD.
  - Week 4: filtering and sorting.
  - Week 5: authentication endpoints.
- `event-planner-be` has been initialized as an ASP.NET Core Web API with layered projects.

## Recommended Architecture [Completed]

Use a Clean Architecture inspired structure:

```txt
event-planner-be/
  EventPlanner.slnx
  src/
    EventPlanner.Api/
    EventPlanner.Application/
    EventPlanner.Domain/
    EventPlanner.Infrastructure/
  tests/
    EventPlanner.UnitTests/
    EventPlanner.IntegrationTests/
```

### Layer Responsibilities

`EventPlanner.Api`

- Owns HTTP concerns.
- Contains controllers, middleware, dependency injection setup, CORS, authentication configuration, Swagger, and API response handling.
- Depends on `Application`, `Infrastructure`, and `Domain`.

`EventPlanner.Application`

- Owns use cases and business workflows.
- Contains service interfaces, request/response DTOs, validators, and query models.
- Depends on `Domain`.
- Defines interfaces such as `IEventRepository`, `IUserRepository`, `IAuthTokenService`, and `ICurrentUserService`.

`EventPlanner.Domain`

- Owns core business models and rules.
- Contains entities, enums, value objects, and domain exceptions.
- Does not depend on ASP.NET Core, EF Core, JWT, or external services.

`EventPlanner.Infrastructure`

- Owns technical implementations.
- Contains EF Core `DbContext`, repositories, migrations, password hashing, JWT token creation, and clock implementation.
- Depends on `Application` and `Domain`.

## SOLID Guidelines

### Single Responsibility Principle

- Controllers only handle HTTP input/output.
- Application services handle use cases.
- Repositories handle persistence.
- Validators handle request validation.
- Token services handle JWT creation.
- Middleware handles cross-cutting error responses.

### Open/Closed Principle

- Add new filters or sorting options through query objects and extension methods instead of rewriting controllers.
- Add new event rules in domain/application services without changing persistence code.

### Liskov Substitution Principle

- Keep repository and service interfaces behaviorally consistent.
- Avoid fake implementations that return different shapes or ignore cancellation tokens.

### Interface Segregation Principle

- Prefer small interfaces:
  - `IEventRepository`
  - `IUserRepository`
  - `IPasswordHasher`
  - `IAuthTokenService`
  - `ICurrentUserService`
  - `IClock`
- Avoid one large service interface that handles events, users, auth, and validation.

### Dependency Inversion Principle

- Application layer depends on abstractions.
- Infrastructure provides concrete implementations.
- API wires dependencies through DI.

## Contract Boundary

Detailed endpoint paths, request DTOs, response DTOs, validation rules, and error response shapes live in `api-contract.md`.

Execution rule:

- `be-exec-plan.md` tracks what to build and in what order.
- `api-contract.md` defines the concrete HTTP contract.
- Backend controllers must use DTOs from the Application layer, not EF entities.
- Domain/EF entities are internal backend models and are not returned directly from controllers.

## Persistence Plan

Use EF Core.

Local database:

- PostgreSQL, using the Npgsql EF Core provider.

Keep the provider isolated in Infrastructure so it can be swapped later.

Main persistence files:

```txt
Persistence/EventPlannerDbContext
Persistence/Configurations/CalendarEventConfiguration
Persistence/Configurations/UserConfiguration
Repositories/EventRepository
Repositories/UserRepository
```

EF Core rules:

- Configure unique index on `User.Email`.
- Configure required fields and max lengths.
- Store enums as strings for readable database values.
- Use UTC `DateTimeOffset`.
- Add migrations when models change.
- Query soft-deleted events out by default.

## Validation Plan

Use FluentValidation if allowed by the internship setup. If not, use manual validators in the Application layer.

Concrete validation rules are part of the HTTP contract and are documented in `api-contract.md`.

## Error Handling Plan

Use centralized middleware.

Error mapping:

```txt
ValidationException -> 400
UnauthorizedAccessException -> 401
Forbidden access -> 403
NotFoundException -> 404
ConflictException -> 409
Unhandled exception -> 500
```

Concrete error response JSON is documented in `api-contract.md`.

## Authentication And Authorization Plan

Use JWT bearer authentication.

Implementation pieces:

```txt
JwtOptions
AuthController
AuthService
IAuthTokenService
JwtTokenService
IPasswordHasher
PasswordHasher
ICurrentUserService
CurrentUserService
```

Security rules:

- Store password hashes only.
- Never return `PasswordHash`.
- Keep JWT secret in configuration or user secrets, not in source code.
- Require authentication for event CRUD after auth is added.
- Use role-based authorization later for admin-only features.
- Concrete auth endpoint request/response schemas live in `api-contract.md`.

## Async Plan

Use async database and service methods everywhere:

```txt
Task<IReadOnlyList<TResponse>> GetManyAsync(...)
Task<TResponse> GetByIdAsync(...)
Task<TResponse> CreateAsync(...)
Task<TResponse> UpdateAsync(...)
Task DeleteEventAsync(...)
```

Always pass `CancellationToken` from controllers to services and repositories.

## Testing Plan

### Unit Tests

Focus on Application and Domain behavior:

- Event creation validation.
- Event update validation.
- Start/end date rule.
- Filtering query construction.
- Sorting option parsing.
- Auth password verification flow.

### Integration Tests

Use `WebApplicationFactory` for API tests.

Test:

- Event list endpoint returns seeded events.
- Event create endpoint creates an event.
- Event update endpoint updates an event.
- Event delete endpoint soft deletes an event.
- Filtering by date works.
- Sorting works.
- Register/login/me flow works.
- Validation errors return stable response shape.

## Execution Roadmap

## Week 1 - API Contract Alignment [Completed]

Focus:

- Define the shared frontend/backend API contract before implementation.
- Agree on resource shapes, endpoint paths, status codes, and error shape.
- Avoid exposing database entities directly through HTTP.

Tasks:

- Define Event and User/Auth resource shapes.
- Define request/response DTO names.
- Define event CRUD endpoints.
- Define future filtering and sorting query parameters.
- Define future auth endpoints.

Output:

- `api-contract.md` contains the concrete API contract.
- Event resource shape is agreed.
- User/auth resource shape is agreed.
- Error response style is agreed.

Acceptance criteria:

- Frontend can create TypeScript API types from the contract.
- Backend can create C# DTOs and controllers from the contract.
- Event CRUD and future auth endpoints are documented.

## Week 2 - ASP.NET Core Web API, DI, And Foundation [Completed]

### Task 1 - Initialize ASP.NET Core Web API [Completed]

Actions:

- Create the solution structure.
- Add API, Application, Domain, Infrastructure, and test projects.
- Configure project references.
- Configure controllers.
- Add OpenAPI.
- Add CORS for the frontend dev server.
- Add `GET /api/health`.

Output:

- Backend starts as an ASP.NET Core Web API.
- Health endpoint works.
- OpenAPI is available in development.
- Project structure supports SOLID layering.

Acceptance criteria:

- `dotnet build` succeeds.
- `dotnet test` succeeds.
- API starts with `dotnet run --project src/EventPlanner.Api/EventPlanner.Api.csproj`.
- `GET /api/health` returns a successful response.

### Task 2 - Add Domain Model Skeletons [Completed]

Actions:

- Add `CalendarEvent`.
- Add `User`.
- Add `EventCategory`.
- Add `UserRole`.
- Add basic domain validation helpers where useful.

Output:

- Domain layer contains the core model.
- Entities are independent from EF Core and ASP.NET.

### Task 3 - Add DTO Skeletons [Completed]

Actions:

- Add event DTOs defined in `api-contract.md`.
- Add auth/user DTOs defined in `api-contract.md`.

Output:

- Application layer contains contract DTOs.
- Controllers can use DTOs without exposing domain or EF entities directly.

## Week 3 - EF Core And Event CRUD [Completed]

### Task 1 - Configure EF Core [Completed]

Actions:

- Add EF Core packages.
- Create `EventPlannerDbContext`.
- Add entity configurations.
- Add initial migration.
- Seed a few development events.

Output:

- Database can be created locally.
- Static frontend integration can be replaced by the real event list endpoint.

### Task 2 - Implement Event CRUD [Completed]

Actions:

- Create `IEventRepository`.
- Implement `EventRepository`.
- Create `IEventService`.
- Implement `EventService`.
- Create `EventsController`.
- Implement event CRUD endpoints defined in `api-contract.md`.

Output:

- Full CRUD works.
- Controllers stay thin.
- Business rules stay in Application/Domain.

Acceptance criteria:

- Event list endpoint returns seeded events.
- Event create/update/delete works through HTTP.
- Deleted events are soft-deleted.
- Controllers delegate business work to Application services.

## Week 4 - LINQ, Filtering, And Sorting [In-Progress]

### Task 1 - Add Query Parameters [Completed]

Actions:

- Add `EventQueryParameters`.
- Support query parameters defined in `api-contract.md`.

Output:

- Frontend can request events for visible calendar ranges.

### Task 2 - Centralize Query Logic [Completed]

Actions:

- Add query extension methods:
  - `ApplyDateRange`
  - `ApplyCategoryFilter`
  - `ApplySearch`
  - `ApplySorting`
- Keep LINQ logic out of controllers.

Output:

- Filtering and sorting are reusable and testable.

### Task 3 - Add Query Tests [Completed]

Actions:

- Test date range filtering.
- Test category filtering.
- Test search.
- Test sorting ascending and descending.

Output:

- Query behavior is reliable before the frontend depends on it.

Acceptance criteria:

- Frontend can request events for visible calendar ranges.
- Sorting defaults to `startDateTime asc`.
- Invalid query values return stable `400 Bad Request` responses.
- Query logic is not duplicated in controllers.

## Week 5 - AuthN, AuthZ, And Async [In-Progress]

### Task 1 - Add User Persistence [Completed]

Actions:

- Add user table configuration.
- Add unique email index.
- Add `IUserRepository`.
- Add `UserRepository`.

Output:

- Users can be persisted and queried safely.

### Task 2 - Add Register, Login, And Me [Completed]

Actions:

- Add auth DTOs.
- Add password hashing.
- Add JWT token generation.
- Add `AuthController`.
- Implement auth endpoints defined in `api-contract.md`.

Output:

- Frontend can connect login/register pages.
- JWT token and user context are available.

### Task 3 - Protect Event Endpoints

Actions:

- Enable JWT bearer auth.
- Require auth for create/update/delete.
- Decide whether event reads are public or authenticated.
- Set `OrganizerId` from the current user on create.

Output:

- Event ownership starts to matter.
- Unauthorized writes are blocked.

### Task 4 - Ensure Async Consistency

Actions:

- Make all repository and service methods async.
- Pass `CancellationToken`.
- Avoid sync EF Core calls.

Output:

- Backend follows modern ASP.NET Core async patterns.

Acceptance criteria:

- User can register.
- User can log in.
- Current-user endpoint returns the authenticated user.
- Unauthorized users cannot create/update/delete events.
- Async EF Core methods are used instead of sync database calls.

## Week 6 - Validation, Error Handling, Tests, And Deployment Prep

### Task 1 - Add Validation

Actions:

- Add validators for auth and event requests.
- Return stable validation responses.
- Match frontend form error needs.

Output:

- Invalid payloads fail predictably.
- Frontend can show clear messages.

### Task 2 - Add Error Middleware

Actions:

- Add custom exceptions.
- Add centralized exception handling.
- Use `ProblemDetails` style responses.

Output:

- API errors have consistent shape.
- Controllers do not repeat try/catch blocks.

### Task 3 - Add Test Projects

Actions:

- Add unit tests for services and validators.
- Add integration tests for API endpoints.
- Add test database setup.

Output:

- Core backend behavior has coverage.
- Refactors are safer.

### Task 4 - Prepare For Azure

Actions:

- Move secrets to environment variables or user secrets.
- Add production configuration.
- Add health check endpoint.
- Prepare database connection string.
- Document deployment steps.

Output:

- Backend is ready for optional basic Azure deployment.

Acceptance criteria:

- Invalid payloads return predictable validation errors.
- Not found and conflict cases return stable responses.
- Controllers do not repeat try/catch blocks.
- Core event and auth flows have test coverage.
- Deployment configuration is documented.

## Initial Implementation Order

When starting backend implementation, do this first:

1. Convert `event-planner-be` into an ASP.NET Core Web API. [Completed]
2. Create the solution and layered projects. [Completed]
3. Add Domain entities and enums. [Completed]
4. Add Application DTOs and interfaces. [Completed]
5. Add Infrastructure EF Core setup. [Completed]
6. Add API controllers and DI. [Completed]
7. Implement event CRUD. [Completed]
8. Add seed data. [Completed]
9. Verify with Swagger.
10. Connect frontend `GET /events`.

## Recommended Packages

Core:

```txt
Microsoft.AspNetCore.OpenApi
Swashbuckle.AspNetCore
Microsoft.EntityFrameworkCore
Microsoft.EntityFrameworkCore.Design
Npgsql.EntityFrameworkCore.PostgreSQL
Microsoft.AspNetCore.Authentication.JwtBearer
```

Optional:

```txt
FluentValidation.AspNetCore
BCrypt.Net-Next
Microsoft.AspNetCore.Mvc.Testing
Microsoft.EntityFrameworkCore.InMemory
xunit
FluentAssertions
```

## Definition Of Done

The backend is considered complete when:

- Event CRUD works through HTTP.
- Filtering and sorting work through query parameters.
- Register, login, and current user endpoints work.
- JWT authentication protects the required endpoints.
- Validation errors return stable responses.
- Unexpected errors return safe responses.
- Controllers are thin.
- Business logic is not inside controllers.
- EF Core details are isolated in Infrastructure.
- Unit and integration tests cover the core flow.
- Frontend can register, log in, load events, create events, update events, and delete events.

## Notes For Future Features

Possible later improvements:

- Event attendees.
- Event reminders.
- Recurring events.
- Pagination for large event lists.
- Admin user management.
- Audit logs.
- Drag-and-drop event updates from the frontend.
- Resize event duration from the frontend.
