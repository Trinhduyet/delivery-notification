using Microsoft.Extensions.Configuration;

namespace DeliveryNotification.Services;

public class MobilePushNotificationService(
    ILogger<MobilePushNotificationService> logger,
    IHttpClientFactory httpClientFactory,
    IActivityLogService activityLogService,
    INotificationTemplateService templateService,
    IConfiguration configuration
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
        HttpResponseMessage response;

        if (payload.User.DeviceType?.Equals("iOS", StringComparison.OrdinalIgnoreCase) == true)
        {
            response = await SendApplePushNotificationAsync(
                payload,
                title,
                content,
                cancellationToken
            );
        }
        else
        {
            response = await SendFcmPushNotificationAsync(
                payload,
                title,
                content,
                cancellationToken
            );
        }

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            logger.LogError(
                "Failed to send Mobile Push {Name}, Error: {Error}",
                payload.User.Name,
                errorContent
            );
        }

        await _activityLogService.LogActivityAsync(
            new ActivityLog
            {
                UserId = payload.User.Id,
                Channel = nameof(NotificationChannelType.MobilePush),
                CompanyCode = payload.CompanyCode,
                Status = response.IsSuccessStatusCode ? "Success" : "Failed",
                Title = title,
                Message = content,
            },
            cancellationToken
        );
    }

    private async Task<HttpResponseMessage> SendFcmPushNotificationAsync(
        NotificationRequest payload,
        string title,
        string content,
        CancellationToken cancellationToken
    )
    {
        var fcmServerKey = configuration["Fcm:ServerKey"];
        var fcmEndpoint = "https://fcm.googleapis.com/fcm/send";

        var message = new
        {
            to = payload.User.DeviceToken,
            notification = new { title, body = content },
            data = new { companyCode = payload.CompanyCode, type = "push" },
        };

        var client = httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "key",
            $"={fcmServerKey}"
        );

        return await client.PostAsJsonAsync(fcmEndpoint, message, cancellationToken);
    }

    private async Task<HttpResponseMessage> SendApplePushNotificationAsync(
        NotificationRequest payload,
        string title,
        string content,
        CancellationToken cancellationToken
    )
    {
        var apnsEndpoint = configuration["Apns:Endpoint"]; // e.g., https://api.sandbox.push.apple.com/3/device/{deviceToken}
        var jwtToken = configuration["Apns:JwtToken"]; // pre-generated JWT for Apple authentication

        var client = httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "bearer",
            jwtToken
        );
        client.DefaultRequestHeaders.TryAddWithoutValidation(
            "apns-topic",
            configuration["Apns:BundleId"]
        );

        var body = new
        {
            aps = new { alert = new { title, body = content }, sound = "default" },
            data = new { companyCode = payload.CompanyCode },
        };

        var endpoint = $"{apnsEndpoint}/{payload.User.DeviceToken}";
        return await client.PostAsJsonAsync(endpoint, body, cancellationToken);
    }
}
