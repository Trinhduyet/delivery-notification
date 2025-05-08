namespace DeliveryNotification.Entities;

public class ActivityLog
{
    public int Id { get; set; }
    public string UserId { get; set; } = default!;
    public string CompanyCode { get; set; } = default!;
    public string Channel { get; set; } = default!;
    public string Status { get; set; } = default!;
    public string Title { get; set; } = default!;
    public string Message { get; set; } = default!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
