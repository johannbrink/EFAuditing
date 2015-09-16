using EFAuditing.TestHarness.Model;
using Microsoft.Data.Entity;

namespace EFAuditing.TestHarness
{
    public class MyDBContext : AuditingDbContext
    {
        public DbSet<Customer> Customers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(
                "Server=(localdb)\\mssqllocaldb;Database=EFAuditingTestHarness;Trusted_Connection=True;MultipleActiveResultSets=true");
        }
    }
}
