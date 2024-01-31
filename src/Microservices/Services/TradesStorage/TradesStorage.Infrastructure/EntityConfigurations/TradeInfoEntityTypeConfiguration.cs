
namespace TradesStorage.Infrastructure.EntityConfigurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using TradesStorage.Infrastructure.Dto;

    public class TradeInfoEntityTypeConfiguration : IEntityTypeConfiguration<TradeInfo>
    {
        public void Configure(EntityTypeBuilder<TradeInfo> configuration)
        {
            configuration.ToTable("TradeInfo");

            configuration.HasKey(o => o.Guid);
            configuration.Property(ct => ct.Guid)
                .IsRequired();

            configuration.Property<string>("Exchange").IsRequired();
            configuration.Property<string>("Symbol").IsRequired();
            configuration.Property<long>("TimestampMin").IsRequired();
            configuration.Property<long>("TimestampMax").IsRequired();
        }
    }
}
