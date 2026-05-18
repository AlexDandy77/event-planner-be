using System.Security.Claims;

using EventPlanner.Application.Common.Abstractions;

namespace EventPlanner.Api.Services;

public sealed class CurrentUserService(IHttpContextAccessor httpContextAccessor)
    : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public Guid? UserId
    {
        get
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(
                ClaimTypes.NameIdentifier
            );

            return Guid.TryParse(userId, out var parsedUserId) ? parsedUserId : null;
        }
    }
}
