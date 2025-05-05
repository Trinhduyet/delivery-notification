using System.Net.Http.Json;

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
    public async Task HandleAsync(NotificationPayload payload, CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting Webhook notification for user: {User}", payload.User.Email);

        var template = await _templateService.GetTemplateContentAsync(
            payload.NotificationId,
            "webhook",
            cancellationToken
        );

        var content = _templateService.MergeContent(template, payload.MergeTags);

        if (string.IsNullOrEmpty(payload.User.WebhookUrl))
        {
            throw new InvalidOperationException("Webhook URL is missing for user.");
        }

        var webhookPayload = new
        {
            email = payload.User.Email,
            content,
            timestamp = DateTime.UtcNow,
        };

        // Replace the ambiguous line with the following:
        var response = await httpClient.PostAsJsonAsync(
            payload.User.WebhookUrl,
            webhookPayload,
            cancellationToken
        );
        response.EnsureSuccessStatusCode();

        await _activityLogService.LogActivityAsync(
            new ActivityLog
            {
                PartitionKey = payload.User.Name,
                RowKey = Guid.NewGuid().ToString(),
                Channel = NotificationChannelType.Webhook.ToString(),
                NotificationId = payload.NotificationId,
                Status = "Success",
                Timestamp = DateTime.UtcNow,
            },
            cancellationToken
        );
    }
}
