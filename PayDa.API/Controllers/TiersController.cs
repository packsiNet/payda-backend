using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PayDa.Application.Tiers.Commands.CreateTier;
using PayDa.Application.Tiers.Commands.UpdateTier;
using PayDa.Application.Tiers.Queries.GetAllTiers;

namespace PayDa.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class TiersController : ControllerBase
{
    private readonly ISender _sender;

    public TiersController(ISender sender) => _sender = sender;

    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Ok(await _sender.Send(new GetAllTiersQuery()));

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateTierCommand cmd)
    {
        var id = await _sender.Send(cmd);
        return CreatedAtAction(nameof(GetAll), new { id }, new { id });
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTierRequest req)
    {
        await _sender.Send(new UpdateTierCommand(id, req.Name, req.MaxActiveRequests,
            req.MaxAmountPerRequest, req.RequiredCompletedTransactions));
        return NoContent();
    }
}

public record UpdateTierRequest(string Name, int MaxActiveRequests, decimal MaxAmountPerRequest, int RequiredCompletedTransactions);
