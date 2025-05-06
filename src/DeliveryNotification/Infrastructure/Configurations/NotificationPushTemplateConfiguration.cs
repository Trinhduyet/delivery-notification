namespace DeliveryNotification.Infrastructure.Configurations;

public class NotificationPushTemplateConfiguration : IEntityTypeConfiguration<NotificationPushTemplate>
{
    public void Configure(EntityTypeBuilder<NotificationPushTemplate> builder)
    {
        builder.ToTable(nameof(NotificationPushTemplate));

        builder.HasKey(pt => pt.Id);

        builder.Property(pt => pt.NotificationId)
            .IsRequired();

        builder.Property(pt => pt.CultureCode)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(pt => pt.Title)
            .IsRequired()
            .HasMaxLength(250);

        builder.Property(pt => pt.Message)
            .IsRequired();
    }
}