using DeliveryNotification.Infrastructure;

namespace DeliveryNotification.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        var tableStorageConnectionString = Environment.GetEnvironmentVariable("ConnectionString");

        services.AddSingleton(_ => new TableServiceClient(tableStorageConnectionString));

        services.AddSingleton<ITableClientsProvider, TableClientsProvider>();

        var sendGridApiKey = Environment.GetEnvironmentVariable("SendGrid_API_Key");
        services.AddSingleton<ISendGridClient>(sp => new SendGridClient(sendGridApiKey));

        services.AddSingleton(provider =>
        {
            var connectionString = Environment.GetEnvironmentVariable("ServiceBusConnection");
            return new ServiceBusClient(connectionString);
        });

        var connectionString = Environment.GetEnvironmentVariable("ConnectionString");

        services.AddDbContext<NotificationDbContext>(options =>
            options.UseSqlServer(connectionString)
        );

        var azureSignalRConnectionString = Environment.GetEnvironmentVariable(
            "AzureSignalRConnectionString"
        );
        services.AddSignalR().AddAzureSignalR(azureSignalRConnectionString);

        return services;
    }

    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Services
        services.AddSingleton<INotificationTemplateService, NotificationTemplateService>();
        services.AddSingleton<IActivityLogService, ActivityLogService>();

        // Channel Handlers
        services.AddKeyedScoped<INotificationChannelService, EmailNotificationService>(
            NotificationChannelType.Email
        );
        services.AddKeyedScoped<INotificationChannelService, SmsNotificationService>(
            NotificationChannelType.Sms
        );
        services.AddKeyedScoped<INotificationChannelService, MobilePushNotificationService>(
            NotificationChannelType.MobilePush
        );
        services.AddKeyedScoped<INotificationChannelService, WebPushNotificationService>(
            NotificationChannelType.WebPush
        );
        services.AddKeyedScoped<INotificationChannelService, InAppNotificationService>(
            NotificationChannelType.InApp
        );
        services.AddKeyedScoped<INotificationChannelService, WebhookNotificationService>(
            NotificationChannelType.Webhook
        );

        return services;
    }
}
