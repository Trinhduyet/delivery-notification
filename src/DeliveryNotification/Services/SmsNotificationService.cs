namespace DeliveryNotification.Services;

public class SmsNotificationService(
    ILogger<SmsNotificationService> logger,
    INotificationTemplateService templateService,
    IActivityLogService activityLogService,
    HttpClient httpClient
)
    : BaseNotificationChannelService(templateService, activityLogService, logger),
        INotificationChannelService
{
    public async Task HandleAsync(
        NotificationPayload payload,
        CancellationToken cancellationToken = default
    )
    {

        var toPhoneNumber = payload.User.PhoneNumber;
        if (string.IsNullOrWhiteSpace(toPhoneNumber))
        {
            logger.LogError("Phone number is null or empty");
            return;
        }

        var (_, body) = await _templateService.GetTemplateContentAsync(
            payload.CompanyCode,
           nameof(NotificationChannelType.MobilePush),
            cancellationToken
        );

        if (string.IsNullOrWhiteSpace(body))
        {
            logger.LogError("Template not found for company: {CompanyCode}", payload.CompanyCode);
            return;
        }

        var content = _templateService.MergeContent(body, payload.MergeTags);

        var accountSid = Environment.GetEnvironmentVariable("Twilio_AccountSid");
        var authToken = Environment.GetEnvironmentVariable("Twilio_AuthToken");
        var fromPhoneNumber = Environment.GetEnvironmentVariable("Twilio_FromPhoneNumber") ?? "Euroland";
        var sendUrl = Environment.GetEnvironmentVariable("Twilio_SendUrl");


        var request = new HttpRequestMessage(
            HttpMethod.Post,
            sendUrl?.Replace(
                "{AccountSid}",
                accountSid
            )
        );

        var bodySms = new FormUrlEncodedContent(
            [
                new KeyValuePair<string, string>("To", toPhoneNumber),
                new KeyValuePair<string, string>("From", fromPhoneNumber),
                new KeyValuePair<string, string>("Body", content),
            ]
        );
        request.Content = bodySms;

        var byteArray = Encoding.ASCII.GetBytes($"{accountSid}:{authToken}");
        request.Headers.Authorization = new AuthenticationHeaderValue(
            "Basic",
            Convert.ToBase64String(byteArray)
        );
        var response = await httpClient.SendAsync(request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
            logger.LogError(
                "Failed to send SMS to {PhoneNumber}, Reason: {Reason}",
                toPhoneNumber,
                responseBody
            );
        }

        await _activityLogService.LogActivityAsync(
            new ActivityLog
            {
                PartitionKey = payload.User.Name,
                RowKey = Guid.NewGuid().ToString(),
                Channel = NotificationChannelType.Sms.ToString(),
                CompanyCode = payload.CompanyCode,
                Status = response.IsSuccessStatusCode ? "Success" : "Failed",
                Timestamp = DateTime.UtcNow,
            },
            cancellationToken
        );
    }
}
