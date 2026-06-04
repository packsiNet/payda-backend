using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PayDa.Infrastructure.Persistence;

namespace PayDa.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DevController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IConfiguration _config;

    public DevController(AppDbContext db, IConfiguration config)
    {
        _db = db;
        _config = config;
    }

    [HttpPost("reset")]
    public async Task<IActionResult> Reset([FromBody] ResetRequest req)
    {
        var expectedHash = _config["DevReset:PasswordHash"];
        if (string.IsNullOrEmpty(expectedHash))
            return StatusCode(503, "DevReset not configured");

        var inputHash = Convert.ToHexString(
            SHA256.HashData(Encoding.UTF8.GetBytes(req.Password))
        ).ToLowerInvariant();

        if (!string.Equals(inputHash, expectedHash.ToLowerInvariant(), StringComparison.Ordinal))
            return Unauthorized("Wrong password");

        await _db.Database.ExecuteSqlRawAsync("DELETE FROM \"Transactions\"");
        await _db.Database.ExecuteSqlRawAsync("DELETE FROM \"Matches\"");
        await _db.Database.ExecuteSqlRawAsync("DELETE FROM \"RequestForeignAccounts\"");
        await _db.Database.ExecuteSqlRawAsync("DELETE FROM \"Requests\"");
        await _db.Database.ExecuteSqlRawAsync("DELETE FROM \"Receivers\"");
        await _db.Database.ExecuteSqlRawAsync("DELETE FROM \"Users\"");

        return Ok(new { message = "Database reset. Seed data preserved." });
    }

    [HttpPost("make-admin")]
    public async Task<IActionResult> MakeAdmin([FromBody] MakeAdminRequest req)
    {
        var expectedHash = _config["DevReset:PasswordHash"];
        if (string.IsNullOrEmpty(expectedHash))
            return StatusCode(503, "DevReset not configured");

        var inputHash = Convert.ToHexString(
            SHA256.HashData(Encoding.UTF8.GetBytes(req.Password))
        ).ToLowerInvariant();

        if (!string.Equals(inputHash, expectedHash.ToLowerInvariant(), StringComparison.Ordinal))
            return Unauthorized("Wrong password");

        var user = await _db.Users.FirstOrDefaultAsync(u => u.TelegramId == req.TelegramId);
        if (user is null)
            return NotFound($"User with TelegramId {req.TelegramId} not found");

        user.SetRole(PayDa.Domain.Enums.UserRole.Admin);
        await _db.SaveChangesAsync();

        return Ok(new { message = $"User {req.TelegramId} is now Admin" });
    }

    public record ResetRequest(string Password);
    public record MakeAdminRequest(string Password, long TelegramId);
}
