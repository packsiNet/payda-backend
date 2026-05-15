using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PayDa.Application.ExchangeRates.Commands.UpdateExchangeRate;
using PayDa.Application.ExchangeRates.Queries.GetExchangeRates;

namespace PayDa.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class ExchangeRatesController : ControllerBase
{
    private readonly ISender _sender;

    public ExchangeRatesController(ISender sender) => _sender = sender;

    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Ok(await _sender.Send(new GetExchangeRatesQuery()));

    [HttpPut]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update([FromBody] UpdateExchangeRateCommand cmd)
    {
        await _sender.Send(cmd);
        return NoContent();
    }
}
