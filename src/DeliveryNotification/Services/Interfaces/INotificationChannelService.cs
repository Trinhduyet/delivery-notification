namespace DeliveryNotification.Services.Interfaces;

public interface INotificationChannelService
{
    Task HandleAsync(NotificationRequest payload, CancellationToken cancellationToken);
}
