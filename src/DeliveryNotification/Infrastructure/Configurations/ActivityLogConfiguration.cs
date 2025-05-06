namespace DeliveryNotification.Infrastructure.Configurations;

public class ActivityLogConfiguration : IEntityTypeConfiguration<ActivityLog>
{
    public void Configure(EntityTypeBuilder<ActivityLog> builder)
    {
        builder.ToTable("ActivityLogs");

        builder.HasKey(al => new { al.PartitionKey, al.RowKey });

        builder.Property(al => al.PartitionKey)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(al => al.RowKey)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(al => al.CompanyCode)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(al => al.Channel)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(al => al.Status)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(al => al.Message)
            .HasMaxLength(500);

        builder.Property(al => al.Timestamp)
            .IsRequired();

        builder.Ignore(al => al.ETag);
    }
}