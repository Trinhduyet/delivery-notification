namespace DeliveryNotification.Functions;

public class SmsNotificationFunction(
    [FromKeyedServices(NotificationChannelType.Sms)] INotificationChannelService notificationChannelService,
    ILogger<SmsNotificationFunction> logger
)
{
    //[Function(nameof(SmsNotificationFunction))]
    public async Task RunAsync(
        [ServiceBusTrigger("notification-topic", "sms", Connection = "ServiceBusConnection")]
            ServiceBusReceivedMessage message,
        CancellationToken cancellationToken
    )
    {
        var payload = JsonSerializer.Deserialize<NotificationPayload>(message.Body);

        if (payload == null)
        {
            logger.LogError("Invalid notification payload in sms handler.");
            return;
        }

        await notificationChannelService.HandleAsync(payload, cancellationToken);
    }
}
