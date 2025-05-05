namespace DeliveryNotification.Services;

public class EmailNotificationService(
    INotificationTemplateService templateService,
    IActivityLogService logService,
    ILogger<EmailNotificationService> logger,
    ISendGridClient sendGridClient
) : BaseNotificationChannelService(templateService, logService, logger), INotificationChannelService
{
    public async Task HandleAsync(NotificationPayload payload, CancellationToken cancellationToken)
    {
        var template = await _templateService.GetTemplateContentAsync(
            payload.NotificationId,
            "email",
            cancellationToken
        );

        var content = _templateService.MergeContent(template, payload.MergeTags);
        var message = new SendGridMessage();
        message.SetFrom(new EmailAddress("trinhngo.12196@gmail.com", "Notification"));
        message.AddTo(payload.User.Email, payload.User.Name);
        message.SetSubject(template);
        message.AddContent("text/html", content);

        var result = await _retryPolicy.ExecuteAsync(async () =>
            await sendGridClient.SendEmailAsync(message, cancellationToken)
        );

        await _activityLogService.LogActivityAsync(
            new ActivityLog
            {
                PartitionKey = payload.User.Name,
                RowKey = Guid.NewGuid().ToString(),
                Channel = NotificationChannelType.Email.ToString(),
                NotificationId = payload.NotificationId,
                Status = result.IsSuccessStatusCode ? "Success" : "Failed",
                Timestamp = DateTime.UtcNow,
            },
            cancellationToken
        );

        if (!result.IsSuccessStatusCode)
        {
            _logger.LogError("Failed to send email: {StatusCode}", result.StatusCode);
        }
    }
}
