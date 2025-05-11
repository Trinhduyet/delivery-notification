namespace DeliveryNotification.Infrastructure.Configurations;

public class CompanyConfiguration : IEntityTypeConfiguration<Company>
{
    public void Configure(EntityTypeBuilder<Company> builder)
    {
        builder.ToTable(nameof(Company));

        builder.HasKey(c => c.Id);

        builder.Property(c => c.CompanyCode).IsRequired().HasMaxLength(10);

        builder.Property(c => c.CompanyName).IsRequired().HasMaxLength(50);
    }
}
