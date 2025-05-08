using DeliveryNotification.Infrastructure;

namespace DeliveryNotification.Services;

public class NotificationTemplateService(NotificationDbContext dbContext)
    : INotificationTemplateService
{
    public async Task<(string? Title, string? Message)> GetTemplateContentAsync(
        string companyCode,
        NotificationChannelType channel,
        CancellationToken cancellationToken
    )
    {
        var notification = await dbContext.Notifications.FirstOrDefaultAsync(
            n => n.CompanyCode == companyCode && n.Channels.Contains(nameof(channel)),
            cancellationToken
        );

        if (notification is null)
        {
            return (null, null);
        }

        if (channel == NotificationChannelType.Email)
        {
            var emailTemplate = await dbContext.NotificationEmailTemplates.FirstOrDefaultAsync(
                et => et.NotificationId == notification.Id,
                cancellationToken
            );

            return (emailTemplate?.Subject, emailTemplate?.HtmlBody);
        }
        else
        {
            var pushTemplate = await dbContext.NotificationPushTemplates.FirstOrDefaultAsync(
                pt => pt.NotificationId == notification.Id,
                cancellationToken
            );

            return (pushTemplate?.Title, pushTemplate?.Message);
        }
    }

    public string MergeContent(string templateContent, Dictionary<string, string> mergeTags)
    {
        if (string.IsNullOrEmpty(templateContent) || mergeTags == null || mergeTags.Count == 0)
            return templateContent;

        foreach (var tag in mergeTags)
        {
            templateContent = templateContent.Replace(
                $"{{{{{tag.Key}}}}}",
                tag.Value ?? string.Empty
            );
        }

        return templateContent;
    }
}
