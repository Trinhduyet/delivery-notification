namespace DeliveryNotification.Infrastructure.Configurations;

public class ActivityLogConfiguration : IEntityTypeConfiguration<ActivityLog>
{
    public void Configure(EntityTypeBuilder<ActivityLog> builder)
    {
        builder.ToTable("ActivityLogs");

        builder.HasKey(c => c.Id);

        builder.Property(al => al.UserId).IsRequired().HasMaxLength(250);

        builder.Property(al => al.CompanyCode).IsRequired().HasMaxLength(50);

        builder.Property(al => al.Channel).IsRequired().HasMaxLength(50);

        builder.Property(al => al.Status).IsRequired().HasMaxLength(50);

        builder.Property(al => al.Title).HasMaxLength(500);

        builder.Property(al => al.CreatedAt).IsRequired();
    }
}
