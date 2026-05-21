using EventPlanner.Api.Middleware;
using EventPlanner.Api.Errors;
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

builder
    .Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwtSecret = GetRequiredConfigurationValue(builder.Configuration, "Jwt:Secret");

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],
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

ValidateRequiredConfiguration(app.Configuration);

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

app.UseStatusCodePages(async statusCodeContext =>
{
    var httpContext = statusCodeContext.HttpContext;
    var statusCode = httpContext.Response.StatusCode;

    if (httpContext.Response.HasStarted || statusCode < StatusCodes.Status400BadRequest)
    {
        return;
    }

    var (title, detail) = statusCode switch
    {
        StatusCodes.Status400BadRequest => ("Bad Request", "The request could not be processed."),
        StatusCodes.Status401Unauthorized => ("Unauthorized", "Authentication is required."),
        StatusCodes.Status403Forbidden => ("Forbidden", "You do not have access to this resource."),
        StatusCodes.Status404NotFound => ("Not Found", "The requested resource was not found."),
        _ => ("Error", "The request failed.")
    };

    await ProblemDetailsResponseWriter.WriteAsync(
        httpContext,
        new ProblemDetails
        {
            Type = $"https://httpstatuses.com/{statusCode}",
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = httpContext.Request.Path
        }
    );
});

app.UseCors(FrontendCorsPolicy);

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();

static void ValidateRequiredConfiguration(IConfiguration configuration)
{
    _ = GetRequiredConfigurationValue(configuration, "ConnectionStrings:DefaultConnection");
    _ = GetRequiredConfigurationValue(configuration, "Jwt:Secret");
}

static string GetRequiredConfigurationValue(IConfiguration configuration, string key)
{
    var value = configuration[key];

    if (string.IsNullOrWhiteSpace(value))
    {
        throw new InvalidOperationException($"Configuration value '{key}' is not configured.");
    }

    return value;
}

static string NormalizeModelStateKey(string key)
{
    if (string.IsNullOrWhiteSpace(key))
    {
        return "request";
    }

    var lastSegment = key.Split('.').Last();

    return char.ToLowerInvariant(lastSegment[0]) + lastSegment[1..];
}
