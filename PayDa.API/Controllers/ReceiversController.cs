using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PayDa.Application.Receivers.Commands.CreateReceiver;
using PayDa.Application.Receivers.Commands.DeleteReceiver;
using PayDa.Application.Receivers.Queries.GetMyReceivers;

namespace PayDa.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class ReceiversController : ControllerBase
{
    private readonly ISender _sender;

    public ReceiversController(ISender sender) => _sender = sender;

    [HttpGet]
    public async Task<IActionResult> GetMyReceivers()
        => Ok(await _sender.Send(new GetMyReceiversQuery()));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateReceiverCommand cmd)
    {
        var id = await _sender.Send(cmd);
        return CreatedAtAction(nameof(GetMyReceivers), new { id }, new { id });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _sender.Send(new DeleteReceiverCommand(id));
        return NoContent();
    }
}
