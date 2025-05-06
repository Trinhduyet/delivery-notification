namespace DeliveryNotification.Models;

public class NotificationPayload
{
    public string CompanyCode { get; set; } = default!;
    public UserPreference User { get; set; } = default!;
    public Dictionary<string, string> MergeTags { get; set; } = [];
}

public class UserPreference
{
    public string Name { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string? PhoneNumber { get; set; }
    public List<string> Apps { get; set; } = [];
    public string? WebhookUrl { get; set; }
    public string? DeviceToken { get; set; }
    public List<string> Channels { get; set; } = [];
}