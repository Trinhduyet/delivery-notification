namespace DeliveryNotification.Functions;

public class WebPushNotificationFunction(
    [FromKeyedServices(NotificationChannelType.WebPush)] INotificationChannelService notificationChannelService,
    ILoggerFactory loggerFactory
)
{
    private readonly ILogger<WebPushNotificationFunction> _logger = loggerFactory.CreateLogger<WebPushNotificationFunction>();

    [Function(nameof(WebPushNotificationFunction))]
    public async Task RunAsync(
        [ServiceBusTrigger(NotificationConstants.TopicName, NotificationConstants.WebPushSub, Connection = "ServiceBusConnection")]
            ServiceBusReceivedMessage message,
        CancellationToken cancellationToken
    )
    {
        using var scope = _logger.BeginScope("Mobile Push Processing MessageId: {MessageId}", message.MessageId);
        _logger.LogInformation("Message body: {Body}", message.Body.ToString());

        var payloadJson = message.Body.ToString();
        var payload = JsonSerializer.Deserialize<NotificationRequest>(payloadJson);

        if (payload is null)
        {
            _logger.LogError("Invalid notification payload in webpush handler.");
            return;
        }

        try
        {
            await notificationChannelService.HandleAsync(payload, cancellationToken);
            _logger.LogInformation("Successfully processed webpush for: {Name}", payload.User?.Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to webpush handle notification for messageid: {MessageId}", message.MessageId);
            throw;
        }
    }
}
