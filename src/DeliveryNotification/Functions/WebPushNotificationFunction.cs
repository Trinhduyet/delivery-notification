namespace DeliveryNotification.Functions;

public class WebPushNotificationFunction(
    [FromKeyedServices(NotificationChannelType.WebPush)] INotificationChannelService notificationChannelService,
    ILogger<WebPushNotificationFunction> logger
)
{
    //[Function(nameof(WebPushNotificationFunction))]
    public async Task RunAsync(
        [ServiceBusTrigger("notification-topic", "webpush", Connection = "ServiceBusConnection")]
            ServiceBusReceivedMessage message,
        CancellationToken cancellationToken
    )
    {
        var payload = JsonSerializer.Deserialize<NotificationPayload>(message.Body);

        if (payload == null)
        {
            logger.LogError("Invalid notification payload in webpush handler.");
            return;
        }

        await notificationChannelService.HandleAsync(payload, cancellationToken);
    }
}
