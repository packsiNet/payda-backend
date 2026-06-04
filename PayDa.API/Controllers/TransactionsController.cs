using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PayDa.Application.Transactions.Commands.AdminSettleTransaction;
using PayDa.Application.Transactions.Commands.ConfirmForeignReceipt;
using PayDa.Application.Transactions.Commands.ConfirmPayment;
using PayDa.Application.Transactions.Commands.SettleTransaction;
using PayDa.Application.Transactions.Commands.UploadScreenshot;
using PayDa.Application.Transactions.Queries.GetAdminPendingTomanTransactions;
using PayDa.Application.Transactions.Queries.GetMyTransactions;
using PayDa.Application.Transactions.Queries.GetTransactionDetail;
using PayDa.Domain.Enums;

namespace PayDa.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class TransactionsController : ControllerBase
{
    private readonly ISender _sender;

    public TransactionsController(ISender sender) => _sender = sender;

    [HttpGet]
    public async Task<IActionResult> GetMyTransactions(
        [FromQuery] RequestType? type,
        [FromQuery] TransactionStatus? status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
        => Ok(await _sender.Send(new GetMyTransactionsQuery(type, status, page, pageSize)));

    [HttpGet("{id}")]
    public async Task<IActionResult> GetDetail(Guid id)
        => Ok(await _sender.Send(new GetTransactionDetailQuery(id)));

    /// <summary>Receiver declares they have paid toman (no receipt needed)</summary>
    [HttpPost("{id}/declare-toman-payment")]
    public async Task<IActionResult> DeclareTomanPayment(Guid id)
    {
        await _sender.Send(new DeclareTomanPaymentCommand(id));
        return NoContent();
    }

    /// <summary>Admin: list transactions waiting for toman confirmation</summary>
    [HttpGet("admin/pending-toman")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAdminPendingToman()
        => Ok(await _sender.Send(new GetAdminPendingTomanTransactionsQuery()));

    /// <summary>Admin confirms toman payment received</summary>
    [HttpPost("{id}/confirm-toman")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ConfirmTomanPayment(Guid id)
    {
        await _sender.Send(new ConfirmTomanPaymentCommand(id));
        return NoContent();
    }

    /// <summary>Sender uploads foreign transfer receipt (after toman confirmed)</summary>
    [HttpPost("{id}/foreign-receipt")]
    public async Task<IActionResult> UploadForeignReceipt(Guid id, IFormFile file)
    {
        await _sender.Send(new UploadForeignReceiptCommand(id, file.OpenReadStream(), file.FileName));
        return NoContent();
    }

    /// <summary>Receiver confirms foreign currency received — awaiting admin settlement</summary>
    [HttpPost("{id}/confirm-foreign")]
    public async Task<IActionResult> ConfirmForeignReceipt(Guid id)
    {
        await _sender.Send(new ConfirmForeignReceiptCommand(id));
        return NoContent();
    }

    /// <summary>Admin settles toman payment to toman receiver and finalizes the match</summary>
    [HttpPost("{id}/admin-settle")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AdminSettle(Guid id)
    {
        await _sender.Send(new AdminSettleTransactionCommand(id));
        return NoContent();
    }
}
