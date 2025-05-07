namespace DeliveryNotification.Functions;

public class MobilePushNotificationFunction(
    [FromKeyedServices(NotificationChannelType.MobilePush)] INotificationChannelService notificationChannelService,
    FunctionContext executionContext
)
{
    private readonly ILogger _logger = executionContext.GetLogger<MobilePushNotificationFunction>();

    [Function(nameof(MobilePushNotificationFunction))]
    public async Task Run(
        [ServiceBusTrigger(
            NotificationConstants.TopicName,
            NotificationConstants.MobilePushSub,
            Connection = "ServiceBusConnection"
        )]
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
            _logger.LogError("Invalid notification payload in mobile push handler.");
            return;
        }

        try
        {
            await notificationChannelService.HandleAsync(payload, cancellationToken);
            _logger.LogInformation("Successfully processed mobile push for: {Name}", payload.User?.Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to mobile push handle notification for messageid: {MessageId}", message.MessageId);
            throw;
        }
    }
}
