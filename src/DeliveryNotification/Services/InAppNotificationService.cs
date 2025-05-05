namespace DeliveryNotification.Services;

public class InAppNotificationService(
    INotificationTemplateService templateService,
    IActivityLogService activityLogService,
    ILogger<InAppNotificationService> logger
)
    : BaseNotificationChannelService(templateService, activityLogService, logger),
        INotificationChannelService
{
    public async Task HandleAsync(NotificationPayload payload, CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting InApp notification for user: {User}", payload.User.Email);
        var template = await _templateService.GetTemplateContentAsync(
            payload.NotificationId,
            "inapp",
            cancellationToken
        );

        var content = _templateService.MergeContent(template, payload.MergeTags);

        logger.LogInformation(
            "[MOCK] Sending InApp notification to user: {User} - Content: {Content}",
            payload.User.Email,
            content
        );

        await _activityLogService.LogActivityAsync(
            new ActivityLog
            {
                PartitionKey = payload.User.Name,
                RowKey = Guid.NewGuid().ToString(),
                Channel = NotificationChannelType.InApp.ToString(),
                NotificationId = payload.NotificationId,
                Status = "Success",
                Timestamp = DateTime.UtcNow,
            },
            cancellationToken
        );
    }
}
