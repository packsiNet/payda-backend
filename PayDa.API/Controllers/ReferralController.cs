using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PayDa.Application.Referral.Commands.ApplyReferralCode;

namespace PayDa.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class ReferralController : ControllerBase
{
    private readonly ISender _sender;

    public ReferralController(ISender sender) => _sender = sender;

    [HttpPost("apply")]
    public async Task<IActionResult> ApplyReferralCode([FromBody] ApplyReferralCodeRequest req)
    {
        await _sender.Send(new ApplyReferralCodeCommand(req.ReferralCode));
        return NoContent();
    }
}

public record ApplyReferralCodeRequest(string ReferralCode);
