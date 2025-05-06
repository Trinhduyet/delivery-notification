namespace DeliveryNotification.Entities;

public class NotificationPushTemplate
{
    public int Id { get; set; }
    public int NotificationId { get; set; }
    public string CultureCode { get; set; } = default!;
    public string Title { get; set; } = default!;
    public string Message { get; set; } = default!;
}
