namespace DeliveryNotification.Services.Interfaces;

public interface INotificationTemplateService
{
    Task<string> GetTemplateContentAsync(
        string notificationId,
        string channel,
        CancellationToken cancellationToken
    );
    string MergeContent(string templateContent, Dictionary<string, string> mergeTags);
}
