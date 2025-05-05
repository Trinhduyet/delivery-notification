namespace DeliveryNotification.Services;

public class ActivityLogService(ITableClientsProvider provider) : IActivityLogService
{
    private readonly TableClient _tableClient = provider.ActivityLogs;

    public async Task LogActivityAsync(ActivityLog log, CancellationToken cancellationToken)
    {
        var tableEntity = new TableEntity(log.PartitionKey, Guid.NewGuid().ToString())
        {
            { "Channel", log.Channel },
            { "Status", log.Status },
            { "Timestamp", log.Timestamp },
            { "Message", log.Message },
        };

        await _tableClient.AddEntityAsync(tableEntity, cancellationToken: cancellationToken);
    }
}
