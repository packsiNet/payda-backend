using PayDa.Domain.Enums;

namespace PayDa.Application.Common.Interfaces;

public interface ITelegramBotService
{
    Task SendMatchNotificationAsync(long telegramId, string displayName, RequestType requestType, decimal price, CancellationToken ct = default);
}
