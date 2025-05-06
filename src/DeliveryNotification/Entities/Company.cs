namespace DeliveryNotification.Entities;

public class Company
{
    public int Id { get; set; }
    public string CompanyCode { get; set; } = default!;
    public string CompanyName { get; set; } = default!;
}