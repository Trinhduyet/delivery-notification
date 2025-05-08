namespace DeliveryNotification.Infrastructure.Configurations;

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.ToTable(nameof(Notification));

        builder.HasKey(n => n.Id);

        builder.Property(n => n.CompanyCode).IsRequired().HasMaxLength(10);

        builder.Property(n => n.ServiceCode).IsRequired().HasMaxLength(50);

        builder.Property(n => n.Channels).IsRequired();

        builder.Property(n => n.AlertTypeCode).IsRequired().HasMaxLength(50);
    }
}
