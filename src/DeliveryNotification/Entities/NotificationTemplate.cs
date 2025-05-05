namespace DeliveryNotification.Entities;

public class NotificationTemplate : ITableEntity
{
    public string PartitionKey { get; set; } = "Templates"; // NotificationId
    public string RowKey { get; set; } = default!;
    public string Subject { get; set; } = default!;
    public string Body { get; set; } = default!;
    public DateTimeOffset? Timestamp { get; set; } = DateTimeOffset.UtcNow;
    public ETag ETag { get; set; } = ETag.All;
}
