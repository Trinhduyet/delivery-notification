using DeliveryNotification.Constants;

namespace DeliveryNotification.Functions;

public class SendNotificationFunction(
    ServiceBusClient serviceBusClient,
    FunctionContext executionContext
)
{
    private readonly ILogger _logger = executionContext.GetLogger(nameof(SendNotificationFunction));

    [Function(nameof(SendNotificationFunction))]
    public async Task<HttpResponseData> RunAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "send-notification")]
            HttpRequestData req,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Received HTTP request to send notification.");

        var requestBody = await new StreamReader(req.Body).ReadToEndAsync(cancellationToken);
        var notification = JsonSerializer.Deserialize<NotificationPayload>(requestBody);

        if (notification == null)
        {
            _logger.LogError("Notification payload is null or invalid.");
            return await CreateResponseAsync(req, HttpStatusCode.BadRequest, "Invalid payload.");
        }

        try
        {
            await SendToChannelsAsync(notification, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send notification to one or more channels.");
            return await CreateResponseAsync(req, HttpStatusCode.InternalServerError, "Failed to send notification.");
        }

        return await CreateResponseAsync(req, HttpStatusCode.OK, "Notification sent successfully.");
    }

    private async Task SendToChannelsAsync(NotificationPayload notification, CancellationToken cancellationToken)
    {
        await using var sender = serviceBusClient.CreateSender(NotificationConstants.TopicName);

        foreach (var channel in notification.User?.Channels ?? Enumerable.Empty<string>())
        {
            var message = new ServiceBusMessage(JsonSerializer.Serialize(notification))
            {
                Subject = channel,
            };

            await sender.SendMessageAsync(message, cancellationToken);
            _logger.LogInformation("Notification forwarded to topic for channel: {Channel}", channel);
        }
    }

    private static async Task<HttpResponseData> CreateResponseAsync(HttpRequestData req, HttpStatusCode statusCode, string message)
    {
        var response = req.CreateResponse(statusCode);
        await response.WriteStringAsync(message);
        return response;
    }
}
