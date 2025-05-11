using System.Reflection;

namespace DeliveryNotification.Infrastructure;

public class NotificationDbContext(DbContextOptions<NotificationDbContext> options) : DbContext(options)
{
    public DbSet<ActivityLog> ActivityLogs { get; set; } = null!;
    public DbSet<Company> Companies { get; set; } = null!;
    public DbSet<Service> Services { get; set; } = null!;
    public DbSet<Channel> Channels { get; set; } = null!;
    public DbSet<Notification> Notifications { get; set; } = null!;
    public DbSet<NotificationEmailTemplate> NotificationEmailTemplates { get; set; } = null!;
    public DbSet<NotificationPushTemplate> NotificationPushTemplates { get; set; } = null!;
    public DbSet<ChannelSetting> ChannelSettings { get; set; } = null!;
    public DbSet<NotificationSetting> NotificationSettings { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
