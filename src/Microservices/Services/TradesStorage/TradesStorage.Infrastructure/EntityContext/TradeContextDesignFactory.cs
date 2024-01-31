namespace TradesStorage.Infrastructure.EntityContext
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Design;

    public class TradeContextDesignFactory : IDesignTimeDbContextFactory<TradeContext>
    {
        public TradeContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<TradeContext>()
                .UseSqlite(args[0]);

            return new TradeContext(optionsBuilder.Options);
        }
    }
}
