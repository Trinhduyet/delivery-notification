namespace DeliveryNotification.Infrastructure.Configurations;

public class ChannelSettingsConfiguration : IEntityTypeConfiguration<ChannelSetting>
{
    public void Configure(EntityTypeBuilder<ChannelSetting> builder)
    {
        builder.ToTable("ChannelSettings");

        builder.HasKey(cs => cs.ChannelId);

        builder.Property(cs => cs.Settings).IsRequired();
    }
}
