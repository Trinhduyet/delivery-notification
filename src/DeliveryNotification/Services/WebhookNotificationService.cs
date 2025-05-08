namespace DeliveryNotification.Services;

public class WebhookNotificationService(
    INotificationTemplateService templateService,
    IActivityLogService activityLogService,
    ILogger<WebhookNotificationService> logger,
    HttpClient httpClient
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

        var content = _templateService.MergeContent(message, payload.MergeTags);

        if (string.IsNullOrEmpty(payload.User.WebhookUrl))
        {
            logger.LogError("WebhookUrl is null or empty");
            return;
        }

        var webhookPayload = new
        {
            user = payload.User.Name,
            message,
            content,
            timestamp = DateTime.UtcNow,
        };

        var response = await httpClient.PostAsJsonAsync(
            payload.User.WebhookUrl,
            webhookPayload,
            cancellationToken
        );
        response.EnsureSuccessStatusCode();

        if (!response.IsSuccessStatusCode)
        {
            var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
            logger.LogError(
                "Failed to send to webhook {Url}, Reason: {Reason}",
                payload.User.WebhookUrl,
                responseBody
            );
        }

        await _activityLogService.LogActivityAsync(
            new ActivityLog
            {
                UserId = payload.User.Id,
                Channel = nameof(NotificationChannelType.Webhook),
                CompanyCode = payload.CompanyCode,
                Status = response.IsSuccessStatusCode ? "Success" : "Failed",
                Title = title,
                Message = content
            },
            cancellationToken
        );
    }
}
