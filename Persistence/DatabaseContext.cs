using Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Persistence.Configurations;

namespace Persistence
{
    public class DatabaseContext : DbContext
    {
        public DbSet<Account> Accounts { get; set; }

        public DatabaseContext(DbContextOptions<DatabaseContext> options)
            : base(options)
        {
            Database.Migrate();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Suppress pending model changes warning
            optionsBuilder.ConfigureWarnings(w =>
                w.Ignore(RelationalEventId.PendingModelChangesWarning));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new AccountConfiguration());
            base.OnModelCreating(modelBuilder);
        }
    }
}
