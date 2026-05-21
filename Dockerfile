FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

COPY src/EventPlanner.Api/EventPlanner.Api.csproj src/EventPlanner.Api/
COPY src/EventPlanner.Application/EventPlanner.Application.csproj src/EventPlanner.Application/
COPY src/EventPlanner.Domain/EventPlanner.Domain.csproj src/EventPlanner.Domain/
COPY src/EventPlanner.Infrastructure/EventPlanner.Infrastructure.csproj src/EventPlanner.Infrastructure/

RUN dotnet restore src/EventPlanner.Api/EventPlanner.Api.csproj

COPY . .

RUN dotnet publish src/EventPlanner.Api/EventPlanner.Api.csproj \
    --configuration Release \
    --output /app/publish \
    --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime

WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080

EXPOSE 8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "EventPlanner.Api.dll"]
