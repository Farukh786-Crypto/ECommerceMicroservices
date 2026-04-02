using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;


namespace Ordering.Infrastucture.Data
{
    public class OrderContextFactory : IDesignTimeDbContextFactory<OrderContext>
    {
        public OrderContext CreateDbContext(string[] args)
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            // user configuration here
            var connectionStrings = configuration.GetConnectionString("OrderingConnectionString");
            // configure DbContext with retry logic
            var optionBuilder = new DbContextOptionsBuilder<OrderContext>();
            optionBuilder.UseSqlServer(connectionStrings, sql =>
            {
                // THIS IS WHAT EF CLI USES
                sql.MigrationsAssembly("Ordering.Infrastructure");
            });
            return new OrderContext(optionBuilder.Options);
        }
    }
}
