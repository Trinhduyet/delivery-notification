namespace DeliveryNotification.Services.Interfaces;

public interface INotificationTemplateService
{
    Task<(string? Title, string? Message)> GetTemplateContentAsync(
        string companyCode,
        NotificationChannelType channel,
        CancellationToken cancellationToken
    );

    string MergeContent(string templateContent, Dictionary<string, string> mergeTags);
}
