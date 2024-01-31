namespace TradesCoordinator.Infrastructure.EntityContext
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Design;

    public class CoordinatorContextDesignFactory : IDesignTimeDbContextFactory<CoordinatorContext>
    {
        public CoordinatorContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<CoordinatorContext>()
                .UseSqlite(args[0]);

            return new CoordinatorContext(optionsBuilder.Options);
        }
    }
}
