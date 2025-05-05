namespace DeliveryNotification.Models;

public class NotificationPayload
{
    public string NotificationId { get; set; } = default!;
    public UserPreference User { get; set; } = default!;
    public Dictionary<string, string> MergeTags { get; set; } = [];
}

public class UserPreference
{
    public string Email { get; set; } = default!;
    public string Phone { get; set; } = default!;
    public string Name { get; set; } = default!;
    public List<string> Apps { get; set; } = [];
    public string? WebhookUrl { get; set; }
    public string? DeviceToken { get; set; }
    public string? PhoneNumber { get; set; }
    public List<string> Channels { get; set; } = [];
}