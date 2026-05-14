using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Persistence
{
    public class DatabaseContextFactory : IDesignTimeDbContextFactory<DatabaseContext>
    {
        public DatabaseContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<DatabaseContext>();

            // Replace with your database provider and connection string
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=PostgresDatabase;Username=postgres;Password=12345;");

            return new DatabaseContext(optionsBuilder.Options);
        }

    }
}
