using DeliveryNotification.Infrastructure;

namespace DeliveryNotification.Services;

public class ActivityLogService(NotificationDbContext dbContext) : IActivityLogService
{
    private readonly NotificationDbContext _dbContext = dbContext;

    public async Task LogActivityAsync(ActivityLog log, CancellationToken cancellationToken)
    {
        await _dbContext.ActivityLogs.AddAsync(log, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}