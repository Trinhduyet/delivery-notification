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
        var template = await _templateService.GetTemplateContentAsync(
            payload.NotificationId,
            "mobilepush",
            cancellationToken
        );

        if (template == null)
        {
            logger.LogError("Template not found for Id: {NotificationId}", payload.NotificationId);
            return;
        }

        var content = _templateService.MergeContent(template, payload.MergeTags);

        var pushNotificationPayload = new
        {
            deviceToken = payload.User.DeviceToken,
            title = content,
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
            logger.LogInformation("Successfully sent Mobile Push to {User}", payload.User.Email);
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
                NotificationId = payload.NotificationId,
                Status = "Success",
                Timestamp = DateTime.UtcNow,
            },
            cancellationToken
        );
    }
}
