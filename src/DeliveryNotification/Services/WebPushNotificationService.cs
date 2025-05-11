namespace DeliveryNotification.Services;

public class WebPushNotificationService(
    [SignalR(HubName = "notificationHub")] IAsyncCollector<SignalRMessage> signalRMessages,
    IActivityLogService activityLogService,
    INotificationTemplateService templateService,
    ILogger<WebPushNotificationService> logger
)
    : BaseNotificationChannelService(templateService, activityLogService, logger),
        INotificationChannelService
{
    public async Task HandleAsync(NotificationRequest payload, CancellationToken cancellationToken)
    {
        var (title, message) = await _templateService.GetTemplateContentAsync(
            payload.CompanyCode,
            NotificationChannelType.MobilePush,
            cancellationToken
        );

        if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(message))
        {
            logger.LogError("Template not found for company: {CompanyCode}", payload.CompanyCode);
            return;
        }

        var body = _templateService.MergeContent(message, payload.MergeTags);

        await signalRMessages.AddAsync(
            new SignalRMessage
            {
                UserId = payload.User.Id,
                Target = "ReceiveNotification",
                Arguments = [new { title, body }],
            },
            cancellationToken
        );

        await _activityLogService.LogActivityAsync(
            new ActivityLog
            {
                UserId = payload.User.Id,
                Channel = nameof(NotificationChannelType.WebPush),
                CompanyCode = payload.CompanyCode,
                Status = "Success",
                Title = title,
                Message = message,
            },
            cancellationToken
        );
    }
}
