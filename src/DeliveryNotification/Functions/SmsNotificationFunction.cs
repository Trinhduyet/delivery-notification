namespace DeliveryNotification.Functions;

public class SmsNotificationFunction(
    [FromKeyedServices(NotificationChannelType.Sms)] INotificationChannelService notificationChannelService,
    FunctionContext executionContext
)
{
    private readonly ILogger _logger = executionContext.GetLogger<SmsNotificationFunction>();

    [Function(nameof(SmsNotificationFunction))]
    public async Task RunAsync(
        [ServiceBusTrigger(NotificationConstants.TopicName, NotificationConstants.SmsSub, Connection = "ServiceBusConnection")]
            ServiceBusReceivedMessage message,
        CancellationToken cancellationToken
    )
    {
        using var scope = _logger.BeginScope("Sms Processing MessageId: {MessageId}", message.MessageId);
        _logger.LogInformation("Message body: {Body}", message.Body.ToString());

        var payloadJson = message.Body.ToString();
        var payload = JsonSerializer.Deserialize<NotificationPayload>(payloadJson);

        if (payload is null)
        {
            _logger.LogError("Invalid notification payload in sms handler.");
            return;
        }

        try
        {
            await notificationChannelService.HandleAsync(payload, cancellationToken);
            _logger.LogInformation("Successfully processed sms for: {Name}", payload.User?.PhoneNumber);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to sms handle notification for messageid: {MessageId}", message.MessageId);
            throw;
        }
    }
}
