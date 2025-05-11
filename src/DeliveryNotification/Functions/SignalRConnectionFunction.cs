namespace DeliveryNotification.Functions;

public class SignalRConnectionFunction
{
    [FunctionName("negotiate")]
    public static SignalRConnectionInfo Negotiate(
        [HttpTrigger(AuthorizationLevel.Anonymous)] HttpRequest req,
        [SignalRConnectionInfo
        (HubName = "notificationHub")]
        SignalRConnectionInfo connectionInfo)
    {
        return connectionInfo;
    }
}
