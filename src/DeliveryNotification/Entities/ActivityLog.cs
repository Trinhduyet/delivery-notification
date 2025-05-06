namespace DeliveryNotification.Entities;

public class ActivityLog : ITableEntity
{
    public string PartitionKey { get; set; } = default!;
    public string RowKey { get; set; } = Guid.NewGuid().ToString();
    public string CompanyCode { get; set; } = default!;
    public string Channel { get; set; } = default!;
    public string Status { get; set; } = default!;
    public string Message { get; set; } = default!;
    public DateTimeOffset? Timestamp { get; set; } = DateTimeOffset.UtcNow;
    public ETag ETag { get; set; } = ETag.All;
}
