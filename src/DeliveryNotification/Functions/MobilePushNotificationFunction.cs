namespace DeliveryNotification.Functions;

public class MobilePushNotificationFunction(
    [FromKeyedServices(NotificationChannelType.MobilePush)] INotificationChannelService notificationChannelService,
    ILogger<MobilePushNotificationFunction> logger
)
{
    //[Function(nameof(MobilePushNotificationFunction))]
    public async Task Run(
        [ServiceBusTrigger(
            "notification-topic",
            "mobile-push-subscription",
            Connection = "ServiceBusConnection"
        )]
            ServiceBusReceivedMessage message,
        CancellationToken cancellationToken
    )
    {
        logger.LogInformation("Mobile Push Notification Function triggered");

        var payloadJson = message.Body.ToString();
        var payload = JsonSerializer.Deserialize<NotificationPayload>(payloadJson);

        if (payload != null)
        {
            await notificationChannelService.HandleAsync(payload, cancellationToken);
        }
        else
        {
            logger.LogError("Failed to deserialize NotificationPayload");
        }
    }
}
