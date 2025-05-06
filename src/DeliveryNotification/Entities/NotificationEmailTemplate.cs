namespace DeliveryNotification.Entities;

public class NotificationEmailTemplate
{
    public int Id { get; set; }
    public int NotificationId { get; set; }
    public string CultureCode { get; set; } = default!;
    public string Subject { get; set; } = default!;
    public string HtmlBody { get; set; } = default!;
}