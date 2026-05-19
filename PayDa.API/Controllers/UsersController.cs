using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PayDa.Application.Users.Commands.ApproveKyc;
using PayDa.Application.Users.Commands.RejectKyc;
using PayDa.Application.Users.Commands.SetTrusted;
using PayDa.Application.Users.Commands.SetUserRole;
using PayDa.Application.Users.Commands.SubmitKyc;
using PayDa.Application.Users.Commands.VerifyPhone;
using PayDa.Application.Users.Queries.GetAllUsers;
using PayDa.Application.Users.Queries.GetKycStatus;
using PayDa.Application.Users.Queries.GetMyProfile;
using PayDa.Domain.Enums;

namespace PayDa.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly ISender _sender;

    public UsersController(ISender sender) => _sender = sender;

    [HttpGet("me")]
    public async Task<IActionResult> GetMyProfile()
        => Ok(await _sender.Send(new GetMyProfileQuery()));

    [HttpPost("me/phone")]
    public async Task<IActionResult> VerifyPhone([FromBody] VerifyPhoneRequest req)
    {
        await _sender.Send(new VerifyPhoneCommand(req.PhoneNumber));
        return NoContent();
    }

    [HttpGet("me/kyc")]
    public async Task<IActionResult> GetKycStatus()
        => Ok(await _sender.Send(new GetKycStatusQuery()));

    [HttpPost("me/kyc")]
    public async Task<IActionResult> SubmitKyc([FromForm] SubmitKycRequest req)
    {
        await _sender.Send(new SubmitKycCommand(
            req.FirstName, req.LastName, req.DateOfBirth,
            req.SelfieImage.OpenReadStream(), req.SelfieImage.FileName,
            req.DocumentImage.OpenReadStream(), req.DocumentImage.FileName));
        return NoContent();
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllUsers([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        => Ok(await _sender.Send(new GetAllUsersQuery(page, pageSize)));

    [HttpPost("{id}/approve-kyc")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ApproveKyc(Guid id)
    {
        await _sender.Send(new ApproveKycCommand(id));
        return NoContent();
    }

    [HttpPost("{id}/reject-kyc")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> RejectKyc(Guid id)
    {
        await _sender.Send(new RejectKycCommand(id));
        return NoContent();
    }

    [HttpPost("{id}/trusted")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> SetTrusted(Guid id, [FromBody] SetTrustedRequest req)
    {
        await _sender.Send(new SetTrustedCommand(id, req.IsTrusted));
        return NoContent();
    }

    [HttpPost("{id}/role")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> SetRole(Guid id, [FromBody] SetRoleRequest req)
    {
        await _sender.Send(new SetUserRoleCommand(id, req.Role));
        return NoContent();
    }
}

public record VerifyPhoneRequest(string PhoneNumber);

public record SubmitKycRequest(
    string FirstName, string LastName, string DateOfBirth,
    IFormFile SelfieImage, IFormFile DocumentImage);

public record SetTrustedRequest(bool IsTrusted);
public record SetRoleRequest(UserRole Role);
