using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Web;
using Microsoft.Extensions.Configuration;
using PayDa.Application.Common.Interfaces;

namespace PayDa.Infrastructure.Services;

public class TelegramAuthService : ITelegramAuthService
{
    private readonly string _botToken;

    public TelegramAuthService(IConfiguration config)
    {
        _botToken = config["Telegram:BotToken"]!;
    }

    public bool ValidateInitData(string initData)
    {
        var parsed = HttpUtility.ParseQueryString(initData);
        var hash = parsed["hash"];
        parsed.Remove("hash");

        var dataCheckString = string.Join("\n",
            parsed.AllKeys!.OrderBy(k => k).Select(k => $"{k}={parsed[k]}"));

        using var keyHmac = new HMACSHA256(Encoding.UTF8.GetBytes("WebAppData"));
        var secretKey = keyHmac.ComputeHash(Encoding.UTF8.GetBytes(_botToken));
        using var hmac = new HMACSHA256(secretKey);
        var computedHash = Convert.ToHexString(
            hmac.ComputeHash(Encoding.UTF8.GetBytes(dataCheckString))).ToLower();

        return computedHash == hash;
    }

    public TelegramUserData ParseInitData(string initData)
    {
        var parsed = HttpUtility.ParseQueryString(initData);
        var userJson = parsed["user"]!;
        var user = JsonSerializer.Deserialize<JsonElement>(userJson);

        return new TelegramUserData(
            user.GetProperty("id").GetInt64(),
            user.TryGetProperty("username", out var un) ? un.GetString() : null,
            user.TryGetProperty("first_name", out var fn) ? fn.GetString() : null,
            user.TryGetProperty("last_name", out var ln) ? ln.GetString() : null,
            user.TryGetProperty("photo_url", out var ph) ? ph.GetString() : null
        );
    }

    public bool ValidateContactResponse(string contactResponse) => ValidateInitData(contactResponse);

    public bool ValidateWidgetData(Dictionary<string, string> data)
    {
        if (!data.TryGetValue("hash", out var hash)) return false;

        var dataCheckString = string.Join("\n",
            data.Where(kv => kv.Key != "hash")
                .OrderBy(kv => kv.Key)
                .Select(kv => $"{kv.Key}={kv.Value}"));

        // Widget uses SHA256(botToken) as secret — different from WebApp's HMAC("WebAppData", botToken)
        using var sha = SHA256.Create();
        var secretKey = sha.ComputeHash(Encoding.UTF8.GetBytes(_botToken));
        using var hmac = new HMACSHA256(secretKey);
        var computedHash = Convert.ToHexString(
            hmac.ComputeHash(Encoding.UTF8.GetBytes(dataCheckString))).ToLower();

        return computedHash == hash;
    }

    public TelegramUserData ParseWidgetData(Dictionary<string, string> data)
    {
        data.TryGetValue("id", out var idStr);
        data.TryGetValue("username", out var username);
        data.TryGetValue("first_name", out var firstName);
        data.TryGetValue("last_name", out var lastName);
        data.TryGetValue("photo_url", out var photoUrl);

        return new TelegramUserData(
            long.Parse(idStr!),
            username,
            firstName,
            lastName,
            photoUrl);
    }

    public TelegramContactData ParseContactResponse(string contactResponse)
    {
        var parsed = HttpUtility.ParseQueryString(contactResponse);
        var contactJson = parsed["contact"]!;
        var contact = JsonSerializer.Deserialize<JsonElement>(contactJson);

        return new TelegramContactData(
            contact.GetProperty("phone_number").GetString()!,
            contact.TryGetProperty("first_name", out var fn) ? fn.GetString() : null,
            contact.GetProperty("user_id").GetInt64()
        );
    }
}
