namespace DeliveryNotification.Services;

public class WebPushNotificationService(
    INotificationTemplateService templateService,
    IActivityLogService activityLogService,
    ILogger<WebPushNotificationService> logger
)
    : BaseNotificationChannelService(templateService, activityLogService, logger),
        INotificationChannelService
{
    public async Task HandleAsync(NotificationPayload payload, CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting WebPush notification for user: {User}", payload.User.Email);

        {
            var template = await _templateService.GetTemplateContentAsync(
                payload.NotificationId,
                "webpush",
                cancellationToken
            );

            var content = _templateService.MergeContent(template, payload.MergeTags);

            // TODO
            logger.LogInformation(
                "[MOCK] Sending WebPush notification to user: {User} - Content: {Content}",
                payload.User.Email,
                content
            );

            await _activityLogService.LogActivityAsync(
                new ActivityLog
                {
                    PartitionKey = payload.User.Name,
                    RowKey = Guid.NewGuid().ToString(),
                    Channel = NotificationChannelType.WebPush.ToString(),
                    NotificationId = payload.NotificationId,
                    Status = "Success",
                    Timestamp = DateTime.UtcNow,
                },
                cancellationToken
            );
        }
    }
}
