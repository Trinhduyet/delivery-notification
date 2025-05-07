namespace DeliveryNotification.Functions;

public class InAppNotificationFunction(
    [FromKeyedServices(NotificationChannelType.InApp)] INotificationChannelService notificationChannelService,
    FunctionContext executionContext
)
{
    private readonly ILogger _logger = executionContext.GetLogger<InAppNotificationFunction>();

    [Function(nameof(InAppNotificationFunction))]
    public async Task RunAsync(
        [ServiceBusTrigger(NotificationConstants.TopicName, NotificationConstants.InAppSub, Connection = "ServiceBusConnection")]
            ServiceBusReceivedMessage message,
        CancellationToken cancellationToken
    )
    {
        using var scope = _logger.BeginScope("InApp Processing MessageId: {MessageId}", message.MessageId);
        _logger.LogInformation("Message body: {Body}", message.Body.ToString());

        var payload = JsonSerializer.Deserialize<NotificationRequest>(message.Body);

        if (payload is null)
        {
            _logger.LogError("Invalid notification payload in inapp handler.");
            return;
        }

        try
        {
            await notificationChannelService.HandleAsync(payload, cancellationToken);
            _logger.LogInformation("Successfully processed inapp for: {Name}", payload.User?.Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to inapp handle notification for messageid: {MessageId}", message.MessageId);
            throw;
        }
    }
}
