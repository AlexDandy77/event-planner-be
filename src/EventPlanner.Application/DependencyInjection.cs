using EventPlanner.Application.Auth.Dtos;
using EventPlanner.Application.Auth.Services;
using EventPlanner.Application.Auth.Validation;
using EventPlanner.Application.Common.Validation;
using EventPlanner.Application.Events.Dtos;
using EventPlanner.Application.Events.Services;
using EventPlanner.Application.Events.Validation;

using Microsoft.Extensions.DependencyInjection;

namespace EventPlanner.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IEventService, EventService>();
        services.AddSingleton<IRequestValidator<RegisterRequest>, RegisterRequestValidator>();
        services.AddSingleton<IRequestValidator<LoginRequest>, LoginRequestValidator>();
        services.AddSingleton<IRequestValidator<CreateEventRequest>, EventRequestValidator>();
        services.AddSingleton<IRequestValidator<UpdateEventRequest>, EventRequestValidator>();
        services.AddSingleton<IRequestValidator<EventQueryParameters>, EventQueryParametersValidator>();

        return services;
    }
}
