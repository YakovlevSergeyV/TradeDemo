namespace TradesStorage.Infrastructure.EntityContext
{
    using Microsoft.Data.Sqlite;
    using Microsoft.EntityFrameworkCore;
    using TradesStorage.Infrastructure.Dto;
    using TradesStorage.Infrastructure.EntityConfigurations;

    public class TradeContext : DbContext
    {
        public TradeContext(DbContextOptions<TradeContext> options) : base(options)
        {
        }

        public DbSet<Trade> Trades { get; set; }
        public DbSet<TradeInfo> TradeInfos { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new TradeEntityTypeConfiguration());
            builder.ApplyConfiguration(new TradeInfoEntityTypeConfiguration());
        }

        public void CloseDb()
        {
            SqliteConnection.ClearAllPools();
        }
    }
}
