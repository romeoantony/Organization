using Microsoft.EntityFrameworkCore;

namespace Organization.Models
{
    public class OrganizationDbContext : DbContext
    {
        public DbSet<Employee> Employees { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=organization.db");
        }
    }
}
