namespace DeliveryNotification.Functions;

public class WebhookNotificationFunction(
    [FromKeyedServices(NotificationChannelType.Webhook)] INotificationChannelService notificationChannelService,
    FunctionContext executionContext
)
{
    private readonly ILogger _logger = executionContext.GetLogger<WebhookNotificationFunction>();

    [Function(nameof(WebhookNotificationFunction))]
    public async Task RunAsync(
        [ServiceBusTrigger(NotificationConstants.TopicName, NotificationConstants.SmsSub, Connection = "ServiceBusConnection")]
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
            _logger.LogError("Invalid notification payload in webhook handler.");
            return;
        }

        try
        {
            await notificationChannelService.HandleAsync(payload, cancellationToken);
            _logger.LogInformation("Successfully processed webhook for: {Name}", payload.User?.Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to webhook handle notification for messageid: {MessageId}", message.MessageId);
            throw;
        }
    }
}
