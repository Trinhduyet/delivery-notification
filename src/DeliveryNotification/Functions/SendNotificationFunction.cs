using Newtonsoft.Json;

namespace DeliveryNotification.Functions;

public class SendNotificationFunction(
    ServiceBusClient client,
    ILogger<SendNotificationFunction> logger
)
{
    private readonly ServiceBusSender _sender = client.CreateSender("notification-topic");
    private readonly ILogger<SendNotificationFunction> _logger = logger;

    [Function(nameof(SendNotificationFunction))]
    public async Task<HttpResponseData> RunAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "send-notification")]
            HttpRequestData req,
        CancellationToken cancellationToken
    )
    {
        _logger.LogInformation("Received HTTP request to send notification.");

        var requestBody = await new StreamReader(req.Body).ReadToEndAsync(cancellationToken);
        var notification = JsonConvert.DeserializeObject<NotificationPayload>(requestBody);

        if (notification == null)
        {
            _logger.LogError("Notification payload is null or invalid.");
            var badResponse = req.CreateResponse(System.Net.HttpStatusCode.BadRequest);
            await badResponse.WriteStringAsync("Invalid payload.");
            return badResponse;
        }

        foreach (var channel in notification.User?.Channels ?? [])
        {
            var channelMessage = new ServiceBusMessage(JsonConvert.SerializeObject(notification))
            {
                Subject = channel,
            };

            try
            {
                await _sender.SendMessageAsync(channelMessage, cancellationToken);
                _logger.LogInformation(
                    "Notification forwarded to topic for channel: {Channel}",
                    channel
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send notification to channel {Channel}", channel);
                var errorResponse = req.CreateResponse(
                    System.Net.HttpStatusCode.InternalServerError
                );
                await errorResponse.WriteStringAsync("Failed to send notification.");
                return errorResponse;
            }
        }

        await _sender.DisposeAsync();
        await client.DisposeAsync();
        var response = req.CreateResponse(System.Net.HttpStatusCode.OK);
        await response.WriteStringAsync("Notification sent successfully.");
        return response;
    }
}
