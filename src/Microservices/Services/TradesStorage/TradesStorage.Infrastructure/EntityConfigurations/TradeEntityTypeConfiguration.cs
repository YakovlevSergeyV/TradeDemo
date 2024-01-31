namespace TradesStorage.Infrastructure.EntityConfigurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using TradesStorage.Infrastructure.Dto;

    public class TradeEntityTypeConfiguration : IEntityTypeConfiguration<Trade>
    {
        public void Configure(EntityTypeBuilder<Trade> configuration)
        {
            configuration.ToTable("Trade");

            configuration.HasKey(cr => cr.Id);
            configuration.Property(ct => ct.Id)
                .ValueGeneratedNever()
                .IsRequired();

            configuration.Property<long>("Timestamp").IsRequired();
            configuration.Property<double>("Price").IsRequired();
            configuration.Property<double>("Amount").IsRequired();
        }
    }
}
