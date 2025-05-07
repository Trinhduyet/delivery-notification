namespace DeliveryNotification.Entities;

public class ActivityLog
{
    public int Id { get; set; }
    public string CompanyCode { get; set; } = default!;
    public string Channel { get; set; } = default!;
    public string Status { get; set; } = default!;
    public string Message { get; set; } = default!;
    public DateTimeOffset? Timestamp { get; set; } = DateTimeOffset.UtcNow;
    public ETag ETag { get; set; } = ETag.All;
}
