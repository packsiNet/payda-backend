using MediatR;
using Microsoft.AspNetCore.Mvc;
using PayDa.Application.Auth.Commands.TelegramLogin;

namespace PayDa.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ISender _sender;

    public AuthController(ISender sender) => _sender = sender;

    [HttpPost("telegram-login")]
    public async Task<IActionResult> TelegramLogin([FromBody] TelegramLoginRequest req)
    {
        var result = await _sender.Send(new TelegramLoginCommand(req.InitData));
        return Ok(result);
    }
}

public record TelegramLoginRequest(string InitData);
