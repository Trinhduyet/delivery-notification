namespace DeliveryNotification.Entities;

public class NotificationSetting
{
    public int Id { get; set; }
    public string Deduplication { get; set; } = default!; // Assuming this is a JSON string
    public string Throttling { get; set; } = default!; // Assuming this is a JSON string
    public string Retention { get; set; } = default!; // Assuming this is a JSON string
}
