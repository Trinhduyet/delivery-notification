namespace DeliveryNotification.Services;

public class SmsNotificationService(
    ILogger<SmsNotificationService> logger,
    INotificationTemplateService templateService,
    IActivityLogService activityLogService,
    IHttpClientFactory httpClientFactory
)
    : BaseNotificationChannelService(templateService, activityLogService, logger),
        INotificationChannelService
{
    public async Task HandleAsync(
        NotificationPayload payload,
        CancellationToken cancellationToken = default
    )
    {
        var template = await _templateService.GetTemplateContentAsync(
            payload.NotificationId,
            "sms",
            cancellationToken
        );

        if (template == null)
        {
            logger.LogError(
                "Template not found for NotificationTemplateId {NotificationTemplateId}",
                payload.NotificationId
            );
            return;
        }
        var content = _templateService.MergeContent(template, payload.MergeTags);

        var accountSid = Environment.GetEnvironmentVariable("Twilio__AccountSid");
        var authToken = Environment.GetEnvironmentVariable("Twilio__AuthToken");
        var fromPhoneNumber = Environment.GetEnvironmentVariable("Twilio__FromPhoneNumber");

        var toPhoneNumber = payload.User.PhoneNumber;
        var request = new HttpRequestMessage(
            HttpMethod.Post,
            $"https://api.twilio.com/2010-04-01/Accounts/{accountSid}/Messages.json"
        );

        var body = new FormUrlEncodedContent(
            [
                new KeyValuePair<string, string>("To", toPhoneNumber ?? "0000000000"),
                new KeyValuePair<string, string>("From", fromPhoneNumber ?? "Euroland"),
                new KeyValuePair<string, string>("Body", content),
            ]
        );
        request.Content = body;

        var byteArray = Encoding.ASCII.GetBytes($"{accountSid}:{authToken}");
        request.Headers.Authorization = new AuthenticationHeaderValue(
            "Basic",
            Convert.ToBase64String(byteArray)
        );
        var client = httpClientFactory.CreateClient();
        var response = await client.SendAsync(request);

        if (response.IsSuccessStatusCode)
        {
            logger.LogInformation("SMS sent successfully to {PhoneNumber}", toPhoneNumber);
        }
        else
        {
            logger.LogError(
                "Failed to send SMS. Status: {StatusCode}, Reason: {Reason}",
                response.StatusCode,
                await response.Content.ReadAsStringAsync(cancellationToken)
            );
        }

        await _activityLogService.LogActivityAsync(
            new ActivityLog
            {
                PartitionKey = payload.User.Name,
                RowKey = Guid.NewGuid().ToString(),
                Channel = NotificationChannelType.Sms.ToString(),
                NotificationId = payload.NotificationId,
                Status = "Success",
                Timestamp = DateTime.UtcNow,
            },
            cancellationToken
        );
    }
}
