namespace DeliveryNotification.Infrastructure.Configurations;

public class NotificationEmailTemplateConfiguration : IEntityTypeConfiguration<NotificationEmailTemplate>
{
    public void Configure(EntityTypeBuilder<NotificationEmailTemplate> builder)
    {
        builder.ToTable(nameof(NotificationEmailTemplate));

        builder.HasKey(et => et.Id);

        builder.Property(et => et.NotificationId)
            .IsRequired();

        builder.Property(et => et.CultureCode)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(et => et.Subject)
            .IsRequired()
            .HasMaxLength(250);

        builder.Property(et => et.HtmlBody)
            .IsRequired();
    }
}