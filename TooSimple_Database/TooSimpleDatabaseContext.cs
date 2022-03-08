
using Microsoft.EntityFrameworkCore;
using TooSimple_Database.Entities;

namespace TooSimple_Database
{
    public class TooSimpleDatabaseContext : DbContext
    {
        public TooSimpleDatabaseContext(DbContextOptions<TooSimpleDatabaseContext> options) :
               base(options)
        {

        }
        public DbSet<Account> Accounts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>()
                .HasKey(a => a.AccountId);
        }
    }
}