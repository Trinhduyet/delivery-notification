namespace DeliveryNotification.Functions;

public class WebhookNotificationFunction(
    [FromKeyedServices(NotificationChannelType.Webhook)] INotificationChannelService notificationChannelService,
    ILogger<WebhookNotificationFunction> logger
)
{
    //[Function(nameof(WebhookNotificationFunction))]
    public async Task RunAsync(
        [ServiceBusTrigger("notification-topic", "webhook", Connection = "ServiceBusConnection")]
            ServiceBusReceivedMessage message,
        CancellationToken cancellationToken
    )
    {
        var payload = JsonSerializer.Deserialize<NotificationPayload>(message.Body);

        if (payload == null)
        {
            logger.LogError("Invalid notification payload in webhook handler.");
            return;
        }

        await notificationChannelService.HandleAsync(payload, cancellationToken);
    }
}
