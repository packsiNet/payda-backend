using MediatR;
using Microsoft.AspNetCore.Mvc;
using PayDa.Application.Auth.Commands.AdminWidgetLogin;
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
        try
        {
            var result = await _sender.Send(new TelegramLoginCommand(req.InitData));
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                error = ex.Message,
                type = ex.GetType().FullName,
                stackTrace = ex.StackTrace,
                innerError = ex.InnerException?.Message,
                innerStackTrace = ex.InnerException?.StackTrace
            });
        }
    }

    [HttpPost("admin-login")]
    public async Task<IActionResult> AdminLogin([FromBody] AdminWidgetLoginRequest req)
    {
        var widgetData = new Dictionary<string, string>
        {
            ["id"]        = req.Id,
            ["auth_date"] = req.AuthDate,
            ["hash"]      = req.Hash,
        };
        if (req.FirstName  is not null) widgetData["first_name"]  = req.FirstName;
        if (req.LastName   is not null) widgetData["last_name"]   = req.LastName;
        if (req.Username   is not null) widgetData["username"]    = req.Username;
        if (req.PhotoUrl   is not null) widgetData["photo_url"]   = req.PhotoUrl;

        var result = await _sender.Send(new AdminWidgetLoginCommand(widgetData));
        return Ok(result);
    }
}

public record TelegramLoginRequest(string InitData);

public record AdminWidgetLoginRequest(
    string Id,
    string AuthDate,
    string Hash,
    string? FirstName,
    string? LastName,
    string? Username,
    string? PhotoUrl);
