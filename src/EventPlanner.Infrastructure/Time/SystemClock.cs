using EventPlanner.Application.Common.Abstractions;

namespace EventPlanner.Infrastructure.Time;

public sealed class SystemClock : IClock
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}
