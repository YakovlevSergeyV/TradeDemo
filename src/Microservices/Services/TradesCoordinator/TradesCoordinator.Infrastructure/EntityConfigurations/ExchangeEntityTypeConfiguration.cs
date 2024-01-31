
namespace TradesCoordinator.Infrastructure.EntityConfigurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using TradesCoordinator.Infrastructure.Dto;

    class ExchangeEntityTypeConfiguration : IEntityTypeConfiguration<ExchangeInfo>
    {
        public void Configure(EntityTypeBuilder<ExchangeInfo> configuration)
        {
            configuration.ToTable("Exchanges");

            configuration.HasKey(cr => cr.Guid);
            configuration.Property(ct => ct.Guid)
                .IsRequired();

            configuration.Property<string>("ExchangeName").IsRequired();
            configuration.Property<int>("HeartBeatCycleInMs").IsRequired();
        }
    }
}
