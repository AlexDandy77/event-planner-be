using Microsoft.AspNetCore.Mvc;

namespace EventPlanner.Api.Controllers;

[ApiController]
[Route("api/health")]
public sealed class HealthController : ControllerBase
{
    [HttpGet]
    public ActionResult<HealthCheckResponse> Get()
    {
        return Ok(new HealthCheckResponse("healthy", DateTimeOffset.UtcNow));
    }
}

public sealed record HealthCheckResponse(string Status, DateTimeOffset CheckedAt);
