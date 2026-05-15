using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PayDa.Application.Transactions.Commands.ConfirmPayment;
using PayDa.Application.Transactions.Commands.SettleTransaction;
using PayDa.Application.Transactions.Commands.UploadScreenshot;
using PayDa.Application.Transactions.Queries.GetTransactionDetail;

namespace PayDa.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class TransactionsController : ControllerBase
{
    private readonly ISender _sender;

    public TransactionsController(ISender sender) => _sender = sender;

    [HttpGet("{id}")]
    public async Task<IActionResult> GetDetail(Guid id)
        => Ok(await _sender.Send(new GetTransactionDetailQuery(id)));

    [HttpPost("{id}/screenshot")]
    public async Task<IActionResult> UploadScreenshot(Guid id, IFormFile file)
    {
        await _sender.Send(new UploadScreenshotCommand(id, file.OpenReadStream(), file.FileName));
        return NoContent();
    }

    [HttpPost("{id}/confirm")]
    public async Task<IActionResult> Confirm(Guid id)
    {
        await _sender.Send(new ConfirmPaymentCommand(id));
        return NoContent();
    }

    [HttpPost("{id}/settle")]
    [Authorize(Roles = "Admin,Agent")]
    public async Task<IActionResult> Settle(Guid id)
    {
        await _sender.Send(new SettleTransactionCommand(id));
        return NoContent();
    }
}
