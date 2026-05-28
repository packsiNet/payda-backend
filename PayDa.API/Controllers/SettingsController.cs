using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PayDa.Application.Settings.Commands.SetMatchConfirmationHours;
using PayDa.Application.Settings.Queries.GetMatchConfirmationHours;

namespace PayDa.API.Controllers;

[ApiController]
[Authorize(Roles = "Admin")]
[Route("api/[controller]")]
public class SettingsController : ControllerBase
{
    private readonly ISender _sender;

    public SettingsController(ISender sender) => _sender = sender;

    [HttpGet("match-confirmation-hours")]
    public async Task<IActionResult> GetMatchConfirmationHours()
        => Ok(new { hours = await _sender.Send(new GetMatchConfirmationHoursQuery()) });

    [HttpPut("match-confirmation-hours")]
    public async Task<IActionResult> SetMatchConfirmationHours([FromBody] SetMatchConfirmationHoursRequest req)
    {
        await _sender.Send(new SetMatchConfirmationHoursCommand(req.Hours));
        return NoContent();
    }
}

public record SetMatchConfirmationHoursRequest(int Hours);
