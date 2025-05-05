namespace DeliveryNotification.Services;

public class NotificationTemplateService(ITableClientsProvider provider) : INotificationTemplateService
{
    private readonly TableClient _tableClient = provider.NotificationTemplates;

    public async Task<string> GetTemplateContentAsync(
        string notificationId,
        string channel,
        CancellationToken cancellationToken
    )
    {
        var response = await _tableClient.GetEntityAsync<NotificationTemplate>(
            partitionKey: channel,
            rowKey: notificationId,
            cancellationToken: cancellationToken
        );

        return response.Value.Body;
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
