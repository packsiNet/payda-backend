namespace PayDa.Application.Common.Interfaces;

public interface ITelegramAuthService
{
    bool ValidateInitData(string initData);
    TelegramUserData ParseInitData(string initData);
}

public record TelegramUserData(long Id, string? Username, string? FirstName, string? LastName, string? PhotoUrl);
