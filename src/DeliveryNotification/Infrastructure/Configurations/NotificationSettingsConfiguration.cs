namespace DeliveryNotification.Infrastructure.Configurations;

public class NotificationSettingsConfiguration : IEntityTypeConfiguration<NotificationSetting>
{
    public void Configure(EntityTypeBuilder<NotificationSetting> builder)
    {
        builder.ToTable("NotificationSettings");

        builder.HasKey(ns => ns.Id);

        builder.Property(ns => ns.Deduplication)
            .IsRequired();

        builder.Property(ns => ns.Throttling)
            .IsRequired();

        builder.Property(ns => ns.Retention)
            .IsRequired();
    }
}