namespace DeliveryNotification.Services.Interfaces;

public interface INotificationTemplateService
{
    Task<(string? Subject, string? Body)> GetTemplateContentAsync(
        string companyCode,
        string channel,
        CancellationToken cancellationToken
        );

    string MergeContent(string templateContent, Dictionary<string, string> mergeTags);
}
