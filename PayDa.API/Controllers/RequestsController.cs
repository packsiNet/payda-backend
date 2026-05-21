using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PayDa.Application.Requests.Commands.CancelRequest;
using PayDa.Application.Requests.Commands.CreateRequest;
using PayDa.Application.Requests.Commands.PreviewRequest;
using PayDa.Application.Requests.Queries.GetAdminRequests;
using PayDa.Application.Requests.Queries.GetMyOwnRequests;
using PayDa.Application.Requests.Queries.GetMyRequests;
using PayDa.Application.Requests.Queries.GetRequestDetail;
using PayDa.Application.Requests.Queries.SearchMatchingRequests;
using PayDa.Domain.Enums;

namespace PayDa.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class RequestsController : ControllerBase
{
    private readonly ISender _sender;

    public RequestsController(ISender sender) => _sender = sender;

    [HttpGet("admin")]
    [Authorize(Roles = "Admin,Agent")]
    public async Task<IActionResult> GetAdminRequests(
        [FromQuery] RequestType type,
        [FromQuery] Currency currency,
        [FromQuery] decimal? amount)
        => Ok(await _sender.Send(new GetAdminRequestsQuery(type, currency, amount)));

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] RequestType type,
        [FromQuery] Currency currency, [FromQuery] decimal amount)
        => Ok(await _sender.Send(new SearchMatchingRequestsQuery(type, currency, amount)));

    [HttpGet]
    public async Task<IActionResult> GetRequests([FromQuery] RequestType? type)
        => Ok(await _sender.Send(new GetMyRequestsQuery(type)));

    [HttpGet("mine")]
    public async Task<IActionResult> GetMyOwnRequests([FromQuery] RequestType? type)
        => Ok(await _sender.Send(new GetMyOwnRequestsQuery(type)));

    [HttpGet("{id}")]
    public async Task<IActionResult> GetDetail(Guid id)
        => Ok(await _sender.Send(new GetRequestDetailQuery(id)));

    [HttpPost("preview")]
    public async Task<IActionResult> Preview([FromBody] PreviewRequestCommand cmd)
        => Ok(await _sender.Send(cmd));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateRequestCommand cmd)
    {
        var id = await _sender.Send(cmd);
        return CreatedAtAction(nameof(GetDetail), new { id }, new { id });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Cancel(Guid id)
    {
        await _sender.Send(new CancelRequestCommand(id));
        return NoContent();
    }
}
