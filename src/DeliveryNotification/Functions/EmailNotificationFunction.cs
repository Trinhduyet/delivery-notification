namespace DeliveryNotification.Functions;

public class EmailNotificationFunction(
    [FromKeyedServices(NotificationChannelType.Email)] INotificationChannelService notificationChannelService,
    ILogger<EmailNotificationFunction> logger
)
{
    [Function(nameof(EmailNotificationFunction))]
    public async Task RunAsync(
        [ServiceBusTrigger("notification-topic", "email", Connection = "ServiceBusConnection")]
            ServiceBusReceivedMessage message,
        CancellationToken cancellationToken
    )
    {
        logger.LogInformation(
            "EmailNotificationFunction triggered. MessageId: {MessageId}, Subject: {Subject}",
            message.MessageId,
            message.Subject
        );
        logger.LogError("Message body: {Body}", message.Body.ToString());

        try
        {
            var payload = JsonSerializer.Deserialize<NotificationPayload>(message.Body);

            if (payload == null)
            {
                logger.LogError(
                    "Invalid notification payload in email handler. Body: {Body}",
                    message.Body.ToString()
                );
                return;
            }

            logger.LogInformation(
                "Successfully deserialized payload for user: {Email}",
                payload.User?.Email
            );
            await notificationChannelService.HandleAsync(payload, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Error processing message. MessageId: {MessageId}, Body: {Body}",
                message.MessageId,
                message.Body.ToString()
            );
            throw;
        }
    }
}
