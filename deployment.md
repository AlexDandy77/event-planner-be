# Event Planner Backend Deployment Notes

## Configuration Sources

The API reads configuration from `appsettings.json`, environment-specific files, user secrets in development, and environment variables.

Use these rules:

- Keep shared non-secret defaults in `appsettings.json`.
- Keep local development defaults in `appsettings.Development.json`.
- Keep local secrets in .NET user secrets.
- Keep Docker, CI, and Azure secrets in environment variables.

Required runtime values:

```txt
ConnectionStrings__DefaultConnection
Jwt__Secret
Jwt__Issuer
Jwt__Audience
Jwt__AccessTokenMinutes
Cors__AllowedOrigins__0
```

`Jwt__Secret` must be a long random value and must not be committed.

## Local Development

Set the local JWT secret once:

```bash
dotnet user-secrets set "Jwt:Secret" "development-only-jwt-secret-change-before-production-1234567890" --project src/EventPlanner.Api/EventPlanner.Api.csproj
```

Start PostgreSQL:

```bash
docker compose up -d postgres
```

Apply EF Core migrations:

```bash
dotnet tool run dotnet-ef database update --project src/EventPlanner.Infrastructure --startup-project src/EventPlanner.Api
```

Run the API:

```bash
dotnet run --project src/EventPlanner.Api/EventPlanner.Api.csproj
```

Health check:

```txt
GET /api/health
```

Swagger UI in development:

```txt
/swagger
```

## Docker Compose

Create a local `.env` from `.env.example` and replace `JWT_SECRET` with a real long random secret.

Run PostgreSQL and the API:

```bash
docker compose up --build
```

The containerized API listens on:

```txt
http://localhost:5080
```

With the default compose file:

- Health: `http://localhost:5080/api/health`
- Swagger: `http://localhost:5080/swagger`
- PostgreSQL: `localhost:5432`

## Azure Deployment Shape

For a basic Azure App Service or Azure Container Apps deployment:

- Build and publish the Docker image from `Dockerfile`.
- Configure `ASPNETCORE_ENVIRONMENT=Production`.
- Configure `ConnectionStrings__DefaultConnection` from the Azure PostgreSQL connection string.
- Configure `Jwt__Secret`, `Jwt__Issuer`, `Jwt__Audience`, and `Jwt__AccessTokenMinutes`.
- Configure `Cors__AllowedOrigins__0` with the deployed frontend URL.
- Run EF Core migrations as a deployment step before sending traffic to the new version.
- Use `/api/health` as the health endpoint.
