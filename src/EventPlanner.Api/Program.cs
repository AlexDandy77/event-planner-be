using EventPlanner.Api.Middleware;
using EventPlanner.Api.Services;
using EventPlanner.Application;
using EventPlanner.Application.Common.Abstractions;
using EventPlanner.Infrastructure;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

using System.Text;

var builder = WebApplication.CreateBuilder(args);

const string FrontendCorsPolicy = "Frontend";

var allowedOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .GetChildren()
    .Select(origin => origin.Value!)
    .Where(origin => !string.IsNullOrWhiteSpace(origin))
    .ToArray();

builder.Services.AddControllers();
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context
            .ModelState
            .Where(entry => entry.Value?.Errors.Count > 0)
            .ToDictionary(
                entry => NormalizeModelStateKey(entry.Key),
                entry =>
                    entry
                        .Value!
                        .Errors
                        .Select(error =>
                            string.IsNullOrWhiteSpace(error.ErrorMessage)
                                ? "The value is invalid."
                                : error.ErrorMessage
                        )
                        .ToArray(),
                StringComparer.Ordinal
            );

        var problemDetails = new ValidationProblemDetails(errors)
        {
            Type = "https://httpstatuses.com/400",
            Status = StatusCodes.Status400BadRequest,
            Title = "Validation failed",
            Detail = "One or more validation errors occurred.",
            Instance = context.HttpContext.Request.Path
        };

        return new BadRequestObjectResult(problemDetails)
        {
            ContentTypes = { "application/problem+json" }
        };
    };
});
builder.Services.AddHttpContextAccessor();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddCors(options =>
{
    options.AddPolicy(
        FrontendCorsPolicy,
        policy =>
        {
            policy.WithOrigins(allowedOrigins).AllowAnyHeader().AllowAnyMethod();
        }
    );
});

var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];
var jwtSecret = builder.Configuration["Jwt:Secret"];

if (string.IsNullOrWhiteSpace(jwtSecret))
{
    throw new InvalidOperationException("JWT secret is not configured.");
}

builder
    .Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,
            ValidateAudience = true,
            ValidAudience = jwtAudience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(1)
        };
    });
builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc(
        "v1",
        new OpenApiInfo
        {
            Title = "Event Planner API",
            Version = "v1",
            Description = "Backend API for the Event Planner application."
        }
    );

    options.AddSecurityDefinition(
        JwtBearerDefaults.AuthenticationScheme,
        new OpenApiSecurityScheme
        {
            Description = "JWT Bearer token. Paste the access token returned by login.",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT"
        }
    );

    options.AddSecurityRequirement(document =>
    {
        var bearerSecurityScheme = new OpenApiSecuritySchemeReference(
            JwtBearerDefaults.AuthenticationScheme,
            document
        );

        return new OpenApiSecurityRequirement
        {
            { bearerSecurityScheme, [] }
        };
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Event Planner API v1");
        options.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseCors(FrontendCorsPolicy);

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();

static string NormalizeModelStateKey(string key)
{
    if (string.IsNullOrWhiteSpace(key))
    {
        return "request";
    }

    var lastSegment = key.Split('.').Last();

    return char.ToLowerInvariant(lastSegment[0]) + lastSegment[1..];
}
