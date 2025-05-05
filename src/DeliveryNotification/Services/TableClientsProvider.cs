namespace DeliveryNotification.Services;

public class TableClientsProvider : ITableClientsProvider
{
    public TableClient NotificationTemplates { get; }
    public TableClient ActivityLogs { get; }

    public TableClientsProvider(TableServiceClient serviceClient)
    {
        NotificationTemplates = serviceClient.GetTableClient("NotificationTemplates");
        NotificationTemplates.CreateIfNotExists();

        ActivityLogs = serviceClient.GetTableClient("ActivityLogs");
        ActivityLogs.CreateIfNotExists();
    }
}

