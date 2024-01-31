namespace TradesCoordinator.Infrastructure.EntityContext
{
    using Microsoft.EntityFrameworkCore;
    using TradesCoordinator.Infrastructure.Dto;
    using TradesCoordinator.Infrastructure.EntityConfigurations;

    public class CoordinatorContext : DbContext
    {
        public CoordinatorContext(DbContextOptions<CoordinatorContext> options) : base(options)
        {
        }

        public DbSet<SymbolInfo> CurrencyPairs { get; set; }
        public DbSet<ExchangeInfo> Exchanges { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new CurrencyPairEntityTypeConfiguration());
            builder.ApplyConfiguration(new ExchangeEntityTypeConfiguration());
        }
    }
}
