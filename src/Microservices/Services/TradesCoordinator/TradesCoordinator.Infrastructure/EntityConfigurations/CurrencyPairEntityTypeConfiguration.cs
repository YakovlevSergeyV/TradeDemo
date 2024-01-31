
namespace TradesCoordinator.Infrastructure.EntityConfigurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using TradesCoordinator.Infrastructure.Dto;

    public class CurrencyPairEntityTypeConfiguration : IEntityTypeConfiguration<SymbolInfo>
    {
        public void Configure(EntityTypeBuilder<SymbolInfo> configuration)
        {
            configuration.ToTable("CurrencyPairs");

            configuration.HasKey(o => o.Guid);
            configuration.Property(ct => ct.Guid)
                .IsRequired();

            configuration.Property<string>("ExchangeName").IsRequired();
            configuration.Property<string>("CurrencyPairName").IsRequired();
            configuration.Property<long>("TimestampInitial").IsRequired();
            configuration.Property<int>("LastCount").IsRequired();
            configuration.Property<int>("LastIntervalMin").IsRequired();
        }
    }
}
