namespace DeliveryNotification.Entities;

public class Channel
{
    public int Id { get; set; }
    public string Code { get; set; } = default!;
    public string Name { get; set; } = default!;
}