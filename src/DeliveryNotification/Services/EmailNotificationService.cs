namespace DeliveryNotification.Services;

public class EmailNotificationService(
    INotificationTemplateService templateService,
    IActivityLogService logService,
    ILogger<EmailNotificationService> logger,
    ISendGridClient sendGridClient
) : BaseNotificationChannelService(templateService, logService, logger), INotificationChannelService
{
    public async Task HandleAsync(NotificationRequest payload, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(payload.User.Email))
        {
            logger.LogError("Email is null or empty");
            return;
        }

        var (subject, body) = await _templateService.GetTemplateContentAsync(
            payload.CompanyCode,
            NotificationChannelType.Email,
            cancellationToken
        );

        if (string.IsNullOrWhiteSpace(subject) || string.IsNullOrWhiteSpace(body))
        {
            logger.LogError("Template not found for company: {CompanyCode}", payload.CompanyCode);
            return;
        }

        var email = Environment.GetEnvironmentVariable("SendGrid_Email");
        var content = _templateService.MergeContent(body, payload.MergeTags);
        var message = new SendGridMessage();
        message.SetFrom(new EmailAddress(email, "Notification"));
        message.AddTo(payload.User.Email, payload.User.Name);
        message.SetSubject(body);
        message.AddContent("text/html", content);

        var response = await _retryPolicy.ExecuteAsync(async () =>
            await sendGridClient.SendEmailAsync(message, cancellationToken)
        );

        if (!response.IsSuccessStatusCode)
        {
            var responseBody = await response.Body.ReadAsStringAsync(cancellationToken);
            logger.LogError(
                "Failed to send SMS to {Email}, Reason: {Reason}",
                email,
                responseBody
            );
        }

        await _activityLogService.LogActivityAsync(
            new ActivityLog
            {
                UserId = payload.User.Id,
                Channel = nameof(NotificationChannelType.Email),
                CompanyCode = payload.CompanyCode,
                Status = response.IsSuccessStatusCode ? "Success" : "Failed",
                Title = subject,
                Message = content,
            },
            cancellationToken
        );
    }
}
