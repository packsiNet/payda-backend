using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using PayDa.Application.Common.Interfaces;
using PayDa.Domain.Enums;

namespace PayDa.Infrastructure.Services;

public class TelegramBotService : ITelegramBotService
{
    private static readonly HttpClient _http = new();
    private readonly string _token;
    private readonly string _appUrl;

    public TelegramBotService(IConfiguration config)
    {
        _token = config["Telegram:BotToken"]!;
        _appUrl = config["Telegram:AppUrl"]!;
    }

    public async Task SendMatchNotificationAsync(long telegramId, string displayName, RequestType requestType, decimal price, CancellationToken ct = default)
    {
        var typeLabel = requestType == RequestType.Send ? "ارسال" : "دریافت";
        var priceFormatted = price.ToString("N0");

        var text = $"""
            کاربر گرامی: {displayName}
            درخواست {typeLabel} شما آماده‌ی انجام است
            با مبلغ: {priceFormatted} تومان
            چنانچه برای انجام با این مبلغ آماده‌اید
            تایید کنید
            در غیراینصورت برای مبالغ بهتر می‌توانید درخواست را رد کنید و منتظر شرایط بهتر باشید
            """;

        var payload = new
        {
            chat_id = telegramId,
            text,
            reply_markup = new
            {
                inline_keyboard = new[]
                {
                    new[]
                    {
                        new
                        {
                            text = "بازبینی درخواست",
                            web_app = new { url = _appUrl }
                        }
                    }
                }
            }
        };

        var url = $"https://api.telegram.org/bot{_token}/sendMessage";
        await _http.PostAsJsonAsync(url, payload, ct);
    }
}
