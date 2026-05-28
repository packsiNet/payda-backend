using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PayDa.Application.Matches.Commands.ConfirmMatch;
using PayDa.Application.Matches.Commands.CreateUserMatch;
using PayDa.Application.Matches.Commands.MatchRequests;
using PayDa.Application.Matches.Commands.RejectMatch;
using PayDa.Application.Matches.Queries.GetMyMatches;
using PayDa.Application.Matches.Queries.GetPendingConfirmationMatches;

namespace PayDa.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class MatchesController : ControllerBase
{
    private readonly ISender _sender;

    public MatchesController(ISender sender) => _sender = sender;

    [HttpGet("my")]
    public async Task<IActionResult> GetMyMatches()
        => Ok(await _sender.Send(new GetMyMatchesQuery()));

    [HttpGet("pending-confirmation")]
    public async Task<IActionResult> GetPendingConfirmationMatches()
        => Ok(await _sender.Send(new GetPendingConfirmationMatchesQuery()));

    [HttpPost("{id}/confirm")]
    public async Task<IActionResult> ConfirmMatch(Guid id)
    {
        await _sender.Send(new ConfirmMatchCommand(id));
        return NoContent();
    }

    [HttpPost("{id}/reject")]
    public async Task<IActionResult> RejectMatch(Guid id)
    {
        await _sender.Send(new RejectMatchCommand(id));
        return NoContent();
    }

    [HttpPost]
    public async Task<IActionResult> CreateMatch([FromBody] CreateUserMatchCommand cmd)
    {
        var result = await _sender.Send(cmd);
        return Ok(new { matchId = result.MatchId, message = result.Message });
    }

    [HttpPost("admin")]
    [Authorize(Roles = "Admin,Agent")]
    public async Task<IActionResult> AdminMatch([FromBody] MatchRequestsCommand cmd)
    {
        var id = await _sender.Send(cmd);
        return Ok(new { id });
    }
}
