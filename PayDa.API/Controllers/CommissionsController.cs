using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PayDa.Application.Commissions.Commands.SetTierCommission;
using PayDa.Application.Commissions.Queries.GetTierCommissions;

namespace PayDa.API.Controllers;

[ApiController]
[Authorize(Roles = "Admin")]
[Route("api/[controller]")]
public class CommissionsController : ControllerBase
{
    private readonly ISender _sender;

    public CommissionsController(ISender sender) => _sender = sender;

    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Ok(await _sender.Send(new GetTierCommissionsQuery()));

    [HttpPut("{tierId:guid}")]
    public async Task<IActionResult> Set(Guid tierId, [FromBody] SetCommissionRequest req)
    {
        await _sender.Send(new SetTierCommissionCommand(tierId, req.SenderCommissionPercent, req.ReceiverCommissionPercent));
        return NoContent();
    }
}

public record SetCommissionRequest(decimal SenderCommissionPercent, decimal ReceiverCommissionPercent);
