namespace DeliveryNotification.Services.Interfaces;

public interface INotificationChannelService
{
    Task HandleAsync(NotificationPayload payload, CancellationToken cancellationToken);
}
