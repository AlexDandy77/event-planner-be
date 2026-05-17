using EventPlanner.Application.Events.Dtos;
using EventPlanner.Application.Events.Services;

using Microsoft.AspNetCore.Mvc;

namespace EventPlanner.Api.Controllers;

[ApiController]
[Route("api/events")]
public sealed class EventsController(IEventService eventService) : ControllerBase
{
    private readonly IEventService _eventService = eventService;

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<EventResponse>>> GetAll(
        [FromQuery] EventQueryParameters queryParameters,
        CancellationToken cancellationToken
    )
    {
        var events = await _eventService.GetEventsAsync(queryParameters, cancellationToken);

        return Ok(events);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<EventResponse>> GetById(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        var calendarEvent = await _eventService.GetEventByIdAsync(id, cancellationToken);

        if (calendarEvent is null)
        {
            return NotFound();
        }

        return Ok(calendarEvent);
    }

    [HttpPost]
    public async Task<ActionResult<EventResponse>> Create(
        CreateEventRequest request,
        CancellationToken cancellationToken
    )
    {
        var calendarEvent = await _eventService.CreateEventAsync(request, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = calendarEvent.Id }, calendarEvent);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<EventResponse>> Update(
        Guid id,
        UpdateEventRequest request,
        CancellationToken cancellationToken
    )
    {
        var calendarEvent = await _eventService.UpdateEventAsync(id, request, cancellationToken);

        if (calendarEvent is null)
        {
            return NotFound();
        }

        return Ok(calendarEvent);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var wasDeleted = await _eventService.DeleteEventAsync(id, cancellationToken);

        if (!wasDeleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}
