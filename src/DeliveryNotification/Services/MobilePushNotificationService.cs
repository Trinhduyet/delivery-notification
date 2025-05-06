using System.Net.Http.Json;

namespace DeliveryNotification.Services;

public class MobilePushNotificationService(
    ILogger<MobilePushNotificationService> logger,
    IHttpClientFactory httpClientFactory,
    IActivityLogService activityLogService,
    INotificationTemplateService templateService
)
    : BaseNotificationChannelService(templateService, activityLogService, logger),
        INotificationChannelService
{
    public async Task HandleAsync(NotificationPayload payload, CancellationToken cancellationToken)
    {
        var (subject, body) = await _templateService.GetTemplateContentAsync(
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

        var pushNotificationPayload = new
        {
            deviceToken = payload.User.DeviceToken,
            title = subject,
            body = content,
        };

        var client = httpClientFactory.CreateClient();
        var response = await client.PostAsJsonAsync(
            "https://your-push-service.com/send",
            pushNotificationPayload,
            cancellationToken: cancellationToken
        );

        if (response.IsSuccessStatusCode)
        {
            logger.LogInformation("Successfully sent Mobile Push to {User}", payload.User.Name);
        }
        else
        {
            logger.LogError(
                "Failed to send Mobile Push. StatusCode: {StatusCode}",
                response.StatusCode
            );
        }

        await _activityLogService.LogActivityAsync(
            new ActivityLog
            {
                PartitionKey = payload.User.Name,
                RowKey = Guid.NewGuid().ToString(),
                Channel = NotificationChannelType.MobilePush.ToString(),
                CompanyCode = payload.CompanyCode,
                Status = response.IsSuccessStatusCode ? "Success" : "Failed",
                Timestamp = DateTime.UtcNow,
            },
            cancellationToken
        );
    }
}
