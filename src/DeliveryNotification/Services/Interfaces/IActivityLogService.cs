namespace DeliveryNotification.Services.Interfaces;

public interface IActivityLogService
{
    Task LogActivityAsync(ActivityLog log, CancellationToken cancellationToken);
}
