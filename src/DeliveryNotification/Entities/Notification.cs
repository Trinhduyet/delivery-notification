
namespace DeliveryNotification.Entities;

public class Notification
{
    public int Id { get; set; }
    public string CompanyCode { get; set; } = default!;
    public string ServiceCode { get; set; } = default!;
    public string Channels { get; set; } = default!; // Assuming this is a comma-separated string
    public string AlertTypeCode { get; set; } = default!;
}