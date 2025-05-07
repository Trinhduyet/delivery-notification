namespace DeliveryNotification.Functions;

public class EmailNotificationFunction(
    [FromKeyedServices(NotificationChannelType.Email)] INotificationChannelService notificationChannelService,
    FunctionContext executionContext
)
{
    private readonly ILogger _logger = executionContext.GetLogger<EmailNotificationFunction>();

    [Function(nameof(EmailNotificationFunction))]
    public async Task RunAsync(
        [ServiceBusTrigger(NotificationConstants.TopicName, NotificationConstants.EmailSub, Connection = "ServiceBusConnection")]
            ServiceBusReceivedMessage message,
        CancellationToken cancellationToken)
    {
        using var scope = _logger.BeginScope("Processing MessageId: {MessageId}", message.MessageId);
        _logger.LogInformation("Message body: {Body}", message.Body.ToString());

        var payload = JsonSerializer.Deserialize<NotificationRequest>(message.Body);

        if (payload is null)
        {
            _logger.LogError("Notification payload is null. Raw body: {Body}", message.Body.ToString());
            return;
        }

        try
        {
            await notificationChannelService.HandleAsync(payload, cancellationToken);
            _logger.LogInformation("Successfully processed email for: {Email}", payload.User?.Email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to email handle notification for messageid: {MessageId}", message.MessageId);
            throw;
        }
    }
}
