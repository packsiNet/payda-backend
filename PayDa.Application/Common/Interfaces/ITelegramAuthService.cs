namespace PayDa.Application.Common.Interfaces;

public interface ITelegramAuthService
{
    bool ValidateInitData(string initData);
    TelegramUserData ParseInitData(string initData);
    bool ValidateContactResponse(string contactResponse);
    TelegramContactData ParseContactResponse(string contactResponse);
    bool ValidateWidgetData(Dictionary<string, string> data);
    TelegramUserData ParseWidgetData(Dictionary<string, string> data);
}

public record TelegramUserData(long Id, string? Username, string? FirstName, string? LastName, string? PhotoUrl);
public record TelegramContactData(string PhoneNumber, string? FirstName, long UserId);
