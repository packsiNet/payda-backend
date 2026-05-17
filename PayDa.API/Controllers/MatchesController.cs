using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PayDa.Application.Matches.Commands.MatchRequests;
using PayDa.Application.Matches.Queries.GetMyMatches;

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

    [HttpPost]
    [Authorize(Roles = "Admin,Agent")]
    public async Task<IActionResult> Match([FromBody] MatchRequestsCommand cmd)
    {
        var id = await _sender.Send(cmd);
        return Ok(new { id });
    }
}
