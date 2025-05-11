namespace DeliveryNotification.Services;

public abstract class BaseNotificationChannelService
{
    protected readonly IAsyncPolicy _retryPolicy;
    protected readonly ILogger _logger;
    protected readonly IActivityLogService _activityLogService;
    protected readonly INotificationTemplateService _templateService;

    protected BaseNotificationChannelService(
        INotificationTemplateService templateService,
        IActivityLogService activityLogService,
        ILogger logger
    )
    {
        _templateService = templateService;
        _activityLogService = activityLogService;
        _logger = logger;
        _retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(
                3,
                retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                (exception, timeSpan, retryCount, context) =>
                {
                    _logger.LogError(
                        exception,
                        "Retry {RetryCount} after {TimeSpan} due to {Message}",
                        retryCount,
                        timeSpan,
                        exception.Message
                    );
                }
            );
    }
}
