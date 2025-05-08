namespace DeliveryNotification.Services.Interfaces;

public interface ITableClientsProvider
{
    TableClient NotificationTemplates { get; }
    TableClient ActivityLogs { get; }
}
