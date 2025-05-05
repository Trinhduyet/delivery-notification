namespace DeliveryNotification.Functions;

public class InAppNotificationFunction(
    [FromKeyedServices(NotificationChannelType.InApp)] INotificationChannelService notificationChannelService,
    ILogger<InAppNotificationFunction> logger
)
{
    //[Function(nameof(InAppNotificationFunction))]
    public async Task RunAsync(
        [ServiceBusTrigger("notification-topic", "inapp", Connection = "ServiceBusConnection")]
            ServiceBusReceivedMessage message,
        CancellationToken cancellationToken
    )
    {
        var payload = JsonSerializer.Deserialize<NotificationPayload>(message.Body);

        if (payload == null)
        {
            logger.LogError("Invalid notification payload in inapp handler.");
            return;
        }

        await notificationChannelService.HandleAsync(payload, cancellationToken);
    }
}
