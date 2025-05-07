namespace DeliveryNotification.Services;

public class InAppNotificationService(
    INotificationTemplateService templateService,
    IActivityLogService activityLogService,
    ILogger<InAppNotificationService> logger
)
    : BaseNotificationChannelService(templateService, activityLogService, logger),
        INotificationChannelService
{
    public async Task HandleAsync(NotificationRequest payload, CancellationToken cancellationToken)
    {
        var (_, body) = await _templateService.GetTemplateContentAsync(
            payload.CompanyCode,
           nameof(NotificationChannelType.InApp),
            cancellationToken
        );

        if (string.IsNullOrWhiteSpace(body))
        {
            logger.LogError("Template not found for company: {CompanyCode}", payload.CompanyCode);
            return;
        }
        var content = _templateService.MergeContent(body, payload.MergeTags);

        logger.LogInformation(
            "[MOCK] Sending InApp notification to user: {User} - Content: {Content}",
            payload.User.Email,
            content
        );

        await _activityLogService.LogActivityAsync(
            new ActivityLog
            {
                Channel = nameof(NotificationChannelType.InApp),
                CompanyCode = payload.CompanyCode,
                Status = "Success",
                Timestamp = DateTime.UtcNow,
            },
            cancellationToken
        );
    }
}
