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
        var (title, message) = await _templateService.GetTemplateContentAsync(
            payload.CompanyCode,
            NotificationChannelType.InApp,
            cancellationToken
        );

        if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(message))
        {
            logger.LogError("Template not found for company: {CompanyCode}", payload.CompanyCode);
            return;
        }
        var content = _templateService.MergeContent(message, payload.MergeTags);

        await _activityLogService.LogActivityAsync(
            new ActivityLog
            {
                UserId = payload.User.Id,
                Channel = nameof(NotificationChannelType.InApp),
                CompanyCode = payload.CompanyCode,
                Status = "Success",
                Title = title,
                Message = content,
            },
            cancellationToken
        );
    }
}
