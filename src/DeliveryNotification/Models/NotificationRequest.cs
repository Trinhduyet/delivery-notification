namespace DeliveryNotification.Models;

public record NotificationRequest(
    string CompanyCode,
    UserPreference User,
    Dictionary<string, string> MergeTags
);

public record UserPreference(
    string Id,
    string Name,
    string Email,
    string? PhoneNumber,
    List<string> Apps,
    string? WebhookUrl,
    string? DeviceType,
    string? DeviceToken,
    List<string> Channels
);
